using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2DLogicGame.GraphicObjects
{



    public class BridgeBlock : Block
    {
        /// <summary>
        /// Atribut - Typ WaterBlock - Reprezentuje block vody pod mostom
        /// </summary>
        private WaterBlock aWaterBlock;

        /// <summary>
        /// Atribut reprezentujuci hru - typ LogicGame
        /// </summary>
        private LogicGame aGame;

        private bool aWantsToShow;

        private int aTimer;

        public BridgeBlock(LogicGame parGame, Vector2 parPosition, WaterBlock parWaterBlock, Texture2D parTexture = null) : base(parGame, parPosition, parTexture, parCollisionType: BlockCollisionType.Zap)
        {
            SetImageLocation("Sprites\\Blocks\\bridgeBlock");
            this.LayerDepth = LayerDepth - 0.1F;

            //Pridame, pod most aj blok vody
            aGame = parGame;
            aWaterBlock = parWaterBlock;
            IsHidden = true;
            aWantsToShow = false;
            Visibility = 0F;
            aTimer = 0;
            DefaultBlockCollisionType = BlockCollisionType.Standable; //Nastavime, ze prednastavena kolizie je - da na moste stat.
        }

        public void Show(bool parMakeBridgeVisible)
        {
            //Kedze sa nam most objavi nad vodou, odstranime koliziu bloku pod mostom
            aWaterBlock.BlockCollisionType = BlockCollisionType.None;
            this.BlockCollisionType = BlockCollisionType.Standable;

            if (parMakeBridgeVisible) //Pokial pride poziadavka, ze most sa ma aj vykreslit
            {
                aWantsToShow = true;
            }
        }

        public override void Update(GameTime gameTime)
        {

            if (aWantsToShow)
            {
                IsHidden = false;
            }

            if (aWantsToShow == true && Visibility < 1)
            {
                aTimer += gameTime.ElapsedGameTime.Milliseconds;
            }

            if (aTimer > 25)
            {
                aTimer = 0;
                Visibility += 0.1F;
            }

            if (Visibility >= 1)
            {
                Visibility = 1F;
            }


            base.Update(gameTime);
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