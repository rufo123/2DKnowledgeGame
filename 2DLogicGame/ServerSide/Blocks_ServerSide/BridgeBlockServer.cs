using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    /// <summary>
    /// Trieda ktora reprezentuje blok - most. - Server.
    /// </summary>
    public class BridgeBlockServer : BlockServer
    {
        /// <summary>
        /// Konstruktor bloku - mostu.
        /// </summary>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parCollisionType">Parameter, reprezentujuci koliziu mostu - typ BlockCollisionType.</param>
        public BridgeBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Zap) : base(parPosition, parCollisionType)
        {
            IsHidden = true;
        }

        /// <summary>
        /// Metoda, ktora sa stara o zobrazenie mostu.
        /// </summary>
        public void Show()
        {
            this.BlockCollisionType = BlockCollisionType.Standable;
        }
    }
}
