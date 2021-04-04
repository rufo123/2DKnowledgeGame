using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using _2DLogicGame.ServerSide.Questions_ServeSide;
using Lidgren.Network;
using SharpFont;
using XMLData;

namespace _2DLogicGame.ServerSide.LevelEnglish_Server
{

    public enum VocabularyFeedback
    {
        None = 0,
        Init = 1,
        AnswerCorrect = 2
    }


    public class EnglishManagerServer
    {

        private LogicGame aLogicGame;

        private List<VocabularyItems> aVocabularyItemsList;

        /// <summary>
        /// Dictionary, ktora reprezentuje uz vybrate polozky z Vocabulary
        /// </summary>
        private Dictionary<int, bool> aAlreadyPickedDictionary;

        /// <summary>
        /// List, ktory v sebe uchovava informacie o ButtonServerBlokoch - typ List<ButtonBlockServer>
        /// </summary>
        private List<ButtonBlockServer> aButtonList;

        /// <summary>
        /// List, ktory v sebe uchovava informacie o vybranych slovach z vocabulary - typ List<VocabularyItems>
        /// </summary>
        private List<VocabularyItems> aPickedWordsList;

        /// <summary>
        /// Atribut, ktory reprezentuje Vocabulary Feedback - typ Vocabulary Feedback
        /// </summary>
        private VocabularyFeedback aVocabularyFeedback;

        /// <summary>
        /// Atribut, ktory reprezentuje posledne zvolene anglicke slovo - typ int
        /// </summary>
        private int aLastEnglishWord;

        /// <summary>
        /// Atribut, ktory reprezentuje posledne zvolene slovenske slovo - typ int
        /// </summary>
        private int aLastSlovakWord;

        /// <summary>
        /// Atribut, ktory reprezentuje posledne dobre ID odpovedi - typ int
        /// </summary>
        private int aLastSubmittedAnswerID;

        /// <summary>
        /// Atribut, ktory reprezentuje ci je update pripraveny na odoslanie - typ bool
        /// </summary>
        private bool aUpdateIsReady;

        public VocabularyFeedback VocabularyFeedback
        {
            get => aVocabularyFeedback;
            set => aVocabularyFeedback = value;
        }
        public bool UpdateIsReady
        {
            get => aUpdateIsReady;
            set => aUpdateIsReady = value;
        }

        /// <summary>
        /// Konstruktor triedy, ktora sa stara o spravu Anglickeho Levelu na serveri
        /// </summary>
        /// <param name="parLogicGame">Parameter reprezentujuci hru - typ LogicGame - Musi tu byt skrz Content.Load</param>
        public EnglishManagerServer(LogicGame parLogicGame)
        {
            aLogicGame = parLogicGame;
            aAlreadyPickedDictionary = new Dictionary<int, bool>();
            aButtonList = new List<ButtonBlockServer>();
            aPickedWordsList = new List<VocabularyItems>();
            InitVocabulary("Vocabulary\\vocabulary");
            aVocabularyFeedback = VocabularyFeedback.None;
            aLastSubmittedAnswerID = -1;

            aLastSlovakWord = -1;
            aLastEnglishWord = -1;

            aUpdateIsReady = false;
        }

