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
        Initialization = 3
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
        /// Atribut, ktory reprezentuje pocet spravce zodpovedanych otazok, pri zlej odpovedi sa zresetuje
        /// </summary>
        private int aCountOfCorrectlyAnswered;

        /// <summary>
        /// Atribut, ktory reprezentuje Feedback, ci bola otazka zodpovedana spravne alebo nie
        /// </summary>
        private QuestionFeedback aQuestionFeedback;

        /// <summary>
        /// Atribut, ktory reprezentuje List tlacitko - v tomto pripade - prve 4 - Odpovede, 5-ty potvrdenie vyberu farieb
        /// </summary>
        private List<ButtonBlockServer> aButtonList;

        /// <summary>
        /// Atribut, reprezentujuci momentalne zadanu odpoved - typ char
        /// </summary>
        private char aCurrentAnswer;

        /// <summary>
        /// Atribut, reprezentujuci, ci je Update pripraveny, napr pri Inicializacii, Spravnej odpovedi alebo nespravnej odpovedi - typ bool
        /// </summary>
        private bool aUpdateIsReady;

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
            aCountOfCorrectlyAnswered = 0;
            aButtonList = new List<ButtonBlockServer>();
            aUpdateIsReady = false;
            aDictionaryOfAlreadySentAnswers = new Dictionary<int, bool>();
            PickNewQuestion();
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

        /// <summary>
        /// Metoda, ktora zresetuje otazky
        /// </summary>
        public void ResetQuestions()
        {
            aCountOfCorrectlyAnswered = 0;
            aDictionaryOfAlreadySentAnswers.Clear();
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
                parOutgoingMessage.Write((byte)aCurrentAnswer);

                parOutgoingMessage.Write(aQuestionsList[aCurrentQuestionNumber].Question);

                for (int i = 0; i < 4; i++)
                {
                    parOutgoingMessage.Write(aQuestionsList[aCurrentQuestionNumber].ListOfAnswers[i]);
                }

                return parOutgoingMessage;
            }

            //Poradie - Odpoved -> FeedBack -> Otazka -> List Odpovedi

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
            for (int i = 0; i < aButtonList.Count; i++)
            {
                if (aButtonList[i].WantsToInteract == true)
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
                        }
                        else
                        {
                            QuestionFeedback = QuestionFeedback.Incorrect;
                            ResetQuestions();
                            PickNewQuestion();
                            aCurrentAnswer = ConvertNumberToChar(i);
                            aUpdateIsReady = true;
                            aButtonList[i].WantsToInteract = false;

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

        /// <summary>
        /// Metoda, ktora sa stara o upratanie "neporiadku", ktory po sebe zanechala
        /// </summary>
        public void Destroy()
        {
            if (aButtonList != null)
            {
                aButtonList.Clear();
                aButtonList = null; //GC uz sa o zbytok postara
            }
        }
    }
}
