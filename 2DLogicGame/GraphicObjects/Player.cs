using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{
    class Player : Entity
    {
        public Player(int parPlayerID, LogicGame parGame, Vector2 parPosition, Vector2 parSize, Color parColor, Direction parDirection = Direction.UP, float parSpeed = 1) : base(parGame, parPosition, parSize, parColor, parDirection, parSpeed)
        {
            SetImage("Sprites\\Entities\\postava");
        }

    }
}
