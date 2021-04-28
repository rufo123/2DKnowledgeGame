using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Trieda, ktora reprezentuje blok - dvere. - Klient.
    /// </summary>
    public class DoorBlock : Block
    {
        /// <summary>
        /// Konstruktor bloku - dveri. - Klient.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parTexture">Parameter, reprezentujuci texturu - typ Texture2D.</param>
        /// <param name="parIsAnimated">Parameter, reprezentujuci ci sa jedna o animovany blok - typ bool.</param>
        /// <param name="parHasStates">Parameter, reprezentujuci ci blok ma stavy - typ bool.</param>
        /// <param name="parCountOfFrames">Parameter, reprezentujuci pocet framov bloku - typ int.</param>
        /// <param name="parCollisionType">Parameter, reprezentujuci koliziu bloku - typ BlockCollisionType - enum. </param>
        public DoorBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parIsAnimated = false, bool parHasStates = false, int parCountOfFrames = 0, BlockCollisionType parCollisionType = BlockCollisionType.Wall) : base(parGame, parPosition, parTexture, parIsAnimated, parHasStates, parCountOfFrames, parCollisionType)
        {
            SetImageLocation("Sprites\\Blocks\\doorBlock");
        }
    }
}
