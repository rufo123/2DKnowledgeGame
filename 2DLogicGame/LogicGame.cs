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
    /// <summary>
    /// Enumeracna trieda reperzentujuca stavy hry.
    /// </summary>
    public enum GameState
    {
        MainMenu, //Hlavne Menu
        Submenu, //Podmenu
        Playing, //Hra je hrana
        Paused, //Hra je pozastavena
        Exit, //Hra je ukoncena
        Executed, //Nepouzite
        Typing //Hrac pise
    }

    /// <summary>
    /// Trieda, ktora reprezentuje hru.
    /// </summary>
    public class LogicGame : Game
    {
        /// <summary>
        /// Atribut, reprezentujuci manazera grafickeho zariadenia - typ GraphicsDeviceManager.
        /// </summary>
        private GraphicsDeviceManager _graphics;

        /// <summary>
        /// Atribut, reprezentujuci SpriteBatch, teda objekt na ktory sa vykresluje graficka cast hry - typ SpriteBatch.
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Atribut, reprezentujuci font o velkosti 72 - typ SpriteFont;
        /// </summary>
        private SpriteFont aFont72;

        /// <summary>
        /// Atribut, reprezentujuci font o velkosti 48 - typ SpriteFont;
        /// </summary>
        private SpriteFont aFont48;

        /// <summary>
        /// Atribut, reprezentujuci font o velkosti 28 - typ SpriteFont;
        /// </summary>
        private SpriteFont aFont28;

        /// <summary>
        /// Atribut, reprezentujuci Menu Box - typ MenuBox.
        /// </summary>
        private MenuBox aMenuBox;

        /// <summary>
        /// Atribut, reprezentujuci Menu - typ Menu.
        /// </summary>
        private Menu aMenu;

        /// <summary>
        /// Atribut, reprezentujuci polozku, ktora podporuje vstup prezyvky hraca - typ MenuInput.
        /// </summary>
        private MenuInput aNickNameInput;

        /// <summary>
        /// Atribut, reprezentujuci polozku, ktora podporuje vstup IP adresy - typ MenuInput.
        /// </summary>
        private MenuInput aIPAddressInput;

        /// <summary>
        /// Atribut, ktory reprezentuje chyby, ktore sa vykresluju v menu - typ MenuErrors;
        /// </summary>
        private MenuErrors aMenuErrors;

        /// <summary>
        /// Atribut, ktory reprezentuje texturu pozadia menu - typ Texture2D.
        /// </summary>
        private Texture2D aBackgroundTexture;

        /// <summary>
        /// Atribut, ktory reprezentuje manazera chatu - typ Chat.
        /// </summary>
        private ClientSide.Chat.Chat aChat;

        /// <summary>
        /// Atribut, ktory reprezentuje predosly stav klavesnice - typ KeyboardState.
        /// </summary>
        private KeyboardState aPreviousPressedKey;

        /// <summary>
        /// Atribut, ktory reprezentuje momentalny stav klavenice - typ KeyboradState.
        /// </summary>
        private KeyboardState aCurrentPressedKey;

        /// <summary>
        /// Atribut, ktora reprezentuje kolekciu komponentov, reprezentujucich hlavne menu - typ ComponentCollection.
        /// </summary>
        private ComponentCollection aMainMenu;

        /// <summary>
        /// Atribut, ktora reprezentuje kolekciu komponentov, reprezentujucich hraciu obrazovku - typ ComponentCollection.
        /// </summary>
        private ComponentCollection aPlayingScreen;

        /// <summary>
        /// Atribut, ktory reprezentuje stav hry - typ GameState - enum.
        /// </summary>
        private GameState aGameState = GameState.MainMenu;

        /// <summary>
        /// Atribut, ktory reprezentuje Server - typ Server.
        /// </summary>
        private Server aServerClass;

        /// <summary>
        /// Atribut, ktory reprezentuje Klienta - typ Klient.
        /// </summary>
        private Client aClientClass;

        /// <summary>
        /// Atribut - Vlakno, ktore sluzi na prijmanie sprav u servera - typ Thread.
        /// </summary>
        private Thread aServerReadThread;

        /// <summary>
        /// Atribut - Vlakno, ktore sluzi na prijmanie sprav u klienta - typ Thread.
        /// </summary>
        private Thread aClientReadThread;

        /// <summary>
        /// Atribut, ktory reprezentuje ciel na aky sa maju vykresli objekty, napr 1920x1080 a nasledne sa takato obrazovka pretransformuje.
        /// Robi sa to aby sa nemuseli prisposobovat velkosti pre rozne velkosti obrazoviek.
        /// </summary>
        private RenderTarget2D aRenderTarget;

        /// <summary>
        /// Atribut, reprezentujuci ovladac postavy hraca - typ PlayerController.
        /// </summary>
        private PlayerController aPlayerController;

        /// <summary>
        /// Atribut, reprezentujuci manazera urovni - typ LevelManager.
        /// </summary>
        private LevelManager aLevelManager;

        /// <summary>
        /// Atribut, reprezentujuci spravcu hodnotiacej tabulky - typ StatisticsHandler.
        /// </summary>
        private StatisticsHandler aStatisticsHandler;

        /// <summary>
        /// Atribut, reprezentujuci graficku cast hodnotiacej tabulky - typ ScoreboardUI.
        /// </summary>
        private ScoreboardUI aScoreboardUI;

        /// <summary>
        /// Atribut, reprezentujuci ovladac hodnotiacej tabulky - typ ScoreboardController.
        /// </summary>
        private ScoreboardController aScoreboardController;

        /// <summary>
        /// Atribut, reprezentujuci graficku cast obrazovky pri pripajani - typ ConnectingUI.
        /// </summary>
        private ConnectingUI aConnectUI;

        /// <summary>
        /// Atribut, reprezentujuci obrazovky pri dokonceni hry - typ LevelGameCompletedScreen.
        /// </summary>
        private LevelGameCompletedScreen aCompletedScreen;

        /// <summary>
        /// Atribut, reprezentujuci ovladac nastaveni klaves - typ OptionsController.
        /// </summary>
        private OptionsController aOptionsController;

        /// <summary>
        /// Atribut, ktory reprezentuje indikator stavu databazy hodnotenia hracov, ci je zapnuta alebo nie - typ MenuStatsIndicator.
        /// </summary>
        private MenuStatsIndicator aMenuStatIndicator;

        /// <summary>
        /// Atribut, ktroy reprezentuje potvrdzovaciu obrazovku pri ukoncovani urovne - typ LevelQuitConfirm.
        /// </summary>
        private LevelQuitConfirm aLevelQuitConfirm;

        /// <summary>
        /// Atribut, ktory reprezentuje pozadie urovne - typ Texture2D.
        /// </summary>
        private Texture2D aLevelBackgroundTexture;

        /// <summary>
        /// Atribut, ktory reprezentuje casovac, ktory indikuje za kolko sa pokusi znova pripojit znova k databaze, ak pripojenie zlyhalo - typ float.
        /// </summary>
        private float aReconnectDatabaseTimer;

        /// <summary>
        /// Atribut, ktory reprezentuje sirku RenderTarget-u - typ int.
        /// </summary>
        private int aRenderTargetWidth = 1920;

        /// <summary>
        /// Atribut, ktory reprezentuje vysku RenderTarget-u - typ int.
        /// </summary>
        private int aRenderTargetHeight = 1080;

        /// <summary>
        /// Atribut, ktory reprezentuje realnu sirku na aku sa ma vykreslit hra - typ int.
        /// </summary>
        private int aBackBufferWidth = 1280; //1280

        /// <summary>
        /// Atribut, ktory reprezentuje realnu vysku na aku sa ma vykreslit hra - typ int.
        /// </summary>
        private int aBackBufferHeight = 720; //720

        /// <summary>
        /// Atribut, ktory reprezentuje prednastavenu sirku na aku sa ma vykreslit hra - typ int.
        /// </summary>
        private int aDefaultBackBufferWidth;

        /// <summary>
        /// Atribut, ktory reprezentuje prednastavenu vysku na aku sa ma vykreslit hra - typ int.
        /// </summary>
        private int aDefaultBackBufferHeight;

        /// <summary>
        /// Atribut, ktory reprezentuje offset x-ovej suradnice kamery - typ float.
        /// </summary>
        private float aCameraX;

        /// <summary>
        /// Atribut, ktory reprezentuje skalu vykreslovanie - typ float.
        /// </summary>
        private float aScale = 1F;

        /// <summary>
        /// Atribut, ktory sa vyuziva pri debugovani, kolko volani metody prebehlo - typ int.
        /// </summary>
        private int aDebugCalls = 0;

        /// <summary>
        /// Atribut, ktory reprezentuje timeout pripojenie - typ int.
        /// </summary>
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

        // Atributy jednotlive klavesy, ktore sa vyuzivaju pri hrani hry.
        private Keys aUpKey = Keys.W;
        private Keys aDownKey = Keys.S;
        private Keys aLeftKey = Keys.A;
        private Keys aRightKey = Keys.D;
        private Keys aProceedKey = Keys.Space;
        private Keys aChatWriteMessageKey = Keys.T;
        private Keys aMusicLower = Keys.Subtract; //Hudba tichsie
        private Keys aMusicHigher = Keys.Add; //Hudba hlasnejsie
        private Keys aMusicStartStop = Keys.M; //Zapnutie hudby

        /// <summary>
        /// Atribut, reprezentujuci hudbu, ktora je prehravana pri hrani urovni - typ Song.
        /// </summary>
        private Song aSong;

        /// <summary>
        /// Atribut, reprezentujuci predoslu uroven hlasitosti hudby - typ float.
        /// </summary>
        private float aOldVolumeLevel;



        /// <summary>
        /// Konstruktor triedy hry.
        /// </summary>
        public LogicGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1D / 30D);
            aCameraX = 0;

            aDefaultBackBufferWidth = aBackBufferWidth;
            aDefaultBackBufferHeight = aBackBufferHeight;

            //ODSTRANIT

            aOldVolumeLevel = 0;

            //ODSTRANIT
        }

        /// <summary>
        /// Metoda, ktora sa stara o inicializaciu hry.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            this.Window.Title = "2D Vedomostna hra | Vytvoril: Rudolf Šimo, 2021 | Bakalarska praca";

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

            aOptionsController.AddButton("Ulozit", MenuButtonAction.Save);
            aOptionsController.AddButton("Reset", MenuButtonAction.ResetToDefault);

            aConnectionTimeout = 20; //Nastavime cas pre connection timeout

            aStatisticsHandler = new StatisticsHandler();
            aScoreboardUI = new ScoreboardUI(this);

            aCompletedScreen = new LevelGameCompletedScreen(this);

            aLevelQuitConfirm = new LevelQuitConfirm(this, new Vector2(aRenderTargetWidth / 2F, aRenderTargetHeight / 2F), new Vector2(500, 300));

            aScoreboardController = new ScoreboardController(aStatisticsHandler, aScoreboardUI);

            aMenuStatIndicator = new MenuStatsIndicator(this, new Vector2(1050, 900), "Statistika");

            aMenuBox = new MenuBox(this, new Vector2(1100, 200), Color.FloralWhite, Color.Black, Color.Gray, 120);
            aMenuBox.AddItem("Host", MenuItemAction.Start_Host);
            aMenuBox.AddItem("Play", MenuItemAction.Start_Play);
            aMenuBox.AddItem("Options", MenuItemAction.Options);
            aMenuBox.AddItem("Stats", MenuItemAction.Stats);
            aMenuBox.AddItem("Exit", MenuItemAction.Exit);

            aNickNameInput = new MenuInput(this, new Vector2(300, 500), new Vector2(500, 100), 25F, "Prezyvka", 10);

            aIPAddressInput = new MenuInput(this, new Vector2(300, 700), new Vector2(500, 100), 15F, "IP Adresa", 15, true);

            aConnectUI = new ConnectingUI(this, "Pripajanie", aConnectionTimeout, '.');

            aMenuErrors = new MenuErrors(this, new Vector2(aRenderTargetWidth / 2F, 50));

            aMenu = new Menu(this, aMenuBox, aScoreboardController, aNickNameInput, aIPAddressInput, aConnectUI, aOptionsController, aMenuStatIndicator, aMenuErrors);

            aMainMenu = new ComponentCollection(this, aMenu, aMenuBox, aScoreboardUI, aNickNameInput, aIPAddressInput, aConnectUI, aOptionsController, aMenuStatIndicator, aMenuErrors);


            ClientSide.Chat.ChatReceiveBox chatReceive = new ClientSide.Chat.ChatReceiveBox(this, Window, 593, 800, Vector2.Zero + new Vector2(10, 10));
            ClientSide.Chat.ChatInputBox chatInput = new ClientSide.Chat.ChatInputBox(this, Window, 1000, 246, new Vector2((aRenderTargetWidth - 1000) / 2F, aRenderTargetHeight - 246));
            aChat = new ClientSide.Chat.Chat(this, chatInput, chatReceive);
            //Player tmpPlayer = new Player(0, this, new Vector2(800, 500), new Vector2(49, 64), Color.White);
            //PlayerController tmpController = new PlayerController(this, tmpPlayer);

            aPlayingScreen = new ComponentCollection(this, aChat, chatInput, chatReceive, aCompletedScreen, aLevelQuitConfirm);

            // Components.Add(tmpController);

            aLevelManager = new LevelManager(this, aPlayingScreen, aCompletedScreen);

            aOptionsController.InitKeysFromConfig();

            aReconnectDatabaseTimer = 0F;

            aLevelBackgroundTexture = new Texture2D(this.GraphicsDevice, aRenderTargetWidth, aRenderTargetHeight);

            Window.AllowUserResizing = true; //Povolime resize okna
            Window.ClientSizeChanged += OnSizeOfWindowChanged; //Priradime zmenu okna metode, ktora bude po tejto zmene volana

            base.Initialize();

            Graphics.PreferredBackBufferWidth = aBackBufferWidth;
            Graphics.PreferredBackBufferHeight = aBackBufferHeight;

            Graphics.ApplyChanges();

            aScale = 1F / (1080F / _graphics.GraphicsDevice.Viewport.Height);
        }

        /// <summary>
        /// Metoda, ktora sa stara o nacitanie obsahu hry.
        /// </summary>
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

            aBackgroundTexture = Content.Load<Texture2D>("Sprites\\Backgrounds\\menuBackground");

            aMainMenu.SetVisibility(true);

            if (aLevelBackgroundTexture != null)
            {
                aLevelBackgroundTexture = this.Content.Load<Texture2D>("Sprites\\Backgrounds\\gameBCK");
            }


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizaciu obsahu hry.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        protected override void Update(GameTime parGameTime)
        {

            if (CheckKeyPressedOnce(Keys.F1)) //Klavesa ktora vypne FullScreen
            {
                if (Graphics.IsFullScreen) //Ak je nastaveny "FullScreen", vypneme ho
                {
                    Graphics.IsFullScreen = false;

                    //Prepiseme velkost Back Buffera
                    aBackBufferHeight = aDefaultBackBufferHeight;
                    aBackBufferWidth = aDefaultBackBufferWidth;

                    Graphics.PreferredBackBufferWidth = aBackBufferWidth;
                    Graphics.PreferredBackBufferHeight = aBackBufferHeight;

                    //Vycentrujeme okno s ohladom na to, ze pouzivatel moze mat viac monitorov a okno moze mat otvorene na sekundarnom monitory
                    Window.Position = new Point(
                        (Window.ClientBounds.Left + (Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width - aBackBufferWidth) / 2),
                        (Window.ClientBounds.Top + (Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height - aBackBufferHeight) / 2)
                        );

                    Graphics.ApplyChanges();
                }
            }


            if (aClientClass != null && aClientClass.ClientNeedsToShutdown)
            {
                aMenuErrors.SetErrorMessage(aClientClass.DisconnectMessage);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || CheckKeyPressedOnce(Keys.Escape) || (aClientClass != null && aClientClass.ClientNeedsToShutdown)) //Ak je stlacene tlacitko ESCAPE alebo o vypnutie poziadal klient.
            {
                if (GameState == GameState.MainMenu)
                {
                    Debug.WriteLine("END");
                    this.GameState = GameState.Exit;
                    Exit();
                }
                else if (GameState == GameState.Typing && aChat != null && aChat.ChatInputBox.IsInputOpen && aClientClass != null && !aClientClass.ClientNeedsToShutdown) //Ak hrac nie je menu a ma otvorene okno na pisanie, zavrieme ho.
                {
                    //Pri zatvarani chat input okna, zmazame aj rozpisanu spravu.
                    aChat.ChatInputBox.DeleteMessage();
                    aChat.ChatInputBox.IsInputOpen = false;
                }
                else if (GameState == GameState.Playing && aLevelQuitConfirm != null && !aLevelQuitConfirm.ShowConfirm && aClientClass != null && !aClientClass.ClientNeedsToShutdown && (aLevelManager != null && !aLevelManager.GameCompleted)) //Ak hrac nie je v menu a stlaci ESC, zobrazime potvrdzovacie okno, pre vypnutie hry, zaroven nastavime, ze potvrdzovacie okno sa zobrazi, len ak este nie je dokoncena hra.
                {
                    if (aLevelQuitConfirm != null)
                    {
                        aLevelQuitConfirm.ShowConfirm = true;
                    }
                }
                else if ((GameState != GameState.MainMenu && aLevelQuitConfirm != null && aLevelQuitConfirm.ShowConfirm) || GameState == GameState.Submenu || (aClientClass != null && aClientClass.ClientNeedsToShutdown) || (aLevelManager != null && aLevelManager.GameCompleted)) //Ak nie sme v hlavnom menu, alebo klient poziadal o vypnutie.
                {//Do menu sa dostaneme hned pokia - Je uz zobrazeny Confirm Box a stlacime Escape | sme v submenu | Client poziadal o vypnutie | Hra bola dokoncena.
                    Debug.WriteLine("BACK_TO_MENU");

                    if (aLevelQuitConfirm != null && aLevelQuitConfirm.ShowConfirm) //Ak je otvorene okno s potvrdenim navratu do menu, zavrieme ho.
                    {
                        aLevelQuitConfirm.ShowConfirm = false;
                    }

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


            if (CheckKeyPressedOnce(aProceedKey) && aLevelQuitConfirm != null && aLevelQuitConfirm.ShowConfirm)  //Ak je stlacene tlacidlo potvrdenia - ProceedKey - prednastavene - Space - Medzernik.
            { //A ak bolo pri stlaceni potvrdenia otvorene potvrdzovacie okno, zavrieme ho.

                aLevelQuitConfirm.ShowConfirm = false;
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


                            if (aServerClass != null && aServerClass.Started)
                            {
                                aClientClass = new Client("Test", this, aChat, aPlayingScreen, aPlayerController, aLevelManager, aConnectionTimeout, tmpNickName);

                                aServerReadThread = new Thread(new ThreadStart(aServerClass.ReadMessages));
                                aServerReadThread.Start();

                                aClientReadThread = new Thread(new ThreadStart(aClientClass.ReadMessages));
                                aClientReadThread.Start();

                                aMenu.TaskToExecute = MenuTasksToBeExecuted.TryToConnect;
                            }
                            else //Pokial sa stane ze Port a IP uz je obsadena, Host nebude fungovat
                            {
                                aMenu.TaskToExecute = MenuTasksToBeExecuted.None;

                                aPlayerController = null;

                                if (aMenuErrors != null)
                                {
                                    aMenuErrors.SetErrorMessage("IP Adresa/Port je uz obsadena/y.");
                                    aGameState = GameState.MainMenu;
                                }
                            }



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

            if (aClientClass != null) //Inicializacia Levelu
            {
                if (aClientClass.Connected && aLevelManager.IsLevelInitalized == false)
                {
                    aLevelManager.InitLevelByNumber(1);
                    SwitchScene(aMainMenu, aPlayingScreen);
                    MediaPlayer.Play(aSong);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = 0.1F;
                }
            }

            if (aMenu != null && (aGameState == GameState.MainMenu || aGameState == GameState.Submenu) && aStatisticsHandler != null && aMenuStatIndicator != null) //Pokus o pripojenie sa do databazy, pokial by sa pripojenie nezdarilo
            {
                aMenuStatIndicator.TurnOnOff(aStatisticsHandler.IsConnected ? IndicatorState.On : IndicatorState.Off); //Ak je pripojenie k databaze uspesne - on, inak off.

                if (!aStatisticsHandler.IsConnected) //Ak sa pripojenie k databaze nezdarilo
                {
                    aReconnectDatabaseTimer += parGameTime.ElapsedGameTime.Seconds; //Zacneme pripocitavat sekundy do casovaca

                    if (aReconnectDatabaseTimer > 60) //Ak casovac dosiahne hodnotu 60 sekund - 1 minuty
                    {
                        aStatisticsHandler.RetryConnect(); //Pokusime sa znova pripojit
                    }
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

                        }
                        else if (CheckKeyPressedOnce(aMusicLower))
                        {

                            if (MediaPlayer.Volume > 0)
                            {
                                MediaPlayer.Volume -= 0.1F;
                            }

                        }
                        else if (CheckKeyPressedOnce(aMusicStartStop))
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
                aPlayerController.ControlRequest(parGameTime);
                aClientClass.CollisionHandler(parGameTime, aLevelManager);
                aPlayerController.ControlPlayerMovement(parGameTime);

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
                aLevelManager.Update(parGameTime: parGameTime);

                if (aLevelManager.LevelChanged)
                {
                    aClientClass.RequestLevelData();
                    aClientClass.HandleRespawnPlayers(parGameTime);
                    aLevelManager.LevelChanged = false;

                }
                else if (aLevelManager.LevelReset)
                {
                    aClientClass.HandleRespawnPlayers(parGameTime);
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
                aClientClass.TeammateMovementHandler(parGameTime, aLevelManager);

            }



            PreviousPressedKey = CurrentPressedKey;
            CurrentPressedKey = Keyboard.GetState();

            if (aClientClass != null)
            {
                aDebugCalls++;
            }

            // TODO: Add your update logic here
            //  Thread.Sleep(1);

            base.Update(parGameTime);

            //ControlRequest

        }

        /// <summary>
        /// Metoda, ktora sa stara o vykreslovanie hry.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        protected override void Draw(GameTime parGameTime)
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



            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.None, rasterizerState: RasterizerState.CullNone, sortMode: SpriteSortMode.BackToFront, blendState: BlendState.AlphaBlend, transformMatrix: Matrix.CreateTranslation((float)CameraX * (Scale / aScale) , 0, 0));

            if (aClientClass != null && aClientClass.Connected)
            {
                SpriteBatch.Draw(aLevelBackgroundTexture, Vector2.Zero, new Rectangle(0, 0, aLevelBackgroundTexture.Width, aLevelBackgroundTexture.Height), Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 1F);
            }

            base.Draw(parGameTime);

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

            if (aServerClass != null && aServerClass.Started)
            {
                aServerReadThread.Join();
            }

            if (aClientClass != null)
            {
                aClientReadThread.Join();
            }

            base.OnExiting(sender, args);


        }

        /// <summary>
        /// Metoda, ktora "pocuva", na zmenu velkosti okna
        /// </summary>
        /// <param name="parSender">Parameter - Sender.</param>
        /// <param name="parArgs">Parameter - Argumenty.</param>
        public void OnSizeOfWindowChanged(Object parSender, EventArgs parArgs)
        {
            float tmpAspectRatio = aRenderTargetHeight / (float)aRenderTargetWidth;

            Window.ClientSizeChanged -= OnSizeOfWindowChanged;

            //Najprv nastavime novu velkost okna do atributov
            if (aBackBufferWidth != Window.ClientBounds.Width && Graphics.IsFullScreen == false) //Ak sa zmenila sirka
            {
                aBackBufferWidth = Window.ClientBounds.Width;
                aBackBufferHeight = (int)(Window.ClientBounds.Width * tmpAspectRatio);
            }
            if (aBackBufferHeight != Window.ClientBounds.Height && Graphics.IsFullScreen == false) //Ak sa zmenila vyska
            {
                aBackBufferHeight = Window.ClientBounds.Height;
                aBackBufferWidth = (int) (Window.ClientBounds.Height / tmpAspectRatio);
            }

            if (Window.ClientBounds.Width == this.GraphicsDevice.Adapter.CurrentDisplayMode.Width) //Ak je sirka okna rovna maximalnej velkosti okna, predpokladame ze doslo k maximalizacii okna, zarovname okno
            {
                Graphics.IsFullScreen = true;
                aBackBufferHeight = Window.ClientBounds.Height;
                aBackBufferWidth = Window.ClientBounds.Width;
            }
            
            //A tuto velkost realne nastavime oknu
            Graphics.PreferredBackBufferWidth = aBackBufferWidth;
            Graphics.PreferredBackBufferHeight = aBackBufferHeight;

            aScale = 1F / (1080F / aBackBufferHeight);

            Graphics.ApplyChanges();


            Window.ClientSizeChanged += OnSizeOfWindowChanged;

        }

    }
}
