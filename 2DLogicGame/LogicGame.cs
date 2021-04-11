using _2DLogicGame.GraphicObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using _2DLogicGame.ClientSide;
using _2DLogicGame.ClientSide.Levels;
using _2DLogicGame.GraphicObjects.Connecting;
using _2DLogicGame.GraphicObjects.Scoreboard;
using _2DLogicGame.ServerSide.Database;
using Microsoft.Xna.Framework.Media;

namespace _2DLogicGame
{
    public enum GameState
    {
        MainMenu,
        Submenu,
        Playing,
        Paused,
        Exit,
        Executed,
        Typing
    }

    public class LogicGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont aFont72;

        private SpriteFont aFont48;

        private SpriteFont aFont28;

        private MenuBox aMenuBox;
        private Menu aMenu;

        private MenuInput aNickNameInput;

        private MenuInput aIPAddressInput;

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

        private StatisticsHandler aStatisticsHandler;

        private ScoreboardUI aScoreboardUI;

        private ScoreboardController aScoreboardController;

        private ConnectingUI aConnectUI;

        private LevelGameCompletedScreen aCompletedScreen;

        private OptionsController aOptionsController;

        private int aRenderTargetWidth = 1920;

        private int aRenderTargetHeight = 1080;

        private int aBackBufferWidth = 1280; //1280

        private int aBackBufferHeight = 720; //720

        private float aCameraX;

        private float aScale = 1F;

        private int aDebugCalls = 0;

        private int aConnectionTimeout;

   

        // Gettery a Settery

        public SpriteBatch SpriteBatch { get => _spriteBatch; set => _spriteBatch = value; } //Getter Setter SpriteBatch
        public SpriteFont Font72 { get => aFont72; set => aFont72 = value; } //Getter Setter Font72
        public SpriteFont Font48 { get => aFont48; set => aFont48 = value; } //Getter Setter Font48
        public SpriteFont Font28 { get => aFont28; set => aFont28 = value; } //Getter Setter Font28
        public KeyboardState PreviousPressedKey { get => aPreviousPressedKey; set => aPreviousPressedKey = value; }
        public KeyboardState CurrentPressedKey { get => aCurrentPressedKey; set => aCurrentPressedKey = value; }
        public GameState GameState { get => aGameState; set => aGameState = value; }
        public Keys UpKey { get => aUpKey; set => aUpKey = value; }
        public Keys DownKey { get => aDownKey; set => aDownKey = value; }
        public Keys LeftKey { get => aLeftKey; set => aLeftKey = value; }
        public Keys RightKey { get => aRightKey; set => aRightKey = value; }
        public Keys ProceedKey { get => aProceedKey; set => aProceedKey = value; }
        public Keys ChatWriteMessageKey { get => aChatWriteMessageKey; set => aChatWriteMessageKey = value; }
        public Keys MusicLower { get => aMusicLower; set => aMusicLower = value; }
        public Keys MusicHigher { get => aMusicHigher; set => aMusicHigher = value; }
        public Keys MusicStartStop { get => aMusicStartStop; set => aMusicStartStop = value; }
        public RenderTarget2D RenderTarget { get => aRenderTarget; }
        public GraphicsDeviceManager Graphics { get => _graphics; set => _graphics = value; }
        public float Scale { get => aScale; }
        public float CameraX { get => aCameraX; set => aCameraX = value; }
        // public int RenderTargetWidth { get => aRenderTargetWidth;  }

        // public int RenderTargetHeight { get => aRenderTargetHeight; }
        //Keys

        private Keys aUpKey = Keys.W;
        private Keys aDownKey = Keys.S;
        private Keys aLeftKey = Keys.A;
        private Keys aRightKey = Keys.D;
        private Keys aProceedKey = Keys.Space;
        private Keys aChatWriteMessageKey = Keys.T;
        private Keys aMusicLower = Keys.Subtract; //Hudba tichsie
        private Keys aMusicHigher = Keys.Add; //Hudba hlasnejsie
        private Keys aMusicStartStop = Keys.M; //Zapnutie hudby

