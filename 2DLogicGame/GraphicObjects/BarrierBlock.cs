using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{
    class BarrierBlock : Block
{
    public BarrierBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null ) : base(parGame, parPosition, parTexture, parCollisionType: BlockCollisionType.Wall)
    {
        SetImageLocation("Sprites\\Blocks\\barrierBlock");
    }
}
}