        /// <summary>
        /// Metoda, ktora Inicializuje Otazky zo xml suboru
        /// </summary>
        /// <param name="parVocabularyPath">Parameter - cesta k suboru z databami o slovniku - typ string</param>
        public void InitVocabulary(string parVocabularyPath)
        {
            try
            {
                var tmpVocabulary = aLogicGame.Content.Load<XMLData.Vocabulary>(parVocabularyPath);
                aVocabularyItemsList = tmpVocabulary.VocabularyItems;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// Metoda, ktora nahodne vyberie polozku z Vocabulary
        /// </summary>
        public VocabularyItems PickVocabularyItem()
        {
            if (aAlreadyPickedDictionary != null && aVocabularyItemsList != null)
            {
                Random tmpRandom = new Random();

                int tmpGeneratedItemKey =
                    tmpRandom.Next(0, aVocabularyItemsList.Count); //Vyberieme od 0 az po velkost Listu Vocabulary

                while (aAlreadyPickedDictionary.TryAdd(tmpGeneratedItemKey, true) == false
                ) //Ak sa sa este takyto kluc nenachadza v Dictionary - pridame ho, inak sa pokusime vygenerovat novy kluc
                {
                    tmpGeneratedItemKey = tmpRandom.Next(0, aVocabularyItemsList.Count);
                }

                return aVocabularyItemsList[tmpGeneratedItemKey];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Metoda, ktora prida tlacidlo do zoznamu tlacidlo
        /// </summary>
        /// <param name="parButton">Parameter, ktory reprezentuje tlacidlo, ktore sa ma pridat do zoznamu tlacidiel - typ ButtonBlockServer</param>
        public void AddButton(ButtonBlockServer parButton)
        {
            if (aButtonList != null)
            {
                aButtonList.Add(parButton);

                if (aButtonList.Count % 2 != 0
                ) //Po pridani kazdeho druheho tlacitka, budeme vediet, ze potrebujeme jedno slovo
                {
                    aPickedWordsList.Add(PickVocabularyItem());
                }
            }
        }

        /// <summary>
        /// Metoda, ktora pripravi data o otazkach na odoslanie klientom
        /// </summary>
        /// <param name="parOutgoingMessage">Parameter reprezentujuci odchadzajucu spravu - typ NetOutgoingMessage</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareVocabularyData(NetOutgoingMessage parOutgoingMessage)
        {
            if (aPickedWordsList != null)
            {
                parOutgoingMessage.Write((byte)aVocabularyFeedback);

                if (aVocabularyFeedback == VocabularyFeedback.Init)
                {
                    parOutgoingMessage.WriteVariableInt32(aPickedWordsList.Count);

                    for (int i = 0; i < aPickedWordsList.Count; i++)
                    {
                        parOutgoingMessage.Write(aPickedWordsList[i].English);
                        parOutgoingMessage.Write(aPickedWordsList[i].Slovak);
                    }
                }

                if (aVocabularyFeedback == VocabularyFeedback.AnswerCorrect)
                {
                    parOutgoingMessage.WriteVariableInt32(aLastSubmittedAnswerID);
                }

                return parOutgoingMessage;
            }


            return parOutgoingMessage;
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizaciu Managera
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < aButtonList.Count; i++)
            {
                if (aButtonList[i].WantsToInteract && aButtonList[i].Succeded == false)
                {
                    if (i % 2 != 0 ) //Slovenske
                    {
                        aLastSlovakWord = i / 2; //Server nepotrebuje vediet realne o ake slova ide, staci ID cislo

                    }
                    else //Anglicke
                    {
                        aLastEnglishWord = i / 2;
                    }

                    aButtonList[i].WantsToInteract = false;
                }

            }

            if (aLastSlovakWord == aLastEnglishWord && aLastSlovakWord != -1 && aLastEnglishWord != -1)
            {
                aVocabularyFeedback = VocabularyFeedback.AnswerCorrect;

                Debug.WriteLine("Vysledok spravny");


                if (aLastSlovakWord == 0 || aLastEnglishWord == 0)
                {
                    aButtonList[0].WantsToInteract = false; //Prve tlacidlo zlava
                    aButtonList[0].ChangeToSuccessState();
                    aButtonList[1].WantsToInteract = false; //Prve tlacidlo zprava
                    aButtonList[1].ChangeToSuccessState();
                }
                else
                {
                    aButtonList[aLastEnglishWord * 2].WantsToInteract = false; //Prve tlacidlo zlava - ID Tlacitka dostaneme -> 2 * Cislo
                    aButtonList[aLastEnglishWord * 2].ChangeToSuccessState();
                    aButtonList[aLastSlovakWord * 2 + 1].WantsToInteract = false; //Prve tlacidlo zprava - ID Tlacitka dostaneme -> 2 * Cislo - 1
                    aButtonList[aLastSlovakWord * 2 + 1].ChangeToSuccessState();
                }

                aLastSubmittedAnswerID = aLastSlovakWord; //Nemusel by tu tento atribut byt, ale chcem aby obe slova som mohol hned resetnut a nie az po odoslani updatu

                aLastSlovakWord = -1;
                aLastEnglishWord = -1;

                aUpdateIsReady = true;
            }

        }

    }
}
