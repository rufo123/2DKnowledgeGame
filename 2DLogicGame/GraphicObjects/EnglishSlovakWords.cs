using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DLogicGame.GraphicObjects
{
    class EnglishSlovakWords : DrawableGameComponent



    {

        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut, reprezentujuci polohu slovenskeho slova - typ Vector2
        /// </summary>
        private Vector2 aPositionSlovak;

        /// <summary>
        /// Atribut, reprezentujuci polohu anglickeho slova - typ Vector2
        /// </summary>
        private Vector2 aPositionEnglish;

        /// <summary>
        /// Atribut, reprezentujuci velkost pozadia slov - typ Vector2
        /// </summary>
        private Vector2 aSize;

        /// <summary>
        /// Atribut, ktory reprezentuje ci je zobrazenie slovenskeho slova povolene - typ bool
        /// </summary>
        private bool aShowingEnabledSlovak;

        /// <summary>
        /// Atribut, ktory reprezentuje ci je zobrazenie anglickeho slova povolene - typ bool
        /// </summary>
        private bool aShowingEnabledEnglish;

        /// <summary>
        /// Atribut, ktory reprezentuje casovac slovenskeho slova - typ float
        /// </summary>
        private float aShowTimerSlovak;

        /// <summary>
        /// Atribut, ktory reprezentuje casovac anglickeho slova - typ float
        /// </summary>
        private float aShowTimerEnglish;

        /// <summary>
        /// Atribut, ktory reprezentuje string, reprezentujuci slovenske slovo - typ string
        /// </summary>
        private string aSlovakWord;

        /// <summary>
        /// Atribut, ktory reprezentuje string, reprezentujuci anglicke slovo - typ string
        /// </summary>
        private string aEnglishWord;

        /// <summary>
        /// Atribut, ktory reprezentuje Texturu - typ Texture2D
        /// </summary>
        private Texture2D aTexture2D;

        /// <summary>
        /// Atribut, ktory reprezentuje Rectangle - typ Rectangle
        /// </summary>
        private Rectangle aRectangle;

        public EnglishSlovakWords(LogicGame parLogicGame, Vector2 parPositionEnglish, Vector2 parPositionSlovak, Vector2 parSize) : base(parLogicGame)
        {
            aLogicGame = parLogicGame;
            aPositionEnglish = parPositionEnglish;
            aPositionSlovak = parPositionSlovak;
            aSize = parSize;
            aShowingEnabledSlovak = false;
            aShowingEnabledEnglish = false;
            aShowTimerSlovak = 0F;
            aShowTimerEnglish = 0F;
            aSlovakWord = "SLOVAK_NOT_INIT";
            aEnglishWord = "ENGLISH_NOT_INIT";
        }

        public string SlovakWord
        {
            get => aSlovakWord;
            set => aSlovakWord = value;
        }

        public string EnglishWord
        {
            get => aEnglishWord;
            set => aEnglishWord = value;
        }

        /// <summary>
        /// Metoda, ktora zobrazi objekt a zaroven sa nastavi casovac na 0
        /// </summary>
        public void Show(int parPlayerID)
        {
            if (parPlayerID == 1) //Mal by nam stacit, len jeden Show Timer, pretoze kazdy k
            {
                aShowingEnabledEnglish = true;
                aShowTimerEnglish = 0;
            }
            else if (parPlayerID == 2)
            {
                aShowingEnabledSlovak = true;
                aShowTimerSlovak = 0;
            }
        }

        public override void Initialize()
        {
            aRectangle = new Rectangle(0, 0, (int)aSize.X, (int)aSize.Y);
            aTexture2D = new Texture2D(aLogicGame.GraphicsDevice, (int) aSize.X, (int) aSize.Y);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Color[] tmpColorFeedBack = new Color[aTexture2D.Width * aTexture2D.Height]; //Farba pre pozadie boxu
            {
                for (int i = 0; i < tmpColorFeedBack.Length; i++) //Pre celu oblast
                {
                    tmpColorFeedBack[i] = Color.Black; //Nastavime Farbu na Ciernu
                }
                aTexture2D.SetData<Color>(tmpColorFeedBack); //Samozrejme nastavime Data o Farbe pre Dummy Texturu
            }

            base.LoadContent();
        }

        public override void Update(GameTime parGameTime)
        {
            if (aLogicGame.CurrentPressedKey.IsKeyDown(Keys.F))
            {
                Show(1);
            }

            if (aLogicGame.CurrentPressedKey.IsKeyDown(Keys.G))
            {
                Show(2);
            }


            if (aShowingEnabledSlovak) //Ak je zobrazenie povolene, casovac sa bude pripocitavat
            {
                aShowTimerSlovak += parGameTime.ElapsedGameTime.Milliseconds;
            }

            if (aShowingEnabledEnglish) //Ak je zobrazenie povolene, casovac sa bude pripocitavat
            {
                aShowTimerEnglish += parGameTime.ElapsedGameTime.Milliseconds;
            }


            if (aShowTimerSlovak >= 60) //Ak ubehne zadany cas, objekt sa schova a zresetuje sa casovac
            {
                aShowTimerSlovak = 0F;
                aShowingEnabledSlovak = false;
            }

            if (aShowTimerEnglish >= 60)
            {
                aShowTimerEnglish = 0F;
                aShowingEnabledEnglish = false;
            }

            base.Update(parGameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (aTexture2D != null && aRectangle != null && aShowingEnabledEnglish)
            {
                float tmpXStringOffset = aLogicGame.Font28.MeasureString(aEnglishWord).X / 2F;
                float tmpYStringOffset = aLogicGame.Font28.MeasureString(aEnglishWord).Y / 2F;
                Vector2 tmpNewStringOffSetVector2 = new Vector2(tmpXStringOffset, tmpYStringOffset);

                aLogicGame.SpriteBatch.Draw(aTexture2D, aPositionEnglish, aRectangle, Color.White * 0.2F, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.3F);
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aEnglishWord, aPositionEnglish + aSize / 2F - tmpNewStringOffSetVector2, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.2F);
            }

            if (aTexture2D != null && aRectangle != null && aShowingEnabledSlovak)
            {
                float tmpXStringOffset = aLogicGame.Font28.MeasureString(aSlovakWord).X / 2F;
                float tmpYStringOffset = aLogicGame.Font28.MeasureString(aSlovakWord).Y / 2F;
                Vector2 tmpNewStringOffSetVector2 = new Vector2(tmpXStringOffset, tmpYStringOffset);

                aLogicGame.SpriteBatch.Draw(aTexture2D, aPositionSlovak, aRectangle, Color.White * 0.2F, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.3F);
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aSlovakWord, aPositionSlovak + aSize / 2F - tmpNewStringOffSetVector2, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.2F);
            }

            base.Draw(gameTime);
        }
    }
}
