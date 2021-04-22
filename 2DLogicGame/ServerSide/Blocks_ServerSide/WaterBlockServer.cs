using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    /// <summary>
    /// Trieda, ktora reprezentuje blok - vodu. - Server.
    /// </summary>
    public class WaterBlockServer : BlockServer
    {
        /// <summary>
        /// Konstuktor bloku - vody.
        /// </summary>
        /// <param name="parPosition">Paramter, reprezentujuci poziciu - typ Vector2</param>
        /// <param name="parCollisionType">Paramter, reprezentujuci koliziu vodneho bloku - typ BlockCollisionType.</param>
        public WaterBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Zap) : base(parPosition, parCollisionType)
        {
        }
    }
}
