using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace _2DLogicGame.GraphicObjects
{
    public class InputBlock : Block
    {

        private int aNumber;

        public int Number
        {
            get => aNumber;
            set => aNumber = value;
        }

        private int aMinNumber;

        private int aMaxNumber;

        private bool aSubmitted;

        public bool Submitted
        {
            get => aSubmitted;
            set => aSubmitted = value;
        }

        public InputBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parIsAnimated = false, bool parHasStates = false, int parCountOfFrames = 0, int parMinNumber = 0, int parMaxNumber = 9) : base(parGame, parPosition, parTexture, parIsAnimated, parHasStates, parCountOfFrames, parCollisionType: BlockCollisionType.Wall)
        {
            aNumber = 0;
            aMinNumber = parMinNumber;
            aMaxNumber = parMaxNumber;

            SetImageLocation("Sprites\\Blocks\\mathInputBlock");

            IsInteractible = true;
        }

        /// <summary>
        /// Metoda, ktora prepne cislo, podla toho ci chceme ist dopredu, alebo dozadu
        /// Ak dopredu, pripocita sa
        /// Ak dozadu, pripocita sa
        /// V pripade ze dosiahne maximum, prepne sa na minimum a naopak
        /// </summary>
        /// <param name="parBackwards"></param>
        public void SwitchNumber(bool parBackwards)
        {
            if (parBackwards) //Ak chceme posunut cisla dozadu
            {
                if (aNumber <= aMinNumber)
                {
                    aNumber = aMaxNumber;
                }
                else
                {
                    aNumber--;
                }
            }
            else
            {
                if (aNumber >= aMaxNumber)
                {
                    aNumber = aMinNumber;
                }
                else
                {
                    aNumber++;
                }
            }
        }

        public override void Interact()
        {
            base.Interact();

            if (WantsToInteract)
            {
                SwitchNumber(false);
            }

            WantsToInteract = false;
        }

        /// <summary>
        /// Metoda, ktora potvrdi cislo -> Napr ako vysledok prikladu
        /// </summary>
        public void SubmitNumber()
        {
            aSubmitted = true;
        }

        /// <summary>
        /// Metoda, ktora zrusi vyber cisla a vynuluje ho
        /// </summary>
        public void ResetSubmission()
        {
            aSubmitted = false;
            aNumber = 0;
        }

        public override void Draw(GameTime gameTime)
        {

            float tmpFontScale = 0.5F;

            string tmpNumberString = aNumber.ToString();


            Vector2 tmpMeasuredString = LogicGame.Font.MeasureString(tmpNumberString);

            Vector2 tmpNumberPosition = aPosition;

            if (aNumber > 9)
            {
                tmpNumberPosition.X = aPosition.X - 1;
            }
            else
            {
                tmpNumberPosition.X = aPosition.X + (tmpMeasuredString.X * tmpFontScale) / 2;
            }
            tmpNumberPosition.Y = aPosition.Y - ((tmpMeasuredString.Y / 8) * tmpFontScale);

            LogicGame.SpriteBatch.DrawString(LogicGame.Font, aNumber.ToString(), tmpNumberPosition, Color.GreenYellow, 0F, Vector2.Zero, tmpFontScale, SpriteEffects.None, 0.2F);

            base.Draw(gameTime);

        }
    }
}
