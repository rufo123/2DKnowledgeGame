﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using _2DLogicGame.GraphicObjects;
using _2DLogicGame.GraphicObjects.Scoreboard;
using _2DLogicGame.ServerSide.Database;
using SharpFont;

namespace _2DLogicGame
{
    /// <summary>
    /// Reprezentuje ake ulohy maju byt vykonane
    /// </summary>
    public enum MenuTasksToBeExecuted
    {
        None,
        Host_Start,
        Play_Start,
        Exit,
        Enroll_Credits,
        Change_Options,
        Show_Stats
    }

    public class Menu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private MenuBox aMenuBox;
        private LogicGame aLogicGame;
        private Texture2D aMenuBackground;
        private ScoreboardController aScoreboardController;

        private MenuInput aNickNameInput;

        private MenuInput aIPInput;

        private List<MenuInput> aListOfInputs;

        private MenuInput aSelectedInput;

        private int aSelectedInputID;

        private MenuTasksToBeExecuted aTaskToExecute;

        public MenuTasksToBeExecuted TaskToExecute
        {
            get => aTaskToExecute;
            set => aTaskToExecute = value;
        }


        /// <summary>
        /// Konstruktor Menu
        /// </summary>
        /// <param name="parGame"> Parameter Hra - typu LogicGame </param>
        /// <param name="parMenuBox"> Parameter Menu Boxu - typu MenuBox</param>
        /// <param name="aScoreboardController"></param>
        /// <param name="parNickNameInput"></param>
        /// <param name="parIpInput"></param>
        public Menu(LogicGame parGame, MenuBox parMenuBox, ScoreboardController aScoreboardController, MenuInput parNickNameInput, MenuInput parIpInput) : base(parGame)
        {
            this.aLogicGame = parGame;
            this.aMenuBox = parMenuBox;
            this.aScoreboardController = aScoreboardController;
            aNickNameInput = parNickNameInput; //Rozdelil som tieto input, preto takto a nedal som ich do listu, lebo sa budu zobrazovat az po prejdeni na urcity menu item
            aIPInput = parIpInput;

            aListOfInputs = new List<MenuInput>(2);

            aListOfInputs.Add(parNickNameInput);
            aListOfInputs.Add(parIpInput);
            if (aListOfInputs != null)
            {
                aSelectedInput = aListOfInputs[0];
            }
            else
            {
                aSelectedInput = null;
            }


            aSelectedInputID = 0;
        }


        protected override void LoadContent()
        {
            aMenuBackground = aLogicGame.Content.Load<Texture2D>("Sprites\\Backgrounds\\menuBackground");
            base.LoadContent();
        }


        public override void Draw(GameTime gameTime)
        {

            //  aLogicGame.SpriteBatch.Begin();
            aLogicGame.SpriteBatch.Draw(aMenuBackground, new Vector2(0, 0), Color.White);
            // aLogicGame.SpriteBatch.End();


            base.Draw(gameTime);
        }

