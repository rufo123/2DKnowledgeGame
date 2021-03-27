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

        private int aNumberOrders;

        private int aFinalNumber;

        /// <summary>
        /// Atribut, typu bool, ktory je pre Manazera klucovy a oznamuje mu, ze uz su udaje o leveli nacitane a moze robit svoju robotu
        /// </summary>
        private bool aCompletelyLoaded = false;

        private int aMathPoints = 0;

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

        public void AddPoints()
        {
            aMathPoints++;
        }

        public void SetPoints(int parCountOfPoints)
        {
            aMathPoints = parCountOfPoints;
        }

        public void AddInput(InputBlock parInputBlock)
        {
            aDictionaryInputBlocks.Add(aNumberOrders, parInputBlock);

            if (aNumberOrders > 0)
            {
                aNumberOrders /=
                    10; //Vzdycky ideme o jedno menej, na zaciatku napr. stovky potom desiatky, potom jednotky a koniec napr.
            }
        }

        public int GetFinalNumberFromInput()
        {
            if (aDictionaryInputBlocks != null)
            {
                int tmpReturnNumber = 0;
                int tmpCounterOfOrders = (int)Math.Pow(10, aDictionaryInputBlocks.Count - 1);

                while (tmpCounterOfOrders > aNumberOrders
                ) //Pokial sa nedostaneme na koniec jednotiek, desiatok, stoviek....
                {
                    tmpReturnNumber += aDictionaryInputBlocks[tmpCounterOfOrders].Number * tmpCounterOfOrders; //Pripocitame cislo, ktore je uchovane v InputBloku

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
                            aMathProblem.ChangeEquation(aEquations[i + 1]);
                            tmpIsAnyButtonTurnedOn = true;
                            aMathProblem.Shown = true;
                        }

                        if (aButtonList[i].IsPressed == true)
                        {
                            if (GetFinalNumberFromInput() == aEquations[i + 1].GetTotalRoundedToInteger())
                            {
                                for (int j = 0; j < aDictionaryOfBridgeSubBlocks[i + 1].Count; j++)
                                {
                                    aDictionaryOfBridgeSubBlocks[i + 1][j].Show(); //Nezabudnut pridat podmienky na kontrolu
                                    
                                }

                                aButtonList[i].ChangeToSuccessState();
                                tmpIsAnyButtonTurnedOn = false;
                            }
                            else
                            {
                                ResetInputNumbers();
                            }

                            aButtonList[i].IsPressed = false;
                            
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
