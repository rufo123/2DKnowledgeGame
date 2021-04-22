
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using _2DLogicGame.ServerSide.Database;
using _2DLogicGame.ServerSide.LevelEnglish_Server;
using _2DLogicGame.ServerSide.LevelMath_Server;
using _2DLogicGame.ServerSide.Questions_ServeSide;
using Assimp;
using Lidgren.Network;
using XMLData;


namespace _2DLogicGame.ServerSide.Levels_ServerSide
{
    /// <summary>
    /// Trieda, reprezentujuca manazer urovni. - Server.
    /// </summary>
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

        /// <summary>
        /// Atribut, ktory reprezentuje ci doslo k zmene defaultnej pozicie, napr pri zmene levelu
        /// </summary>
        private bool aDefaultPosChanged;

        /// <summary>
        /// Atribut, Dictionary, reprezentuje Suradnice hracov podla ich ID
        /// </summary>
        private Dictionary<int, Vector2> aPlayerDefaultPositionsDictionary;

        /// <summary>
        /// Dictionary obsahujúca Hráčov, ku ktorým sa bude pristupovať pomocou ID Hraca - napr. 1,2 - Typ PlayerServer
        /// </summary>
        private Dictionary<int, ServerSide.PlayerServerData> aDictionaryPlayerDataWithKeyID;

        /// <summary>
        /// Atribut, ktory reprezentuje ci v priebehu levelu sa uz uspesne requestli vyherne informacie
        /// </summary>
        private bool aWinInfoRequested;

        /// <summary>
        /// Atribut, reprezentujuci maximalny pocet hracov - typ int.
        /// </summary>
        private int aMaxPlayers = 2;

        /// <summary>
        /// Atribut, reprezentujuci cislo aktualnej urovne - typ int.
        /// </summary>
        private int aCurrentLevelNumber;

        /// <summary>
        /// Atribut, reprezentujuci dosiahnuty pocet bodov za predosle urovne, nepocitajuc aktualnu uroven - typ int.
        /// </summary>
        private int aPointsFromPreviousLevels;

        /// <summary>
        /// Atribut, reprezentujuci dosiahnuty pocet bodov za aktualnu uroven, nepocitajuc predosle urovne - typ int.
        /// </summary>
        private int aPointsForThisLevel;

        /// <summary>
        /// Atribut, ktory reprezentuje casovac hracieho casu, zapne sa vtedy, ked sa jeden z hracov pohne.
        /// </summary>
        private int aSecondElapsedByPlaying;

        /// <summary>
        /// Atribut, ktory specifikuje cas, kedy sa zacala hrat hra - teda kedy sa hrac pohol - nastavuje Server.
        /// </summary>
        private int aTimeWhenPlayingStartedInSeconds;

        /// <summary>
        /// Atribut, ktory reprezentuje handler statistiky, ktory spolupracuje s databazou - vyuziva sa na odosielanie dat o dokonceni mapy
        /// </summary>
        private StatisticsHandler aPlayerStatisticsHandler;

        /// <summary>
        /// Atribut, ktory reprezentuje ci uz hra skoncila, indikator toho, ze ak este neboli odoslanie informacie do databazy - hraci ukoncili hru predcasne. Aby sa taketo data odoslali dodatocne pri ukonceni.
        /// </summary>
        private bool aGameFinished;

        /// <summary>
        /// Atribut, ktory reprezentuje cislo posledneho levelu - default 3
        /// </summary>
        private int aNumberOfFinalLevel;

        /// <summary>
        /// Atribut, ktory reprezentuje ci bola odoslava informacia pre klientov o tom, ze hra bola uspesne dokoncena.
        /// </summary>
        private bool aGameFinishedInfoSent;

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
        public Dictionary<int, Vector2> PlayerDefaultPositions
        {
            get => aPlayerDefaultPositionsDictionary;
            set => aPlayerDefaultPositionsDictionary = value;
        }

