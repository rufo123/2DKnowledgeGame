using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects.Connecting
{

    public class ConnectingUI : DrawableGameComponent
    {
        private LogicGame aGame;

        private Vector2 aPosition;

        private string aConnecting;

        private int aConnectionTimeout;

        private StringBuilder aConnectingProgressBuilder;

        private char aCharacterProgress;

        private bool aStarted;

        private float aHelperGameTimer;

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
