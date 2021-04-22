using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide
{
    /// <summary>
    /// Trieda, reprezentujuca data o postave hraca. - Server.
    /// </summary>
    public class PlayerServerData : EntityServer

    {
        /// <summary>
        /// Reprezentuje Ciselne ID Hraca - Integer
        /// </summary>
        private int aPlayerID;

        /// <summary>
        /// Reprezentuje Prezyvku - NickName Hraca - String
        /// </summary>
        private string aPlayerNickName = "Player";


        /// <summary>
        /// Unikatny Identifikator, ktory uzko spolupracuje s kniznicou Lidgren - Networking
        /// </summary>
        private long aRemoteUniqueIdentifier;

        public long RemoteUniqueIdentifier { get => aRemoteUniqueIdentifier; }
        public int PlayerID { get => aPlayerID; }
        public string PlayerNickName { get => aPlayerNickName; }


        /// <summary>
        /// Konstruktor - Inicializuje zakladne atributy
        /// </summary>
        /// <param name="parPlayerId">Parameter - ID Hraca - Typ int</param>
        /// <param name="parPlayerNickName">Parameter - Prezyvka Hraca - Typ string</param>
        /// <param name="parRemoteUniqueIdentifier">Parameter - Remote Unique Identifier Hraca - Typ long</param>
        public PlayerServerData(int parPlayerId, string parPlayerNickName, long parRemoteUniqueIdentifier, Vector2 parPosition, Vector2 parSize) : base(parPosition, parSize, parSpeed: 200F)
        {
            aPlayerID = parPlayerId; //Priradime ID Hraca
            aPlayerNickName = parPlayerNickName; //Priradime Prezyvku Hraca
            aRemoteUniqueIdentifier = parRemoteUniqueIdentifier; //Priradime Unikatny Identifikator

            EntityScale = 1.5F;

        }

        /// <summary>
        /// Metoda, ktora pripravi indetifikacne data hracov na odoslanie, ignorujuc data o hracovi, ktore tieto data pozaduje
        /// </summary>
        /// <param name="parMessage">Parameter reprezentujuci prichodziu spravu - Typ NetOutgoingMessage</param>
        /// <param name="parRUIDToIgnore">Parameter reprezentujuci RUID, ktore ma byt ignorovane - Typ long</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareIdentificationData(NetOutgoingMessage parMessage, long parRUIDToIgnore = 0)
        {

            if (aRemoteUniqueIdentifier != parRUIDToIgnore)
            {

                parMessage.WriteVariableInt32(aPlayerID);
                parMessage.Write(aPlayerNickName);
                parMessage.WriteVariableInt64(aRemoteUniqueIdentifier);

            }
            return parMessage;
        }

    }
}
