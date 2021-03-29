using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _2DLogicGame
{

    public class ComponentCollection
    {

        private LogicGame aGame;

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

        public void AddComponent(DrawableGameComponent parComponent) {
            if (aComponentList != null) {
                aComponentList.Add(parComponent);
                aGame.Components.Add(parComponent);
            }
        }

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

        public void RemoveComponent(DrawableGameComponent parComponent)
        {
            if (aComponentList != null && aComponentList.Contains(parComponent))
            {
                aComponentList.Remove(parComponent);
            }
        }

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

        public void SetVisibility(bool parVisible)
        {

            int tmpNumberOfComponents = aComponentList.Count;

            for (int i = 0; i < tmpNumberOfComponents; i++)
            {

                aComponentList[i].Enabled = parVisible;
                aComponentList[i].Visible = parVisible;

            }

        }

        public List<DrawableGameComponent> ComponentList { get => aComponentList; set => aComponentList = value; }
    }
}
