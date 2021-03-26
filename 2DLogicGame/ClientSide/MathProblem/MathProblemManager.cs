using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.ClientSide.MathProblem;
using Microsoft.Xna.Framework;

namespace _2DLogicGame.GraphicObjects
{
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
        /// Atribut, typu bool, ktory je pre Manazera klucovy a oznamuje mu, ze uz su udaje o leveli nacitane a moze robit svoju robotu
        /// </summary>
        private bool aCompletelyLoaded = false;

        public bool CompletelyLoaded
        {
            get => aCompletelyLoaded;
            set => aCompletelyLoaded = value;
        }


        public MathProblemManager(LogicGame parLogicGame) : base(parLogicGame)
        {
            aCompletelyLoaded = false;
            aEquations = new Dictionary<int, MathEquation>();
            aButtonList = new List<ButtonBlock>();
            aRandom = new Random();
            aDictionaryOfBridgeSubBlocks = new Dictionary<int, List<BridgeBlock>>();
            aMathProblem = new MathProblem(parLogicGame, new Microsoft.Xna.Framework.Vector2(100, 200), new Microsoft.Xna.Framework.Vector2(855, 300), 5, 64);
            parLogicGame.Components.Add(aMathProblem);
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
            aEquations.Add(aButtonCount, new MathEquation(tmpMinNumber, tmpMaxNumber));

        }

        /// <summary>
        /// Metoda, ktora prida BridgeBlock do dictionary ale tak ze BridgeBlock este pred tym zabali do listu, spolu s identifikatorom BridgeBlocku
        /// Pokial este List nebol vytvoreny vytvori ho a priradi do neho patriaci BridgeBlock
        /// </summary>
        /// <param name="parBridge"></param>
        /// <param name="parBridgePartNumber"></param>
        public void AddBridge(BridgeBlock parBridge, int parBridgePartNumber)
        {
            if (aDictionaryOfBridgeSubBlocks.ContainsKey(parBridgePartNumber)) //Ak sa dany kluc nachadza v Dictionary, len pridame cast mostu
            {
                aDictionaryOfBridgeSubBlocks[parBridgePartNumber].Add(parBridge);
            }
            else
            {
                aDictionaryOfBridgeSubBlocks.Add(parBridgePartNumber, new List<BridgeBlock>()); //Pokial sa tam dany kluc nenachadza, vytvorime novy List
                aDictionaryOfBridgeSubBlocks[parBridgePartNumber].Add(parBridge); //A nasledne do listu pridame danu cast mostu
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

        public override void Initialize()
        {
            base.Initialize();
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
                if (aButtonList != null && aButtonList.Count > 0 && aDictionaryOfBridgeSubBlocks != null && aDictionaryOfBridgeSubBlocks.Count > 0)
                {//Pokial Existuje List tlacidielk, obsahuje nejake tlacitka a podobne aj s Dictionary

                    for (int i = 0; i < aButtonList.Count; i++)
                    {
                        if (aButtonList[i].IsTurnedOn == true)
                        {
                            aMathProblem.ChangeEquation(aEquations[i+1]);
                            tmpIsAnyButtonTurnedOn = true;
                            aMathProblem.Shown = true;
                            for (int j = 0; j < aDictionaryOfBridgeSubBlocks[i + 1].Count; j++)
                            {
                                aDictionaryOfBridgeSubBlocks[i + 1][j].Show(); //Nezabudnut pridat podmienky na kontrolu
                            }


                        }
                    }
                }
            }

            if (tmpIsAnyButtonTurnedOn == false)
            {
                aMathProblem.Shown = false;
            }

            base.Update(gameTime);
        }
    }
}
