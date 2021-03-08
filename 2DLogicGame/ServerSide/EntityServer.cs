using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ServerSide
{
    class EntityServer
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
        /// Atribut reprezentujuci smer entity - typ Enum - Direction
        /// </summary>
        private Direction aDirection = Direction.UP;


        /// <summary>
        /// Atribut reprezentujuci Velkost Entity - typ Vector2
        /// </summary>
        private Vector2 aSize;

        /// <summary>
        /// Atribut reprezentujuci rychlost entity - typ float
        /// </summary>
        private float aSpeed = 1F;

        private float aVelocity;

        private Vector2 aMovementVector;

        public float Speed { get => aSpeed; set => aSpeed = value; }
        public Vector2 Position { get => aPosition; }

        public EntityServer(Vector2 parPosition, Vector2 parSize, Direction parDirection = Direction.UP, float parSpeed = 1F)
        {
            aPosition = parPosition;
            aSize = parSize;
            aSpeed = parSpeed;

            aMovementVector = new Vector2(0, 0);

        }

        public void SetDirection(Direction parDirection)
        {
            aDirection = parDirection;
        }
        public Direction GetDirection()
        {
            return aDirection;
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

        public void SetSize(Vector2 parSize)
        {
            aSize = parSize;
        }

        public void Move(float gameTime)
        {


            aVelocity = aSpeed * (float)gameTime;

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


        }

        public void HandleReceivedData(NetIncomingMessage parMessage)
        {

            aDirection = (Direction)parMessage.ReadByte();
            aMovementVector.X = parMessage.ReadVariableInt32();
            aMovementVector.Y = parMessage.ReadVariableInt32();
            aVelocity = parMessage.ReadFloat();
            aPosition.X = parMessage.ReadFloat();
            aPosition.Y = parMessage.ReadFloat();
          //  Debug.WriteLine(aPosition.X + " " + aPosition.Y);
        }

        public NetOutgoingMessage PrepareDataForUpload(NetOutgoingMessage parMessage)
        {
            
            parMessage.Write((byte)aDirection);
            parMessage.WriteVariableInt32((int)aMovementVector.X);
            parMessage.WriteVariableInt32((int)aMovementVector.Y);
            parMessage.Write(aVelocity);
            parMessage.Write(aPosition.X);
            parMessage.Write(aPosition.Y);

            return parMessage;

            
        }



    }
}
