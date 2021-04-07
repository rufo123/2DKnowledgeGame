using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MySql.Data.MySqlClient;

namespace _2DLogicGame.ServerSide.Database
{
    class DatabaseConnection
    {

        private string aServer;

        private string aDatabaseName;

        private string aUserName;

        private string aPassword;

        private string aDBConnectionString;

        private MySqlConnection aMySqlConnection;

        public MySqlConnection MySqlConnection
        {
            get => aMySqlConnection;
            set => aMySqlConnection = value;
        }

        /// <summary>
        /// Vytvorenie spojenia s databazou
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

        /// <summary>
        /// Metoda, ktora skontroluje ci je pripojenie k databaze uspesne.
        /// </summary>
        /// <returns>Vrati hodnotu true/false na zaklade kontroly pripojenia k databaze.</returns>
        public bool IsConnected()
        {
            if (aMySqlConnection != null)
            {
                return aMySqlConnection.Ping();
            }
            else
            {
                return false;
            }
        }

    }
}
