using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using _2DLogicGame.ClientSide.MathProblem;
using _2DLogicGame.ServerSide;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using _2DLogicGame.ServerSide.Levels_ServerSide;
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

        /// <summary>
        /// Atribut, reprezentujuci Level Managera - Typ LevelManager
        /// </summary>
        private LevelManager aLevelManager;

        /// <summary>
        /// Vlakno, ktore bude zabezpecovat, to, ze ak niekto nestoji na bloku, nebude si to ani nejaky blok mysliet
        /// </summary>
        private Thread aStandableBlocksHandlerThread;

        /// <summary>
        /// List, ktory bude obsahovat, bloky, ktore este neboli aktualizovane o informaciu, ze na nich nic nestoji
        /// </summary>
        private List<BlockServer> aStandableBlocksList;

        /// <summary>
        /// Thread, ktory sa stara o internu funkcnost levelov
        /// </summary>
        private Thread aLevelManagerThread;


        public bool Started
        {
            get => aStarted;
            set => aStarted = value;
        }

        public Server(string parAppName, LogicGame parGame)
        {

            aLogicGame = parGame;
            aLevelManager = new LevelManager(parGame);

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

            aLevelManager.InitLevelByNumber(3);

            aStopWatch = new Stopwatch();

            Debug.WriteLine(aDictionaryPlayerData.Count);

            aMovementThread = new Thread(new ThreadStart(this.MovementHandler));
            aMovementThread.Start();

            aStandableBlocksList = new List<BlockServer>();

             aStandableBlocksHandlerThread = new Thread(new ThreadStart(this.StandableBlocksHandler));
            aStandableBlocksHandlerThread.Start();

            aLevelManagerThread = new Thread(new ThreadStart(this.LevelUpdateHandler));
            aLevelManagerThread.Start();

        }



        /// <summary>
        /// Metoda, ktora sluzi na riadenie pohybu klienotv na serveri
        /// </summary>
        public void MovementHandler()
        {

            if (aStopWatch.IsRunning != true)
            {
                aStopWatch.Start(); //Zapneme casovac
            }

            int tmpTimeToSleep = 0; //Inicializujeme si premennu - reprezentujucu, kolko ma server "spat"

            while (aLogicGame.GameState != GameState.Exit)
            {

                long tmpStartTime = aStopWatch.ElapsedMilliseconds; //Nastavime zaciatocny cas



                if (aDictionaryPlayerData.Count > 0) //Ak je niekto pripojeny na server
                {
                    foreach (KeyValuePair<long, ServerSide.PlayerServerData> dictItem in
                        aDictionaryPlayerData.ToList() //ToList -> Nakopiruje cely Dictionary do Listu... 
                    ) //Prejdeme vsetky data v Dictionary
                    {

                        CollisionHandler(aLevelManager, dictItem.Value, dictItem.Key);

                        dictItem.Value.Move((float)(aTickRate));
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

        public void LevelUpdateHandler()
        {
            if (aStopWatch.IsRunning != true)
            {
                aStopWatch.Start(); //Zapneme casovac
            }

            int tmpTimeToSleep = 0; //Inicializujeme si premennu - reprezentujucu, kolko ma server "spat"

            while (aLogicGame.GameState != GameState.Exit)
            {

                long tmpStartTime = aStopWatch.ElapsedMilliseconds; //Nastavime zaciatocny cas

                if (aDictionaryPlayerData != null && aDictionaryPlayerData.Count > 0)
                {

                    switch (aLevelManager.LevelName)
                    {
                        case "Math":
                            aLevelManager.LevelMap.GetMathProblemManager().Update();
                            break;
                        case "Questions":
                            aLevelManager.LevelMap.GetQuestionManager().Update();
                            break;
                        case "English":
                            aLevelManager.LevelMap.GetEnglishManager().Update();
                            break;
                        default:
                            break;
                    }

                    if (aLevelManager.IsUpdateNeeded())
                    {
                        aLevelManager.UpdatePoints();
                        NetOutgoingMessage tmpOutgoingMessage = aServer.CreateMessage();
                        tmpOutgoingMessage.Write((byte)PacketMessageType.LevelData);
                        tmpOutgoingMessage = aLevelManager.PrepareLevelDataForUpload(tmpOutgoingMessage);
                        aServer.SendToAll(tmpOutgoingMessage, NetDeliveryMethod.ReliableOrdered);

                    }

                    if (aLevelManager.WinCheck() && aLevelManager.WinInfoRequested == false) //Ak doslo k vyhre a este sme sme neprebrali informacie o vyhre
                    {
                        aLevelManager.WinInfoRequested = true;
                        NetOutgoingMessage tmpNetOutgoingMessage = aServer.CreateMessage();
                        tmpNetOutgoingMessage.Write((byte)PacketMessageType.LevelWonChanged);
                        tmpNetOutgoingMessage = aLevelManager.PrepareLevelChangeMessage(tmpNetOutgoingMessage);
                        aServer.SendToAll(tmpNetOutgoingMessage, NetDeliveryMethod.ReliableOrdered);
                        aLevelManager.ChangeToNextLevel();

                    }

                    if (aLevelManager.DefaultPosChanged == true)
                    {
                        NetOutgoingMessage tmpNetOutgoingMessage = aServer.CreateMessage();
                        tmpNetOutgoingMessage.Write((byte)PacketMessageType.DefaultPosChanged);
                        tmpNetOutgoingMessage = aLevelManager.PrepareDefaultPositionUpdate(tmpNetOutgoingMessage);
                        aServer.SendToAll(tmpNetOutgoingMessage, NetDeliveryMethod.ReliableOrdered);
                        aLevelManager.DefaultPosChanged = false;

                    }

                    if (aLevelManager.LevelNeedsReset())
                    {
                        aLevelManager.ResetLevel();
                    }

                }

                tmpTimeToSleep = unchecked((int)(tmpStartTime + aMSPerFrame - aStopWatch.ElapsedMilliseconds));
                if (tmpTimeToSleep < 0) //Poistime si aby cas, ktory ma vlakno spat nebol zaporny
                {
                    tmpTimeToSleep = 0;
                }


                Thread.Sleep(tmpTimeToSleep); //Pokus o implementaciu konstantneho TICKRATU
            }
        }

        public void StandableBlocksHandler()
        {

            if (aStopWatch.IsRunning != true)
            {
                aStopWatch.Start(); //Zapneme casovac
            }

            int tmpTimeToSleep = 0; //Inicializujeme si premennu - reprezentujucu, kolko ma server "spat"

            while (aLogicGame.GameState != GameState.Exit)
            {

                long tmpStartTime = aStopWatch.ElapsedMilliseconds; //Nastavime zaciatocny cas

                if (aStandableBlocksList.Count > 0)
                {
                    if (aDictionaryPlayerData != null && aDictionaryPlayerData.Count > 0)
                    {

                        for (int i = 0; i < aStandableBlocksList.Count; i++)
                        {
                            if (aStandableBlocksList[i] != null)
                            {
                                aStandableBlocksList[i].SomethingIsStandingOnTop = false;
                                aStandableBlocksList.Remove(aStandableBlocksList[i]);
                            }
                        }


                    }
                }

                tmpTimeToSleep = unchecked((int)(tmpStartTime + aMSPerFrame - aStopWatch.ElapsedMilliseconds));

                if (tmpTimeToSleep < 0) //Poistime si aby cas, ktory ma vlakno spat nebol zaporny
                {
                    tmpTimeToSleep = 0;
                }

                Thread.Sleep(tmpTimeToSleep); //Pokus o implementaciu konstantneho TICKRATU

            }


        }


        /// <summary>
        /// Metoda, ktora spravuje koliziu
        /// </summary>
        /// <param name="parGameTime">Parameter Casu Hry - Typ GameTime</param>
        /// <param name="parLevelManager">Parameter Level Managera - Typ LevelManager</param>
        /// <param name="parEntity">Parameter Reprezentuje Entitu - Typ Entity</param>
        /// <param name="parRUID">Parameter, reprezentuje Identifikator Entity - napr RUID Hraca</param>
        public void CollisionHandler(LevelManager parLevelManager, EntityServer parEntity, long parID)
        {

            if (aDictionaryPlayerData != null && aDictionaryPlayerData.Count > 0)
            {

                //Prejdeme vsetky data v Dictionary
                {
                    //Bude reprezentovat, poziciu TILU, kde by sa teda hrac nachadzal - pozor nie bloku!, Tile reprezentuje OBLAST jedneho bloku tzn
                    //Napr. Block so suradnicami 0, 128 - Bude ekvivalentny Tilu so suradnicami 0, 2
                    //Ak mame teda specifikovanu velkost blokov o 64px...

                    int tmpMapBlockDimSize = parLevelManager.GetMapBlocksDimensionSize();

                    float sizeOfPlayerX = parEntity.Size.X * parEntity.EntityScale;
                    float sizeOfPLayerY = parEntity.Size.Y * parEntity.EntityScale;

                    float tmpPositionOffsetY = 0;

                    if (sizeOfPLayerY > 64)
                    {
                        tmpPositionOffsetY = sizeOfPLayerY - (64 - 2); //Preto - 2, lebo o 2 Pixely zarazime ako keby Entitu do Zeme, kvoli vzhladu
                        sizeOfPLayerY = (64 - 4);
                    } //Pokial by bola vyska Entity vyssia ako velkost bloku, zbytok jeho tela, resp hlava nebude kolizna, bude vytrcat nad blokom...

                    int tmpTilePositionX = (int)Math.Floor(parEntity.GetAfterMoveVector2(aTickRate).X / tmpMapBlockDimSize); //Zaciatocna X-ova Tile Suradnica - Vlavo
                    int tmpTilePositionY = (int)Math.Floor((parEntity.GetAfterMoveVector2(aTickRate).Y + tmpPositionOffsetY) / tmpMapBlockDimSize); //Zaciatocna Y-ova Tile Suradnica - Hore - Posunuta o Velkost Entity, ak je vyssia ako 64 pixelov 

                    int tmpEndTilePositionX = (int)Math.Floor((parEntity.GetAfterMoveVector2(aTickRate).X + sizeOfPlayerX) / tmpMapBlockDimSize); //Koncova X-ova Tile Suradnica - Vpravo
                    int tmpEndTilePositionY = (int)Math.Floor((parEntity.GetAfterMoveVector2(aTickRate).Y + sizeOfPLayerY + tmpPositionOffsetY) / tmpMapBlockDimSize); //Koncova Y-ova Tile Suraadnica - Dole

                    //Debug.WriteLine(tmpTilePositionY + " " + tmpEndTilePositionY);

                    // Debug.WriteLine(parEntity.GetAfterMoveVector2(aTickRate).X);

                    // float tmpNumberOfOccupiedBlocks = tmpWidth * tmpHeight;}}
                    bool tmpIsBlocked = false;
                    bool tmpIsSlowed = false;
                    bool tmpButtonActivation = false;
                    bool tmpEntityIsStandingOn = false;
                    bool tmpIsZapped = false;
                    bool tmpEntityInteracted = false;

                    for (int i = tmpTilePositionX; i <= tmpEndTilePositionX; i++) //For Cyklus pre X-ovu Suradnicu, kde by v buducnosti stala Entita
                    {
                        for (int j = tmpTilePositionY; j <= tmpEndTilePositionY; j++) //FOr Cyklus pre Y-ovu Suradnicu, kde by v buducnosti stala Entita 
                        {

                            Vector2 tmpTilePositVector2 = new Vector2(i * tmpMapBlockDimSize, j * tmpMapBlockDimSize);
                            if (parLevelManager.GetBlockByPosition(tmpTilePositVector2) != null) //Ak na takejto suradnici vobec nejaky blok existuje
                            {

                                if (parEntity.WantsToInteract)
                                {
                                    if (parLevelManager.GetBlockByPosition(tmpTilePositVector2).IsInteractible) //Najprv si porovname, ci je mozne interagovat s danym blokom
                                    {
                                        parLevelManager.GetBlockByPosition(tmpTilePositVector2).Interact();

                                        tmpEntityInteracted = true;
                                    }

                                }

                                //Spravca Kolizie

                                switch (parLevelManager.GetBlockByPosition(tmpTilePositVector2).BlockCollisionType)
                                {
                                    case BlockCollisionType.None:
                                        break;
                                    case BlockCollisionType.Wall: //Prekazka typu Stena

                                        if (tmpIsBlocked == false
                                        ) //Preto je tu tato podmienka, aby sme zabranili tomu, ze ak sa uz Entita detegovala jednu koliziu, neprepise ju..
                                        {
                                            tmpIsBlocked = true;

                                        }

                                        break;
                                    case BlockCollisionType.Slow: //Prekazka napr Voda
                                        if (tmpIsSlowed == false
                                        ) //Preto je tu tato podmienka, aby sme zabranili tomu, ze ak sa uz Entita detegovala jednu napr. vodu, neprepise ju..
                                        {
                                            tmpIsSlowed = true;
                                        }

                                        break;
                                    case BlockCollisionType.Zap:
                                        if (tmpIsZapped == false)
                                        {
                                            tmpIsZapped = true;
                                        }

                                        break;
                                    case BlockCollisionType.Button:
                                        tmpButtonActivation = true;
                                        aStandableBlocksList.Add(parLevelManager.GetBlockByPosition(tmpTilePositVector2));
                                        parLevelManager.GetBlockByPosition(tmpTilePositVector2).SomethingIsStandingOnTop = true;

                                        break;
                                    case BlockCollisionType.Standable:
                                        tmpEntityIsStandingOn = true;
                                        aStandableBlocksList.Add(parLevelManager.GetBlockByPosition(tmpTilePositVector2));
                                        parLevelManager.GetBlockByPosition(tmpTilePositVector2).SomethingIsStandingOnTop = true;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                //Spravca Interakcie
                            }
                        }
                    }

                    parEntity.IsBlocked = tmpIsBlocked;

                    if (tmpEntityInteracted)
                    {
                        parEntity.WantsToInteract = false;

                    }

                    if (!tmpEntityIsStandingOn) //Ak Entita nestoji na ziadnom podpornom bloku
                    {
                        parEntity.SlowDown(tmpIsSlowed); //Nastavi ci ma Entita spomalit alebo nie
                                                         //  parEntity.ReSpawn(tmpIsZapped);
                        parEntity.ReSpawn(tmpIsZapped);

                        if (tmpIsZapped)
                        {
                            SendMovementInfo(parID); //Doslo k dost zavaznym zmenam, preto odosleme klientom informacie

                        }
                    }

                    parEntity.IsBlocked = tmpIsBlocked;

                }
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
                            SendMovementInfo(tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier);



                        }
                        else if (tmpReceivedByte == (byte)PacketMessageType.LevelData)
                        {

                            Debug.WriteLine("Prisla level Sprava od " + tmpIncommingMessage.SenderConnection.RemoteUniqueIdentifier);
                            /*   public void SendLevelManagerData(LevelManager parLevelManager)
                               {
                                   NetOutgoingMessage tmpOutgoingMessage = aClient.CreateMessage();
                                   tmpOutgoingMessage.Write((byte)PacketMessageType.LevelData);

                                   if (parLevelManager.LevelUpdateIsReady)
                                   {
                                       parLevelManager.PrepareLevelDataToSend(tmpOutgoingMessage);
                                   }

                                   aClient.SendMessage(tmpOutgoingMessage, NetDeliveryMethod.ReliableOrdered);
                               }
                            */


                        }
                        else if (tmpReceivedByte == (byte)PacketMessageType.RequestLevelInitData)
                        {

                            NetOutgoingMessage tmpOutMovMessage = aServer.CreateMessage();
                            tmpOutMovMessage.Write((byte)PacketMessageType.RequestLevelInitData);

                            //true, lebo pojde o inicializacne data
                            tmpOutMovMessage = SendDataAboutLevel(true, tmpOutMovMessage);
                            aServer.SendMessage(tmpOutMovMessage, tmpIncommingMessage.SenderConnection, NetDeliveryMethod.ReliableOrdered);
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

                int tmpNewPlayerId = aDictionaryPlayerData.Count + 1; //Zainicializujeme si nove ID Hraca

                Vector2 tmpDefaultPlayerPosition = new Vector2(800, 800);

                if (this.aLevelManager.PlayerDefaultPositions[tmpNewPlayerId] != null)
                {
                    tmpDefaultPlayerPosition = this.aLevelManager.PlayerDefaultPositions[tmpNewPlayerId];
                }

                PlayerServerData tmpNewPlayer = new ServerSide.PlayerServerData(tmpNewPlayerId, parPlayerNickname, parRemoteUniqueIdentifier, tmpDefaultPlayerPosition, new ServerSide.Vector2(40, 64));


                aDictionaryPlayerData.Add(parRemoteUniqueIdentifier, tmpNewPlayer);  //Do Dictionary si pridame noveho Hraca s Novym ID a vytvorime objekt typu PlayerServerData podobne spolu s ID a Prezyvkou Hraca
                aLevelManager.DictionaryPlayerDataWithKeyId.Add(tmpNewPlayerId, tmpNewPlayer);

                Debug.WriteLine("Server - Hrac bol pridany!" + " Nickname: " + parPlayerNickname + " ID " + tmpNewPlayerId + " RID " + parRemoteUniqueIdentifier);

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
        /// Metoda, ktora odosiela Klientom data o Leveli
        /// </summary>
        /// <param name="parInitData">Parameter, pomocou, ktoreho sa da specifikovat, ci ide o Inicializacne data alebo nie</param>
        /// <param name="parOutgoingMessage">Parameter - Odchadzajuca sprava - typ NetOurgoingMessage</param>
        public NetOutgoingMessage SendDataAboutLevel(bool parInitData, NetOutgoingMessage parOutgoingMessage)
        {

            if (parInitData)
            {
                aLevelManager.PrepareLevelInitDataForUpload(parOutgoingMessage);
            }
            else
            {
                switch (aLevelManager.LevelName)
                {
                    case "Math":
                        break;
                    default:
                        break;
                }
            }

            return parOutgoingMessage;
        }

        /// <summary>
        /// Metoda, ktora sa stara o odoslanie dat o pohybe/interakcii hraca
        /// </summary>
        /// <param name="parRemoteUniqueIdentifier">Parameter - typu long - Informacia pre klienta od koho su vlastne informacie o Pohybe/Interakcii</param>
        public void SendMovementInfo(long parRemoteUniqueIdentifier)
        {
            NetOutgoingMessage tmpOutMovMessage = aServer.CreateMessage();
            tmpOutMovMessage.Write((byte)PacketMessageType.Movement);
            tmpOutMovMessage.WriteVariableInt64(parRemoteUniqueIdentifier);
            tmpOutMovMessage = aDictionaryPlayerData[parRemoteUniqueIdentifier].PrepareDataForUpload(tmpOutMovMessage);
            aServer.SendToAll(tmpOutMovMessage, NetDeliveryMethod.ReliableOrdered);
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

            if (aDictionaryPlayerData.Count > 0) //Ak je niekto pripojeny na server
            {
                foreach (KeyValuePair<long, ServerSide.PlayerServerData> dictItem in aDictionaryPlayerData.ToList() //ToList -> Nakopiruje cely Dictionary do Listu... 
                ) //Prejdeme vsetky data v Dictionary
                {
                    if (dictItem.Key == tmpRemoteUniqueIdentifier)
                    {
                        aLevelManager.DictionaryPlayerDataWithKeyId.Remove(dictItem.Value.PlayerID);
                    }
                }

            }



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

            aStandableBlocksHandlerThread.Join();

            Debug.WriteLine("Shutting Down Server");


        }

    }
}
