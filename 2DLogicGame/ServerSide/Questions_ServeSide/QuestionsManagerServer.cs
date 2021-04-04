using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using Lidgren.Network;
using XMLData;

namespace _2DLogicGame.ServerSide.Questions_ServeSide
{

    public enum QuestionFeedback
    {
        None = 0,
        Correct = 1,
        Incorrect = 2,
        Initialization = 3,
        IncorrectFinalAnswers = 4,
        Complete = 5
    }

    public enum AnswerColors
    {
        Red = 'A',
        Purple = 'B',
        Green = 'C',
        Blue = 'D'
    }

    public class QuestionsManagerServer
    {
        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut, reprezentujuci List Otazok - typ List<QuestionsStorageItems>
        /// </summary>
        private List<QuestionsStorageItems> aQuestionsList;

        /// <summary>
        /// Atribut, ktory reprezentuje momentalne zvolenu otazku, resp. jej cislo - typ int
        /// </summary>
        private int aCurrentQuestionNumber;

        /// <summary>
        /// Atribut, ktory reprezentuje Feedback, ci bola otazka zodpovedana spravne alebo nie
        /// </summary>
        private QuestionFeedback aQuestionFeedback;

        /// <summary>
        /// Atribut, ktory reprezentuje List tlacitok - v tomto pripade - prve 4 - Odpovede, 5-ty potvrdenie vyberu farieb
        /// </summary>
        private List<ButtonBlockServer> aButtonList;

        /// <summary>
        /// Atribut, ktory reprezentuje List dveri - typ List<DoorBlockServer>
        /// </summary>
        private List<DoorBlockServer> aDoorsList;

        /// <summary>
        /// Atribut, ktory reprezentuje List input blokov - typ List<InputBlockServer>
        /// </summary>
        private List<InputBlockServer> aInputList;

        /// <summary>
        /// Atribut, ktory reprezentuje List znakov odpovedajucim dobrym odpovediam - typ List<char>
        /// </summary>
        private List<char> aGoodAnswersList;

        /// <summary>
        /// Atribut, reprezentujuci momentalne zadanu odpoved - typ char
        /// </summary>
        private char aCurrentAnswer;

        /// <summary>
        /// Atribut, reprezentujuci, ci je Update pripraveny, napr pri Inicializacii, Spravnej odpovedi alebo nespravnej odpovedi - typ bool
        /// </summary>
        private bool aUpdateIsReady;

        /// <summary>
        /// Atribu, ktory reprezentuje specialne cislo, ktore je vysledok scitania, obsahu input blokov, sluzi na porovnavanie zmeny - typ int
        /// </summary>
        private int aOldSumNumber;

        /// <summary>
        /// Atribut, ktory sluzi na zaznamenavanie poctu uz odoslanych otazok - typ iny
        /// </summary>
        private int aCountOfAlreadySentAnswers;

        /// <summary>
        /// Atribut, ktory sluzi na zaznamenavanie bodov za otazky
        /// </summary>
        private int aQuestionPoints;

        /// <summary>
        /// Atribut, ktory signalizuje, ze je potrebny Reset Levelu
        /// </summary>
        private bool aNeedsReset;

        /// <summary>
        /// Atribu, ktory reprezentuje Dictionary obsahujucu cisla, ktore uz boli odoslane -> Key - Int -> Cislo, Value bool -> True/False
        /// </summary>
        private Dictionary<int, bool> aDictionaryOfAlreadySentAnswers;

        public QuestionFeedback QuestionFeedback
        {
            get => aQuestionFeedback;
            set => aQuestionFeedback = value;
        }
        public bool UpdateIsReady
        {
            get => aUpdateIsReady;
            set => aUpdateIsReady = value;
        }
        public bool NeedsReset
        {
            get => aNeedsReset;
            set => aNeedsReset = value;
        }


        /// <summary>
        /// Konstruktor triedy, ktora sa stara o spravu Otazok na serveri
        /// </summary>
        /// <param name="parLogicGame">Parameter reprezentujuci hru - typ LogicGame - Musi tu byt skrz Content.Load</param>
        public QuestionsManagerServer(LogicGame parLogicGame)
        {
            aLogicGame = parLogicGame;
            aQuestionsList = new List<QuestionsStorageItems>();
            InitQuestions("Questions\\questions");
            aCurrentQuestionNumber = 0;
            aButtonList = new List<ButtonBlockServer>();
            aDoorsList = new List<DoorBlockServer>();
            aInputList = new List<InputBlockServer>();
            aGoodAnswersList = new List<char>();
            aUpdateIsReady = false;
            aDictionaryOfAlreadySentAnswers = new Dictionary<int, bool>();
            PickNewQuestion();
            aOldSumNumber = 0;
            aCountOfAlreadySentAnswers = 0;
            aQuestionPoints = 0;
        }

