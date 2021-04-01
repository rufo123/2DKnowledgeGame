using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using XMLData;

namespace _2DLogicGame.ServerSide.Questions_ServeSide
{

    public enum QuestionFeedback
    {
        Correct = 0,
        Incorrect = 1
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
        /// Konstruktor triedy, ktora sa stara o spravu Otazok na serveri
        /// </summary>
        /// <param name="parLogicGame">Parameter reprezentujuci hru - typ LogicGame - Musi tu byt skrz Content.Load</param>
        public QuestionsManagerServer(LogicGame parLogicGame)
        {
            aQuestionsList = new List<QuestionsStorageItems>();
            InitQuestions("Questions\\questions.xml");
            aCurrentQuestionNumber = 0;
            aCountOfCorrectlyAnswered = 0;
            PickNewQuestion();
        }

        /// <summary>
        /// Metoda, ktora Inicializuje Otazky zo xml suboru
        /// </summary>
        /// <param name="parQuestionsPath">Parameter - cesta k suboru z databami o otazkach - typ string</param>
        public void InitQuestions(string parQuestionsPath)
        {
            XMLData.QuestionsStorage tmpQuestions;
            try
            {
                tmpQuestions = aLogicGame.Content.Load<XMLData.QuestionsStorage>(parQuestionsPath);
                aQuestionsList = tmpQuestions.QuestionsStorageItemsList;

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
                aCurrentQuestionNumber = rand.Next(0, aQuestionsList.Count);
            }

        }

        /// <summary>
        /// Metoda, ktora zresetuje otazky
        /// </summary>
        public void ResetQuestions()
        {
            aCountOfCorrectlyAnswered = 0;
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
                parOutgoingMessage.Write(aQuestionsList[aCurrentQuestionNumber].Question);

                for (int i = 0; i < 4; i++)
                {
                    parOutgoingMessage.Write(aQuestionsList[aCurrentQuestionNumber].ListOfAnswers[i]);
                }

                return parOutgoingMessage;
            }

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


    }
}
