using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpFont;

namespace _2DLogicGame
{
    /// <summary>
    /// Trieda, ktora reprezentuje vstupnu polozku v Menu.
    /// </summary>
    public class MenuInput : DrawableGameComponent
    {

        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame.
        /// </summary>
        private LogicGame aGame;

        /// <summary>
        /// Atribut, reprezentujuci textutu - typ Texture2D.
        /// </summary>
        private Texture2D aTexture;

        /// <summary>
        /// Atribut, reprezentujuci rectangle - typ Rectangle.
        /// </summary>
        private Rectangle aRectangle;

        /// <summary>
        /// Atribut, reprezentujuci poziciu - typ Vector2.
        /// </summary>
        private Vector2 aPosition;

        /// <summary>
        /// Atribut, reprezentujuci velkost - typ Vector2.
        /// </summary>
        private Vector2 aSize;

        /// <summary>
        /// Atribut, reprezentujuci informacny ToolTip, ktory poskytuje informacie coho sa tyka vstup - typ string.
        /// </summary>
        private string aToolTip;

        /// <summary>
        /// Atribut, reprezentujuci aky charakter bol predosle stlaceny.
        /// </summary>
        private char aPreviousCharacterPressed;

        /// <summary>
        /// Atribut, reprezentujuci StringBuilder pouzity v inpute - typ StringBuilder.
        /// </summary>
        private StringBuilder aInputStringBuilder;

        /// <summary>
        /// Atribut, reprezentujuci predosle stlacenu klavesu - typ Keys.
        /// </summary>
        private Keys aPreviousKeyPressed;

        /// <summary>
        /// Atribut, ktory specifikuje ci je Input povoleny - typ bool
        /// </summary>
        private bool aEnabled;

        /// <summary>
        /// Atribut, ktory reprezentuje pomocny bool pri switchovani farby, bude sa prepinat medzi true a false - typ bool.
        /// </summary>
        private bool aSwitchedColorBool;

        /// <summary>
        /// Atribut, ktory reprezentuje pomocny casovac ku grafike input buttonu, ked je atribut aHover true alebo false - typ float.
        /// </summary>
        private float aHoverTime;

        /// <summary>
        /// Atribut, ktory reprezentuje ci pouzivatel ukazuje na input, resp presiel na neho klavesou - typ bool;
        /// </summary>
        private bool aHover;

        /// <summary>
        /// Atribut, ktory specifikuje farbu inputu - typ Color.
        /// </summary>
        private Color aInputColor;

        /// <summary>
        /// Atribut, ktory specifikuje prednastavenu farbu inputu - typ Color.
        /// </summary>
        private Color aDefaultColor;

        /// <summary>
        /// Atrbut, ktory vyjadruje limit charakterov, ktore sa mozu zadat do inputu - typ int.
        /// </summary>
        private int aCharLimit;

        /// <summary>
        /// Atribut, ktory vyjadruje, ci uzivatel chce zadavat nieco do Inputu - typ bool.
        /// </summary>
        private bool aFocus;

        /// <summary>
        /// Atribut, ktory povoli do inputu pisat len cisla - typ bool
        /// </summary>
        private bool aNumbersOnly;

        /// <summary>
        /// Atribut, ktory reprezentuje hodnotu, ktorou sa vydeli vyska a sirka a vznikne sirka ramu - typ float.
        /// Cim vacsie cislo, tym mensi ram.
        /// </summary>
        private float aBorderSizeRatio;

        public bool InputEnabled
        {
            get => aEnabled;
            set => aEnabled = value;
        }

        public bool Hover
        {
            get => aHover;
            set => aHover = value;
        }

        public bool Focus
        {
            get => aFocus;
            set => aFocus = value;
        }

        public string InputText
        {
            get => aInputStringBuilder.ToString();
            set => SetStringBuilder(value);
        }

        /// <summary>
        /// Konstruktor polozky vstupu v menu.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ LogicGame.</param>
        /// <param name="parSize">Parameter, reprezentujuci velkost - typ LogicGame.</param>
        /// <param name="parBorderSizeRatio">Parameter, reprezentujuci hodnotu, ktorou sa vydeli vyska a sirka a vznikne sirka ramu - typ float. Cim vacsie cislo, tym mensi ram. - typ float.</param>
        /// <param name="parToolTip">Parameter, reprezentujuci ToolTip - typ string.</param>
        /// <param name="parCharLimit">Parameter, reprezentujuci limit charakterov pri zadavani vstupuu - typ int.</param>
        /// <param name="parNumbersOnly">Parameter, reprezentujuci ci mozu byt do vstupu zadane len cisla (alebo bodky), napr. v pripade zadavania IP adresy - typ bool.</param>
        public MenuInput(LogicGame parGame, Vector2 parPosition, Vector2 parSize, float parBorderSizeRatio, string parToolTip, int parCharLimit, bool parNumbersOnly = false) : base(parGame)
        {
            aGame = parGame;
            aPosition = parPosition;
            aSize = parSize;
            aBorderSizeRatio = parBorderSizeRatio;
            aToolTip = parToolTip;
            aFocus = false;
            aHover = false;
            aCharLimit = parCharLimit;
            aInputStringBuilder = new StringBuilder(parCharLimit);
            aInputColor = Color.White;
            aDefaultColor = aInputColor;
            aHoverTime = 0F;
            aSwitchedColorBool = false;
            aEnabled = false;
            aNumbersOnly = parNumbersOnly;
        }

