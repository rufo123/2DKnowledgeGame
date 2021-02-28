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
        /// Maximalna Dlzka Chatovej Spravy - Default 20
        /// </summary>
        private int aMaxChatMessageLength = 20;

        /// <summary>
        /// Atribut, ktory reprezentuje ci je Klient pripojeny k serveru alebo nie
        /// </summary>
        private bool aConnected;

        /// <summary>
        /// Dictionary obsahujúca Hráčov, ku ktorým sa bude pristupovať pomocou Remote Unique Identifikator - Typ PlayerServer
        /// Dictionary - ma O(1) Access narozdiel O(n) v Liste, preto som nezvolil List
        /// </summary>
        private Dictionary<long, ClientSide.PlayerClientData> aDictionaryPlayerData;

        /// <summary>
        /// Atribut Reprezentuje aky Maximalny pocet Hracov je Povoleny na Serveri - Default 2
        /// </summary>
        private int aMaxPlayers = 2;

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


            aDictionaryPlayerData = new Dictionary<long, ClientSide.PlayerClientData>(aMaxPlayers);


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

                        tmpReceivedByte = tmpIncommingMessage.ReadByte(); //Zainicializujeme lokalnu premennu typu byte - reprezentujucu prijaty byte

                        if (tmpReceivedByte == (byte)PacketMessageType.ChatMessage)
                        { 
                            Debug.WriteLine("Klient - Prijal potvrdenie o odoslanej sprave!");

                            Debug.WriteLine(tmpIncommingMessage.ReadVariableInt64());

                            Debug.WriteLine(tmpIncommingMessage.ReadVariableInt32());

                            Debug.WriteLine(tmpIncommingMessage.ReadString());
                        }

                        if (tmpReceivedByte == (byte)PacketMessageType.Connect) {

                            int tmpID = tmpIncommingMessage.ReadVariableInt32();

                            string tmpNickname = tmpIncommingMessage.ReadString();

                            long tmpRUID = tmpIncommingMessage.ReadVariableInt64();

                            this.AddPlayer(tmpID, tmpNickname, tmpRUID);
                        }

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

        /// <summary>
        /// Metoda, ktora sluzi na odosielanie chatovej spravy
        /// </summary>
        /// <param name="parMessage">Parameter Sprava - Typu String</param>
        /// <returns></returns>
        public bool SendChatMessage(string parMessage)
        {

            if (parMessage.Length <= aMaxChatMessageLength)
            {
                NetOutgoingMessage tmpOutgoingMessage = aClient.CreateMessage();
                tmpOutgoingMessage.Write((byte)PacketMessageType.ChatMessage);
                tmpOutgoingMessage.Write(parMessage);

                aClient.SendMessage(tmpOutgoingMessage, NetDeliveryMethod.ReliableOrdered);

                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AddPlayer(int parPlayerID, string parPlayerNickname, long parRemoteUniqueIdentifier) {

            if (aDictionaryPlayerData.Count < 2)
            {

                aDictionaryPlayerData.Add(parRemoteUniqueIdentifier, new ClientSide.PlayerClientData(parPlayerID, parPlayerNickname, parRemoteUniqueIdentifier));

                Debug.WriteLine("Klient - Data o Hracovi: " + parPlayerNickname + " boli pridane!");

                return true;
            }
            else {

                Debug.WriteLine("Klient - Data o Hracovi: " + parPlayerNickname + " neboli pridane - Toto by sa nemalo stat!!!");

                return false;
            }

        }

        public bool RemovePlayer(long parRemoteUniqueIdentifier) {

            return aDictionaryPlayerData.Remove(parRemoteUniqueIdentifier);
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
