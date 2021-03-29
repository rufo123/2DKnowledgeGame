using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame.ServerSide
{
    public class EntityServer
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

        /// <summary>
        /// Atribut, ktory reprezentuje prednastavenu rychlost Enity - typ float
        /// </summary>
        private float aDefaultSpeed = 0;

        /// <summary>
        /// Atribut, ktory reprezentujuce rychlost Entity - typ float
        /// </summary>
        private float aVelocity;

        /// <summary>
        /// Atribut, ktory reprezentuje movement vektor, resp do ktorej strany sa ma Entita pohnut - typ vector2
        /// </summary>
        private Vector2 aMovementVector;

        /// <summary>
        /// Atribut, ktory reprezentuje boolean o tom ci sa Entita pohybuje alebo nie - typ bool
        /// </summary>
        private bool aIsMoving = false;

        /// <summary>
        /// Atribut, ktory reprezentuje skalu Entity oproti originalnej velkosti - typ float
        /// </summary>
        private float aEntityScale = 1F;

        /// <summary>
        /// Atribut, ktory reprezentuje ci entita narazila na prekazku, resp. nemoze sa pohnut - typ bool
        /// </summary>
        public bool aIsBlocked = false;

        private Vector2 aHitBoxPos;

        private Vector2 aHitBoxSize;

        private bool aWantsToInteract = false;

        private Vector2 aDefaultPosition;



        public float Speed { get => aSpeed; set => aSpeed = value; }
        public Vector2 Position { get => aPosition; }
        public bool IsMoving { get => aIsMoving; set => aIsMoving = value; }
        public Vector2 Size { get => aSize; set => aSize = value; }
        public bool IsBlocked { get => aIsBlocked; set => aIsBlocked = value; }
        public float EntityScale { get => aEntityScale; set => aEntityScale = value; }
        public Vector2 HitBoxPos { get => aHitBoxPos; set => aHitBoxPos = value; }
        public Vector2 HitBoxSize { get => aHitBoxSize; set => aHitBoxSize = value; }
        public bool WantsToInteract { get => aWantsToInteract; set => aWantsToInteract = value; }
        public Vector2 DefaultPosition { get => aDefaultPosition; set => aDefaultPosition = value; }



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
            aDefaultSpeed = aSpeed;
            aHitBoxPos = parPosition;
            aHitBoxSize = parPosition;
            aDefaultPosition = new Vector2(parPosition.X, parPosition.Y);
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
            if (IsMoving == true && IsBlocked == false)
            {
                aPosition.Y += aMovementVector.Y * ((aSpeed / parServerTickRate));
                aPosition.X += aMovementVector.X * ((aSpeed / parServerTickRate));

                Debug.WriteLine("Server Pos: " + aMovementVector.X + " " + aMovementVector.Y);
            }
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

        /// <summary>
        /// Metoda, ktora vrati Vektor2, ktory reprezentuje kde by sa Entita pohla v danom smere v buducnosti
        /// </summary>
        /// <param name="parServerTickRate"></param>
        /// <returns>Vrati Vektor2 - Suradnicu buducej pozicie Entity</returns>
        public Vector2 GetAfterMoveVector2(float parServerTickRate)
        {
            Vector2 tmpNewMovementVector2 = new Vector2(0, 0);

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
                default:
                    throw new ArgumentOutOfRangeException();
            }
            float tmpNewVelocity = aSpeed / parServerTickRate;
            return aPosition + (tmpNewMovementVector2 * tmpNewVelocity); //Zatvorky nemusia byt, pretoze nasobenie ma prednost...
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

            aWantsToInteract = parMessage.ReadBoolean();

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
            parMessage.Write(aWantsToInteract);

            return parMessage;

        }

        public void ReSpawn(bool parRespawn)
        {
            if (parRespawn && aDefaultPosition != null)
            {
                aPosition.X = aDefaultPosition.X;
                aPosition.Y = aDefaultPosition.Y;



                Debug.WriteLine("Default - " + aDefaultPosition.X + " " + aDefaultPosition.Y);
            }
        }



    }
}
