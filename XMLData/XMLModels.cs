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
        /// Auto Property - Oznacujuca Poziciu Bloku X - Typ Int - napr. uz v programe, dostaneme poziciu 1 a tu vynasobime, nami zvolenou velkostou bloku - cize x bude 1*64 ...
        /// </summary>
        public int PositionX { get; set; } //Auto Property - Pozicia

        /// <summary>
        /// Auto Property - Oznacujuca Poziciu Bloku Y - Typ Int - napr. uz v programe, dostaneme poziciu 1 a tu vynasobime, nami zvolenou velkostou bloku -  y bude 1*64...
        /// </summary>
        public int PositionY { get; set; } //Auto Property - Pozicia

        /// <summary>
        /// Auto Property - Oznacujuca Meno Bloku - Meno bloku by sa malo zhodovat s nazvom textury - Pre spravne fungovanie... 
        /// </summary>
        public int BlockName { get; set; } //Auto Property - Pozicia
    }
}
