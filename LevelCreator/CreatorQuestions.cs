using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using XMLData;

namespace LevelCreator
{
    /// <summary>
    /// Trieda, ktora reprezentuje vytvarac XML suborov otazok a odpovedi.
    /// </summary>
    public class CreatorQuestions
    {

        /// <summary>
        /// Konstruktor vytvaraca XML suborov otazok a odpovedi.
        /// </summary>
        public CreatorQuestions()
        {

            XMLData.QuestionsStorage tmpNewQuestions = new XMLData.QuestionsStorage();

            tmpNewQuestions.QuestionsStorageItemsList = new List<QuestionsStorageItems>();

            Console.WriteLine("Zadajte cestu a nazov suboru, kde sa maju ulozit Otazky. (Pozn. Ukoncite .xml)");

            string tmpNewQuestionsFileName = Console.ReadLine();

            Console.WriteLine("Zadaj cestu a nazov suboru pre level. Pozn - Musi sa nachadzat v zlozke s Projektom...");

            string tmpQuestionsTxtFilePath = AppContext.BaseDirectory + Console.ReadLine(); //Nezabudnut nastavit - Tuknut na TXT - Copy to OutPut Directory na Always...

            while (!File.Exists(tmpQuestionsTxtFilePath)) //Ak takyto subor neexistuje
            {
                Console.WriteLine("Takyto subor neexistuje, zadaj platnu cestu!");
                tmpQuestionsTxtFilePath = Console.ReadLine(); //Vypytame si ho znova
            }

            int tmpQuestionCounter = 0;

            using (StreamReader tmpFileStream = new StreamReader(tmpQuestionsTxtFilePath))
            {
                while (tmpFileStream.Peek() > 0) //Ak sa v subore nachadza dalsi character
                {
                    QuestionsStorageItems tmpItems = new QuestionsStorageItems();

                    tmpItems.Question = tmpFileStream.ReadLine();

                    tmpItems.ListOfAnswers = new List<string>(4);

                    for (int i = 0; i < 4; i++)
                    {
                        tmpItems.ListOfAnswers.Add(tmpFileStream.ReadLine());
                    }
                    tmpItems.GoodAnswer = char.Parse(tmpFileStream.ReadLine() ?? string.Empty);

                    tmpNewQuestions.QuestionsStorageItemsList.Add(tmpItems);

                    tmpQuestionCounter++;

                    Console.WriteLine("Nacitana otazka: " + tmpItems.Question);
                }
            }

            Console.WriteLine("Bolo uspesne nacitanych " + tmpQuestionCounter + " otazok.");

            XmlWriterSettings tmpSettings = new XmlWriterSettings();
            tmpSettings.Indent = true;

            using (XmlWriter tmpWriter = XmlWriter.Create(tmpNewQuestionsFileName ?? throw new InvalidOperationException(), tmpSettings))
            {
                //Tu velky pozor, treba dat velky pozor..
                //Ak by isla tato cast projekty do produkcie
                //Budeme tu mat includnutu content pipeline, ktora zabera asi 100MB a to nechceme
                //Preto tuto referenciu pri produkcii odstranim, kedze pre bezneho pouzivatela je tento Level Creator nepouzitelny - Je pre mna.

                //Tu nastal jeden obrovsky problem, kedze include Monogame Pipelinu nefungoval
                //Musel som pouzit Nuget...

                IntermediateSerializer.Serialize(tmpWriter, tmpNewQuestions, null);
            }


        }

    }
}
