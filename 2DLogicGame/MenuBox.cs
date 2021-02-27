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
        private double aBoxTextSize; //Velkost Menu Boxu - Double

        KeyboardState aPreviousKeyPressed;

        private double aTimeSinceLastKeyPress = 0;



       

        /// <summary>
        /// Zatial reprezentuje MenuBox, ktory obsahuje Menu Itemy
        /// </summary>
        /// <param name="parGame"> Parameter Hra</param>
        /// <param name="parPosition"> Parameter Pozicia - Vector</param>
        /// <param name="parItemMenuColor"> Parameter Farba Menu Itemu - Color</param>
        /// <param name="parSelectedItemColor">Parameter </param>
        /// <param name="parBoxTextSize"></param>
        public MenuBox(LogicGame parGame, Vector2 parPosition, Color parItemMenuColor, Color parSelectedItemColor, double parBoxTextSize) : base(parGame)
        {
            this.aLogicGame = parGame;
            aBoxItems = new List<MenuItem>();
            this.aBoxPosition = parPosition;
            this.SelectedItem = null;
            this.aColorMenuItem = parItemMenuColor;
            this.aColorSelectedItem = parSelectedItemColor;
            this.aBoxTextSize = parBoxTextSize;
        }

        public override void Draw(GameTime gameTime)
        {
            aLogicGame.SpriteBatch.Begin();

            for (int i = 0; i < aBoxItems.Count; i++)
            {
                Color tmpColor = aColorMenuItem;

                if (aBoxItems[i] == SelectedItem)
                {
                    tmpColor = aColorSelectedItem;
                }
                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font, aBoxItems[i].MenuText, aBoxItems[i].MenuPosition, tmpColor);




            }

            aLogicGame.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {



            if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.DownKey))
            {
                if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.DownKey)) //Pohyb Hore
                { //Controller - ThumbStick -> Dohora, Klavesnica - W alebo UP
                    MoveNextPrev(false); // Posuvanie Dopredu - false
                } // GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < -0.5f ||


            }
            if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.UpKey))
            {

                if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.UpKey)) //Pohyb Dole
                { //Controller - ThumbStick -> Dohora, Klavesnica - W alebo UP
                    MoveNextPrev(true); // Posuvanie Dozadu - true
                } //GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0.5f ||

            }

            //aTimeSinceLastKeyPress = 0;


            // aTimeSinceLastKeyPress += gameTime.ElapsedGameTime.TotalSeconds;


            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public MenuItem Set { get; set; }
        internal MenuItem SelectedItem { get => aSelectedItem; set => aSelectedItem = value; }

        /// <summary>
        /// Metoda, ktora prida MenuItem do MenuBoxu
        /// </summary>
        /// <param name="parMenuItemText"> Text, ktory sa ma objavit v Menu Iteme - String</param>
        public void AddItem(string parMenuItemText, MenuItemAction parAction)
        {

            float tmpNewPositionY = aBoxPosition.Y + aBoxItems.Count * (float)aBoxTextSize; //Novu poziciu Y vypocitame pomocou, starej pozicie Y + Pocet Akt. Menu Itemov + Velkost Textu
            Vector2 tmpPosition = new Vector2(aBoxPosition.X, tmpNewPositionY);
            MenuItem tmpNewMenuItem = new MenuItem(parMenuItemText, tmpPosition, parAction);
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
