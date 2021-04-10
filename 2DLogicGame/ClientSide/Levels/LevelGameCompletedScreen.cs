using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;

namespace _2DLogicGame.ClientSide.Levels
{
    public class LevelGameCompletedScreen : DrawableGameComponent
    {
        /// <summary>
        /// Atribut, ktory reprezentuje hru - typ LogicGame.
        /// </summary>
        private LogicGame aGame;

        /// <summary>
        /// Atribut, ktory reprezentuje poziciu - typ Vector2.
        /// </summary>
        private Vector2 aPosition;

        /// <summary>
        /// Atribut, ktory reprezentuje bool o tom, ci sa ma objekt zobrazit - typ bool.
        /// </summary>
        private bool aShowEndScreen;

        /// <summary>
        /// Atribut, ktory reprezentuje dosiahnute body v hre - typ int.
        /// </summary>
        private int aGamePoints;

        /// <summary>
        /// Atribut, ktory reprezentuje cas dosiahnuty v hre - format: HH:MM:SS - typ string.
        /// </summary>
        private string aGameFinishedInSecond;

        public bool ShowEndScreen
        {
            get => aShowEndScreen;
            set => aShowEndScreen = value;
        }

        public LevelGameCompletedScreen(LogicGame parGame) : base(parGame)
        {
            aGame = parGame;
            aPosition = new Vector2(1920 / 2F, 1080 / 2F);
            aShowEndScreen = false;
            aGamePoints = 0;
            aGameFinishedInSecond = "";
            SetEndingData(5, 2547);
        }

        /// <summary>
        /// Metoda, ktora nastavi vysledne data hry, pricom dba aj na spravne naformatovanie casu podla: HH:MM:SS.
        /// </summary>
        /// <param name="parPoints">Parameter, reprezentujuci dosiahnute body - typ int.</param>
        /// <param name="parSeconds">Parameter, reprezentujuci dosiahnuty cas v sekundach - typ int.</param>
        public void SetEndingData(int parPoints, int parSeconds)
        {
            aGamePoints = parPoints;

            string tmpFormattedString = "";

            if (parSeconds >= 0)
            {
                TimeSpan tmpTimeSpan = TimeSpan.FromSeconds(parSeconds);

                tmpFormattedString = tmpTimeSpan.ToString(@"hh\:mm\:ss");

            }
            aGameFinishedInSecond = tmpFormattedString;
        }

        public override void Draw(GameTime parGameTime)
        {
            if (aShowEndScreen && aGamePoints > 0 && aGameFinishedInSecond != "")
            {
                Vector2 tmpCenterVector = new Vector2(aPosition.X , aPosition.Y - (aGame.Font48.MeasureString("W").Y * 3) / 2F); //Vypocitame Y-ovy offset len z jedneho znaku

                string tmpStringToDraw = "";
                Vector2 tmpOffsetVector2 = new Vector2(0, 0);

                tmpStringToDraw = "Hra uspesne dokoncena!";
                tmpOffsetVector2.X = -1F * aGame.Font48.MeasureString(tmpStringToDraw).X / 2F;
                aGame.SpriteBatch.DrawString(aGame.Font48, tmpStringToDraw, tmpCenterVector + tmpOffsetVector2, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.1F);

                tmpStringToDraw = "Ziskali ste: " + aGamePoints.ToString() + " bodov!";
                tmpOffsetVector2.Y += aGame.Font48.MeasureString("T").Y; //Vypocitame offset medzi riadkami, staci jeden znak preto by mal byt priblizne rovnaky
                tmpOffsetVector2.X = -1F * aGame.Font48.MeasureString(tmpStringToDraw).X / 2F;
                aGame.SpriteBatch.DrawString(aGame.Font48, tmpStringToDraw, tmpCenterVector + tmpOffsetVector2, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.1F);

                tmpStringToDraw = "Cas: " + aGameFinishedInSecond;
                tmpOffsetVector2.Y += aGame.Font48.MeasureString("T").Y; //Vypocitame offset medzi riadkami, staci jeden znak preto by mal byt priblizne rovnaky
                tmpOffsetVector2.X = -1F * aGame.Font48.MeasureString(tmpStringToDraw).X / 2F;
                aGame.SpriteBatch.DrawString(aGame.Font48, tmpStringToDraw, tmpCenterVector + tmpOffsetVector2, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.1F);
            }

            base.Draw(parGameTime);
        }

        public override void Update(GameTime parGameTime)
        {
            base.Update(parGameTime);
        }
    }
}
