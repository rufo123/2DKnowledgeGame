using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.ClientSide.MathProblem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{
    public class MathProblem : DrawableGameComponent
    {

        private LogicGame aLogicGame;

        private Vector2 aPosition;

        private Vector2 aSize;

        private int aCountOfProblems;

        private Texture2D aBananaTexture;

        private Rectangle aBananaRectangle;

        private int aFirstNumber;

        private int aSecondNumber;

        private Texture2D aFrameTexture;

        private Rectangle aFrameRectangle;

        private int aBananaSize;

        private MathEquation aEquation;

        private bool aShown = false;

        public bool Shown
        {
            get => aShown;
            set => aShown = value;
        }


        public MathProblem(LogicGame parGame, Vector2 parPositionVector2, Vector2 parSizeVector2, int parCountProblemsGenerate, int parBananaSize) : base(parGame)
        {
            aLogicGame = parGame;
            aPosition = parPositionVector2;
            aSize = parSizeVector2;
            aCountOfProblems = parCountProblemsGenerate;
            aBananaSize = parBananaSize;
            aShown = false;
            aEquation = null;

            Random tmpRand = new Random();
            aFirstNumber = 0;
            aSecondNumber = 0;

            aFrameRectangle = new Rectangle(0, 0, (int)parSizeVector2.X, (int)parSizeVector2.Y);



        }

        public void ChangeEquation(MathEquation parEquation)
        {
            aFirstNumber = parEquation.FirstNumber;
            aSecondNumber = parEquation.SecondNumber;
        }

        public override void Initialize()
        {
            aBananaTexture = new Texture2D(aLogicGame.GraphicsDevice, aBananaSize, aBananaSize);
            aBananaRectangle = new Rectangle(0, 0, aBananaSize, aBananaSize);

            aFrameTexture = new Texture2D(aLogicGame.GraphicsDevice, (int)aSize.X, (int)aSize.Y);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            aBananaTexture = aLogicGame.Content.Load<Texture2D>("Sprites\\Items\\banana");


            Color[] tmpColor = new Color[aFrameTexture.Width * aFrameTexture.Height]; //Namapujeme oblast, ktora bude reprezentovat farbu
            {
                for (int i = 0; i < tmpColor.Length; i++) //Pre celu oblast
                {
                    tmpColor[i] = Color.Black; //Nastavime Farbu na Ciernu
                }

                aFrameTexture.SetData<Color>(tmpColor); //Samozrejme nastavime Data o Farbe pre Dummy Texturu

                base.LoadContent();
            }

        }

        public override void Draw(GameTime gameTime)
        {

            if (aShown == true)
            {

                Color tmpColor = Color.White;
                Vector2 tmpNewBananaPosition = aPosition;

                float tmpOffset = 0;


                aLogicGame.SpriteBatch.Draw(aFrameTexture, aPosition, aFrameRectangle, tmpColor * 0.2F, 0F,
                    Vector2.Zero, 1F, SpriteEffects.None, 0.3F);

                tmpNewBananaPosition.Y = aPosition.Y + 50;

                tmpOffset = ((20 - aFirstNumber) * (aBananaSize / 1.5F)) / 2;


                for (int i = 0; i < aFirstNumber; i++)
                {
                    tmpNewBananaPosition.X = aPosition.X + tmpOffset + (aBananaSize / 1.5F) * i;

                    if (i % 2 == 0)
                    {
                        tmpColor = Color.Turquoise;
                    }
                    else
                    {
                        tmpColor = Color.White;
                    }

                    aLogicGame.SpriteBatch.Draw(aBananaTexture, tmpNewBananaPosition, aBananaRectangle, tmpColor, 0F,
                        Vector2.Zero, 0.7F, SpriteEffects.None, 0.2F);
                }

                tmpNewBananaPosition.Y = aPosition.Y + 200;

                tmpOffset = ((20 - aSecondNumber) * (aBananaSize / 1.5F)) / 2;

                for (int i = 0; i < aSecondNumber; i++)
                {

                    if (i % 2 == 0)
                    {
                        tmpColor = Color.Turquoise;
                    }
                    else
                    {
                        tmpColor = Color.White;
                    }

                    tmpNewBananaPosition.X = aPosition.X + tmpOffset + (aBananaSize / 1.5F) * i;

                    aLogicGame.SpriteBatch.Draw(aBananaTexture, tmpNewBananaPosition, aBananaRectangle, tmpColor, 0F,
                        Vector2.Zero, 0.7F, SpriteEffects.None, 0.2F);
                }
            }

            //aLogicGame.SpriteBatch.DrawString(aLogicGame.Font, "Test", aPosition, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
            base.Draw(gameTime);
        }
    }
}
