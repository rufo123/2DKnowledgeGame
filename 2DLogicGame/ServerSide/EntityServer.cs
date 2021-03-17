﻿using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ServerSide
{
    class EntityServer
    {
        /// <summary>
        /// Enum reprezentujuci Smer
        /// </summary>
        public enum Direction
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3
        }

        /// <summary>
        /// Enum reprezentujuci Rotaciu
        /// </summary>
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
        private float aSpeed = 200F;

        private float aVelocity;

        private Vector2 aMovementVector;

        private bool aIsMoving = false;


        public float Speed { get => aSpeed; set => aSpeed = value; }
        public Vector2 Position { get => aPosition; }
        public bool IsMoving { get => aIsMoving; set => aIsMoving = value; }



        /// <summary>
        /// Reprezentuje Entitu na strane Servera
        /// </summary>
        /// <param name="parPosition">Parameter - Pozicie - typ Vector2</param>
        /// <param name="parSize">Parameter - Velkosti - typ Vector2</param>
        /// <param name="parDirection">Parameter - Smeru - typ Enum - Direction</param>
        /// <param name="parSpeed">Prameter - Rychlosti - typ Float - Defaultne 1F</param>
        public EntityServer(Vector2 parPosition, Vector2 parSize, Direction parDirection = Direction.UP, float parSpeed = 200F)
        {
            aPosition = parPosition;
            aSize = parSize;
            aSpeed = parSpeed;

            aMovementVector = new Vector2(0, 0);

        }

        /// <summary>
        /// Setter, ktory nastavuje Smer
        /// </summary>
        /// <param name="parDirection">Parameter smeru - typ Enum - Direction</param>
        public void SetDirection(Direction parDirection)
        {
            aDirection = parDirection;
        }

        /// <summary>
        /// Getter, ktory vrati Smer
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Setter nastavujuci velkost pomocou parametra
        /// </summary>
        /// <param name="parSize">Parameter - Velkosti - Typ Vector2</param>
        public void SetSize(Vector2 parSize)
        {
            aSize = parSize;
        }

        /// <summary>
        /// Metoda, ktora zabezpecuje pohyb
        /// Prakticky si sama vypocita Velocity, ktora zavisi od rychlosti a Tick Ratu servera
        /// </summary>
        /// <param name="parServerTickRate">Parameter reprezentujuci serverový Tick Rate - Default 60F</param>
        public void Move(float parServerTickRate)
        {
            if (IsMoving == true)
            {
                aPosition.Y += aMovementVector.Y * ((aSpeed / parServerTickRate));
                aPosition.X += aMovementVector.X * ((aSpeed / parServerTickRate));

                Debug.WriteLine("Server Pos: " + aMovementVector.X + " " + aMovementVector.Y);
            }
        }

        /// <summary>
        /// Metoda, ktora spravuje data, prijate od klienta
        /// </summary>
        /// <param name="parMessage">Parameter prichadzajucej spravy - Typ NetIncommingMessage - Vlastne je to Buffer</param>
        public void HandleReceivedData(NetIncomingMessage parMessage)
        {
            ClientMovementDataType tmpType = (ClientMovementDataType)parMessage.ReadByte();

            aDirection = (Direction)parMessage.ReadByte();
            aMovementVector.X = parMessage.ReadVariableInt32();
            aMovementVector.Y = parMessage.ReadVariableInt32();
            aVelocity = parMessage.ReadFloat();
            aIsMoving = parMessage.ReadBoolean();

            if (tmpType == ClientMovementDataType.Regular) //Ak pridu klasicke data o pohybe nebude sa nic diat s poziciou
            {
                parMessage.ReadFloat();
                parMessage.ReadFloat();
            }
            else if (tmpType == ClientMovementDataType.ErrorCorrect) //Pokial pojde o pokus o korekciu o error pozicia sa nastavi podla klienta
            {
                aPosition.X = parMessage.ReadFloat();
                aPosition.Y = parMessage.ReadFloat();
            }

        }

        /// <summary>
        /// Metoda, ktora pripravi data na odoslanie klientom
        /// </summary>
        /// <param name="parMessage">Parameter prichadzajucej spravy - Typ NetIncommingMessage - Vlastne je to Buffer</param>
        /// <returns></returns>
        public NetOutgoingMessage PrepareDataForUpload(NetOutgoingMessage parMessage)
        {

            parMessage.Write((byte)aDirection);
            parMessage.WriteVariableInt32((int)aMovementVector.X);
            parMessage.WriteVariableInt32((int)aMovementVector.Y);
            parMessage.Write(aVelocity);
            parMessage.Write(IsMoving);
            parMessage.Write(aPosition.X);
            parMessage.Write(aPosition.Y);

            return parMessage;


        }



    }
}
