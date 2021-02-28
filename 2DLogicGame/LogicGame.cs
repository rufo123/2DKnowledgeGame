using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading;

namespace _2DLogicGame
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Exit,
        Executed,
    }

    public class LogicGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private MenuBox aMenuBox;
        private Menu aMenu;
        private Texture2D bck;

        private KeyboardState aPreviousPressedKey;
        private KeyboardState aCurrentPressedKey;

        private ComponentCollection aMainMenu;

        private GameState aGameState = GameState.MainMenu;

        private Server aServerClass;
        private Client aClientClass;

        private Thread aServerReadThread;
        private Thread aClientReadThread;

        // Gettery a Settery

        public SpriteBatch SpriteBatch { get => _spriteBatch; set => _spriteBatch = value; } //Getter Setter SpriteBatch
        public SpriteFont Font { get => _font; set => _font = value; } //Getter Setter Font
        public KeyboardState PreviousPressedKey { get => aPreviousPressedKey; set => aPreviousPressedKey = value; }
        public KeyboardState CurrentPressedKey { get => aCurrentPressedKey; set => aCurrentPressedKey = value; }
        public GameState GameState { get => aGameState; set => aGameState = value; }
        public Keys UpKey { get => aUpKey; set => aUpKey = value; }
        public Keys DownKey { get => aDownKey; set => aDownKey = value; }
        public Keys ProceedKey { get => aProceedKey; set => aProceedKey = value; }


        //Keys

        private Keys aUpKey = Keys.W;
        private Keys aDownKey = Keys.S;
        private Keys aProceedKey = Keys.Space;

        public LogicGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            aMenuBox = new MenuBox(this, new Vector2(500, 100), Color.FloralWhite, Color.Black, 50);
            aMenuBox.AddItem("Play", MenuItemAction.Start);
            aMenuBox.AddItem("Options", MenuItemAction.Options);
            aMenuBox.AddItem("Stats", MenuItemAction.Stats);
            aMenuBox.AddItem("Credits", MenuItemAction.Credits);
            aMenuBox.AddItem("Exit", MenuItemAction.Exit);
            aMenu = new Menu(this, aMenuBox);

            aMainMenu = new ComponentCollection(this, aMenu, aMenuBox);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("Fonts\\StickRegular12");
            bck = Content.Load<Texture2D>("Sprites\\Backgrounds\\menuBackground");


            aMainMenu.SetVisibility(true);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Debug.WriteLine("END");
                GameState = GameState.Exit;
                Exit();
            }

            

           

            if (aMenu.TaskToExecute != MenuTasksToBeExecuted.None) {
                switch (GameState)
                {
                    case GameState.MainMenu:
                        break;
                    case GameState.Playing:
                        SwitchScene(aMainMenu);
                        aServerClass = new Server("Test", this);
                        aClientClass = new Client("Test", this);

                        aServerReadThread = new Thread(new ThreadStart(aServerClass.ReadMessages));
                        aServerReadThread.Start();

                        aClientReadThread = new Thread(new ThreadStart(aClientClass.ReadMessages));
                        aClientReadThread.Start();

                        break;
                    case GameState.Paused:
                        break;
                    case GameState.Exit:
                        Exit();
                        break;
                    default:
                        break;
                }

                aMenu.TaskToExecute = MenuTasksToBeExecuted.None;
            }


            if (this.CheckKeyPressedOnce(Keys.A))
            {
                if (aClientClass != null) {

                    bool tst = aClientClass.SendChatMessage("Testerino Sprava");

                    Debug.WriteLine("Bolo stlacene A - hodnota: " + tst);

                }
            }


            PreviousPressedKey = CurrentPressedKey;
            CurrentPressedKey = Keyboard.GetState();



            


         

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            if (aServerClass != null && aClientClass != null)
            {

                if (aServerClass.Started == true && aClientClass.Connected == true)
                {
                    GraphicsDevice.Clear(Color.LimeGreen);
                }
                else
                {
                    GraphicsDevice.Clear(Color.Red);
                }

            }
            else {
                GraphicsDevice.Clear(Color.DarkGray);
            }
            

            SpriteBatch.Begin();
            SpriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        /// <summary>
        /// Metoda, ktora kontroluje ci bolo tlacidlo stlacene prave raz, zabranuje "spamu", drzanim tlacitka
        /// </summary>
        /// <param name="parKey">Parameter typu Keys - Ake tlacidlo chcem porovnat</param>
        /// <returns></returns>
        public bool CheckKeyPressedOnce(Keys parKey)
        {
            if (this.CurrentPressedKey.IsKeyDown(parKey) && this.PreviousPressedKey.IsKeyUp(parKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Metód slúži na prepínanie scén napr z Main Menu do Hry
        /// </summary>
        /// <param name="parOldScene">Parameter reprezentujúci Starú - Resp. Momentálne zobrazenú Scénu</param>
        /// <param name="parNewScene">Parameter reprezentujúci Novú - Teda scénu, ktorá sa má zobraziť.</param>
        public void SwitchScene(ComponentCollection parOldScene, ComponentCollection parNewScene = null)
        {
            parOldScene.SetVisibility(false);
            if (parNewScene != null) //Len pre Testovanie, potom odstranit, tak isto, parameter New Scene nemôze byť nikdy null, NEZABUDNUT
            {
                parNewScene.SetVisibility(true);
            }
        }

        /// <summary>
        /// On Exiting s vyvola presne v tedy, ked napriklad stlacime tlacitko X - Vyriesene Joinovanie Threadov
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(object sender, EventArgs args)
        {

            GameState = GameState.Exit;

            aServerReadThread.Join();
            aClientReadThread.Join();

            base.OnExiting(sender, args);

            
        }
    }
}
