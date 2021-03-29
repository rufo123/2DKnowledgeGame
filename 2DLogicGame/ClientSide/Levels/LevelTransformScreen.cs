using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.ClientSide.Levels
{
    class LevelTransformScreen : DrawableGameComponent
    {

        private Vector2 aPositionScreenVector2;

        private Vector2 aSizeScreenVector2;

        private Texture2D aScreenTexture;

        private Rectangle aScreenRectangle;

        private LogicGame aGame;

        private float aVisibility;

        private bool aNeedsFadeOut;

        private bool aNeedsFadeIn;

        private float aTransitionTimer;

        private float aCameraOffset;

        public float CameraOffset
        {
            get => aCameraOffset;
            set => aCameraOffset = value;
        }

        public bool NeedsFadeOut
        {
            get => aNeedsFadeOut;
            set => aNeedsFadeOut = value;
        }

        public bool NeedsFadeIn
        {
            get => aNeedsFadeIn;
            set => aNeedsFadeIn = value;
        }

        public LevelTransformScreen(LogicGame parGame) : base(parGame)
        {
            aGame = parGame;
            aTransitionTimer = 0F;
        }
        public override void Initialize()
        {
            aPositionScreenVector2 = new Vector2(aGame.GraphicsDevice.Viewport.X, aGame.GraphicsDevice.Viewport.Y); //Zaciatocna pozicia ViewPortu
            aSizeScreenVector2 = new Vector2(aGame.GraphicsDevice.Adapter.CurrentDisplayMode.Width* aGame.Scale + aGame.CameraX, aGame.GraphicsDevice.Adapter.CurrentDisplayMode.Height * aGame.Scale); //Velkost ViewPortu
            aCameraOffset = 0F;
            aNeedsFadeIn = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            aScreenTexture = new Texture2D(aGame.GraphicsDevice, (int)aSizeScreenVector2.X, (int)aSizeScreenVector2.Y);
            aScreenRectangle = new Rectangle((int)aPositionScreenVector2.X, (int)aPositionScreenVector2.Y, (int)aSizeScreenVector2.X, (int)aSizeScreenVector2.Y);

            Color[] tmpColor = new Color[aScreenTexture.Width * aScreenTexture.Height]; //Namapujeme oblast, ktora bude reprezentovat farbu
            {
                for (int i = 0; i < tmpColor.Length; i++) //Pre celu oblast
                {
                    tmpColor[i] = Color.Black; //Nastavime Farbu na Ciernu
                }
                aScreenTexture.SetData<Color>(tmpColor); //Samozrejme nastavime Data o Farbe pre Dummy Texturu

                base.LoadContent();

            }

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (aNeedsFadeOut)    //Fade Out - Do Tmava
            {
                aTransitionTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (aTransitionTimer > 16)
                {
                    aVisibility += 0.02F; //Postupne tmavnutie obrazu
                    aTransitionTimer = 0F;
                }

                if (aVisibility >= 1F) 
                {
                    aVisibility = 1F;
                    aNeedsFadeOut = false;
                }
            }
            else if (aNeedsFadeIn && aVisibility > 0F) //Fade In - Do Obrazu
            {
                aTransitionTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (aTransitionTimer > 16)
                {
                    aVisibility -= 0.02F; //Postupne vyjasnovanie obrazu
                    aTransitionTimer = 0F;
                }

                if (aVisibility <= 0F)
                {
                    aVisibility = 0F;
                }
            }

            Vector2 tmpNewPosition = aPositionScreenVector2;
            tmpNewPosition.X -= aCameraOffset;

            aGame.SpriteBatch.Draw(aScreenTexture, tmpNewPosition, aScreenRectangle, Color.Black * aVisibility, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
           


            base.Draw(gameTime);
        }




    }
}
