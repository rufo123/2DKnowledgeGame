using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using _2DLogicGame.ClientSide;
using _2DLogicGame.GraphicObjects;
using _2DLogicGame.GraphicObjects.Connecting;
using _2DLogicGame.GraphicObjects.Scoreboard;
using _2DLogicGame.ServerSide.Database;
using SharpFont;
// ReSharper disable InvalidXmlDocComment

namespace _2DLogicGame
{
    /// <summary>
    /// Enumeracna trieda - Reprezentuje ake ulohy maju byt vykonane.
    /// </summary>
    public enum MenuTasksToBeExecuted
    {
        None,
        Host_Start,
        Play_Start,
        Exit,
        Enroll_Credits,
        Change_Options,
        Show_Stats,
        TryToConnect,
        Connecting
    }

    /// <summary>
    /// Trieda, ktora reprezentuje menu.
    /// </summary>
    public class Menu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// Atribut, ktory reprezentuje MenuBox - typ MenuBox.
        /// </summary>
        private MenuBox aMenuBox;

        /// <summary>
        /// Atribut, ktory reprezentuje hru - typ LogicGame.
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut, reprezentujuci texuturu pozadia menu - typ Texture2D.
        /// </summary>
        private Texture2D aMenuBackground;

        /// <summary>
        /// Atribut, reprezentujuci ovladac hodnotenia hry - typ ScoreboardController.
        /// </summary>
        private ScoreboardController aScoreboardController;

        /// <summary>
        /// Atribut, reprezentujuci vstup prezyvky hraca - typ MenuInput.
        /// </summary>
        private MenuInput aNickNameInput;

        /// <summary>
        /// Atribut, reprezentujuci vstup IP Adresy - typ MenuInput.
        /// </summary>
        private MenuInput aIPInput;

        /// <summary>
        /// Atribut, reprezentujuci List vstupov - typ List<MenuInput>.
        /// </summary>
        private List<MenuInput> aListOfInputs;

        /// <summary>
        /// Atribut, reprezentujuci zvoleny vstup, ci uz IP adresy alebo prezyvky - typ MenuInput.
        /// </summary>
        private MenuInput aSelectedInput;

        /// <summary>
        /// Atribut, reprezentujuci obrazovku pripajania sa do hry - typ ConnectingUI.
        /// </summary>
        private ConnectingUI aConnectingUI;

        /// <summary>
        /// Atribut, reprezentujuci ovladac nastaveni klaves - typ OptionsController.
        /// </summary>
        private OptionsController aOptionsController;

        /// <summary>
        /// Atribut, reprezentujuci indikator stavu pripojenia k databaze hodnotenia hracov v menu - typ MenuStatsIndicator.
        /// </summary>
        private MenuStatsIndicator aStatusIndicator;

        /// <summary>
        /// Atribut, reprezentujuci graficku cast vykreslovania chyb pri pripojovani sa do hry - typ MenuErrors.
        /// </summary>
        private MenuErrors aMenuErrors;

        /// <summary>
        /// Atribut, reprezentujuci zvolene cislene ID vstupu - typ int.
        /// </summary>
        private int aSelectedInputID;

        /// <summary>
        /// Atribut, reprezentujuci aka uloha ma byt vykonana po zvoleni si polozky v MenuBoxe - typ MenuTasksToBeExecuted.
        /// </summary>
        private MenuTasksToBeExecuted aTaskToExecute;

        public MenuTasksToBeExecuted TaskToExecute
        {
            get => aTaskToExecute;
            set => aTaskToExecute = value;
        }


