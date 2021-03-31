using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.ClientSide.Levels
{
    class LevelTransformScreen : DrawableGameComponent
    {
        /// <summary>
        /// Atribut, reprezentujuci zaciatocnu poziciu obrazovky - typ Vector2
        /// </summary>
        private Vector2 aPositionScreenVector2;

        /// <summary>
        /// Atribut, reprezentujuci, velkost obrazovky - typ Vector2
        /// </summary>
        private Vector2 aSizeScreenVector2;

        /// <summary>
        /// Atribut, reprezentujuci Texturu - typ Texture2D
        /// </summary>
        private Texture2D aScreenTexture;

        /// <summary>
        /// Atribut, reprezentujuci Stvorec Screenu - typ Rectangle
        /// </summary>
        private Rectangle aScreenRectangle;

        /// <summary>
        /// Atribut, reprezentujuci Hru - typ LogicGame
        /// </summary>
        private LogicGame aGame;

        /// <summary>
        /// Atribut, reprezentujuci viditelnost textury - typ float
        /// </summary>
        private float aVisibility;

        /// <summary>
        /// Atribut, ktory reprezentuje, ci je potrebne vyblednutie obrazovky - typ bool
        /// </summary>
        private bool aNeedsFadeOut;

        /// <summary>
        /// Atribut, ktory reprezentuje, ci je potrebne ztmavnutie obrazovky - typ bool
        /// </summary>
        private bool aNeedsFadeIn;

        /// <summary>
        /// Atribut, ktory reprezentuje casovac pri tranzicii - typ float
        /// </summary>
        private float aTransitionTimer;

        /// <summary>
        /// Atribut, pomocny, ktory pomaha posunut TransformScreen jemne do prava, aby pokryl celu obrazovky, zahrnujuc offset kamery - typ float
        /// </summary>
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

        /// <summary>
        /// Konstruktor, LevelTransformScreenu - Inicializuje sa Hra a TransitionTimer sa nastavi na 0F
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru</param>
        public LevelTransformScreen(LogicGame parGame) : base(parGame)
        {
            aGame = parGame;
            aTransitionTimer = 0F;
        }


        /// <summary>
        /// Metoda - Inicializacia - Inicializuje sa Pozicia, ScreenSize, CameraOffset sa nastavi na 0F a NeedsFadeIn sa nastavi na true, ako keby bolo treba vyblednutie, ale uz obrazovka vyblednuta bude, tak sa nic nestane.
        /// </summary>
        public override void Initialize()
        {
            aPositionScreenVector2 = new Vector2(aGame.GraphicsDevice.Viewport.X, aGame.GraphicsDevice.Viewport.Y); //Zaciatocna pozicia ViewPortu
            aSizeScreenVector2 = new Vector2(aGame.GraphicsDevice.Adapter.CurrentDisplayMode.Width* aGame.Scale + aGame.CameraX, aGame.GraphicsDevice.Adapter.CurrentDisplayMode.Height * aGame.Scale); //Velkost ViewPortu
            aCameraOffset = 0F;
            aNeedsFadeIn = true;
            base.Initialize();
        }

        /// <summary>
        /// Metoda - LoadContent - Nacita Texturu, Rectangle a Texturu vyplni ciernou farbou
        /// </summary>
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


        /// <summary>
        /// Metoda - Draw - Ak je potrebne vyblednutie, pracuje s Timerom a postupne vybledne celu obrazovku, podobne aj s zatmavenim
        /// </summary>
        /// <param name="gameTime"></param>
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
