using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace LevelCreator
{
    class Creator
    {
        /// <summary>
        /// Atribut Dictionary - Bude nam sluzit na uchovananie mien blokov...
        /// </summary>
        private Dictionary<char, string> aBlockDictionary;

        private FileStream aFileStream;

        private int aPositionX = 0;
        private int aPositionY;

        public Creator()
        {

            aBlockDictionary = new Dictionary<char, string>();

            XMLData.LevelMaker tmpNewLevel = new XMLData.LevelMaker();

            Console.WriteLine("Zadaj nazov levelu: ");

            tmpNewLevel.LevelName = Console.ReadLine();

            Console.WriteLine("Zadaj cestu a nazov suboru pre level " + tmpNewLevel.LevelName + ". Pozn - Musi sa nachadzat v zlozke s Projektom...");

            string tmpFilePath;

            //tmpFileName = Console.ReadLine();

            tmpFilePath = AppContext.BaseDirectory + Console.ReadLine(); //Nezabudnut nastavit - Tuknut na TXT - Copy to OutPut Directory na Always...

            while (!File.Exists(tmpFilePath)) //Ak takyto subor neexistuje
            {
                Console.WriteLine("Takyto subor neexistuje, zadaj platnu cestu!");
                tmpFilePath = Console.ReadLine(); //Vypytame si ho znova
            }

            Console.WriteLine("Subor " + tmpFilePath + " uspesne nacitany!");


            using (StreamReader tmpFileStream = new StreamReader(tmpFilePath))
            {
                while (tmpFileStream.Peek() > 0) //Ak sa v subore nachadza dalsi character
                {
                    char tmpReadChar = (char) tmpFileStream.Read();

                    if (tmpReadChar == '\n' || tmpReadChar == '\r')
                    {
                        if (tmpReadChar == '\r') //Ak je znak Carriage Return posunieme Y-ovu suradnicu
                        {
                            aPositionX = 0; //Ideme na novy riadok, preto vynulujeme Xovu suradnicu
                            aPositionY++; //A samozrejme pripocitame Y-ovu
                        }

                        tmpFileStream.ReadLine();
                        continue;
                    }

                    if (!aBlockDictionary.ContainsKey(tmpReadChar))
                    {
                        Console.WriteLine("Znak " + tmpReadChar + " sa este nenachadza v Dictionary");
                        Console.WriteLine();
                        Console.WriteLine("Zadaj meno pre znak " + tmpReadChar);
                        Console.WriteLine();
                        string tmpNameOfBlock = Console.ReadLine();


                        aBlockDictionary.Add(tmpReadChar, tmpNameOfBlock);

                        Console.WriteLine("Znak " + tmpReadChar + " bude teraz definovany ako " + tmpNameOfBlock);
                        Console.WriteLine("--------------------------------------------------------------------");


                        
                    }

                    Console.WriteLine("Blok - " + aBlockDictionary[tmpReadChar] + " X " + aPositionX + " Y " + aPositionY);

                    aPositionX++; //Po nacitani bloku, zvysime X-ovu suradnicu
                }
            }

        }
    }
}