        /// <summary>
        /// Konstruktor Menu
        /// </summary>
        /// <param name="parGame"> Parameter Hra - typu LogicGame </param>
        /// <param name="parMenuBox"> Parameter Menu Boxu - typu MenuBox</param>
        /// <param name="aScoreboardController"></param>
        /// <param name="parNickNameInput"></param>
        /// <param name="parIpInput"></param>
        public Menu(LogicGame parGame, MenuBox parMenuBox, ScoreboardController aScoreboardController, MenuInput parNickNameInput, MenuInput parIpInput, ConnectingUI parConnectingUI, OptionsController parOptionsController, MenuStatsIndicator parStatusIndicator, MenuErrors parMenuErrors) : base(parGame)
        {
            this.aLogicGame = parGame;
            this.aMenuBox = parMenuBox;
            this.aScoreboardController = aScoreboardController;
            this.aConnectingUI = parConnectingUI;
            this.aStatusIndicator = parStatusIndicator;
            this.aMenuErrors = parMenuErrors;

            aNickNameInput = parNickNameInput; //Rozdelil som tieto input, preto takto a nedal som ich do listu, lebo sa budu zobrazovat az po prejdeni na urcity menu item
            aIPInput = parIpInput;

            aListOfInputs = new List<MenuInput>(2);

            aListOfInputs.Add(parNickNameInput);
            aListOfInputs.Add(parIpInput);
            if (aListOfInputs != null)
            {
                aSelectedInput = aListOfInputs[0];
            }
            else
            {
                aSelectedInput = null;
            }


            aOptionsController = parOptionsController;

            aSelectedInputID = 0;
        }


        /// <summary>
        /// Metoda, ktora sa stara o nacitanie pozadia menu.
        /// </summary>
        protected override void LoadContent()
        {
            aMenuBackground = aLogicGame.Content.Load<Texture2D>("Sprites\\Backgrounds\\menuBackground");
            base.LoadContent();
        }

        /// <summary>
        /// Metoda, ktora sa stara o vykreslenie pozadia menu.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Draw(GameTime parGameTime)
        {

            //  aLogicGame.SpriteBatch.Begin();
            aLogicGame.SpriteBatch.Draw(aMenuBackground, new Vector2(0, 0), null, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.1F);

            //Vykreslenie nazvu autora hudby, prepocitanie suradnic do praveho dolneho rohu
            Vector2 tmpVectorOfMusicCredits = new Vector2(aLogicGame.RenderTarget.Width - aLogicGame.Font28.MeasureString("Music by: Rolemusic").X, aLogicGame.RenderTarget.Height - aLogicGame.Font28.MeasureString("Music by: Rolemusic").Y );
            aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, "Music by: Rolemusic", tmpVectorOfMusicCredits, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);

            //Vykreslenie autora hry, prepocitanie suradnic do laveho dolneho rohu
            Vector2 tmpVectorOfAuthorCredits = new Vector2(0, aLogicGame.RenderTarget.Height - aLogicGame.Font28.MeasureString("Created by: Rudolf Simo").Y);
            aLogicGame.SpriteBatch.DrawString(aLogicGame.Font28, "Created by: Rudolf Simo", tmpVectorOfAuthorCredits, Color.White, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
            // aLogicGame.SpriteBatch.End();


            base.Draw(parGameTime);
        }

