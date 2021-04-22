using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MySql.Data.MySqlClient;

namespace _2DLogicGame.ServerSide.Database
{
    /// <summary>
    /// Trieda, ktora reprezentuje pripojenie k databaze.
    /// </summary>
    public class DatabaseConnection
    {
        /// <summary>
        /// Atribut, reprezentujuci adresu servera - typ string.
        /// </summary>
        private string aServer;

        /// <summary>
        /// Atribut, reprezentujuci meno databazy - typ string.
        /// </summary>
        private string aDatabaseName;

        /// <summary>
        /// Atribut, reprezentujuci pouzivatelske meno do databazy - typ string.
        /// </summary>
        private string aUserName;

        /// <summary>
        /// Atribut, reprezentujuci heslo do databazy - typ string.
        /// </summary>
        private string aPassword;

        /// <summary>
        /// Atribut, reprezentujuci "Connection String", pre pripojenie sa do databazy - typ string.
        /// </summary>
        private string aDBConnectionString;

        /// <summary>
        /// Atribut, reprezentujuci objekt - typu MySqlConnection. Pre pripojenie sa k MySQL databaze.
        /// </summary>
        private MySqlConnection aMySqlConnection;


        public MySqlConnection MySqlConnection
        {
            get => aMySqlConnection;
            set => aMySqlConnection = value;
        }

        /// <summary>
        /// Konstruktor - vytvorenie spojenia s databazou.
        /// </summary>
        public DatabaseConnection()
        {
            aServer = "62.77.159.52";
            aDatabaseName = "KnowledgeGameDB";
            aUserName = "knowledge_admin";
            aPassword = "GoBL1n0Kn0Wl@dG3++";

            aDBConnectionString = "server=" + aServer + ";" + "user=" + aUserName + ";" + "database=" + aDatabaseName +
                                  ";" + "port=3306" + ";" + "password=" + aPassword;

            aMySqlConnection = new MySqlConnection(aDBConnectionString);
        }

        /// <summary>
        /// Metoda, ktora sa pokusi pripojit k databaze, ak sa pripojenie nezdari, vrati false
        /// </summary>
        /// <returns>Vrati hodnotu true/false, na zaklade toho ci sa pripojenie k databaze podarilo.</returns>
        public bool Connect()
        {
            if (aMySqlConnection != null)
            {
                aMySqlConnection.Open();

                return aMySqlConnection.Ping();
            }
            else
            {
                return false;
            }
        }
        
   

    }
}
