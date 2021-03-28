
using System;
using System.Collections.Generic;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using Lidgren.Network;
using XMLData;


namespace _2DLogicGame.ServerSide.Levels_ServerSide
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
        /// Atribut reprezentujuci Mapu Levelu - Typ LevelMap
        /// </summary>
        private LevelMap aLevelMap;


        /// <summary>
        /// Boolean, ktory reprezentuje, ci je Level Nacitany alebo nie
        /// </summary>
        private bool aIsLevelInitalized = false;

        /// <summary>
        /// Atribut reprezentuje nazov levelu
        /// </summary>
        private string aLevelName;

        public string LevelName
        {
            get => aLevelName;
            set => aLevelName = value;
        }
        public LevelMap LevelMap
        {
            get => aLevelMap;
            set => aLevelMap = value;
        }

        public bool IsLevelInitalized
        {
            get => aIsLevelInitalized;
            set => aIsLevelInitalized = value;
        }




        public LevelManager(LogicGame parLogicGame)
        {
            aLevelBlockData = new List<BlockData>();
            aLevelMap = new LevelMap();
            aLogicGame = parLogicGame;
        }

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

        public void InitLevel(string parLevelXmlPath)
        {
            LoadBlockXmlData(parLevelXmlPath: parLevelXmlPath); //Nacitame do Listu data o blokoch


            aLevelMap.InitMap(aLevelBlockData, aLevelName); //Inicializujeme si Data o Blokoch

            aIsLevelInitalized = true;
        }

        /// <summary>
        /// Metoda, ktora nacita level, pomocou zadamneho cisla, levelu, ak existuje nacita sa, pokial nie nenacita sa
        /// </summary>
        /// <param name="parLevelNumber"></param>
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
        public Dictionary<Vector2, BlockServer> GetBlocksPosDictionary()
        {
            return aLevelMap.GetBlocksPositionDictionary();
        }

        public BlockServer GetBlockByPosition(Vector2 parBlockPositionVector)
        {
            //Pouzijeme TryGet aby sme zistili, ci tam zadany kluc - pozicia je, ak ano vratime ho cez out
            BlockServer tmpBlock;
            aLevelMap.GetBlocksPositionDictionary().TryGetValue(parBlockPositionVector, out tmpBlock);
            return tmpBlock;


        }

        public bool IsUpdateNeeded()
        {
            switch (aLevelName)
            {
                case "Math":
                    return aLevelMap.GetMathProblemNaManager().UpdateIsReady;
                    break;
                default:
                    break;
            }

            return false;
        }

        public NetOutgoingMessage PrepareLevelDataForUpload(NetOutgoingMessage parOutgoingMessage)
        {
            switch (aLevelName)
            {
                case "Math": //Budeme vediet na isto ze ide o level typu Math
                    parOutgoingMessage.WriteVariableInt32(aLevelMap.GetMathProblemNaManager().GetFinalNumberFromInput());
                    aLevelMap.GetMathProblemNaManager().UpdateIsReady = false;
                    break;
                default:
                    return null;
                    break;
            }

            return parOutgoingMessage;
        }

    }

}
