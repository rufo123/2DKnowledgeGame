using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.GraphicObjects;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using _2DLogicGame.ServerSide.LevelMath_Server;
using _2DLogicGame.ServerSide.Questions_ServeSide;
using XMLData;

namespace _2DLogicGame.ServerSide.Levels_ServerSide
{
    public class LevelMap
    {
        /// <summary>
        /// Dictionary - Key - Vector (Pozicia Bloku), Value - Objekt Block (Blok)
        /// K jednotlivym blokom budeme pristupovat casto cez suradnice, preto pouzijem Dictionary
        /// V XML Je pouzity list, preto.. Lebo sa musi postupne cez for cyklus prejst cely List
        /// Pri Dictionary bude pristupovat k presnym suradniciam...
        /// </summary>
        private Dictionary<Vector2, BlockServer> aBlockPositionDictionary;

        /// <summary>
        /// Atribut List - Reprezentujuci List Blokov v Leveli - typ DrawableGameComponent
        /// Potrebujeme aj List aby sme mohli bloky pridat do naseho GameComponentu reprezentujuceho Hraciu Obrazovku
        /// </summary>
        private List<BlockServer> aBlockList;


        private MathProblemServerManager aMathProblemServerManager;

        private QuestionsManagerServer aQuestionsServerManager;

        /// <summary>
        /// Atribut, Reprezentujuci Vysku a Sirku Blokov, napr 64 - Defaultne
        /// </summary>
        private int aDefaultBlockDimension;

        private LogicGame aLogicGame;

        public int DefaultBlockDimension
        {
            get => aDefaultBlockDimension;
            set => aDefaultBlockDimension = value;
        }



        public LevelMap(LogicGame parLogicGame, int parDefaultBlockDimension = 64)
        {
            aDefaultBlockDimension = parDefaultBlockDimension;
            aBlockPositionDictionary = new Dictionary<Vector2, BlockServer>(31 * 16);
            aBlockList = new List<BlockServer>(31 * 16);
            aLogicGame = parLogicGame;
        }

        public void DestroyMap(string parOldLevelName)
        {

            switch (parOldLevelName)
            {
                case "Math":
                    aMathProblemServerManager = null;
                    break;
                case "Questions":
                    aQuestionsServerManager.Destroy();
                    aQuestionsServerManager = null;
                    break;
                default:
                    break;
            }

            aBlockPositionDictionary.Clear();
            aBlockList.Clear();

        }

        public void InitMap(List<BlockData> parBlockPositions, string parLevelName)
        {

            switch (parLevelName)
            {
                case "Math":
                    aMathProblemServerManager = new MathProblemServerManager();
                    break;
                case "Questions":
                    aQuestionsServerManager = new QuestionsManagerServer(aLogicGame);
                    break;
                default:
                    break;
            }


            for (int i = 0; i < parBlockPositions.Count; i++)
            {
                Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension));

