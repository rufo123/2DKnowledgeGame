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
        public MathEquation(int parFirstNumber, int parSecondNumber)
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
            int tmpRandomNumber = tmpRand.Next(0, 3);

            switch (tmpRandomNumber)
            {
                case 0:
                    aOperator = (char)MathOperation.Addition;
                    break;
                case 1:
                    aOperator = (char)MathOperation.Subtraction;
                    break;
                case 2:
                    aOperator = (char)MathOperation.Multiplication;
                    break;
                case 3:
                    aOperator = (char)MathOperation.Division;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
                MathOperation.Division => aFirstNumber / aSecondNumber,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
