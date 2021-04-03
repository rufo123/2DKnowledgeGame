using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    public class DoorBlockServer : BlockServer
{
    public DoorBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Wall) : base(parPosition, parCollisionType)
    {
    }
}
}
