using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Trieda, reprezentujuca implementaciu pohybujuceho sa bloku - nachystana pre moznu buducu funkctionalitu. - Klient.
    /// </summary>
    public class MovableBlock : Block
    {
        /// <summary>
        /// Enumeracna trieda, reprezentujuca smer. - Klient.
        /// </summary>
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

        /// <summary>
        /// Konstruktor bloku.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parTexture">Parametere, reprezentujuci textutu - typ Texture2D.</param>
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
