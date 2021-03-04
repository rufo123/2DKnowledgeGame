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

        /// <summary>
        /// Hodnota boolean, ci ide o Data o mne ako klientovi
        /// </summary>
        private bool aIsMe;

        public PlayerClientData(int parPlayerID, string parPlayerNickName, long parRemoteUniqueIdentifier, bool parIsMe = false)
        {
            aPlayerID = parPlayerID; //Priradime ID Hraca
            aPlayerNickName = parPlayerNickName; //Priradime Prezyvku Hraca
            aRemoteUniqueIdentifier = parRemoteUniqueIdentifier; //Priradime Unikatny Identifikator
            aIsMe = parIsMe; //Atribut ci sa jedna o mna - Klienta
        }
        
        /// <summary>
        /// Nastavi, ze sa jedna o "moje" data
        /// </summary>
        /// <returns></returns>
  
        public long RemoteUniqueIdentifier { get => aRemoteUniqueIdentifier; }
        public int PlayerID { get => aPlayerID; }
        public string PlayerNickName { get => aPlayerNickName; set => aPlayerNickName = value; }
    }
}
