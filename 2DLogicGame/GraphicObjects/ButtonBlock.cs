using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DLogicGame.GraphicObjects
{

    enum ButtonBlockStates
    {
        Off = 0,
        On = 1,
        Success = 2
    }



    class ButtonBlock : Block
    {

        private bool aIsTimerStarted = false;



        public ButtonBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parHasBlockStates = true, int parCountOfFrames = 3) : base(parGame, parPosition, parTexture, parHasStates: parHasBlockStates, parCountOfFrames: parCountOfFrames, parCollisionType: BlockCollisionType.Button)
        {
            SetImageLocation("Sprites\\Blocks\\buttonBlock");

            aIsTimerStarted = false;
        }

        public void TurnOn(bool parTurnOn, GameTime parGameTime)
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

    }
}