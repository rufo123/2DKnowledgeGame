using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace _2DLogicGame
{



    class Client
    {

        private NetClient aClient;
        private string aIP = "127.0.0.1";
        private int aPort = 28741;
        private string aNickName;

        /// <summary>
        /// Atribut, ktory reprezentuje ci je Klient pripojeny k serveru alebo nie
        /// </summary>
        private bool aConnected;


        private LogicGame aLogicGame;


        public bool Connected { get => aConnected; set => aConnected = value; }

        public Client(string parAppName, LogicGame parGame, string parNickName = "Player")
        {

            aLogicGame = parGame;

            aNickName = parNickName; //Inicializujeme NickName, default - "Player"

            NetPeerConfiguration tmpClientConfig = new NetPeerConfiguration(parAppName); //Vytvorime konfiguraciu klienta

            //tmpClientConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval); //Povolime prijmanie spravy typu - ConnectionApproval (Enum)


            aClient = new NetClient(tmpClientConfig); //Vytvorime Klienta so zvolenou konfiguraciou


            aClient.Start(); //Spustime Klienta

            NetOutgoingMessage tmpOutgoingMessage = aClient.CreateMessage(); //Vytvorime NetOutgoingMessage
            tmpOutgoingMessage.Write((byte)PacketMessageType.Connect); //Zapiseme do spravy byte o hodnote Enumu - PacketMessageType.Connect
            tmpOutgoingMessage.Write(aNickName); //Odosleme String s Nickom Clienta


            aClient.Connect(host: aIP, port: aPort, hailMessage: tmpOutgoingMessage); //Pripojime klienta na zadaneho hosta, port a spravu, ktoru chceme odoslat spolu s inicializaciou



            if (aClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                aConnected = true;
            }
            else
            {
                aConnected = false;
            }




         /*   Thread thread;
            thread = new Thread(new ThreadStart(ReadMessages));
            thread.Start();

            thread.Join(); */



        }


        public void ReadMessages()
        {


            while (aLogicGame.GameState != GameState.Exit)
            {


                // aClient.MessageReceivedEvent.WaitOne();


                NetIncomingMessage tmpIncommingMessage;

                tmpIncommingMessage = aClient.ReadMessage();

                if (tmpIncommingMessage == null)
                {
                    continue;
                }

                byte tmpReceivedByte;

                switch (tmpIncommingMessage.MessageType)
                {
                    case NetIncomingMessageType.Error:
                        break;
                    case NetIncomingMessageType.StatusChanged:

                        tmpReceivedByte = tmpIncommingMessage.ReadByte(); //Zainicializujeme lokalnu premennu typu byte - reprezentujucu prijaty byte

                        if (tmpReceivedByte == (byte)NetConnectionStatus.Connected)
                        {
                            aConnected = true;
                            Debug.WriteLine("Klient - Klient sa uspesne pripojil!");
                        }

                        break;
                    case NetIncomingMessageType.UnconnectedData:
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
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

            Shutdown(); //Mozno dobre ako destruktor...

        }


        public void Shutdown()
        {
            aClient.Disconnect("Connection Dropped");
            aClient.Shutdown("Shutting Down Client");

            if (aClient.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                aConnected = false;
            }

            aClient = null;

            Debug.WriteLine("Shutting Down Client");
        }


    }
}
