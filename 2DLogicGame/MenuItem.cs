using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame
{

    public enum MenuItemAction { 
    Start_Host,
    Start_Play,
    Options,
    Stats,
    Credits,
    Exit,
    }

    public class MenuItem
    {
        private Vector2 aMenuPosition; //Atribut vyjadrujuci Vector - Pozicia menu
        private string aMenuText; //Atribut typu String - Text menu
        private double aMenuSize = 0.5f; //Atribut typu double - Velkost Menu
        private MenuItemAction aAction; //Atribut typu MenuItemAction - Action Connected With Item

        /// <summary>
        /// Polozka menu, ktora sa da flexibilne vytvorit s pomocou parametrov
        /// </summary>
        /// <param name="parMenuText">Parameter typu String - Text Polozky Menu</param>
        /// <param name="parMenuPos">Parameter typu Vector2 - Pozicia Polozky Menu</param>
        /// <param name="parAction">Parameter typu MenuItemAction - Akcia spojena s Menu Itemom</param>
        /// <param name="parMenuSize">Parameter typy double - Velkost Polozky Menu</param>
        public MenuItem(string parMenuText, Vector2 parMenuPos, MenuItemAction parAction, double parMenuSize = 0.5f)
        {
            this.MenuText = parMenuText;
            this.MenuSize = parMenuSize;
            this.MenuPosition = parMenuPos;
            this.aAction = parAction;
        }

        public Vector2 MenuPosition { get => aMenuPosition; set => aMenuPosition = value; }
        public string MenuText { get => aMenuText; set => aMenuText = value; }
        public double MenuSize { get => aMenuSize; set => aMenuSize = value; }
        public MenuItemAction Action { get => aAction; set => aAction = value; }
    }
}
