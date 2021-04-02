using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.GraphicObjects;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace _2DLogicGame.ClientSide.QuestionSimonSays
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


    public class QuestionManager
    {
        private Questions aQuestionUI;

        private QuestionsColorFeedBack aQuestionColorFeedBack;

        private LogicGame aLogicGame;

        private List<char> aGoodAnswersList;

        /// <summary>
        /// Pocitadlo, kolko tlacitok bolo pridanych - pouziva sa na to aby program vedel akemu tlacitku ma ako zmenit farbu
        /// </summary>
        private int aButtonCounter;

        public QuestionManager(LogicGame parGame)
        {
            //Inicializacia QuestionUI
            aQuestionUI = new Questions(parGame, new Vector2(208, 100), new Vector2(1600, 523));

            //Inicializacia Question FeedBacku
            aQuestionColorFeedBack = new QuestionsColorFeedBack(parGame, new Vector2(800, 200), new Vector2(1920 - 800 * 2, 1920 - 800 * 2));

            aGoodAnswersList = new List<char>();

            aLogicGame = parGame;
            Init();
        }

        /// <summary>
        /// Metoda, ktora inicializuje komponenty
        /// </summary>
        public void Init()
        {
            if (aQuestionUI != null && aQuestionColorFeedBack != null && aLogicGame != null)
            {
                aLogicGame.Components.Add(aQuestionUI);
                aLogicGame.Components.Add(aQuestionColorFeedBack);
            }

        }

        /// <summary>
        /// Metoda, ktora znici inicializovane komponenty
        /// </summary>
        public void Destroy()
        {
            if (aQuestionUI != null && aQuestionColorFeedBack != null && aLogicGame != null)
            {
                aLogicGame.Components.Remove(aQuestionUI);
                aLogicGame.Components.Remove(aQuestionColorFeedBack);
                aGoodAnswersList.Clear();
                aGoodAnswersList = null;
                aQuestionUI = null;
                aQuestionColorFeedBack = null;
            }
        }

        public void AddButton(ButtonBlock parButton)
        {
            parButton.SwitchButtonStyle(aButtonCounter);
            aButtonCounter++;
        }

        public void HandleIncommingData(NetIncomingMessage parIncomingMessage, bool parAmIFirstPlayer)
        {
            if (aQuestionUI != null)
            {
                QuestionFeedback tmpFeedback = new QuestionFeedback();

                tmpFeedback = (QuestionFeedback)parIncomingMessage.ReadByte();

                char tmpCurrentAnswer = (char)parIncomingMessage.ReadByte();

                aQuestionUI.CurrentQuestionText = parIncomingMessage.ReadString();

                List<string> tmpAnswersList = new List<string>();

                for (int i = 0; i < 4; i++)
                {
                    tmpAnswersList.Add(parIncomingMessage.ReadString());
                }

                aQuestionUI.SetAnswers(tmpAnswersList);

                switch (tmpFeedback)
                {
                    case QuestionFeedback.None:
                        break;
                    case QuestionFeedback.Correct:
                        aGoodAnswersList.Add(tmpCurrentAnswer);
                        aQuestionColorFeedBack.Color = ConvertCharToColor(tmpCurrentAnswer);
                        aQuestionColorFeedBack.Show = true;
                        break;
                    case QuestionFeedback.Incorrect:
                        aGoodAnswersList.Clear();
                        break;
                    case QuestionFeedback.Initialization:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //Poradie - FeedBack -> Otazka -> List Odpovedi

           // return parOutgoingMessage;

        }

        private Color ConvertCharToColor(char parAnswerChar)
        {
            switch ((AnswerColors)parAnswerChar)
            {
                case AnswerColors.Red:
                    return Color.Red;
                case AnswerColors.Purple:
                    return Color.Purple;
                case AnswerColors.Green:
                    return Color.Green;
                case AnswerColors.Blue:
                    return Color.Blue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parAnswerChar), parAnswerChar, null);
            }
        }



    }
}
