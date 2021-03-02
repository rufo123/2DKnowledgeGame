using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ClientSide.Chat
{



    class Chat : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// Atribut Hry
        /// </summary>
        private LogicGame aLogicGame;


        /// <summary>
        /// Reprezentuje Chat Input Box - Typ ChatInputBox
        /// </summary>
        private ChatInputBox aChatInputBox;

        /// <summary>
        /// Queue, ktora reprezentuje Spravy, ktore cakaju na odoslanie - Kazdy tick sa odosle jedna
        /// </summary>
        private Queue<string> aMessagesToBeSent;

        /// <summary>
        /// Reprezentuje ci Chat obsahuje nejake spravy, ktore by mali byt odoslane
        /// </summary>
        private bool isMessageWaitingToBeSent;

        public bool IsMessageWaitingToBeSent { get => isMessageWaitingToBeSent;  }

        public Chat(LogicGame parGame, ChatInputBox parChatInputBox) : base(parGame)
        {
            aMessagesToBeSent = new Queue<string>(2);

            aLogicGame = parGame;
            aChatInputBox = parChatInputBox;
        }

        public override void Draw(GameTime gameTime)
        {



            base.Draw(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            if (aLogicGame != null && aChatInputBox != null)
            {

                if (aChatInputBox.IsMessageReadyToBeStored == true)
                {
                    StoreMessageToSend(aChatInputBox.MessageToSend);

                }
            }

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public void StoreAllMessages()
        {

        }

        public void StoreMessageToSend(string parMessage)
        {
            aMessagesToBeSent.Enqueue(parMessage);
            isMessageWaitingToBeSent = true;

            aChatInputBox.DeleteMessage(); //Na konci zmazeme uz odoslanu spravu
        }

        public string ReadAndTakeMessage()
        {
            if (aMessagesToBeSent.Count == 1) //Ak je tam len jedna sprava, zoberieme ju a zaroven nastavime ze uz ziadne nacakaju
            {
                isMessageWaitingToBeSent = false;
                return aMessagesToBeSent.Dequeue();
            }
            else if (aMessagesToBeSent.Count > 0) //Ak je tam Viac sprav ako jedna vratime tu spravu a odoberieme
            {
                return aMessagesToBeSent.Dequeue();
            }
            else //Toto by nemalo nastat ale ak tam nie je nic, proste vratime null
            {
                return null;
            }

        }



    }
}
