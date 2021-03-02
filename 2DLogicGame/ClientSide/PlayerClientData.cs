using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ClientSide
{
  


    class PlayerClientData
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

        public PlayerClientData(int parPlayerID, string parPlayerNickName, long parRemoteUniqueIdentifier)
        {
            aPlayerID = parPlayerID; //Priradime ID Hraca
            aPlayerNickName = parPlayerNickName; //Priradime Prezyvku Hraca
            aRemoteUniqueIdentifier = parRemoteUniqueIdentifier; //Priradime Unikatny Identifikator
        }
        public long RemoteUniqueIdentifier { get => aRemoteUniqueIdentifier; }
        public int PlayerID { get => aPlayerID; }
    }
}
