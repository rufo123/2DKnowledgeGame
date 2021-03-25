using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{
    public class WaterBlock : Block
{
    public WaterBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parIsAnimated = true, int parCountOfFrames = 3) : base(parGame, parPosition, parTexture, parIsAnimated, parCountOfFrames: parCountOfFrames, parCollisionType: BlockCollisionType.Slow)
    {
            SetImageLocation("Sprites\\Blocks\\waterBlock");
    }
}
}
