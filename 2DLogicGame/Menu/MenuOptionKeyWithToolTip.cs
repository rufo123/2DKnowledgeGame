using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _2DLogicGame.ClientSide
{
    /// <summary>
    /// Enumeracna trieda, reprezentujuca typy pouzitych klaves v hre.
    /// </summary>
    public enum KeyTypes
    {
        UpKey = 0,
        DownKey = 1,
        LeftKey = 2,
        RightKey = 3,
        ProceedKey = 4,
        ChatWriteMessageKey = 5,
        MusicLower = 6,
        MusicHigher = 7,
        MusicStartTop = 8
    }
    /// <summary>
    /// Trieda, ktora reprezentuje polozku nastaveni hry, definovanu ToolTipom a samotnou klavesou.
    /// </summary>
    public class MenuOptionKeyWithToolTip
    {

        /// <summary>
        /// Atribut, reprezentujuci ToolTip - typ string
        /// </summary>
        private string aToolTip;

        /// <summary>
        /// Atribut, reprezentujuci Key - typ Keys
        /// </summary>
        private Keys aKey;

        /// <summary>
        /// Atribut, reprezentujuci poziciu itemu - typ Vector2
        /// </summary>
        private Vector2 aToolTipPosition;

        /// <summary>
        /// Atribut, reprezentujuci poziciu tlacidla - typ Vector2
        /// </summary>
        private Vector2 aKeyPosition;

        /// <summary>
        /// Atribut, reprezentujuci typ tlacidla - typ KeyTypes - enum.
        /// </summary>
        private KeyTypes aKeyType;

        /// <summary>
        /// Konstruktor, ktory inicializuje polozku nastavenia tlacidla s tooltipom.
        /// </summary>
        /// <param name="parToolTip">Parameter, reprezentujuci popis tlacidla - typ string.</param>
        /// <param name="parKey">Parameter, reprezentujuci typ tlacidla - typ Keys.</param>
        /// <param name="parToolTipPosition">Parameter, reprezentujuci poziciu tooltipu - typ Vector2.</param>
        /// <param name="parKeyPosition">Parameter, reprezentujuci poziciu popisu tlacidla - typ Vector2.</param>
        /// <param name="parKeyType">Parameter, reprezentujuci typ tlacidla - typ KeyTypes - enum.</param>
        public MenuOptionKeyWithToolTip(string parToolTip, Keys parKey, Vector2 parToolTipPosition, Vector2 parKeyPosition, KeyTypes parKeyType)
        {
            aToolTip = parToolTip;
            aKey = parKey;
            aToolTipPosition = parToolTipPosition;
            aKeyPosition = parKeyPosition;
            aKeyType = parKeyType;
        }

        public string ToolTip
        {
            get => aToolTip;
            set => aToolTip = value;
        }

        public Keys Key
        {
            get => aKey;
            set => aKey = value;
        }

        public Vector2 ToolTipPosition
        {
            get => aToolTipPosition;
            set => aToolTipPosition = value;
        }

        public Vector2 KeyPosition
        {
            get => aKeyPosition;
            set => aKeyPosition = value;
        }

        public KeyTypes KeyType
        {
            get => aKeyType;
            set => aKeyType = value;
        }

    }
}
