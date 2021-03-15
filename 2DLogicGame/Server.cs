using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        /// <summary>
        /// Dictionary obsahujúca Hráčov, ku ktorým sa bude pristupovať pomocou Remote Unique Identifikator - Typ PlayerServer
        /// Dictionary - ma O(1) Access narozdiel O(n) v Liste, preto som nezvolil List
        /// </summary>
        private Dictionary<long, ServerSide.PlayerServerData> aDictionaryPlayerData;

        /// <summary>
        /// Atribut Reprezentuje aky Maximalny pocet Hracov je Povoleny na Serveri - Default 2
        /// </summary>
        private int aMaxPlayers = 2;

        /// <summary>
        /// Atribut, reprezentujuci hry typu - LogicGame
        /// </summary>
        private LogicGame aLogicGame;


        /// <summary>
        /// Atribut reprezeentujuci TickRate serveru - typ int
        /// </summary>
        private const int aTickRate = 60; //Tikov za sekundu

        /// <summary>
        /// Atribut reprezentuje, kolko milisekund sa ma rovnat jednemu framu - typ long
        /// </summary>
        private long aMSPerFrame = 1000 / aTickRate;

        /// <summary>
        /// Atribut, ktory reprezentuje casovac - zabezpecujuci staly TickRate
        /// </summary>
        private System.Diagnostics.Stopwatch aStopWatch;

        /// <summary>
        /// Vlakno, ktore bude zabezpecovat spracovanie pohybu
        /// </summary>
        private Thread aMovementThread;


        public bool Started
        {
            get => aStarted;
            set => aStarted = value;
        }

        public Server(string parAppName, LogicGame parGame)
        {

            aLogicGame = parGame;

            NetPeerConfiguration
                tmpServerConfig = new NetPeerConfiguration(parAppName)
                {
                    Port = aPort
                }; //Nastavime si Meno nasej Aplikacie, ktora bude sluzi na validaciu pri pripojeni a Port

            tmpServerConfig.EnableMessageType(NetIncomingMessageType
                .ConnectionApproval); //Povolime prijmanie spravy typu - ConnectionApproval (Enum)

            aServer = new NetServer(tmpServerConfig); //Inicializujeme Server s nami zadanou Konfiguraciou


            aServer.Start(); //Zapneme nas Web Server
            //Ked volame Start() - Lidgren nabinduje vhodny Network Socket a vytvori na pozadi Thread, ktory spracuje prave Sietovanie...

            if (aServer.Status == NetPeerStatus.Running)
            {
                //Ak server bezi, nastavime atribut aStared na TRUE
                aStarted = true;
            }
            else
            {
                aStarted = false;
            }

            Debug.WriteLine("Server Connection Initiated");

            aDictionaryPlayerData = new Dictionary<long, ServerSide.PlayerServerData>(aMaxPlayers);


            aStopWatch = new Stopwatch();

            Debug.WriteLine(aDictionaryPlayerData.Count);

            aMovementThread = new Thread(new ThreadStart(this.MovementHandler));
            aMovementThread.Start();

        }

        public void MovementHandler()
        {

            if (aStopWatch.IsRunning != true)
            {
                aStopWatch.Start();
            }

            int tmpTimeToSleep = 0; //Inicializujeme si premennu - reprezentujucu, kolko ma server "spat"

            while (aLogicGame.GameState != GameState.Exit)
            {

                long tmpStartTime = aStopWatch.ElapsedMilliseconds; //Nastavime zaciatocny cas



                if (aDictionaryPlayerData.Count > 0) //Ak je niekto pripojeny na server
                {
                    foreach (KeyValuePair<long, ServerSide.PlayerServerData> dictItem in aDictionaryPlayerData.ToList()
                    ) //Prejdeme vsetky data v Dictionary
                    {
                        dictItem.Value.Move((float)(tmpTimeToSleep));
                        var elapsed = aStopWatch.ElapsedMilliseconds;
                    }

                }

                //Pripravi na odoslanie klientom

         

                tmpTimeToSleep = unchecked((int)(tmpStartTime + aMSPerFrame - aStopWatch.ElapsedMilliseconds));
                if (tmpTimeToSleep < 0) //Poistime si aby cas, ktory ma vlakno spat nebol zaporny
                {
                    tmpTimeToSleep = 0;
                }


                Thread.Sleep(tmpTimeToSleep); //Pokus o implementaciu konstantneho TICKRATU
            }


        }

        public void ReadMessages()
        {
            if (aStopWatch.IsRunning != true)
            {
                aStopWatch.Start(); //Zapneme casovat
            }


            while (aLogicGame.GameState != GameState.Exit)
            {

                long tmpStartTime = aStopWatch.ElapsedMilliseconds; //Nastavime zaciatocny cas
                int tmpTimeToSleep; //Inicializujeme si premennu - reprezentujucu, kolko ma server "spat"


                //aServer.MessageReceivedEvent.WaitOne();

                NetIncomingMessage tmpIncommingMessage; //Vytvorime lokalnu premennu typu NetIncommingMessage

                tmpIncommingMessage = aServer.ReadMessage(); //Spravu inicializujeme ako aServer.ReadMessage() - Read a pending message from any connection, if any 

                if (tmpIncommingMessage == null) //Pokial na server nepride ziadna sprava
                {
                    tmpTimeToSleep = unchecked((int)(tmpStartTime + aMSPerFrame - aStopWatch.ElapsedMilliseconds));
                    if (tmpTimeToSleep < 0) //Poistime si aby cas, ktory ma vlakno spat nebol zaporny
                    {
                        tmpTimeToSleep = 0;
                    }

                    Thread.Sleep(tmpTimeToSleep); //Pokus o implementaciu konstantneho TICKRATU

                    continue; //Ak neprisla ziadna sprava, vratime sa spat na zaciatok cyklu, resp na koniec...
                }

                byte tmpReceivedByte;

                switch (tmpIncommingMessage.MessageType)
                {
                    case NetIncomingMessageType.Error:
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        Debug.WriteLine("Status - " + tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier + " Status: " + tmpIncommingMessage.SenderConnection.Status);

                        switch (tmpIncommingMessage.SenderConnection.Status)
                        {
                            case NetConnectionStatus.None:
                                break;
                            case NetConnectionStatus.InitiatedConnect:
                                break;
                            case NetConnectionStatus.ReceivedInitiation:
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                break;
                            case NetConnectionStatus.RespondedConnect:
                                break;
                            case NetConnectionStatus.Connected:
                                SendClientPlayerData(tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier);
                                break;
                            case NetConnectionStatus.Disconnecting:
                                break;
                            case NetConnectionStatus.Disconnected:

                                string tmpReason = tmpIncommingMessage.ReadString();
                                if (string.IsNullOrEmpty(tmpReason))
                                {
                                    Debug.WriteLine("DSDASDADAS ---" + tmpReason);
                                }
                                else
                                {
                                    Debug.WriteLine("dSDAdsdadasda ---- " + tmpReason);
                                    DisconnectClient(tmpIncommingMessage);
                                }

                                break;
                            default:
                                break;
                        }

                        break;
                    case NetIncomingMessageType.UnconnectedData:
                        break;
                    case NetIncomingMessageType.ConnectionApproval:

                        tmpReceivedByte = tmpIncommingMessage.ReadByte(); //Zainicializujeme lokalnu premennu typu byte - reprezentujucu prijaty byte

                        if (tmpReceivedByte == (byte)PacketMessageType.Connect) //Ak sa prijaty byte rovna hodnote Enumu - Connect
                        {

                            Debug.WriteLine("Server - Spojenie bolo uspesne nadviazane!");

                            //Prijmeme spravu od Clienta - Nickname a Potvrdime pripojenie
                            string tmpNickName = tmpIncommingMessage.ReadString(); //Precitame String, ktory reprezentuje NickName

                            long tmpRmtUID = tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier;

                            if (AddPlayer(tmpNickName, tmpRmtUID))
                            {
                                tmpIncommingMessage.SenderConnection.Approve(); //Povolime resp "Approvneme" Pripojenie

                            }
                            else
                            {
                                tmpIncommingMessage.SenderConnection.Deny("Pripojenie sa nezdarilo - Server je Plny"); //Povolime resp "Approvneme" Pripojenie
                            }

                        }
                        else
                        {
                            tmpIncommingMessage.SenderConnection.Deny("Pripojenie sa nezdarilo - Neboli odoslane spravne informacie o pripojeni!");
                        }
                        break;
                    case NetIncomingMessageType.Data:

                        tmpReceivedByte = tmpIncommingMessage.ReadByte();

                        if (tmpReceivedByte == (byte)PacketMessageType.ChatMessage)
                        {

                            Debug.WriteLine("Server - Prijal Spravu:");

                            string tmpChatMessage = tmpIncommingMessage.ReadString();

                            SendChatMessageToClients(tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier, tmpChatMessage);

                        }
                        else if (tmpReceivedByte == (byte)PacketMessageType.RequestConnClientsData)
                        {

                            Debug.WriteLine("Server - Zaregistroval ziadost o odoslani dat o Klientoch");

                            NetOutgoingMessage tmpClientInfoMessage = aServer.CreateMessage();

                            tmpClientInfoMessage.Write((byte)PacketMessageType.RequestConnClientsData); //Zapiseme ze ide o odpoved na Request

                            foreach (KeyValuePair<long, ServerSide.PlayerServerData> dictItem in aDictionaryPlayerData) //Prejdeme vsetky data v Dictionary
                            {
                                tmpClientInfoMessage = dictItem.Value.PrepareIdentificationData(tmpClientInfoMessage, tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier); //Odovzdame vsetky data o Hracoch, okrem ziadatela
                            }

                            if (tmpClientInfoMessage.LengthBits <= 8)
                            {

                                tmpClientInfoMessage.WriteVariableInt32(-1);
                            }

                            aServer.SendMessage(tmpClientInfoMessage, tmpIncommingMessage.SenderConnection, NetDeliveryMethod.ReliableOrdered);


                        }
                        else if (tmpReceivedByte == (byte)PacketMessageType.Movement)
                        {

                            Debug.WriteLine("Prisla sprava od " + tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier);
                            //Prijme data od klienta
                            aDictionaryPlayerData[tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier].HandleReceivedData(tmpIncommingMessage);

                            //Odosle spat

                            NetOutgoingMessage tmpOutMovMessage = aServer.CreateMessage();
                            tmpOutMovMessage.Write((byte)PacketMessageType.Movement);
                            tmpOutMovMessage.WriteVariableInt64(tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier);
                            tmpOutMovMessage = aDictionaryPlayerData[tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier].PrepareDataForUpload(tmpOutMovMessage);
                            aServer.SendToAll(tmpOutMovMessage, NetDeliveryMethod.ReliableOrdered); 
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

                tmpTimeToSleep = unchecked((int)(tmpStartTime + aMSPerFrame - aStopWatch.ElapsedMilliseconds)); //POkus o TickRate
                if (tmpTimeToSleep < 0)
                {
                    tmpTimeToSleep = 0;
                }
                Thread.Sleep(tmpTimeToSleep); //Pokus o implementaciu konstantneho TICKRATU

            }

            aStopWatch.Stop();
            Shutdown();
        }

        /// <summary>
        /// Odosle Chatovu Spravu Vsetkym Klientom spolu s Unikatnym Identifikatorom Odosielatela a samozrejme spravou
        /// </summary>
        /// <param name="parRemoteUniqueIdentifier">Parameter Remote Unique Identifier - Typu Long</param>
        /// <param name="parMessage">Parameter Sprava - Typu String</param>
        public void SendChatMessageToClients(long parRemoteUniqueIdentifier, string parMessage)
        {

            NetOutgoingMessage tmpOutgoingMessage = aServer.CreateMessage();
            tmpOutgoingMessage.Write((byte)PacketMessageType.ChatMessage); //Vratime spat spravu, ktoru server odoslal
            tmpOutgoingMessage.WriteVariableInt64(parRemoteUniqueIdentifier);
            //tmpOutgoingMessage.WriteVariableInt32(aDictionaryPlayerData[parRemoteUniqueIdentifier].PlayerID);
            tmpOutgoingMessage.Write(parMessage); //Hodnota boolean o tom, ze pripojenie prebehlo uspesne

            aServer.SendToAll(tmpOutgoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Prida Udajove o Hracovi do Udajovej Struktury - Dictionary, kde sa zaroven vytvori novy objekt typu PlayerServerData na zaklade ID, Nicknamu a Remove Unique Identifieru
        /// </summary>
        /// <param name="parPlayerNickname">Parameter Prezyvka Hraca - Typu String</param>
        /// <param name="parRemoteUniqueIdentifier">Parameter Remote Unique Identifier - Typu Long</param>
        /// <returns></returns>
        public bool AddPlayer(string parPlayerNickname, long parRemoteUniqueIdentifier)
        {

            if (aDictionaryPlayerData.Count < 2)
            { //Ak je na Serveri volne Miesto

                int tmpNewPlayerID = aDictionaryPlayerData.Count + 1; //Zainicializujeme si nove ID Hraca
                aDictionaryPlayerData.Add(parRemoteUniqueIdentifier, new ServerSide.PlayerServerData(tmpNewPlayerID, parPlayerNickname, parRemoteUniqueIdentifier, new ServerSide.Vector2(800, 800), new ServerSide.Vector2(49, 64))); //Do Dictionary si pridame noveho Hraca s Novym ID a vytvorime objekt typu PlayerServerData podobne spolu s ID a Prezyvkou Hraca

                Debug.WriteLine("Server - Hrac bol pridany!" + " Nickname: " + parPlayerNickname + " ID " + tmpNewPlayerID + " RID " + parRemoteUniqueIdentifier);

                return true;
            } //Ak je Server plny
            else
            {
                Debug.WriteLine("Server - Hrac nemohol byt Pridany - Server je Plny!");
                return false;
            }
        }

        /// <summary>
        /// Odosleme Clientom Data o Pripojenom Hracovi - Pouzite ked sa aktualne napoji
        /// </summary>
        /// <param name="parRemoteUniqueIdentifier"> Remote Unique Identifikator - Typu Long</param>
        public void SendClientPlayerData(long parRemoteUniqueIdentifier)
        {

            NetOutgoingMessage tmpOutgoingMessage = aServer.CreateMessage();
            tmpOutgoingMessage.Write((byte)PacketMessageType.Connect); //Identifikator o tom, ze ide o Spravu ohladom Pripojenia

            tmpOutgoingMessage.WriteVariableInt32(aDictionaryPlayerData[parRemoteUniqueIdentifier].PlayerID); //Player ID
            tmpOutgoingMessage.Write(aDictionaryPlayerData[parRemoteUniqueIdentifier].PlayerNickName); //Player Nickname
            tmpOutgoingMessage.WriteVariableInt64(parRemoteUniqueIdentifier); //Remote Unique Identifcator

            aServer.SendToAll(tmpOutgoingMessage, NetDeliveryMethod.ReliableOrdered); //Chceme - Reliable a zachovat postupnost dat...

        }

        /// <summary>
        /// Odoberie Data o Hracovi z Udajovej struktury typu Dictionary na zaklade Kluca - Remote Unique Indentifier
        /// </summary>
        /// <param name="parRemoveUniqueIdentifier">Parameter - Remote Unique Identifier - Typu Long</param>
        /// <returns> Vracia hodnotu boolean - Ci odstranenie Hraca prebehlo uspesne alebo nie</returns>
        public bool RemovePlayer(long parRemoveUniqueIdentifier)
        {
            return aDictionaryPlayerData.Remove(parRemoveUniqueIdentifier);
        }

        /// <summary>
        /// Metoda, ktora odpoji klienta - vymaze ho z uloziska, a odosle informaciu ostatnym o jeho odpojeni
        /// </summary>
        /// <param name="parMessage">Parameter reprezentujuci prichodziu spravu - Typ NetIncommingMessage</param>
        /// <returns></returns>
        public bool DisconnectClient(NetIncomingMessage parMessage)
        {

            long tmpRemoteUniqueIdentifier = parMessage.SenderConnection.RemoteUniqueIdentifier;
            aDictionaryPlayerData.Remove(tmpRemoteUniqueIdentifier);

            // parMessage.SenderConnection.Disconnect("Disconnect Requested");

            NetOutgoingMessage tmpOutgoingMessage = aServer.CreateMessage();
            tmpOutgoingMessage.Write((byte)PacketMessageType.Disconnect);

            tmpOutgoingMessage.WriteVariableInt64(tmpRemoteUniqueIdentifier);

            aServer.SendToAll(tmpOutgoingMessage, NetDeliveryMethod.ReliableOrdered);


            return true;
        }


        /// <summary>
        /// Metoda, ktora ma za nasledok vypnutie serveru
        /// </summary>
        public void Shutdown()
        {
            aServer.Shutdown("Shutting down Server");
            if (aServer.Status == NetPeerStatus.NotRunning) //Ak server nebesi
            {
                aStarted = false; //Nastavime atribut aStarted na FALSE
            }

            aServer = null;

            aMovementThread.Join();

            Debug.WriteLine("Shutting Down Server");


        }

    }
}
