// ReSharper disable All
using System;
using System.Collections.Generic;
using XMLData;

namespace LevelCreator
{
    class CreatorMain
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Co si zelate vytvorit?");
            Console.WriteLine("Level - L | Otazky - Q");

            string tmpChooseChar = Console.ReadLine();

            while (!tmpChooseChar.Equals("L") && !tmpChooseChar.Equals("Q"))
            {
                tmpChooseChar = Console.ReadLine();
            }

            if (tmpChooseChar == "L")
            {
                CreatorLevel tmpCreatorLevel = new CreatorLevel();
            }
            else
            {
                CreatorQuestions tmpCreatorQuestions = new CreatorQuestions();
            }


        }
    }
}
