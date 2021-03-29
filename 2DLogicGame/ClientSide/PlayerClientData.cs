using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using static _2DLogicGame.GraphicObjects.Entity;

namespace _2DLogicGame.ClientSide
{



    public class PlayerClientData : GraphicObjects.Entity
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
        private bool aIsControlledByClient;

        /// <summary>
        /// Hodnota, boolean, ktora reprezentuje, ci je hrac naozaj pripojeny na server, resp server to pripojenie aj potvrdil.
        /// </summary>
        private bool aConnected;



        /// <summary>
        /// Konstruktor - Inicializuje zakladne atributy
        /// </summary>
        /// <param name="parPlayerID">Parameter reprezentujuci ID Hraca - Typ int</param>
        /// <param name="parPlayerNickName">Parameter reprezentujuci Prezyvku Hraca - Typ string</param>
        /// <param name="parRemoteUniqueIdentifier">Parameter reprezentujuci Remote Unique Identifier Hraca - Typ long</param>
        /// <param name="parSpeed">Paramter reprezentujuci rychlost - typ float</param>
        /// <param name="parIsMe">Parameter reprezentujuci ci ide o mna, teda toho hraca, ktory hra na tomto pocitaci... - Typ boolean</param>
        /// <param name="parGame">Parameter reprezentujuci hru - typ LogicGame</param>
        /// <param name="parPosition">Parameter reprezentujuci poziciu - typ Vector2</param>
        /// <param name="parSize">Parametere reprezentujuci velkost - typ Vector2</param>
        /// <param name="parDirection">Parameter reprezentujuci Smer - typ Entity.Direction - enum</param>
        public PlayerClientData(int parPlayerID, string parPlayerNickName, long parRemoteUniqueIdentifier, LogicGame parGame, Vector2 parPosition, Vector2 parSize, Direction parDirection = Direction.UP, float parSpeed = 200, bool parIsMe = false) : base(parGame, parPosition, parSize, parDirection, parSpeed)
        {
            aPlayerID = parPlayerID; //Priradime ID Hraca
            aPlayerNickName = parPlayerNickName; //Priradime Prezyvku Hraca
            aRemoteUniqueIdentifier = parRemoteUniqueIdentifier; //Priradime Unikatny Identifikator
            aIsControlledByClient = parIsMe; //Atribut ci sa jedna o mna - Klienta
            aConnected = false;
            SetImage("Sprites\\Entities\\postavaFrames");

            switch (parPlayerID)
            {
                case 1:
                    Color = Color.White;
                    break;
                case 2:
                    Color = Color.Silver;
                    break;
                default:
                    Color = Color.Black;
                    break;
            }
            EntityScale = 1.5F;

        }

        public long RemoteUniqueIdentifier { get => aRemoteUniqueIdentifier; }
        public int PlayerID { get => aPlayerID; }
        public string PlayerNickName { get => aPlayerNickName; set => aPlayerNickName = value; } 
        public bool Connected { get => aConnected; set => aConnected = value; }

    }
}
