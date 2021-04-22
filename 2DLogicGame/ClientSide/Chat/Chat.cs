using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ClientSide.Chat
{

    /// <summary>
    /// Enumeracna trieda, pomocou ktorej sa odlisuju farby sprav v chate.
    /// </summary>
    public enum ChatColors { 
    White = 0,
    Red = 1,
    Green = 2,
    Purple = 3
    }

    /// <summary>
    /// Trieda, ktora spravuje Chat.
    /// </summary>
    public class Chat : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// Atribut Hry - typ LogicGame.
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Reprezentuje Chat Input Box - Typ ChatInputBox.
        /// </summary>
        private ChatInputBox aChatInputBox;

        /// <summary>
        /// Reprezentuje Chat Receive Box - Typ ChatReceiveBox.
        /// </summary>
        private ChatReceiveBox aChatReceiveBox;

        /// <summary>
        /// Queue, ktora reprezentuje Spravy, ktore cakaju na odoslanie - Kazdy tick sa odosle jedna.
        /// </summary>
        private Queue<string> aMessagesToBeSent;

        /// <summary>
        /// Reprezentuje ci Chat obsahuje nejake spravy, ktore by mali byt odoslane - typ bool.
        /// </summary>
        private bool isMessageWaitingToBeSent;

        /// <summary>
        /// Atribut, ktory reprezentuje hodnotu, ci je sprava pripravena na odoslanie - typ bool.
        /// </summary>
        public bool IsMessageWaitingToBeSent { get => isMessageWaitingToBeSent;  }

        public ChatInputBox ChatInputBox
        {
            get => aChatInputBox;
            set => aChatInputBox = value;
        }

        public ChatReceiveBox ChatReceiveBox
        {
            get => aChatReceiveBox;
            set => aChatReceiveBox = value;
        }

        /// <summary>
        /// Konstruktor Chatu.
        /// </summary>
        /// <param name="parGame">Parameter reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parChatInputBox">Parameter, reprezenujuci okno chatu pre pisanie sprav - typ ChatInputBox.</param>
        /// <param name="parChatReceiveBox">Parameter, reprezentujuci okno chatu pre prijmanie sprav - typ ChatReceiveBox.</param>
        public Chat(LogicGame parGame, ChatInputBox parChatInputBox, ChatReceiveBox parChatReceiveBox) : base(parGame)
        {
            aMessagesToBeSent = new Queue<string>(2);
            aLogicGame = parGame;
            aChatInputBox = parChatInputBox;
            aChatReceiveBox = parChatReceiveBox;
        }

        /// <summary>
        /// Override metoda, ktora sa stara o aktualizaciu hernych dat.
        /// Ak je pripravena sprava na odoslanie z Inputu, tuto spravu metoda ulozi.
        /// A tiez sa stara o vypocet doby, pocas ktorej ma byt Receive okno otvorene.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (aLogicGame != null && aChatInputBox != null)
            {
                if (aChatInputBox.IsMessageReadyToBeStored == true)
                {
                    StoreMessageToSend(aChatInputBox.MessageToSend);
                }
            }

            if (aChatInputBox != null && aChatInputBox.IsInputOpen && aChatReceiveBox != null)
            {
                aChatReceiveBox.TimeCounter += gameTime.ElapsedGameTime.Milliseconds;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Metoda, ktora sa stara o ukladanie vsetkych sprav.
        /// </summary>
        /// <param name="parSenderName">Parameter, reprezentujuci meno odosielatela.</param>
        /// <param name="parMessage">Parameter, reprezentujuci spravu.</param>
        /// <param name="parMessageColor">Parameter, reprezentujuci farbu spravy.</param>
        public void StoreAllMessages(string parSenderName, string parMessage, ChatColors parMessageColor = 0)
        {
            aChatReceiveBox.StoreMessage(parSenderName + ": " + parMessage , (int)parMessageColor);
        }

        /// <summary>
        /// Metoda, sluziaca na Uskladnenie spravy pred odoslanim
        /// </summary>
        /// <param name="parMessage">Parameter sprava - typ string</param>
        public void StoreMessageToSend(string parMessage)
        {
            aMessagesToBeSent.Enqueue(parMessage); //Ulozime spravu do Queue - Front
            isMessageWaitingToBeSent = true; //Nastavime pomocnu premennu - sprava je pripravena na odoslanie - resp. Front nie je prazdny 

            aChatInputBox.DeleteMessage(); //Na konci zmazeme uz odoslanu spravu
        }

        /// <summary>
        /// Metoda, ktora sluzi na odovzdanie spravy
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Metoda, ktora sa stara o vycistenie interneho ukladacieho priestoru prijatych sprav
        /// </summary>
        public void ResetStorage()
        {
            if (aChatReceiveBox != null)
            {
                aChatReceiveBox.ClearStorage();
            }
        }



    }
}