        /// <summary>
        /// Metoda, ktora sa stara o incializaciu textury a rectanglu.
        /// </summary>
        public override void Initialize()
        {
            aTexture = new Texture2D(aGame.GraphicsDevice, (int)aSize.X, (int)aSize.Y);
            aRectangle = new Rectangle(0, 0, (int)aSize.X, (int)aSize.Y);
            base.Initialize();
        }

        /// <summary>
        /// Metoda, ktora sa stara o textury, kde celej texture priradi bielu farbu.
        /// </summary>
        protected override void LoadContent()
        {
            Color[] tmpColor = new Color[aTexture.Width * aTexture.Height]; //Namapujeme oblast, ktora bude reprezentovat farbu
            for (int i = 0; i < tmpColor.Length; i++)
            {
                if (i % aTexture.Width <= (aTexture.Height / aBorderSizeRatio) || i % aTexture.Width >= aTexture.Width - 1F - (aTexture.Height / aBorderSizeRatio) ||
                    i / (float)aTexture.Height <= (aTexture.Width / aBorderSizeRatio) || i / (float)aTexture.Height >= aTexture.Width - 1F - (aTexture.Width / aBorderSizeRatio))
                {
                    tmpColor[i] = Color.White; //Nastavime Farbu na Ciernu
                }



            }
            aTexture.SetData<Color>(tmpColor); //Samozrejme nastavime Data o Farbe pre Dummy Texturu
            base.LoadContent();
        }


        /// <summary>
        /// Pomocna metoda pre Property InputText, ktora nahradi string v StringBuildery zadanym.
        /// </summary>
        /// <param name="parValue">Parameter, reprezentujuci novy string - typ string.</param>
        public void SetStringBuilder(string parValue)
        {
            if (aInputStringBuilder != null)
            {
                aInputStringBuilder.Clear();
                aInputStringBuilder.Append(parValue);
            }
        }


