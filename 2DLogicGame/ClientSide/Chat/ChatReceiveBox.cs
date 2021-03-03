using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ClientSide.Chat
{
    class ChatReceiveBox : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private LogicGame aLogicGame;
        private GameWindow aGameWindow;
        private Texture2D aChatInputDummyTexture;
        private Vector2 aPositionVector;
        private Rectangle aChatOutputRectagle;
        private int aWindowHeight;
        private int aWindowWidth;

        private int aOldStorageSize = 0;

        private double aTimeCounter = 0.0;

        private List<string> aMessageStorage;

        private float aMessagesScale = 0.35F;

        private float aSpacing;

        private int aStartOfMessageDrawing = 0;

        private int aCountOfDrawableRows;

        /// <summary>
        /// Na kolko sekund sa ukaze ReceiveBox po prijati spravy
        /// </summary>
        private int aReceiveBoxShownFor = 5;

        public ChatReceiveBox(LogicGame parGame, GameWindow parGameWindow, int parWidth, int parHeight, Vector2 parPosVector) : base(parGame)
        {
            aLogicGame = parGame;
            aGameWindow = parGameWindow;
            aPositionVector = parPosVector;
            aWindowHeight = parHeight;
            aWindowWidth = parWidth;
            aMessageStorage = new List<string>(10);


            

        }

        public override void Draw(GameTime gameTime)
        {
            if (aMessageStorage.Count > aOldStorageSize || aTimeCounter > 0)
            {
                aTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                aLogicGame.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                aLogicGame.SpriteBatch.Draw(aChatInputDummyTexture, aChatOutputRectagle, Color.Black * 0.3F); //Vykrasli ChatInputBox pomocou Textury, Rectangle a farby - Color.White zachovava povodne farby

                if (aMessageStorage.Count > aOldStorageSize) //Zmazat, len kvoli tomu aby sa nespamovala konzola
                {
                    aTimeCounter = gameTime.ElapsedGameTime.TotalSeconds; //Reset na prvy TICK

                }

                for (int countRow = aStartOfMessageDrawing; countRow < aMessageStorage.Count; countRow++)
                {
                    int tmpRowDividedByCountOfDrawable = countRow;

                    if (aCountOfDrawableRows != 0)
                    {
                        tmpRowDividedByCountOfDrawable = countRow % aCountOfDrawableRows;
                    }
 
                    

                    Vector2 tmpOffSetVector = CalculateOffSetVector(aPositionVector, aLogicGame.Font, aMessagesScale, countRow - aStartOfMessageDrawing);
                    aLogicGame.SpriteBatch.DrawString(aLogicGame.Font, aMessageStorage[countRow], tmpOffSetVector, Color.White, 0F, Vector2.Zero, aMessagesScale, SpriteEffects.None, 0);

                    // tmpMessageVector = tmpOffSetVector;
                }

                aLogicGame.SpriteBatch.End();

                aOldStorageSize = aMessageStorage.Count;

                if (aTimeCounter > 5)
                {
                    aTimeCounter = 0;
                }
            }

            base.Draw(gameTime);
        }
        public override void Initialize()
        {
            base.Initialize();

        }

        /// <summary>
        /// Metoda, ktora sluzi na rozdelovanie velkeho Stringu na riadky - Pokial sa nezmesti do Boxu
        /// </summary>
        /// <param name="parFont">Parameter typu Font - typ SpriteFont</param>
        /// <param name="parFontScale">Parameter Skala Fontu - Typ float</param>
        /// <param name="parReceiveBoxWidth">Parameter Sirka Boxu - Typ int</param>
        /// <param name="parMessage">Parameter Reprezentujuci Spravu - Typ String</param>
        public void ParseMessageIntoRows(SpriteFont parFont, float parFontScale, int parReceiveBoxWidth, string parMessage)
        {

            Vector2 tmpStringSizeVector = aLogicGame.Font.MeasureString(parMessage) * parFontScale; //Vektor, ktory reprezentuje velkost celeho stringu pocitajuc uz so skalou

            if (tmpStringSizeVector.X <= parReceiveBoxWidth) //Ak je sirka stringu mensia rovna ako sirka boxu
            {
                aMessageStorage.Add(parMessage); //Proste pridame spravu do Uloziska
            }
            else
            {

                StringBuilder tmpNewMessage = new StringBuilder(); //Vytvorime si StringBuilder, vyhoda je v tom, ze sa za kazdym nemusi vytvarat novy string

                for (int i = 0; i < parMessage.Length; i++) //Cez For Cyklus, prechadzame celu dlzku spravy
                {
                    tmpNewMessage.Append(parMessage[i]); //Do naseho StringBuildera postupne pridavame charaktery z povodneho stringu

                    float tmpNewMessageWidth = aLogicGame.Font.MeasureString(tmpNewMessage.ToString()).X * parFontScale; //Pomocna premenna - Sirka novej spravy pocitajuc, ako vzdy, so skalou.

                    if (tmpNewMessageWidth + aLogicGame.Font.LineSpacing * parFontScale > parReceiveBoxWidth)
                    { //Ak je Sirka novej spravy + "Spacing Pismen, vynasobeny skalou" vacsi ako sirka boxu

                        aMessageStorage.Add(tmpNewMessage.ToString()); //Pridame takto nasu, vynatu spravu do uloziska
                        tmpNewMessage.Clear(); //A Stringuilder si vynulujeme, aby sme mohli podobne dokoncit aj zbytok stringu

                    }

                }

                aMessageStorage.Add(tmpNewMessage.ToString()); //Pokial by sa stalo, ze uz rozdelujeme druhu cast stringu, ale tento uz nie je vacsi ako sirka boxu, mozeme ho v klude ulozit do uloziska


            }


        }

        public Vector2 CalculateOffSetVector(Vector2 parPositionVector, SpriteFont parFont, float parMessageScale, int parRowNumber)
        {

            if (aSpacing == 0) {
                SetSpacing(parFont, parMessageScale);
            }

            parPositionVector.Y += (parRowNumber * aSpacing) ;
     

            return parPositionVector;
        }

        public void SetSpacing(SpriteFont parFont, float parMessageScale)
        {
            if (parFont != null && parMessageScale != null) { 
            aSpacing = parFont.LineSpacing * parMessageScale;
            }
        }

        public override void Update(GameTime gameTime)
        {

            if ((aMessageStorage.Count + 1) * aSpacing > aWindowHeight) {
                if (aCountOfDrawableRows == 0 ) {
                    aCountOfDrawableRows = (int)(aWindowHeight / aSpacing);
                }
                aStartOfMessageDrawing = aMessageStorage.Count - aCountOfDrawableRows;
            }

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            aChatInputDummyTexture = new Texture2D(aLogicGame.GraphicsDevice, aWindowWidth, aWindowHeight);
            aChatOutputRectagle = new Rectangle((int)aPositionVector.X, (int)aPositionVector.Y, aWindowWidth, aWindowHeight);

            Color[] tmpColor = new Color[aChatInputDummyTexture.Width * aChatInputDummyTexture.Height];
            for (int i = 0; i < tmpColor.Length; i++)
            {
                tmpColor[i] = Color.Black;
            }

            aChatInputDummyTexture.SetData<Color>(tmpColor);

            


            base.LoadContent();

            
        }

        public void StoreMessage(string parMessage)
        {

            ParseMessageIntoRows(aLogicGame.Font, aMessagesScale, aWindowWidth, parMessage);

        }
    }
}
