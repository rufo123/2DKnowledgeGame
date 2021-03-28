using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    public class EndBlockServer : BlockServer
{
    public EndBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Standable) : base(parPosition, parCollisionType)
    {
    }
}
}