        /// <summary>
        /// Metoda, ktora sa stara o vykreslenie objektu.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Draw(GameTime parGameTime)
        {
            if (aRectangle != null && aTexture != null && aEnabled)
            {
                string tmpInputText = aInputStringBuilder.ToString();

                float tmpFontScale = 0.8F;

                while (aGame.Font48.MeasureString(tmpInputText).X * tmpFontScale >= aSize.X) //Ak by velkost prekrocila velkost boxu, postupne zmensime velkost pisma
                {
                    tmpFontScale -= 0.1F;
                }

                while (Math.Abs(tmpFontScale - 0.8F) > 0.001F && aGame.Font48.MeasureString(tmpInputText).X * tmpFontScale < aSize.X) //Ak by bola velkost zbytocne mala, tak sa ju postupne budeme pokusat davat naspat
                {
                    tmpFontScale += 0.1F;
                }


                Vector2 tmpToolTipPositionOffset = new Vector2(0, 0);
                tmpToolTipPositionOffset.X = aSize.X / 2F - aGame.Font48.MeasureString(aToolTip).X / 2;
                tmpToolTipPositionOffset.Y = aGame.Font48.MeasureString(aToolTip).Y * -1F;

                Vector2 tmpWrittenTextOffset = new Vector2(0, 0);
                tmpWrittenTextOffset.X = aSize.X / 2F - aGame.Font48.MeasureString(tmpInputText).X * tmpFontScale / 2F;
                tmpWrittenTextOffset.Y = aSize.Y / 2F - aGame.Font48.MeasureString(tmpInputText).Y * tmpFontScale / 2F;


                aGame.SpriteBatch.DrawString(aGame.Font48, aToolTip, aPosition + tmpToolTipPositionOffset, aInputColor, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                aGame.SpriteBatch.Draw(aTexture, aPosition, aRectangle, aInputColor, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                aGame.SpriteBatch.DrawString(aGame.Font48, tmpInputText, aPosition + tmpWrittenTextOffset, Color.Gray, 0F, Vector2.Zero, tmpFontScale, SpriteEffects.None, 0F);
            }

            base.Draw(parGameTime);
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizaciu objektu.
        /// </summary>
        /// <param name="parGameTime">Paramter, reprezentujuci GameTime.</param>
        public override void Update(GameTime parGameTime)
        {
            if (aEnabled)
            {
                if (aFocus)
                {
                    aHoverTime += parGameTime.ElapsedGameTime.Milliseconds;

                    if (aHoverTime > 240F)
                    {
                        SwitchColor(Color.LightGray);
                        aHoverTime = 0F;
                    }
                }
                if (aHover && aFocus != true)
                {
                    aInputColor = Color.Black;
                    aDefaultColor = aInputColor;
                    aGame.Window.TextInput += TextInputHandle;
                }
                else if (aHover != true && aFocus != true)
                {
                    aInputColor = Color.White;
                    aDefaultColor = aInputColor; //Defaultnu farbu prenastavime, pretoze sa zmenila a pri Focuse by to mohlo sposobit problem, podobne vyssie.
                    aGame.Window.TextInput -= TextInputHandle;
                }

                if (Keyboard.GetState().IsKeyDown(aPreviousKeyPressed) == false
                ) //Preto je v update metode, aby sa toto mohlo periodicky kontrolovat, inak by podmienka bola v TextInputHandle
                {
                    aPreviousCharacterPressed = (char)Keys.None;
                }
            }

            base.Update(parGameTime);
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

            if (aFocus && tmpCharacter != aPreviousCharacterPressed) // Ak je Input Otvoreny a Predosly a Aktualny Character Nie je Rovnaky - Prejdeme takto duplikacii vstupu
            {

                if (tmpKeyPressed == Keys.Back)
                {
                    RemoveCharacterFromMessage();
                    Debug.WriteLine(aInputStringBuilder.ToString());
                }
                if (aGame.Font48.Characters.Contains(tmpCharacter) && tmpCharacter != (char)aGame.ProceedKey) //Overime si ci Font (Font48) Hry obsahuje charakter, ktory sa snazime napisat
                {
                    if (aNumbersOnly)
                    {
                        
                        if (char.IsNumber(tmpCharacter) || tmpCharacter == '.') //Ak su povolene len cisla, skontrolujeme ci charakter reprezentuje cislo
                        {
                            AddCharacterToMessage(tmpCharacter);
                            Debug.WriteLine(aInputStringBuilder.ToString());
                        }
                    }
                    else
                    {
                        AddCharacterToMessage(tmpCharacter);
                        Debug.WriteLine(aInputStringBuilder.ToString());
                    }



                }
                if (tmpKeyPressed == Keys.Enter) //Ak je momentalne stlacene tlacitko rovne - ENTERU - Spravu sa snazi pouzivatel odoslat
                {
                    Debug.WriteLine("Chat Window Closed"); //Debug - Sprava do konzole, ze Chat je zatvoreny
                    aFocus = false; //Nastavime premennu, ktora symbolizuje ci je Input Otvoreny na - FALSE
                }
            }

            aPreviousCharacterPressed = tmpCharacter; //Ulozime si tlacitko, ktore bolo stlacene
            aPreviousKeyPressed = tmpKeyPressed; //Ulozime si charakter, ktory bol stlaceny
        }

        /// <summary>
        /// Metoda, ktora prida charakter do vstupu.
        /// </summary>
        /// <param name="parChar">Parameter, reprezentujuci charakter, ktory sa ma pridat - typ char.</param>
        public void AddCharacterToMessage(char parChar)
        {
            if (aInputStringBuilder.Length < aCharLimit)
            {
                aInputStringBuilder.Append(parChar);

            }
        }

        /// <summary>
        /// Metoda, ktora vymaze jeden charakter zo vstupu.
        /// </summary>
        public void RemoveCharacterFromMessage()
        {
            if (aInputStringBuilder.Length > 0)
            {
                aInputStringBuilder.Remove(aInputStringBuilder.Length - 1, 1);
            }
        }

        /// <summary>
        /// Metoda, ktora prepina farby inputu medzi 2-ma specifikovanymi farbami.
        /// </summary>
        /// <param name="parAlterColor">Parameter, reprezentujuci alternativnu farbu - typ Color.</param>
        public void SwitchColor(Color parAlterColor)
        {
            if (aSwitchedColorBool)
            {
                aInputColor = aDefaultColor;
            }
            else
            {
                aInputColor = parAlterColor;
            }

            aSwitchedColorBool = !aSwitchedColorBool;
        }
    }


}
