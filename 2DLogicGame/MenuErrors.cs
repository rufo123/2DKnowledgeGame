using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame
{
    public class MenuErrors : DrawableGameComponent
    {

        /// <summary>
        /// Atribut, ktory reprezentuje hru - typ LogicGame.
        /// </summary>
        private LogicGame aLogicGame;
        
        /// <summary>
        /// Atribut, ktory reprezentuje poziciu objektu - typ Vector2.
        /// </summary>
        private Vector2 aPosition;

        /// <summary>
        /// Atribut, ktory reprezentuje spravu chyby - typ string;
        /// </summary>
        private string aErrorMessage;

        /// <summary>
        /// Atribut, ktory reprezentuje ci ma byt sprava zobrazena alebo nie - typ bool.
        /// </summary>
        private bool aShowError;

        /// <summary>
        /// Atribut, ktory reprezentuje casovac, aky dlhy cas je zobrazena sprava - typ int
        /// </summary>
        private int aTimerOfErrorShown;

        public bool ShowError
        {
            get => aShowError;
            set => aShowError = value;
        }

        public MenuErrors(LogicGame parLogicGame, Vector2 parPosition) : base(parLogicGame)
        {
            aLogicGame = parLogicGame;
            aPosition = parPosition;
            aShowError = false;
            aErrorMessage = "";
            aTimerOfErrorShown = 0;
        }

        /// <summary>
        /// Metoda, ktora nastavi Error Message podla parametra.
        /// </summary>
        /// <param name="parErrorMessage">Parameter, reprezentujuci Error Message - typ string.</param>
        public void SetErrorMessage(string parErrorMessage)
        {
            aErrorMessage = parErrorMessage;
        }

        /// <summary>
        /// Metoda, ktora zmaze Error Message.
        /// </summary>
        public void DeleteErrorMessage()
        {
            aErrorMessage = "";
        }


        public override void Update(GameTime parGameTime)
        {
            if (aShowError && aErrorMessage != "") //Ak sprava nie je prazdna a je zobrazena, zacneme odpocitavat a postupne ju schovame resp. vymazeme.
            {
                aTimerOfErrorShown += parGameTime.ElapsedGameTime.Milliseconds;

                if (aTimerOfErrorShown > 5000) //Error Message sa zobrazi na 5 sekund
                {
                    DeleteErrorMessage();
                    aTimerOfErrorShown = 0;
                }
            }

            base.Update(parGameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            if (aShowError && aErrorMessage != "") //Vykresli Error Message, pokial sa ma Error Message zobrazit a message nie je prazdna.
            {
                Vector2 tmpOffsetVector = Vector2.Zero;
                tmpOffsetVector.X = -1 * (aLogicGame.Font28.MeasureString(aErrorMessage).X / 2F);

                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, aErrorMessage, aPosition + tmpOffsetVector, Color.Orange, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
            }

            
            base.Draw(gameTime);
        }
    }
}
