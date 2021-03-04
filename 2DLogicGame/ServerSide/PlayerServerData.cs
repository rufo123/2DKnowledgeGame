﻿using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide
{
    class PlayerServerData

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

        public PlayerServerData(int parPlayerID, string parPlayerNickName, long parRemoteUniqueIdentifier)
        {
            aPlayerID = parPlayerID; //Priradime ID Hraca
            aPlayerNickName = parPlayerNickName; //Priradime Prezyvku Hraca
            aRemoteUniqueIdentifier = parRemoteUniqueIdentifier; //Priradime Unikatny Identifikator

        }

        public long RemoteUniqueIdentifier { get => aRemoteUniqueIdentifier; }
        public int PlayerID { get => aPlayerID; }
        public string PlayerNickName { get => aPlayerNickName; }

        public NetOutgoingMessage PrepareIdentificationData(NetOutgoingMessage parMessage, long parRUIDToIgnore = 0) {

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
