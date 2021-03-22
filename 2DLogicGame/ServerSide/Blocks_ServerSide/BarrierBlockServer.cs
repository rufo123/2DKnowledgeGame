using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.ServerSide.Blocks_ServerSide;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    public class BarrierBlockServer : BlockServer
{
    public BarrierBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Wall) : base(parPosition, parCollisionType)
    {
        
    }
}
}
