using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame
{
    public class MenuBox : Microsoft.Xna.Framework.DrawableGameComponent
    {

        private LogicGame aLogicGame;


        private List<MenuItem> aBoxItems; //List, resp. Box - Menu Itemov - List je efektivnejsi v C# ako Array
        private Vector2 aBoxPosition; //Vector - Pozicia Menu Boxu
        private MenuItem aSelectedItem;


        private Color aColorMenuItem; //Farba Menu Itemu - Color
        private Color aColorSelectedItem; //Farba Celeho Menu Boxu - Color
        private Color aColorUnFocusedSelectedItem; //Farba Selected Itemu, pokial na neho neukazujeme - Color

        private double aBoxTextSize; //Velkost Menu Boxu - Double

        /// <summary>
        /// Atribut, ktory reprezentuje ci je MenuBox povoleny, ci sa hrac nachadza v hlavnom menu alebo v podmenu - typ bool.
        /// </summary>
        private bool aBoxEnabled;

        private float aMenuItemSize;

        KeyboardState aPreviousKeyPressed;

        private double aTimeSinceLastKeyPress = 0;

        /// <summary>
        /// Atribut, ktory reprezentuje ci sa pouzivatel pokusa vybrat si nejaky item z MenuBoxu, alebo je niekde inde - typ bool.
        /// </summary>
        private bool aSelectingFromMenuBox;

        public bool SelectingFromMenuBox
        {
            get => aSelectingFromMenuBox;
            set => aSelectingFromMenuBox = value;
        }

        public bool BoxEnabled
        {
            get => aBoxEnabled;
            set => aBoxEnabled = value;
        }

        public MenuItem Set { get; set; }
        internal MenuItem SelectedItem { get => aSelectedItem; set => aSelectedItem = value; }

        /// <summary>
        /// Zatial reprezentuje MenuBox, ktory obsahuje Menu Itemy
        /// </summary>
        /// <param name="parGame"> Parameter Hra</param>
        /// <param name="parPosition"> Parameter Pozicia - Vector</param>
        /// <param name="parItemMenuColor"> Parameter Farba Menu Itemu - Color</param>
        /// <param name="parSelectedItemColor">Parameter </param>
        /// <param name="parNotFocusedSelected">Parameter, ktory specifikuje farby, ktora sa zobrazi pokial sme si zvolili nejaky item, ale momentalne na neho neukazujeme.</param>
        /// <param name="parBoxTextSize"></param>
        public MenuBox(LogicGame parGame, Vector2 parPosition, Color parItemMenuColor, Color parSelectedItemColor, Color parNotFocusedSelected, double parBoxTextSize) : base(parGame)
        {
            this.aLogicGame = parGame;
            this.aBoxItems = new List<MenuItem>();
            this.aBoxPosition = parPosition;
            this.SelectedItem = null;
            this.aColorMenuItem = parItemMenuColor;
            this.aColorSelectedItem = parSelectedItemColor;
            this.aBoxTextSize = parBoxTextSize;
            this.aBoxEnabled = true;
            this.aSelectingFromMenuBox = true;
            this.aColorUnFocusedSelectedItem = parNotFocusedSelected;
        }

        public override void Draw(GameTime gameTime)
        {

            if (aBoxEnabled)
            {

                // aLogicGame.SpriteBatch.Begin();

                for (int i = 0; i < aBoxItems.Count; i++)
                {
                    Color tmpColor = aColorMenuItem;

                    if (aBoxItems[i] == SelectedItem && this.aSelectingFromMenuBox) //Ak mame nejaky item zvoleny a zaroven pracujeme s menu boxom
                    {
                        tmpColor = aColorSelectedItem;
                    }
                    else if (aBoxItems[i] == SelectedItem && !this.aSelectingFromMenuBox) //Ak mame nejaky item zvoleny ale neukazujeme na menu box
                    {
                        tmpColor = aColorUnFocusedSelectedItem;
                    }

                    aLogicGame.SpriteBatch.DrawString(aLogicGame.Font72, aBoxItems[i].MenuText, aBoxItems[i].MenuPosition, tmpColor);


                }

            }

            //  aLogicGame.SpriteBatch.End();


            base.Draw(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
       

            //aTimeSinceLastKeyPress = 0;


            // aTimeSinceLastKeyPress += gameTime.ElapsedGameTime.TotalSeconds;


            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Metoda, ktora vrati nazov itemu, na ktory uzivatel prave "ukazuje" - typ string
        /// </summary>
        /// <returns></returns>
        public string GetNameOfHoverOn()
        {
            return SelectedItem.MenuText;
        }


        /// <summary>
        /// Metoda, ktora prida MenuItem do MenuBoxu
        /// </summary>
        /// <param name="parMenuItemText"> Text, ktory sa ma objavit v Menu Iteme - String</param>
        public void AddItem(string parMenuItemText, MenuItemAction parAction)
        {

            float tmpNewPositionY = aBoxPosition.Y + aBoxItems.Count * (float)aBoxTextSize; //Novu poziciu Y vypocitame pomocou, starej pozicie Y + Pocet Akt. Menu Itemov + Velkost Textu
            Vector2 tmpPosition = new Vector2(aBoxPosition.X, tmpNewPositionY);
            MenuItem tmpNewMenuItem = new MenuItem(parMenuItemText, tmpPosition, parAction, aBoxTextSize);
            aBoxItems.Add(tmpNewMenuItem);
            SelectedItem = tmpNewMenuItem;

        }
        /// <summary>
        /// Metoda, ktora posuva zvoleny MenuItem podla parametra dopredu/dozadu
        /// </summary>
        /// <param name="parMoveBackward">Posuvanie dopredu - false, Posuvanie dozadu - true</param>
        public void MoveNextPrev(bool parMoveBackward = false)
        {

            int tmpSelectedIndex = aBoxItems.IndexOf(SelectedItem); //Ziska index, momentalne zvoleneho itemu
            int tmpSizeOfMenu = aBoxItems.Count - 1;

            if (!parMoveBackward) //Ak je parameter false
            { //Pohyb dopredu

                if (tmpSelectedIndex < (tmpSizeOfMenu)) //Ak sme na konci MenuItemov
                {
                    SelectedItem = aBoxItems[tmpSelectedIndex + 1]; //Posunieme sa dopredu
                }


            }
            else //Ak je parameter true
            { //Pohyb dozadu
                if (tmpSelectedIndex != 0) //Ak nie sme na prvom oznacenom MenuIteme
                {
                    SelectedItem = aBoxItems[tmpSelectedIndex - 1]; //Posunieme sa dozadu
                }
            }
        }
    }
}
