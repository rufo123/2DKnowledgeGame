using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2DLogicGame.GraphicObjects
{
    public class EndBlock : Block
    {
        public EndBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null) : base(parGame, parPosition, parTexture, parCollisionType: BlockCollisionType.Standable)
        {
            SetImageLocation("Sprites\\Blocks\\endBlock");
            IsInteractible = true;
        }

        public override void Update(GameTime gameTime)
        {
            this.ChangeColor(false, this.EntityIsStandingOnTop ? Color.Orange : Color.White);

            base.Update(gameTime);
        }
    }
}
