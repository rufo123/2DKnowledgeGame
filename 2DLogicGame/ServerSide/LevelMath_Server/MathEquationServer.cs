using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ClientSide.MathProblem
{
    /// <summary>
    /// Enum reprezentujuci 4 zakladne matematicke operacie: +, -, *, /
    /// </summary>
    public enum MathOperationServer
    {
        Addition = '+',
        Subtraction = '-',
        Multiplication = '*',
        Division = '/'
    }

    /// <summary>
    /// Trieda, reprezentujuca jeden matematicky problem, specifikovany prvym cislom, druhym cislom a operatorom
    /// </summary>
    public class MathEquationServer
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

        /// <summary>
        /// Konstruktor, reprezentujuci Matematicky Problem
        /// </summary>
        /// <param name="parFirstNumber">Parameter reprezentujuci prve cislo - typ int</param>
        /// <param name="parSecondNumber">Parameter reprezentujuci druhe cislo - typ int</param>
        public MathEquationServer(int parFirstNumber, int parSecondNumber)
        {
            aFirstNumber = parFirstNumber;
            aSecondNumber = parSecondNumber;
            GenerateOperator();
            Debug.WriteLine("Swapped: " + SwapWithCheck());
        }

        /// <summary>
        /// Metoda, ktora nahodne vygeneruje matematicky operator
        /// </summary>
        public void GenerateOperator()
        {
            Random tmpRand = new Random();
            int tmpRandomNumber = tmpRand.Next(0, 100);

            if (tmpRandomNumber < 10) // 10% Sanca na +
            {
                aOperator = (char)MathOperation.Addition;
            }
            else if (tmpRandomNumber < 20) //10% Sanca na -
            {
                aOperator = (char)MathOperation.Subtraction;
            }
            else if (tmpRandomNumber < 50) //30% Sanca na *
            {
                aOperator = (char)MathOperation.Multiplication;
            }
            else //50% Sanca na /
            {
                aOperator = (char)MathOperation.Division;
            }
        }

        /// <summary>
        /// Metoda, ktora nastavi matematicky operator
        /// </summary>
        /// <param name="parOperation">Parameter, ktory reprezentuje Enum typu MathOperation</param>
        public void SetOperator(MathOperation parOperation)
        {
            aOperator = (char)parOperation;
        }

        /// <summary>
        /// Metoda, kde pokial je druhe cislo vacsie ako prve, prehodime ich
        /// </summary>
        /// <returns>Vrati true ak doslo k swapu inak false</returns>
        public bool SwapWithCheck()
        {
            if (aSecondNumber > aFirstNumber)
            {
                int tmpNewSecond = aFirstNumber;
                aFirstNumber = aSecondNumber;
                aSecondNumber = tmpNewSecond;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Metoda, ktora vrati vysledok matematickeho prikladu
        /// </summary>
        /// <returns>Vrati cislo - zaokruhlene na cele cislo(int) - vysledok matematickeho prikladu - typ int</returns>
        public int GetTotalRoundedToInteger() //Pouzity Switch Expression, namiesto statementu -> Navrhlo IDE
        {
            return (MathOperation)aOperator switch
            {
                MathOperation.Addition => aFirstNumber + aSecondNumber,
                MathOperation.Subtraction => aFirstNumber - aSecondNumber,
                MathOperation.Multiplication => aFirstNumber * aSecondNumber,
                MathOperation.Division => (int)Math.Round((double)aFirstNumber / aSecondNumber,0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
