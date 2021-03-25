using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.ClientSide.MathProblem;
using Microsoft.Xna.Framework;

namespace _2DLogicGame.GraphicObjects
{
    public class MathProblemManager : GameComponent
    {
        private Dictionary<int, MathEquation> aEquations;

        private List<ButtonBlock> aButtonList;

        private Dictionary<int, List<BridgeBlock>> aDictionaryOfBridgeSubBlocks;

        private Random aRandom;

        private MathProblem aMathProblem;

        private int aButtonCount = 0;

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

        public void AddButton(ButtonBlock parButton)
        {
            aButtonCount++;
            aButtonList.Add(parButton);
            int tmpMinNumber = GenerateNumberForEquation(1, 20);
            int tmpMaxNumber = GenerateNumberForEquation(1, 20);
            aEquations.Add(aButtonCount, new MathEquation(tmpMinNumber, tmpMaxNumber));

        }

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

        public int GenerateNumberForEquation(int parMin, int parMax)
        {
            return aRandom.Next(parMin, parMax);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

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
