using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ClientSide.Chat
{
    public class ChatInputBox : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// Objekt Hra - typ LogicGame
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Objekt GameWindow - Typ GameWindow
        /// </summary>
        private GameWindow aGameWindow;

        /// <summary>
        /// Textura pozadia
        /// </summary>
        private Texture2D aChatInputTexture2D;

        /// <summary>
        /// Vector reprezentujuci poziciu chatu - Typ Vector2
        /// </summary>
        private Vector2 aPositionVector;

        /// <summary>
        /// Rectangle reprezentujuci objekt - Input Box - Typ Rectangle
        /// </summary>
        private Rectangle aChatInputRectangle;

        /// <summary>
        /// Hodnota Boolean, ktora oznacuje ci je Input Otvoreny alebo Nie
        /// </summary>
        private bool isInputOpen = false;

        /// <summary>
        /// Tlacitko, ktore bolo stlacene pred aktualnym? -- Pozn. Lepsie popisat
        /// </summary>
        private char aPreviousCharacterPressed;

        private Keys aPreviousKeyPressed;

        /// <summary>
        /// Reprezentuje spravu na odoslanie - Typ - String
        /// </summary>
        private string aMessageToSend = "";

        /// <summary>
        /// Signalizacie pre Chat - ze mmze odoslat spravu ak je TRUE - Typ Boolean
        /// </summary>
        private bool isMessageReadyToBeStored = false;

        /// <summary>
        /// Vyska okna Input Boxu - Typ Int
        /// </summary>
        private int aWindowHeight;

        /// <summary>
        /// Sirka okna Input Boxu - Typ Int
        /// </summary>
        private int aWindowWidth;

        /// <summary>
        /// Maximalna dlzka spravy, defaultne 50 - Typ Int
        /// </summary>
        private int aMessageMaxLength = 50;

        /// <summary>
        /// Getter - MessageToSend
        /// </summary>
        public string MessageToSend { get => aMessageToSend; }

        /// <summary>
        /// Reprezentuje Scale Fontu - Typ Float
        /// </summary>
        private float aFontScale = 0.5F;

        private const float aFontEnlargingConstant = 0.025F;

        private float aDefaultFontScale;


        /// <summary>
        /// Getter a Setter - Boolean - Je Sprava Uz Uskladnena? 
        /// </summary>
        public bool IsMessageReadyToBeStored { get => isMessageReadyToBeStored; set => isMessageReadyToBeStored = value; }

        /// <summary>
        /// Konštruktor ChatInputBoxu - Vytvori ChatInputBox
        /// </summary>
        /// <param name="parGame">Parameter Hra - Typ 2DLogicGame</param>
        /// <param name="parGameWindow">Parameter Okno Hry - Typ GameWindow</param>
        /// <param name="parWidth">Parameter Specifikujuci Sirku Boxu - Typ Int</param>
        /// <param name="parHeight">Paramter Specifikujuci Vysku Boxu - Typ Int</param>
        /// <param name="parPosVector">Parameter Specifikujuci Poziciu Boxu - Typ Vector2</param>
        public ChatInputBox(LogicGame parGame, GameWindow parGameWindow, int parWidth, int parHeight, Vector2 parPosVector) : base(parGame)
        {
            aLogicGame = parGame;
            aGameWindow = parGameWindow;
            aPositionVector = parPosVector;
            aChatInputTexture2D = new Texture2D(aLogicGame.GraphicsDevice, parWidth, parHeight); //Inicializujeme si Texture2D ako Pozadie Inputu
            aChatInputRectangle = new Rectangle(0, 0, parWidth, parHeight);
            aWindowHeight = parHeight;
            aWindowWidth = parWidth;
            aDefaultFontScale = aFontScale;

        }

        /// <summary>
        /// Vykresli Chat Input Box a String Obsahujuci Text, Ktory uzivatel prave pise
        /// </summary>
        /// <param name="gameTime">Parameter GameTime - Cas</param>
        public override void Draw(GameTime gameTime)
        {
            if (isInputOpen)
            {
                // aLogicGame.SpriteBatch.Begin();
                aLogicGame.SpriteBatch.Draw(aChatInputTexture2D, aPositionVector ,aChatInputRectangle, Color.White, 0F, Vector2.Zero, 1F,  SpriteEffects.None, 0.1F); //Vykrasli ChatInputBox pomocou Textury, Rectangle a farby - Color.White zachovava povodne farby

                float tmpNextStringSize = (aLogicGame.Font.MeasureString(aMessageToSend).X * aFontScale) + aLogicGame.Font.LineSpacing; //Reprezentuje buducu moznu velkost Stringu s ohladom na Skalovanie
                float tmpPreviousStringSize = ((aLogicGame.Font.MeasureString(aMessageToSend).X * (aFontScale + aFontEnlargingConstant)) + aLogicGame.Font.LineSpacing); //Reprezentuje predoslu velkost Stringu s ohladom na Skalovanie

                if (tmpNextStringSize > aWindowWidth) //Ak je buduca Velkost Stringu väcsia ako momentalna sirka Input Boxu
                {
                    aFontScale = aFontScale - aFontEnlargingConstant;
                }
                else if (aFontScale != aDefaultFontScale && tmpPreviousStringSize < aWindowWidth)
                {
                    if (aFontScale < aDefaultFontScale)
                    {
                        aFontScale = aFontScale + aFontEnlargingConstant;
                    }
                }

                Vector2 tmpStringSizeVector = aLogicGame.Font.MeasureString(aMessageToSend) * aFontScale;

                Vector2 tmpVectorChat = aPositionVector + new Vector2(aWindowWidth / 2 - (int)tmpStringSizeVector.X / 2, aWindowHeight / 2 - (aLogicGame.Font.LineSpacing * aFontScale)/2); //Pomocny Pozicny Vektor pre Text Input Boxu

                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font, aMessageToSend, tmpVectorChat, Color.White, 0f, Vector2.Zero, aFontScale, SpriteEffects.None, 0F); //Vykresli String
                //aLogicGame.SpriteBatch.End();

            }
            base.Draw(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Update - Najprv kontroluje ci bolo tlacitko pre pisanie sprav stalcene prave jeden krat a uz nebol otvoreny Input Box
        /// Ak je Input uz Otvoreny, porovnavame este ci je este stale drzane predosle tlacitko, ak ako nastavime jemu korespondujuci character na '0' a Window.TexInputu priradime nas Handle
        /// V opacnom pripade, odoberieme nas Handle od Window.TextInputu
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (aLogicGame.CheckKeyPressedOnce(aLogicGame.ChatWriteMessageKey) && isInputOpen == false) //Neotvoreny Input Box
            {
                isInputOpen = !isInputOpen; //Prakticky to robi to, ze sa hodnota boolean zmeni na opacnu hodnotu... Usetrime riadky kodu
            }

            if (isInputOpen) //Otvoreny Input
            {
                if (Keyboard.GetState().IsKeyDown(aPreviousKeyPressed) == false)
                {
                    aPreviousCharacterPressed = '0';
                }
                aLogicGame.Window.TextInput += TextInputHandle;
            }
            else //Opacny Pripad
            {
                aLogicGame.Window.TextInput -= TextInputHandle;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Nacitava Texturu Pozadia pre Chat Input
        /// </summary>
        protected override void LoadContent()
        {
            aChatInputTexture2D = aLogicGame.Content.Load<Texture2D>("Sprites\\Backgrounds\\chatInputBackground");
            base.LoadContent();
        }


        /// <summary>
        /// Handle sluziaci na Text Input - Parametre vyplyvaju z Window.TextInputu...
        /// </summary>
        /// <param name="parSender">Parameter - Objekt odosielatel</param>
        /// <param name="parArgs">Parameter - Argumenty - Typ TextInputEventArgs</param>
        public void TextInputHandle(object parSender, TextInputEventArgs parArgs)
        {
            Keys tmpKeyPressed = parArgs.Key;
            char tmpCharacter = parArgs.Character;

            if (isInputOpen && tmpCharacter != aPreviousCharacterPressed) // Ak je Input Otvoreny a Predosly a Aktualny Character Nie je Rovnaky - Prejdeme takto duplikacii vstupu
            {
                if (tmpKeyPressed == Keys.Back)
                { //Ak je momentalne stlacene tlacitko rovne - BACKSPACU
                    this.TructMessage(); //Odstranime posledny charakter zo momentalne pisanej spravy
                    Debug.WriteLine(aMessageToSend); //Debug - Sprava do konzole
                }
                if (aLogicGame.Font.Characters.Contains(tmpCharacter)) //Overime si ci Font Hry obsahuje charakter, ktory sa snazime napisat
                {
                    this.AppendMessage(tmpCharacter); //Ak ano, priradime charakter ku sprave
                    Debug.WriteLine(aMessageToSend); //Debug - Sprava do konzole

                }
                if (tmpKeyPressed == Keys.Enter) //Ak je momentalne stlacene tlacitko rovne - ENTERU - Spravu sa snazi pouzivatel odoslat
                {
                    Debug.WriteLine("Chat Window Closed"); //Debug - Sprava do konzole, ze Chat je zatvoreny
                    isInputOpen = false; //Nastavime premennu, ktora symbolizuje ci je Input Otvoreny na - FALSE
                    IsMessageReadyToBeStored = true; //Nastavime premennu, ktora symbolizuje ci je Sprava Pripravena Na Ulozene (Pred odoslanim) na - TRUE
                }
            }

            aPreviousCharacterPressed = tmpCharacter; //Ulozime si tlacitko, ktore bolo stlacene
            aPreviousKeyPressed = tmpKeyPressed; //Ulozime si charakter, ktory bol stlaceny
        }

        /// <summary>
        /// Metoda, ktora sluzi na pridavanie znaku do Stringu reprezentujuceho Spravu
        /// </summary>
        /// <param name="parCharacterToAppend">Parameter Charakter na pridanie do Stringu - Typ Char</param>
        public void AppendMessage(char parCharacterToAppend)
        {
            if (aMessageToSend != null) //Ak string spravy nie je prazdny - Pre bezpecnost
            {
                if (aMessageToSend.Length < aMessageMaxLength) //Ak je dlzka spravy mensia ako specifikovane maximum
                {
                    this.aMessageToSend += parCharacterToAppend; //Pridame do spravy Charakter
                }
            }
            else
            {
                this.aMessageToSend += parCharacterToAppend; //Ak je sprava null - neexistuje, pridame tam prvy charakter
            }
        }

        /// <summary>
        /// Metoda, ktora odstranuje posledny charakter zo spravy
        /// </summary>
        public void TructMessage()
        {
            if (aMessageToSend.Length > 0) //Ak sprava obsahuje nejaky charakter
            {
                this.aMessageToSend = this.aMessageToSend.Remove(aMessageToSend.Length - 1); //Spravu odstranime tak, ze pouzijeme Metodu Remove, ktora odstranuje vsetky charaktery od nami specifikovaneho - Doprava
            }
        }

        /// <summary>
        /// Metoda, sluziaca na odstranenie spravy
        /// </summary>
        public void DeleteMessage()
        {
            IsMessageReadyToBeStored = false; //Nastavime premennu IsMessageReadyTobeStored na - FALSE
            aMessageToSend = ""; //Nastavime string reprezentujuci spravu na - "" - "Ako keby" ju zmazeme
        }
    }
}
