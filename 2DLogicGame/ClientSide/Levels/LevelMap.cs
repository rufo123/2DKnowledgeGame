using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using _2DLogicGame.GraphicObjects;
using Microsoft.Xna.Framework;
using XMLData;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace _2DLogicGame.ClientSide.Levels
{
    class LevelMap
    {

        /// <summary>
        /// Dictionary - Key - Vector (Pozicia Bloku), Value - Objekt Block (Blok)
        /// K jednotlivym blokom budeme pristupovat casto cez suradnice, preto pouzijem Dictionary
        /// V XML Je pouzity list, preto.. Lebo sa musi postupne cez for cyklus prejst cely List
        /// Pri Dictionary bude pristupovat k presnym suradniciam...
        /// </summary>
        private Dictionary<Vector2, Block> aBlockPositionDictionary;

        /// <summary>
        /// Atribut List - Reprezentujuci List Blokov v Leveli - typ DrawableGameComponent
        /// Potrebujeme aj List aby sme mohli bloky pridat do naseho GameComponentu reprezentujuceho Hraciu Obrazovku
        /// </summary>
        private List<DrawableGameComponent> aBlockList;


        /// <summary>
        /// Atribut - LogicGame - Hra
        /// </summary>
        private LogicGame aLogicGame;

        /// <summary>
        /// Atribut, Reprezentujuci Vysku a Sirku Blokov, napr 64 - Defaultne
        /// </summary>
        private int aDefaultBlockDimension;

        public int DefaultBlockDimension
        {
            get => aDefaultBlockDimension;
            set => aDefaultBlockDimension = value;
        }

        public LevelMap(LogicGame parLogicGame, int parDefaultBlockDimension = 64)
        {
            aLogicGame = parLogicGame;

            aDefaultBlockDimension = parDefaultBlockDimension;
            aBlockPositionDictionary = new Dictionary<Vector2, Block>(31 * 16);
            aBlockList = new List<DrawableGameComponent>(31 * 16);
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
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension ), parBlockPositions[i].Position.Y * (aDefaultBlockDimension ));

                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    WaterBlock tmpWaterBlock = new WaterBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpWaterBlock);
                    aBlockList.Add(tmpWaterBlock);
                }
                else if (parBlockPositions[i].BlockName == "barrierBlock")
                {
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension ));

                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    BarrierBlock tmpBarrierBlock = new BarrierBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpBarrierBlock);
                    aBlockList.Add(tmpBarrierBlock);
                }
                else if (parBlockPositions[i].BlockName == "bridgeBlock")
                {
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension));
                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    BridgeBlock tmpBridgeBlock = new BridgeBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpBridgeBlock);
                    aBlockList.Add(tmpBridgeBlock);
                }
                else if (parBlockPositions[i].BlockName == "endBlock")
                {
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension));
                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    EndBlock tmpEndBlock = new EndBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpEndBlock);
                    aBlockList.Add(tmpEndBlock);
                } else if (parBlockPositions[i].BlockName == "mathGenerateBlock")
                {
                    Vector2 tmpBlockPosition = new Vector2(parBlockPositions[i].Position.X * (aDefaultBlockDimension), parBlockPositions[i].Position.Y * (aDefaultBlockDimension));
                    //Water Block inizializujeme s pouzitim Atributu hry a pozicie bloku
                    ButtonBlock tmpButtonBlock = new ButtonBlock(aLogicGame, tmpBlockPosition);
                    aBlockPositionDictionary.Add(tmpBlockPosition, tmpButtonBlock);
                    aBlockList.Add(tmpButtonBlock);
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
        public List<DrawableGameComponent> GetBLockList()
        {
            return aBlockList;
        }

        /// <summary>
        /// Metoda, ktora vrati Dictionary Pozicie Blokov
        /// </summary>
        /// <returns>Dictionary<Vector2, Block> - Pozicie Blokov</returns>
        public Dictionary<Vector2, Block> GetBlocksPositionDictionary()
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
