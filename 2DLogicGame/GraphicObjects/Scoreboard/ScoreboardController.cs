﻿using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.ServerSide.Database;

namespace _2DLogicGame.GraphicObjects.Scoreboard
{
    /// <summary>
    /// Trieda ktora reprezentuje kontrolér hodnotiacej tabulky.
    /// </summary>
    public class ScoreboardController
    {

        /// <summary>
        /// Atribut, reprezentujuci spravcu hodnotiacej tabulky.
        /// </summary>
        private StatisticsHandler aStatisticsHandler;

        /// <summary>
        /// Atribut, reprezentujuci graficku cas hodnotiacej tabulky.
        /// </summary>
        private ScoreboardUI aScoreboardUI;

        /// <summary>
        /// Atribut, reprezentujuci stav stiahnutych dat z databazy. Stiahnute - true, inak false.
        /// </summary>
        private bool aDataDownloaded;

        /// <summary>
        /// Konstruktor kontroléra hodnotiacej tabulky.
        /// </summary>
        /// <param name="parStatisticsHandler"></param>
        /// <param name="parScoreboardUI"></param>
        public ScoreboardController(StatisticsHandler parStatisticsHandler, ScoreboardUI parScoreboardUI)
        {
            aStatisticsHandler = parStatisticsHandler;
            aScoreboardUI = parScoreboardUI;
        }


        /// <summary>
        /// Metoda, ktora zainicializuje tabulku
        /// </summary>
        public void InitScoreboard()
        {
            if (aScoreboardUI != null && aStatisticsHandler != null)
            {
                aScoreboardUI.InitScoreboardItems(aStatisticsHandler.DownloadScoreboard());
                aDataDownloaded = true;
            }
        }

        /// <summary>
        /// Metoda, ktora sa pokusi aktualizovat data hodnotiacej tabulky
        /// </summary>
        public void RefreshDownloadedData()
        {
            if (aScoreboardUI != null && aStatisticsHandler != null && aDataDownloaded)
            {
                aScoreboardUI.InitScoreboardItems(aStatisticsHandler.DownloadScoreboard());
                aDataDownloaded = true;
            }
        }

        /// <summary>
        /// Metoda, ktora riadi ci sa ma hodnotiaca tabulka zobrazit alebo nie
        /// </summary>
        /// <param name="parShow">Parameter, reprezentujuci, ci sa ma hodnotiaca tabulka zobrazit alebo nie</param>
        public void ShowStats(bool parShow)
        {
            if (aScoreboardUI != null && aDataDownloaded)
            {
                aScoreboardUI.Show = parShow;
            }

        }

        /// <summary>
        /// Metoda, ktora vrati hodnotu true/false na zaklade toho ci bolo pripojenie k databaze uspesne.
        /// </summary>
        /// <returns>Vrati hodnot true/false - ci je pripojenie k databaze uspesne.</returns>
        public bool IsConnected()
        {
            if (aStatisticsHandler != null)
            {
                return aStatisticsHandler.IsConnected;
            }

            return false;
        }

    }
}
