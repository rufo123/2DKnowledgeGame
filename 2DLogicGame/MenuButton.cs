using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DLogicGame
{
    /// <summary>
    /// Enumeracna trieda, ktora reprezentuje akciu spojenu s tlacidlom v Menu.
    /// </summary>
    public enum MenuButtonAction
    {
        Save = 0,
        ResetToDefault = 1
    }

    /// <summary>
    /// Trieda, ktora reprezentuje tlacidlo v menu.
    /// </summary>
    public class MenuButton
{

    /// <summary>
    /// Atribut, reprezentujuci text tlacidla - typ string
    /// </summary>
    private string aButtonText;


    /// <summary>
    /// Atribut, reprezentujuci poziciu tlacidla - typ Vector2
    /// </summary>
    private Vector2 aButtonPosition;

    /// <summary>
    /// Atribut, ktory reprezentuje funkciu tlacidla - typ MenuButtonAction - enum.
    /// </summary>
    private MenuButtonAction aButtonAction;


    /// <summary>
    /// Konstruktor, ktory inicializuje polozku nastavenia tlacidla.
    /// </summary>
    /// <param name="parButtonText">Parameter, reprezentujuci popis tlacidla - typ string.</param>
    /// <param name="parButtonPosition">Parameter, reprezentujuci poziciu tlacidla - typ Vector2.</param>
    /// <param name="parButtonAction">Parameter, ktory reprezentuje funkciu tlacidla - typ MenuButtonAction - enum.</param>
    public MenuButton(string parButtonText, Vector2 parButtonPosition, MenuButtonAction parButtonAction)
    {
        aButtonPosition = parButtonPosition;
        aButtonText = parButtonText;
        aButtonAction = parButtonAction;
    }

    public string ButtonText
    {
        get => aButtonText;
        set => aButtonText = value;
    }

    public Vector2 ButtonPosition
    {
        get => aButtonPosition;
        set => aButtonPosition = value;
    }

    public MenuButtonAction ButtonAction
    {
        get => aButtonAction;
        set => aButtonAction = value;
    }

       
    }
}
