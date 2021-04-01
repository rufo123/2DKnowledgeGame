﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using XMLData;

// using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;


// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace LevelCreator
{
    public class CreatorLevel
    {
        /// <summary>
        /// Atribut Dictionary - Bude nam sluzit na uchovananie dat o blokoch...
        /// </summary>
        private Dictionary<char, BlockData> aBlockDictionary;

        /// <summary>
        /// Atribut Dictionary - Ako kluc je pozicia bloku a hodnota je nazov bloku
        /// </summary>
       // private Dictionary<Vector2, string> aBlockPositionDictionary; 

        private int aPositionX = 0;
        private int aPositionY;

        public CreatorLevel()
        {

            XMLData.LevelMaker tmpNewLevel = new XMLData.LevelMaker();

            aBlockDictionary = new Dictionary<char, BlockData>();

            tmpNewLevel.DefaultPlayerPositionList = new List<PlayerPosition>();

            tmpNewLevel.BlockDataList = new List<BlockData>();

            Console.WriteLine("Zadaj nazov levelu: ");

            tmpNewLevel.LevelName = Console.ReadLine();

            Console.WriteLine("Zadaj meno suboru, ktory bude obsahovat udaje o " + tmpNewLevel.LevelName);

            string tmpNewFileName = Console.ReadLine();

            Console.WriteLine("Zadaj cestu a nazov suboru pre level " + tmpNewLevel.LevelName + ". Pozn - Musi sa nachadzat v zlozke s Projektom...");

            string tmpFilePath = AppContext.BaseDirectory + Console.ReadLine(); //Nezabudnut nastavit - Tuknut na TXT - Copy to OutPut Directory na Always...

            while (!File.Exists(tmpFilePath)) //Ak takyto subor neexistuje
            {
                Console.WriteLine("Takyto subor neexistuje, zadaj platnu cestu!");
                tmpFilePath = Console.ReadLine(); //Vypytame si ho znova
            }

            Console.WriteLine("Subor " + tmpFilePath + " uspesne nacitany!");

            int tmpPlayerIdCounter = 1; //Pomocne pocitadlo ID Hracov - Pouzite pri definovani prednastavenych suradnic


            using (StreamReader tmpFileStream = new StreamReader(tmpFilePath))
            {
                while (tmpFileStream.Peek() > 0) //Ak sa v subore nachadza dalsi character
                {
                    char tmpReadChar = (char)tmpFileStream.Read();

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
                        string tmpNameOfBlock = Console.ReadLine(); //Nazov Bloku - napr. waterBlock

                        Console.WriteLine("Zadajte doplnujuce informacie o bloku (Enter -> Ziadne)");

                        string tmpAdditionalData = Console.ReadLine(); //Doplnujuce udaje o bloku - ako napr. Cislo Mostu...

                        BlockData tmpNewBlockData = new BlockData();
                        tmpNewBlockData.BlockName = tmpNameOfBlock;

                        if (!string.IsNullOrEmpty(tmpAdditionalData)) //Ak sme zadali nejake doplnujuce udaje o bloku, tak ich aj ulozime
                        {
                            tmpNewBlockData.AdditionalData = tmpAdditionalData;
                        }

                        aBlockDictionary.Add(tmpReadChar, tmpNewBlockData);

                        Console.WriteLine("Znak " + tmpReadChar + " bude teraz definovany ako " + tmpNameOfBlock);
                        Console.WriteLine("--------------------------------------------------------------------");

                    }

                    Console.WriteLine("Blok - " + aBlockDictionary[tmpReadChar].BlockName + " X " + aPositionX + " Y " + aPositionY);

                    // tmpNewLevel.BlockPositionDictionary.Add(new Vector2(aPositionX, aPositionY), aBlockDictionary[tmpReadChar]); //Pocitam s tym, ze pozicie nebudu rovnake

                  


                    BlockData tmpBlockData = new BlockData();
                    tmpBlockData.BlockName = aBlockDictionary[tmpReadChar].BlockName;
                    tmpBlockData.Position = new Vector2(aPositionX, aPositionY);
                    tmpBlockData.AdditionalData = aBlockDictionary[tmpReadChar].AdditionalData;

                    if (tmpReadChar == 'S' || tmpReadChar == 's') //Oznacenie - Spawn Hraca
                    {
                        Console.WriteLine("Bol detegovany spawn hraca");
                        Console.WriteLine("Pridavam Suradnicu " + aPositionX + " " + aPositionY + " ako prednastaveny bod znovuzrodenia pre hraca - " + tmpPlayerIdCounter);

                        PlayerPosition tmpPlayerPosition = new PlayerPosition();
                        tmpPlayerPosition.PositionX = aPositionX;
                        tmpPlayerPosition.PositionY = aPositionY;

                        tmpNewLevel.DefaultPlayerPositionList.Add(tmpPlayerPosition); //Tu netreba zabudnut na to, ze hrac 1 - bude ulozeny na poziicii 0
                        tmpPlayerIdCounter++;
                    }

                    tmpNewLevel.BlockDataList.Add(tmpBlockData);
                    aPositionX++; //Po nacitani bloku, zvysime X-ovu suradnicu
                }
            }

            XmlWriterSettings tmpSettings = new XmlWriterSettings();
            tmpSettings.Indent = true;


            using (XmlWriter tmpWriter = XmlWriter.Create(tmpNewFileName, tmpSettings))
            {
                //Tu velky pozor, treba dat velky pozor..
                //Ak by isla tato cast projekty do produkcie
                //Budeme tu mat includnutu content pipeline, ktora zabera asi 100MB a to nechceme
                //Preto tuto referenciu pri produkcii odstranim, kedze pre bezneho pouzivatela je tento Level Creator nepouzitelny - Je pre mna.

                //Tu nastal jeden obrovsky problem, kedze include Monogame Pipelinu nefungoval
                //Musel som pouzit Nuget...

                IntermediateSerializer.Serialize(tmpWriter, tmpNewLevel, null);
            }

            //Poznamka od Veduceho - Mozno by bolo dobre spravit to, ze ak sa tam nejaky blok opakuje vela krat napríklad 0 - cize vzduch. Tak to tam proste nedať. 
            //Proste to neulozit do XML-ka -> To znamena, ze pri nacitani by tam ostal vzduch. Alebo zadefinovat, ze pri nacitani, tam kde nie je nic, dat napriklad travu...

            
        }

    }
}