using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{
    public class DoorBlock : Block
    {
        public DoorBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parIsAnimated = false, bool parHasStates = false, int parCountOfFrames = 0, BlockCollisionType parCollisionType = BlockCollisionType.Wall) : base(parGame, parPosition, parTexture, parIsAnimated, parHasStates, parCountOfFrames, parCollisionType)
        {
            SetImageLocation("Sprites\\Blocks\\doorBlock");
        }
    }
}
