// ReSharper disable All
using System;
using System.Collections.Generic;
using XMLData;

namespace LevelCreator
{
    /// <summary>
    /// Trieda, ktora je hlavna trieda a vytvara jednotlive instancie vytvaracov.
    /// </summary>
    class CreatorMain
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Co si zelate vytvorit?");
            Console.WriteLine("Level - L | Otazky - Q | Slovnik - V");

            string tmpChooseChar = Console.ReadLine();

            while (!tmpChooseChar.Equals("L") && !tmpChooseChar.Equals("Q") && !tmpChooseChar.Equals("V"))
            {
                tmpChooseChar = Console.ReadLine();
            }

            if (tmpChooseChar == "L")
            {
                CreatorLevel tmpCreatorLevel = new CreatorLevel();
            }
            else if (tmpChooseChar == "V")
            {
                CreatorVocabulary tmpCreatorVocabulary = new CreatorVocabulary();
            }
            else
            {
                CreatorQuestions tmpCreatorQuestions = new CreatorQuestions();
            }


        }
    }
}
