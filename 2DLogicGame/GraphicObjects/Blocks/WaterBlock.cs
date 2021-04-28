using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Trieda reprezentujuca blok - vodu. - Klient.
    /// </summary>
    public class WaterBlock : Block
{
    /// <summary>
    /// Konstruktor bloku - vody.
    /// </summary>
    /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
    /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vectore2.</param>
    /// <param name="parTexture">Parameter, reprezentujuci texturu - typ Texture2D.</param>
    /// <param name="parIsAnimated">Parameter, reprezentujuci ci je blok animovany - typ bool.</param>
    /// <param name="parCountOfFrames">Parameter, reprezentujuci pocet framov animacie - typ int.</param>
    public WaterBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parIsAnimated = true, int parCountOfFrames = 3) : base(parGame, parPosition, parTexture, parIsAnimated, parCountOfFrames: parCountOfFrames, parCollisionType: BlockCollisionType.Zap)
    {
        SetImageLocation("Sprites\\Blocks\\waterBlock");
    }
}
}
