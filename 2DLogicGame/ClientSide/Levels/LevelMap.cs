using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using _2DLogicGame.GraphicObjects;
using Microsoft.Xna.Framework;
using XMLData;
using Vector2 = Microsoft.Xna.Framework.Vector2;
// ReSharper disable InvalidXmlDocComment

namespace _2DLogicGame.ClientSide.Levels
{
    public class LevelMap
    {

        /// <summary>
        /// Dictionary - Key - Vector (Pozicia Bloku), Value - Objekt Block (Blok)
        /// K jednotlivym blokom budeme pristupovat casto cez suradnice, preto pouzijem Dictionary
        /// V XML Je pouzity list, preto.. Lebo sa musi postupne cez for cyklus prejst cely List
        /// Pri Dictionary bude pristupovat k presnym suradniciam...
        /// </summary>
        private Dictionary<Vector2, Block> aBlockPositionDictionary;

        /// <summary>
        /// Atribut List - Reprezentujuci List Blokov v Leveli - typ DrawableGameComponent
        /// Potrebujeme aj List aby sme mohli bloky pridat do naseho GameComponentu reprezentujuceho Hraciu Obrazovku
        /// </summary>
        private List<DrawableGameComponent> aBlockList;

        /// <summary>
        /// Atribut - LogicGame - Hra
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut reprezentujuci manazer matematickeho problemu - typ MathProblemManager
        /// </summary>
        private MathProblemManager aMathProblemManager;

        /// <summary>
        /// Atribut, Reprezentujuci Vysku a Sirku Blokov, napr 64 - Defaultne
        /// </summary>
        private int aDefaultBlockDimension;

        public int DefaultBlockDimension
        {
            get => aDefaultBlockDimension;
            set => aDefaultBlockDimension = value;
        }

        /// <summary>
        /// Konstruktor, reprezentujuci mapu levelu
        /// Inicializuje Hru, Defaultnu Velkost bloku, Dictionary pozicii blokov a List Blokov
        /// </summary>
        /// <param name="parLogicGame">Paramter reprezentujuci hru - typ LogicGame</param>
        /// <param name="parDefaultBlockDimension">Parameter reprezentujuci prednastavenu velkost bloku - typ Int (sirka a vyska je rovnaka)</param>
        public LevelMap(LogicGame parLogicGame, int parDefaultBlockDimension = 64)
        {
            aLogicGame = parLogicGame;
            aDefaultBlockDimension = parDefaultBlockDimension;
            aBlockPositionDictionary = new Dictionary<Vector2, Block>(31 * 16);
            aBlockList = new List<DrawableGameComponent>(31 * 16);
        }

        /// <summary>
        /// Metoda, ktora Inicializuje mapu pomocou zadaneho Listu dat o Blokoch - typ List<BlockData>
        /// </summary>
        /// <param name="parBlockPositions">Parameter reprezentujuci List Dat o blokoch - typ List<BlockData></param>
        /// <param name="parLevelName"></param>
        public void InitMap(List<BlockData> parBlockPositions, string parLevelName)
        {
            if (parLevelName == "Math") //Ak ide o level typu Math, potrebujeme vytvorit manazera Matematickeho Problemu
            {
                aMathProblemManager = new MathProblemManager(aLogicGame); 
            }

            for (int i = 0; i < parBlockPositions.Count; i++)
            {
                if (parBlockPositions[i].BlockName == "waterBlock")
                {
                    //Preto som to musel spravit takto, lebo v "XMLData", som pouzil Systemovy Vector2
                    //Treba pripomenut, ze nase pozicie boli ulozene len v jednotkovych suradniciach tzn 0, 1, 2..
                    //Preto k nim treba pripocitat aj velkost blokov
                    //A samozrejme netreba zabudnut aj na Skalovanie - to si preberieme priamo od aLogicGame
                    //Klasicky pouzivam Vector z MonoGame
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension ), parBlockPositions[i].Position.Y * (aDefaultBlockDimension ));

                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    WaterBlock tmpWaterBlock = new WaterBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpWaterBlock);
                    aBlockList.Add(tmpWaterBlock);
                }
                else if (parBlockPositions[i].BlockName == "barrierBlock")
                {
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension ));

                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    BarrierBlock tmpBarrierBlock = new BarrierBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpBarrierBlock);
                    aBlockList.Add(tmpBarrierBlock);
                }
                else if (parBlockPositions[i].BlockName == "bridgeBlock")
                {
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension));
                    BridgeBlock tmpBridgeBlock = new BridgeBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpBridgeBlock);
                    aBlockList.Add(tmpBridgeBlock);

                    if (aMathProblemManager != null && parLevelName == "Math" && parBlockPositions[i].AdditionalData != null) //Ak vieme ze pojde o level typu Math, odosleme MathProblem manazeru, informacie o Bridge Blokoch
                    {
                        int tmpBridgeSubNumber = 0;
                        Int32.TryParse(parBlockPositions[i].AdditionalData, out tmpBridgeSubNumber);
                        aMathProblemManager.AddBridge(tmpBridgeBlock, tmpBridgeSubNumber);
                    }

                }
                else if (parBlockPositions[i].BlockName == "endBlock")
                {
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension));
                    EndBlock tmpEndBlock = new EndBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpEndBlock);
                    aBlockList.Add(tmpEndBlock);
                } else if (parBlockPositions[i].BlockName == "mathGenerateBlock")
                {
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension));
                    ButtonBlock tmpButtonBlock = new ButtonBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpButtonBlock);
                    aBlockList.Add(tmpButtonBlock);

                    if (aMathProblemManager != null && parLevelName == "Math") //Ak vieme ze pojde o level typu Math, odosleme MathProblem manazeru, informacie o Button Blokoch
                    {
                        aMathProblemManager.AddButton(tmpButtonBlock);
                    }
                }
                else
                {
                    //Blank
                }

            }

            if (aMathProblemManager != null) //Ak existuje Math Problem Manazer, odosleme informaciu o tom ze sa uz cely level nacital a pridame ho do GameComponentov, tu treba dat pozor aj na odobranie 
            {//z komponentov pri zmene levelu alebo ukonceni hry
                aMathProblemManager.CompletelyLoaded = true;
                aLogicGame.Components.Add(aMathProblemManager);
            }
        }

        /// <summary>
        /// Metoda, ktora vrati List Blokov v podobe DrawableGameComponentov
        /// </summary>
        /// <returns>List<DrawableGameComponent> - Block List</returns>
        public List<DrawableGameComponent> GetBLockList()
        {
            return aBlockList;
        }

        /// <summary>
        /// Metoda, ktora vrati Dictionary Pozicie Blokov
        /// </summary>
        /// <returns>Dictionary<Vector2, Block> - Pozicie Blokov</returns>
        public Dictionary<Vector2, Block> GetBlocksPositionDictionary()
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

    }
}
