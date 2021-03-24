using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2DLogicGame.GraphicObjects
{



    class BridgeBlock : Block
    {
        /// <summary>
        /// Atribut - Typ WaterBlock - Reprezentuje block vody pod mostom
        /// </summary>
        private WaterBlock aWaterBlock;

        /// <summary>
        /// Atribut reprezentujuci hru - typ LogicGame
        /// </summary>
        private LogicGame aGame;

        public BridgeBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null) : base(parGame, parPosition, parTexture, parCollisionType: BlockCollisionType.None)
        {
            SetImageLocation("Sprites\\Blocks\\bridgeBlock");
            this.LayerDepth = LayerDepth - 0.1F;

            //Pridame, pod most aj blok vody
            aGame = parGame;
            aWaterBlock = new WaterBlock(parGame, parPosition);


            parGame.Components.Add(aWaterBlock); //Mozeme si dovolit rovno takto pridat komponent do hry
        }

        ~BridgeBlock()
        {
            if (aWaterBlock != null)
            {
                aGame.Components.Remove(aWaterBlock);
                aWaterBlock = null;
            }

        }
    }
}