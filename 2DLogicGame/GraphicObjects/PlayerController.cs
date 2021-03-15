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

        private KeyboardState aOldKeyboardState;

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
            aOldKeyboardState = new KeyboardState(Keys.None);
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

            KeyboardState tmpNewKeyBoardState = Keyboard.GetState();

            if (aPlayer != null)
            {
                Keys tmpCurrentKeyPresed = Keys.None;

                double tmpElapsedTime = gameTime.ElapsedGameTime.TotalSeconds;

                if (tmpNewKeyBoardState.IsKeyDown(aUp))
                {
                    aPlayer.SetDirection(Entity.Direction.UP);
                    aPlayer.Move(gameTime);
                    aPlayer.IsMoving = true;
                    tmpCurrentKeyPresed = aUp;
                }
                else if (tmpNewKeyBoardState.IsKeyDown(aRight))
                {
                    aPlayer.SetDirection(Entity.Direction.RIGHT);
                    aPlayer.Move(gameTime);
                    aPlayer.IsMoving = true;
                    tmpCurrentKeyPresed = aRight;
                }
                else if (tmpNewKeyBoardState.IsKeyDown(aLeft))
                {
                    aPlayer.SetDirection(Entity.Direction.LEFT);
                    aPlayer.Move(gameTime);
                    aPlayer.IsMoving = true;
                    tmpCurrentKeyPresed = aLeft;
                }
                else if (tmpNewKeyBoardState.IsKeyDown(aDown))
                {

                    aPlayer.SetDirection(Entity.Direction.DOWN);
                    aPlayer.Move(gameTime);
                    aPlayer.IsMoving = true;
                    tmpCurrentKeyPresed = aDown;
                }
                else
                {
                    aPlayer.IsMoving = false;
                }


                //Kedy je update pre server potrebny?

                if (tmpNewKeyBoardState.IsKeyDown(tmpCurrentKeyPresed) && aOldKeyboardState.IsKeyUp(tmpCurrentKeyPresed)) //Porovname, ci uzivatel stlacil nove tlacitko
                {
                    aUpdateNeeded = true;

                }
                else if (tmpNewKeyBoardState.IsKeyUp(tmpCurrentKeyPresed)) //Ak dojde k tomu, ze pustime tlacitka
                {
                    if (aOldKeyboardState.IsKeyDown(aUp) || aOldKeyboardState.IsKeyDown(aRight) || //Porovname ci pred tym bolo stlacene tlacitko pohybu
                        aOldKeyboardState.IsKeyDown(aLeft) || aOldKeyboardState.IsKeyDown(aDown))
                    {
                        aUpdateNeeded = true;

                    }
                }
                else if (tmpNewKeyBoardState.GetPressedKeyCount() == 2 && aOldKeyboardState.GetPressedKeyCount() == 1 || tmpNewKeyBoardState.GetPressedKeyCount() == 1 && aOldKeyboardState.GetPressedKeyCount() == 2) //Poistka proti stlacaniu 2 klaves naraz, hlavne viditelne pri starsich PS2 klavesniciach
                {

                    if (tmpNewKeyBoardState.IsKeyDown(aUp) || tmpNewKeyBoardState.IsKeyDown(aRight) ||
                        tmpNewKeyBoardState.IsKeyDown(aDown) || tmpNewKeyBoardState.IsKeyDown(aLeft))
                    {

                        aUpdateNeeded = true;
                    }

                } 

                aOldKeyboardState = tmpNewKeyBoardState;
            }

            base.Update(gameTime);
        }
        /// <summary>
        /// Metoda reprezentujuca, kedy je update potrebny - pre Server, pokial si server zavola tuto metodu, zaroven ohlasi, ze update uz nie je potrebny a caka sa na dalsi...
        /// </summary>
        /// <returns></returns>
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
