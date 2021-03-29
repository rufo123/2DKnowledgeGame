using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using _2DLogicGame.ClientSide.MathProblem;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using _2DLogicGame.ServerSide.LevelMath_Server;

namespace _2DLogicGame.ServerSide.LevelMath_Server
{
    public enum Feedback
    {
        NotSubmitted = 0,
        SubmitSucceeded = 1,
        SubmitFailed = 2,
        AllSolved = 3
    }

    public class MathProblemServerManager
    {
        /// <summary>
        /// Dictionary, ktora obsahuje udaje o matematickych rovniciach - Key: Int - Identifikator Rovnice, Value - MathEquation
        /// </summary>
        private Dictionary<int, MathEquationServer> aEquations;

        /// <summary>
        /// List, ktory obsahuje udaje o ButtonBlokoch
        /// </summary>
        private List<ButtonBlockServer> aButtonList;

        /// <summary>
        /// Dictionay, ktora obsahuje List BridgeBlokov identifikovanych identifikacnym cislom - Key: int - Identifikator Bloku Mostu, Value: - List<BridgeBlock>
        /// </summary>
        private Dictionary<int, List<BridgeBlockServer>> aDictionaryOfBridgeSubBlocks;

        /// <summary>
        /// Dictionary, ktora obsahuje informacie o InputBlokoch, identifikovanych ci ide o jednotky, desiatky, alebo stovky.. -> Key, Value je teda InputBlock
        /// </summary>
        private Dictionary<int, InputBlockServer> aDictionaryInputBlocks;

        /// <summary>
        /// List, ktory obsahuje EndBlocky - bude sluzit na detekciu, ci na danych blokoch stoja hraci, ak ano, mozu postupit do dalsieho levelu
        /// </summary>
        private List<EndBlockServer> aEndBlockList;

        /// <summary>
        /// Random generator
        /// </summary>
        private Random aRandom;

        /// <summary>
        /// Reprezentuje, predosle cislo, ktore sa nachadzalo ako sucet input blokov...
        /// </summary>
        private int aOldSumNumber;

        /// <summary>
        /// Kolko tlacitok aktivujicich Matematicky Problem sa nachadza v leveli
        /// </summary>
        private int aButtonCount = 0;

        private int aNumberOrders;

        private int aFinalNumber;

        private bool aUpdateIsReady;

        private int aIdOfLastButtonSucceeded;

        private int aCountOfSucceededButtons;
        
        private Feedback aProblemFeedback;

        public bool UpdateIsReady
        {
            get => aUpdateIsReady;
            set => aUpdateIsReady = value;
        }

        private int aIdOfPressedButton;

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
        public Dictionary<int, MathEquationServer> Equations
        {
            get => aEquations;
            set => aEquations = value;
        }

        public Feedback ProblemFeedback
        {
            get => aProblemFeedback;
            set => aProblemFeedback = value;
        }

                public int IdOfLastButtonSucceeded
        {
            get => aIdOfLastButtonSucceeded;
            set => aIdOfLastButtonSucceeded = value;
        }

        public MathProblemServerManager()
        {
            aCompletelyLoaded = false;
            aEquations = new Dictionary<int, MathEquationServer>();
            aButtonList = new List<ButtonBlockServer>();
            aRandom = new Random();
            aDictionaryOfBridgeSubBlocks = new Dictionary<int, List<BridgeBlockServer>>();
            aDictionaryInputBlocks = new Dictionary<int, InputBlockServer>();
            aEndBlockList = new List<EndBlockServer>();
            aMathPoints = 0; //Debug zmenit na 0
            aNumberOrders = 100; //Na zaciatku zainicializujeme, ake najvyssie jednotky sa tu nachadzaju, v tomto pripade stovky
            aOldSumNumber = GetFinalNumberFromInput();
            aProblemFeedback = Feedback.NotSubmitted;
            aIdOfLastButtonSucceeded = -1;
            aCountOfSucceededButtons = 0;
        }

        /// <summary>
        /// Metoda, ktora prida tlacidlo do listu tlacidiel a zaroven vytvori asociaciu tlacidla a prislusnej Matematickej Rovnice
        /// Zaroven si metoda pocita, kolko tlacidiel uz asociovala
        /// </summary>
        /// <param name="parButton">Parameter reprezentujuci tlacidlo - typ ButtonBlock</param>
        public void AddButton(ButtonBlockServer parButton)
        {
            aButtonCount++;
            aButtonList.Add(parButton);
            int tmpMinNumber = GenerateNumberForEquation(1, 20);
            int tmpMaxNumber = GenerateNumberForEquation(1, 20);
            aEquations.Add(aButtonCount, new MathEquationServer(tmpMinNumber, tmpMaxNumber));

        }

