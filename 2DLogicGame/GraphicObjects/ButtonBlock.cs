using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{

    public enum ButtonBlockStates
    {
        Off = 0,
        On = 1,
        Success = 2
    }


    public class ButtonBlock : Block
    {

        private bool aIsTurnedOn;

        private bool aIsPressed;

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

        public ButtonBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parHasBlockStates = true, int parCountOfFrames = 3) : base(parGame, parPosition, parTexture, parHasStates: parHasBlockStates, parCountOfFrames: parCountOfFrames, parCollisionType: BlockCollisionType.Button)
        {

            SetImageLocation("Sprites\\Blocks\\buttonBlock");

            IsInteractible = true;
        }

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

        public void ChangeToSuccessState()
        {
            TimerStateChanged = 0;
            SwitchState((int)ButtonBlockStates.Success, 0); //Zmenime State Buttonu, a tym, ze ako cas zadame 0. Tak ostane tento State natrvalo
            Succeded = true;
            aIsTurnedOn = false;
            IsInteractible = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

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