using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    public class WaterBlockServer : BlockServer
{
    public WaterBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Zap) : base(parPosition, parCollisionType)
    {
    }
}
}
