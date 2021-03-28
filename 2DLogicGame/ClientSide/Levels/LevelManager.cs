using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using _2DLogicGame.ClientSide.MathProblem;
using _2DLogicGame.GraphicObjects;
using Assimp;
using Lidgren.Network;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using SharpFont;
using XMLData;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace _2DLogicGame.ClientSide.Levels
{

    public class LevelManager
    {
        /// <summary>
        /// Atribut List - typu BlockData (z XMLData) - Reprezentuje Suradnice a Mena Blokov pre nacitanie
        /// </summary>
        private List<BlockData> aLevelBlockData;

        /// <summary>
        /// Atribut Hry - Reprezentujuci Hru
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut ComponentCollection - Reprezentujuci Komponenty Hry pri hrani
        /// </summary>
        private ComponentCollection aPlayScreenComponentCollection;

        /// <summary>
        /// Atribut reprezentujuci Mapu Levelu - Typ LevelMap
        /// </summary>
        private LevelMap aLevelMap;

        /// <summary>
        /// Atribut - Boolean, ktory reprezentuje, ci je Level Nacitany alebo nie
        /// </summary>
        private bool aIsLevelInitalized = false;

        /// <summary>
        /// Atribut reprezentuje nazov levelu - typ string
        /// </summary>
        private string aLevelName;

        private bool aLevelUpdateIsReady = false;

        public bool IsLevelInitalized
        {
            get => aIsLevelInitalized;
            set => aIsLevelInitalized = value;
        }
        public bool LevelUpdateIsReady
        {
            get => aLevelUpdateIsReady;
            set => aLevelUpdateIsReady = value;
        }
        public string LevelName
        {
            get => aLevelName;
            set => aLevelName = value;
        }

        /// <summary>
        /// Konstruktor LevelManageru -
        /// </summary>
        /// <param name="parLogicGame">Parameter reprezentujuci hru - typu LogicGame</param>
        /// <param name="parPlayingScreenComponentCollection">Paramter reprezentujuci kolekciu komponentov na hracej obrazovke - typ ComponentCollection</param>
        public LevelManager(LogicGame parLogicGame, ComponentCollection parPlayingScreenComponentCollection)
        {
            aLogicGame = parLogicGame;
            aPlayScreenComponentCollection = parPlayingScreenComponentCollection;
            aLevelBlockData = new List<BlockData>();
            aLevelMap = new LevelMap(parLogicGame);
            aLevelUpdateIsReady = false;

        }

        /// <summary>
        /// Metoda, ktora nacitava level data z XML suboru
        /// </summary>
        /// <param name="parLevelXmlPath">Parameter reprezentujuci cestu k XML suboru - typ string</param>
        /// <returns>Vrati hodnotu true ak metoda prebehla uspesne, inak false</returns>
        public bool LoadBlockXmlData(string parLevelXmlPath)
        {

            XMLData.LevelMaker tmpLevel;

            try
            {
                tmpLevel = aLogicGame.Content.Load<XMLData.LevelMaker>(parLevelXmlPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            if (tmpLevel != null) //Ak sa nam uspesne podarilo nacitat XML subor s levelom
            {
                aLevelBlockData = tmpLevel.BlockDataList; //Pridatime Data o Blokoch do Listu
                aLevelName = tmpLevel.LevelName;
                return true;
            }

            return false;

        }

        /// <summary>
        /// Metoda, ktora inicializuje level na zaklade cesty k XML suboru
        /// </summary>
        /// <param name="parLevelXmlPath">Paramter reprezentujuci cestu k XML suboru - typ string</param>
        public void InitLevel(string parLevelXmlPath)
        {
            LoadBlockXmlData(parLevelXmlPath: parLevelXmlPath); //Nacitame do Listu data o blokoch

            aLevelMap.InitMap(aLevelBlockData, aLevelName); //Inicializujeme si Data o Blokoch

            aPlayScreenComponentCollection.AddComponents(aLevelMap.GetBLockList());

            aIsLevelInitalized = true;
        }

        /// <summary>
        /// Metoda, ktora nacita level, pomocou zadaneho cisla levelu, ak existuje nacita sa, pokial nie nenacita sa
        /// </summary>
        /// <param name="parLevelNumber">Parameter reprezentujuci cislo levelu - typ int</param>
        public void InitLevelByNumber(int parLevelNumber)
        {
            switch (parLevelNumber)
            {
                case 1:
                    InitLevel("Levels\\levelMath");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Metoda, ktora vrati velkost blokov v mape
        /// </summary>
        /// <returns>Int - Velkost Blokov v Hre</returns>
        public int GetMapBlocksDimensionSize()
        {
            return aLevelMap.DefaultBlockDimension;
        }

        /// <summary>
        /// Metoda, ktora vrati Dictionary - Pozicie Blokov v Hre
        /// </summary>
        /// <returns>Dictionary - Key Vector2, Value - Block - Block Position Dictionary</returns>
        public Dictionary<Vector2, Block> GetBlocksPosDictionary()
        {
            return aLevelMap.GetBlocksPositionDictionary();
        }

        /// <summary>
        /// Metoda, ktora vrati Block, podla zadaneho Vektora reprezentujuceho poziciu
        /// </summary>
        /// <param name="parBlockPositionVector">Paramter reprezentujuci poziciu bloku reprezentovanu vektorom - typ Vector2</param>
        /// <returns>Vrati Block podla zadaneho vectoru v parametri</returns>
        public Block GetBlockByPosition(Vector2 parBlockPositionVector)
        {
            //Pouzijeme TryGet aby sme zistili, ci tam zadany kluc - pozicia je, ak ano vratime ho cez out
            Block tmpBlock;
            aLevelMap.GetBlocksPositionDictionary().TryGetValue(parBlockPositionVector, out tmpBlock);
            return tmpBlock;
        }

        public bool CheckForUpdate()
        {
            bool tmpIsUpdateNeeded = false;

            switch (aLevelName)
            {
                case "Math":
                    tmpIsUpdateNeeded = aLevelMap.GetMathProblemNaManager().UpdateIsReady;
                    break;
                default:
                    break;
            }

            aLevelUpdateIsReady = tmpIsUpdateNeeded;

            return tmpIsUpdateNeeded;
        }

        public NetOutgoingMessage PrepareLevelDataToSend(NetOutgoingMessage parNetOutgoingMessage)
        {
            switch (aLevelName)
            {
                case "Math":
                    parNetOutgoingMessage.Write("NumberSum");
                    parNetOutgoingMessage.WriteVariableInt32(aLevelMap.GetMathProblemNaManager().GetFinalNumberFromInput());
                    break;
                default:
                    break;
            }
            return parNetOutgoingMessage;
        }

        public void HandleLevelData(NetIncomingMessage parMessage)
        {
            switch (aLevelName)
            {
                case "Math":
                    aLevelMap.GetMathProblemNaManager().SetNumberToInput(parMessage.ReadVariableInt32());
                    break;
                default:
                    break;
            }
        }

        public void HandleLevelInitData(NetIncomingMessage parMessage, string parLevelName)
        {
            switch (parLevelName)
            {
                case "Math":
                    int tmpCountOfEquations = parMessage.ReadVariableInt32();
                    for (int i = 0; i < tmpCountOfEquations; i++)
                    {
                        int tmpFirstNumber = parMessage.ReadVariableInt32();
                        int tmpSecondNumber = parMessage.ReadVariableInt32();
                        char tmpOperator = (char)(parMessage.ReadByte());

                        this.aLevelMap.GetMathProblemNaManager().Equations.Add(i+1, new MathEquation(tmpFirstNumber, tmpSecondNumber, tmpOperator));
                    }

                    break;
                default:
                    break;
            }
        }


    }
}
