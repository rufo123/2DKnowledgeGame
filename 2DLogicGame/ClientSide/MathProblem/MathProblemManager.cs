﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using _2DLogicGame.ClientSide.MathProblem;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Enumeracna trieda, reprezentujuca spatnu vazbu manazera matematickeho problemu na strane klienta.
    /// </summary>
    public enum Feedback
    {
        NotSubmitted = 0,
        SubmitSucceeded = 1,
        SubmitFailed = 2,
        AllSolved = 3
    }

    /// <summary>
    /// Trieda, reprezentujuca manazer matematickeho problemu na strane klienta.
    /// </summary>
    public class MathProblemManager : GameComponent
    {
        /// <summary>
        /// Dictionary, ktora obsahuje udaje o matematickych rovniciach - Key: Int - Identifikator Rovnice, Value - MathEquation
        /// </summary>
        private Dictionary<int, MathEquation> aEquations;

        /// <summary>
        /// List, ktory obsahuje udaje o ButtonBlokoch
        /// </summary>
        private List<ButtonBlock> aButtonList;

        /// <summary>
        /// Dictionay, ktora obsahuje List BridgeBlokov identifikovanych identifikacnym cislom - Key: int - Identifikator Bloku Mostu, Value: - List<BridgeBlock>
        /// </summary>
        private Dictionary<int, List<BridgeBlock>> aDictionaryOfBridgeSubBlocks;

        /// <summary>
        /// Dictionary, ktora obsahuje informacie o InputBlokoch, identifikovanych ci ide o jednotky, desiatky, alebo stovky.. -> Key, Value je teda InputBlock
        /// </summary>
        private Dictionary<int, InputBlock> aDictionaryInputBlocks;


        /// <summary>
        /// Random generator
        /// </summary>
        private Random aRandom;

        /// <summary>
        /// Matematicky Problem
        /// </summary>
        private MathProblem aMathProblem;

        /// <summary>
        /// Kolko tlacitok aktivujicich Matematicky Problem sa nachadza v leveli
        /// </summary>
        private int aButtonCount = 0;

        /// <summary>
        /// Atribut, reprezentujuci Kluc, vyuzity napr pri Dictionary Input Blokov, kde specifikuje ci ide o jednotky, desiatky a pod.
        /// </summary>
        private int aNumberOrders;

        /// <summary>
        /// Atribut, ktory sluzi na informovanie o tom ci je update pripraveny
        /// </summary>
        private bool aUpdateIsReady;

        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame
        /// </summary>
        private LogicGame aGame;

        /// <summary>
        /// Atribut, typu bool, ktory je pre Manazera klucovy a oznamuje mu, ze uz su udaje o leveli nacitane a moze robit svoju robotu
        /// </summary>
        private bool aCompletelyLoaded = false;

        /// <summary>
        /// Atribut, ktory reprezentuje body dosiahnute za vyriesenie Matematickeho problemu, standardne za kazdy jeden pribude jeden bod
        /// </summary>
        private int aMathPoints = 0;

        /// <summary>
        /// Atribut, ktory reprezentuje FeedBack od Matetickeho problemu, ci bol vysledok odoslany, nie, popr. uspesne vyrieseny, alebo bol nespravne vyrieseny - typ enum - Feedback
        /// </summary>
        private Feedback aProblemFeedback;

        public bool CompletelyLoaded
        {
            get => aCompletelyLoaded;
            set => aCompletelyLoaded = value;
        }

        public Dictionary<int, MathEquation> Equations
        {
            get => aEquations;
            set => aEquations = value;
        }

        public Dictionary<int, InputBlock> DictionaryInputBlocks
        {
            get => aDictionaryInputBlocks;
            set => aDictionaryInputBlocks = value;
        }

        public Feedback ProblemFeedback
        {
            get => aProblemFeedback;
            set => aProblemFeedback = value;
        }

        public bool UpdateIsReady //Pozriet ci naozaj treba
        {
            get => aUpdateIsReady;
            set => aUpdateIsReady = value;
        }

        /// <summary>
        /// Konstruktor matematickeho problemu na strane klienta.
        /// </summary>
        /// <param name="parLogicGame"></param>
        public MathProblemManager(LogicGame parLogicGame) : base(parLogicGame)
        {
            aCompletelyLoaded = false;
            aEquations = new Dictionary<int, MathEquation>();
            aButtonList = new List<ButtonBlock>();
            aRandom = new Random();
            aGame = parLogicGame;
            aDictionaryOfBridgeSubBlocks = new Dictionary<int, List<BridgeBlock>>();
            aMathProblem = new MathProblem(parLogicGame, new Microsoft.Xna.Framework.Vector2(100, 200), new Microsoft.Xna.Framework.Vector2(855, 300), 64);
            aGame.Components.Add(aMathProblem);
            aDictionaryInputBlocks = new Dictionary<int, InputBlock>();
            aMathPoints = 0;
            aNumberOrders = 100; //Na zaciatku zainicializujeme, ake najvyssie jednotky sa tu nachadzaju, v tomto pripade stovky
        }

        /// <summary>
        /// Metoda, ktora prida tlacidlo do listu tlacidiel a zaroven vytvori asociaciu tlacidla a prislusnej Matematickej Rovnice
        /// Zaroven si metoda pocita, kolko tlacidiel uz asociovala
        /// </summary>
        /// <param name="parButton">Parameter reprezentujuci tlacidlo - typ ButtonBlock</param>
        public void AddButton(ButtonBlock parButton)
        {
            aButtonCount++;
            aButtonList.Add(parButton);
            int tmpMinNumber = GenerateNumberForEquation(1, 20);
            int tmpMaxNumber = GenerateNumberForEquation(1, 20);

        }

        /// <summary>
        /// Metoda, ktora prida BridgeBlock do dictionary ale tak ze BridgeBlock este pred tym zabali do listu, spolu s identifikatorom BridgeBlocku
        /// Pokial este List nebol vytvoreny vytvori ho a priradi do neho patriaci BridgeBlock
        /// </summary>
        /// <param name="parBridge"></param>
        /// <param name="parBridgePartNumber"></param>
        public void AddBridge(BridgeBlock parBridge, int parBridgePartNumber)
        {
            if (aDictionaryOfBridgeSubBlocks.ContainsKey(parBridgePartNumber)
            ) //Ak sa dany kluc nachadza v Dictionary, len pridame cast mostu
            {
                aDictionaryOfBridgeSubBlocks[parBridgePartNumber].Add(parBridge);
            }
            else
            {
                aDictionaryOfBridgeSubBlocks.Add(parBridgePartNumber,
                    new List<BridgeBlock>()); //Pokial sa tam dany kluc nenachadza, vytvorime novy List
                aDictionaryOfBridgeSubBlocks[parBridgePartNumber]
                    .Add(parBridge); //A nasledne do listu pridame danu cast mostu
            }

        }

        /// <summary>
        /// Metoda, ktora sluzi na pridavanie bodov za vyrieseny matematicky problem
        /// </summary>
        public void AddPoints()
        {
            aMathPoints++;
        }

        /// <summary>
        /// Metoda, ktora nastavi body na specificku hodnotu
        /// </summary>
        /// <param name="parCountOfPoints">Parameter, reprezentujuci na aku hodnotu bodov sa maju doterajsie body nastavit - typ int</param>
        public void SetPoints(int parCountOfPoints)
        {
            aMathPoints = parCountOfPoints;
        }

        /// <summary>
        /// Metoda, ktora sluzi na pridanie InputBlocku aby MathProblemManager, vedel odkial ma ocakavat input
        /// </summary>
        /// <param name="parInputBlock"></param>
        public void AddInput(InputBlock parInputBlock)
        {
            aDictionaryInputBlocks.Add(aNumberOrders, parInputBlock);  //Key je charakterizovany tym, ze ci ide o jednotky, desiatky, stovky a podobne

            if (aNumberOrders > 0)
            {
                aNumberOrders /=
                    10; //Vzdycky ideme o jedno menej, na zaciatku napr. stovky potom desiatky, potom jednotky a koniec napr.
            }
        }

        /// <summary>
        /// Metoda, ktora sluzi na ziskanie vysledku matematickeho problemu
        /// </summary>
        /// <returns>Vrati vysledok matematickeho problemu - Typ int</returns>
        public int GetFinalNumberFromInput()
        {
            if (aDictionaryInputBlocks != null)
            {
                int tmpReturnNumber = 0;
                int tmpCounterOfOrders = (int)Math.Pow(10, aDictionaryInputBlocks.Count - 1);

                while (tmpCounterOfOrders > aNumberOrders
                ) //Pokial sa nedostaneme na koniec jednotiek, desiatok, stoviek....
                {
                    tmpReturnNumber +=
                        aDictionaryInputBlocks[tmpCounterOfOrders].Number *
                        tmpCounterOfOrders; //Pripocitame cislo, ktore je uchovane v InputBloku

                    if (tmpCounterOfOrders > 0)
                    {
                        tmpCounterOfOrders /= 10;
                    }
                }
                return tmpReturnNumber;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Metoda, ktora nastavi cislo, ktora ma byt zadane v blokoch reprezentujucich input.
        /// </summary>
        /// <param name="parNewNumber">Parameter, reprezentujuci cislo, ktora sa nasledne zada do inputu - typ int.</param>
        /// <returns>Vrati bool hodnotu v zavislosti od uspechu metody.</returns>
        public bool SetNumberToInput(int parNewNumber)
        {
            if (aDictionaryInputBlocks != null)
            {
                int tmpCounterOfOrders = (int)Math.Pow(10, aDictionaryInputBlocks.Count - 1);

                while (tmpCounterOfOrders > aNumberOrders
                ) //Pokial sa nedostaneme na koniec jednotiek, desiatok, stoviek....
                {
                    aDictionaryInputBlocks[tmpCounterOfOrders].Number =
                        (int)(parNewNumber / tmpCounterOfOrders); //Pripocitame cislo, ktore je uchovane v InputBloku

                    if (tmpCounterOfOrders > 0)
                    {
                        parNewNumber %= tmpCounterOfOrders;
                        tmpCounterOfOrders /= 10;

                    }
                }

                return true;
            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// Metoda, ktora na zaklade minima a maxima vygeneruje nahodne cislo z rozsahu
        /// </summary>
        /// <param name="parMin">Parameter - Minimum - typ int</param>
        /// <param name="parMax">Parameter - Maximum - typ int</param>
        /// <returns></returns>
        public int GenerateNumberForEquation(int parMin, int parMax)
        {
            return aRandom.Next(parMin, parMax + 1);
        }

        /// <summary>
        /// Metoda, ktora zmeni, vsetky cisla v inpute na 0 napr. pri zlom vstupe
        /// </summary>
        public void ResetInputNumbers()
        {
            int tmpMaxOrder = (int)Math.Pow(10, aDictionaryInputBlocks.Count - 1);

            int tmpCountOrder = 1;

            while (tmpCountOrder <= tmpMaxOrder)
            {
                aDictionaryInputBlocks[tmpCountOrder].ResetSubmission();
                tmpCountOrder *= 10;
            }

        }


        /// <summary>
        /// Metoda - Update -> Ak je uz level kompletne nacitany, porovnava ci je tlacitko stlacene ak ano, zobrazi k nemu prislusnu Matematicku Rovnicu 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            bool tmpIsAnyButtonTurnedOn = false;

            if (aCompletelyLoaded) //Ak nam Level Manager oznami, ze je Level kompletne nacitany
            {
                if (aButtonList != null && aButtonList.Count > 0 && aDictionaryOfBridgeSubBlocks != null && aDictionaryOfBridgeSubBlocks.Count > 0 && aEquations != null)
                {
                    //Pokial Existuje List tlacidielk, obsahuje nejake tlacitka a podobne aj s Dictionary

                    for (int i = 0; i < aButtonList.Count; i++)
                    {
                        if (aMathProblem != null && aButtonList[i].IsTurnedOn == true)
                        {
                            if (aEquations.Count > 0)
                            {
                                aMathProblem.ChangeEquation(aEquations[i + 1]);
                                tmpIsAnyButtonTurnedOn = true;
                                aMathProblem.Shown = true;
                            }
                        }

                        if (aButtonList[i].IsPressed == true)
                        {
                            //aUpdateIsReady = true;
                        }
                    }
                }
            }
            if (aMathProblem != null && tmpIsAnyButtonTurnedOn == false)
            {
                aMathProblem.Shown = false;
            }
            base.Update(gameTime);
        }


        /// <summary>
        /// Metoda, ktora sa vykona vtedy, ak prisla zo servera informacia o uspesnosti niektoreho prikladu
        /// </summary>
        /// <param name="parButtonID">Parameter reprezentujuci, tlacitko, ktoreho stav sa ma zmenit na Succeeded - typ int</param>
        /// <param name="parShowBridge">Parameter reprezentujuci, ci sa maju zobrazit aj bloky mostu - typ bool</param>
        public void ButtonSucceeded(int parButtonID, bool parShowBridge)
        {
            if (parButtonID >= 0)
            {
                aButtonList[parButtonID].ChangeToSuccessState();

                for (int j = 0; j < aDictionaryOfBridgeSubBlocks[parButtonID + 1].Count; j++)
                {
                    aDictionaryOfBridgeSubBlocks[parButtonID + 1][j].Show(parShowBridge);
                }
            }

        }

        /// <summary>
        /// Metoda, ktora sa stara o upratanie veci zanechanych po MathProblemManageri
        /// </summary>
        public void RemoveRedundantThings()
        {
            if (aMathProblem != null)
            {
                aGame.Components.Remove(aMathProblem);
                aMathProblem = null;
            }
        }


    }
}
