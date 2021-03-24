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

        /// <summary>
        /// Atribut reprezentujuci prednastavenu rychlost entity - typ float
        /// </summary>
        private float aDefaultSpeed = 0;

        private float aVelocity;

        private Vector2 aMovementVector;

        private bool isBlocked = false;

        /// <summary>
        /// Atribut reprezentujuci nasobnu velkost oproti originalu - typ float
        /// </summary>
        private float aEntityScale = 1F;

        private bool aAwaitingMovementMessage = false;

        private bool aIsTryingToMove = false;

        private bool aMovingDataErrored = false;

        private bool aEntityNeedsPosCorrect = false;

        private bool aEntityNeedsInterpolation = false;


        // Confirm
        // Atributy
        // Od servera

        public Vector2 Size { get => aSize; }
        public float Speed { get => aSpeed; set => aSpeed = value; }
        public Color Color { get => aColor; set => aColor = value; }
        public Vector2 Position { get => aPosition; }
        public float EntityScale { get => aEntityScale; set => aEntityScale = value; }
        public bool AwaitingMovementMessage { get => aAwaitingMovementMessage; set => aAwaitingMovementMessage = value; }
        public bool IsTryingToMove { get => aIsTryingToMove; set => aIsTryingToMove = value; }
        public Vector2 MovementVector { get => aMovementVector; set => aMovementVector = value; }

        public bool IsBlocked { get => isBlocked; set => isBlocked = value; }




        public Entity(LogicGame parGame, Vector2 parPosition, Vector2 parSize, Direction parDirection = Direction.UP, float parSpeed = 1F) : base(parGame)
        {
            aLogicGame = parGame;
            aPosition = parPosition;
            aRemotePosition = parPosition;
            aSize = parSize;
            aTexture = new Texture2D(parGame.GraphicsDevice, (int)parSize.X, (int)parSize.Y);
            aRectangle = new Rectangle(0, 0, (int)(parSize.X), (int)(parSize.Y));
            if (aColor == null)
            {
                aColor = Color.White;
            }
            aSpeed = parSpeed;
            aDefaultSpeed = aSpeed;

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

            if (aIsTryingToMove)
            {

                SwitchAnimation(gameTime.ElapsedGameTime.TotalMilliseconds);

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
            }
            else
            {
                aMovementVector.X = 0;
                aMovementVector.Y = 0;
            }

            if (isBlocked == false)
            {
                aVelocity = aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                aPosition += aMovementVector * aVelocity;

            }
            //   Debug.WriteLine("Client Pos - " + aPosition.X + " " + aPosition.Y);}}


        }


        /// <summary>
        /// Metoda, ktora spomali entitu o polovicu - Zalezi od atributu aDefualtSpeed
        /// </summary>
        /// <param name="parSlowDown">Parameter ci ma Entita spomalit - Typ Boolean</param>
        public void SlowDown(bool parSlowDown)
        {
            if (parSlowDown) //Ak ma Entit spomalit
            {
                aSpeed = aDefaultSpeed / 1.5F;
            }
            else //Ak nema spomalit
            {
                aSpeed = aDefaultSpeed;
            }
        }

        public void SwitchAnimation(double parElapsedTime, int vypis = 0)
        {

            //Prepina, framy korespondujuce zmenam smeru - Y - AXIS
            aRectangle.Y = (int)aDirection * aRectangle.Size.Y;

            int tmpTimeThreshold = 100;


            //Spomalenie/Zrychlenie Animacie v pripade zmeny rychlosti
            if (Math.Abs(aDefaultSpeed - aSpeed) > 0.5) //Pri porovnavani hodnot - float musime brat do uvahy aj urcitu toleranciu rozdielu.. Vlastne porovnavame ci sa hodnoty nerovanaju ALE s toleranciou
            {
                tmpTimeThreshold = (int)(tmpTimeThreshold * (aDefaultSpeed / aSpeed));
            }


            if (IsBlocked == false) //Ak entita narazila na barieru, nebude sa prepinat animacia pohybu..
            {

                if (aTimeCounter >= 0 && aTimeCounter < tmpTimeThreshold)
                {
                    aTimeCounter += parElapsedTime;
                    if (vypis == 1)
                    {
                        //Debug.WriteLine("Cudzia " + aCounterZmazat);
                    }
                    else if (vypis == 0)
                    {
                        //Debug.WriteLine("Moja " + aCounterZmazat);}}

                    }
                }
                else if (aTimeCounter > tmpTimeThreshold)
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

                if (aMovingDataErrored != true)
                {
                    parMessage.Write((byte)ClientMovementDataType.Regular);
                }
                else
                {
                    parMessage.Write((byte)ClientMovementDataType.ErrorCorrect);
                    aMovingDataErrored = false;
                }

                parMessage.Write((byte)aDirection);
                parMessage.WriteVariableInt32((int)aMovementVector.X);
                parMessage.WriteVariableInt32((int)aMovementVector.Y);
                parMessage.Write(aVelocity);
                parMessage.Write(aIsTryingToMove);
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
        /// <param name="parMessage">Parameter, reprezentuje prichadzajuci Buffer</param>
        /// <param name="parGameTime">>Reprezentuje cas u klienta, u ktoreho bude toto vsetko vykreslovane</param>
        /// <param name="parIsControlledByClient">Reprezentuje ci je je Entita kontrolovana Klientom</param>
        /// <returns></returns>
        public bool PrepareDownloadedData(NetIncomingMessage parMessage, GameTime parGameTime, bool parIsControlledByClient = false)
        {
            if (parIsControlledByClient == false)
            {
                aDirection = (Direction)parMessage.ReadByte();
                aMovementVector.X = parMessage.ReadVariableInt32();
                aMovementVector.Y = parMessage.ReadVariableInt32();
                aVelocity = parMessage.ReadFloat();
                aIsTryingToMove = parMessage.ReadBoolean();

                aRemotePosition.X = parMessage.ReadFloat();
                aRemotePosition.Y = parMessage.ReadFloat();

                if (aRemotePosition != aPosition) //Zavola sa korekcia pozicie
                {
                    aEntityNeedsPosCorrect = true;

                }

                PositionCorrection(parGameTime);
                SwitchAnimation(parGameTime.ElapsedGameTime.TotalMilliseconds, 1);

                return true; //V tomto pripade pojde o spoluhraca... tu sa neda co kontrolovat ....
            }
            else
            {
                return DownloadedDataErrorDetection(parMessage);
            }
        }

        /// <summary>
        /// Taka velmi jednoducha forma korekcia suradnic.. ak sa prijate suradnice zo servera nerovnaju proste sa prenastavia
        /// </summary>
        /// <param name="parGameTime"></param>
        public void PositionCorrection(GameTime parGameTime)
        {
            if (aPosition != aRemotePosition)
            {
                aPosition = aRemotePosition;
                aEntityNeedsPosCorrect = false;
            }
        }

        //Nakoniec INterpolacia Scrapped


        public bool DownloadedDataErrorDetection(NetIncomingMessage parMessage)
        {
            Direction tmpDir = (Direction)parMessage.ReadByte();
            float tmpMovX = parMessage.ReadVariableInt32();
            float tmpMovY = parMessage.ReadVariableInt32();
            float tmpVel = parMessage.ReadFloat();
            bool tmpDownMov = parMessage.ReadBoolean();

            aRemotePosition.X = parMessage.ReadFloat();
            aRemotePosition.Y = parMessage.ReadFloat();

            if (aRemotePosition != aPosition) //Ak suradnice nie su rovnake, zavola sa pokus o interpolaciu - Kedze ide o Klientom ovladanu Entitu, kvoli jemnosti...
            {
                aEntityNeedsInterpolation = true; //Zatial nevyuzite
            }

            if (tmpDownMov != aIsTryingToMove || tmpDir != aDirection)
            {
                aMovingDataErrored = true;
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Metoda, ktora vrati vypocitanu, buducu poziciu hraca, v pripade, ze by nedoslo ku kolizii
        /// </summary>
        /// <returns></returns>
        public Vector2 GetAfterMoveVector2(GameTime parGameTime)
        {
            Vector2 tmpNewMovementVector2 = new Vector2();

            switch (aDirection)
            {
                case Direction.UP:
                    tmpNewMovementVector2.X = 0;
                    tmpNewMovementVector2.Y = -1;
                    break;
                case Direction.RIGHT:
                    tmpNewMovementVector2.X = 1;
                    tmpNewMovementVector2.Y = 0;
                    break;
                case Direction.DOWN:
                    tmpNewMovementVector2.X = 0;
                    tmpNewMovementVector2.Y = 1;
                    break;
                case Direction.LEFT:
                    tmpNewMovementVector2.X = -1;
                    tmpNewMovementVector2.Y = 0;
                    break;
            }

            double tmpNewVelocity = aSpeed * parGameTime.ElapsedGameTime.TotalSeconds;
            return aPosition + (tmpNewMovementVector2 * aVelocity); //Zatvorky nemusia byt, pretoze nasobenie ma prednost...
        }



        public override void Update(GameTime gameTime)
        {
            if (aEntityNeedsPosCorrect == true)
            {
                PositionCorrection(gameTime);
            }


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
            aLogicGame.SpriteBatch.Draw(aTexture, aPosition, aRectangle, aColor, aRotation, Vector2.Zero, aEntityScale, aEffects, 0.2F);


            //   aLogicGame.SpriteBatch.Draw(aTexture, aPosition, aRectangle, aColor, aRotation, Vector2.Zero, aLogicGame.Scale, aEffects, aLayerDepth);

            //            aLogicGame.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
