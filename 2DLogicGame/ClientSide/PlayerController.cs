using _2DLogicGame.ClientSide;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Trida, ktora sa stara o ovladanie pohybu a interagovania hraca. - Klient.
    /// </summary>
    public class PlayerController : GameComponent
    {
        /// <summary>
        /// Atribut, ktory reprezentuje data o hracoch. Klient.
        /// </summary>
        private PlayerClientData aPlayer;

        /// <summary>
        /// Atibut, reprezentujuci hru - typ LogicGame.
        /// </summary>
        private LogicGame aGame;

        /// <summary>
        /// Atribut, reprezentujuci poslednu stlacenu klavesu - typ Keys.
        /// </summary>
        private Keys aOldKeyPressed = Keys.None;

        /// <summary>
        /// Atribut, reprezentujuci ci je update potrebny - typ bool.
        /// </summary>
        private bool aUpdateNeeded = false;

        /// <summary>
        /// Atribut, reprezentujuci ci sa hrac predtym pohyboval - typ bool.
        /// </summary>
        private bool aOldMoving = false;

        /// <summary>
        /// Atribut, reprezentujuci GameTime.
        /// </summary>
        private GameTime aGameTime;

        /// <summary>
        /// Atribut, reprezentujuci predosly stav klavesnice - typ KeyboardState.
        /// </summary>
        private KeyboardState aOldKeyboardState;

        public bool UpdateNeeded
        {
            get => aUpdateNeeded;
            set => aUpdateNeeded = value;
        }

        public GameTime GameTime
        {
            get => aGameTime;
            set => aGameTime = value;
        }

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


        /// <summary>
        /// Metoda, ktora nastavi hraca, ktory bude ovladany - Controllerom
        /// </summary>
        public void SetPlayer(PlayerClientData parPlayer)
        {
            this.aPlayer = parPlayer;
        }


        /// <summary>
        /// Metoda, ktora si vyziada, resp zmeni Smer a odosle informaciu, ze sa hrac snazi alebo nesnazi pohnut ako dalsie odosle aj informaciu o tom ci sa hrac snazi s niecim interagovat
        /// </summary>
        public void ControlRequest(GameTime parGameTime)
        {

            this.aGameTime = parGameTime; //Nacitame aj GameTime

            if (aPlayer != null && aPlayer.Connected && aGame.GameState != GameState.Typing)
            {
                KeyboardState tmpNewKeyBoardState = Keyboard.GetState();

                Keys tmpCurrentKeyPresed = Keys.None;


                if (tmpNewKeyBoardState.IsKeyDown(aGame.UpKey))
                {
                    tmpCurrentKeyPresed = aGame.UpKey;
                    aPlayer.SetDirection(Entity.Direction.UP);

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                    aPlayer.IsTryingToMove = true;
                }
                else if (tmpNewKeyBoardState.IsKeyDown(aGame.RightKey))
                {
                    tmpCurrentKeyPresed = aGame.RightKey;
                    aPlayer.SetDirection(Entity.Direction.RIGHT);

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                    aPlayer.IsTryingToMove = true;
                }
                else if (tmpNewKeyBoardState.IsKeyDown(aGame.LeftKey))
                {
                    tmpCurrentKeyPresed = aGame.LeftKey;
                    aPlayer.SetDirection(Entity.Direction.LEFT);

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                    aPlayer.IsTryingToMove = true;
                }
                else if (tmpNewKeyBoardState.IsKeyDown(aGame.DownKey))
                {
                    tmpCurrentKeyPresed = aGame.DownKey;
                    aPlayer.SetDirection(Entity.Direction.DOWN);

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                    aPlayer.IsTryingToMove = true;
                }
                else
                {
                    aPlayer.IsTryingToMove = false;
                    aPlayer.MovementVector = new Vector2(0, 0);
                    tmpCurrentKeyPresed = Keys.None;

                    if (tmpCurrentKeyPresed != aOldKeyPressed)
                    {
                        aUpdateNeeded = true;
                    }

                }



                aOldKeyPressed = tmpCurrentKeyPresed;
                aOldKeyboardState = tmpNewKeyBoardState;
                aOldMoving = aPlayer.IsTryingToMove;
            }
        }

        /// <summary>
        /// Metoda, ktora realne ovlada pohyb hraca - vola Move
        /// </summary>
        /// <param name="parGameTime"></param>
        public void ControlPlayerMovement(GameTime parGameTime)
        {

            this.aGameTime = parGameTime; //Nacitame tie aj GameTime

            if (aPlayer != null)
            {
                if (aOldMoving == true && aPlayer.IsBlocked == true) //Ak sa pred tym hrac pohyboval, a teraz je v stave zablokovany
                {
                    aUpdateNeeded = true;
                }

                if (aGame.CheckKeyPressedOnce(aGame.ProceedKey) && aPlayer.WantsToInteract == false) //Ak je stlacene tlacitko interakcie, k zruseniu WantToInteract prebehne priemo po interakcii
                {
                    aPlayer.WantsToInteract = true;
                    aUpdateNeeded = true;
                    Debug.WriteLine("Tuk - True");
                }

                aPlayer.Move(gameTime: parGameTime);
            }
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
