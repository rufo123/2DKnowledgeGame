using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    public class ButtonBlockServer : BlockServer
    {

        private bool aSucceded = false;

        private bool aIsTurnedOn = false;

        public bool Succeded
        {
            get => aSucceded;
            set => aSucceded = value;
        }

        public ButtonBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Button) : base(parPosition, parCollisionType)
        {
            aSucceded = false;
            aIsTurnedOn = false;
            IsInteractible = true;
        }
        public void ChangeToSuccessState()
        {
            aSucceded = true;
            aIsTurnedOn = false;
            IsInteractible = false;
            this.BlockCollisionType = BlockCollisionType.None;
        }

    }
}