        /// <summary>
        /// Metoda, ktora prida BridgeBlock do dictionary ale tak ze BridgeBlock este pred tym zabali do listu, spolu s identifikatorom BridgeBlocku
        /// Pokial este List nebol vytvoreny vytvori ho a priradi do neho patriaci BridgeBlock
        /// </summary>
        /// <param name="parBridge"></param>
        /// <param name="parBridgePartNumber"></param>
        public void AddBridge(BridgeBlockServer parBridge, int parBridgePartNumber)
        {
            if (aDictionaryOfBridgeSubBlocks.ContainsKey(parBridgePartNumber)) //Ak sa dany kluc nachadza v Dictionary, len pridame cast mostu
            {
                aDictionaryOfBridgeSubBlocks[parBridgePartNumber].Add(parBridge);
            }
            else
            {
                aDictionaryOfBridgeSubBlocks.Add(parBridgePartNumber, new List<BridgeBlockServer>()); //Pokial sa tam dany kluc nenachadza, vytvorime novy List
                aDictionaryOfBridgeSubBlocks[parBridgePartNumber].Add(parBridge); //A nasledne do listu pridame danu cast mostu
            }
        }

        public void AddEndBlock(EndBlockServer parEndBlockServer)
        {
            if (aEndBlockList != null && aEndBlockList.Count < 2)
            {
                aEndBlockList.Add(parEndBlockServer);
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

        public void AddInput(InputBlockServer parInputBlock)
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

        /// <summary>
        /// Metoda, ktora sa vykona vtedy, ak prisla zo servera informacia o uspesnosti niektoreho prikladu
        /// </summary>
        /// <param name="parButtonID">Parameter reprezentujuci, tlacitko, ktoreho stav sa ma zmenit na Succeeded - typ int</param>
        /// <param name="parShowBridge">Parameter reprezentujuci, ci sa maju zobrazit aj bloky mostu - typ bool</param>
        public void ButtonSucceeded(int parButtonID)
        {
            if (parButtonID >= 0)
            {
                aButtonList[parButtonID].ChangeToSuccessState();

                    for (int j = 0; j < aDictionaryOfBridgeSubBlocks[parButtonID + 1].Count; j++)
                    {
                        aDictionaryOfBridgeSubBlocks[parButtonID + 1][j].Show();
                    }
            }

        }

        /*

        public MathProblemServerClientData GetMathProblemData()
        {
            MathProblemServerClientData tmpNewData = new MathProblemServerClientData();
            tmpNewData.CurrentButtonPressed = aIdOfPressedButton;
            aUpdateIsReady = false; //Update uz je v tomto momente odovzdany
            return tmpNewData;
        }
        */

        /// <summary>
        /// Metoda, ktora sa stara o napr spravu interakcie medzi inputom a vysledkom
        /// </summary>
        public void Update()
        {

            //if (aCompletelyLoaded) //Ak nam Level Manager oznami, ze je Level kompletne nacitany
            //{

            for (int i = 0; i < aButtonList.Count; i++)
            {

                if (aButtonList[i].WantsToInteract == true)
                {
                    if (GetFinalNumberFromInput() == aEquations[i + 1].GetTotalRoundedToInteger()) //Ak je vysledok spravny
                    {
                        Debug.WriteLine("Vysledok spravny!");
                        aButtonList[i].WantsToInteract = false;
                        aButtonList[i].ChangeToSuccessState();
                        this.ButtonSucceeded(i);
                        aIdOfLastButtonSucceeded = i;
                        aProblemFeedback = Feedback.SubmitSucceeded;
                        aCountOfSucceededButtons++;
                        aUpdateIsReady = true;
                        aMathPoints++; //Pripocitame bod, za spravne vyrieseny priklad
                    }
                    else
                    {
                        ResetInputNumbers();
                        aButtonList[i].WantsToInteract = false;
                        aProblemFeedback = Feedback.SubmitFailed;
                        aUpdateIsReady = true;
                    }
                }

                if (GetFinalNumberFromInput() != aOldSumNumber)
                {
                    aProblemFeedback = Feedback.NotSubmitted;
                    aUpdateIsReady = true;
                    aOldSumNumber = GetFinalNumberFromInput();
                }

            }

            if (aMathPoints >= 5)
            {
                int tmpCountOfStandingPlayers = 0;
                
                for (int j = 0; j < aEndBlockList.Count; j++)
                {
                    if (aEndBlockList[j].SomethingIsStandingOnTop)
                    {
                        tmpCountOfStandingPlayers++;
                    }
                }

                if (tmpCountOfStandingPlayers >= 2) //Debug Len jeden
                {
                    Debug.WriteLine("Server - Vyhra");
                    aProblemFeedback = Feedback.AllSolved;
                    //Odtialto si nasledne Server sam preberie informaciu i vyhre
                }
            }

            //}

        }

    }
}
