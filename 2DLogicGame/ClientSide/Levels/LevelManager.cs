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
using Microsoft.Xna.Framework;
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

        /// <summary>
        /// Atribut, reprezentuje ci je poziadavka na zmenu levelu alebo nie - typu boolean 
        /// </summary>
        private bool aLevelChangeRequested;

        /// <summary>
        /// Atribut, ktory sluzi ako doplnujuca informacia Level Change Requestu o tom na aky level sa ma zmenit momentalny level
        /// </summary>
        private int aLevelNumberRequested;

        /// <summary>
        /// Atribut, ktory reprezentuje pomocny casovac, napr. vyuzivany pri vypocte casu
        /// </summary>
        private float aHelperTimer;

        /// <summary>
        /// Atribut, Dictionary, reprezentuje Suradnice hracov podla ich ID
        /// </summary>
        private Dictionary<int, Vector2> aPlayerDefaultPositionsDictionary;

        /// <summary>
        /// Dictionary obsahujúca Hráčov, ku ktorým sa bude pristupovať pomocou Remote Unique Identifikator - Typ PlayerServer
        /// Aktualizuje podla Triedy Client
        /// </summary>
        private Dictionary<long, ClientSide.PlayerClientData> aDictionaryPlayerData; //Zatial ani nevyuzite, mozno aj zmazat


        private LevelTransformScreen aLevelTransformScreen;

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
        public Dictionary<int, Vector2> PlayerDefaultPositions
        {
            get => aPlayerDefaultPositionsDictionary;
            set => aPlayerDefaultPositionsDictionary = value;
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
            aPlayerDefaultPositionsDictionary = new Dictionary<int, Vector2>(2);
            aLevelChangeRequested = false;
            aLevelNumberRequested = 0;
            aHelperTimer = 0F;
            aLevelTransformScreen = new LevelTransformScreen(parLogicGame);
            aPlayScreenComponentCollection.AddComponent(aLevelTransformScreen);
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

                if (tmpLevel.DefaultPlayerPositionList != null)
                {
                    for (int i = 0; i < tmpLevel.DefaultPlayerPositionList.Count; i++)
                    {
                        Vector2 tmpNewVector2 = new Vector2(tmpLevel.DefaultPlayerPositionList[i].PositionX * GetMapBlocksDimensionSize(), tmpLevel.DefaultPlayerPositionList[i].PositionY * GetMapBlocksDimensionSize());
                        aPlayerDefaultPositionsDictionary.Add(i + 1, tmpNewVector2);

                    }
                }

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

            aLogicGame.CameraX = -48 * aLogicGame.Scale;

            aLevelTransformScreen.CameraOffset = aLogicGame.CameraX;


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
                case 2:
                    InitLevel("Levels\\levelSwap");
                    break;
                default:
                    LevelName = "NONE";
                    break;

            }

        }


        /// <summary>
        /// Metoda, ktora ovlada zmenu Levelu
        /// </summary>
        /// <param name="parLevelNumber">Parameter, ktory reprezentuje menu levelu na zmenenie</param>
        public void ChangeLevel(int parLevelNumber)
        {
            aPlayScreenComponentCollection.RemoveComponents(aLevelMap.GetBLockList());
            aLevelMap.DestroyMap(aLevelName);
            aPlayerDefaultPositionsDictionary.Clear();
            InitLevelByNumber(parLevelNumber);
            //Samozrejme, ked uz doslo k zmene levu, oznamime ze uz nie je treba takato zmena
            aLevelNumberRequested = 0;
            aLevelChangeRequested = false;
        }

        /// <summary>
        /// Metoda, ktora nastavi LevelManageru, request na zmenu levelu - Preto, aby mohlo dojst aj k nejakym grafickym zmenam.. napr fade out - fade in...
        /// </summary>
        /// <param name="parLevelNumber">Parameter - cislo levelu, na ktory sa ma zmenit</param>
        public void SetRequestOfLevelChange(int parLevelNumber)
        {
            aLevelChangeRequested = true;
            aLevelNumberRequested = parLevelNumber;
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

        /// <summary>
        /// Metoda, ktora spravuje prichadzajuce data o leveli
        /// </summary>
        /// <param name="parMessage"></param>
        /// <param name="parAmIFirstPlayer"></param>
        public void HandleLevelData(NetIncomingMessage parMessage, bool parAmIFirstPlayer)
        {
            switch (aLevelName)
            {
                case "Math":
                    aLevelMap.GetMathProblemManager().SetNumberToInput(parMessage.ReadVariableInt32());
                    Feedback tmpFeedBack = (Feedback)parMessage.ReadByte();
                    switch (tmpFeedBack)
                    {
                        case Feedback.NotSubmitted:
                            break;
                        case Feedback.SubmitSucceeded:
                            int tmpIdOfButton = parMessage.ReadVariableInt32();
                            bool tmpShowButton = parAmIFirstPlayer;
                            if (tmpIdOfButton >= 0)
                            {
                                aLevelMap.GetMathProblemManager().ButtonSucceeded(tmpIdOfButton, tmpShowButton);
                                aLevelMap.GetMathProblemManager().AddPoints();
                            }
                            break;
                        case Feedback.SubmitFailed:
                            aLevelMap.GetMathProblemManager().ResetInputNumbers();
                            break;
                        case Feedback.AllSolved:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Pomocna metoda, ktora sa stara o aktualizacie dolezitych data
        /// </summary>
        public void Update(GameTime parGameTime)
        {
            if (aLevelChangeRequested && aLevelTransformScreen.NeedsFadeOut == false && aLevelTransformScreen.NeedsFadeIn)
            {
                aLevelTransformScreen.NeedsFadeIn = false;
                aLevelTransformScreen.NeedsFadeOut = true;
            }

            if (aLevelChangeRequested)
            {
                if (aLevelTransformScreen.NeedsFadeOut == false)
                {
                    ChangeLevel(aLevelNumberRequested);

                    aLevelTransformScreen.NeedsFadeIn = true;
                }
            }
        }

        /// <summary>
        /// Metoda, ktora sluzi an spracovanie dat o Inicializacii Leve
        /// </summary>
        /// <param name="parMessage"></param>
        /// <param name="parLevelName"></param>
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
                        this.aLevelMap.GetMathProblemManager().Equations.Add(i + 1, new MathEquation(tmpFirstNumber, tmpSecondNumber, tmpOperator));
                    }

                    break;
                default:
                    break;
            }
        }


    }
}
