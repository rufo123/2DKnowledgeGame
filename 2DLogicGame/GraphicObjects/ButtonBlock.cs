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

        public bool IsTurnedOn
        {
            get => !TextureIsOnFirstState;
            set => TextureIsOnFirstState = !value;
        }

        public ButtonBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parHasBlockStates = true, int parCountOfFrames = 3) : base(parGame, parPosition, parTexture, parHasStates: parHasBlockStates, parCountOfFrames: parCountOfFrames, parCollisionType: BlockCollisionType.Button)
        {
            SetImageLocation("Sprites\\Blocks\\buttonBlock");
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}