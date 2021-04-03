using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DLogicGame.GraphicObjects
{
    class QuestionsColorFeedBack : DrawableGameComponent
    {

        /// <summary>
        /// Atribut, reprezentujuci poziciu objektu - typ Vector2
        /// </summary>
        private Vector2 aPositionVector2;

        /// <summary>
        /// Atribut, reprezentujuci velkost objektu - typ Vector2
        /// </summary>
        private Vector2 aSizeVector2;

        /// <summary>
        /// Atribut, reprezentujuci texturu objektu - typ Texture2D
        /// </summary>
        private Texture2D aFeedBackTexture2D;

        /// <summary>
        /// Atribut, reprezentujuci rectangle objektu - typ Rectangle
        /// </summary>
        private Rectangle aFeedBackRectangle;

        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut, reprezentujuci casovac, na aku dlhu dobu sa ma zobrazit FeedBack - typ float
        /// </summary>
        private float aShowTimer;

        /// <summary>
        /// Atribut, reprezentujuci hodnotu boolean, ktora specifikuje, ci ma byt FeedBack zobrazeny alebo nie - typ bool
        /// </summary>
        private bool aShow;

        /// <summary>
        /// Atribut, reprezentujuci viditelnost FeedBacku - typ float
        /// </summary>
        private float aVisibility;

        /// <summary>
        /// Atribut, reprezentujuci momentalnu farbu Feedbacku - typ Color
        /// </summary>
        private Color aColor;

        /// <summary>
        /// Atribut, reprezentujuci ci sa ma FeedBack postupne zobrazit, ak nie postupne sa schova - typ bool
        /// </summary>
        private bool aFadeIn;

        /// <summary>
        /// Atribut, reprezentujuci ci je povolene zobrazovanie objektu - typ bool
        /// </summary>
        private bool aShowEnabled;
        

        public bool Show
        {
            get => aShow;
            set => aShow = value;
        }
        public Color Color
        {
            get => aColor;
            set => aColor = value;
        }
        public bool ShowEnabled
        {
            get => aShowEnabled;
            set => aShowEnabled = value;
        }

        public QuestionsColorFeedBack(LogicGame parLogicGame, Vector2 parPosition, Vector2 parSize) : base(parLogicGame)
        {
            aPositionVector2 = parPosition;
            aSizeVector2 = parSize;
            aLogicGame = parLogicGame;
            aVisibility = 0F;
            aColor = Color.White;
            aFadeIn = true;

        }

        public override void Initialize()
        {
            aFeedBackRectangle = new Rectangle(0, 0, (int)aSizeVector2.X, (int)aSizeVector2.Y);
            aFeedBackTexture2D = new Texture2D(aLogicGame.GraphicsDevice, (int)aSizeVector2.X, (int)aSizeVector2.Y);

            base.Initialize();
        }

        protected override void LoadContent()
        {

            Color[] tmpColorFeedBack = new Color[aFeedBackTexture2D.Width * aFeedBackTexture2D.Height]; //Farba pre pozadie boxu otazok
            {
                for (int i = 0; i < tmpColorFeedBack.Length; i++) //Pre celu oblast
                {
                    tmpColorFeedBack[i] = Color.White; //Nastavime Farbu na Bielu
                }
                aFeedBackTexture2D.SetData<Color>(tmpColorFeedBack); //Samozrejme nastavime Data o Farbe pre Dummy Texturu
            }

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (aLogicGame.CheckKeyPressedOnce(Keys.Z))
            {
                aShow = true;
                aColor = Color.Red;
            }

            if (aShow)
            {
                aLogicGame.SpriteBatch.Draw(aFeedBackTexture2D, aPositionVector2, aFeedBackRectangle, this.Color * aVisibility, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.3F);
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (aShowEnabled && aShow)
            {
                aShowTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (aShowTimer > 32)
                {

                    if (aFadeIn)
                    {
                        aVisibility += 0.015F; //Postupne zobrazujeme FeedBack
                    }
                    else
                    {
                        aVisibility -= 0.015F; //Postupne schovavame FeedBack
                    }

                    if (aVisibility <= 0)
                    {
                        aShow = false;
                        aFadeIn = true;
                        aVisibility = 0F;

                    }
                    else if (aVisibility >= 1F)
                    {
                        aFadeIn = false;
                    }

                }

               
            }

            base.Update(gameTime);
        }
    }
}