        public bool IsLevelInitalized
        {
            get => aIsLevelInitalized;
            set => aIsLevelInitalized = value;
        }
        public bool DefaultPosChanged
        {
            get => aDefaultPosChanged;
            set => aDefaultPosChanged = value;
        }
        public Dictionary<int, PlayerServerData> DictionaryPlayerDataWithKeyId
        {
            get => aDictionaryPlayerDataWithKeyID;
            set => aDictionaryPlayerDataWithKeyID = value;
        }
        public bool WinInfoRequested
        {
            get => aWinInfoRequested;
            set => aWinInfoRequested = value;
        }

        public int SecondsElapsedByPlaying
        {
            get => aSecondElapsedByPlaying;
            set => aSecondElapsedByPlaying = value;
        }

        public int TimeWhenPlayingStartedInSeconds
        {
            get => aTimeWhenPlayingStartedInSeconds;
            set => aTimeWhenPlayingStartedInSeconds = value;
        }

        public bool GameFinished
        {
            get => aGameFinished;
            set => aGameFinished = value;
        }

        public int PointsFromPreviousLevels
        {
            get => aPointsFromPreviousLevels;
            set => aPointsFromPreviousLevels = value;
        }

        public bool GameFinishedInfoSent
        {
            get => aGameFinishedInfoSent;
            set => aGameFinishedInfoSent = value;
        }

        public LevelMap LevelMap1
        {
            get => default;
            set
            {
            }
        }

        /// <summary>
        /// Konstruktor manazera urovni.
        /// </summary>
        /// <param name="parLogicGame">Parameter, reprezentujuci hru - typ LogicGame. Musel sa pouzit kvoli pouzitiu Content Loadera z kniznice MonoGame.</param>
        public LevelManager(LogicGame parLogicGame)
        {
            aPointsForThisLevel = 0;
            aPointsFromPreviousLevels = 0;
            aLevelBlockData = new List<BlockData>();
            aLevelMap = new LevelMap(parLogicGame);
            aLogicGame = parLogicGame;
            aPlayerDefaultPositionsDictionary = new Dictionary<int, Vector2>(aMaxPlayers); //2 Hraci Max
            aCurrentLevelNumber = 0;
            aDictionaryPlayerDataWithKeyID = new Dictionary<int, PlayerServerData>(aMaxPlayers);
            aSecondElapsedByPlaying = 0;
            aTimeWhenPlayingStartedInSeconds = 0;
            aGameFinished = false;
            aPlayerStatisticsHandler = new StatisticsHandler();
            aNumberOfFinalLevel = 3;
            aGameFinishedInfoSent = false;

        }

        /// <summary>
        /// Metoda, ktora sa stara o nacitanie dat o urovni z XML suboru.
        /// </summary>
        /// <param name="parLevelXmlPath">Parameter, reprezentujuci cestu k XML suboru obsahujuceho data o urovni - typ string.</param>
        /// <returns></returns>
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

