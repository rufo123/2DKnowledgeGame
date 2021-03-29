using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame.ServerSide
{
    /// <summary>
    /// Trieda reprezentuje 2-rozmerný Vektor
    /// </summary>
    public class Vector2 : IEquatable<Vector2>
    {

        private float aVectorX;

        private float aVectorY;

        public Vector2(float parSurX, float parSurY)
        {
            aVectorX = parSurX;
            aVectorY = parSurY;
        }

        public float X
        {
            get => aVectorX;
            set => aVectorX = value;
        }

        public float Y
        {
            get => aVectorY;
            set => aVectorY = value;
        }

        //Vygenerovane
        public override bool Equals(object obj)
        {
            return Equals(obj as Vector2);
        }

        //Vygenerovane - Poznamka. Kod musel byt prerobeny aby fungoval aj s NULL -> Vyuzite pri operaciach != a ==
        public bool Equals(Vector2 other)
        {
            return (X == other.X) && (Y == other.Y);
        }

        //Vygenerovane
        public override int GetHashCode()
        {
            return HashCode.Combine(aVectorX, aVectorY, X, Y);
        }

        /// <summary>
        /// Operator Scitavania Dvoch Vektorov
        /// </summary>
        /// <param name="parHodn1">Lavy parameter - Typ Vector2</param>
        /// <param name="parHodn2">Pravy parameter - Typ Vector2</param>
        /// <returns>Vrati Vector - Vysledok Scitania</returns>
        public static Vector2 operator +(Vector2 parHodn1, Vector2 parHodn2)
        {

            float tmpSumX = parHodn1.X + parHodn2.X;
            float tmpSumY = parHodn1.Y + parHodn2.Y;
            return new Vector2(tmpSumX, tmpSumY);

        }

        /// <summary>
        /// Operator Odcitania Dvoch Vektorov
        /// </summary>
        /// <param name="parHodn1">Lavy parameter - Typ Vector2</param>
        /// <param name="parHodn2">Pravy parameter - Typ Vector2</param>
        /// <returns>Vrati Vector - Vysledok Odcitania</returns>
        public static Vector2 operator -(Vector2 parHodn1, Vector2 parHodn2)
        {
            float tmpSubX = parHodn1.X - parHodn2.X;
            float tmpSubY = parHodn1.Y - parHodn2.Y;
            return new Vector2(tmpSubX, tmpSubY);
        }

        /// <summary>
        /// Operator Nasobenia Dvoch Vektorov
        /// </summary>
        /// <param name="parHodn1">Lavy parameter - Typ Vector2</param>
        /// <param name="parHodn2">Pravy parameter - Typ Vector2</param>
        /// <returns>Vrati Vector - Vysledok Nasobenia</returns>
        public static Vector2 operator *(Vector2 parHodn1, Vector2 parHodn2)
        {
            float tmpMultX = parHodn1.X * parHodn2.X;
            float tmpMultY = parHodn1.Y * parHodn2.Y;
            return new Vector2(tmpMultX, tmpMultY);
        }



        /// <summary>
        /// Operator Nasobenia ScaleFactor a Vektor
        /// </summary>
        /// <param name="parScaleFactor">Scale Factor - Typ float</param>
        /// <param name="parHodn2">Pravy parameter - Typ Vector2</param>
        /// <returns>Vrati Vector - Vysledok Scitania</returns>
        public static Vector2 operator *(float parScaleFactor, Vector2 parHodn2)
        {
            float tmpMultScaleX = parScaleFactor * parHodn2.X;
            float tmpMulScaleY = parScaleFactor * parHodn2.Y;
            return new Vector2(tmpMultScaleX, tmpMulScaleY);
        }

        /// <summary>
        /// Operator Nasobenia Vektor a ScaleFactor
        /// </summary>
        /// <param name="parHodn1">Lavy parameter - Typ Vector2</param>
        /// <param name="parScaleFactor">Scale Factor - Typ float</param>
        /// <returns>Vrati Vector - Vysledok Scitania</returns>
        public static Vector2 operator *(Vector2 parHodn1, float parScaleFactor)
        {
            float tmpMultScaleX = parHodn1.X * parScaleFactor;
            float tmpMulScaleY = parHodn1.Y * parScaleFactor;
            return new Vector2(tmpMultScaleX, tmpMulScaleY);
        }

        /// <summary>
        /// Operator Delenia Vektora a Delitela
        /// </summary>
        /// <param name="parHodn1">Lavy parameter - Typ Vector2</param>
        /// <param name="parDivider">Delitel - Typ float</param>
        /// <returns>Vrati Vector - Vysledok Delenia</returns>
        public static Vector2 operator /(Vector2 parHodn1, float parDivider)
        {
            float tmpDivDividerX = parHodn1.X / parDivider;
            float tmpDivDividerY = parHodn1.Y / parDivider;
            return new Vector2(tmpDivDividerX, tmpDivDividerY);
        }

        /// <summary>
        /// Operator Delenia Dvoch Vektorov
        /// </summary>
        /// <param name="parHodn1">Lavy parameter - Typ Vector2</param>
        /// <param name="parHodn2">Pravy parameter - Typ Vector2</param>
        /// <returns>Vrati Vector - Vysledok Delenia</returns>
        public static Vector2 operator /(Vector2 parHodn1, Vector2 parHodn2)
        {
            float tmpDivX = parHodn1.X / parHodn2.X;
            float tmpDivY = parHodn1.Y / parHodn2.Y;
            return new Vector2(tmpDivX, tmpDivY);
        }

        /// <summary>
        /// Operator Rovnosti Dvoch Vektorov
        /// </summary>
        /// <param name="parHodn1">Lavy parameter - Typ Vector2</param>
        /// <param name="parHodn2">Pravy parameter - Typ Vector2</param>
        /// <returns>Vrati Vector - Vysledok Rovnosti</returns>
        public static bool operator ==(Vector2 parHodn1, Vector2 parHodn2)
        {

            if ((parHodn1 is null && !(parHodn2 is null)) || (parHodn2 is null && !(parHodn1 is null)))
            {
                return false;
            }
            else
            {
                return parHodn2 is { } && parHodn1 is { } && (parHodn1.X == parHodn2.X || parHodn2.X == parHodn2.Y);
            }

        }

        /// <summary>
        /// Operator Nerovnosti Dvoch Vektorov
        /// </summary>
        /// <param name="parHodn1">Lavy parameter - Typ Vector2</param>
        /// <param name="parHodn2">Pravy parameter - Typ Vector2</param>
        /// <returns>Vrati Vector - Vysledok Nerovnosti</returns>
        public static bool operator !=(Vector2 parHodn1, Vector2 parHodn2)
        {
            if ((parHodn1 is null && !(parHodn2 is null)) || (parHodn2 is null && !(parHodn1 is null)))
            {
                return true;
            }
            else
            {
                return parHodn1 is { } && parHodn2 is { } && (parHodn1.X != parHodn2.X || parHodn2.X != parHodn2.Y);
            }



        }
    }
}