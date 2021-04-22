using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.ClientSide.Levels
{
    /// <summary>
    /// Trieda, ktora reprezentuje potvrdzovaciu obrazovku pre vychod z urovne.
    /// </summary>
    public class LevelQuitConfirm : DrawableGameComponent
    {
        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame.
        /// </summary>
        private LogicGame aGame;

        /// <summary>
        /// Atribut, reprezentujuci ci sa ma potvrdenie zobrazit alebo nie, typ bool.
        /// </summary>
        private bool aShowConfirm;

        /// <summary>
        /// Atribut, reprezentujuci poziciu confirm boxu - typ Vector2.
        /// </summary>
        private Vector2 aPosition;

        /// <summary>
        /// Atribut, reprezentujuci velkost confirm boxu - typ Vector2.
        /// </summary>
        private Vector2 aSizeOfBackground;

        /// <summary>
        /// Atribut, reprezentujuci texturu confirm boxu - typ Texture2D.
        /// </summary>
        private Texture2D aTexture;

        /// <summary>
        /// Atribut, reprezentujuci Rectangle confirm boxu - typ Rectangle.
        /// </summary>
        private Rectangle aRectangle;

        public bool ShowConfirm
        {
            get => aShowConfirm;
            set => aShowConfirm = value;
        }

        /// <summary>
        /// Konstruktor triedy potvrdzovania vystupu z urovne.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parSize">Parameter, reprezentujuci velkost - typ Vector2.</param>
        public LevelQuitConfirm(LogicGame parGame, Vector2 parPosition, Vector2 parSize) : base(parGame)
        {
            aGame = parGame;
            aShowConfirm = false;
            aPosition = parPosition;
            aSizeOfBackground = parSize;
        }

        /// <summary>
        /// Override metoda, ktora sa stara o incializaciu textury a rectanglu.
        /// </summary>
        public override void Initialize()
        {
            aTexture = new Texture2D(aGame.GraphicsDevice, (int)aSizeOfBackground.X, (int)aSizeOfBackground.Y);
            aRectangle = new Rectangle(0, 0, (int)aSizeOfBackground.X, (int)aSizeOfBackground.Y);
            base.Initialize();
        }

        /// <summary>
        /// Override metoda, ktora sa stara o nacitanie farieb textury - konkretne ciernej farby.
        /// </summary>
        protected override void LoadContent()
        {
            Color[] tmpColor = new Color[aRectangle.Width * aRectangle.Height]; //Namapujeme oblast, ktora bude reprezentovat farbu Textury
            {
                for (int i = 0; i < tmpColor.Length; i++) //Pre celu oblast
                {
                    tmpColor[i] = Color.Black; //Nastavime Farbu na Ciernu
                }
                aTexture.SetData<Color>(tmpColor); //Samozrejme nastavime Data o Farbe pre Dummy Texturu

            }
            base.LoadContent();
        }

        /// <summary>
        /// Override metoda, ktora sa stara o vykreslovanie.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (ShowConfirm)
            {
                Vector2 tmpOffSetVector = Vector2.Zero;
                tmpOffSetVector.X = -1 * aGame.CameraX + (-1 * aSizeOfBackground.X / 2F);
                tmpOffSetVector.Y = -aSizeOfBackground.Y / 2F;

                Vector2 tmpTextOffSetVector = Vector2.Zero;

                aGame.SpriteBatch.Draw(aTexture, aPosition + tmpOffSetVector, aRectangle, Color.White * 0.50F, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.19F);

                tmpTextOffSetVector.Y = (-1 * aGame.Font28.MeasureString("Y").Y / 2F) - (aSizeOfBackground.Y / 4F); //Y-ovu suradnicu staci vypocitat z jedneho znaku.
                tmpTextOffSetVector.X = -1 * aGame.CameraX - aGame.Font28.MeasureString("ESC - Menu").X / 2F; 
                aGame.SpriteBatch.DrawString(aGame.Font28, "ESC - Menu", aPosition + tmpTextOffSetVector, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.18F);

                tmpTextOffSetVector.Y = (-1 * aGame.Font28.MeasureString("Y").Y / 2F) + (aSizeOfBackground.Y / 4F); //Y-ovu suradnicu staci vypocitat z jedneho znaku.
                tmpTextOffSetVector.X = -1 * aGame.CameraX - aGame.Font28.MeasureString(aGame.ProceedKey.ToString() + " - Pokracovat").X / 2F;
                aGame.SpriteBatch.DrawString(aGame.Font28, aGame.ProceedKey.ToString() + " - Pokracovat", aPosition + tmpTextOffSetVector, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.18F);
            }

            base.Draw(gameTime);
        }

    }
}
