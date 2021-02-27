using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Threading;
using Lidgren.Network;


namespace _2DLogicGame
{
    public class Server
    {
        /// <summary>
        /// Port sluziaci na pripojenie sa medzi Serverom a Klientom
        /// </summary>
        private int aPort = 28741;

        /// <summary>
        /// Atribut typu NetServer - Reprezentuje Server ako taky
        ///</summary>
        private NetServer aServer;

        /// <summary>
        /// Atribut reprezentuje, ci je Server spusteny alebo nie
        /// </summary>
        private bool aStarted = false;

        private LogicGame aLogicGame;


        public bool Started { get => aStarted; set => aStarted = value; }

        public Server(string parAppName, LogicGame parGame)
        {

            aLogicGame = parGame;

            NetPeerConfiguration tmpServerConfig = new NetPeerConfiguration(parAppName) { Port = aPort }; //Nastavime si Meno nasej Aplikacie, ktora bude sluzi na validaciu pri pripojeni a Port

            tmpServerConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval); //Povolime prijmanie spravy typu - ConnectionApproval (Enum)

            aServer = new NetServer(tmpServerConfig); //Inicializujeme Server s nami zadanou Konfiguraciou


            aServer.Start(); //Zapneme nas Web Server
            //Ked volame Start() - Lidgren nabinduje vhodny Network Socket a vytvori na pozadi Thread, ktory spracuje prave Sietovanie...

            if (aServer.Status == NetPeerStatus.Running)
            { //Ak server bezi, nastavime atribut aStared na TRUE
                aStarted = true;
            }
            else
            {
                aStarted = false;
            }

            Debug.WriteLine("Server Connection Initiated");

           /*  Thread thread;
            thread = new Thread(new ThreadStart(ReadMessages));
            thread.Start();

            



            Debug.WriteLine("Server Connection Initiated"); */


        }

        public void ReadMessages()
        {

            while (aLogicGame.GameState != GameState.Exit)
            {

                //aServer.MessageReceivedEvent.WaitOne();

                NetIncomingMessage tmpIncommingMessage; //Vytvorime lokalnu premennu typu NetIncommingMessage

                tmpIncommingMessage = aServer.ReadMessage(); //Spravu inicializujeme ako aServer.ReadMessage() - Read a pending message from any connection, if any 

                if (tmpIncommingMessage == null)
                {
                    continue;
                }

                switch (tmpIncommingMessage.MessageType)
                {
                    case NetIncomingMessageType.Error:
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        break;
                    case NetIncomingMessageType.UnconnectedData:
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        byte tmpReceivedByte = tmpIncommingMessage.ReadByte(); //Zainicializujeme lokalnu premennu typu byte - reprezentujucu prijaty byte

                        if (tmpReceivedByte == (byte)PacketMessageType.Connect) //Ak sa prijaty byte rovna hodnote Enumu - Connect
                        {

                            Debug.WriteLine("Server - Spojenie bolo uspesne nadviazane!");

                            //Prijmeme spravu od Clienta - Nickname a Potvrdime pripojenie
                            string tmpNickName = tmpIncommingMessage.ReadString(); //Precitame String, ktory reprezentuje NickName
                            tmpIncommingMessage.SenderConnection.Approve(); //Povolime resp "Approvneme" Pripojenie

                            //Teraz musime odoslat Klientovi Acknowledgment o tom, ze pripojenie prebehlo uspesne

                            NetOutgoingMessage tmpOutgoingMessage = aServer.CreateMessage();
                            tmpOutgoingMessage.Write((byte)PacketMessageType.Connect); //Vratime spat spravu, ktoru server odoslal
                            tmpOutgoingMessage.Write(true); //Hodnota boolean o tom, ze pripojenie prebehlo uspesne



                            /* aServer.SendMessage(tmpOutgoingMessage, tmpIncommingMessage.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0); //Sprava, Komu, Typ - UDP - Reliable Ordered, Sequence Channel - Gets the sequence channel this message was sent with 
                             // tmpIncommingMessage.SenderConnection.Approve(tmpOutgoingMessage);
                             Debug.WriteLine("Pripojil sa hrac s menom: " + tmpNickName); */

                        }
                        else
                        {
                            tmpIncommingMessage.SenderConnection.Deny("Pripojenie sa nezdarilo - Neboli odoslane spravne informacie o pripojeni!");
                        }
                        break;
                    case NetIncomingMessageType.Data:


                        break;
                    case NetIncomingMessageType.Receipt:
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        break;
                    case NetIncomingMessageType.NatIntroductionSuccess:
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        break;
                    default:
                        break;
                }
            }
        }


        ~Server()
        {
            aServer.Shutdown("Shutting down Server");
            if (aServer.Status == NetPeerStatus.NotRunning) //Ak server nebesi
            {
                aStarted = false; //Nastavime atribut aStarted na FALSE
            }

            aServer = null;
        }
    }
}
