﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{


    class Block : DrawableGameComponent
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
        /// Atribut reprezentujuci oblast bloku - Typ Rectangle
        /// </summary>
        private Rectangle aRectangle;

        /// <summary>
        /// Atribut, reprezentujuci ci sa jedna o animovany blok alebo nie
        /// </summary>
        private bool aIsAnimated;

        /// <summary>
        /// Atribut, reprezentujuci pocet framov animacie
        /// </summary>
        private int aCountOfFrames;

        /// <summary>
        /// Atribut, reprezentujuci velkost bloku
        /// </summary>
        private Vector2 aSize;

        /// <summary>
        /// Atribut, reprezentujuci hru
        /// </summary>
        private LogicGame aGame;

        /// <summary>
        /// Atribut, obsahujuci umiestnenie obrazka textury
        /// </summary>
        private string aImageLocation;

        /// <summary>
        /// Atribut reprezentujuci nasobnu velkost bloku oproti originalnej velkosti - typ float
        /// </summary>
        private float aBlockScale = 1F;

        /// <summary>
        /// Atribut, ktory sluzi na spravne nacasovanie animacie
        /// </summary>
        private float aAnimationTimer = 0;

        /// <summary>
        /// Atribut, ktory reprezentuje, ci framy animacie sa maju posuvat doprava
        /// </summary>
        private bool aAnimateForwards = true;

        public string ImageLocation
        {
            get => aImageLocation;
        }

        public bool IsAnimated
        {
            get => aIsAnimated;
            set => aIsAnimated = value;
        }

        public int CountOfFrames
        {
            get => aCountOfFrames;
            set => aCountOfFrames = value;
        }




        /// <summary>
        /// Trieda reprezentujuca Blok, ktory je specifikovany poziciou a pokial pouzivatel potrebuje, moze hned inicializovat aj Texturu
        /// </summary>
        /// <param name="parGame">Atribut Hry - Typ LogicGame</param>
        /// <param name="parPosition">Atribut Pozicie -Typ Vector2</param>
        /// <param name="parTexture">Atribut Textury - Typ Texture2D</param>
        public Block(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parIsAnimated = false, int parCountOfFrames = 0) : base(parGame)
        {
            aGame = parGame;
            aPosition = parPosition;
            aIsAnimated = parIsAnimated;
            aCountOfFrames = parCountOfFrames;

            if (parTexture != null)
            {
                aTexture = parTexture;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {

            if (!string.IsNullOrEmpty(aImageLocation))
            {
                aTexture = aGame.Content.Load<Texture2D>(aImageLocation);

                if (aIsAnimated == true && aCountOfFrames != 0) //V pripade koretne animovaneho bloku
                {
                    int tmpTextureFrameWidth = aTexture.Width / aCountOfFrames;
                    int tmpTextureFrameHeight = aTexture.Height;

                    aSize = new Vector2(tmpTextureFrameWidth, tmpTextureFrameHeight);

                    aRectangle = new Rectangle(0, 0, tmpTextureFrameWidth, tmpTextureFrameHeight);
                }
                else //V pripade neanimovaneho bloku
                {
                    aSize = new Vector2(aTexture.Width, aTexture.Height);

                    aRectangle = new Rectangle(0, 0, aTexture.Width, aTexture.Height);
                }

            }

            base.LoadContent();
        }

        /// <summary>
        /// Metoda, ktora vykresli blok
        /// </summary>
        /// <param name="gameTime">Parameter reprezentujuci cas hry - typ GameTime</param>
        public override void Draw(GameTime gameTime)
        {
            if (aTexture != null )
            {
                aGame.SpriteBatch.Draw(aTexture, aPosition, aRectangle, Color.White, 0F, Vector2.Zero, aGame.Scale * aBlockScale, SpriteEffects.None, 1F);
            }
            else
            
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {

            if (aTexture != null && aIsAnimated == true && aCountOfFrames > 1) //Prepinanie medzi framami animacie
            {
                if (aAnimationTimer >= 0 && aAnimationTimer < 500)
                {
                    aAnimationTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                else
                {
                    SwitchAnimation();
                    aAnimationTimer = 0;
                }
            }

            base.Update(gameTime);
        }

        public void SwitchAnimation()
        {
            if (aAnimateForwards && aRectangle.X + aRectangle.Size.X <= aRectangle.Size.X * (aCountOfFrames - 1)) //Ak mozeme posuvat animaciu dopredu - su tam este framy
            {
                aRectangle.X += aRectangle.Size.X;
            }
            else if (aAnimateForwards == false && aRectangle.X - aRectangle.Size.X >= 0) //Podobne ak mozeme posuvat dozadu - su tam este framy
            {
                aRectangle.X -= aRectangle.Size.X;
            }
            else //V Else vetve by bez dalsich podmienok, teda nebola vykonana animacia
            {
                if (aAnimateForwards) //Preto vieme, ze ak bolo nastavene vykonavanie animacie - dopredu, posunieme ju o jeden frame dozadu
                {
                    aRectangle.X -= aRectangle.Size.X;
                }
                else //Podobne vieme, ze ak bolo nastavene vykonavanie animacie - dozadu, posunieme ju o jeden frame vpred
                {
                    aRectangle.X += aRectangle.Size.X;
                }

                aAnimateForwards = !aAnimateForwards; //Switch Animacie - otocenie do druhej strany
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

        /// <summary>
        /// Metoda, ktora nastavi umiestnenie obrazka
        /// </summary>
        /// <param name="parImageLocation">Parameter Umiestnenia Obrazka - Typ String</param>
        public void SetImageLocation(string parImageLocation)
        {
            aImageLocation = parImageLocation;

        }


    }
}