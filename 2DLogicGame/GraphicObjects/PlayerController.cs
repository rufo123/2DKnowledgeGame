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

            if (aPlayer != null)
            {

                double tmpElapsedTime = gameTime.ElapsedGameTime.TotalSeconds;

               

                if (Keyboard.GetState().IsKeyDown(aUp))
                {
                    aPlayer.SetDirection(Entity.Direction.UP);
                    aPlayer.Move(gameTime);
                    Debug.WriteLine("PEs");
                }
                else if (Keyboard.GetState().IsKeyDown(aRight))
                {
                    aPlayer.SetDirection(Entity.Direction.RIGHT);
                    aPlayer.Move(gameTime);
                }
                else if (Keyboard.GetState().IsKeyDown(aLeft))
                {
                    aPlayer.SetDirection(Entity.Direction.LEFT);
                    aPlayer.Move(gameTime);
                }
                else if (Keyboard.GetState().IsKeyDown(aDown))
                {
                    aPlayer.SetDirection(Entity.Direction.DOWN);
                    aPlayer.Move(gameTime);
                }

            }

            base.Update(gameTime);
        }

        protected override void OnUpdateOrderChanged(object sender, EventArgs args)
        {
            base.OnUpdateOrderChanged(sender, args);
        }
    }
}
