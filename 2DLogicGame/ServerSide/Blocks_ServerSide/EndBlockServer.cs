using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    /// <summary>
    /// Trieda, reprezentujuca konecny block. - Server.
    /// </summary>
    public class EndBlockServer : BlockServer
    {
        /// <summary>
        /// Konstruktor konecneho bloku.
        /// </summary>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parCollisionType">Parameter, reprezentujuci koliziu konecneho bloku - typ BlockCollisionType.</param>
        public EndBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Standable) : base(parPosition, parCollisionType)
        {
        }
    }
}