        public override void Initialize()
        {
            this.TaskToExecute = MenuTasksToBeExecuted.None;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (aMenuBox.BoxEnabled && aLogicGame.GameState != GameState.Typing)
            {

                if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.DownKey))
                {
                    if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.DownKey)) //Pohyb Hore
                    {
                        if (aMenuBox.SelectingFromMenuBox)
                        {
                            aMenuBox.MoveNextPrev(false); // Posuvanie Dopredu - false
                        }
                        else
                        {
                            MoveToNextInput(true); //Ideme na dalsi
                        }
                    }

                }
                if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.UpKey))
                {
                    if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.UpKey)) //Pohyb Dole
                    {
                        if (aMenuBox.SelectingFromMenuBox)
                        {
                            aMenuBox.MoveNextPrev(true); // Posuvanie Dozadu - true
                        }
                        else
                        {
                            MoveToNextInput(false); //Ideme na predosly
                        }
                    }

                }

                if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.LeftKey))
                {
                    if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.LeftKey)) //Pohyb Dole
                    {
                        //Nastavime, ze si uz nevybera z MenuBoxu
                        if (aMenuBox.SelectingFromMenuBox)
                        {
                            aMenuBox.SelectingFromMenuBox = !CheckIfAnyInputEnabled(); //Ak zistime ze ziaden z Input Boxov nie je povoleny, nastavime SelectingFromMenuBox na opacnu hodnotu
                            //Ak bude input povoleny -> true tak Selecting sa nastavi na false, cize prejdeme k input boxom
                        }
                        else
                        {
                            aMenuBox.SelectingFromMenuBox = !aMenuBox.SelectingFromMenuBox;
                        }

                    }
                }
                if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.RightKey))
                {
                    if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.RightKey)) //Pohyb Dole
                    {
                        //Nastavime, ze si uz nevybera z MenuBoxu
                        if (aMenuBox.SelectingFromMenuBox)
                        {
                            aMenuBox.SelectingFromMenuBox = !CheckIfAnyInputEnabled(); //Ak zistime ze ziaden z Input Boxov nie je povoleny, nastavime SelectingFromMenuBox na opacnu hodnotu
                            //Ak bude input povoleny -> true tak Selecting sa nastavi na false, cize prejdeme k input boxom
                        }
                        else
                        {
                            aMenuBox.SelectingFromMenuBox = !aMenuBox.SelectingFromMenuBox;
                        }

                    }

                }

            }

            if (!aMenuBox.SelectingFromMenuBox) //Ak si uz nevyberame z Menu Boxu ale z inputov
            {
                for (int i = 0; i < aListOfInputs.Count; i++)
                {
                    if (aListOfInputs[i] == aSelectedInput)
                    {
                        aListOfInputs[i].Hover = true;
                    }
                    else
                    {
                        aListOfInputs[i].Hover = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < aListOfInputs.Count; i++)
                {
                    aListOfInputs[i].Hover = false;
                }
            }


            switch (aMenuBox.GetNameOfHoverOn()) //Ak bude uzivatel ukazovat na Play alebo Host, zobrazia sa mu aj Input Boxy
            {
                case "Play":
                    if (aNickNameInput != null)
                    {
                        aNickNameInput.InputEnabled = true;
                        aIPInput.InputEnabled = true;
                    }
                    break;
                case "Host":
                    if (aNickNameInput != null && aIPInput != null)
                    {
                        aNickNameInput.InputEnabled = true;
                        aIPInput.InputEnabled = false;
                    }
                    break;
                default:
                    aNickNameInput.InputEnabled = false;
                    aIPInput.InputEnabled = false;
                    break;
            }


            switch (TaskToExecute)
            {
                case MenuTasksToBeExecuted.None:
                    if (aScoreboardController != null)
                    {
                        aMenuBox.BoxEnabled = true;
                        aScoreboardController.ShowStats(false);
                    }
                    MenuHandle(); //Pokial nie je nic zvolene zobrazi sa hlavna cast menu
                    break;
                case MenuTasksToBeExecuted.Enroll_Credits:
                    break;
                case MenuTasksToBeExecuted.Change_Options:
                    break;
                case MenuTasksToBeExecuted.Show_Stats:
                    if (aScoreboardController != null)
                    {
                        aMenuBox.BoxEnabled = false;
                        aScoreboardController.InitScoreboard();
                        aScoreboardController.ShowStats(true);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            base.Update(gameTime);
        }

        public void MenuHandle()
        {
            if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.ProceedKey))
            {
                if (aMenuBox.SelectingFromMenuBox)
                {
                    switch (this.aMenuBox.SelectedItem.Action)
                    {
                        case MenuItemAction.Start_Host:
                            Debug.WriteLine("Host");
                            aLogicGame.GameState = GameState.Playing;
                            this.TaskToExecute = MenuTasksToBeExecuted.Host_Start;
                            break;
                        case MenuItemAction.Start_Play:
                            Debug.WriteLine("Play");
                            aLogicGame.GameState = GameState.Playing;
                            this.TaskToExecute = MenuTasksToBeExecuted.Play_Start;
                            break;
                        case MenuItemAction.Options:
                            Debug.WriteLine("Options");
                            aLogicGame.GameState = GameState.Submenu;
                            this.TaskToExecute = MenuTasksToBeExecuted.Change_Options;
                            break;
                        case MenuItemAction.Stats:
                            Debug.WriteLine("Stats");
                            aLogicGame.GameState = GameState.Submenu;
                            this.TaskToExecute = MenuTasksToBeExecuted.Show_Stats;
                            break;
                        case MenuItemAction.Exit:
                            Debug.WriteLine("Exit");
                            aLogicGame.GameState = GameState.Exit;
                            this.TaskToExecute = MenuTasksToBeExecuted.Exit;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (aListOfInputs != null && aSelectedInput != null)
                    {
                        aSelectedInput.Focus = !aSelectedInput.Focus; //Nastavime Focus na opacnu hodnotu

                        if (aSelectedInput.Focus) //Zaleziac od toho ci je Focus povoleny ale nie nastavime GameState na Typing
                        {
                            aLogicGame.GameState = GameState.Typing;
                        }
                        else
                        {
                            aLogicGame.GameState = GameState.Submenu;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Metoda, ktora posuva vybar na dalsi input, pokial je povoleny.
        /// </summary>
        /// <param name="parNext">Parameter, ktory reprezentuje ci si uzivatel chce vybrat Input dalsi - true, alebo predosly - false</param>
        public void MoveToNextInput(bool parNext)
        {
            bool tmpSomeInputEnabled = false;
            if (aListOfInputs != null) //Najprv si positeme ci vobec nejaky Input je povoleny
            {
                for (int i = 0; i < aListOfInputs.Count; i++)
                {
                    if (aListOfInputs[i].InputEnabled)
                    {
                        tmpSomeInputEnabled = true;
                    }

                }
            }

            if (aListOfInputs != null && tmpSomeInputEnabled)
            {
                if (parNext) //Dopredu
                {
                    aSelectedInputID++;

                    if (aSelectedInputID == aListOfInputs.Count) //Kontrola aby sme nepresli na index vacsi ako je pocet prvkov v Liste
                    {
                        aSelectedInputID = 0;
                    }


                    while (aListOfInputs[aSelectedInputID].InputEnabled == false) //Budeme prechadzat itemy az pokial nenajdeme taky, ktory je povolen, az potom pojde dalsia kontrola
                    {
                        aSelectedInputID++;
                        if (aSelectedInputID == aListOfInputs.Count)
                        {
                            aSelectedInputID = 0;
                        }
                    }

                    if (aSelectedInputID < aListOfInputs.Count)
                    {

                        aSelectedInput = aListOfInputs[aSelectedInputID];
                    }
                    else
                    {
                        aSelectedInputID = 0;
                        aSelectedInput = aListOfInputs[aSelectedInputID];
                    }
                }
                else //Dozadu
                {
                    aSelectedInputID--;

                    if (aSelectedInputID < 0) //Kontrola aby sme nepresli na index -1
                    {
                        aSelectedInputID = aListOfInputs.Count - 1;
                    }

                    while (aListOfInputs[aSelectedInputID].InputEnabled == false) //Budeme prechadzat itemy az pokial nenajdeme taky, ktory je povoleny, az potom pojde dalsia kontrola
                    {
                        aSelectedInputID--;

                        if (aSelectedInputID < 0)
                        {
                            aSelectedInputID = aListOfInputs.Count - 1;
                        }
                    }

                    if (aSelectedInputID >= 0)
                    {

                        aSelectedInput = aListOfInputs[aSelectedInputID];
                    }
                    else
                    {
                        aSelectedInputID = aListOfInputs.Count - 1;
                        aSelectedInput = aListOfInputs[aSelectedInputID];
                    }
                }
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o kontrolu Input Itemov, ci je nejaky z nich povoleny;
        /// </summary>
        /// <returns></returns>
        public bool CheckIfAnyInputEnabled()
        {
            if (aListOfInputs != null)
            {
                for (int i = 0; i < aListOfInputs.Count; i++) //Ak najdeme jeden taky Input ktory je povoleny
                {
                    if (aListOfInputs[i].InputEnabled)
                    {
                        return true;
                    }
                }
            }
            return false; //Inak vratime false
        }


    }
}

