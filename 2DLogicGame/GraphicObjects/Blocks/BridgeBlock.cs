using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Trieda reprezentujuca blok - most. - Klient.
    /// </summary>
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

        /// <summary>
        /// Atribut, ktory reprezentuje ci sa ma blok zobrazit - typ bool.
        /// </summary>
        private bool aWantsToShow;

        /// <summary>
        /// Atribut, ktory reprezentuje casovac - typ int.
        /// </summary>
        private int aTimer;
        
        /// <summary>
        /// Konstruktor bloku - mosta.
        /// Je dobre spomenut, ze si inicializuje aj blok vody, ktory sa zobrazi pod mostom.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parWaterBlock">Parameter, reprezentujuci blok - vodu - typ WaterBlock. </param>
        /// <param name="parTexture">Parameter, reprezentujuci textutu - typ Texture2D.</param>
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

        /// <summary>
        /// Metoda, ktora sa stara o zobrazenie mostu.
        /// </summary>
        /// <param name="parMakeBridgeVisible">Parameter, ci sa ma most zobrazit alebo nie - typ bool.</param>
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

        /// <summary>
        /// Metoda, ktora sa satara o plynule vykreslovanie mostu.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Update(GameTime parGameTime)
        {

            if (aWantsToShow)
            {
                IsHidden = false;
            }

            if (aWantsToShow == true && Visibility < 1)
            {
                aTimer += parGameTime.ElapsedGameTime.Milliseconds;
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


            base.Update(parGameTime);
        }

        /// <summary>
        /// Destruktor
        /// </summary>
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