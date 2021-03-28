using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide.Blocks_ServerSide
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
        Button = 4,
        Standable = 5
    }

    public class BlockServer
    {
        /// <summary>
        /// Atribut reprezentujuci poziciu bloku - Typ Vector2
        /// </summary>
        protected Vector2 aPosition;

        /// <summary>
        /// Atribut, reprezentujuci velkost bloku
        /// </summary>
        private Vector2 aSize;

        /// <summary>
        /// Atribut reprezentujuci nasobnu velkost bloku oproti originalnej velkosti - typ float
        /// </summary>
        private float aBlockScale = 1F;

        private bool aIsInteractible;

        private bool aWantsToInteract;

        private bool aIsHidden = false;

        private bool aSomethingIsStandingOnTop = false;

        private float aTimerLastStanding = 0F;

        /// <summary>
        /// Atribut, ktory reprezentuje informacie o tom, ci sa jedna o kolizny objekt alebo nie
        /// </summary>
        private BlockCollisionType aBlockCollisionType = BlockCollisionType.None;

        public BlockCollisionType BlockCollisionType
        {
            get => aBlockCollisionType;
            set => aBlockCollisionType = value;
        }

        public bool IsHidden
        {
            get => aIsHidden;
            set => aIsHidden = value;
        }

        public bool IsInteractible
        {
            get => aIsInteractible;
            set => aIsInteractible = value;
        }

        public bool WantsToInteract
        {
            get => aWantsToInteract;
            set => aWantsToInteract = value;
        }
        public bool SomethingIsStandingOnTop
        {
            get => aSomethingIsStandingOnTop;
            set => aSomethingIsStandingOnTop = value;
        }

        /// <summary>
        /// Trieda reprezentujuca Blok, ktory je specifikovany poziciou a pokial pouzivatel potrebuje, moze hned inicializovat aj Texturu
        /// </summary>
        /// <param name="parGame">Parameter Hry - Typ LogicGame</param>
        /// <param name="parPosition">Parameter Pozicie -Typ Vector2</param>
        /// <param name="parTexture">Volitelny Parameter Textury - Typ Texture2D</param>
        /// <param name="parIsAnimated">Volitelny Parameter - Reprezentujuci ci je blok animovany - Typ Bool</param>
        /// <param name="parCountOfFrames">Volitelny Parameter - Reprezentujuci pocet framov animacie - Typ Int</param>
        /// <param name="parCollisionType">Volitelny Parameter - Reprezentujuci o aky typ kolizie sa jedna - Typ BlockCollisionType - Enum</param>
        public BlockServer(Vector2 parPosition, BlockCollisionType parCollisionType = BlockCollisionType.None)
        {
            aPosition = parPosition;
            aBlockCollisionType = parCollisionType;
            aSize = new Vector2(64, 64);

            aIsInteractible = false;

            aWantsToInteract = false;

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
        /// Originalna implementacia metody Interact, vo svojej podstate robi len to, ze nastavi ze chce block interagovat, zavola base a nasledne nastavi, ze blok uz nechce interagovat
        /// </summary>
        public virtual void Interact()
        {
            if (IsInteractible)
            {
                aWantsToInteract = true;
            }
        }


    }
}

