using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.ClientSide.Levels
{
    class LevelPointsCounter : DrawableGameComponent
    {

        /// <summary>
        /// Atribut, ktory reprezentuje hru - typ LogicGame
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut, ktory reprezentuje poziciu - typ Vector2
        /// </summary>
        private Vector2 aPosition;

        /// <summary>
        /// Atribut, ktory reprezentuje velkost - typ Vector2
        /// </summary>
        private Vector2 aSize;

        /// <summary>
        /// Atribut, ktory reprezentuje pozadie framu bodov - typ Texture2D
        /// </summary>
        private Texture2D aPointsFrameTexture2D;

        /// <summary>
        /// Atribut, ktory reprezentuje Rectangle framu bodov - typ Rectangle
        /// </summary>
        private Rectangle aPointsRectangle;

        /// <summary>
        /// Atribut, ktory reprezentuje Texturu bananu - typ Texture2D
        /// </summary>
        private Texture2D aBananaTexture2D;

        /// <summary>
        /// Atribut, ktory reprezentuje Rectangle bananu - typ Rectangle
        /// </summary>
        private Rectangle aBananaRectangle;

        /// <summary>
        /// Atribut, ktory reprezentuje skalu pisma - typ float
        /// </summary>
        private float aFontScale;

        /// <summary>
        /// Atribut, reprezentujuci body - typ int
        /// </summary>
        private int aPoints;

        public int Points
        {
            get => aPoints;
            set => aPoints = value;
        }

        public LevelPointsCounter(LogicGame parLogicGame, Vector2 parPosition, Vector2 parSize) : base(parLogicGame)
        {
            aLogicGame = parLogicGame;
            aPosition = parPosition;
            aSize = parSize;
            aPoints = 0;
            aFontScale = 0.8F;
        }

        public override void Initialize()
        {
            aPointsRectangle = new Rectangle(0, 0, (int)aSize.X, (int)aSize.Y);
            aPointsFrameTexture2D = new Texture2D(aLogicGame.GraphicsDevice, (int)aSize.X, (int)aSize.Y);

            aBananaRectangle = new Rectangle(0, 0, 64, 64);
            aBananaTexture2D = new Texture2D(aLogicGame.GraphicsDevice, 64, 64);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            aBananaTexture2D = aLogicGame.Content.Load<Texture2D>("Sprites\\Items\\banana");

            Color[] tmpColor = new Color[aPointsFrameTexture2D.Width * aPointsFrameTexture2D.Height]; //Namapujeme oblast, ktora bude reprezentovat farbu Textury
            {
                for (int i = 0; i < tmpColor.Length; i++) //Pre celu oblast
                {
                    tmpColor[i] = Color.Black; //Nastavime Farbu na Ciernu
                }
                aPointsFrameTexture2D.SetData<Color>(tmpColor); //Samozrejme nastavime Data o Farbe pre Dummy Texturu

            }
            base.LoadContent();
        }

        /// <summary>
        /// Metoda, ktora umozni nastavit body
        /// </summary>
        /// <param name="parPoints">Parameter, reprezentujuci pocet bodov, ktore sa maju nastavit - typ int</param>
        public void SetPoints(int parPoints)
        {
            aPoints = parPoints;
        }

        public override void Draw(GameTime gameTime)
        {
            string tmpNewPointsString = aPoints.ToString("00") + " * ";

            Vector2 tmpNewFontPosition = new Vector2();
            tmpNewFontPosition = aPosition;
            tmpNewFontPosition.X += (aLogicGame.Font48.MeasureString(aPoints.ToString()).X * aFontScale) / 2F;
            tmpNewFontPosition.Y += (aSize.Y - (aLogicGame.Font48.MeasureString(aPoints.ToString()).Y * aFontScale)) / 3F;

            Vector2 tmpNewBananaPosition = new Vector2();
            tmpNewBananaPosition = aPosition;
            tmpNewBananaPosition.X += aLogicGame.Font48.MeasureString(tmpNewPointsString).X * aFontScale;
            tmpNewBananaPosition.Y += (aSize.Y - aBananaRectangle.Size.Y * aFontScale) / 2F;

            aLogicGame.SpriteBatch.Draw(aBananaTexture2D, tmpNewBananaPosition, aBananaRectangle, Color.White, 0F, Vector2.Zero, aFontScale, SpriteEffects.None, 0.1F);

            aLogicGame.SpriteBatch.Draw(aPointsFrameTexture2D, aPosition, aPointsRectangle, Color.White * 0.5F, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.2F);

            aLogicGame.SpriteBatch.DrawString(aLogicGame.Font48, tmpNewPointsString, tmpNewFontPosition, Color.White, 0f, Vector2.Zero, aFontScale, SpriteEffects.None, 0F);

            base.Draw(gameTime);
        }
    }
}
