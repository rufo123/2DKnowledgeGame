using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace _2DLogicGame.GraphicObjects
{
    class Questions : DrawableGameComponent
    {

        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut, reprezentujuci poziciu - typ Vector2
        /// </summary>
        private Vector2 aPositon;

        /// <summary>
        /// Atribut, reprezentujuci velkost - typ Vector2
        /// </summary>
        private Vector2 aSize;

        /// <summary>
        /// Atribut, reprezentujuci text momentalnej otazky - typ string
        /// </summary>
        private string aCurrentQuestionText;

        /// <summary>
        /// Atribut, reprezentujuci vsetky otazky, ktore su na vyber
        /// </summary>
        private List<string> aPossibleAnswersList;

        private Vector2 aAnswerPosition;

        private Vector2 aAnswerSize;

        private Texture2D aQuestionsBackTexture2D;

        private Rectangle aQuestionBackRectangle;

        private Texture2D aAnswerBackTexture2D;

        private Rectangle aAnswerBackRectangle;



        public Questions(LogicGame parGame, Vector2 parPositionVector2, Vector2 parSizeVector2) : base(parGame)
        {
            aLogicGame = parGame;
            aPositon = parPositionVector2;
            aSize = parPositionVector2;
            aPossibleAnswersList = new List<string>();
            aCurrentQuestionText = "";
            aAnswerPosition = new Vector2(parPositionVector2.X + aSize.X / 20, parPositionVector2.Y + aSize.Y / 3);
            aAnswerSize = new Vector2(aSize.X / (48 / 17), aSize.Y / (16));

        }

        public override void Initialize()
        {
            aQuestionBackRectangle = new Rectangle(0, 0, (int)aSize.X, (int)aSize.Y);
            aQuestionsBackTexture2D = new Texture2D(aLogicGame.GraphicsDevice, (int)aSize.X, (int)aSize.Y);

            aAnswerBackTexture2D = new Texture2D(aLogicGame.GraphicsDevice, (int)aAnswerSize.X, (int)aAnswerSize.Y);
            aAnswerBackRectangle = new Rectangle(0, 0, (int)aAnswerSize.X, (int)aAnswerSize.Y);

            base.Initialize();
        }

        protected override void LoadContent()
        {

            Color[] tmpColorBack = new Color[aQuestionsBackTexture2D.Width * aQuestionsBackTexture2D.Height]; //Farba pre pozadie boxu otazok
            {
                for (int i = 0; i < tmpColorBack.Length; i++) //Pre celu oblast
                {
                    tmpColorBack[i] = Color.Black; //Nastavime Farbu na Ciernu
                }
                aQuestionsBackTexture2D.SetData<Color>(tmpColorBack); //Samozrejme nastavime Data o Farbe pre Dummy Texturu
            }

            Color[] tmpColorAnswer = new Color[aAnswerBackTexture2D.Width * aAnswerBackTexture2D.Height]; //Farba pre pozadie odpovedi
            {
                for (int i = 0; i < tmpColorAnswer.Length; i++) //Pre celu oblast
                {
                    tmpColorAnswer[i] = Color.Black; //Nastavime Farbu na Ciernu
                }
                aQuestionsBackTexture2D.SetData<Color>(tmpColorAnswer); //Samozrejme nastavime Data o Farbe pre Dummy Texturu
            }

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {

            aLogicGame.SpriteBatch.Draw(aQuestionsBackTexture2D, aPositon, aQuestionBackRectangle, Color.White * 0.2F, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.3F);

            for (int i = 0; i < 4; i++)
            {
                aLogicGame.SpriteBatch.Draw(aAnswerBackTexture2D, CalculateOffsetVector(i), aAnswerBackRectangle, CalculateColor(i) * 0.2F, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.3F);
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Metoda, ktora prepocita offset od pozicia v zavislosti od cisla odpovede z listu
        /// </summary>
        /// <param name="parAnsNumber">Parameter, reprezentujuci cislo odpovede z listu.</param>
        /// <returns></returns>
        public Vector2 CalculateOffsetVector(int parAnsNumber)
        {
            int tmpPosOffsetX = 0;
            int tmpPosOffsetY = 0;

            if (parAnsNumber >= 2) //Posunieme na pravu stranu
            {
                tmpPosOffsetX = (int)(aAnswerSize.X / 8.5) + (int)aAnswerSize.X;
            }

            if (parAnsNumber % 2 != 0) //Posunieme Nizsie
            {
                tmpPosOffsetY = (int)(aAnswerSize.X / 2.5) + (int)aAnswerSize.Y;
            }

            return new Vector2(aAnswerPosition.X + tmpPosOffsetX, aAnswerPosition.Y + tmpPosOffsetY);
        }

        /// <summary>
        /// Metoda, ktora vypocita farbu na zaklade indexu moznej odpovedi
        /// </summary>
        /// <param name="parAnsNumber">Parameter, reprezentujuci cislo odpovedi - typ int</param>
        /// <returns>Vrati vypocitanu farbu</returns>
        public Color CalculateColor(int parAnsNumber)
        {
            switch (parAnsNumber)
            {
                case 0:
                    return Color.Red;
                case 1:
                    return Color.Purple;
                case 2:
                    return Color.Green;
                case 3:
                    return Color.Blue;
                default:
                    return Color.White;
            }
        }

        /// <summary>
        /// Metoda, ktora nastavi mozne odpovede podla parametra
        /// </summary>
        /// <param name="parAnswersList">Parameter odpovedi - typ List<string></param>
        public void SetAnswers(List<string> parAnswersList)
        {
            if (aPossibleAnswersList != null)
            {
                aPossibleAnswersList = parAnswersList;
            }
        }
    }
}
