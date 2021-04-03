using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{
    class WrongAnswerIndicator : DrawableGameComponent
    {
        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame
        /// </summary>
        private LogicGame aGame;

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
        private Texture2D aIndicatorTexture2D;


        /// <summary>
        /// Atribut, reprezentujuci rectangle objektu - typ Rectangle
        /// </summary>
        private Rectangle aIndicatorRectangle;

        /// <summary>
        /// Atribut, ktory reprezentuje, ci sa ma indikator zobrazit - typ bool
        /// </summary>
        private bool aShow;

        /// <summary>
        /// Atribut, ktory reprezentuje casovac, prejdenych milisekund - typ float
        /// </summary>
        private float aHelperTimer;

        /// <summary>
        /// Atribut, ktory reprezentuje priehladnost indikatora - typ float
        /// </summary>
        private float aOpacity;


        public WrongAnswerIndicator(LogicGame parGame, Vector2 parPosition, Vector2 parSize) : base(parGame)
        {
            aGame = parGame;
            aPositionVector2 = parPosition;
            aSizeVector2 = parSize;
            aOpacity = 1F;
            aShow = false;
            aHelperTimer = 0F;
        }

        public bool Show
        {
            get => aShow;
            set => aShow = value;
        }

        public override void Initialize()
        {

            aIndicatorRectangle = new Rectangle(0, 0, (int) aSizeVector2.X, (int) aSizeVector2.Y);
            aIndicatorTexture2D = new Texture2D(aGame.GraphicsDevice, (int)aSizeVector2.X, (int)aSizeVector2.Y); //Inicializujeme si Texture2D ako Pozadie Inputu

            base.Initialize();
        }

        protected override void LoadContent()
        {
            aIndicatorTexture2D = aGame.Content.Load<Texture2D>("Sprites\\Items\\wrongIndicator");
            base.LoadContent();
        }


        public override void Draw(GameTime gameTime)
        {
            if (aShow)
            {
                aHelperTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (aHelperTimer > 32)
                {
                    aOpacity -= 0.02F;
                }

                if (aOpacity <= 0F)
                {
                    aShow = false;
                    aOpacity = 1F;
                    aHelperTimer = 0F;
                }

                aGame.SpriteBatch.Draw(aIndicatorTexture2D, aPositionVector2, aIndicatorRectangle, Color.White * aOpacity, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.3F);
            }

            base.Draw(gameTime);
        }

    }
}
