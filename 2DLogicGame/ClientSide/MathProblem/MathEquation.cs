using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ClientSide.MathProblem
{
    /// <summary>
    /// Enum reprezentujuci 4 zakladne matematicke operacie: +, -, *, /
    /// </summary>
    public enum MathOperation
    {
        Addition = '+',
        Subtraction = '-',
        Multiplication = '*',
        Division = '/'
    }

    /// <summary>
    /// Trieda, reprezentujuca jeden matematicky problem, specifikovany prvym cislom, druhym cislom a operatorom
    /// </summary>
    public class MathEquation
    {
        /// <summary>
        /// Atribut reprezentujuci prve cislo - typ int
        /// </summary>
        private int aFirstNumber;

        /// <summary>
        /// Atribut reprezentujuci druhe cislo - typ int
        /// </summary>
        private int aSecondNumber;

        /// <summary>
        /// Atribut reprezentujuci matematicky operator - typ char
        /// </summary>
        private char aOperator;

        public MathEquation(int parFirstNumber, int parSecondNumber, char parOperator)
        {
            aFirstNumber = parFirstNumber;
            aSecondNumber = parSecondNumber;
            aOperator = parOperator;
        }

        public char Operator
        {
            get => aOperator;
            set => aOperator = value;
        }

        public int FirstNumber
        {
            get => aFirstNumber;
            set => aFirstNumber = value;
        }

        public int SecondNumber
        {
            get => aSecondNumber;
            set => aSecondNumber = value;
        }

    }
}
