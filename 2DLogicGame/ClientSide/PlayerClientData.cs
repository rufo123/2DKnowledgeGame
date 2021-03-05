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

        /// <summary>
        /// Konstruktor - Inicializuje zakladne atributy
        /// </summary>
        /// <param name="parPlayerID">Parameter reprezentujuci ID Hraca - Typ int</param>
        /// <param name="parPlayerNickName">Parameter reprezentujuci Prezyvku Hraca - Typ string</param>
        /// <param name="parRemoteUniqueIdentifier">Parameter reprezentujuci Remote Unique Identifier Hraca - Typ long</param>
        /// <param name="parIsMe">Parameter reprezentujuci ci ide o mna, teda toho hraca, ktory hra na tomto pocitaci... - Typ boolean</param>
        public PlayerClientData(int parPlayerID, string parPlayerNickName, long parRemoteUniqueIdentifier, bool parIsMe = false)
        {
            aPlayerID = parPlayerID; //Priradime ID Hraca
            aPlayerNickName = parPlayerNickName; //Priradime Prezyvku Hraca
            aRemoteUniqueIdentifier = parRemoteUniqueIdentifier; //Priradime Unikatny Identifikator
            aIsMe = parIsMe; //Atribut ci sa jedna o mna - Klienta
        }
       
        public long RemoteUniqueIdentifier { get => aRemoteUniqueIdentifier; }
        public int PlayerID { get => aPlayerID; }
        public string PlayerNickName { get => aPlayerNickName; set => aPlayerNickName = value; }
    }
}
