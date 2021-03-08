using _2DLogicGame.ClientSide;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{
    class PlayerController : GameComponent
    {
        private PlayerClientData aPlayer;

        private LogicGame aGame;

        private Keys aUp = Keys.W, aRight = Keys.D, aDown = Keys.S, aLeft = Keys.A;

        private bool aUpdateNeeded = false;

        private GameTime aGameTime;

        public GameTime GameTime { get => aGameTime; set => aGameTime = value; }

        /// <summary>
        /// Konstruktor PlayerControlleru, Hrac mozu byt zadany uz pri konstrukcii, alebo neskor...
        /// </summary>
        /// <param name="parPlayer">Volitelny parameter, reprezentujuci hraca - typ Player</param>
        /// <param name="parGame">Parameter reprezentujuci hru - typ LogicGame</param>
        public PlayerController(LogicGame parGame, PlayerClientData parPlayer = null) : base(parGame)
        {
            aGame = parGame;
            aPlayer = parPlayer;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Metoda, ktora nastavi hraca, ktory bude ovladany - Controllerom
        /// </summary>
        public void SetPlayer(PlayerClientData parPlayer)
        {
            this.aPlayer = parPlayer;
        }

        public override void Update(GameTime gameTime)
        {

            aGameTime = gameTime;

            if (aPlayer != null)
            {

                double tmpElapsedTime = gameTime.ElapsedGameTime.TotalSeconds;

                if (Keyboard.GetState().IsKeyDown(aUp))
                {
                    aUpdateNeeded = true;
                    aPlayer.SetDirection(Entity.Direction.UP);
                    aPlayer.Move(gameTime);
                }
                else if (Keyboard.GetState().IsKeyDown(aRight))
                {
                    aUpdateNeeded = true;
                    aPlayer.SetDirection(Entity.Direction.RIGHT);
                    aPlayer.Move(gameTime);
                }
                else if (Keyboard.GetState().IsKeyDown(aLeft))
                {
                    aUpdateNeeded = true;
                    aPlayer.SetDirection(Entity.Direction.LEFT);
                    aPlayer.Move(gameTime);
                }
                else if (Keyboard.GetState().IsKeyDown(aDown))
                {
                    aUpdateNeeded = true;
                    aPlayer.SetDirection(Entity.Direction.DOWN);
                    aPlayer.Move(gameTime);
                }

            }

            base.Update(gameTime);
        }
        public bool IsUpdateNeeded()
        {
            if (aUpdateNeeded == true)
            {
                aUpdateNeeded = false;
                return true;
            }
            else
            {
                return false;
            }
        }


        protected override void OnUpdateOrderChanged(object sender, EventArgs args)
        {
            base.OnUpdateOrderChanged(sender, args);
        }
    }
}
