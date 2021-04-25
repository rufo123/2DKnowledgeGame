using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    /// <summary>
    /// Trieda, ktora reprezentuje blok - stupny blok. - Server.
    /// </summary>
    public class InputBlockServer : BlockServer
    {
        /// <summary>
        /// Atribut, reprezentujuci cislo vstupu - typ int.
        /// </summary>
        private int aNumber;

        /// <summary>
        /// Atribut, reprezentujuci najmensie dosiahnutelne cislo - typ int.
        /// </summary>
        private int aMinNumber;

        /// <summary>
        /// Atribut, reprezentujuci najvacsie dosiahnutelne cislo - typ int.
        /// </summary>
        private int aMaxNumber;

        public int Number
        {
            get => aNumber;
            set => aNumber = value;
        }

        public int MaxNumber
        {
            get => aMaxNumber;
            set => aMaxNumber = value;
        }

        /// <summary>
        /// Konstruktor input bloku.
        /// </summary>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parCollisionType">Parameter, reprezentujuci koliziu input bloku - typ BlockCollisionType.</param>
        /// <param name="parMinNumber">Parameter, reprezentujuci najmensie dosiahnutelne cislo - typ int.</param>
        /// <param name="parMaxNumber">Parameter, reprezentujuci najvacsie dosiahnutelne cislo - typ int.</param>
        public InputBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Wall, int parMinNumber = 0, int parMaxNumber = 9) : base(parPosition, parCollisionType)
        {
            aNumber = 0;
            aMinNumber = parMinNumber;
            aMaxNumber = parMaxNumber;
            IsInteractible = true;

        }

        /// <summary>
        /// Metoda, ktora prepne cislo, podla toho ci chceme ist dopredu, alebo dozadu
        /// Ak dopredu, pripocita sa
        /// Ak dozadu, pripocita sa
        /// V pripade ze dosiahne maximum, prepne sa na minimum a naopak
        /// </summary>
        /// <param name="parBackwards"></param>
        public void SwitchNumber(bool parBackwards)
        {
            if (parBackwards) //Ak chceme posunut cisla dozadu
            {
                if (aNumber <= aMinNumber)
                {
                    aNumber = aMaxNumber;
                }
                else
                {
                    aNumber--;
                }
            }
            else
            {
                if (aNumber >= aMaxNumber)
                {
                    aNumber = aMinNumber;
                }
                else
                {
                    aNumber++;
                }
            }
        }

        /// <summary>
        /// Metoda, ktora zrusi vyber cisla a vynuluje ho
        /// </summary>
        public void ResetSubmission()
        {
            aNumber = 0;
        }

        /// <summary>
        /// Metoda, ktora overriduje originalnu, metodu a pripocitava cislo reprezentovane v InputBlockServer, nasledne nastavi, ze uz nechce nic s blokom interagovat
        /// </summary>
        public override void Interact()
        {
            base.Interact();

            if (WantsToInteract)
            {
                SwitchNumber(false);
            }

            WantsToInteract = false;
        }
    }
}
