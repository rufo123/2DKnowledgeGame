using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame
{

    /// <summary>
    /// Enumeracna trieda, ktora reprezentuje stav indikatora hodnotiacej tabulky v menu.
    /// </summary>
    public enum IndicatorState
    {
        Off = 0,
        On = 1
    }

    /// <summary>
    /// Trieda, ktora reprezentuje indikator v menu.
    /// </summary>
    public class MenuStatsIndicator : DrawableGameComponent
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
        /// Atribut, reprezentujuci texturu statusu - typ Texture2D.
        /// </summary>
        private Texture2D aStatusTexture;

        /// <summary>
        /// Atribut, reprezentujuci texturu pozadia statusu - typ Texture2D.
        /// </summary>
        private Texture2D aStatusBackgroundTexture;

        /// <summary>
        /// Atribut, reprezentujuci rectangle statusu - typ Rectangle.
        /// </summary>
        private Rectangle aStatusRectangle;

        /// <summary>
        /// Atribut, reprezentujuci rectangle pozadia statusu - typ Rectangle.
        /// </summary>
        private Rectangle aStatusBackgroundRectangle;

        /// <summary>
        /// Atribut, reprezentujuci text statusu - typ string.
        /// </summary>
        private string aStatusText;

        /// <summary>
        /// Atribut, reprezentujuci stav indikatora - typ IndicatorState - enum.
        /// </summary>
        private IndicatorState aIndicatorState;

        /// <summary>
        /// Atribut, reprezentujuci ci je objekt povoleny - typ bool.
        /// </summary>
        private bool aEnabled;
        
        /// <summary>
        /// Atribut, reprezentuje maly informacny text pod statusom - typ string.
        /// </summary>
        private string aStatusMiniText;

        public IndicatorState IndicatorState
        {
            get => aIndicatorState;
            set => aIndicatorState = value;
        }

        public bool IndicatorEnabled
        {
            get => aEnabled;
            set => aEnabled = value;
        }

        /// <summary>
        /// Konstruktor, indikatora statusu hodnotiacej tabulky v menu.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parStatusText">Parameter, reprezentujuci text statusu - typ string.</param>
        public MenuStatsIndicator(LogicGame parGame, Vector2 parPosition, string parStatusText) : base(parGame)
        {
            aPosition = parPosition;
            aLogicGame = parGame;
            aIndicatorState = IndicatorState.Off;
            aStatusText = parStatusText;

            aStatusTexture = new Texture2D(parGame.GraphicsDevice, 230, 115);
            aStatusBackgroundTexture = new Texture2D(parGame.GraphicsDevice, 512, 128);

            aEnabled = false;

        }

        /// <summary>
        /// Metoda, ktora sluzi na nacitanie textur a rectanglov triedy.
        /// </summary>
        protected override void LoadContent()
        {

            aStatusTexture = aLogicGame.Content.Load<Texture2D>("Sprites\\Backgrounds\\menuStatIndicator");
            aStatusBackgroundTexture = aLogicGame.Content.Load<Texture2D>("Sprites\\Backgrounds\\menuStatIndicatorBCK");

            aStatusRectangle = new Rectangle(0, 0, aStatusTexture.Width / 2, aStatusTexture.Height); //Delime 2-ma pretoze su v obrazku 2 stadia indikatora.
            aStatusBackgroundRectangle = new Rectangle(0, 0, aStatusBackgroundTexture.Width, aStatusBackgroundTexture.Height);
            base.LoadContent();
        }


        /// <summary>
        /// Metoda, ktora prepocitava polohu rectangla pre indikator podla toho ci ma zobrazit ON alebo OFF state.
        /// </summary>
        /// <param name="parTurnOn">Parameter - bool, reprezentujuci ci ide o zapnutie alebo vypnutie.</param>
        public void SwitchStatusRectanglePosition(bool parTurnOn)
        {
            if (aStatusRectangle != null)
            {

                aStatusRectangle.X = parTurnOn ? aStatusRectangle.Size.X + 1 : 0;
                //Pouzijeme ternarny operator, ak sa ma zapnut, tak suradnica X, bude rovna velkosti inak suradnica X bude rovna 0.
            }
        }

        /// <summary>
        /// Metoda, ktora zapne alebo vypne indikator, resp nastavi farbu indikatora na zelenu alebo cervenu
        /// </summary>
        /// <param name="parIndicatorState">Parameter, reprezentujuci state indikatora na ktory chceme "prepnut" - typ IndicatorState - enum.</param>
        public void TurnOnOff(IndicatorState parIndicatorState)
        {
            aIndicatorState = parIndicatorState;

            switch (parIndicatorState)
            {
                case IndicatorState.Off:
                    aStatusMiniText = "Pripojenie k databaze zlyhalo (Reconnecting)!";
                    SwitchStatusRectanglePosition(false);
                    break;
                case IndicatorState.On:
                    SwitchStatusRectanglePosition(true);
                    aStatusMiniText = "Pripojenie k databaze uspesne!";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parIndicatorState), parIndicatorState, null);
            }
        }

        /// <summary>
        /// Metoda, ktora sluzi na vykreslovanie objektu triedy.
        /// </summary>
        /// <param name="parGameTime">Parameter, ktory reprezentuje GameTime.</param>
        public override void Draw(GameTime parGameTime)
        {
            if (aEnabled)
            {
                Vector2 tmpNewOffSetVector2 = new Vector2(0, 0);

                tmpNewOffSetVector2.Y = ((aStatusBackgroundRectangle.Size.Y * 0.7F - aLogicGame.Font48.MeasureString(aStatusText).Y * 0.7F) / 3F);
                tmpNewOffSetVector2.X = ((aStatusBackgroundRectangle.Size.X * 0.7F - aStatusRectangle.Size.X * 0.7F) / 2F - (aLogicGame.Font48.MeasureString(aStatusText).X * 0.7F) / 2F);
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font48, aStatusText, aPosition + tmpNewOffSetVector2, Color.White, 0F, Vector2.Zero, 0.7F, SpriteEffects.None, 0F);

                

                tmpNewOffSetVector2.Y = ((aStatusBackgroundRectangle.Size.Y * 0.7F - aStatusRectangle.Size.Y * 0.7F) / 2F);
                tmpNewOffSetVector2.X = aStatusBackgroundRectangle.Size.X * 0.7F - aStatusRectangle.Size.X * 0.7F - tmpNewOffSetVector2.Y;
                aLogicGame.SpriteBatch.Draw(aStatusTexture, aPosition + tmpNewOffSetVector2, aStatusRectangle, Color.White, 0F, Vector2.Zero, 0.7F, SpriteEffects.None, 0F);

                

                tmpNewOffSetVector2.X = (aStatusBackgroundRectangle.Size.X * 0.7F) / 2F + (-1 * aLogicGame.Font28.MeasureString(aStatusMiniText).X / 2F * 0.8F) ;
                tmpNewOffSetVector2.Y = aStatusBackgroundRectangle.Size.Y * 0.7F ;
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aStatusMiniText, aPosition + tmpNewOffSetVector2, Color.Gold, 0F, Vector2.Zero, 0.8F, SpriteEffects.None, 0F);

                aLogicGame.SpriteBatch.Draw(aStatusBackgroundTexture, aPosition, aStatusBackgroundRectangle, Color.White, 0F, Vector2.Zero, 0.7F, SpriteEffects.None, 0.1F);
            }

            base.Draw(parGameTime);

            
        }
    }
}
