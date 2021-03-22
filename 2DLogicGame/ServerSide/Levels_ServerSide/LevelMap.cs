using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.ServerSide.Blocks_ServerSide;
using XMLData;

namespace _2DLogicGame.ServerSide.Levels_ServerSide
{
    public class LevelMap
{
        /// <summary>
        /// Dictionary - Key - Vector (Pozicia Bloku), Value - Objekt Block (Blok)
        /// K jednotlivym blokom budeme pristupovat casto cez suradnice, preto pouzijem Dictionary
        /// V XML Je pouzity list, preto.. Lebo sa musi postupne cez for cyklus prejst cely List
        /// Pri Dictionary bude pristupovat k presnym suradniciam...
        /// </summary>
        private Dictionary<Vector2, BlockServer> aBlockPositionDictionary;

        /// <summary>
        /// Atribut List - Reprezentujuci List Blokov v Leveli - typ DrawableGameComponent
        /// Potrebujeme aj List aby sme mohli bloky pridat do naseho GameComponentu reprezentujuceho Hraciu Obrazovku
        /// </summary>
        private List<BlockServer> aBlockList;



        /// <summary>
        /// Atribut, Reprezentujuci Vysku a Sirku Blokov, napr 64 - Defaultne
        /// </summary>
        private int aDefaultBlockDimension;

        public int DefaultBlockDimension
        {
            get => aDefaultBlockDimension;
            set => aDefaultBlockDimension = value;
        }

        public LevelMap(int parDefaultBlockDimension = 64)
        {
            aDefaultBlockDimension = parDefaultBlockDimension;
            aBlockPositionDictionary = new Dictionary<Vector2, BlockServer>(30 * 16);
            aBlockList = new List<BlockServer>(30 * 16);
        }

        public void InitMap(List<BlockData> parBlockPositions)
        {
            for (int i = 0; i < parBlockPositions.Count; i++)
            {
                if (parBlockPositions[i].BlockName == "waterBlock")
                {
                    //Preto som to musel spravit takto, lebo v "XMLData", som pouzil Systemovy Vector2
                    //Treba pripomenut, ze nase pozicie boli ulozene len v jednotkovych suradniciach tzn 0, 1, 2..
                    //Preto k nim treba pripocitat aj velkost blokov
                    //A samozrejme netreba zabudnut aj na Skalovanie - to si preberieme priamo od aLogicGame
                    //Klasicky pouzivam Vector z MonoGame
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension));

                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    WaterBlockServer tmpWaterBlock = new WaterBlockServer(tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpWaterBlock);
                    aBlockList.Add(tmpWaterBlock);

                }
                else if (parBlockPositions[i].BlockName == "barrierBlock")
                {
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension));

                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    BarrierBlockServer tmpBarrierBlock = new BarrierBlockServer(tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpBarrierBlock);
                    aBlockList.Add(tmpBarrierBlock);
                }
                else
                {
                    //Blank
                }

            }
        }

        /// <summary>
        /// Metoda, ktora vrati List Blokov v podobe DrawableGameComponentov
        /// </summary>
        /// <returns>List<DrawableGameComponent> - Block List</returns>
        public List<BlockServer> GetBLockList()
        {
            return aBlockList;
        }

        /// <summary>
        /// Metoda, ktora vrati Dictionary Pozicie Blokov
        /// </summary>
        /// <returns>Dictionary<Vector2, Block> - Pozicie Blokov</returns>
        public Dictionary<Vector2, BlockServer> GetBlocksPositionDictionary()
        {
            if (aBlockPositionDictionary != null)
            {
                return aBlockPositionDictionary;
            }
            else
            {
                return null;
            }
        }
    }
}
