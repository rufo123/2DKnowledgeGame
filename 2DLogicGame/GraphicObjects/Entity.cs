using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.GraphicObjects
{
    class Entity : DrawableGameComponent
    {
        public enum Direction
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3
        }

        public enum Rotation
        {
            NONE = 0,
            TO_RIGHT = 1,
            TO_LEFT = -1
        }

        /// <summary>
        /// Atribut reprezentujuci poziciu entity - Typ Vector2
        /// </summary>
        private Vector2 aPosition;

        /// <summary>
        /// Atribut reprezentujuci texturu entity - typ Texture2D
        /// </summary>
        protected Texture2D aTexture;

        /// <summary>
        /// Atribut reprezentujuci oblast entity - typ Rectangle
        /// </summary>
        private Rectangle aRectangle;

        /// <summary>
        /// Atribut reprezentujuci farbu entity - typ Color
        /// </summary>
        private Color aColor;

        /// <summary>
        /// Atribut reprezentujuci smer entity - typ Enum - Direction
        /// </summary>
        private Direction aDirection = Direction.UP;

        /// <summary>
        /// Atribut reprezentujuci hru - Typ LogicGame
        /// </summary>
        protected LogicGame aLogicGame;

        /// <summary>
        /// Nastavi Texturu
        /// </summary>
        private string aImage;

        private float aRotation = 0F;

        private float aLayerDepth = 0F;

        private SpriteEffects aEffects = SpriteEffects.None;

        /// <summary>
        /// Atribut reprezentujuci rychlost entity - typ float
        /// </summary>
        private float aSpeed = 1F;

        public float Speed { get => aSpeed; set => aSpeed = value; }
        public Color Color { get => aColor; set => aColor = value; }
        public Vector2 Position { get => aPosition; }

        public Entity(LogicGame parGame, Vector2 parPosition, Vector2 parSize, Color parColor, Direction parDirection = Direction.UP, float parSpeed = 1F) : base(parGame)
        {
            aLogicGame = parGame;
            aPosition = parPosition;
            aTexture = new Texture2D(parGame.GraphicsDevice, (int)parSize.X, (int)parSize.Y);
            aRectangle = new Rectangle(0,0, (int)(parSize.X * aLogicGame.Scale), (int)(parSize.Y * aLogicGame.Scale));
            aColor = parColor;

        }

        public void SetDirection(Direction parDirection)
        {
            aDirection = parDirection;
        }

        public void SetImage(string parImage) {
            if (!string.IsNullOrEmpty(parImage)) {
                aImage = parImage;
            }
        }

        public Direction GetDirection()
        {
            return aDirection;
        }

        public bool Rotate(Rotation parWhereToRotate)
        {
            int tmpDirection = (int)aDirection;
            int tmpIntRotate = (int)parWhereToRotate;

            int newDirection = tmpDirection + tmpIntRotate;

            if ((newDirection) >= 0 && (newDirection) <= 3) //Ak rotacia nesposobi zmenu na neprijatelne hodnoty
            {
                this.SetDirection((Direction)newDirection);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Metoda nastavi poziciu na zaklade 2 parametrov reprezentujucich suradnicu X a suradnicu Y
        /// </summary>
        /// <param name="parX">Parameter reprezentujuci X-ovu suradnicu - typ float</param>
        /// <param name="parY">Parameter reprezentujuci Y-ovu suradnicu - typ float</param>
        public void SetPosition(float parX, float parY) {
            aPosition.X = parX;
            aPosition.Y = parY;
        }

        /// <summary>
        /// Metoda nastavi poziciu na zaklade novo zadaneho 2D Vectora - typ Vector2
        /// </summary>
        /// <param name="parPosition">Parameter reprezentujuci suradnicovy 2D vector - typ Vectro2</param>
        public void SetPosition(Vector2 parPosition) {
            aPosition = parPosition;
        }

        public void Move(float parTickSpeed) {
            switch (aDirection)
            {
                case Direction.UP:
                    aPosition.Y -= parTickSpeed;
                    break;
                case Direction.RIGHT:
                    aPosition.X += parTickSpeed;
                    break;
                case Direction.DOWN:
                    aPosition.Y += parTickSpeed;
                    break;
                case Direction.LEFT:
                    aPosition.X -= parTickSpeed;
                    break;
                default:
                    break;
            }
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            if (!string.IsNullOrEmpty(aImage))
            {
                aTexture = aLogicGame.Content.Load<Texture2D>(aImage);
            }
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
                aLogicGame.SpriteBatch.Begin();
              // aLogicGame.SpriteBatch.Draw(aTexture, aRectangle, Color.White);
            aLogicGame.SpriteBatch.Draw(aTexture, aPosition, aRectangle, aColor, aRotation, Vector2.Zero, aLogicGame.Scale, aEffects, aLayerDepth);
          

            //   aLogicGame.SpriteBatch.Draw(aTexture, aPosition, aRectangle, aColor, aRotation, Vector2.Zero, aLogicGame.Scale, aEffects, aLayerDepth);

            aLogicGame.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
