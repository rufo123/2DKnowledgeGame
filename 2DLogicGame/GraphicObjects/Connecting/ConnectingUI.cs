using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects.Connecting
{
    /// <summary>
    /// Trieda, ktora reprezentuje obrazovku pripajania sa do hry.
    /// </summary>
    public class ConnectingUI : DrawableGameComponent
    {
        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame.
        /// </summary>
        private LogicGame aGame;

        /// <summary>
        /// Atribut, reprezentujuci poziciu - typ Vector2.
        /// </summary>
        private Vector2 aPosition;

        /// <summary>
        /// Atribut, reprezentujuci text, ktory je zobrazeny pri pripajani sa do hry - typ string.
        /// </summary>
        private string aConnecting;

        /// <summary>
        /// Atribut, ktory reprezentuje timeout, teda hodnotu, ktora ak nadobudne hodnotu 0, vrati hraca do menu - typ int.
        /// </summary>
        private int aConnectionTimeout;

        /// <summary>
        /// Atribut, reprezentujuci builder stringov, reprezentujuci nacitavacie charaktery, aby sa predislo neustalemu vytvaraniu novych stringov - typ StringBuilder.
        /// </summary>
        private StringBuilder aConnectingProgressBuilder;

        /// <summary>
        /// Atribut, ktory reprezentuje charakter, ktory bude reprezentovat progress pripajania sa - typ char.
        /// </summary>
        private char aCharacterProgress;

        /// <summary>
        /// Atribut, ktory reprezentuje ci sa pripajanie zacalo - typ bool.
        /// </summary>
        private bool aStarted;

        /// <summary>
        /// Atribut, ktory reprezentuje pomocny casovac - typ float.
        /// </summary>
        private float aHelperGameTimer;

        /// <summary>
        /// Atribut, ktory reprezentuje urceny cas connection timeoutu - typ int.
        /// </summary>
        private int aInitialConnectionTimeoutTime;

        public bool StartTimer
        {
            get => aStarted;
            set => Start(value);
        }
        public int ConnectionTimeout
        {
            get => aConnectionTimeout;
            set => aConnectionTimeout = value;
        }

        /// <summary>
        /// Konstruktor, objektu pripajacej sa obrazovky.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parConnecting">Parameter, reprezentujuci text pripajania sa - typ string.</param>
        /// <param name="parConnectionTimeout">Parameter, reprezentujuci timeout pripajania sa - typ int.</param>
        /// <param name="parCharacterProgress">Parameter, reprezentujuci charakter, ktory sa ma vykreslovat pri pripajani - typ char.</param>
        public ConnectingUI(LogicGame parGame, string parConnecting, int parConnectionTimeout, char parCharacterProgress) : base(parGame)
        {
            aGame = parGame;
            aPosition = new Vector2(1920 / 2F, 1080 / 3F);
            aConnecting = parConnecting;
            aConnectionTimeout = parConnectionTimeout;
            aConnectingProgressBuilder = new StringBuilder(3);
            aCharacterProgress = parCharacterProgress;
            aInitialConnectionTimeoutTime = parConnectionTimeout;
            aStarted = false;
            aHelperGameTimer = 0F;
        }

        /// <summary>
        /// Metoda, ktora sa stara o vykreslovanie objektu.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Draw(GameTime parGameTime)
        {

            if (aStarted && aConnectionTimeout > 0)
            {

                Vector2 tmpOffsetVector = new Vector2(0, 0);
                tmpOffsetVector.X -= aGame.Font48.MeasureString(aConnecting).X / 2F;

                aGame.SpriteBatch.DrawString(aGame.Font48, aConnecting, aPosition + tmpOffsetVector, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);

                tmpOffsetVector.X += aGame.Font48.MeasureString(aConnecting).X / 2F - aGame.Font48.MeasureString(aConnectionTimeout.ToString()).X / 2F;

                tmpOffsetVector.Y += aGame.Font48.MeasureString(aConnectionTimeout.ToString()).Y; 
                aGame.SpriteBatch.DrawString(aGame.Font48, aConnectingProgressBuilder.ToString(), aPosition + tmpOffsetVector, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);

                tmpOffsetVector.Y += aGame.Font48.MeasureString(aConnecting).Y; aGame.SpriteBatch.DrawString(aGame.Font48, aConnectionTimeout.ToString(), aPosition + tmpOffsetVector, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);

            }

            base.Draw(parGameTime);
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizacie objektu. Najme o aktualizovanie casovaca.
        /// </summary>
        /// <param name="parGameTime"></param>
        public override void Update(GameTime parGameTime)
        {

            if (aStarted && aConnectionTimeout > 0)
            {
                aHelperGameTimer += (float)parGameTime.ElapsedGameTime.Milliseconds;

                if (aHelperGameTimer > 1000)
                {
                    HandleProgressBuilder();
                    aHelperGameTimer = 0F;
                    aConnectionTimeout--;
                }
            }

            base.Update(parGameTime);
        }

        /// <summary>
        /// Metoda, ktora sa stara o spustenie/zasavenie odpoctu pri pripajani sa.
        /// </summary>
        /// <param name="parStart">Parameter, reprezentujuci ci sa ma odpocitavanie zacat alebo nie - typ bool.</param>
        public void Start(bool parStart)
        {
            if (parStart)
            {
                aConnectionTimeout = aInitialConnectionTimeoutTime;
                aStarted = true;
            }
            else
            {
                aStarted = false;
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o spravu charakterov pri pripajani sa.
        /// V praxi to funguje tak ak charakter je napr '.' .
        /// Postupne sa vykresluje - "." -> ". ." -> ". . ." .
        /// </summary>
        public void HandleProgressBuilder()
        {
            if (aConnectingProgressBuilder != null)
            {
                if (aConnectingProgressBuilder.Length <= 3)
                {
                    aConnectingProgressBuilder.Append(aCharacterProgress);
                }
                else
                {
                    aConnectingProgressBuilder.Clear();
                }
            }
        }
    }
}
