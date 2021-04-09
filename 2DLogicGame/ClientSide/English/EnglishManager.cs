using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.GraphicObjects;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace _2DLogicGame.ClientSide.English
{

    public enum VocabularyFeedback
    {
        None = 0,
        Init = 1,
        CorrectAnswer = 2
    }

    public class EnglishManager : GameComponent
    {
        private List<string> aSlovakWords;

        private List<string> aEnglishWords;

        private EnglishSlovakWords aEnglishSlovakUI;

        private List<ButtonBlock> aButtonList;

        private LogicGame aLogicGame;

        private VocabularyFeedback aVocabularyFeedback;

        public EnglishManager(LogicGame parLogicGame) : base(parLogicGame)
        {
            aSlovakWords = new List<string>();
            aEnglishWords = new List<string>();

            aLogicGame = parLogicGame;

            float tmpYPosition = 1080 / 2;

            float tmpSizeOfWordsX = 1920 / 5F;

            float tmpXPositionsEnglish = 1920 / 4F - tmpSizeOfWordsX / 3F;
            float tmpXPositionsSlovak = 1920 / (4 / 3F) - tmpSizeOfWordsX / 3F;


            aButtonList = new List<ButtonBlock>();

            aEnglishSlovakUI = new EnglishSlovakWords(parLogicGame, new Vector2(tmpXPositionsEnglish, tmpYPosition), new Vector2(tmpXPositionsSlovak, tmpYPosition), new Vector2(tmpSizeOfWordsX, 100));

            aVocabularyFeedback = VocabularyFeedback.None;

            Init();

        }

        public void Init()
        {
            aLogicGame.Components.Add(this);
            aLogicGame.Components.Add(aEnglishSlovakUI);
        }

        public void Destroy()
        {
            aLogicGame.Components.Remove(this);
            aLogicGame.Components.Remove(aEnglishSlovakUI);

        }

        public void AddButton(ButtonBlock parButton)
        {
            if (aButtonList != null)
            {
                aButtonList.Add(parButton);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (aButtonList != null && aButtonList.Count > 0 && aEnglishSlovakUI != null)
            {
                for (int i = 0; i < aButtonList.Count; i++)
                {
                    if (aButtonList[i].IsTurnedOn && aEnglishSlovakUI != null)
                    {
                        if (i % 2 == 0)
                        {
                            aEnglishSlovakUI.Show(1);
                            aEnglishSlovakUI.EnglishWord = aEnglishWords[i / 2];
                        }
                        else
                        {
                            aEnglishSlovakUI.Show(2);
                            aEnglishSlovakUI.SlovakWord = aSlovakWords[i / 2];
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Metoda, ktora sa stara o spravu prijatych dat
        /// </summary>
        /// <param name="parNetIncomingMessage">Paramtere, reprezentujuci prijate spravu obsahujucu prijate data - typ NetIncommingMessage - buffer</param>
        public void HandleIncomingData(NetIncomingMessage parNetIncomingMessage)
        {
            if (aEnglishSlovakUI != null)
            {

                VocabularyFeedback tmpFeedBack = (VocabularyFeedback)parNetIncomingMessage.ReadByte();

                if (tmpFeedBack == VocabularyFeedback.Init)
                {
                    int tmpCountOfVocabulary = parNetIncomingMessage.ReadVariableInt32();

                    for (int i = 0; i < tmpCountOfVocabulary; i++)
                    {
                        aEnglishWords.Add(parNetIncomingMessage.ReadString());
                        aSlovakWords.Add(parNetIncomingMessage.ReadString());

                    }
                }

                if (tmpFeedBack == VocabularyFeedback.CorrectAnswer)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int tmpButtonID = parNetIncomingMessage.ReadVariableInt32();

                        aButtonList[tmpButtonID].WantsToInteract = false; //Prve tlacidlo zlava - ID Tlacitka dostaneme -> 2 * Cislo
                        aButtonList[tmpButtonID].ChangeToSuccessState();
                    }

                }

            }
        }
    }
}
