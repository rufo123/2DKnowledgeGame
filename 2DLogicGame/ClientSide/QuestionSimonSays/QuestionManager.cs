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
        Initialization = 3,
        IncorrectFinalAnswers = 4
    }

    public enum AnswerColors
    {
        Red = 'A',
        Purple = 'B',
        Green = 'C',
        Blue = 'D'
    }


    public class QuestionManager : GameComponent
    {
        private Questions aQuestionUI;

        private QuestionsColorFeedBack aQuestionColorFeedBack;

        private WrongAnswerIndicator aWrongAnswerIndicator;

        private LogicGame aLogicGame;

        private List<char> aGoodAnswersList;

        private List<DoorBlock> aDoorsList;

        private List<InputBlock> aInputList;

        private int aQuestionPoints;

        private bool aNeedsReset;

        private DoorBlock aDoor; //Dvere budu len 1

        /// <summary>
        /// Atribut, ktory reprezentuje ci ide o prveho hraca - typ bool
        /// </summary>
        private bool aAmIFirstPlayer;

        /// <summary>
        /// Pocitadlo, kolko tlacitok bolo pridanych - pouziva sa na to aby program vedel akemu tlacitku ma ako zmenit farbu
        /// </summary>
        private int aButtonCounter;

        public bool NeedsReset
        {
            get => aNeedsReset;
            set => aNeedsReset = value;
        }


        public QuestionManager(LogicGame parGame) : base(parGame)
        {
            //Inicializacia QuestionUI
            aQuestionUI = new Questions(parGame, new Vector2(208, 100), new Vector2(1600, 523));

            //Inicializacia Question FeedBacku
            aQuestionColorFeedBack = new QuestionsColorFeedBack(parGame, new Vector2(800, 200), new Vector2(1920 - 800 * 2, 1920 - 800 * 2));

            Vector2 tmpSizeOfIndicator = new Vector2(256, 256);
            Vector2 tmpPositionOfIndicator = new Vector2(1920 / 2F - tmpSizeOfIndicator.X / 2 + 48, 1080 / 2F - tmpSizeOfIndicator.Y / 2);
            aWrongAnswerIndicator = new WrongAnswerIndicator(parGame, tmpPositionOfIndicator, tmpSizeOfIndicator);

            aGoodAnswersList = new List<char>();

            aLogicGame = parGame;

            aLogicGame.Components.Add(this);

            aDoorsList = new List<DoorBlock>();

            aInputList = new List<InputBlock>();

            aNeedsReset = false;

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
                aLogicGame.Components.Add(aWrongAnswerIndicator);
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
                aLogicGame.Components.Remove(aWrongAnswerIndicator);
                aGoodAnswersList.Clear();
                aGoodAnswersList = null;
                aQuestionUI = null;
                aQuestionColorFeedBack = null;
                aLogicGame.Components.Remove(this);
                aDoorsList.Clear();
                aDoorsList = null;
                aQuestionUI = null;
                aQuestionColorFeedBack = null;
                aWrongAnswerIndicator = null;
            }
        }

        public void AddButton(ButtonBlock parButton)
        {
            parButton.SwitchButtonStyle(aButtonCounter);
            aButtonCounter++;
        }

        public void AddDoors(DoorBlock parDoors)
        {
            aDoorsList.Add(parDoors);
        }

        public void AddInput(InputBlock parInput)
        {
            aInputList.Add(parInput);
        }

        public void HandleIncomingData(NetIncomingMessage parIncomingMessage, bool parAmIFirstPlayer)
        {
            if (aQuestionUI != null)
            {
                QuestionFeedback tmpFeedback = new QuestionFeedback();

                tmpFeedback = (QuestionFeedback)parIncomingMessage.ReadByte();

                bool tmpNeedsCompleteReset = (tmpFeedback == QuestionFeedback.IncorrectFinalAnswers);

                if (aGoodAnswersList != null && (aGoodAnswersList.Count <= 4 || tmpNeedsCompleteReset)) //Bude citat, nasledujuce data len vtedy ak zodpovedane otazky su 4 alebo je potrebny kompletny reset
                {
                    char tmpCurrentAnswer = (char)parIncomingMessage.ReadByte();

                    aQuestionUI.CurrentQuestionText = parIncomingMessage.ReadString();

                    List<string> tmpAnswersList = new List<string>();

                    for (int i = 0; i < 4; i++)
                    {
                        tmpAnswersList.Add(parIncomingMessage.ReadString());
                    }

                    aQuestionUI.SetAnswers(tmpAnswersList);


                    aAmIFirstPlayer = parAmIFirstPlayer;

                    switch (tmpFeedback)
                    {
                        case QuestionFeedback.None:
                            break;
                        case QuestionFeedback.Correct:
                            aGoodAnswersList.Add(tmpCurrentAnswer);
                            aQuestionColorFeedBack.Color = ConvertCharToColor(tmpCurrentAnswer);
                            if (!parAmIFirstPlayer)
                            {
                                aQuestionColorFeedBack.Show = true;
                            }
                            break;
                        case QuestionFeedback.Incorrect:
                            Reset();
                            break;
                        case QuestionFeedback.Initialization:
                            if (parAmIFirstPlayer) //Ak pojde o prveho hraca, tomu zobrazime iba otazky a mozne odpovede
                            {
                                aQuestionUI.ShowEnabled = true;
                            }
                            else
                            {
                                aQuestionColorFeedBack.ShowEnabled = true; //A ak pojde o druheho hraca tomu zobrazime len farebny Feedback
                            }
                            break;
                        case QuestionFeedback.IncorrectFinalAnswers:
                            aNeedsReset = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                //Spracovanie dat o Inpute

                if (aGoodAnswersList != null && aGoodAnswersList.Count > 3)
                { //Ak boli zodpovedane aspon 3 otazky, preto nie 4, pretoze u klienta este nedoslo k aktu

                    if (parIncomingMessage.ReadString() == "InputBlocks")
                    {
                        int tmpCountOfInputCycle = parIncomingMessage.ReadVariableInt32();

                        for (int i = 0; i < tmpCountOfInputCycle; i++)
                        {

                            if (aInputList[i] != null)
                            {
                                aInputList[i].Number = parIncomingMessage.ReadVariableInt32();
                            }

                        }
                    }
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

        public override void Update(GameTime gameTime)
        {
            if (aGoodAnswersList.Count > 3) //Ak su vsetky 4 odpovede spravne
            {
                if (aQuestionUI != null)
                {
                    if (aQuestionUI.ShowEnabled)
                    {
                        aQuestionUI.ShowEnabled = false;
                    }
                }

                if (aDoorsList != null && aDoorsList.Count >= 2)
                {
                    aDoorsList[1].IsHidden = true; //Schovame druhe dvere
                }
            }


            base.Update(gameTime);
        }

        public void Reset()
        {
            aGoodAnswersList.Clear();
            aWrongAnswerIndicator.Show = true;
            for (int i = 0; i < aDoorsList.Count; i++)
            {
                aDoorsList[i].IsHidden = false;
            }
        }


    }
}