                if (parBlockPositions[i].BlockName == "waterBlock")
                {
                    //Preto som to musel spravit takto, lebo v "XMLData", som pouzil Systemovy Vector2
                    //Treba pripomenut, ze nase pozicie boli ulozene len v jednotkovych suradniciach tzn 0, 1, 2..
                    //Preto k nim treba pripocitat aj velkost blokov
                    //A samozrejme netreba zabudnut aj na Skalovanie - to si preberieme priamo od aLogicGame
                    //Klasicky pouzivam Vector z MonoGame

                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    WaterBlockServer tmpWaterBlock = new WaterBlockServer(tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpWaterBlock);
                    aBlockList.Add(tmpWaterBlock);
                }
                else if (parBlockPositions[i].BlockName == "barrierBlock")
                {
                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    BarrierBlockServer tmpBarrierBlock = new BarrierBlockServer(tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpBarrierBlock);
                    aBlockList.Add(tmpBarrierBlock);
                }
                else if (parBlockPositions[i].BlockName == "bridgeBlock")
                {
                    BridgeBlockServer tmpBridgeBlock = new BridgeBlockServer(tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpBridgeBlock);
                    aBlockList.Add(tmpBridgeBlock);

                    if (aMathProblemServerManager != null && parLevelName == "Math" && parBlockPositions[i].AdditionalData != null) //Ak vieme ze pojde o level typu Math, odosleme MathProblem manazeru, informacie o Bridge Blokoch
                    {
                        int tmpBridgeSubNumber = 0;
                        Int32.TryParse(parBlockPositions[i].AdditionalData, out tmpBridgeSubNumber);
                        aMathProblemServerManager.AddBridge(tmpBridgeBlock, tmpBridgeSubNumber);
                    }
                }
                else if (parBlockPositions[i].BlockName == "endBlock")
                {
                    EndBlockServer tmpEndBlock = new EndBlockServer(tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpEndBlock);
                    aBlockList.Add(tmpEndBlock);

                    if (aMathProblemServerManager != null && parLevelName == "Math") //Ak vieme ze pojde o level typu Math, odosleme MathProblem manazeru, informacie o Button Blokoch
                    {
                        aMathProblemServerManager.AddEndBlock(tmpEndBlock);
                    }
                }
                else if (parBlockPositions[i].BlockName == "mathGenerateBlock")
                {
                    ButtonBlockServer tmpButtonBlock = new ButtonBlockServer(tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpButtonBlock);
                    aBlockList.Add(tmpButtonBlock);

                    if (aMathProblemServerManager != null && parLevelName == "Math") //Ak vieme ze pojde o level typu Math, odosleme MathProblem manazeru, informacie o Button Blokoch
                    {
                        aMathProblemServerManager.AddButton(tmpButtonBlock);
                    }
                    else if (aQuestionsServerManager != null && parLevelName == "Questions")
                    {
                        aQuestionsServerManager.AddButton(tmpButtonBlock);
                    }
                }
                else if (parBlockPositions[i].BlockName == "mathInputBlock")
                {
                    InputBlockServer tmpInputBlock = new InputBlockServer(tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpInputBlock);
                    aBlockList.Add(tmpInputBlock);

                    if (aMathProblemServerManager != null && parLevelName == "Math") //Ak vieme ze pojde o level typu Math, odosleme MathProblem manazeru, informacie o InputBlokoch
                    {
                        aMathProblemServerManager.AddInput(tmpInputBlock);
                    }
                    else if (aQuestionsServerManager != null && parLevelName == "Questions")
                    {
                        tmpInputBlock.MaxNumber = 5; //Pretoze, mame Otazky o A,B,C,D a 0 bude reprezentovat nic...
                        aQuestionsServerManager.AddInput(tmpInputBlock);
                    }
                }
                else if (parBlockPositions[i].BlockName == "doorBlock")
                {
                    DoorBlockServer tmpDoorBlock = new DoorBlockServer(tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpDoorBlock);
                    aBlockList.Add(tmpDoorBlock);

                    if (aQuestionsServerManager != null && parLevelName == "Questions") //Ak vieme ze pojde o level typu Math, odosleme MathProblem manazeru, informacie o InputBlokoch
                    {
                        aQuestionsServerManager.AddDoors(tmpDoorBlock);
                    }


                    //Blank
                }

            }
        }
        /// <summary>
        /// Metoda, ktora vrati List Blokov v podobe DrawableGameComponentov
        /// </summary>
        /// <returns>List<DrawableGameComponent> - Block List</returns>
        public List<BlockServer> GetBLockList()
        {
            return aBlockList;
        }

        /// <summary>
        /// Metoda, ktora vrati Dictionary Pozicie Blokov
        /// </summary>
        /// <returns>Dictionary<Vector2, Block> - Pozicie Blokov</returns>
        public Dictionary<Vector2, BlockServer> GetBlocksPositionDictionary()
        {
            if (aBlockPositionDictionary != null)
            {
                return aBlockPositionDictionary;
            }
            else
            {
                return null;
            }
        }

        public MathProblemServerManager GetMathProblemManager()
        {
            if (aMathProblemServerManager != null)
            {
                return aMathProblemServerManager;
            }

            return null;
        }

        public QuestionsManagerServer GetQuestionManager()
        {
            if (aQuestionsServerManager != null)
            {
                return aQuestionsServerManager;
            }

            return null;
        }
    }
}
