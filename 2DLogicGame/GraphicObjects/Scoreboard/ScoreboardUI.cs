using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.GraphicObjects.Scoreboard;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DLogicGame.GraphicObjects
{
    public class ScoreboardUI : DrawableGameComponent
    {

        private LogicGame aLogicGame;

        private Vector2 aSize;

        private Vector2 aPosition;

        private List<ScoreboardItems> aScoreboardItems;

        /// <summary>
        /// Atribut, reprezentujuci ci ma byt tabulka zobrazena, alebo nie - typ bool.
        /// </summary>
        private bool aShow;

        /// <summary>
        /// Atribut, ktory reprezentuje nazov Tooltipu - Poradie Hraca - typ string
        /// </summary>
        private string aTTPlace;

        /// <summary>
        /// Atribut, ktory reprezentuje nazov Tooltipu - Prvy Hrac - typ string
        /// </summary>
        private string aTTFirstPlayer;

        /// <summary>
        /// Atribut, ktory reprezentuje nazov Tooltipu - Druhy Hrac - typ string
        /// </summary>
        private string aTTSecondPlayer;

        /// <summary>
        /// Atribut, ktory reprezentuje nazov Tooltipu - Body Hracaa - typ string
        /// </summary>
        private string aTTPoints;

        /// <summary>
        /// Atribut, ktory reprezentuje nazov Tooltipu - Cas Hraca - typ string
        /// </summary>
        private string aTTTime;

        public bool Show
        {
            get => aShow;
            set => aShow = value;
        }

        public ScoreboardUI(LogicGame parGame) : base(parGame)
        {
            aLogicGame = parGame;
            DefineLegendTooltip("Poradie", "Hrac c.1", "Hrac c.2", "Body", "Cas");
        }

        /// <summary>
        /// Metoda, ktora inicializuje polozky hodnotiacej tabulky
        /// </summary>
        /// <param name="parScoreboardItems"></param>
        public void InitScoreboardItems(List<ScoreboardItems> parScoreboardItems)
        {
            aScoreboardItems = parScoreboardItems;
            aSize = new Vector2(1920 * (5 / 7F),  1080 * (5 / 7F));
            aPosition = new Vector2((1920 - aSize.X) / 2F, (1080 - aSize.Y) / 3.5F);
        }

        public override void Initialize()
        {
     
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {

            if (aScoreboardItems != null && aShow)
            {
                Vector2 tmpRowPosition = new Vector2(aPosition.X, aPosition.Y + aSize.Y * 1 / 11);
                Vector2 tmpAlignVector = new Vector2(0, 0);
                float tmpToolTipOffset = aSize.X * 1 / 5F;

                //Vykreslenie legendy - Poradie, Hrac 1, Hrac2, Body, Cas

                tmpAlignVector.X = tmpToolTipOffset / 2F - CalcCenterOffsetX(aTTPlace); //Najprv vypocitame tkzv. Align Vector, ktory zarovna polozku do stredu
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aTTPlace, tmpRowPosition + tmpAlignVector, Color.LightGray, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                tmpRowPosition.X += tmpToolTipOffset;

                tmpAlignVector.X = tmpToolTipOffset / 2F - CalcCenterOffsetX(aTTFirstPlayer);
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aTTFirstPlayer, tmpRowPosition + tmpAlignVector, Color.LightGray, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);;
                tmpRowPosition.X += tmpToolTipOffset;

                tmpAlignVector.X = tmpToolTipOffset / 2F - CalcCenterOffsetX(aTTSecondPlayer);
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aTTSecondPlayer, tmpRowPosition + tmpAlignVector, Color.LightGray, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                tmpRowPosition.X += tmpToolTipOffset;

                tmpAlignVector.X = tmpToolTipOffset / 2F - CalcCenterOffsetX(aTTPoints);
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aTTPoints, tmpRowPosition + tmpAlignVector, Color.LightGray, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                tmpRowPosition.X += tmpToolTipOffset;

                tmpAlignVector.X = tmpToolTipOffset / 2F - CalcCenterOffsetX(aTTTime);
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aTTTime, tmpRowPosition + tmpAlignVector, Color.LightGray, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);

                Vector2 tmpUnderLinePosition = new Vector2(aPosition.X, tmpRowPosition.Y);
                tmpUnderLinePosition.Y += aSize.Y * 0.1F / 11;

                //Vykreslenie podciarkovnikov
                int tmpUnderLinePos = 0;
                while (tmpUnderLinePosition.X <= aPosition.X + aSize.X)
                {
                    aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, "_", tmpUnderLinePosition, Color.LightGray, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                    tmpUnderLinePosition.X += tmpUnderLinePos;
                    tmpUnderLinePos = (int)aLogicGame.Font28.MeasureString("_").X;
                }

                tmpRowPosition.X = aPosition.X;
                tmpRowPosition.Y += aSize.Y * 1 / 11;

                //Vykreslenie vyslednej hodnotiacej tabulky
                for (int i = 0; i < aScoreboardItems.Count; i++)
                {

                    tmpRowPosition.X = aPosition.X;
                    
                    // i - Poradie
                    // aScoreboardItems[i].FirstPlayerName;
                    // aScoreboardItems[i].SecondPlayerName;
                    // aScoreboardItems[i].Points;
                    // aScoreboardItems[i].Time;
                    float tmpItemOffsetX = aSize.X * 1 / 5F;


                    tmpAlignVector.X = tmpItemOffsetX / 2F - CalcCenterOffsetX((i + 1).ToString()); //Najprv vypocitame tkzv. Align Vector, ktory zarovna polozku do stredu
                    aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, (i+1).ToString(), tmpRowPosition + tmpAlignVector, ColorizeByPlace(i + 1), 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F); //Vykreslime polozku
                    tmpRowPosition.X += tmpItemOffsetX; //A posunieme poziciu riadku doprava o specifikovany offset

                    tmpAlignVector.X = tmpItemOffsetX / 2F - CalcCenterOffsetX(aScoreboardItems[i].FirstPlayerName);
                    aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aScoreboardItems[i].FirstPlayerName ,tmpRowPosition + tmpAlignVector, ColorizeByPlace(i+1), 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                    tmpRowPosition.X += tmpItemOffsetX;

                    tmpAlignVector.X = tmpItemOffsetX / 2F - CalcCenterOffsetX(aScoreboardItems[i].SecondPlayerName);
                    aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aScoreboardItems[i].SecondPlayerName , tmpRowPosition + tmpAlignVector, ColorizeByPlace(i + 1), 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                    tmpRowPosition.X += tmpItemOffsetX;

                    tmpAlignVector.X = tmpItemOffsetX / 2F - CalcCenterOffsetX(aScoreboardItems[i].Points.ToString());
                    aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aScoreboardItems[i].Points.ToString() , tmpRowPosition + tmpAlignVector, ColorizeByPlace(i + 1), 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                    tmpRowPosition.X += tmpItemOffsetX;

                    tmpAlignVector.X = tmpItemOffsetX / 2F - CalcCenterOffsetX(aScoreboardItems[i].Time);
                    aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aScoreboardItems[i].Time , tmpRowPosition + tmpAlignVector, ColorizeByPlace(i + 1), 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                    tmpRowPosition.Y += aSize.Y * 1/ 11;
                }
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
        /// <summary>
        /// Metoda, ktora si vypocita offset itemu aby sa zarovnal do stredu.
        /// </summary>
        public float CalcCenterOffsetX(string parString)
        {
            return aLogicGame.Font28.MeasureString(parString).X / 2F;
        }

        /// <summary>
        /// Metoda, ktora specifikuje tooltip k jednotlivym statistikam
        /// </summary>
        public void DefineLegendTooltip(string parPlace, string parPlayer1, string parPlayer2, string parPoints, string parTime)
        {
            aTTPlace = parPlace;
            aTTFirstPlayer = parPlayer1;
            aTTSecondPlayer = parPlayer2;
            aTTPoints = parPoints;
            aTTTime = parTime;
        }

        /// <summary>
        /// Metoda, ktora specifikuje jednotlivym umiestneniam farbu podla ich umiestnenia
        /// </summary>
        /// <returns>Vrati farbu podla specifikovaneho umiestnenia</returns>
        public Color ColorizeByPlace(int parPlace)
        {
            switch (parPlace)
            {
                case 1:
                    return Color.Gold;
                case 2:
                    return Color.Silver;
                case 3:
                    return Color.OrangeRed;
                default:
                    return Color.White;

            }
        }
    }
}
