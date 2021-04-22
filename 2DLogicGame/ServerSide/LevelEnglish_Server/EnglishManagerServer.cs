using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using _2DLogicGame.ServerSide.Questions_ServeSide;
using Lidgren.Network;
using SharpFont;
using XMLData;

namespace _2DLogicGame.ServerSide.LevelEnglish_Server
{
    /// <summary>
    /// Enumeracna trieda, ktora reprezentuje spatnu vazbu manazera prekladu. - Server.
    /// </summary>
    public enum VocabularyFeedback
    {
        None = 0,
        Init = 1,
        AnswerCorrect = 2,
        AllCorrect = 3
    }

    /// <summary>
    /// Trieda, ktora reprezentuje manazera prekladu. - Server.
    /// </summary>
    public class EnglishManagerServer
    {
        private LogicGame aLogicGame;

        private List<VocabularyItems> aVocabularyItemsList;

        /// <summary>
        /// Dictionary, ktora reprezentuje uz vybrate polozky z Vocabulary
        /// </summary>
        private Dictionary<int, bool> aAlreadyPickedDictionary;

        /// <summary>
        /// List, ktory v sebe uchovava informacie o ButtonServerBlokoch - typ List<ButtonBlockServer>
        /// </summary>
        private List<ButtonBlockServer> aButtonList;

        /// <summary>
        /// List, ktory v sebe uchovava informacie o EndBlokoch - typ List<EndBlockServer>
        /// </summary>
        private List<EndBlockServer> aEndBlockList;

        /// <summary>
        /// List, ktory v sebe uchovava informacie o vybranych anglickych slovach z vocabulary - typ List<string>
        /// </summary>
        private List<string> aPickedEnglishWordsList;

        /// <summary>
        /// List, ktory v sebe uchovava informacie o vybranych slovenskych slovach z vocabulary - typ List<string>
        /// </summary>
        private List<string> aPickedSlovakWordsList;

        /// <summary>
        /// Dictionary, ktora bude obsahovat na jednej strane slovenske slovo a na druhej anglicke - typ Dictionary<string, string>
        /// </summary>
        private Dictionary<string, string> aCompareWordsDictionary;

        /// <summary>
        /// Atribut, ktory reprezentuje Vocabulary Feedback - typ Vocabulary Feedback
        /// </summary>
        private VocabularyFeedback aVocabularyFeedback;

        /// <summary>
        /// Atribut, ktory reprezentuje posledne zvolene anglicke slovo - typ int
        /// </summary>
        private int aLastEnglishWord;

        /// <summary>
        /// Atribut, ktory reprezentuje posledne zvolene slovenske slovo - typ int
        /// </summary>
        private int aLastSlovakWord;

        /// <summary>
        /// Atribut, ktory reprezentuje ci je update pripraveny na odoslanie - typ bool
        /// </summary>
        private bool aUpdateIsReady;

        /// <summary>
        /// Atribut, ktory informuje objekt o tom, ze uroven je uz nacitana - typ bool
        /// </summary>
        private bool aLevelIsLoaded;

        /// <summary>
        /// Atribut, ktory bude reprezentovat, ze listy su uz poprehadzovane a zabrani tak nekonecnemu prehadzovaniu - typ bool
        /// </summary>
        private bool aAlreadyShuffled;

        /// <summary>
        /// Atribut, ktory reprezentuje body dosiahnute za uspesne prelozenie slov
        /// </summary>
        private int aEnglishPoints;

        public VocabularyFeedback VocabularyFeedback
        {
            get => aVocabularyFeedback;
            set => aVocabularyFeedback = value;
        }
        public bool UpdateIsReady
        {
            get => aUpdateIsReady;
            set => aUpdateIsReady = value;
        }
        public bool LevelIsLoaded
        {
            get => aLevelIsLoaded;
            set => aLevelIsLoaded = value;
        }
        public int EnglishPoints
        {
            get => aEnglishPoints;
            set => aEnglishPoints = value;
        }


        /// <summary>
        /// Konstruktor triedy, ktora sa stara o spravu Anglickeho Levelu na serveri
        /// </summary>
        /// <param name="parLogicGame">Parameter reprezentujuci hru - typ LogicGame - Musi tu byt skrz Content.Load</param>
        public EnglishManagerServer(LogicGame parLogicGame)
        {
            aLogicGame = parLogicGame;
            aAlreadyPickedDictionary = new Dictionary<int, bool>();

            aButtonList = new List<ButtonBlockServer>();
            aEndBlockList = new List<EndBlockServer>();

            aCompareWordsDictionary = new Dictionary<string, string>();

            // aPickedWordsList = new List<VocabularyItems>();
            aPickedEnglishWordsList = new List<string>();
            aPickedSlovakWordsList = new List<string>();

            //Preto 2 listy, namiesto jedneho, pretoze pouzijeme Shuffle Algoritmus na nahodne zoradenie slov
            //Pouzijeme Durstenfieldov modifikovany Fisher and Yates algoritmus, lebo ma zlozitost O(n), pricom Fisher ma O(n^2)

            InitVocabulary("Vocabulary\\vocabulary");
            aVocabularyFeedback = VocabularyFeedback.None;

            ResetLastWords();

            aLevelIsLoaded = false;

            aUpdateIsReady = false;

            aAlreadyShuffled = false;

        }

