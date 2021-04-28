using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.ClientSide.MathProblem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Trieda reprezentujuci graficku cast manazera matematickeho problemu. - Klient.
    /// </summary>
    public class MathProblem : DrawableGameComponent
    {
        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame.
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut, reprezentujuci poziciu - typ Vector2.
        /// </summary>
        private Vector2 aPosition;

        /// <summary>
        /// Atribut, reprezentujuci velkost - typ Vector2.
        /// </summary>
        private Vector2 aSize;

        /// <summary>
        /// Atribut, reprezentujuci texturu bananov - typ Texture2D.
        /// </summary>
        private Texture2D aBananaTexture;

        /// <summary>
        /// Atribut, reprezentujuci rectangle bananov - typ Rectangle.
        /// </summary>
        private Rectangle aBananaRectangle;

        /// <summary>
        /// Atribut, reprezentujuci texturu pozadia objektu - typ Texture2D.
        /// </summary>
        private Texture2D aFrameTexture;

        /// <summary>
        /// Atribut, reprezentujuci rectangle pozadia objektu - typ Rectangle.
        /// </summary>
        private Rectangle aFrameRectangle;

        /// <summary>
        /// Atribut, reprezentujuci velkost bananov - typ int.
        /// </summary>
        private int aBananaSize;

        /// <summary>
        /// Parameter, reprezentujuci objekt obsahujuci informacie o rovnici - typ MathEquation.
        /// </summary>
        private MathEquation aEquation;

        /// <summary>
        /// Parameter, reprezentujuci ci ma byt objekt zobrazeny alebo nie - typ bool.
        /// </summary>
        private bool aShown = false;

        public bool Shown
        {
            get => aShown;
            set => aShown = value;
        }

        /// <summary>
        /// Konstruktor grafickej casti matematickeho problemu.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parPositionVector2">Parameter, reprezentujuci poziciu pozadia - typ Vector2.</param>
        /// <param name="parSizeVector2">Parameter, reprezentujuci velkost pozadia - typ Vector2.</param>
        /// <param name="parBananaSize">Parameter, reprezentujuci velkost bananov - typ int.</param>
        public MathProblem(LogicGame parGame, Vector2 parPositionVector2, Vector2 parSizeVector2, int parBananaSize) : base(parGame)
        {
            aLogicGame = parGame;
            aPosition = parPositionVector2;
            aSize = parSizeVector2;
            aBananaSize = parBananaSize;
            aShown = false;
            aEquation = null;

            Random tmpRand = new Random();

            aFrameRectangle = new Rectangle(0, 0, (int)parSizeVector2.X, (int)parSizeVector2.Y);



        }

        /// <summary>
        /// Metoda, ktora sa stara o zmenu rovnice.
        /// </summary>
        /// <param name="parEquation">Paramete, reprezentujuci rovnicu - typ MathEquation.</param>
        public void ChangeEquation(MathEquation parEquation)
        {
            aEquation = parEquation;
        }

        /// <summary>
        /// Override metoda, ktora sa stara o inicializaciu. Incializuje Texturu a rectangle bananu a zaroven texturu pozadia.
        /// </summary>
        public override void Initialize()
        {
            aBananaTexture = new Texture2D(aLogicGame.GraphicsDevice, aBananaSize, aBananaSize);
            aBananaRectangle = new Rectangle(0, 0, aBananaSize, aBananaSize);

            aFrameTexture = new Texture2D(aLogicGame.GraphicsDevice, (int)aSize.X, (int)aSize.Y);

            base.Initialize();
        }

        /// <summary>
        /// Override metoda, ktora sa stara o nacitanie textur bananov a textury pozadia - kde nacita ako pozadie ciernu farbu.
        /// </summary>
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

        /// <summary>
        /// Override metoda, ktora sa stara o vykreslovanie objektu.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Draw(GameTime parGameTime)
        {

            if (aShown == true)
            {

                Color tmpColor = Color.White;
                Vector2 tmpNewBananaPosition = aPosition;

                float tmpOffset = 0;


                aLogicGame.SpriteBatch.Draw(aFrameTexture, aPosition, aFrameRectangle, tmpColor * 0.2F, 0F,
                    Vector2.Zero, 1F, SpriteEffects.None, 0.3F);

                tmpNewBananaPosition.Y = aPosition.Y + aSize.Y/6;

                tmpOffset = ((20 - aEquation.FirstNumber) * (aBananaSize / 1.5F)) / 2;


                for (int i = 0; i < aEquation.FirstNumber; i++)
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

                tmpNewBananaPosition.Y = aPosition.Y + (aSize.Y/1.5F);

                tmpOffset = ((20 - aEquation.SecondNumber) * (aBananaSize / 1.5F)) / 2;

                for (int i = 0; i < aEquation.SecondNumber; i++)
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

                Vector2 tmpNewOperatorPosition = new Vector2();

                tmpNewOperatorPosition.Y = aPosition.Y + 100;
                tmpNewOperatorPosition.X = (aPosition.X + aSize.X/2 - (aLogicGame.Font72.LineSpacing * aLogicGame.Scale * 0.7F  / 2F) * 0.7F);

                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font72, aEquation.Operator.ToString(), tmpNewOperatorPosition, Color.GreenYellow, 0F, Vector2.Zero, 0.7F, SpriteEffects.None, 0.2F);
            }

            //aLogicGame.SpriteBatch.DrawString(aLogicGame.Font72, "Test", aPosition, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
            base.Draw(parGameTime);
        }
    }
}
