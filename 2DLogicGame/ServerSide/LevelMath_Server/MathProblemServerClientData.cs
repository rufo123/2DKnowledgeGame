using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ClientSide.MathProblem
{
    public class MathProblemServerClientData
    {

        private int aCurrentButtonPressed;

        private bool aIsEquationCorrect;

        public bool IsEquationCorrect
        {
            get => aIsEquationCorrect;
            set => aIsEquationCorrect = value;
        }

        public int CurrentButtonPressed
        {
            get => aCurrentButtonPressed;
            set => aCurrentButtonPressed = value;
        }

        public MathProblemServerClientData()
    {

    }

}
}
