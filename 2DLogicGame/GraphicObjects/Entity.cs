using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Trieda, ktora reprezentuje Entitu. - Klient.
    /// </summary>
    public class Entity : DrawableGameComponent
    {
        /// <summary>
        /// Enumeracna trieda, reprezentujuca smer. - Klient.
        /// </summary>
        public enum Direction
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3
        }

        /// <summary>
        /// Enumeracna trieda, reprezentujuca rotaciu. - Klient.
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

        /// <summary>
        /// Atribut. reprezentujuci Rotaciu Entity - typ float
        /// </summary>
        private float aRotation = 0F;

        /// <summary>
        /// Atribut, reprezentujuci hlbku aktualneho Layera, v tomto pripade Entity - typ float
        /// </summary>
        private float aLayerDepth = 0F;

        /// <summary>
        /// Atribut, reprezentujuci Sprite Effecty - Defaultne ziadne - typ SpriteEffects
        /// </summary>
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

        /// <summary>
        /// Atribut, reprezentujuci rychlost Entity - typ float
        /// </summary>
        private float aVelocity;

        /// <summary>
        /// Atribut, reprezentujuci Movement Vector Entity - typ Vector2
        /// </summary>
        private Vector2 aMovementVector;

        /// <summary>
        /// Atribut, reprezentujuci ci je entita zablokovaná, teda napr narazila do steny - typ bool
        /// </summary>
        private bool aIsBlocked = false;

        /// <summary>
        /// Atribut, reprezentujuci prednastavenu poziciu Entity, kde sa objavi po znovuzrodeni - typ Vector2
        /// </summary>
        private Vector2 aDefaultPosition;

        /// <summary>
        /// Atribut, ktory reprezentuje ci chce Entita interagovat s nejakym objektom - typ bool
        /// </summary>
        private bool aWantsToInteract = false;

        /// <summary>
        /// Atribut, ktory reprezentuje, ci entita vyzaduje Respawn - typ bool
        /// </summary>
        private bool aEntityNeedsReSpawn;

        /// <summary>
        /// Atribut reprezentujuci nasobnu velkost oproti originalu - typ float
        /// </summary>
        private float aEntityScale = 1F;

        /// <summary>
        /// Atribut, reprezentujuci ci entita ocakava spravy o pohybe - typ bool.
        /// </summary>
        private bool aAwaitingMovementMessage = false;

        /// <summary>
        /// Atribut, reprezentujuci ci sa entita snazi pohnut - typ bool.
        /// </summary>
        private bool aIsTryingToMove = false;

        /// <summary>
        /// Atribut, reprezentujuci ci doslo k zisteniu chybnych dat o pohybe - typ bool.
        /// </summary>
        private bool aMovingDataErrored = false;

        /// <summary>
        /// Atribut, reprezentujuci ci entita potrebuje korekciu pozicie - typ bool.
        /// </summary>
        private bool aEntityNeedsPosCorrect = false;

        /// <summary>
        /// Atribut, reprezentujuci ci entita potrebuje jednoduchu interpolaciu - typ bool.
        /// </summary>
        private bool aEntityNeedsInterpolation = false;

        public bool WantsToInteract
        {
            get => aWantsToInteract;
            set => aWantsToInteract = value;
        }
        public Vector2 DefaultPosition
        {
            get => aDefaultPosition;
            set => aDefaultPosition = value;
        }
        public bool EntityNeedsRespawn
        {
            get => aEntityNeedsReSpawn;
            set => aEntityNeedsReSpawn = value;
        }

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
        public bool IsBlocked { get => aIsBlocked; set => aIsBlocked = value; }




        /// <summary>
        /// Konstruktor Entity.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parSize">Parameter, reprezentujuci velkost - typ Vector2.</param>
        /// <param name="parDirection">Parameter, reprezentujuci smer - typ Direction - enum.</param>
        /// <param name="parSpeed">Parameter, reprezentujuci rychlost - typ float.</param>
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

            aDefaultPosition = parPosition;

            aEntityNeedsReSpawn = false;

        }

        /// <summary>
        /// Metoda, ktora nstavi
        /// </summary>
        /// <param name="parDirection"></param>
        public void SetDirection(Direction parDirection)
        {
            aDirection = parDirection;
        }

        /// <summary>
        /// Metoda, ktora nastavi obrazok pre Entitu
        /// </summary>
        /// <param name="parImage"></param>
        public void SetImage(string parImage)
        {
            if (!string.IsNullOrEmpty(parImage))
            {
                aImage = parImage;
            }
        }

        /// <summary>
        /// Metoda, ktora vrati Smer Entity
        /// </summary>
        /// <returns>Vrati Direction - podla toho na ktory smer sa Entita pozera</returns>
        public Direction GetDirection()
        {
            return aDirection;
        }

        /// <summary>
        /// Metoda, ktora otoci Entitu
        /// </summary>
        /// <param name="parWhereToRotate"></param>
        /// <returns>Vrati true ak dojde k otoceniu, inak false</returns>
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

            if (aIsBlocked == false)
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

        /// <summary>
        /// Metoda, ktora sluzi na prepocitanie suradnic, framov textury Entity
        /// </summary>
        /// <param name="parElapsedTime">Parameter, reprezentujuci ubehnuty cas - typ double</param>
        /// <param name="parZmazaVypis">ZMAZAT DEBUG</param>
        public void SwitchAnimation(double parElapsedTime, int parZmazaVypis = 0)
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
                    if (parZmazaVypis == 1)
                    {
                        //Debug.WriteLine("Cudzia " + aCounterZmazat);
                    }
                    else if (parZmazaVypis == 0)
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
                parMessage.Write(aWantsToInteract);

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

                aWantsToInteract = parMessage.ReadBoolean();

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

        /// <summary>
        /// Taka velmi jednoducha forma korekcia suradnic.. s podporou pripustnej chyby - vyuzita pri klientovi
        /// </summary>
        /// <param name="parGameTime"></param>
        public void InterpolationErrorCheck()
        {
            float tmpAllowedErrorOffset = 50F; //Znamena - 

            if (Math.Abs(aPosition.X - aRemotePosition.X) >= tmpAllowedErrorOffset || (Math.Abs(aPosition.Y - aRemotePosition.Y) >= tmpAllowedErrorOffset))
            {
                aPosition = aRemotePosition;
                aEntityNeedsInterpolation = false;
            }
        }


        /// <summary>
        /// Metoda, ktora sluzi na detegovanie chyby v svojich datach Entity stiahnutych od servera
        /// </summary>
        /// <param name="parMessage">Parameter spravy - typ NetIncomingMessage - buffer</param>
        /// <returns></returns>
        public bool DownloadedDataErrorDetection(NetIncomingMessage parMessage)
        {
            Direction tmpDir = (Direction)parMessage.ReadByte();
            float tmpMovX = parMessage.ReadVariableInt32();
            float tmpMovY = parMessage.ReadVariableInt32();
            float tmpVel = parMessage.ReadFloat();
            bool tmpDownMov = parMessage.ReadBoolean();

            aRemotePosition.X = parMessage.ReadFloat();
            aRemotePosition.Y = parMessage.ReadFloat();

            bool tmpInteract = parMessage.ReadBoolean();

            if (aRemotePosition != aPosition) //Ak suradnice nie su rovnake, zavola sa pokus o interpolaciu - Kedze ide o Klientom ovladanu Entitu, kvoli jemnosti...
            {
                InterpolationErrorCheck(); //Odosleme aby doslo k takej jednoduchej interpolacii, skontroluje sa ci suradnice su rovnake, ak nie zmeni sa pozicia v pripade daneho offsetu chyby
            }
            //|| tmpInteract != aWantsToInteract
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

        /// <summary>
        /// Metoda, ktora vrati Entitu, na prednastavenu poziciu
        /// <param name="parDoZap">Parameter, reprezentujuci ci ma Entitu ReSpawnut alebo nie</param>
        /// </summary>
        public void ReSpawn(bool parDoZap, GameTime parGameTime)
        {
            if (parDoZap == true)
            {
                if (aDefaultPosition != null)
                {
                    aPosition = aDefaultPosition;
                    // PositionCorrection(parGameTime); //Kedze ide o respawn zavolame si aj PositionCorrect -> Vyziadame si od servera aby skontroloval ci nase suradnice su spravne
                }
            }

        }

        
        /// <summary>
        /// Override Metoda, ktora sa stara o aktualizovanie dat entity. Ci entita nepotrebuje korekciu pozicie alebo znovuzrodenie sa.
        /// </summary>
        /// <param name="parGameTime"></param>
        public override void Update(GameTime parGameTime)
        {
            if (aEntityNeedsPosCorrect == true)
            {
                PositionCorrection(parGameTime);
            }

            if (aEntityNeedsReSpawn)
            {
                ReSpawn(true, parGameTime); //True - Entitu Respawneme, a posleme jej aj gameTime
                aEntityNeedsReSpawn = false;
            }


            base.Update(parGameTime);
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
