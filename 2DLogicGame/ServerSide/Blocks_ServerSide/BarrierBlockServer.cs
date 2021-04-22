using System;
using System.Collections.Generic;
using System.Text;
using _2DLogicGame.ServerSide.Blocks_ServerSide;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    /// <summary>
    /// Trieda, reprezentujuca blok - barieru. - Server.
    /// </summary>
    public class BarrierBlockServer : BlockServer
{
        /// <summary>
        /// Konstruktor barier bloku.
        /// </summary>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu bloku - typ Vector2.</param>
        /// <param name="parCollisionType">Parameter, reprezentujuci koliziu bloku - typ BlockCollisionType.</param>
        public BarrierBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Wall) : base(parPosition, parCollisionType)
    {
        
    }
}
}
