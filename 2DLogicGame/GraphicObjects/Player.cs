using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{

    

    class Player : Entity
    {
        /// <summary>
        /// Atribut, ktory reprezentuje, ci je Hrac ovladany mnou, alebo sa jedna o spoluhraca
        /// </summary>
        private bool isControlledByMe;

        public Player(int parPlayerID, LogicGame parGame, Vector2 parPosition, Vector2 parSize,  Direction parDirection = Direction.UP, float parSpeed = 200) : base(parGame, parPosition, parSize, parDirection, parSpeed)
        {
            SetImage("Sprites\\Entities\\postavaFrames");
            
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
