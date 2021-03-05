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
        private int aMaxChatMessageLength = 50;

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

        /// <summary>
        /// Atribut, ktory reprezentuje identifikator mna - Hraca - Typ Long
        /// </summary>
        private long aMyIdentifier;

        private LogicGame aLogicGame;

        private ClientSide.Chat.Chat aChatManager;


        public bool Connected { get => aConnected; set => aConnected = value; }

        public Client(string parAppName, LogicGame parGame, ClientSide.Chat.Chat parChatManager, string parNickName = "Player")
        {

            aLogicGame = parGame;

            aChatManager = parChatManager;

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
                            RequestConnectedClients();
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

                            // Debug.WriteLine(tmpIncommingMessage.ReadVariableInt32());

                            long tmpRUID = tmpIncommingMessage.ReadVariableInt64();
                            string tmpNickname = aDictionaryPlayerData[tmpRUID].PlayerNickName;
                            string tmpMessage = tmpIncommingMessage.ReadString();


                            HandleChatMessage(tmpNickname, tmpMessage);

                        }

                        if (tmpReceivedByte == (byte)PacketMessageType.Connect)
                        {

                            this.AddPlayer(tmpIncommingMessage, PacketInfoRequestType.Init_Connect);
                        }

                        if (tmpReceivedByte == (byte)PacketMessageType.RequestConnClientsData) //Ak Server odpovedal na ziadosť o RequestConnClientsData
                        {

                            while (this.AddPlayer(tmpIncommingMessage, PacketInfoRequestType.Request))
                            {

                            }
                        }

                        if (tmpReceivedByte == (byte)PacketMessageType.Disconnect) {

                            long tmpRID = tmpIncommingMessage.ReadVariableInt64();
                            Debug.WriteLine("Typ" + tmpReceivedByte);
                            Debug.WriteLine("Pokus o Remove" + tmpRID);
                            RemovePlayer(tmpRID); 
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

        /// <summary>
        /// Metoda, ktora spravuje Chat spravy - Odosle Chat Managerovi informaciu o tom aby ulozil spravu
        /// </summary>
        /// <param name="parSenderName">Parameter reprezentujuci meno odosielatela spravy - Typ string</param>
        /// <param name="parMessage">Parameter reprezentujuci spravu - Typ string</param>
        /// <param name="parMessageColor">Parameter reprezentujuci farbu spravy - Typ - Enum - ChatColors</param>
        /// <returns></returns>
        public bool HandleChatMessage(string parSenderName, string parMessage, ClientSide.Chat.ChatColors parMessageColor = 0)
        {
            if (aChatManager != null)
            {
                aChatManager.StoreAllMessages(parSenderName, parMessage, parMessageColor);
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Metoda, ktorou si Klient vyziada odoslanie informacii o uz pripojenych pouzivateloch
        /// </summary>
        public void RequestConnectedClients()
        {
            NetOutgoingMessage tmpOutgoingMessage = aClient.CreateMessage();
            tmpOutgoingMessage.Write((byte)PacketMessageType.RequestConnClientsData);
            aClient.SendMessage(tmpOutgoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Metoda, pomocou, ktorej sa pridava hrac do uloziska - Dictionary
        /// </summary>
        /// <param name="parIncommingMessage">Parameter reprezentujuci prichadzajucu spravu - Typ NetIncommingMessage</param>
        /// <param name="parRequestType">Parameter reprezentujuci o aky typ requestu ide - Connect, alebo Request informacii o ostatnym pouzivateloch - Typ - Enum - PacketInfoRequestType</param>
        /// <returns></returns>
        public bool AddPlayer(NetIncomingMessage parIncommingMessage, PacketInfoRequestType parRequestType)
        { 
            int tmpID = parIncommingMessage.ReadVariableInt32();

            string tmpNickname = parIncommingMessage.ReadString();

            long tmpRUID = parIncommingMessage.ReadVariableInt64();

            if (aDictionaryPlayerData.Count < 2) //Zabezpecime, aby pridalo do uloziska najviac 2 hracov, kedze hra je prave pre 2
            {
                if (parRequestType == PacketInfoRequestType.Init_Connect) //Ak ide o pripojenie hraca do hry
                {
                    if (aDictionaryPlayerData.Count <= 0) //Ak este ziaden hrac nie je ulozeny v databaze, vieme ze ide o mna
                    {
                        aDictionaryPlayerData.Add(tmpRUID, new ClientSide.PlayerClientData(tmpID, tmpNickname, tmpRUID, true)); //Pridame nove data o hracovi do uloziska, na zaklade Remote UID a pri udajoch o hracovi zadame, ze ide o nas
                    }
                    else {
                        aDictionaryPlayerData.Add(tmpRUID, new ClientSide.PlayerClientData(tmpID, tmpNickname, tmpRUID)); //Pridame nove data o hracovi do uloziska
                    }
                    
                    HandleChatMessage(tmpNickname, "Connected", ClientSide.Chat.ChatColors.Purple); //Odosleme spravu o tom, ze sa nejaky hrac pripojil
                    Debug.WriteLine("Klient - Connect - Data o Hracovi: " + tmpNickname + " boli pridane!");

                    return true;
                }
                else if (parRequestType == PacketInfoRequestType.Request) //Ak ide o Request Udajov o Inych Hracoch, teda ja som sa pripojil a chcem vediet, kto uz je pripojeny
                {
                    if (tmpID != -1) //Ak sa nejedna o prazdnu spravu
                    {
                        aDictionaryPlayerData.Add(tmpRUID, new ClientSide.PlayerClientData(tmpID, tmpNickname, tmpRUID)); //Pridame hraca do uloziska
                        HandleChatMessage(tmpNickname, "Is Already Here", ClientSide.Chat.ChatColors.Purple); //Odosleme spravu o tom, kto uz bol pripojeny - predomnou
                        Debug.WriteLine("Klient - Request - Data o Hracovi: " + tmpNickname + " boli pridane!");
                        return true;
                    }
                    else if (tmpID == 0 && string.IsNullOrEmpty(tmpNickname) && tmpRUID == 0) //Ak bol detegovany koniec spravy
                    {
                        Debug.WriteLine("Klient - Detekovany Koniec / Prazdna Sprava");
                        return false;
                    }
                    else //Ak nam prisla "prazdna" sprava
                    {
                        Debug.WriteLine("Klient - Request - Ziadne data neboli pridane! - Na Serveri ste len vy!");
                        return false;
                    }
                }
                else
                {
                    Debug.WriteLine("Klient - Request - Data neboli pridane - BAD REQUEST - Toto by nemalio nastat!!!");
                    return false;
                }
            }
            else if (tmpID == 0 && string.IsNullOrEmpty(tmpNickname) && tmpRUID == 0) //Ak bol detegovany koniec spravy
            {
                Debug.WriteLine("Klient - Detekovany Koniec / Prazdna Sprava");
                return false;
            }
            else // Pokial doslo k chybe
            {
                Debug.WriteLine("Klient - Request - Data o Hracovi: " + tmpNickname + " neboli pridane - toto by sa nemalo stat!!");
                return false;
            }
        }

        /// <summary>
        /// Metoda, ktora odstrani hraca z uloziska
        /// </summary>
        /// <param name="parRemoteUniqueIdentifier">Parameter reprezentujuci Remote Unique Idenfifier hraca - Typ Long</param>
        /// <returns>Vrati hodnotu true/false na zaklade uspechu/neuspechu operacie</returns>
        public bool RemovePlayer(long parRemoteUniqueIdentifier)
        {
            HandleChatMessage(aDictionaryPlayerData[parRemoteUniqueIdentifier].PlayerNickName, "Disconnected", ClientSide.Chat.ChatColors.Red);
            return aDictionaryPlayerData.Remove(parRemoteUniqueIdentifier);
        }

        /// <summary>
        /// Metoda, ktora ma za nasledok "vypnutie klienta"
        /// </summary>
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
