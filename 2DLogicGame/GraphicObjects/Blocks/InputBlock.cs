﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Enumeracna trieda, reprezentujuca typ vstupu, ci ide o cisla alebo farby. - Klient.
    /// </summary>
    public enum InputBlockType
    {
        Numbers = 0, //Default
        Colors = 1 //Alternativa
    }

    /// <summary>
    /// Trieda, reprezentuje blok - vstup. - Klient.
    /// </summary>
    public class InputBlock : Block
    {
        /// <summary>
        /// Atribut, reprezentujuci cislo - typ int.
        /// </summary>
        private int aNumber;

        /// <summary>
        /// Atribut, reprezentujuci minimalne mozne dosiahnutelne cislo - typ int.
        /// </summary>
        private int aMinNumber;

        /// <summary>
        /// Atribut, reprezentujuci maximalne mozne dosiahnutelne cislo - typ int.
        /// </summary>
        private int aMaxNumber;

        /// <summary>
        /// Atribut, reprezentujuci ci doslo k odoslaniu odpovede, ktora vyzadovala vstup - typ bool.
        /// </summary>
        private bool aSubmitted;

        /// <summary>
        /// Atribut, reprezentujuci typ vstupu - typ InputBlockType - enum.
        /// </summary>
        private InputBlockType aInputBlockType;

        public int Number
        {
            get => aNumber;
            set => aNumber = value;
        }

        public bool Submitted
        {
            get => aSubmitted;
            set => aSubmitted = value;
        }
        
        /// <summary>
        /// Konstruktor input bloku.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parInputBlockType">Parameter, reprezentujuci typ inputu - typ InputBlockType - enum.</param>
        /// <param name="parTexture">Parameter, reprezentujuci texturu - typ Texture2D.</param>
        /// <param name="parIsAnimated">Parameter, reprezentujuci ci je blok animovany - typ bool.</param>
        /// <param name="parHasStates">Parameter, reprezentujuci ci blok ma viacej stavov - typ bool.</param>
        /// <param name="parCountOfFrames">Parameter, reprezentujuci pocet framov bloku, ak nejake ma - typ int.</param>
        /// <param name="parMinNumber">Parameter, reprezentujuci najmensie dosiahnutelne cislo - typ int.</param>
        /// <param name="parMaxNumber">Parameter, reprezentujuci najvacsie dosiahnutelne cislo - typ int.</param>
        public InputBlock(LogicGame parGame, Vector2 parPosition, InputBlockType parInputBlockType = InputBlockType.Numbers, Texture2D parTexture = null, bool parIsAnimated = false, bool parHasStates = false, int parCountOfFrames = 0, int parMinNumber = 0, int parMaxNumber = 9) : base(parGame, parPosition, parTexture, parIsAnimated, parHasStates, parCountOfFrames, parCollisionType: BlockCollisionType.Wall)
        {
            aInputBlockType = parInputBlockType;

            aNumber = 0;
            aMinNumber = parMinNumber;
            aMaxNumber = parMaxNumber;

            SetImageLocation("Sprites\\Blocks\\mathInputBlock");

            SwitchState(2, 0);

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

        /// <summary>
        /// Metoda, ktora overriduje originalnu, metodu a pripocitava cislo reprezentovane v InputBloku, nasledne nastavi, ze uz nechce nic s blokom interagovat
        /// </summary>
        public override void Interact()
        {
            base.Interact();

            if (WantsToInteract)
            {
                //    SwitchNumber(false); -> Switchovanie prebieha na serveri, napr. v Leveli Math server odosiela cislo .. Cize asi zakomentovat ditto aj s SwitchNumber...
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

        /// <summary>
        /// Metoda, ktora sa stara o zmenu vstuput.
        /// </summary>
        /// <param name="parType"></param>
        public void ChangeInputType(InputBlockType parType)
        {
            aInputBlockType = parType;
        }

        /// <summary>
        /// Metoda, ktora sa stara o aktualizaciu bloku. Pokial ide o blok, ktory ma vstup farbu.
        /// Stara sa o zmenu stavu blokov.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Update(GameTime parGameTime)
        {

            if (aInputBlockType == InputBlockType.Colors)
            {
                switch (aNumber)
                {
                    case 0:
                        SwitchState(0, parGameTime.ElapsedGameTime.Milliseconds);
                        break;
                    case 1:
                        SwitchState(1, parGameTime.ElapsedGameTime.Milliseconds);
                        break;
                    case 2:
                        SwitchState(2, parGameTime.ElapsedGameTime.Milliseconds);
                        break;
                    case 3:
                        SwitchState(3, parGameTime.ElapsedGameTime.Milliseconds);
                        break;
                    case 4:
                        SwitchState(4, parGameTime.ElapsedGameTime.Milliseconds);
                        break;
                }
            }

            base.Update(parGameTime);
        }

        /// <summary>
        /// Metoda, ktora sa stara o vykreslovanie bloku.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Draw(GameTime parGameTime)
        {

            if (aInputBlockType == InputBlockType.Numbers)
            {

                float tmpFontScale = 0.5F;

                string tmpNumberString = aNumber.ToString();


                Vector2 tmpMeasuredString = LogicGame.Font72.MeasureString(tmpNumberString);

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

                LogicGame.SpriteBatch.DrawString(LogicGame.Font72, aNumber.ToString(), tmpNumberPosition, Color.GreenYellow, 0F, Vector2.Zero, tmpFontScale, SpriteEffects.None, 0.2F);

                
            }

            base.Draw(parGameTime);

        }

    }
}
