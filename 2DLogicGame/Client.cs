using _2DLogicGame.GraphicObjects;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using _2DLogicGame.ClientSide.Levels;

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

        private ComponentCollection aClientObjects;

        private PlayerController aPlayerController;

        private Stopwatch aStopWatch;

        private const int aTickRate = 60;

        private long aMSPerFrame = 1000 / aTickRate;


        public bool Connected { get => aConnected; set => aConnected = value; }

        public Client(string parAppName, LogicGame parGame, ClientSide.Chat.Chat parChatManager, ComponentCollection parClientObjects, PlayerController parPlayController, string parNickName = "Player", string parIP = "127.0.0.1")
        {

            aIP = parIP;

            aLogicGame = parGame;

            aChatManager = parChatManager;

            aNickName = parNickName; //Inicializujeme NickName, default - "Player"

            NetPeerConfiguration tmpClientConfig = new NetPeerConfiguration(parAppName); //Vytvorime konfiguraciu klienta

            //tmpClientConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval); //Povolime prijmanie spravy typu - ConnectionApproval (Enum)

            aClientObjects = parClientObjects;

            aClient = new NetClient(tmpClientConfig); //Vytvorime Klienta so zvolenou konfiguraciou

            aStopWatch = new Stopwatch();

            aClient.Start(); //Spustime Klienta

            NetOutgoingMessage tmpOutgoingMessage = aClient.CreateMessage(); //Vytvorime NetOutgoingMessage
            tmpOutgoingMessage.Write((byte)PacketMessageType.Connect); //Zapiseme do spravy byte o hodnote Enumu - PacketMessageType.Connect
            tmpOutgoingMessage.Write(aNickName); //Odosleme String s Nickom Clienta


            aClient.Connect(host: aIP, port: aPort, hailMessage: tmpOutgoingMessage); //Pripojime klienta na zadaneho hosta, port a spravu, ktoru chceme odoslat spolu s inicializaciou


            aDictionaryPlayerData = new Dictionary<long, ClientSide.PlayerClientData>(aMaxPlayers);

            aPlayerController = parPlayController;

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
                aStopWatch.Start();
                long tmpStartTime = aStopWatch.ElapsedMilliseconds;
                int tmpTimeToSleep;


                // aClient.MessageReceivedEvent.WaitOne();
                NetIncomingMessage tmpIncommingMessage;

                tmpIncommingMessage = aClient.ReadMessage();

                if (tmpIncommingMessage == null)
                {
                    tmpTimeToSleep = tmpTimeToSleep = unchecked((int)(tmpStartTime + aMSPerFrame - aStopWatch.ElapsedMilliseconds));
                    if (tmpTimeToSleep < 0)
                    {
                        tmpTimeToSleep = 0;
                    }
                    Thread.Sleep(tmpTimeToSleep);
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

                        if (tmpReceivedByte == (byte)PacketMessageType.Disconnect)
                        {

                            long tmpRID = tmpIncommingMessage.ReadVariableInt64();
                            Debug.WriteLine("Typ" + tmpReceivedByte);
                            Debug.WriteLine("Pokus o Remove" + tmpRID);
                            RemovePlayer(tmpRID);
                        }

                        if (tmpReceivedByte == (byte)PacketMessageType.Movement)
                        {
                            long tmpRUID = tmpIncommingMessage.ReadVariableInt64();
                            if (tmpRUID == aMyIdentifier)
                            {
                                aDictionaryPlayerData[tmpRUID].AwaitingMovementMessage = false;
                                bool tmpDataOk = aDictionaryPlayerData[tmpRUID].PrepareDownloadedData(tmpIncommingMessage, aPlayerController.GameTime, true); //Ide o Moje Data
                                if (!tmpDataOk) //Ak boli detekovane chybne data, pravdepodobne nespravne odoslane
                                {
                                    aPlayerController.UpdateNeeded = true; //Nastavime, ze je treba znova poslat update

                                }

                                continue;
                            }
                            aDictionaryPlayerData[tmpRUID].PrepareDownloadedData(tmpIncommingMessage, aPlayerController.GameTime); //Ide o data o spoluhracoch

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

                tmpTimeToSleep = tmpTimeToSleep = unchecked((int)(tmpStartTime + aMSPerFrame - aStopWatch.ElapsedMilliseconds));
                if (tmpTimeToSleep < 0)
                {
                    tmpTimeToSleep = 0;
                }
                Thread.Sleep(tmpTimeToSleep);

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
                        aDictionaryPlayerData.Add(tmpRUID, new ClientSide.PlayerClientData(tmpID, tmpNickname, tmpRUID, aLogicGame, new Vector2(800, 800), new Vector2(49, 64), parIsMe: true)); //Pridame nove data o hracovi do uloziska, na zaklade Remote UID a pri udajoch o hracovi zadame, ze ide o nas
                        aPlayerController.SetPlayer(aDictionaryPlayerData[tmpRUID]);
                        aClientObjects.AddComponent(aDictionaryPlayerData[tmpRUID]);
                        aLogicGame.Components.Add(aPlayerController);
                        aMyIdentifier = tmpRUID;

                    }
                    else
                    {
                        aDictionaryPlayerData.Add(tmpRUID, new ClientSide.PlayerClientData(tmpID, tmpNickname, tmpRUID, aLogicGame, new Vector2(800, 800), new Vector2(49, 64))); //Pridame nove data o hracovi do uloziska
                        aClientObjects.AddComponent(aDictionaryPlayerData[tmpRUID]);
                    }

                    HandleChatMessage(tmpNickname, "Connected", ClientSide.Chat.ChatColors.Purple); //Odosleme spravu o tom, ze sa nejaky hrac pripojil
                    Debug.WriteLine("Klient - Connect - Data o Hracovi: " + tmpNickname + " boli pridane!");

                    return true;
                }
                else if (parRequestType == PacketInfoRequestType.Request) //Ak ide o Request Udajov o Inych Hracoch, teda ja som sa pripojil a chcem vediet, kto uz je pripojeny
                {
                    if (tmpID != -1) //Ak sa nejedna o prazdnu spravu
                    {
                        aDictionaryPlayerData.Add(tmpRUID, new ClientSide.PlayerClientData(tmpID, tmpNickname, tmpRUID, aLogicGame, new Vector2(800, 800), new Vector2(49, 64))); //Pridame hraca do uloziska
                        aClientObjects.AddComponent(aDictionaryPlayerData[tmpRUID]);
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
            aLogicGame.Components.Remove(aDictionaryPlayerData[parRemoteUniqueIdentifier]);
            return aDictionaryPlayerData.Remove(parRemoteUniqueIdentifier);
        }

        public void SendClientData()
        {
            NetOutgoingMessage tmpOutMessPlayData = aClient.CreateMessage();
            tmpOutMessPlayData = aDictionaryPlayerData[aMyIdentifier].PrepareDataForUpload(tmpOutMessPlayData);
            if (tmpOutMessPlayData != null)
            {
                aClient.SendMessage(tmpOutMessPlayData, NetDeliveryMethod.ReliableOrdered);
            }
        }

        /// <summary>
        /// Metoda, ktora ovlada pohyb spoluhracov
        /// </summary>
        /// <param name="gameTime"></param>
        public void TeammateMovementHandler(GameTime gameTime) //Metoda, na ovladanie pohybu spolhracov
        {
            if (aDictionaryPlayerData.Count > 1)
            {
                foreach (KeyValuePair<long, ClientSide.PlayerClientData> dictItem in aDictionaryPlayerData.ToList())
                {
                    //Prejdeme vsetky data v Dictionary
                    {
                        if (dictItem.Key != aMyIdentifier)
                        {
                            dictItem.Value.Move(gameTime);
                        }

                    }

                }
            }
        }

        //Docasne LevelManagera odovzdam ako referenciu, porozmyslat, ci by nebolo lepsie ho definovat rovno v triede
        public void CollisionHandler(GameTime gameTime, LevelManager parLevelManager)
        {
            if (aDictionaryPlayerData != null && aDictionaryPlayerData.Count > 0)
            {
                foreach (KeyValuePair<long, ClientSide.PlayerClientData> dictItem in aDictionaryPlayerData.ToList())
                {
                    //Prejdeme vsetky data v Dictionary
                    {
                        //Bude reprezentovat, poziciu TILU, kde by sa teda hrac nachadzal - pozor nie bloku!, Tile reprezentuje OBLAST jedneho bloku tzn
                        //Napr. Block so suradnicami 0, 128 - Bude ekvivalentny Tilu so suradnicami 0, 2
                        //Ak mame teda specifikovanu velkost blokov o 64px...

                        int tmpMapBlockDimSize = parLevelManager.GetMapBlocksDimensionSize();

                        float sizeOfPlayerX = dictItem.Value.Size.X * dictItem.Value.EntityScale;
                        float sizeOfPLayerY = dictItem.Value.Size.Y * dictItem.Value.EntityScale;

                        int tmpTilePositionX = (int)Math.Floor(dictItem.Value.GetAfterMoveVector2(gameTime).X / tmpMapBlockDimSize); //Zaciatocna X-ova Tile Suradnica - Vlavo
                        int tmpTilePositionY = (int)Math.Floor(dictItem.Value.GetAfterMoveVector2(gameTime).Y / tmpMapBlockDimSize); //Zaciatocna Y-ova Tile Suradnica - Hore

                        int tmpEndTilePositionX = (int)Math.Floor((dictItem.Value.GetAfterMoveVector2(gameTime).X + sizeOfPlayerX)/ tmpMapBlockDimSize); //Koncova X-ova Tile Suradnica - Vpravo
                        int tmpEndTilePositionY = (int)Math.Floor((dictItem.Value.GetAfterMoveVector2(gameTime).Y + sizeOfPLayerY) / tmpMapBlockDimSize); //Koncova Y-ova Tile Suraadnica - Dole

                        Debug.WriteLine(dictItem.Value.GetAfterMoveVector2(gameTime).X);
                        
                        //Vypocitame si kolko blokov zabera entita

                      /*  int tmpWidth = (int)Math.Ceiling(sizeOfPlayerX / tmpMapBlockDimSize);
                        int tmpHeight = (int)Math.Ceiling(sizeOfPLayerY / tmpMapBlockDimSize);

                        if (tmpWidth < 1)
                        {
                            tmpWidth = 1;
                        }

                        if (tmpHeight < 1)
                        {
                            tmpHeight = 1;
                        } */
                       
                        // float tmpNumberOfOccupiedBlocks = tmpWidth * tmpHeight;}}
                        bool tmpIsBlocked = false;

                        for (int i = tmpTilePositionX; i <= tmpEndTilePositionX; i++) //For Cyklus pre X-ovu Suradnicu, kde by v buducnosti stala Entita
                        {
                            for (int j = tmpTilePositionY; j <= tmpEndTilePositionY; j++) //FOr Cyklus pre Y-ovu Suradnicu, kde by v buducnosti stala Entita
                            {
             
                                Vector2 tmpTilePositVector2 = new Vector2(i * tmpMapBlockDimSize, j * tmpMapBlockDimSize);
                                if (parLevelManager.GetBlockByPosition(tmpTilePositVector2) != null) //Ak na takejto suradnici vobec nejaky blok existuje
                                {
                                    if (parLevelManager.GetBlockByPosition(tmpTilePositVector2).BlockCollisionType == BlockCollisionType.Wall) //Ak je prekazka typu WALL - Dojde ku kolizii
                                    {
                                        //Debug.Flush();
                                        //Debug.WriteLine("Collision");
                                        parLevelManager.GetBlockByPosition(tmpTilePositVector2).ChangeColor(true); /////////////// ZMAZAT
                                        tmpIsBlocked = true;
                                        dictItem.Value.IsBlocked = tmpIsBlocked;
                                    }
                                }
                            }
                        }
                        dictItem.Value.IsBlocked = tmpIsBlocked;
                    }
                }

            }



            //Zajtra dorobit detekciu kolizie blokov
            //Budu sa musiet prehladavat v Dictionary vsetky bloky na ktorych postava stoji


            //P 1 - dictItem.Value.GetAfterMoveVector2().X < 
            //P 2
            //P 3
            //P 4

            //Po zaokruhleni, prekonvertujeme suradnice spat




            //Takto nejako by sa riesila kolizia, ale ja to chcem zjednodusit a navyse s pristupom do Dictionary
            /*  if (PLAY_X_AFTER < BLOCK_X + BLOCK_X_SIRKA &&
                   PLAY_X_AFTER + BLOCK_X_SIRKA > BLOCK_X &&
                   PLAY_Y_AFTER < BLOCK_Y + BLOCK_Y_SIRKA &&
                   PLAY_Y_AFTER + BLOCK_Y_SIRKA > BLOCK_Y)
            { */

        }


        /// <summary>
        /// Metoda, ktora ma za nasledok "vypnutie klienta"
        /// </summary>
        public void Shutdown()
        {

            aClient.Disconnect("Connection Dropped");
            aClient.Shutdown("Shutting Down Client");

            foreach (KeyValuePair<long, ClientSide.PlayerClientData> dictItem in aDictionaryPlayerData) //Prejdeme vsetky data v Dictionary
            {
                aLogicGame.Components.Remove(dictItem.Value);
            }

            aDictionaryPlayerData.Clear();




            if (aClient.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                aConnected = false;
            }

            aClient = null;

            Debug.WriteLine("Shutting Down Client");
        }


    }
}
