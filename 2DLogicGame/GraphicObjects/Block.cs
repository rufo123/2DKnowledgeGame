using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{


    class Block
    {
        /// <summary>
        /// Atribut reprezentujuci poziciu bloku - Typ Vector2
        /// </summary>
        protected Vector2 aPosition;

        /// <summary>
        /// Atribut reprezentujuci texturu bloku - Typ Texture2D
        /// </summary>
        private Texture2D aTexture;

        /// <summary>
        /// Trieda reprezentujuca Blok, ktory je specifikovany poziciou a pokial pouzivatel potrebuje, moze hned inicializovat aj Texturu
        /// </summary>
        /// <param name="parPosition"></param>
        /// <param name="parTexture"></param>
        public Block(Vector2 parPosition, Texture2D parTexture = null)
        {
            aPosition = parPosition;
            if (parTexture != null)
            {
                aTexture = parTexture;
            }
        }
        /// <summary>
        /// Metoda nastavi Texturu Bloku
        /// </summary>
        /// <param name="parTexture">Parameter reprezentujuci Texturu Bloku - Typ Texture2D</param>
        public void SetTexture(Texture2D parTexture)
        {
            aTexture = parTexture;
        }

        /// <summary>
        /// Metoda vrati Texturu Bloku
        /// </summary>
        /// <returns>Vrati texturu bloku - typ Texture2D</returns>
        public Texture2D GetTexture()
        {
            return aTexture;
        }

        /// <summary>
        /// Metoda nastavi Poziciu Bloku
        /// </summary>
        /// <param name="parPosition">Parameter reprezentujuci poziciu bloku - Typ Vector2</param>
        public void SetPosition(Vector2 parPosition)
        {
            aPosition = parPosition;
        }

        /// <summary>
        /// Metoda vrati Poziciu Bloku
        /// </summary>
        /// <returns>Vrati poziciu bloku - Typ Vector2</returns>
        public Vector2 GetPosition()
        {
            return aPosition;
        }

       
    }
}
