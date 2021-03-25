using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// None - Nema koliziu, Wall - Cez takyto blok entita neprejde, Zap - Znici Entitu
    /// </summary>
    public enum BlockCollisionType
    {
        None = 0,
        Wall = 1,
        Slow = 2,
        Zap = 3,
        Button = 4
    }


    public class Block : DrawableGameComponent
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
        /// Atribut, reprezentujuci ci ma blok rozne "staty"(eng), napr. inu texturu ked na nom niekto stoji a inu v pravy opak
        /// Atribut nie je potrebne nastavovat na true, v pripade ak uz je nastaveny atribut aIsAnimated na true
        /// </summary>
        private bool aHasStates;

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

        // Atribut, ktory reprezentuje, casovac, ktory pocita cas od kedy sa zmenil stav - textura bloku
        private float aTimerStateChanged = 0F;

        private Color aColor = Color.White;

        private float aVisibility = 1F;

        /// <summary>
        /// Atribut, reprezentuje ci sa textura bloku nachadza v prvom stadiu - v pripade statickeho bloku bude vzdy true
        /// </summary>
        private bool aTextureIsOnFirstState = true;

        /// <summary>
        /// Atribut, reprezentujuci - v akej vyske sa vykresli Block v zavislosti od druhych vykreslenych Spritov - Typ float - Default 1F
        /// </summary>
        private float aLayerDepth = 1F;

        public float LayerDepth { get => aLayerDepth; set => aLayerDepth = value; }

        private bool aIsHidden = false;

        /// <summary>
        /// Atribut, ktory reprezentuje informacie o tom, ci sa jedna o kolizny objekt alebo nie
        /// </summary>
        private BlockCollisionType aBlockCollisionType = BlockCollisionType.None;

        public BlockCollisionType BlockCollisionType
        {
            get => aBlockCollisionType;
            set => aBlockCollisionType = value;
        }

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

        public float TimerStateChanged
        {
            get => aTimerStateChanged; 
            set => aTimerStateChanged = value;
        }
        public bool TextureIsOnFirstState
        {
            get => aTextureIsOnFirstState;
            set => aTextureIsOnFirstState = value;
        }
        public bool IsHidden
        {
            get => aIsHidden;
            set => aIsHidden = value;
        }
        public float Visibility
        {
            get => aVisibility;
            set => aVisibility = value;
        }


        /// <summary>
        /// Trieda reprezentujuca Blok, ktory je specifikovany poziciou a pokial pouzivatel potrebuje, moze hned inicializovat aj Texturu
        /// </summary>
        /// <param name="parGame">Parameter Hry - Typ LogicGame</param>
        /// <param name="parPosition">Parameter Pozicie -Typ Vector2</param>
        /// <param name="parTexture">Volitelny Parameter Textury - Typ Texture2D</param>
        /// <param name="parIsAnimated">Volitelny Parameter - Reprezentujuci ci je blok animovany - Typ Bool</param>
        /// <param name="parHasStates">Volitelny Parameter - Reprezentujuci, ci sa jedna o blok, ktory ma viac "statov"(eng) - stadii, netreba ho nastavovat na true, v pripade ak uz je paramter parIsAnimated true</param>
        /// <param name="parCountOfFrames">Volitelny Parameter - Reprezentujuci pocet framov animacie - Typ Int</param>
        /// <param name="parCollisionType">Volitelny Parameter - Reprezentujuci o aky typ kolizie sa jedna - Typ BlockCollisionType - Enum</param>
        public Block(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null, bool parIsAnimated = false, bool parHasStates = false, int parCountOfFrames = 0, BlockCollisionType parCollisionType = BlockCollisionType.None) : base(parGame)
        {
            aGame = parGame;
            aPosition = parPosition;
            aIsAnimated = parIsAnimated;
            aCountOfFrames = parCountOfFrames;
            aBlockCollisionType = parCollisionType;
            aTimerStateChanged = 0F;
            aTextureIsOnFirstState = true;

            aHasStates = parHasStates;

            if (parIsAnimated == true) //V pripade, ze je blok animovany, je samozrejme ze ma aj urcite stadia
            {
                aHasStates = true;
            }

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

                if ((aIsAnimated == true || aHasStates == true) && aCountOfFrames != 0) //V pripade koretne animovaneho bloku
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
            if (aTexture != null && aIsHidden == false)
            {
                aGame.SpriteBatch.Draw(aTexture, aPosition, aRectangle, aColor * Visibility, 0F, Vector2.Zero, 1F, SpriteEffects.None, aLayerDepth);
            }

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
            if (aHasStates && aTimerStateChanged > 50)
            {
                ResetState(true);
                aTimerStateChanged = 0;
            }
            else if (aHasStates && aTimerStateChanged > 0F)
            {
                aTimerStateChanged += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// Metoda, ktora spravuje posuvanie framov animacie -> Doprava, potom sa otoci, samozrejme uz nebude zobrazovat znova posledny frame ale skoci do stredu ide smerom dolava, tam podobne a znova
        /// </summary>
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
        /// Metoda, ktora sluzi na prepinanie "statov" textur blokov
        /// Zaroven metoda zapocne pocitanie casu od peslednej zmeny textury bloku
        /// </summary>
        /// <param name="parStateNumber">Parameter reprezentujuci </param>
        /// <param name="parTime">Parameter reprezentujuci Ubehnute Milisekundy</param>
        public void SwitchState(int parStateNumber, float parTime)
        {
            aTimerStateChanged = parTime;
            aRectangle.X = (aRectangle.Size.X * parStateNumber - 1) + 1;
            aTextureIsOnFirstState = false;
        }

        /// <summary>
        /// Metoda, ktora sluzi na reset statov bloku, resp posunie suradnicu rectanglu bloku na X - 0
        /// </summary>
        /// <param name="parReset"></param>
        public void ResetState(bool parReset)
        {
            if (parReset)
            {
                aRectangle.X = 0;
                aTimerStateChanged = 0F;
                aTextureIsOnFirstState = true;
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

        /// <summary>
        /// Pomocna Debug Metoda - DEBUG ONLY - Zmazat
        /// </summary>
        /// <param name="par">DEBUG ONLY</param>
        public void ChangeColor(bool par)
        {
            if (aColor == Color.White)
            {
                aColor = Color.Red;
            }
            else
            {
                aColor = Color.White;
            }
        }
    }
}
