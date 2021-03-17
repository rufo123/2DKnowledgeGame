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

        private Keys aOldKeyPressed = Keys.None;

        private bool aUpdateNeeded = false;

        public bool UpdateNeeded { get => aUpdateNeeded; set => aUpdateNeeded = value; }

        private bool aOldMoving = false;

        private float aMovementUpdateTime = 0;

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
                    tmpCurrentKeyPresed = aUp;
                    aPlayer.IsMoving = true;
                    aPlayer.SetDirection(Entity.Direction.UP);

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                    aPlayer.Move(gameTime);
                   



                }
                else if (tmpNewKeyBoardState.IsKeyDown(aRight))
                {
                    tmpCurrentKeyPresed = aRight;
                    aPlayer.IsMoving = true;
                    aPlayer.SetDirection(Entity.Direction.RIGHT);

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                    aPlayer.Move(gameTime);
                    
                }
                else if (tmpNewKeyBoardState.IsKeyDown(aLeft))
                {
                    tmpCurrentKeyPresed = aLeft;
                    aPlayer.IsMoving = true;
                    aPlayer.SetDirection(Entity.Direction.LEFT);

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                    aPlayer.Move(gameTime);
                   

                    

                }
                else if (tmpNewKeyBoardState.IsKeyDown(aDown))
                {
                    tmpCurrentKeyPresed = aDown;
                    aPlayer.IsMoving = true;
                    aPlayer.SetDirection(Entity.Direction.DOWN);

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                    aPlayer.Move(gameTime);
                    

                    

                }
                else
                {
                    aPlayer.IsMoving = false;
                    aPlayer.MovementVector = new Vector2(0,0);
                    tmpCurrentKeyPresed = Keys.None;

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                }

                

                //Kedy je update pre server potrebny?

                /*  if (tmpNewKeyBoardState.IsKeyDown(tmpCurrentKeyPresed) && aOldKeyboardState.IsKeyUp(tmpCurrentKeyPresed)) //Porovname, ci uzivatel stlacil nove tlacitko
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

                  }  */

                aOldKeyPressed = tmpCurrentKeyPresed;
                aOldKeyboardState = tmpNewKeyBoardState;
                aOldMoving = aPlayer.IsMoving;
            }

            base.Update(gameTime);
        }
        /// <summary>
        /// Metoda reprezentujuca, kedy je update potrebny - pre Server, pokial si server zavola tuto metodu, zaroven ohlasi, ze update uz nie je potrebny a caka sa na dalsi...
        /// </summary>
        /// <returns></returns>
        public bool ConfirmUpdate()
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
