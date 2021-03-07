using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame
{
    /// <summary>
    /// Reprezentuje ake ulohy maju byt vykonane
    /// </summary>
    public enum MenuTasksToBeExecuted { 
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

        private MenuTasksToBeExecuted aTaskToExecute;

        public MenuTasksToBeExecuted TaskToExecute { get => aTaskToExecute; set => aTaskToExecute = value; }


        /// <summary>
        /// Konstruktor Menu
        /// </summary>
        /// <param name="parGame"> Parameter Hra - typu LogicGame </param>
        /// <param name="parMenuBox"> Parameter Menu Boxu - typu MenuBox</param>
        public Menu(LogicGame parGame, MenuBox parMenuBox) : base(parGame)
        {
            this.aLogicGame = parGame;
            this.aMenuBox = parMenuBox;
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
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.ProceedKey)) {

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
                        this.TaskToExecute = MenuTasksToBeExecuted.Show_Stats;
                        break;
                    case MenuItemAction.Stats:
                        Debug.WriteLine("Stats");
                        this.TaskToExecute = MenuTasksToBeExecuted.Enroll_Credits;
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

            base.Update(gameTime);
        }

    }
}