        /// <summary>
        /// Metoda, ktora inicializuje objekt. Nastavi, ze uloha, ktora ma byt na zaciatku vykonana nebude nijaka a teda ziadna.
        /// </summary>
        public override void Initialize()
        {
            this.TaskToExecute = MenuTasksToBeExecuted.None;
            base.Initialize();
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizaciu dat v menu.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Update(GameTime parGameTime)
        {
            if (aMenuBox != null && aLogicGame != null && aMenuBox.BoxEnabled && aLogicGame.GameState != GameState.Typing)
            {

                if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.DownKey))
                {
                    if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.DownKey)) //Pohyb Hore
                    {
                        if (aMenuBox.SelectingFromMenuBox)
                        {
                            aMenuBox.MoveNextPrev(false); // Posuvanie Dopredu - false
                        }
                        else
                        {
                            MoveToNextInput(true); //Ideme na dalsi
                        }
                    }

                }
                if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.UpKey))
                {
                    if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.UpKey)) //Pohyb Dole
                    {
                        if (aMenuBox.SelectingFromMenuBox)
                        {
                            aMenuBox.MoveNextPrev(true); // Posuvanie Dozadu - true
                        }
                        else
                        {
                            MoveToNextInput(false); //Ideme na predosly
                        }
                    }

                }

                if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.LeftKey))
                {
                    if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.LeftKey)) //Pohyb Dole
                    {
                        //Nastavime, ze si uz nevybera z MenuBoxu
                        if (aMenuBox.SelectingFromMenuBox)
                        {
                            aMenuBox.SelectingFromMenuBox = !CheckIfAnyInputEnabled(); //Ak zistime ze ziaden z Input Boxov nie je povoleny, nastavime SelectingFromMenuBox na opacnu hodnotu
                            //Ak bude input povoleny -> true tak Selecting sa nastavi na false, cize prejdeme k input boxom
                        }
                        else
                        {
                            aMenuBox.SelectingFromMenuBox = !aMenuBox.SelectingFromMenuBox;
                        }

                    }
                }
                if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.RightKey))
                {
                    if (aLogicGame.CurrentPressedKey.IsKeyDown(this.aLogicGame.RightKey)) //Pohyb Dole
                    {
                        //Nastavime, ze si uz nevybera z MenuBoxu
                        if (aMenuBox.SelectingFromMenuBox)
                        {
                            aMenuBox.SelectingFromMenuBox = !CheckIfAnyInputEnabled(); //Ak zistime ze ziaden z Input Boxov nie je povoleny, nastavime SelectingFromMenuBox na opacnu hodnotu
                            //Ak bude input povoleny -> true tak Selecting sa nastavi na false, cize prejdeme k input boxom
                        }
                        else
                        {
                            aMenuBox.SelectingFromMenuBox = !aMenuBox.SelectingFromMenuBox;
                        }

                    }

                }

            }

            if (aMenuBox != null && !aMenuBox.SelectingFromMenuBox) //Ak si uz nevyberame z Menu Boxu ale z inputov
            {
                for (int i = 0; i < aListOfInputs.Count; i++)
                {
                    if (aListOfInputs[i] == aSelectedInput)
                    {
                        aListOfInputs[i].Hover = true;
                    }
                    else
                    {
                        aListOfInputs[i].Hover = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < aListOfInputs.Count; i++)
                {
                    aListOfInputs[i].Hover = false;
                }
            }

            if (aLogicGame != null &&  aMenuBox != null && (aLogicGame.GameState == GameState.MainMenu || aLogicGame.GameState == GameState.Submenu))
            {
                switch (aMenuBox.GetNameOfHoverOn()
                ) //Ak bude uzivatel ukazovat na Play alebo Host, zobrazia sa mu aj Input Boxy
                {
                    case "Play":
                        if (aNickNameInput != null)
                        {
                            aNickNameInput.InputEnabled = true;
                            aIPInput.InputEnabled = true;
                        }

                        break;
                    case "Host":
                        if (aNickNameInput != null && aIPInput != null)
                        {
                            aNickNameInput.InputEnabled = true;
                            aIPInput.InputEnabled = false;
                        }
                        break;
                    default:
                        if (aListOfInputs != null)
                        {
                            for (int i = 0; i < aListOfInputs.Count; i++)
                            {
                                aListOfInputs[i].InputEnabled = false;
                            }
                        }
                        break;
                }
            }

            if (aMenuBox != null && aScoreboardController != null && aConnectingUI != null && aOptionsController != null && aStatusIndicator != null && aMenuErrors != null)
            { // Jednotliva sprava uloh, ake maju byt vykonane v menu a logikou spojenou s nimi.
                switch (TaskToExecute)
                {
                    case MenuTasksToBeExecuted.None:
                        if (aScoreboardController != null)
                        {
                            aScoreboardController.ShowStats(false);
                            aConnectingUI.StartTimer = false;
                            aOptionsController.EnabledOptions = false;
                            aStatusIndicator.IndicatorEnabled = true;
                            aMenuErrors.ShowError = true;
                            aMenuBox.BoxEnabled = true;

                        }

                        MenuHandle(); //Pokial nie je nic zvolene zobrazi sa hlavna cast menu
                        break;
                    case MenuTasksToBeExecuted.Enroll_Credits:
                        break;
                    case MenuTasksToBeExecuted.Change_Options:
                        if (aOptionsController != null)
                        {
                            aMenuBox.BoxEnabled = false;
                            aOptionsController.EnabledOptions = true;
                            aStatusIndicator.IndicatorEnabled = false;
                            aMenuErrors.ShowError = false;
                            aMenuErrors.DeleteErrorMessage();
                        }

                        break;
                    case MenuTasksToBeExecuted.Show_Stats:
                        if (aScoreboardController != null && aScoreboardController != null &&
                            aScoreboardController.IsConnected())
                        {
                            aMenuBox.BoxEnabled = false;
                            aScoreboardController.InitScoreboard();
                            aScoreboardController.ShowStats(true);
                            aStatusIndicator.IndicatorEnabled = false;
                            aMenuErrors.ShowError = false;
                            aMenuErrors.DeleteErrorMessage();
                        }

                        break;
                    case MenuTasksToBeExecuted.Exit:
                        break;
                    case MenuTasksToBeExecuted.TryToConnect:
                        if (aConnectingUI != null)
                        {
                            aMenuBox.BoxEnabled = false;
                            aConnectingUI.StartTimer = true;
                            aStatusIndicator.IndicatorEnabled = false;
                            aMenuErrors.ShowError = false;
                            aMenuErrors.DeleteErrorMessage();
                            if (aListOfInputs != null)
                            {
                                for (int i = 0; i < aListOfInputs.Count; i++)
                                {
                                    aListOfInputs[i].InputEnabled = false;
                                }
                            }

                            TaskToExecute = MenuTasksToBeExecuted.Connecting;
                        }

                        break;
                    case MenuTasksToBeExecuted.Connecting:
                        if (aConnectingUI != null && aLogicGame != null)
                        {
                            aStatusIndicator.IndicatorEnabled = false;
                            if (aConnectingUI.ConnectionTimeout <= 0)
                            {
                                TaskToExecute = MenuTasksToBeExecuted.None;
                                aLogicGame.GameState = GameState.MainMenu;
                            }

                            aMenuErrors.DeleteErrorMessage();
                            aMenuErrors.ShowError = false;
                        }

                        break;
                    default:
                        //throw new ArgumentOutOfRangeException();
                        break;
                }
            }



            base.Update(parGameTime);
        }

        /// <summary>
        /// Metoda, ktora sluzi na spravu akcii spojenych s MenuBoxom
        /// </summary>
        public void MenuHandle()
        {
            if (aLogicGame.CheckKeyPressedOnce(this.aLogicGame.ProceedKey))
            {
                if (aMenuBox.SelectingFromMenuBox)
                {
                    switch (this.aMenuBox.SelectedItem.Action)
                    {
                        case MenuItemAction.Start_Host:
                            Debug.WriteLine("Host");
                            aLogicGame.GameState = GameState.Playing;
                            this.TaskToExecute = MenuTasksToBeExecuted.Host_Start;
                            break;
                        case MenuItemAction.Start_Play:
                            Debug.WriteLine("Play");
                            aLogicGame.GameState = GameState.Playing;
                            this.TaskToExecute = MenuTasksToBeExecuted.Play_Start;
                            break;
                        case MenuItemAction.Options:
                            Debug.WriteLine("Options");
                            aLogicGame.GameState = GameState.Submenu;
                            this.TaskToExecute = MenuTasksToBeExecuted.Change_Options;
                            break;
                        case MenuItemAction.Stats:
                            Debug.WriteLine("Stats");
                            aLogicGame.GameState = GameState.Submenu;
                            this.TaskToExecute = MenuTasksToBeExecuted.Show_Stats;
                            break;
                        case MenuItemAction.Exit:
                            Debug.WriteLine("Exit");
                            aLogicGame.GameState = GameState.Exit;
                            this.TaskToExecute = MenuTasksToBeExecuted.Exit;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (aListOfInputs != null && aSelectedInput != null)
                    {
                        aSelectedInput.Focus = !aSelectedInput.Focus; //Nastavime Focus na opacnu hodnotu

                        if (aSelectedInput.Focus) //Zaleziac od toho ci je Focus povoleny ale nie nastavime GameState na Typing
                        {
                            aLogicGame.GameState = GameState.Typing;
                        }
                        else
                        {
                            aLogicGame.GameState = GameState.Submenu;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Metoda, ktora posuva vybar na dalsi input, pokial je povoleny.
        /// </summary>
        /// <param name="parNext">Parameter, ktory reprezentuje ci si uzivatel chce vybrat Input dalsi - true, alebo predosly - false</param>
        public void MoveToNextInput(bool parNext)
        {
            bool tmpSomeInputEnabled = false;
            if (aListOfInputs != null) //Najprv si positeme ci vobec nejaky Input je povoleny
            {
                for (int i = 0; i < aListOfInputs.Count; i++)
                {
                    if (aListOfInputs[i].InputEnabled)
                    {
                        tmpSomeInputEnabled = true;
                    }

                }
            }

            if (aListOfInputs != null && tmpSomeInputEnabled)
            {
                if (parNext) //Dopredu
                {
                    aSelectedInputID++;

                    if (aSelectedInputID == aListOfInputs.Count) //Kontrola aby sme nepresli na index vacsi ako je pocet prvkov v Liste
                    {
                        aSelectedInputID = 0;
                    }
                    while (aListOfInputs[aSelectedInputID].InputEnabled == false) //Budeme prechadzat itemy az pokial nenajdeme taky, ktory je povolen, az potom pojde dalsia kontrola
                    {
                        aSelectedInputID++;
                        if (aSelectedInputID == aListOfInputs.Count)
                        {
                            aSelectedInputID = 0;
                        }
                    }
                    if (aSelectedInputID < aListOfInputs.Count)
                    {

                        aSelectedInput = aListOfInputs[aSelectedInputID];
                    }
                    else
                    {
                        aSelectedInputID = 0;
                        aSelectedInput = aListOfInputs[aSelectedInputID];
                    }
                }
                else //Dozadu
                {
                    aSelectedInputID--;

                    if (aSelectedInputID < 0) //Kontrola aby sme nepresli na index -1
                    {
                        aSelectedInputID = aListOfInputs.Count - 1;
                    }
                    while (aListOfInputs[aSelectedInputID].InputEnabled == false) //Budeme prechadzat itemy az pokial nenajdeme taky, ktory je povoleny, az potom pojde dalsia kontrola
                    {
                        aSelectedInputID--;

                        if (aSelectedInputID < 0)
                        {
                            aSelectedInputID = aListOfInputs.Count - 1;
                        }
                    }
                    if (aSelectedInputID >= 0)
                    {

                        aSelectedInput = aListOfInputs[aSelectedInputID];
                    }
                    else
                    {
                        aSelectedInputID = aListOfInputs.Count - 1;
                        aSelectedInput = aListOfInputs[aSelectedInputID];
                    }
                }
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o kontrolu Input Itemov, ci je nejaky z nich povoleny;
        /// </summary>
        /// <returns></returns>
        public bool CheckIfAnyInputEnabled()
        {
            if (aListOfInputs != null)
            {
                for (int i = 0; i < aListOfInputs.Count; i++) //Ak najdeme jeden taky Input ktory je povoleny
                {
                    if (aListOfInputs[i].InputEnabled)
                    {
                        return true;
                    }
                }
            }
            return false; //Inak vratime false
        }


    }
}

