using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ClientSide.Chat
{
    class ChatInputBox : Microsoft.Xna.Framework.DrawableGameComponent
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

        private int aWindowHeight;
        private int aWindowWidth;




        public string MessageToSend { get => aMessageToSend; }
        public bool IsMessageReadyToBeStored { get => isMessageReadyToBeStored; set => isMessageReadyToBeStored = value; }

        public ChatInputBox(LogicGame parGame, GameWindow parGameWindow, int parWidth, int parHeight, Vector2 parPosVector) : base(parGame)
        {
            aLogicGame = parGame;
            aGameWindow = parGameWindow;
            aPositionVector = parPosVector;
            aChatInputTexture2D = new Texture2D(aLogicGame.GraphicsDevice, parWidth, parHeight); //Inicializujeme si Texture2D ako Pozadie Inputu
            aChatInputRectangle = new Rectangle((int)parPosVector.X, (int)parPosVector.Y, parWidth, parHeight);
            aWindowHeight = parHeight;
            aWindowWidth = parWidth;

        }

        public override void Draw(GameTime gameTime)
        {
            if (isInputOpen)
            {
                aLogicGame.SpriteBatch.Begin();
                aLogicGame.SpriteBatch.Draw(aChatInputTexture2D, aChatInputRectangle, Color.White);

                Vector2 tmpVectorChat = aPositionVector + new Vector2(36, aWindowHeight/2 - 36);

                aLogicGame.SpriteBatch.DrawString(aLogicGame.Font, aMessageToSend, tmpVectorChat, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
                aLogicGame.SpriteBatch.End();


            }
            base.Draw(gameTime);
        }

        public override void Initialize()
        {



            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (aLogicGame.CheckKeyPressedOnce(aLogicGame.ChatWriteMessageKey) && isInputOpen == false)
            {
                isInputOpen = !isInputOpen; //Prakticky to robi to, ze sa hodnota boolean zmeni na opacnu hodnotu... Usetrime riadky kodu

            }

            if (isInputOpen)
            {
                if (Keyboard.GetState().IsKeyDown(aPreviousKeyPressed) == false)
                {
                    aPreviousCharacterPressed = '0';
                }
                aLogicGame.Window.TextInput += TextInputHandle;


            }
            else
            {

                aLogicGame.Window.TextInput -= TextInputHandle;
            }


            base.Update(gameTime);
        }

        protected override void LoadContent()
        {


            aChatInputTexture2D = aLogicGame.Content.Load<Texture2D>("Sprites\\Backgrounds\\chatInputBackground");

            base.LoadContent();
        }


        public void TextInputHandle(object parSender, TextInputEventArgs parArgs)
        {


            Keys tmpKeyPressed = parArgs.Key;
            char tmpCharacter = parArgs.Character;


            if (isInputOpen && tmpCharacter != aPreviousCharacterPressed)
            {

                if (tmpKeyPressed == Keys.Back) {
                    this.TructMessage();
                    Debug.WriteLine(aMessageToSend);
                }


                if (aLogicGame.Font.Characters.Contains(tmpCharacter))
                {

                    this.AppendMessage(tmpCharacter);
                    Debug.WriteLine(aMessageToSend);

                }

                if (tmpKeyPressed == Keys.Enter) //Ak odosleme spravu
                {
                    Debug.WriteLine("Chat Window Closed");
                    isInputOpen = false;
                    IsMessageReadyToBeStored = true;
                }




            }

            aPreviousCharacterPressed = tmpCharacter;
            aPreviousKeyPressed = tmpKeyPressed;







        }

        public void AppendMessage(char parCharacterToAppend)
        {
            if (aMessageToSend != null)
            {
                if (aMessageToSend.Length < 50)
                {
                    this.aMessageToSend += parCharacterToAppend;
                }

            }
            else
            {

                this.aMessageToSend += parCharacterToAppend;
            }
        }

        public void TructMessage() {

            if (aMessageToSend.Length > 0) {

               this.aMessageToSend = this.aMessageToSend.Remove(aMessageToSend.Length - 1);
            }

        }

        public void DeleteMessage()
        {
            IsMessageReadyToBeStored = false;
            aMessageToSend = "";
        }





    }
}
