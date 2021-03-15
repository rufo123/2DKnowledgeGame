using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private Vector2 aRemotePosition;

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

        /// <summary>
        /// Atribut reprezentujuci Velkost Entity - typ Vector2
        /// </summary>
        private Vector2 aSize;

        private float aRotation = 0F;

        private float aLayerDepth = 0F;

        private SpriteEffects aEffects = SpriteEffects.None;

        /// <summary>
        /// Pocitadlo casu, potrebne k spravnemu vykraslovaniu framov animacie Entity
        /// </summary>
        private double aTimeCounter = 0.0D;

        /// <summary>
        /// Atribut reprezentujuci rychlost entity - typ float
        /// </summary>
        private float aSpeed = 1F;

        private float aVelocity;

        private Vector2 aMovementVector;

        private int aCounterZmazat = 0;

        /// <summary>
        /// Atribut reprezentujuci nasobnu velkost oproti originalu - typ float
        /// </summary>
        private float aEntityScale = 1F;

        private bool aAwaitingMovementMessage = false;

        private bool aIsMoving = false;

        public float Speed { get => aSpeed; set => aSpeed = value; }
        public Color Color { get => aColor; set => aColor = value; }
        public Vector2 Position { get => aPosition; }
        public float EntityScale { get => aEntityScale; set => aEntityScale = value; }
        public bool AwaitingMovementMessage { get => aAwaitingMovementMessage; set => aAwaitingMovementMessage = value; }
        public bool IsMoving { get => aIsMoving; set => aIsMoving = value; }



        public Entity(LogicGame parGame, Vector2 parPosition, Vector2 parSize, Direction parDirection = Direction.UP, float parSpeed = 1F) : base(parGame)
        {
            aLogicGame = parGame;
            aPosition = parPosition;
            aRemotePosition = parPosition;
            aTexture = new Texture2D(parGame.GraphicsDevice, (int)parSize.X, (int)parSize.Y);
            aRectangle = new Rectangle(0, 0, (int)(parSize.X), (int)(parSize.Y));
            if (aColor == null)
            {
                aColor = Color.White;
            }
            aSpeed = parSpeed;

        }

        public void SetDirection(Direction parDirection)
        {
            aDirection = parDirection;
        }

        public void SetImage(string parImage)
        {
            if (!string.IsNullOrEmpty(parImage))
            {
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
        public void SetPosition(float parX, float parY)
        {
            aPosition.X = parX;
            aPosition.Y = parY;
        }

        /// <summary>
        /// Metoda nastavi poziciu na zaklade novo zadaneho 2D Vectora - typ Vector2
        /// </summary>
        /// <param name="parPosition">Parameter reprezentujuci suradnicovy 2D vector - typ Vectro2</param>
        public void SetPosition(Vector2 parPosition)
        {
            aPosition = parPosition;
        }

        /// <summary>
        /// Setter nastavujuci velkost
        /// </summary>
        /// <param name="parSize">Parameter - Velkosti - typ Vector2</param>
        public void SetSize(Vector2 parSize)
        {
            aSize = parSize;
        }

        /// <summary>
        /// Metoda riadiaca pohyb
        /// </summary>
        /// <param name="gameTime"></param>
        public void Move(GameTime gameTime)
        {

            SwitchAnimation(gameTime.ElapsedGameTime.TotalMilliseconds);


            aVelocity = aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (aDirection)
            {
                case Direction.UP:
                    aMovementVector.X = 0;
                    aMovementVector.Y = -1;
                    break;
                case Direction.RIGHT:
                    aMovementVector.X = 1;
                    aMovementVector.Y = 0;
                    break;
                case Direction.DOWN:
                    aMovementVector.X = 0;
                    aMovementVector.Y = 1;
                    break;
                case Direction.LEFT:
                    aMovementVector.X = -1;
                    aMovementVector.Y = 0;
                    break;
                default:
                    aMovementVector.X = 0;
                    aMovementVector.Y = 0;
                    break;
            }

            aPosition += aMovementVector * aVelocity;

            //   Debug.WriteLine("Client Pos - " + aPosition.X + " " + aPosition.Y);}}


        }

        public void SwitchAnimation(double parElapsedTime, int vypis = 0)
        {

            //Prepina, framy korespondujuce zmenam smeru - Y - AXIS
            aRectangle.Y = (int)aDirection * aRectangle.Size.Y;


            if (aTimeCounter >= 0 && aTimeCounter < 100)
            {
                aTimeCounter += parElapsedTime;
                if (vypis == 1)
                {
                    aCounterZmazat++;
                    //Debug.WriteLine("Cudzia " + aCounterZmazat);
                }
                else if (vypis == 0)
                {
                    aCounterZmazat++; 
                    //Debug.WriteLine("Moja " + aCounterZmazat);}}

                }
            }
            else if (aTimeCounter > 100)
            {
                //Prepina, framy korespondujuce zmenam smeru - X - AXIS
                if (aRectangle.X + aRectangle.Size.X <= aRectangle.Size.X * 4)
                {
                    aRectangle.X = aRectangle.X + aRectangle.Size.X;
                }
                else
                {
                    aRectangle.X = 0;
                }
                aTimeCounter = 0;
            }

        }

        /// <summary>
        /// Pripravi data pre odoslanie
        /// </summary>
        /// <param name="parMessage"></param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareDataForUpload(NetOutgoingMessage parMessage)
        {

            if (aAwaitingMovementMessage != true) //Ak bola prijata sprava od servera, moze sa odoslat nova
            {
                Debug.Write("Move False");
                parMessage.Write((byte)PacketMessageType.Movement);
                parMessage.Write((byte)aDirection);
                parMessage.WriteVariableInt32((int)aMovementVector.X);
                parMessage.WriteVariableInt32((int)aMovementVector.Y);
                parMessage.Write(aVelocity);
                parMessage.Write(aIsMoving);
                parMessage.Write(aPosition.X);
                parMessage.Write(aPosition.Y);

                aAwaitingMovementMessage = true;

                return parMessage;
            }
            else //Ak este nebola prijata sprava od servera, nebude sa posielat nic
            {
                return null;
            }


        }

        /// <summary>
        /// Spracuje stiahnute data
        /// </summary>
        /// <param name="parMessage"></param>
        /// <param name="parClientElapsedTime">Reprezentuje cas u klienta, u ktoreho bude toto vsetko vykreslovan</param>
        public void PrepareDownloadedData(NetIncomingMessage parMessage, GameTime parGameTime)
        {
            aDirection = (Direction)parMessage.ReadByte();
            aMovementVector.X = parMessage.ReadVariableInt32();
            aMovementVector.Y = parMessage.ReadVariableInt32();
            aVelocity = parMessage.ReadFloat();
            aIsMoving = parMessage.ReadBoolean();
            aRemotePosition.X = parMessage.ReadFloat();
            aRemotePosition.Y = parMessage.ReadFloat();

            Interpolate(parGameTime);
            SwitchAnimation(parGameTime.ElapsedGameTime.TotalMilliseconds, 1);

        }

        /// <summary>
        /// Nedokoncene - Pokus o Interpolaciu - Zatial Nefunkcne
        /// </summary>
        /// <param name="parGameTime"></param>
        public void Interpolate(GameTime parGameTime)
        {

            float interpolation_constant = 0.5F;
            float treshold = 0.2F;
            float differenceX = aRemotePosition.X - aRemotePosition.X;
            float differenceY = aRemotePosition.Y - aRemotePosition.Y;
            if ((Math.Abs(differenceX) < treshold) && Math.Abs(differenceY) < treshold)
            {
                aPosition = aRemotePosition;
            }
            else
            {
                aPosition.X = differenceX * (float)parGameTime.ElapsedGameTime.TotalSeconds * interpolation_constant;
                aPosition.Y = differenceY * (float)parGameTime.ElapsedGameTime.TotalSeconds * interpolation_constant;
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

        /// <summary>
        /// Overridnuta Metoda LoadContent, ktora nacitava Texturu Entity
        /// </summary>
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

            // aLogicGame.SpriteBatch.Draw(aTexture, aRectangle, Color.White);
            aLogicGame.SpriteBatch.Draw(aTexture, aPosition, aRectangle, aColor, aRotation, Vector2.Zero, aLogicGame.Scale * aEntityScale, aEffects, 0.2F);


            //   aLogicGame.SpriteBatch.Draw(aTexture, aPosition, aRectangle, aColor, aRotation, Vector2.Zero, aLogicGame.Scale, aEffects, aLayerDepth);

            //            aLogicGame.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
