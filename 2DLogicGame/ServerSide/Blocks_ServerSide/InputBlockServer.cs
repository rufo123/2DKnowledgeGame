using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
{
    public class InputBlockServer : BlockServer
    {

        private int aNumber;

        private int aMinNumber;

        private int aMaxNumber;

        private bool aSubmitted;

        public int Number
        {
            get => aNumber;
            set => aNumber = value;
        }


        public InputBlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.Wall, int parMinNumber = 0, int parMaxNumber = 9) : base(parPosition, parCollisionType)
        {
            aNumber = 0;
            aMinNumber = parMinNumber;
            aMaxNumber = parMaxNumber;
            IsInteractible = true;
            aSubmitted = false;

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
            aSubmitted = false;
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
              // SwitchNumber(false);
            }

            WantsToInteract = false;
        }
    }
}
