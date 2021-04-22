using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
// ReSharper disable InvalidXmlDocComment

namespace _2DLogicGame
{
    /// <summary>
    /// Trida, ktora reprezentuje kolekciu vykreslitelnych komponentov - pre kniznicu MonoGame. 
    /// </summary>
    public class ComponentCollection
    {
        /// <summary>
        /// Atribut, reprezentujuci hru - typ LogicGame.
        /// </summary>
        private LogicGame aGame;


        /// <summary>
        /// Atribut, reprezentujuci List komponentov - typ List<DrawableGameComponent>
        /// </summary>
        private List<DrawableGameComponent> aComponentList;

        /// <summary>
        /// Konstruktor, ktory vytvara kolekciu Componentov, hlavne kvoli sudrznosti Komponentov, ktora sa vykresluju naraz, napr obsah Menu
        /// </summary>
        /// <param name="parGame">Parameter LogicGame - Kvoli pridaniu komponentov</param>
        /// <param name="parComponent">Variabilny pocet Argumentov typu GameComponent</param>
        public ComponentCollection(LogicGame parGame, params DrawableGameComponent[] parComponent)
        {
            aGame = parGame;

            aComponentList = new List<DrawableGameComponent>();

            int tmpNumberOfComponents = parComponent.Length;

            // Init
            for (int i = 0; i < tmpNumberOfComponents; i++)
            {
                aComponentList.Add(parComponent[i]);
                parGame.Components.Add(parComponent[i]);
                Debug.WriteLine("Silverhand");
            }

            this.SetVisibility(false);
        }

        /// <summary>
        /// Metoda, ktora prida komponent do Listu.
        /// </summary>
        /// <param name="parComponent">Parameter, reprezentujuci jeden komponent - typ DrawableGameComponent.</param>
        public void AddComponent(DrawableGameComponent parComponent) {
            if (aComponentList != null) {
                aComponentList.Add(parComponent);
                aGame.Components.Add(parComponent);
            }
        }

        /// <summary>
        /// Metoda, ktora prida List komponentov do Listu.
        /// </summary>
        /// <param name="parComponents">Parameter, reprezentujuci list komponentov - typ List<DrawableGameComponent>.</param>
        public void AddComponents(List<DrawableGameComponent> parComponents)
        {
            if (aComponentList != null)
            {
                for (int i = 0; i < parComponents.Count; i++)
                {
                    aComponentList.Add(parComponents[i]);
                    aGame.Components.Add(parComponents[i]);
                }
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o odobratie komponentu z Listu.
        /// </summary>
        /// <param name="parComponents">Parameter, reprezentujuci jeden komponent - typ DrawableGameComponent.</param>
        public void RemoveComponent(DrawableGameComponent parComponent)
        {
            if (aComponentList != null && aComponentList.Contains(parComponent))
            {
                aComponentList.Remove(parComponent);
            }

        }
        /// <summary>
        /// Metoda, ktora sa stara o odobranie Listu komponentov z Listu.
        /// </summary>
        /// <param name="parComponents">Parameter, reprezentujuci list komponentov - typ List<DrawableGameComponent>.</param>
        public void RemoveComponents(List<DrawableGameComponent> parComponents)
        {
            if (aComponentList != null)
            {
                for (int i = 0; i < parComponents.Count; i++)
                {
                    if (aComponentList.Contains(parComponents[i]))
                    {
                        aComponentList.Remove(parComponents[i]);
                        aGame.Components.Remove(parComponents[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Metoda, ktora sa stara o nastavenie vidielnosti komponentov v Liste.
        /// </summary>
        /// <param name="parVisible">Parameter, reprezentujuci hodnotu, ci sa maju komponenty zobrazit alebo nie - typ bool.</param>
        public void SetVisibility(bool parVisible)
        {

            int tmpNumberOfComponents = aComponentList.Count;

            for (int i = 0; i < tmpNumberOfComponents; i++)
            {

                aComponentList[i].Enabled = parVisible;
                aComponentList[i].Visible = parVisible;

            }

        }

    }
}