        /// <summary>
        /// Metoda, ktora Inicializuje Otazky zo xml suboru
        /// </summary>
        /// <param name="parVocabularyPath">Parameter - cesta k suboru z databami o slovniku - typ string</param>
        public void InitVocabulary(string parVocabularyPath)
        {
            try
            {
                var tmpVocabulary = aLogicGame.Content.Load<XMLData.Vocabulary>(parVocabularyPath);
                aVocabularyItemsList = tmpVocabulary.VocabularyItems;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// Metoda, ktora nahodne vyberie polozku z Vocabulary a vrati ju
        /// <returns>Vrati nahodne zvolenu polozku z Vocabulary - typ VocabularyItem</returns>
        /// </summary>
        public VocabularyItems PickVocabularyItem()
        {
            if (aAlreadyPickedDictionary != null && aVocabularyItemsList != null)
            {
                Random tmpRandom = new Random();

                int tmpGeneratedItemKey =
                    tmpRandom.Next(0, aVocabularyItemsList.Count); //Vyberieme od 0 az po velkost Listu Vocabulary

                while (aAlreadyPickedDictionary.TryAdd(tmpGeneratedItemKey, true) == false
                ) //Ak sa sa este takyto kluc nenachadza v Dictionary - pridame ho, inak sa pokusime vygenerovat novy kluc
                {
                    tmpGeneratedItemKey = tmpRandom.Next(0, aVocabularyItemsList.Count);
                }

                aCompareWordsDictionary.Add(aVocabularyItemsList[tmpGeneratedItemKey].Slovak, aVocabularyItemsList[tmpGeneratedItemKey].English);

                return aVocabularyItemsList[tmpGeneratedItemKey];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Metoda, ktora prida tlacidlo do zoznamu tlacidlo
        /// </summary>
        /// <param name="parButton">Parameter, ktory reprezentuje tlacidlo, ktore sa ma pridat do zoznamu tlacidiel - typ ButtonBlockServer</param>
        public void AddButton(ButtonBlockServer parButton)
        {
            if (aButtonList != null)
            {
                aButtonList.Add(parButton);

                if (aButtonList.Count % 2 != 0) //Po pridani kazdeho druheho tlacitka, budeme vediet, ze potrebujeme jedno slovo
                {

                    VocabularyItems tmpVocabularyItem = new VocabularyItems();
                    tmpVocabularyItem = PickVocabularyItem();

                    aPickedEnglishWordsList.Add(tmpVocabularyItem.English);
                    aPickedSlovakWordsList.Add(tmpVocabularyItem.Slovak);
                }
            }
        }

        /// <summary>
        /// Metoda, ktora prida End Block do dictionary
        /// </summary>
        /// <param name="parEndBlock">Parameter, charakterizujuci EndBlock</param>
        public void AddEndBlock(EndBlockServer parEndBlock)
        {
            if (aEndBlockList != null && aEndBlockList.Count < 2)
            {
                aEndBlockList.Add(parEndBlock);
            }
        }


        /// <summary>
        /// Metoda, ktora pripravi data o otazkach na odoslanie klientom
        /// </summary>
        /// <param name="parOutgoingMessage">Parameter reprezentujuci odchadzajucu spravu - typ NetOutgoingMessage</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareVocabularyData(NetOutgoingMessage parOutgoingMessage)
        {
            if (aPickedEnglishWordsList != null && aPickedSlovakWordsList != null && aPickedSlovakWordsList.Count == aPickedEnglishWordsList.Count) //Porovname ci existuju a su rovnakej dlzky
            {
                parOutgoingMessage.Write((byte)aVocabularyFeedback);

                if (aVocabularyFeedback == VocabularyFeedback.Init)
                {
                    parOutgoingMessage.WriteVariableInt32(aPickedSlovakWordsList.Count); //Vieme, ze su oba rovnakej dlzky, takze mozme pouzit pocet len z jedneho

                    for (int i = 0; i < aPickedSlovakWordsList.Count; i++)
                    {
                        parOutgoingMessage.Write(aPickedEnglishWordsList[i]);
                        parOutgoingMessage.Write(aPickedSlovakWordsList[i]);
                    }
                }

                if (aVocabularyFeedback == VocabularyFeedback.AnswerCorrect)
                {
                    parOutgoingMessage.WriteVariableInt32(aLastEnglishWord);
                    parOutgoingMessage.WriteVariableInt32(aLastSlovakWord);

                    ResetLastWords();
                    aVocabularyFeedback = VocabularyFeedback.None; //Zresetujeme Feedback
                }

                return parOutgoingMessage;
            }


            return parOutgoingMessage;
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizaciu Managera
        /// </summary>
        public void Update()
        {

            if (aAlreadyShuffled == false && aLevelIsLoaded && aPickedEnglishWordsList != null && aPickedSlovakWordsList != null)
            {
                ShuffleList(aPickedSlovakWordsList);
                ShuffleList(aPickedEnglishWordsList);

                aAlreadyShuffled = true;
            }

            for (int i = 0; i < aButtonList.Count; i++)
            {
                if (aButtonList[i].WantsToInteract && aButtonList[i].Succeded == false)
                {
                    if (i % 2 != 0) //Slovenske
                    {
                        aLastSlovakWord = i / 2; //Server nepotrebuje vediet realne o ake slova ide, staci ID cislo

                    }
                    else //Anglicke
                    {
                        aLastEnglishWord = i / 2;
                    }

                    aButtonList[i].WantsToInteract = false;
                }

            }

            if (aCompareWordsDictionary!= null && aPickedSlovakWordsList != null && aPickedEnglishWordsList != null && aLastSlovakWord != -1 && aLastEnglishWord != -1)
            {
                if (aVocabularyFeedback != VocabularyFeedback.AnswerCorrect && aCompareWordsDictionary[aPickedSlovakWordsList[aLastSlovakWord]] == aPickedEnglishWordsList[aLastEnglishWord])
                {//Prva podmienka, sluzi na to aby sa nestalo, ze este pred odoslanim updatu o uspesnej odpovedi sa kod v podmienke vykonal zbytocne znova
                    aVocabularyFeedback = VocabularyFeedback.AnswerCorrect;

                    Debug.WriteLine("Vysledok spravny");

                    //V tomto momente, doposial aLastSlovak/EnglishWord reprezentovalo IDcko v Listoch aPickedSlovak/EnglishWordsList
                    //Teraz, kedze aj pouzivatelovi budeme chciet poslat IDcka tlacidiel a porovnaj aj ID tlacidiel u servera
                    //Vypocitame ButtonID priamo takto:

                    aLastEnglishWord *= 2; //ID * 2 | 0 -> 0, 1-> 2, 2-> 4 = Tlacitka na lavej strane
                    aLastSlovakWord = aLastSlovakWord * 2 + 1; //ID * 2 + 1 |  0 -> 1, 1-> 3, 2-> 5, Tlacitka na pravej strane

                    //Tu to bolo celkom zaujimave, pretoze aLastSlovakWord*= 2 + 1 -> Najprv vypocitalo scitanie a az potom nasobilo

                    aButtonList[aLastEnglishWord].WantsToInteract = false; //tlacidlo zlava - anglicke slovo
                    aButtonList[aLastEnglishWord].ChangeToSuccessState();
                    aButtonList[aLastSlovakWord].WantsToInteract = false; //tlacidlo zprava - slovenske slovo
                    aButtonList[aLastSlovakWord].ChangeToSuccessState();


                    aUpdateIsReady = true;

                    aEnglishPoints++;  //Kedze odpoved bola spravna, mozeme pripocitat aj body
                }

            }

            if (aEndBlockList != null && aButtonList != null && aEnglishPoints == aButtonList.Count / 2) //Podmineka osetrenie null a porovnavame ci sme dosiahli tolko bodov, kolko je tlacitok pre anglicke a slovenske slova - preto / 2-
            {
                int tmpCountOfStandingPlayers = 0;

                for (int j = 0; j < aEndBlockList.Count; j++)
                {
                    if (aEndBlockList[j].SomethingIsStandingOnTop)
                    {
                        tmpCountOfStandingPlayers++;
                    }
                }
                if (tmpCountOfStandingPlayers >= 2) //Debug Len jeden
                {
                    Debug.WriteLine("Server - Vyhra");
                    aVocabularyFeedback = VocabularyFeedback.AllCorrect;
                    //Odtialto si nasledne Server sam preberie informaciu i vyhre
                }
            }
        }


        /// <summary>
        /// Metoda, ktora implementuje modifikaciu Fisher–Yates algoritmu -> Durstenfeld
        /// </summary>
        /// <typeparam name="T">Type Parameter T - Je tu pre to aby metoda nema specifikovany konkretny typ - napr. string.</typeparam>
        /// <param name="parList">Parameter - List v ktorom chceme poprehadzovat prvky.</param>
        public void ShuffleList<T>(List<T> parList)
        {
            Random tmpRand = new Random();

            for (int i = 0; i < parList.Count - 1; i++)
            {
                int tmpRandInteger = tmpRand.Next(i, parList.Count);

                if (i != tmpRandInteger) //Viem, ze tu moze nastat situacia, ze prakticky sa nic neprehodia prvky, ale to nevadi, hraci budu mat proste stastie a tym lahsiu uroven
                { //Takyto pripad by vedel vyriesit Sattolov algoritmus.
                    T tmpListComponent = parList[i]; 
                    parList[i] = parList[tmpRandInteger]; 
                    parList[tmpRandInteger] = tmpListComponent; 
                }
            }
        }

        /// <summary>
        /// Metoda, ktora vymaze indexovy zaznam o poslednych vybranych slovach
        /// </summary>
        public void ResetLastWords()
        {
            aLastEnglishWord = -1;
            aLastSlovakWord = -1;
        }

    }
}
