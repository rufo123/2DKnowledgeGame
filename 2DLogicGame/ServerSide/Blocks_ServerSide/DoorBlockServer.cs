using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    /// <summary>
    /// Trieda, reprezentujuca blok - dvere. - Server.
    /// </summary>
    public class DoorBlockServer : BlockServer
    {
        /// <summary>
        /// Konstruktor bloku - dver.
        /// </summary>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parCollisionType">Parameter, reprezentujuci koliziu dveri - typ BlockCollisionType</param>
        public DoorBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Wall) : base(parPosition, parCollisionType)
        {
        }
    }
}
