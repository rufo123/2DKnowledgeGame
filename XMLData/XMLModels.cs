using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace XMLData
{
    //Musime si dat pozor na to aby trieda bola PUBLIC, kvoli XML Serializacii
    
    public class LevelMaker
    {
        /// <summary>
        /// Auto Property - Oznacujuca Nazov Levelu - Typ String
        /// </summary>
        public string LevelName { get; set; } //Auto property - LevelName

        /// <summary>
        /// Auto Property - Oznacujuca Prednastavenu Suradnicu hrac - List Suradnice X a Y, pozor list zacina na 0, ale ID hracov na 1
        /// </summary>
        public List<PlayerPosition> DefaultPlayerPositionList { get; set; } //Auto

        /// <summary>
        /// Auto Property - Oznacujuca Dictionary: Key - Pozicia, Value - BlockName
        /// </summary>
        /// Pozn. Kvoli ContentSerializeru, musela byt pridana referencia na projekt s hrou
        //public Dictionary<Vector2, string> BlockPositionDictionary { get; set; }

        //Poznamka - Problem s includovanim Monogame Pipelinu
        //Trebalo pouzit Nuget a Pipeline stiahnut na novo...

        [ContentSerializer(ElementName = "BlockData", CollectionItemName = "BlockPosition")]
        public List<BlockData> BlockDataList { get; set; }

    }

    public class BlockData
    {
        [XmlElement(elementName: "Position")]
        public Vector2 Position;

        [XmlElement(elementName: "BlockName")]
        public string BlockName;

        [ContentSerializer(Optional = true)]
        [XmlElement(elementName: "AdditionalData")]
        public string AdditionalData;
    }

    public class PlayerPosition
    {
        [XmlElement(elementName: "X")] 
        public int PositionX;

        [XmlElement(elementName: "Y")] 
        public int PositionY;
    }

    public class QuestionsStorage
    {
        [ContentSerializer(ElementName = "Questions", CollectionItemName = "Answers")]
        public List<QuestionsStorageItems> QuestionsStorageItemsList;
    }

    public class QuestionsStorageItems
    {
        public string Question;

        public List<string> ListOfAnswers; //List vsetkych moznych odpovedi

        public char GoodAnswer; //Charakterizovana A, B, C, D
    }

    public class Vocabulary
    {
        [ContentSerializer(ElementName = "Vocabulary", CollectionItemName = "Word")]
        public List<VocabularyItems> VocabularyItems;
    }

    public class VocabularyItems
    {
        public string English;

        public string Slovak;
    }
}