        //ODSTRANIT

        private Song aSong;
        private float aOldVolumeLevel;

        //ODSTRANIT


        public LogicGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1D / 30D);
            aCameraX = 0;

            //ODSTRANIT

            aOldVolumeLevel = 0;

            //ODSTRANIT
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            aOptionsController = new OptionsController(this);

            aOptionsController.AddOption("Hore", aUpKey, KeyTypes.UpKey);
            aOptionsController.AddOption("Dole", aDownKey, KeyTypes.DownKey);
            aOptionsController.AddOption("Dolava", aLeftKey, KeyTypes.LeftKey);
            aOptionsController.AddOption("Doprava", aRightKey, KeyTypes.RightKey);
            aOptionsController.AddOption("Potvrdit", aProceedKey, KeyTypes.ProceedKey);
            aOptionsController.AddOption("Pisat", aChatWriteMessageKey, KeyTypes.ChatWriteMessageKey);
            aOptionsController.AddOption("Hudba - Tichsie", aMusicLower, KeyTypes.MusicLower);
            aOptionsController.AddOption("Hudba - Hlasnejsie", aMusicHigher, KeyTypes.MusicHigher);
            aOptionsController.AddOption("Hudba - Stisit/Zapnut", aMusicStartStop, KeyTypes.MusicStartTop);

            aOptionsController.AddButton("Save", MenuButtonAction.Save);
            aOptionsController.AddButton("Reset", MenuButtonAction.ResetToDefault);

            aConnectionTimeout = 20; //Nastavime cas pre connection timeout

            aStatisticsHandler = new StatisticsHandler();
            aScoreboardUI = new ScoreboardUI(this);

            aCompletedScreen = new LevelGameCompletedScreen(this);

            aScoreboardController = new ScoreboardController(aStatisticsHandler, aScoreboardUI);

            aMenuBox = new MenuBox(this, new Vector2(1100, 200), Color.FloralWhite, Color.Black, Color.Gray, 120);
            aMenuBox.AddItem("Host", MenuItemAction.Start_Host);
            aMenuBox.AddItem("Play", MenuItemAction.Start_Play);
            aMenuBox.AddItem("Options", MenuItemAction.Options);
            aMenuBox.AddItem("Stats", MenuItemAction.Stats);
            aMenuBox.AddItem("Exit", MenuItemAction.Exit);

            aNickNameInput = new MenuInput(this, new Vector2(300, 500), new Vector2(500, 100), 25F, "Nickname", 10);

            aIPAddressInput = new MenuInput(this, new Vector2(300, 700), new Vector2(500, 100), 15F, "IP Address", 15, true);

            aConnectUI = new ConnectingUI(this, "Connecting", aConnectionTimeout, '.');


            aMenu = new Menu(this, aMenuBox, aScoreboardController, aNickNameInput, aIPAddressInput, aConnectUI, aOptionsController);

            aMainMenu = new ComponentCollection(this, aMenu, aMenuBox, aScoreboardUI, aNickNameInput, aIPAddressInput, aConnectUI, aOptionsController);


            ClientSide.Chat.ChatReceiveBox chatReceive = new ClientSide.Chat.ChatReceiveBox(this, Window, 593, 800, Vector2.Zero + new Vector2(10, 10));
            ClientSide.Chat.ChatInputBox chatInput = new ClientSide.Chat.ChatInputBox(this, Window, 1000, 246, new Vector2((aRenderTargetWidth - 1000) / 2, aRenderTargetHeight - 246));
            aChat = new ClientSide.Chat.Chat(this, chatInput, chatReceive);
            //Player tmpPlayer = new Player(0, this, new Vector2(800, 500), new Vector2(49, 64), Color.White);
            //PlayerController tmpController = new PlayerController(this, tmpPlayer);

            aPlayingScreen = new ComponentCollection(this, aChat, chatInput, chatReceive, aCompletedScreen);

            // Components.Add(tmpController);

            aLevelManager = new LevelManager(this, aPlayingScreen, aCompletedScreen);

            aOptionsController.InitKeysFromConfig();

            base.Initialize();

            Graphics.PreferredBackBufferWidth = aBackBufferWidth;
            Graphics.PreferredBackBufferHeight = aBackBufferHeight;
            

            Graphics.ApplyChanges();



            aScale = 1F / (1080F / _graphics.GraphicsDevice.Viewport.Height);
        }

        protected override void LoadContent()
        {
            //ODSTRANIT

            aSong = Content.Load<Song>("Music\\playing");

            //ODSTRANIT


            Font72 = Content.Load<SpriteFont>("Fonts\\StickRegular72");

            Font48 = Content.Load<SpriteFont>("Fonts\\StickRegular48");

            Font28 = Content.Load<SpriteFont>("Fonts\\StickRegular28");

            aRenderTarget = new RenderTarget2D(this.GraphicsDevice, aRenderTargetWidth, aRenderTargetHeight);

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            bck = Content.Load<Texture2D>("Sprites\\Backgrounds\\menuBackground");

            aMainMenu.SetVisibility(true);


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {

           

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || CheckKeyPressedOnce(Keys.Escape) || (aClientClass != null && aClientClass.ClientNeedsToShutdown)) //Ak je stlacene tlacitko ESCAPE alebo o vypnutie poziadal klient.
            {
                if (GameState == GameState.MainMenu)
                {
                    Debug.WriteLine("END");
                    this.GameState = GameState.Exit;
                    Exit();
                } 
                else if (GameState != GameState.MainMenu || (aClientClass != null && aClientClass.ClientNeedsToShutdown)) //Ak nie sme v hlavnom menu, alebo klient poziadal o vypnutie.
                {
                    Debug.WriteLine("BACK_TO_MENU");
                    this.GameState = GameState.MainMenu;
                    this.aMenu.TaskToExecute = MenuTasksToBeExecuted.None;

                    aServerClass = null;
                    aClientClass = null;

                    aLevelManager.DestroyLevelManagerData();
                    aCameraX = 0;

                    if (aChat != null)
                    {
                        aChat.ResetStorage();
                    }

                    SwitchScene(aPlayingScreen, aMainMenu);

                    MediaPlayer.Stop();



                }

            }

            if (this.GameState == GameState.Exit)
            {
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
                            //aLevelManager.InitLevelByNumber(2);

                            // SwitchScene(aMainMenu, aPlayingScreen);}}

                            string tmpNickName = "Player1";

                            if (aNickNameInput != null && aNickNameInput.InputText != "") //Ak existuje objekt a je definovany aj string
                            {
                                tmpNickName = aNickNameInput.InputText;
                            }

                            aPlayerController = new GraphicObjects.PlayerController(this);

                            aServerClass = new Server("Test", this);
                            aClientClass = new Client("Test", this, aChat, aPlayingScreen, aPlayerController, aLevelManager, aConnectionTimeout, tmpNickName);

                            aServerReadThread = new Thread(new ThreadStart(aServerClass.ReadMessages));
                            aServerReadThread.Start();

                            aClientReadThread = new Thread(new ThreadStart(aClientClass.ReadMessages));
                            aClientReadThread.Start();

                            aMenu.TaskToExecute = MenuTasksToBeExecuted.TryToConnect;



                        }
                        else if (aMenu.TaskToExecute == MenuTasksToBeExecuted.Play_Start)
                        {
                           

                            string tmpIP = "127.0.0.1";
                            // string tmpIP = "25.81.200.231";}

                            if (aIPAddressInput != null && aIPAddressInput.InputText != "") //Ak existuje objekt Menu Input IP Adresy a je nejaka definovana
                            {
                                tmpIP = aIPAddressInput.InputText;
                            }

                            string tmpNickName = "Player2";

                            if (aNickNameInput != null && aNickNameInput.InputText != "") //Ak existuje objekt a je definovany aj string
                            {
                                tmpNickName = aNickNameInput.InputText;
                            }

                            aPlayerController = new GraphicObjects.PlayerController(this);

                            aClientClass = new Client("Test", this, aChat, aPlayingScreen, aPlayerController, aLevelManager, aConnectionTimeout, tmpNickName, tmpIP);

                            aClientReadThread = new Thread(new ThreadStart(aClientClass.ReadMessages));
                            aClientReadThread.Start();

                            aMenu.TaskToExecute = MenuTasksToBeExecuted.TryToConnect;

                        }
                        
                        break;
                    case GameState.Paused:
                        break;
                    case GameState.Exit:
                        break;
                    default:
                        break;
                }

                
            }

            if (aClientClass != null)
            {
                if (aClientClass.Connected && aLevelManager.IsLevelInitalized == false)
                {
                    aLevelManager.InitLevelByNumber(2);
                    SwitchScene(aMainMenu, aPlayingScreen);
                    MediaPlayer.Play(aSong);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = 0.1F;


                }
            }

            if (aGameState == GameState.Playing)
            {
                if (aSong != null)
                {
                    if (aGameState != GameState.Typing)
                    {
                        if (CheckKeyPressedOnce(aMusicHigher))
                        {
                            if (MediaPlayer.Volume < 1)
                            {
                                MediaPlayer.Volume += 0.1F;
                            }

                        } else if (CheckKeyPressedOnce(aMusicLower))
                        {

                            if (MediaPlayer.Volume > 0)
                            {
                                MediaPlayer.Volume -=0.1F;
                            }

                        } else if (CheckKeyPressedOnce(aMusicStartStop))
                        {
                            if (MediaPlayer.Volume <= 0)
                            {
                                MediaPlayer.Volume = aOldVolumeLevel;
                            }
                            else
                            {
                                aOldVolumeLevel = MediaPlayer.Volume;
                                MediaPlayer.Volume = 0;
                            }

                        }

                    }
                }
            }


            //Riadenie kolizie s podmienkou existencie Klienta, LevelManazera a vytvoreneho levelu
            if (aClientClass != null && aLevelManager != null && aLevelManager.IsLevelInitalized && aPlayerController != null)
            {
                aPlayerController.ControlRequest(gameTime);
                aClientClass.CollisionHandler(gameTime, aLevelManager);
                aPlayerController.ControlPlayerMovement(gameTime);

            }


            //Odosielanie dat, klienta
            if (aPlayerController != null && aClientClass != null)
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


            //Kontrola ci LevelManager nepotrebuje odoslat update | Kontrola ci nie je potrebny respawn hracov
            if (aLevelManager != null && aClientClass != null)
            {

                if (aClientClass.Connected && aLevelManager.LevelName == "NONE")
                {
                    

                }

                //Skontrolujem ci aj LevelManager nepotrebuje nieco aktualizovat
                aLevelManager.Update(parGameTime: gameTime);

                if (aLevelManager.LevelChanged) 
                {
                    aClientClass.HandleRespawnPlayers(gameTime);
                    aLevelManager.LevelChanged = false;
                } else if (aLevelManager.LevelReset)
                {
                    aClientClass.HandleRespawnPlayers(gameTime);
                    aLevelManager.LevelReset = false;
                }

            }

            if (this.CurrentPressedKey.IsKeyDown(Keys.M))
            {
                aConnectUI.StartTimer = true;
            }
            else if (this.CurrentPressedKey.IsKeyDown(Keys.N))
            {
                aConnectUI.StartTimer = false;
            }


            //Riadenie pohybu spoluhracov
            if (aClientClass != null && aLevelManager != null)
            {
                aClientClass.TeammateMovementHandler(gameTime, aLevelManager);
                
            }



            PreviousPressedKey = CurrentPressedKey;
            CurrentPressedKey = Keyboard.GetState();

            if (aClientClass != null)
            {
                aDebugCalls++;
            }

            // TODO: Add your update logic here
          //  Thread.Sleep(1);

              base.Update(gameTime);

            //ControlRequest

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



            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.None, rasterizerState: RasterizerState.CullNone ,sortMode: SpriteSortMode.BackToFront, blendState: BlendState.AlphaBlend, transformMatrix: Matrix.CreateTranslation(aCameraX, 0, 0));
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

            GameState = GameState.MainMenu;

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
