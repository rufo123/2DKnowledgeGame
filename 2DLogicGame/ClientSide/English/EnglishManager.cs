using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.GraphicObjects;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace _2DLogicGame.ClientSide.English
{

    /// <summary>
    /// Enumeracna trieda, ktora reprezentuje feedback prekladu.
    /// </summary>
    public enum VocabularyFeedback
    {
        None = 0,
        Init = 1,
        CorrectAnswer = 2
    }

    /// <summary>
    /// Trieda, ktora reprezentuje manazer prekladu.
    /// </summary>
    public class EnglishManager : GameComponent
    {
        /// <summary>
        /// Atribut, reprezentujuci List - typu string, obsahujuci slovenske slova.
        /// </summary>
        private List<string> aSlovakWords;

        /// <summary>
        /// Atribut, reprezentujuci List - typu string, obsahujuci anglicke slova.
        /// </summary>
        private List<string> aEnglishWords;

        /// <summary>
        /// Atribut, reprezentujuci graficky komponent prekladu. - typ EnglishSlovakUI.
        /// </summary>
        private EnglishSlovakWords aEnglishSlovakUI;

        /// <summary>
        /// Atribut, reprezentujuci List - typu ButtonBlock, obsahujuci bloky, resp. tlacidla.
        /// </summary>
        private List<ButtonBlock> aButtonList;

        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame.
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut, reprezentujuci feedback - typ VocabularyFeedback - enum.
        /// </summary>
        private VocabularyFeedback aVocabularyFeedback;

        /// <summary>
        /// Konstruktor, manazera prekladu.
        /// </summary>
        /// <param name="parLogicGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
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

        /// <summary>
        /// Metoda, ktora inicializuje komponenty suvisiace s manazerom.
        /// </summary>
        public void Init()
        {
            aLogicGame.Components.Add(this);
            aLogicGame.Components.Add(aEnglishSlovakUI);
        }

        /// <summary>
        /// Metoda, ktora odstrani komponenty suvisiace s manazerom.
        /// </summary>
        public void Destroy()
        {
            aLogicGame.Components.Remove(this);
            aLogicGame.Components.Remove(aEnglishSlovakUI);

        }

        /// <summary>
        /// Metoda, ktora sa stara o pridanie tlacidiel do Listu.
        /// </summary>
        /// <param name="parButton">Parameter, reprezentujuci blok typu ButtonBlock.</param>
        public void AddButton(ButtonBlock parButton)
        {
            if (aButtonList != null)
            {
                aButtonList.Add(parButton);
            }
        }

        /// <summary>
        /// Override metoda, ktora sa stara o aktualizaciu manazera, rozdeluje vykreslovanie anglickych a slovenskych slov.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Update(GameTime parGameTime)
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

            base.Update(parGameTime);
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
