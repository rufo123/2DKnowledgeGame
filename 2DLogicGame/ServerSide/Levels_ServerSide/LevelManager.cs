

using System;
using System.Collections.Generic;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
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
                return true;
            }

            return false;

        }

        public void InitLevel(string parLevelXmlPath)
        {
            LoadBlockXmlData(parLevelXmlPath: parLevelXmlPath); //Nacitame do Listu data o blokoch

            aLevelMap.InitMap(aLevelBlockData); //Inicializujeme si Data o Blokoch

            aIsLevelInitalized = true;
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

    }

}
