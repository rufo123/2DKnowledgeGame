using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using XMLData;

namespace LevelCreator
{
    public class CreatorVocabulary
    {

        public CreatorVocabulary()
        {

            XMLData.Vocabulary tmpNewVocabulary = new XMLData.Vocabulary();

            tmpNewVocabulary.VocabularyItems = new List<VocabularyItems>();

            Console.WriteLine("Zadajte cestu a nazov suboru, kde sa ma ulozit Slovnik. (Pozn. Ukoncite .xml)");

            string tmpNewVocabularyFileName = Console.ReadLine();

            tmpNewVocabularyFileName = "Vocabulary\\vocabulary.xml";

            Console.WriteLine("Zadaj cestu a nazov suboru pre level. Pozn - Musi sa nachadzat v zlozke s Projektom...");

            string tmpVocabularyTxtFilePath = AppContext.BaseDirectory + Console.ReadLine(); //Nezabudnut nastavit - Tuknut na TXT - Copy to OutPut Directory na Always...

            tmpVocabularyTxtFilePath = "Vocabulary\\vocabulary.txt";

            while (!File.Exists(tmpVocabularyTxtFilePath)) //Ak takyto subor neexistuje
            {
                Console.WriteLine("Takyto subor neexistuje, zadaj platnu cestu!");
                tmpVocabularyTxtFilePath = Console.ReadLine(); //Vypytame si ho znova
            }

            int tmpVocabularyCounter = 0;

            using (StreamReader tmpFileStream = new StreamReader(tmpVocabularyTxtFilePath))
            {
                while (tmpFileStream.Peek() > 0) //Ak sa v subore nachadza dalsi character
                {
                    VocabularyItems tmpItems = new VocabularyItems();

                    tmpItems.English = tmpFileStream.ReadLine();

                    tmpItems.Slovak = tmpFileStream.ReadLine();

                    Console.WriteLine("Nacitane slovo: " + tmpItems.English + " " + tmpItems.Slovak);

                    tmpVocabularyCounter++;

                    tmpNewVocabulary.VocabularyItems.Add(tmpItems);
                }
            }

            Console.WriteLine("Bolo uspesne nacitanych " + tmpVocabularyCounter + " slov.");

            XmlWriterSettings tmpSettings = new XmlWriterSettings();
            tmpSettings.Indent = true;

            using (XmlWriter tmpWriter = XmlWriter.Create(tmpNewVocabularyFileName ?? throw new InvalidOperationException(), tmpSettings))
            {
                //Tu velky pozor, treba dat velky pozor..
                //Ak by isla tato cast projekty do produkcie
                //Budeme tu mat includnutu content pipeline, ktora zabera asi 100MB a to nechceme
                //Preto tuto referenciu pri produkcii odstranim, kedze pre bezneho pouzivatela je tento Level Creator nepouzitelny - Je pre mna.

                //Tu nastal jeden obrovsky problem, kedze include Monogame Pipelinu nefungoval
                //Musel som pouzit Nuget...

                IntermediateSerializer.Serialize(tmpWriter, tmpNewVocabulary, null);
            }


        }


    }
}
