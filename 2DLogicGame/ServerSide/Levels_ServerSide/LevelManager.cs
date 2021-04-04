﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using _2DLogicGame.ServerSide.LevelEnglish_Server;
using _2DLogicGame.ServerSide.LevelMath_Server;
using _2DLogicGame.ServerSide.Questions_ServeSide;
using Assimp;
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


        private int aMaxPlayers = 2;

        private int aCurrentLevelNumber;

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


        public LevelManager(LogicGame parLogicGame)
        {
            aLevelBlockData = new List<BlockData>();
            aLevelMap = new LevelMap(parLogicGame);
            aLogicGame = parLogicGame;
            aPlayerDefaultPositionsDictionary = new Dictionary<int, Vector2>(aMaxPlayers); //2 Hraci Max
            aCurrentLevelNumber = 0;
            aDictionaryPlayerDataWithKeyID = new Dictionary<int, PlayerServerData>(aMaxPlayers);
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

        public void ChangeToNextLevel()
        {
            LevelMap.DestroyMap(aLevelName);
            aPlayerDefaultPositionsDictionary.Clear();
            InitLevelByNumber(++aCurrentLevelNumber);
        }

        public void ResetLevel()
        {
            LevelMap.DestroyMap(aLevelName);
            aPlayerDefaultPositionsDictionary.Clear();
            InitLevelByNumber(aCurrentLevelNumber);
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
                    return aLevelMap.GetMathProblemManager().UpdateIsReady;
                case "Questions":
                    return aLevelMap.GetQuestionManager().UpdateIsReady;
                case "English":
                    return aLevelMap.GetEnglishManager().UpdateIsReady;
                    break;
                default:
                    break;
            }

            return false;
        }

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
                    return false;
                default:
                    return false;
            }
        }


        public NetOutgoingMessage PrepareLevelDataForUpload(NetOutgoingMessage parOutgoingMessage)
        {
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
                    
                    break;
                default:
                    return parOutgoingMessage;
            }
            return parOutgoingMessage;
        }

        public NetOutgoingMessage PrepareLevelInitDataForUpload(NetOutgoingMessage parOutgoingMessage)
        {
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

        public NetOutgoingMessage PrepareLevelChangeMessage(NetOutgoingMessage parOutgoingMessage)
        {
            int tmpNewLevel = aCurrentLevelNumber + 1;
            parOutgoingMessage.WriteVariableInt32(tmpNewLevel); //Na aky level sa ma zmenit
            return parOutgoingMessage;
        }

        /// <summary>
        /// Metoda, ktora sa stara o pripravenie dat o zmene prednastavenej pozicie
        /// </summary>
        /// <param name="parOutgoingMessage">Parameter spravy - typ NetOutGoingMessage - Buffer</param>
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
