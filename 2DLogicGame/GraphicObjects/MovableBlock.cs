using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{
    public class MovableBlock : Block
    {
        public enum Direction
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3
        }

        /// <summary>
        /// Atribut reprezentujuci, kde sa ma "blok pohnut" - resp Smer - Typ - Enum - BlockMove 
        /// </summary>
        private Direction aDirection = Direction.UP;

        public MovableBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null) : base(parGame, parPosition, parTexture)
        {
        }

        /// <summary>
        /// Metoda nastavi Smer, ktorym sa ma "Blok pozerat"
        /// </summary>
        /// <param name="parDirection"></param>
        public void SetDirection(Direction parDirection)
        {
            aDirection = parDirection;
        }

        /// <summary>
        /// Metoda, ktora "hybe" blokom urcenou rychlostou
        /// </summary>
        /// <param name="parSpeed">Parameter reprezentujuci rychlost - Typ float</param>
        public void Move(float parSpeed)
        {
            switch (aDirection)
            {
                case Direction.UP:
                    aPosition.Y -= parSpeed;
                    break;
                case Direction.RIGHT:
                    aPosition.X += parSpeed;
                    break;
                case Direction.DOWN:
                    aPosition.Y += parSpeed;
                    break;
                case Direction.LEFT:
                    aPosition.X -= parSpeed;
                    break;
                default:
                    break;
            }
        }
    }
}
