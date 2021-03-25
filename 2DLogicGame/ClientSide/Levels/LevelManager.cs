using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using _2DLogicGame.GraphicObjects;
using Assimp;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
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

        public bool IsLevelInitalized
        {
            get => aIsLevelInitalized;
            set => aIsLevelInitalized = value;
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

    }
}
