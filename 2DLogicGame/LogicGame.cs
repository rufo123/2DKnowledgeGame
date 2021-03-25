using _2DLogicGame.GraphicObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading;
using _2DLogicGame.ClientSide.Levels;

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

        private ClientSide.Chat.Chat aChat;

        private KeyboardState aPreviousPressedKey;
        private KeyboardState aCurrentPressedKey;

        private ComponentCollection aMainMenu;
        private ComponentCollection aPlayingScreen;

        private GameState aGameState = GameState.MainMenu;

        private Server aServerClass;
        private Client aClientClass;

        private Thread aServerReadThread;
        private Thread aClientReadThread;

        private RenderTarget2D aRenderTarget;

        private PlayerController aPlayerController;

        private LevelManager aLevelManager;

        private int aRenderTargetWidth = 1920;

        private int aRenderTargetHeight = 1080;

        private int aBackBufferWidth = 1280; //1280

        private int aBackBufferHeight = 720; //720






        private float aScale = 1F;

        // Gettery a Settery

        public SpriteBatch SpriteBatch { get => _spriteBatch; set => _spriteBatch = value; } //Getter Setter SpriteBatch
        public SpriteFont Font { get => _font; set => _font = value; } //Getter Setter Font
        public KeyboardState PreviousPressedKey { get => aPreviousPressedKey; set => aPreviousPressedKey = value; }
        public KeyboardState CurrentPressedKey { get => aCurrentPressedKey; set => aCurrentPressedKey = value; }
        public GameState GameState { get => aGameState; set => aGameState = value; }
        public Keys UpKey { get => aUpKey; set => aUpKey = value; }
        public Keys DownKey { get => aDownKey; set => aDownKey = value; }
        public Keys ProceedKey { get => aProceedKey; set => aProceedKey = value; }
        public Keys ChatWriteMessageKey { get => aChatWriteMessageKey; set => aChatWriteMessageKey = value; }
        public RenderTarget2D RenderTarget { get => aRenderTarget; }
        public GraphicsDeviceManager Graphics { get => _graphics; set => _graphics = value; }
        public float Scale { get => aScale; }

        // public int RenderTargetWidth { get => aRenderTargetWidth;  }

        // public int RenderTargetHeight { get => aRenderTargetHeight; }
        //Keys

        private Keys aUpKey = Keys.W;
        private Keys aDownKey = Keys.S;
        private Keys aProceedKey = Keys.Space;
        private Keys aChatWriteMessageKey = Keys.T;


        public LogicGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1D / 30D);
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            aMenuBox = new MenuBox(this, new Vector2(1100, 200), Color.FloralWhite, Color.Black, 120);
            aMenuBox.AddItem("Host", MenuItemAction.Start_Host);
            aMenuBox.AddItem("Play", MenuItemAction.Start_Play);
            aMenuBox.AddItem("Options", MenuItemAction.Options);
            aMenuBox.AddItem("Stats", MenuItemAction.Stats);
            aMenuBox.AddItem("Exit", MenuItemAction.Exit);
            aMenu = new Menu(this, aMenuBox);

            aMainMenu = new ComponentCollection(this, aMenu, aMenuBox);

            ClientSide.Chat.ChatReceiveBox chatReceive = new ClientSide.Chat.ChatReceiveBox(this, Window, 593, 800, Vector2.Zero + new Vector2(10, 10));
            ClientSide.Chat.ChatInputBox chatInput = new ClientSide.Chat.ChatInputBox(this, Window, 1000, 246, new Vector2((aRenderTargetWidth - 1000) / 2, aRenderTargetHeight - 246));
            aChat = new ClientSide.Chat.Chat(this, chatInput, chatReceive);
            //Player tmpPlayer = new Player(0, this, new Vector2(800, 500), new Vector2(49, 64), Color.White);
            //PlayerController tmpController = new PlayerController(this, tmpPlayer);

            aPlayingScreen = new ComponentCollection(this, aChat, chatInput, chatReceive);

            aPlayerController = new GraphicObjects.PlayerController(this);

            // Components.Add(tmpController);

            aLevelManager = new LevelManager(this, aPlayingScreen);


            

            



            base.Initialize();

            Graphics.PreferredBackBufferWidth = aBackBufferWidth;
            Graphics.PreferredBackBufferHeight = aBackBufferHeight;
            Graphics.ApplyChanges();

          


            aScale = 1F / (1080F / _graphics.GraphicsDevice.Viewport.Height);
        }

        protected override void LoadContent()
        {

            Font = Content.Load<SpriteFont>("Fonts\\StickRegular12");

            aRenderTarget = new RenderTarget2D(this.GraphicsDevice, aRenderTargetWidth, aRenderTargetHeight);

            SpriteBatch = new SpriteBatch(GraphicsDevice);

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

            if (aMenu.TaskToExecute != MenuTasksToBeExecuted.None)
            {
                switch (GameState)
                {
                    case GameState.MainMenu:
                        break;
                    case GameState.Playing:

                        if (aMenu.TaskToExecute == MenuTasksToBeExecuted.Host_Start)
                        {
                            SwitchScene(aMainMenu, aPlayingScreen);
                            aServerClass = new Server("Test", this);
                            aClientClass = new Client("Test", this, aChat, aPlayingScreen, aPlayerController);

                            aServerReadThread = new Thread(new ThreadStart(aServerClass.ReadMessages));
                            aServerReadThread.Start();

                            aClientReadThread = new Thread(new ThreadStart(aClientClass.ReadMessages));
                            aClientReadThread.Start();

                            aLevelManager.InitLevelByNumber(1);

                            

                        }
                        else if (aMenu.TaskToExecute == MenuTasksToBeExecuted.Play_Start)
                        {

                            SwitchScene(aMainMenu, aPlayingScreen);

                            string tmpIP = "127.0.0.1";

                            aClientClass = new Client("Test", this, aChat, aPlayingScreen, aPlayerController, "Tester", tmpIP);

                            aClientReadThread = new Thread(new ThreadStart(aClientClass.ReadMessages));
                            aClientReadThread.Start();

                            aLevelManager.InitLevel("Levels\\levelMath");

                            
                        }
                        
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


            //Riadenie kolizie s podmienkou existencie Klienta, LevelManazera a vytvoreneho levelu
            if (aClientClass != null && aLevelManager != null && aLevelManager.IsLevelInitalized && aPlayerController != null)
            {
                aPlayerController.MoveRequest(gameTime);
                aClientClass.CollisionHandler(gameTime, aLevelManager);
                aPlayerController.ControlPlayerMovement(gameTime);

            }


            //Odosielanie dat, klienta
            if (aPlayerController != null)
            {
                if (aPlayerController.ConfirmUpdate() == true)
                {
                    aClientClass.SendClientData(); //Odosleme data len vtedy, pokial sa zmenil pohyb...
                }

            }


            //Citanie sprav
            if (aChat != null && aChat.IsMessageWaitingToBeSent)
            {
                if (aClientClass != null)
                {
                    string tst = aChat.ReadAndTakeMessage();

                    aClientClass.SendChatMessage(tst);

                    

                }
            }


            //Riadenie pohybu spoluhracov
            if (aClientClass != null && aLevelManager != null)
            {
                aClientClass.TeammateMovementHandler(gameTime, aLevelManager);
                
            }

           




            PreviousPressedKey = CurrentPressedKey;
            CurrentPressedKey = Keyboard.GetState();

            // TODO: Add your update logic here

            base.Update(gameTime);

         


            //MoveRequest



        }

        protected override void Draw(GameTime gameTime)
        {

            this.GraphicsDevice.SetRenderTarget(aRenderTarget);

            if (aServerClass != null && aClientClass != null)
            {
                if (aServerClass.Started == true && aClientClass.Connected == true)
                {
                    GraphicsDevice.Clear(Color.DeepSkyBlue);
                }
                else
                {
                    GraphicsDevice.Clear(Color.Red);
                }
            }
            else if (aClientClass != null)
            {

                if (aClientClass.Connected == true)
                {

                    GraphicsDevice.Clear(Color.Green);
                }
                else
                {

                    GraphicsDevice.Clear(Color.Orange);
                }


            }

            else
            {
                GraphicsDevice.Clear(Color.DarkGray);
            }

            // TODO: Add your drawing code here



            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.None, rasterizerState: RasterizerState.CullNone ,sortMode: SpriteSortMode.BackToFront, blendState: BlendState.AlphaBlend);
            base.Draw(gameTime);

            SpriteBatch.End();


            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.None, rasterizerState: RasterizerState.CullNone, sortMode: SpriteSortMode.BackToFront, blendState: BlendState.AlphaBlend);

            this.GraphicsDevice.SetRenderTarget(null);

            SpriteBatch.Draw(aRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, aScale, SpriteEffects.None, 0f);
            SpriteBatch.End();
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

            if (aServerClass != null)
            {
                aServerReadThread.Join();
            }

            if (aClientClass != null)
            {
                aClientReadThread.Join();
            }

            base.OnExiting(sender, args);


        }
    }
}
