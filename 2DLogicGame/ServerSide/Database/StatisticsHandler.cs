using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using MySql.Data.MySqlClient;

namespace _2DLogicGame.ServerSide.Database
{
    class StatisticsHandler
    {
        /// <summary>
        /// Atribut, ktory reprezentuje pripojenie k databaze - typ DatabaseConnection.
        /// </summary>
        private DatabaseConnection aDatabaseConnection;

        /// <summary>
        /// Atribut, ktory reprezentuje prikaz v databaze - typ MySqlCommand
        /// </summary>
        private MySqlCommand aMySqlCommand;

        /// <summary>
        /// Atribut, list - ktory reprezentuje 10 najlepsich hracov vybranych z databazy - typ List<string>
        /// </summary>
        private List<string> aListStats;

        /// <summary>
        /// Atribut, ktoreho hodnota reprezentuje ci pripojenie k databaze prebehlo uspesne, alebo nie - typ bool.
        /// </summary>
        private bool aIsConnected;

        public bool IsConnected
        {
            get => aIsConnected;
            set => aIsConnected = value;
        }

        public StatisticsHandler()
        {
            aDatabaseConnection = new DatabaseConnection();
            aIsConnected = aDatabaseConnection.Connect();
            aListStats = new List<string>(10);

        }

        /// <summary>
        /// Metoda, ktora sa handleru statistiky, pokusi znovu pripojit na server.
        /// </summary>
        /// <returns>Vrati hodnotu true/false, podla toho ci sa pokus o pripojenie podari znova.</returns>
        public bool RetryConnect()
        {
            if (!aIsConnected && aDatabaseConnection != null)
            {
                aIsConnected = aDatabaseConnection.Connect();
            }
            return aIsConnected;
        }

        public void UploadNewScore(string parPlayer1Name, string parPlayer2Name, int parPoints, int parTime)
        {
            if (!aIsConnected) //Najprv sa skusime znovu pripojit k databaze, ak sa prvotne spojenie nepodarilo
            {
                RetryConnect();
            }

            if (aIsConnected)
            {
                try
                {

                    aMySqlCommand = new MySqlCommand();

                    aMySqlCommand.Connection = aDatabaseConnection.MySqlConnection;
                    aMySqlCommand.CommandText = "INSERT INTO knowledge_stats (player1Name, player2Name, points, time) Values(@DBParPlayer1Name, @DBParPlayer2Name, @DBParPoints, @DBParTime)";

                    aMySqlCommand.Parameters.AddWithValue("@DBParPlayer1Name", parPlayer1Name);
                    aMySqlCommand.Parameters.AddWithValue("@DBParPlayer2Name", parPlayer2Name);
                    aMySqlCommand.Parameters.AddWithValue("@DBParPoints", parPoints);
                    aMySqlCommand.Parameters.AddWithValue("@DBParTime", parTime);

                    aMySqlCommand.Prepare();

                    if (aMySqlCommand.ExecuteNonQuery() >= 1)
                    {
                        Debug.WriteLine("Data uploaded successfully.");
                    }
                }

                catch (Exception tmpException)
                {
                    Debug.WriteLine(tmpException);
                    throw;
                }

            }
        }

        public List<string> DownloadScoreboard()
        {
            if (!aIsConnected) //Najprv sa skusime znovu pripojit k databaze, ak sa prvotne spojenie nepodarilo
            {
                RetryConnect();
            }

            if (aIsConnected)
            {
                aListStats.Clear();

                try
                {
                    aMySqlCommand = new MySqlCommand();

                    aMySqlCommand.Connection = aDatabaseConnection.MySqlConnection;
                    //INSERT INTO knowledge_stats (player1Name, player2Name, points, time) Values(@DBParPlayer1Name, @DBParPlayer2Name, @DBParPoints, @DBParTime)
                    aMySqlCommand.CommandText = "SELECT id, player1Name, player2Name, points, time FROM knowledge_stats ORDER BY points ASC, time";

                    aMySqlCommand.Prepare();

                    MySqlDataReader tmpDataReader = aMySqlCommand.ExecuteReader();

                    if (tmpDataReader.HasRows)
                    {
                        string tmpNewString = "";
                        while (tmpDataReader.Read())
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                tmpNewString +=" " + tmpDataReader.GetString(i);
                            }

                            aListStats.Add(tmpNewString);

                            tmpNewString = "";
                        }

                        return aListStats;
                    }

                }

                catch (Exception tmpException)
                {
                    Debug.WriteLine(tmpException);
                    throw;
                }

            }
            return null;
        }
    }
}
