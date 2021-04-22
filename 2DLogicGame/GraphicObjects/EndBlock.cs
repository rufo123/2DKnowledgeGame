using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2DLogicGame.GraphicObjects
{
    /// <summary>
    /// Trieda reprezentujuca block - konecny blok. - Klient.
    /// </summary>
    public class EndBlock : Block
    {
        /// <summary>
        /// Konstruktor, bloku - konecneho bloku.
        /// </summary>
        /// <param name="parGame">Parameter, reprezentujuci hru - typ LogicGame.</param>
        /// <param name="parPosition">Parameter, reprezentujuci poziciu - typ Vector2.</param>
        /// <param name="parTexture">Parameter, reprezentujuci texturu - typ Texture2D.</param>
        public EndBlock(LogicGame parGame, Vector2 parPosition, Texture2D parTexture = null) : base(parGame, parPosition, parTexture, parCollisionType: BlockCollisionType.Standable)
        {
            SetImageLocation("Sprites\\Blocks\\endBlock");
            IsInteractible = true;
        }

        /// <summary>
        /// Override metoda, starajuca sa o zmenu farby, pokial nejaka entita stoji na bloku.
        /// </summary>
        /// <param name="parGameTime">Parameter, reprezentujuci GameTime.</param>
        public override void Update(GameTime parGameTime)
        {
            this.ChangeColor(false, this.EntityIsStandingOnTop ? Color.Orange : Color.White);

            base.Update(parGameTime);
        }
    }
}
