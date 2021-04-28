using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Trieda ktora reprezentujuci blok - stenu. - Cast Klienta.
    /// </summary>
    public class BarrierBlock : Block
{
    /// <summary>
    /// Konstruktor bloku - steny.
    /// </summary>
    /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
    /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
    /// <param name="parTexture">Parameter, reprezentujuci texturu - typ Texture2D.</param>
    public BarrierBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null ) : base(parGame, parPosition, parTexture, parCollisionType: BlockCollisionType.Wall)
    {
        SetImageLocation("Sprites\\Blocks\\barrierBlock");
    }
}
}
