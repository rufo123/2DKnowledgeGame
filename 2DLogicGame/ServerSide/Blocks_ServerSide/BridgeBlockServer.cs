using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    public class BridgeBlockServer : BlockServer
{
    public BridgeBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Zap) : base(parPosition, parCollisionType)
    {
        IsHidden = true;
    }

    public void Show()
    {
        this.BlockCollisionType = BlockCollisionType.Standable;
    }
}
}
