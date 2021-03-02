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

        private int aRenderTargetWidth = 1920;

        private int aRenderTargetHeight = 1080;

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
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            aMenuBox = new MenuBox(this, new Vector2(1100, 200), Color.FloralWhite, Color.Black, 120);
            aMenuBox.AddItem("Play", MenuItemAction.Start);
            aMenuBox.AddItem("Options", MenuItemAction.Options);
            aMenuBox.AddItem("Stats", MenuItemAction.Stats);
            aMenuBox.AddItem("Credits", MenuItemAction.Credits);
            aMenuBox.AddItem("Exit", MenuItemAction.Exit);
            aMenu = new Menu(this, aMenuBox);

            aMainMenu = new ComponentCollection(this, aMenu, aMenuBox);
            
            ClientSide.Chat.ChatInputBox chatInput = new ClientSide.Chat.ChatInputBox(this, Window, 1000, 246, new Vector2((aRenderTargetWidth-1000)/2, aRenderTargetHeight-246));
            aChat = new ClientSide.Chat.Chat(this, chatInput);

            aPlayingScreen = new ComponentCollection(this, aChat, chatInput);

       

            base.Initialize();

            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

        }
        
        protected override void LoadContent()
        {

            aRenderTarget = new RenderTarget2D(this.GraphicsDevice, aRenderTargetWidth, aRenderTargetHeight);
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

            if (aMenu.TaskToExecute != MenuTasksToBeExecuted.None)
            {
                switch (GameState)
                {
                    case GameState.MainMenu:
                        break;
                    case GameState.Playing:
                        SwitchScene(aMainMenu, aPlayingScreen);
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


            if (aChat != null && aChat.IsMessageWaitingToBeSent)
            {
                if (aClientClass != null)
                {

                    string tst = aChat.ReadAndTakeMessage();

                    aClientClass.SendChatMessage(tst);

                }
            }


            PreviousPressedKey = CurrentPressedKey;
            CurrentPressedKey = Keyboard.GetState();








            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            this.GraphicsDevice.SetRenderTarget(aRenderTarget);

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
            else
            {
                GraphicsDevice.Clear(Color.DarkGray);
            }

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            this.GraphicsDevice.SetRenderTarget(null);

            float tmpScale = 1F / (1080F / _graphics.GraphicsDevice.Viewport.Height);

            SpriteBatch.Begin();
            SpriteBatch.Draw(aRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, tmpScale, SpriteEffects.None, 0f);
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
                aClientReadThread.Join();
            }

            if (aClientClass != null)
            {

            }

            base.OnExiting(sender, args);


        }
    }
}
