﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ClientSide.Chat
{
    /// <summary>
    /// Trieda reprezentujuca cast Chatu - vystup.
    /// </summary>
    public class ChatReceiveBox : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// Atribut Hry - typ LogicGame
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut Okna - Typ GameWindow
        /// </summary>
        private GameWindow aGameWindow;

        /// <summary>
        /// Atribut Reprezentujici Texturu - Typ Texture2D
        /// </summary>
        private Texture2D aChatInputDummyTexture;

        /// <summary>
        /// Atribut Pozicie - Typu Vector
        /// </summary>
        private Vector2 aPositionVector;

        /// <summary>
        /// Atribut Pozicie - Prednastaveny Vektor Pozicie - Typ Vector2
        /// </summary>
        private Vector2 aPositionVectorDefault;

        /// <summary>
        /// Atribut Chat Output Rectanglu - Typ Rectangle
        /// </summary>
        private Rectangle aChatOutputRectagle;

        /// <summary>
        /// Atribut reprezentujuci Vysku Receive Boxu - Typ Int
        /// </summary>
        private int aWindowHeight;

        /// <summary>
        /// Atribut reprezentujuci Sirku Receive Boxu - Typ Int
        /// </summary>
        private int aWindowWidth;

        /// <summary>
        /// Atribut, kde sa uklada predosla velkost ukladacieho priestoru pre spravy - Typ Int
        /// </summary>
        private int aOldStorageSize = 0;

        /// <summary>
        /// Atribut, ktory reprezentuje pocitadlo ubehnuteho casu - Typ Double
        /// </summary>
        private double aTimeCounter = 0.0;

        /// <summary>
        /// Atribut, reprezentujuci Zoznam obsahujuci ukladaci priestor pre spravy - typ List
        /// </summary>
        private List<string> aMessageStorageList;

        /// <summary>
        /// Atribut, reprezentujuci skalovanie velkosti sprav oproti prednastavenej velkosti - Typ Float
        /// </summary>
        private float aMessagesScale = 0.35F;

        /// <summary>
        /// Atribut, reprezentujuci vzdialenost (Y-ovu), medzi riadkami sprav, pocitajuc aj so skalovanim - Typ Float
        /// </summary>
        private float aSpacing;

        /// <summary>
        /// Atribut, ktory reprezentuje, odkial sa ma sprava vykreslovat, defaultne od 0, postupne ako dochadza miesto v Receive Boxe, hodnota sa zvysuje - Typ Int
        /// </summary>
        private int aStartOfMessageDrawing = 0;

        /// <summary>
        /// Atribut, ktory reprezentuje, kolko riadkov sprav sa ma vykreslit do Receive Boxu - Typ Int
        /// </summary>
        private int aCountOfDrawableRows;

        /// <summary>
        /// Atribut, ktory reprezentuje - na kolko sekund sa ukaze ReceiveBox po prijati spravy - Typ Int
        /// </summary>
        private int aReceiveBoxShownFor = 5;
      
        public double TimeCounter
        {
            get => aTimeCounter;
            set => aTimeCounter = value;
        }

        /// <summary>
        /// Konstruktor Chat Receive Boxu
        /// </summary>
        /// <param name="parGame">Parameter Hry - Typ LogicGame</param>
        /// <param name="parGameWindow">Parameter Okna Hry - Typ GameWindow</param>
        /// <param name="parWidth">Parameter Sirky Boxu - Typ Int</param>
        /// <param name="parHeight">Parameter Vysky Boxu - Typ Int</param>
        /// <param name="parPosVector">Parameter Pozicie Boxu - Typ Vector2 </param>
        public ChatReceiveBox(LogicGame parGame, GameWindow parGameWindow, int parWidth, int parHeight, Vector2 parPosVector) : base(parGame)
        {
            aLogicGame = parGame;
            aGameWindow = parGameWindow;
            aPositionVector = parPosVector;
            aWindowHeight = parHeight;
            aWindowWidth = parWidth;
            aMessageStorageList = new List<string>(10);
            aPositionVectorDefault = new Vector2(aPositionVector.X, aPositionVector.Y);
        }

        /// <summary>
        /// Overrida metoda, ktora sa stara o vykreslovanie logiky Receive Boxu.
        /// Zaroven sa stara aj o rozdelovanie dlheho textu do samostatnych riadkov.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Draw(GameTime parGameTime)
        {
            if (aMessageStorageList.Count > aOldStorageSize || aTimeCounter > 0) // Ak pribudla nejaka nova sprava, alebo zacalo odpocitavanie casu, pre zobrazenie Receive Boxu
            {
                aTimeCounter += parGameTime.ElapsedGameTime.TotalSeconds; //Pripocitame kolko "sekund" ubehlo za vykreslenie jedneho framu k pocitadlu casu

                aLogicGame.SpriteBatch.Draw(aChatInputDummyTexture, aPositionVector, aChatOutputRectagle, Color.Black * 0.3F, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0.15F ); //Vykresli ChatInputBox pomocou Textury, Rectangle a farby - Color.White zachovava povodne farby

                if (aMessageStorageList.Count > aOldStorageSize) //Zabezpecime, ze po prijati novej spravy sa zresetuje pocitadlo na aku dlhu dobu ma byt zobraeny Receive Box
                {
                    aTimeCounter = parGameTime.ElapsedGameTime.TotalSeconds; //Reset na prvy TICK

                    aOldStorageSize = aMessageStorageList.Count; //Ked pribudne nova sprava, treba samozrejme aktualizovat aj startu velkost ukladacieho priestoru
                }
                for (int countRow = aStartOfMessageDrawing; countRow < aMessageStorageList.Count; countRow++) //For cyklus, pocitajuci, kolko riadkov sprav sa ma zobrazit - meni sa dynamicky, zalezi od - Atributu aStartOfMessageDrawing a od poctu sprav v uloznom priestore
                {
                    Vector2 tmpOffSetVector = CalculateOffSetVector(aPositionVector, aLogicGame.Font72, aMessagesScale, countRow - aStartOfMessageDrawing); //Vypocet OffSet Vectoru - pre vykreslenie na novy riadok



                    int tmpIndexOfLastCharacter = aMessageStorageList[countRow].Length - 1;
                    string tmpStringWithoutColorCode = aMessageStorageList[countRow].Remove(aMessageStorageList[countRow].Length - 1);
                    string tmpColorCode = aMessageStorageList[countRow].Substring(tmpIndexOfLastCharacter, 1);


                    ChatColors tmpMessageColor = Enum.Parse<ChatColors>(tmpColorCode); //Posledny Charakter je Ulozeny ako farba

                     //Cisty String bez informacie o farbe



                   aLogicGame.SpriteBatch.DrawString(aLogicGame.Font72, tmpStringWithoutColorCode, tmpOffSetVector, ConvertEnumColor(tmpMessageColor), 0F, Vector2.Zero, aMessagesScale, SpriteEffects.None, 0.1F); //Samotne vykreslovanie riadkov sprav
                }
                //    aLogicGame.SpriteBatch.End();

                if (aTimeCounter > aReceiveBoxShownFor) //Ak vyprsal cas pre vykreslenie Receive Boxu
                {
                    aTimeCounter = 0; //Vynulujeme pocitadlo
                }
            }
            base.Draw(parGameTime);
        }

        /// <summary>
        /// Metoda, ktora sluzi na rozdelovanie velkeho Stringu na riadky - Pokial sa nezmesti do Boxu
        /// </summary>
        /// <param name="parFont">Parameter typu Font72 - typ SpriteFont</param>
        /// <param name="parFontScale">Parameter Skala Fontu - Typ float</param>
        /// <param name="parReceiveBoxWidth">Parameter Sirka Boxu - Typ int</param>
        /// <param name="parMessage">Parameter Reprezentujuci Spravu - Typ String</param>
        public void ParseMessageIntoRows(SpriteFont parFont, float parFontScale, int parReceiveBoxWidth, string parMessage, int parMessageColor)
        {
            Vector2 tmpStringSizeVector = aLogicGame.Font72.MeasureString(parMessage) * parFontScale; //Vektor, ktory reprezentuje velkost celeho stringu pocitajuc uz so skalou
            if (tmpStringSizeVector.X <= parReceiveBoxWidth) //Ak je sirka stringu mensia rovna ako sirka boxu
            {
                aMessageStorageList.Add(parMessage + parMessageColor); //Proste pridame spravu do Uloziska
            }
            else
            {
                StringBuilder tmpNewMessage = new StringBuilder(); //Vytvorime si StringBuilder, vyhoda je v tom, ze sa za kazdym nemusi vytvarat novy string

                for (int i = 0; i < parMessage.Length; i++) //Cez For Cyklus, prechadzame celu dlzku spravy
                {
                    tmpNewMessage.Append(parMessage[i]); //Do naseho StringBuildera postupne pridavame charaktery z povodneho stringu

                    float tmpNewMessageWidth = aLogicGame.Font72.MeasureString(tmpNewMessage.ToString()).X * parFontScale; //Pomocna premenna - Sirka novej spravy pocitajuc, ako vzdy, so skalou.

                    if (tmpNewMessageWidth + aLogicGame.Font72.LineSpacing * parFontScale > parReceiveBoxWidth)
                    { //Ak je Sirka novej spravy + "Spacing Pismen, vynasobeny skalou" vacsi ako sirka boxu

                        aMessageStorageList.Add(tmpNewMessage.ToString() + parMessageColor); //Pridame takto nasu, vynatu spravu do uloziska
                        tmpNewMessage.Clear(); //A Stringuilder si vynulujeme, aby sme mohli podobne dokoncit aj zbytok stringu

                    }

                }
                aMessageStorageList.Add(tmpNewMessage.ToString() + parMessageColor); //Pokial by sa stalo, ze uz rozdelujeme druhu cast stringu, ale tento uz nie je vacsi ako sirka boxu, mozeme ho v klude ulozit do uloziska
            }
        }

        /// <summary>
        /// Vypocet OffSet Vectoru - Pouziteho pri vypocte vykreslovania sprav na riadky
        /// </summary>
        /// <param name="parPositionVector">Parameter Vectoru Reprezentujuceho Poziciu - Typ Vector2</param>
        /// <param name="parFont">Parameter Reprezentujuci Pismo - Typ Font72</param>
        /// <param name="parMessageScale">Parameter Reprezentujuci Skalovanie Sprav - Typ Float</param>
        /// <param name="parRowNumber">Parameter Reprezentujuci Cislo Riadku - Typ Int</param>
        /// <returns>Vrati vypocitanu poziciu - Vector - Typ Vector2</returns>
        public Vector2 CalculateOffSetVector(Vector2 parPositionVector, SpriteFont parFont, float parMessageScale, int parRowNumber)
        {
            if (aSpacing == 0) //Ak este nie je nastaveny Spacing 
            {
                SetSpacing(parFont, parMessageScale); //Nastavime Spacing
            }
            parPositionVector.Y += (parRowNumber * aSpacing); //Y-ovu suradnicu Vectoru pre Poziciu vypocitame ako cislo riadku vynasobene spacingom
            return parPositionVector; //Vratime novo vypocitany Vector Pozicie 
        }

        /// <summary>
        /// Setter - Nastavi Spacing, na zaklade typu Pisma a Skalovania Sprav
        /// </summary>
        /// <param name="parFont">Parameter Reprezentujuci Pismo - Typ Font72</param>
        /// <param name="parMessageScale">Parameter Reprezentujuci Skalovanie Pisma - Typ Float</param>
        public void SetSpacing(SpriteFont parFont, float parMessageScale)
        {
            if (parFont != null) //Overime ci Font72 existuje
            {
                aSpacing = parFont.LineSpacing * parMessageScale; //Spacing nastavime ako font line spacing vynasobeny skalou
            }
        }

        /// <summary>
        /// V hre ja pusunuta kamera, preto na toto treba dbat aj pri Chate
        /// </summary>
        public void AddChatOffset(float parChatOffsetX)
        {
            aPositionVector.X -= parChatOffsetX;
        }

        
        public override void Update(GameTime gameTime)
        {

            if (aPositionVector.X != aPositionVectorDefault.X - aLogicGame.CameraX) //Chat musi pocitat aj s Offsetom Kamery, v nasom pripade nam staci pracovat s Offset suradnicou X, ine sa nebudu menit
            {
                aPositionVector.X = aPositionVectorDefault.X - aLogicGame.CameraX;
            }

            if ((aMessageStorageList.Count + 1) * aSpacing > aWindowHeight) //Pozrieme sa ci sa spravy zmestia do Receive Boxu - Velkostne - Ak nie vstupime do vnutra podmienky
            {
                if (aCountOfDrawableRows == 0) //Ak este nie je vypocitany pocet riadkov, ktore sa mozu vykreslit
                {
                    aCountOfDrawableRows = (int)(aWindowHeight / aSpacing); //Vypocitame ako Vyska okna / spacing
                }
                aStartOfMessageDrawing = aMessageStorageList.Count - aCountOfDrawableRows; //Zaroven vypocitame index, od ktoreho ma byt zapocate vykreslovanie
            }

            base.Update(gameTime);
        }
        
        /// <summary>
        /// Override metoda, ktora sa stara o nacitavanie textur a rectanglu.
        /// </summary>
        protected override void LoadContent()
        {
            aChatInputDummyTexture = new Texture2D(aLogicGame.GraphicsDevice, aWindowWidth, aWindowHeight); //Dummy Texture - Pretoze texturu nenacitavam ale "dopocitam ju"
            aChatOutputRectagle = new Rectangle((int)aPositionVector.X, (int)aPositionVector.Y, aWindowWidth, aWindowHeight); //Nastavime Rectangle reprezentujuci oblast ReceiveBoxu

            Color[] tmpColor = new Color[aChatInputDummyTexture.Width * aChatInputDummyTexture.Height]; //Namapujeme oblast, ktora bude reprezentovat farbu
            { 
                for (int i = 0; i < tmpColor.Length; i++) //Pre celu oblast
                {
                    tmpColor[i] = Color.Black; //Nastavime Farbu na Ciernu
                }
                aChatInputDummyTexture.SetData<Color>(tmpColor); //Samozrejme nastavime Data o Farbe pre Dummy Texturu

            }

            base.LoadContent();
        }

        /// <summary>
        /// Metoda - Ukladajuca Spravy - Zabezpecuje aj Rozdelenie velkych sprav do mensich stringov, ktore sa adekvatne zmestia do riadkov Receive Boxu
        /// </summary>
        /// <param name="parMessage"></param>
        public void StoreMessage(string parMessage, int parMessageColor) 
        {
            ParseMessageIntoRows(aLogicGame.Font72, aMessagesScale, aWindowWidth, parMessage, parMessageColor); //Parsovanie celej spravy do riadkov
        }

        /// <summary>
        /// Metoda, ktora sa stara o vycistenie Listu, ktory obsahuje spravy
        /// </summary>
        public void ClearStorage()
        {
            if (aMessageStorageList != null)
            {
                aMessageStorageList.Clear();
            }
        }

        /// <summary>
        /// Metoda, ktora pretransformuje ChatColor - farbu na farbu - Color, ktoru podporuje kniznica MonoGame.
        /// </summary>
        /// <param name="parColor">Parameter, reprezentujuci farbu - typ ChatColors - enum</param>
        /// <returns>Vrati farbu - Color. - Z kniznice MonoGame.</returns>
        public Color ConvertEnumColor(ChatColors parColor) {

            switch (parColor)
            {
                case ChatColors.White:
                    return Color.White;
                case ChatColors.Red:
                    return Color.Red;
                case ChatColors.Green:
                    return Color.Green;
                case ChatColors.Purple:
                    return Color.Purple;
                default:
                    return Color.White;
            }
        }
    }
}
