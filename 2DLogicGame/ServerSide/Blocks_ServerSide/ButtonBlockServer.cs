using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    /// <summary>
    /// Trieda, ktora reprezentuje blok - tlacidlo. - Server.
    /// </summary>
    public class ButtonBlockServer : BlockServer
    {

        /// <summary>
        /// Atribut, ktory reprezentuje ci bola splnena uloha suvisiaca s tlacidlom - typ bool.
        /// </summary>
        private bool aSucceded = false;


        public bool Succeded
        {
            get => aSucceded;
            set => aSucceded = value;
        }

        /// <summary>
        /// Konstruktor bloku - tlacidla.
        /// </summary>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parCollisionType">Parameter, reprezentujuci koliziu bloku - typ BlockCollisionType.</param>
        public ButtonBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Button) : base(parPosition, parCollisionType)
        {
            aSucceded = false;
            IsInteractible = true;
        }

        /// <summary>
        /// Metoda, ktora sa stara o zmenu stavu tlacidla na uspesny. Teda uloha, ktora bola spojena s tlacidlom bola splnena.
        /// </summary>
        public void ChangeToSuccessState()
        {
            aSucceded = true;
            IsInteractible = false;
            this.BlockCollisionType = BlockCollisionType.None;
        }

    }
}
