using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{

    /// <summary>
    /// Enumeracna trieda - reprezentujuca stavy tlacidla. - Klient.
    /// </summary>
    public enum ButtonBlockStates
    {
        Off = 0,
        On = 1,
        Success = 2
    }

    /// <summary>
    /// Trida, ktora reprezentuje blok - tlacidlo. - Klient.
    /// </summary>
    public class ButtonBlock : Block
    {
        /// <summary>
        /// Atribut, ktory reprezentuje ci je tlacidlo zapnute - typ bool.
        /// </summary>
        private bool aIsTurnedOn;

        /// <summary>
        /// Atribut, ktory reprezentuje ci je tlacidlo stlacene - typ bool.
        /// </summary>
        private bool aIsPressed;

        /// <summary>
        /// Atribut, ktory reprezentuje ci uloha spojena s tlacidlom bola splnena a tlacidlo to moze nejako identifikovat - typ bool.
        /// </summary>
        private bool aSucceded = false;

        public bool IsTurnedOn
        {
            get => TurnOnGetCheck();
            set => TextureIsOnFirstState = !value;
        }

        public bool IsPressed
        {
            get => aIsPressed;
            set => aIsPressed = value;
        }
        public bool Succeded
        {
            get => aSucceded;
            set => aSucceded = value;
        }

     
        /// <summary>
        /// Kontruktor bloku - tlacidla.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hry - typ LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parTexture">Parameter, reprezentujuci textutu - typ Vector2.</param>
        /// <param name="parHasBlockStates">Parameter, reprezentujuci, ci ma blok stavy - typ bool.</param>
        /// <param name="parCountOfFrames">Parameter, reprezentujuci z kolkych framov sa skladaju stavy bloku - typ int.</param>
        public ButtonBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parHasBlockStates = true, int parCountOfFrames = 3) : base(parGame, parPosition, parTexture, parHasStates: parHasBlockStates, parCountOfFrames: parCountOfFrames, parCollisionType: BlockCollisionType.Button)
        {

            SetImageLocation("Sprites\\Blocks\\buttonBlock");

            IsInteractible = true;
        }

        /// <summary>
        /// Metoda, ktora sa stara o zapnutie tlacidla.
        /// </summary>
        /// <param name="parTurnOn">Parameter, reprezentujuci ci je tlacidlo zapnute - typ bool.</param>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public void TurnOn(bool parTurnOn, GameTime parGameTime)
        {
            if (Succeded == false)
            {
                if (parTurnOn)
                {
                    SwitchState((int)ButtonBlockStates.On, (float)parGameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    ResetState(true);
                }
            }
            else
            {
                aIsTurnedOn = false;
            }
        }

        /// <summary>
        /// Metoda, ktora sluzi ako pomocny getter, pri fielde - IsTurnedOn.
        /// </summary>
        /// <returns></returns>
        public bool TurnOnGetCheck()
        {
            if (Succeded == true && aIsTurnedOn == false)
            {
                return false;
            }
            else
            { 
                return !TextureIsOnFirstState;
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o prepnutie stavu tlacidla do splneneho, ze uloha spojena s nim bola splnena.
        /// </summary>
        public void ChangeToSuccessState()
        {
            TimerStateChanged = 0;
            SwitchState((int)ButtonBlockStates.Success, 0); //Zmenime State Buttonu, a tym, ze ako cas zadame 0. Tak ostane tento State natrvalo
            Succeded = true;
            aIsTurnedOn = false;
            IsInteractible = false;
        }

        /// <summary>
        /// Metoda, ktora sa stara o prepnutie stavu tlacidla, vyuzite napriklad pri urovni 2.
        /// </summary>
        /// <param name="parStyleNumber">Parameter, reprezentujuci cislo suvisiace so stylom tlacidla - typ int.</param>
        public void SwitchButtonStyle(int parStyleNumber)
        {

           switch (parStyleNumber)
                {
                    case 0:
                        SetImageLocation("Sprites\\Blocks\\buttonBlockRed");
                        break;
                    case 1:
                        SetImageLocation("Sprites\\Blocks\\buttonBlockPurple");
                        break;
                    case 2:
                        SetImageLocation("Sprites\\Blocks\\buttonBlockGreen");
                        break;
                    case 3:
                        SetImageLocation("Sprites\\Blocks\\buttonBlockBlue");
                        break;
                    default:
                        SetImageLocation("Sprites\\Blocks\\buttonBlock");
                        break;
                }
            
        }

        /// <summary>
        /// Metoda, ktora sa stara o interakciu s tlacidlom.
        /// </summary>
        public override void Interact()
        {
            base.Interact();

            if (WantsToInteract == true)
            {
                aIsPressed = true;
                WantsToInteract = false;
            }
        }
    }
}