        /// <summary>
        /// Metoda, ktora Inicializuje Otazky zo xml suboru
        /// </summary>
        /// <param name="parQuestionsPath">Parameter - cesta k suboru z databami o otazkach - typ string</param>
        public void InitQuestions(string parQuestionsPath)
        {
            XMLData.QuestionsStorage tmpQuestionsStorage;
            try
            {
                tmpQuestionsStorage = aLogicGame.Content.Load<XMLData.QuestionsStorage>(parQuestionsPath);
                aQuestionsList = tmpQuestionsStorage.QuestionsStorageItemsList;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// Metoda, ktora nahodne vyberie jednu otazku
        /// </summary>
        public void PickNewQuestion()
        {
            Random rand = new Random();

            if (aQuestionsList != null)
            {
                bool tmpIsQuestionUnique = false;

                while (tmpIsQuestionUnique == false) //Pokusi sa vygenerovat unikatnu otazku, aby sa 2 krat za sebou neopakovala..
                {
                    aCurrentQuestionNumber = rand.Next(0, aQuestionsList.Count);
                    tmpIsQuestionUnique = aDictionaryOfAlreadySentAnswers.TryAdd(aCurrentQuestionNumber, true);
                }
            }
        }

        public void AddButton(ButtonBlockServer parButton)
        {
            if (aButtonList != null)
            {
                aButtonList.Add(parButton);
            }
        }

        public void AddDoors(DoorBlockServer parDoors)
        {
            if (aDoorsList != null)
            {
                aDoorsList.Add(parDoors);
            }
        }

        public void AddInput(InputBlockServer parInput)
        {
            if (aInputList != null)
            {
                aInputList.Add(parInput);
            }
        }

        /// <summary>
        /// Metoda, ktora zresetuje otazky
        /// </summary>
        public void ResetQuestions()
        {
            aDictionaryOfAlreadySentAnswers.Clear();
            aGoodAnswersList.Clear();
            aCountOfAlreadySentAnswers = 0;
            aQuestionPoints = 0;
        }

        /// <summary>
        /// Metoda, ktora pripravi data o otazkach na odoslanie klientom
        /// </summary>
        /// <param name="parOutgoingMessage">Parameter reprezentujuci odchadzajucu spravu - typ NetOutgoingMessage</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareQuestionData(NetOutgoingMessage parOutgoingMessage)
        {
            if (aQuestionsList != null)
            {
                parOutgoingMessage.Write((byte)aQuestionFeedback);

                if (aCountOfAlreadySentAnswers <= 4) //Ak este neboli zodpovedane vsetky otazky, tak ich odosleme
                {
                    parOutgoingMessage.Write((byte)aCurrentAnswer);
                    aCountOfAlreadySentAnswers = aGoodAnswersList.Count;

                    parOutgoingMessage.Write(aQuestionsList[aCurrentQuestionNumber].Question);

                    for (int i = 0; i < 4; i++)
                    {
                        parOutgoingMessage.Write(aQuestionsList[aCurrentQuestionNumber].ListOfAnswers[i]);
                    }
                }

                if (aGoodAnswersList.Count > 3) //Umoznime odosielanie dat v Input Blokoch
                {

                    parOutgoingMessage.Write("InputBlocks");
                    parOutgoingMessage.WriteVariableInt32(aInputList.Count);
                    if (aInputList != null && aInputList.Count > 3)
                    {
                        for (int i = 0; i < aInputList.Count; i++)
                        {
                            parOutgoingMessage.WriteVariableInt32(aInputList[i].Number);
                        }
                    }
                }

                QuestionFeedback = QuestionFeedback.None;

                return parOutgoingMessage;
            }

            //Poradie - Odpoved -> FeedBack -> Otazka -> List Odpovedi

            QuestionFeedback = QuestionFeedback.None;
            return parOutgoingMessage;
        }

        /// <summary>
        /// Metoda, ktora porovnava odpoved na otazku, podla parametra, ci je spravna
        /// </summary>
        /// <param name="parQuestionAnswer"></param>
        /// <returns></returns>
        public bool CompareQuestionAnswer(char parQuestionAnswer)
        {
            if (aQuestionsList != null)
            {
                if (parQuestionAnswer == aQuestionsList[aCurrentQuestionNumber].GoodAnswer)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizaciu Managera
        /// </summary>
        public void Update()
        {

            if (aDoorsList[1].IsHidden == false && aGoodAnswersList != null && aGoodAnswersList.Count > 3 && aDoorsList != null)
            {
                aDoorsList[1].IsHidden = true;
            }

            if (aGoodAnswersList != null && aGoodAnswersList.Count > 3) //Ak boli zodpovedane vsetky otazky, cize 4 nebudeme posielat znova otazky
            {

                int tmpSumOfInput = 0;

                for (int i = 0; i < aInputList.Count; i++)
                {
                    tmpSumOfInput += aInputList[i].Number;
                }

                if (aOldSumNumber != tmpSumOfInput)
                {
                    aUpdateIsReady = true;
                    aOldSumNumber = tmpSumOfInput;
                }

                if (aButtonList[aButtonList.Count - 1].WantsToInteract)
                {

                    if (CheckAnswers())
                    {
                        aQuestionPoints++;
                        aQuestionFeedback = QuestionFeedback.Complete; //Oznamime ze ulohy oboch hracov su splnene
                    }
                    else
                    {
                        ResetQuestions();
                        aQuestionFeedback = QuestionFeedback.IncorrectFinalAnswers;
                        aUpdateIsReady = true; //Musime odoslat update znova o tom ze doslo k nespravnej odpovedi, tentokrat od druheho klienta, no aj napriek tomu dojde k zresetovaniu
                        aNeedsReset = true;

                    }

                    //Ak nejaky hrac chce interagovat s poslednym tlacitkom, berieme to ako, ze chce odoslat odpovede...

                }
            }

            for (int i = 0; i < aButtonList.Count - 1; i++) //Posledny zatial netreba, ten sa kontroluje az na konci, potvrdzuje odpovede zadane druhym hracom
            {
                if (aButtonList[i].WantsToInteract == true && aGoodAnswersList != null && aCountOfAlreadySentAnswers < 4) //Druha podmienka, zabranuje generacii novych otazok, pokial su uz 4 zodpovedane 
                {
                    if (i >= 0 && i <= 3)
                    {
                        if (CompareQuestionAnswer(ConvertNumberToChar(i)))
                        {
                            PickNewQuestion();
                            QuestionFeedback = QuestionFeedback.Correct;
                            aCurrentAnswer = ConvertNumberToChar(i);
                            aUpdateIsReady = true;
                            aButtonList[i].WantsToInteract = false;
                            aGoodAnswersList.Add(ConvertNumberToChar(i));
                            aQuestionPoints++;
                        }
                        else
                        {
                            QuestionFeedback = QuestionFeedback.Incorrect;
                            ResetQuestions();
                            PickNewQuestion();
                            aCurrentAnswer = ConvertNumberToChar(i);
                            aUpdateIsReady = true;
                            aButtonList[i].WantsToInteract = false;
                            aGoodAnswersList.Clear();

                        }
                    }
                }

            }


        }

        /// <summary>
        /// Metoda, ktora prelozi cislo na znak, napr 1 -> A a podobne
        /// </summary>
        /// <param name="parAnswerNumber">Parameter - cislo, ktore sa ma prelozit na znak.</param>
        /// <returns>Vrati korespondujuci znak, podla parametra</returns>
        public char ConvertNumberToChar(int parAnswerNumber)
        {
            return Convert.ToChar(65 + parAnswerNumber);
        }

        public bool CheckAnswers()
        {
            bool tmpAreAnswersOk = false;

            for (int i = 0; i < aInputList.Count; i++) //Prejdeme cely list INputov
            {
                tmpAreAnswersOk = (ConvertNumberToChar(aInputList[i].Number - 1) == aGoodAnswersList[i]); //Porovname vzdy po jednej odpovedi, ci zodpoveda

            }
            return tmpAreAnswersOk;
        }

        /// <summary>
        /// Metoda, ktora sa stara o upratanie "neporiadku", ktory po sebe zanechala
        /// </summary>
        public void Destroy()
        {
            if (aButtonList != null)
            {
                aButtonList.Clear();
                aButtonList = null; //GC uz sa o zbytok postara
                aDoorsList.Clear();
                aDoorsList = null;
                aGoodAnswersList.Clear();
                aGoodAnswersList = null;
            }
        }
    }
}
