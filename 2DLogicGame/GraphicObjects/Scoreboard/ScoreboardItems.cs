using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.GraphicObjects.Scoreboard
{
    /// <summary>
    /// Trieda, ktora reprezentuje polozky nachadzajuce sa v hodnotiacej tabulke.
    /// </summary>
    public class ScoreboardItems
    {

        /// <summary>
        /// Atribut, reprezentujuci ID zaznamu v databaze - typ int
        /// </summary>
        private int aID;

        /// <summary>
        /// Atribut, reprezentujuci meno prveho hraca - typ string
        /// </summary>
        private string aFirstPlayerName;

        /// <summary>
        /// Atribut, reprezentujuci meno druheho hraca - typ string
        /// </summary>
        private string aSecondPlayerName;

        /// <summary>
        /// Atribut, reprezentujuci body dosiahnute za level - typ int
        /// </summary>
        private int aPoints;

        /// <summary>
        /// Atribut, reprezentujuci cas, za ktory hraci uspesne dokoncili hru - typ string. Pozn. Format - HH:MM:SS
        /// </summary>
        private string aTime;

        public int Id
        {
            get => aID;
            set => aID = value;
        }

        public string FirstPlayerName
        {
            get => aFirstPlayerName;
            set => aFirstPlayerName = value;
        }

        public string SecondPlayerName
        {
            get => aSecondPlayerName;
            set => aSecondPlayerName = value;
        }

        public int Points
        {
            get => aPoints;
            set => aPoints = value;
        }

        public string Time
        {
            get => aTime;
            set => aTime = value;
        }

        /// <summary>
        /// Konstruktor, ktory inicializuje atributy suvisiace s hodnotiacou tabulkou. Parametre, su volitelne a nezadany parameter, bude mat za nasledok - int -> -1 a string -> "".
        /// </summary>
        /// <param name="parID">Parameter, reprezentujuci ID zaznamu - typ int</param>
        /// <param name="parFirstPlayerName">Parameter, reprezentujuci meno prveho hraca - typ string</param>
        /// <param name="parSecondPlayerName">Parameter, reprezentujuci meno druheho hraca - typ string</param>
        /// <param name="parPoints">Parameter, reprezentujuci body - typ int</param>
        /// <param name="parTime">Parameter, reprezentujuci cas - typ int</param>
        public ScoreboardItems(int parID = -1, string parFirstPlayerName = "", string parSecondPlayerName = "", int parPoints = -1, int parTime = -1)
        {
            aID = parID;
            aFirstPlayerName = parFirstPlayerName;
            aSecondPlayerName = parSecondPlayerName;
            aPoints = parPoints;
            aTime = ConvertTimeToString(parTime);
        }

        /// <summary>
        /// Metoda, ktora premenni cas v sekundach do tvaru - HH:MM:SS
        /// </summary>
        public string ConvertTimeToString(int parSeconds)
        {
            if (parSeconds >= 0)
            {
                TimeSpan tmpTimeSpan = TimeSpan.FromSeconds(parSeconds);

                string tmpFormattedString = tmpTimeSpan.ToString(@"hh\:mm\:ss");

                return tmpFormattedString;
            }
            else
            {
                return "UNDEFINED";
            }
        }



    }
}