                        PlayerServerData tmpOutData = null;
                        if (aDictionaryPlayerDataWithKeyID.TryGetValue(i + 1, out tmpOutData))
                        {
                            tmpOutData.ReSpawn(true);
                        }

                    }

                    aDefaultPosChanged = true;

                }

                return true;
            }

            return false;

        }

        /// <summary>
        /// Metoda, ktora sa stara o incializaciu urovne, podla zadanej cesty k XML suboru obsahujuceho data o urovni.
        /// </summary>
        /// <param name="parLevelXmlPath">Paramter, reprezentujuci cestu k XML suboru, obsahujuceho data o urovni - typ string.</param>
        public void InitLevel(string parLevelXmlPath)
        {
            LoadBlockXmlData(parLevelXmlPath: parLevelXmlPath); //Nacitame do Listu data o blokoch

            aLevelMap.InitMap(aLevelBlockData, aLevelName); //Inicializujeme si Data o Blokoch

            aIsLevelInitalized = true;

            aWinInfoRequested = false;
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
                    aCurrentLevelNumber = 1;
                    break;
                case 2:
                    InitLevel("Levels\\levelQuestions");
                    aCurrentLevelNumber = 2;
                    break;
                case 3:
                    InitLevel("Levels\\levelEnglish");
                    aCurrentLevelNumber = 3;
                    break;
                default:
                    LevelName = "NONE";
                    break;
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o zmenu urovne na dalsiu uroven.
        /// </summary>
        public void ChangeToNextLevel()
        {
            LevelMap.DestroyMap(aLevelName);
            aPlayerDefaultPositionsDictionary.Clear();
            InitLevelByNumber(++aCurrentLevelNumber);
            //Ak dojde k zmene urovne, pripocitame celkove body
            aPointsFromPreviousLevels += aPointsForThisLevel;
            aPointsForThisLevel = 0;

            if (aCurrentLevelNumber > aNumberOfFinalLevel) //Ak je cislo levelu vacsie ako finalny level, vieme ze hraci dokoncili hru
            {
                SetFinish(); //Vysledok pojde do databazy
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o reset urovne.
        /// </summary>
        public void ResetLevel()
        {
            LevelMap.DestroyMap(aLevelName);
            aPlayerDefaultPositionsDictionary.Clear();
            InitLevelByNumber(aCurrentLevelNumber);
        }

        /// <summary>
        /// Metoda, ktora nastavenie hry na dokoncenu.
        /// </summary>
        public void SetFinish()
        {
            if (aPlayerStatisticsHandler != null && aPlayerStatisticsHandler.IsConnected && aGameFinished == false)
            {
                aGameFinished = true; //Nastavime - hra skoncila na true

                if (aDictionaryPlayerDataWithKeyID != null && aDictionaryPlayerDataWithKeyID.Count == 2 && aSecondElapsedByPlaying > 0)
                {
                    string tmpPlayer1Name = aDictionaryPlayerDataWithKeyID[1].PlayerNickName;
                    string tmpPlayer2Name = aDictionaryPlayerDataWithKeyID[2].PlayerNickName;
                    int tmpPoints = aPointsForThisLevel + aPointsFromPreviousLevels;
                    int tmpFinishedInSecond = aSecondElapsedByPlaying;

                    aPlayerStatisticsHandler.UploadNewScore(tmpPlayer1Name, tmpPlayer2Name, tmpPoints, tmpFinishedInSecond);

                    aGameFinished = true;
                }
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

        /// <summary>
        /// Metoda, ktora vrati poziciu bloku podla zadaneho suradnice bloku.
        /// </summary>
        /// <param name="parBlockPositionVector">Parameter, reprezentujuci suradnicu bloku.</param>
        /// <returns></returns>
        public BlockServer GetBlockByPosition(Vector2 parBlockPositionVector)
        {
            //Pouzijeme TryGet aby sme zistili, ci tam zadany kluc - pozicia je, ak ano vratime ho cez out
            BlockServer tmpBlock;
            aLevelMap.GetBlocksPositionDictionary().TryGetValue(parBlockPositionVector, out tmpBlock);
            return tmpBlock;

        }


        /// <summary>
        /// Metoda, ktora sa stara o vratenie informacie o tom ci je potrebna nejaka aktualizacia.
        /// </summary>
        /// <returns>Vrati pravdivostnu hodnotu na zaklade potrebnosti aktualizacie - typ bool.</returns>
        public bool IsUpdateNeeded()
        {
            switch (aLevelName)
            {
                case "Math":
                    return aLevelMap.GetMathProblemManager().UpdateIsReady;
                case "Questions":
                    return aLevelMap.GetQuestionManager().UpdateIsReady;
                case "English":
                    return aLevelMap.GetEnglishManager().UpdateIsReady;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizaciu bodov.
        /// </summary>
        public void UpdatePoints()
        {
            switch (aLevelName)
            {
                case "Math":
                    aPointsForThisLevel = aLevelMap.GetMathProblemManager().MathPoints;
                    break;
                case "Questions":
                    aPointsForThisLevel = aLevelMap.GetQuestionManager().QuestionPoints;
                    break;
                case "English":
                    aPointsForThisLevel = aLevelMap.GetEnglishManager().EnglishPoints;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizaciu informacie o tom, ci server nepotrebuje reset.
        /// </summary>
        /// <returns>Vrati pravdivostnu hodnotu, na zaklade toho ci level nepotrebuje reset - typ bool.</returns>
        public bool LevelNeedsReset()
        {
            switch (aLevelName)
            {
                case "Math":
                    return false;
                case "Questions":
                    return aLevelMap.GetQuestionManager().NeedsReset;
                case "English":
                    return false;
                default:
                    break;
            }

            return false;

        }

        /// <summary>
        /// Metoda, ktora sa stara o kontrolu, ci nedoslo k dokonceniu urovne.
        /// </summary>
        /// <returns></returns>
        public bool WinCheck()
        {
            switch (aLevelName)
            {
                case "Math":
                    if (aLevelMap.GetMathProblemManager().ProblemFeedback == Feedback.AllSolved)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "Questions":
                    if (aLevelMap.GetQuestionManager().QuestionFeedback == QuestionFeedback.Complete)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "English":
                    if (aLevelMap.GetEnglishManager().VocabularyFeedback == VocabularyFeedback.AllCorrect)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }


        /// <summary>
        /// Metoda, ktora vrati prednastavenu poziciu hraca na zaklade zadaneho ID
        /// <param name="parPlayerID">Parameter, reprezentujuci ciselne id hraca - napr. 1, 2 a pod.</param>
        /// </summary>
        public Vector2 GetPositionForPlayerId(int parPlayerID)
        {
            if (aPlayerDefaultPositionsDictionary != null && aPlayerDefaultPositionsDictionary.Count > 0)
            {
                if (aPlayerDefaultPositionsDictionary.TryGetValue(parPlayerID, out Vector2 tmpDefaultPosVector2))
                {
                    return tmpDefaultPosVector2;
                }
                else
                {
                    return null;
                }

            }

            return null;
        }

        /// <summary>
        /// Metoda, ktora sa stara o pripravenie dat o urovni na odovzdanie.
        /// </summary>
        /// <param name="parOutgoingMessage">Parameter, reprezentujuci odchadzajucu spravu - typ NetOutgoingMessage - buffer.</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareLevelDataForUpload(NetOutgoingMessage parOutgoingMessage)
        {
            parOutgoingMessage.WriteVariableInt32(aPointsFromPreviousLevels + aPointsForThisLevel);
            Debug.WriteLine("Points " + aPointsFromPreviousLevels + aPointsForThisLevel);
            switch (aLevelName)
            {
                case "Math": //Budeme vediet na isto ze ide o level typu Math
                    parOutgoingMessage.WriteVariableInt32(aLevelMap.GetMathProblemManager().GetFinalNumberFromInput());
                    parOutgoingMessage.Write((byte)aLevelMap.GetMathProblemManager().ProblemFeedback);
                    parOutgoingMessage.WriteVariableInt32(aLevelMap.GetMathProblemManager().IdOfLastButtonSucceeded);
                    aLevelMap.GetMathProblemManager().UpdateIsReady = false;
                    aLevelMap.GetMathProblemManager().ProblemFeedback = Feedback.NotSubmitted;
                    break;
                case "Questions":
                    parOutgoingMessage = aLevelMap.GetQuestionManager().PrepareQuestionData(parOutgoingMessage);
                    aLevelMap.GetQuestionManager().UpdateIsReady = false;
                    break;
                case "English":
                    parOutgoingMessage = aLevelMap.GetEnglishManager().PrepareVocabularyData(parOutgoingMessage);
                    aLevelMap.GetEnglishManager().UpdateIsReady = false;
                    return parOutgoingMessage;
                default:
                    return parOutgoingMessage;
            }
            return parOutgoingMessage;
        }

        /// <summary>
        /// Metoda, ktora sa stara o pripravenie inicializacnych dat urovne na odovzdanie.
        /// </summary>
        /// <param name="parOutgoingMessage">Parameter, reprezentujuci odchadzajucu spravu - typ NetOutgoingMessage - buffer.</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareLevelInitDataForUpload(NetOutgoingMessage parOutgoingMessage)
        {
            parOutgoingMessage.WriteVariableInt32(aPointsFromPreviousLevels + aPointsForThisLevel);

            switch (aLevelName)
            {
                case "Math":
                    parOutgoingMessage.WriteVariableInt32(this.LevelMap.GetMathProblemManager().Equations.Count);

                    for (int i = 0; i < this.LevelMap.GetMathProblemManager().Equations.Count; i++)
                    {
                        parOutgoingMessage.WriteVariableInt32(this.LevelMap.GetMathProblemManager().Equations[i + 1].FirstNumber);
                        parOutgoingMessage.WriteVariableInt32(this.LevelMap.GetMathProblemManager().Equations[i + 1].SecondNumber);
                        parOutgoingMessage.Write((byte)this.LevelMap.GetMathProblemManager().Equations[i + 1].Operator);
                    }

                    return parOutgoingMessage;
                case "Questions":
                    aLevelMap.GetQuestionManager().QuestionFeedback = QuestionFeedback.Initialization;
                    parOutgoingMessage = aLevelMap.GetQuestionManager().PrepareQuestionData(parOutgoingMessage);
                    return parOutgoingMessage;
                case "English":
                    aLevelMap.GetEnglishManager().VocabularyFeedback = VocabularyFeedback.Init;
                    parOutgoingMessage = aLevelMap.GetEnglishManager().PrepareVocabularyData(parOutgoingMessage);
                    return parOutgoingMessage;
                default:
                    return parOutgoingMessage;
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o pripravenie spravy o zmene urovne.
        /// </summary>
        /// <param name="parOutgoingMessage">Parameter, reprezentujuci odchadzajucu spravu - typ NetOutgoingMessage - buffer.</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareLevelChangeMessage(NetOutgoingMessage parOutgoingMessage)
        {
            int tmpNewLevel = aCurrentLevelNumber + 1;
            parOutgoingMessage.WriteVariableInt32(tmpNewLevel); //Na aky level sa ma zmenit
            return parOutgoingMessage;
        }

        /// <summary>
        /// Metoda, ktora sa stara o pripravenie dat pri spojenych s dokoncenim hry (bodoch a casu).
        /// </summary>
        /// <param name="parOutgoingMessage">Parameter, reprezentujuci odchadzajucu spravu - typ NetOutgoingMessage - buffer.</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareFinishedMessageData(NetOutgoingMessage parOutgoingMessage)
        {
            parOutgoingMessage.WriteVariableInt32(aPointsForThisLevel + aPointsFromPreviousLevels);
            parOutgoingMessage.WriteVariableInt32(SecondsElapsedByPlaying);
            return parOutgoingMessage;
        }

        /// <summary>
        /// Metoda, ktora sa stara o pripravenie dat o zmene prednastavenej pozicie
        /// </summary>
        /// <param name="parOutgoingMessage">Parameter odchadzajucej spravy - typ NetOutGoingMessage - Buffer</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareDefaultPositionUpdate(NetOutgoingMessage parOutgoingMessage)
        {
            if (aPlayerDefaultPositionsDictionary != null)
            {
                for (int i = 1; i <= aPlayerDefaultPositionsDictionary.Count; i++)
                {
                    parOutgoingMessage.WriteVariableInt32(i);
                    parOutgoingMessage.Write(aPlayerDefaultPositionsDictionary[i].X);
                    parOutgoingMessage.Write(aPlayerDefaultPositionsDictionary[i].Y);
                }
            }

            return parOutgoingMessage;
        }

    }

}
