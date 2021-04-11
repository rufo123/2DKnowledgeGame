using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpFont;

namespace _2DLogicGame.ClientSide
{
    public class OptionsController : DrawableGameComponent
    {
        private LogicGame aGame;

        private List<MenuOptionKeyWithToolTip> aListOptionsItem;

        /// <summary>
        /// Atribut, ktory reprezentuje Dictionary, ktora sa sklada z Option itemov, tato Dictionary je potrebna na to aby sme pri nastavovani novych tlacidiel
        /// Nemuseli prechadzat cely list ale pristupili priamo k tlacidlu a zmenili ho, inak by sa neaktualizovalo tlacidlo v nastaveniach
        /// </summary>
        private Dictionary<KeyTypes, MenuOptionKeyWithToolTip> aDictionaryOptionItems;

        private List<MenuButton> aListConfirmButtons;

        private Vector2 aPositionToolTip;

        private Vector2 aPositionKey;

        private int aIndexOfSelected;

        private bool aEnabled;

        private bool aFocused;

        private float aHelperBlinkTimer;

        private float aHelperPressedTimer;

        private Color aBlinkingColor;

        private bool aButtonPressed;

        private StringBuilder aConfigStringBuilder;

        private string aConfigFilePath;

        private string aConfigDefaultFilePath;



        /// <summary>
        /// Atribut, ktory reprezentuje, kolko krat ma tlacidlo zablikat, pokial je stlacene - typ int.
        /// </summary>
        private int aButtonTimesToBlink;

        /// <summary>
        /// Atribut, ktory reprezentuje pocitadlo zablikania tlacidla - typ int.
        /// </summary>
        private int aBlinkCounter;

        public bool EnabledOptions
        {
            get => aEnabled;
            set => aEnabled = value;
        }

        public OptionsController(LogicGame parGame) : base(parGame)
        {
            aGame = parGame;

            aPositionToolTip = new Vector2(1920 * (1 / 10F), 60); //X-Ova suradnica bude posunuta o 4/5-iny sirky
            aPositionKey = new Vector2(1920 * (4 / 5F), 60); //X-Ova suradnica bude posunuta o 1/5-inu sirky
            aListOptionsItem = new List<MenuOptionKeyWithToolTip>(9); //Viem, ze v hre budem potrebovat prave 9 nastaveni tlacidiel, v pripade potreby sa da navysit
            aListConfirmButtons = new List<MenuButton>(2); //Ditto - viem, ze budu prave 2.
            aDictionaryOptionItems = new Dictionary<KeyTypes, MenuOptionKeyWithToolTip>(9);

            aIndexOfSelected = 0;

            aEnabled = false;
            aFocused = false;

            aHelperBlinkTimer = 0F;

            aBlinkingColor = Color.White;

            aButtonPressed = false;

            aButtonTimesToBlink = 3;

            aBlinkCounter = 0;

            aConfigStringBuilder = new StringBuilder();

            aConfigFilePath = "Content\\Configuration\\controlsConfig.txt";
            aConfigDefaultFilePath = "Content\\Configuration\\controlsDefaultConfig.txt";
        }

        /// <summary>
        /// Metoda, ktora prida polozku nastavenia tlacidla.
        /// </summary>
        /// <param name="parOptionTooltip">Parameter, reprezentujuci popis nastavenia - typ string.</param>
        /// <param name="parKey">Parameter, ktory reprezentuje kluc, ktory chceme definovat - typ Keys.</param>
        /// <param name="parKeyType">Parameter, ktory reprezentuje typ tlacidla - typ KeyTypes - enum.</param>
        public void AddOption(string parOptionTooltip, Keys parKey, KeyTypes parKeyType)
        {
            if (aListOptionsItem != null)
            {

                float tmpOffsetY = 48 * 2; //Y-ovy offset nam staci urcit z vybraneho pisma a my vieme ze pouzijeme Font48.
                if (aListOptionsItem.Count > 0)
                {
                    aPositionKey.Y += tmpOffsetY;
                    aPositionToolTip.Y += tmpOffsetY;
                }


                aListOptionsItem.Add(new MenuOptionKeyWithToolTip(parOptionTooltip, parKey, aPositionToolTip, aPositionKey, parKeyType));
                aDictionaryOptionItems.Add(parKeyType, aListOptionsItem[aListOptionsItem.Count - 1]);
            }

        }

        /// <summary>
        /// Metoda, ktora prida tlacidlo, pre spravnu funkcnost je dobre ju volat az po pridani vsetkych menu itemov
        /// </summary>
        /// <param name="parButtonName"></param>
        /// <param name="parButtonAction"></param>
        public void AddButton(string parButtonName, MenuButtonAction parButtonAction) //Budeme pocitat s tym ze prve tlacidlo bude znamenat
        {
            if (aListConfirmButtons != null)
            {
                float tmpPosX = 1920 * (1 / 3F);
                float tmpPosY = aPositionToolTip.Y + 48 * 2;

                if (aListConfirmButtons.Count >= 1) //Ak pribudne druhe tlacidlo, posunieme ho uplne doprava
                {
                    tmpPosX = 1920 * (2 / 3F);
                }

                aListConfirmButtons.Add(new MenuButton(parButtonName, new Vector2(tmpPosX, tmpPosY), parButtonAction));
            }
        }

        /// <summary>
        /// Metoda, ktora prepina medzi zvolenymi polozkami nastaveni.
        /// </summary>
        /// <param name="parGoNext">Parameter, reprezentujuci ci prejdeme na dalsie nastavenie - true, alebo predosle - false -> typ bool.</param>
        public void SwitchToNextOption(bool parGoNext)
        {
            if (aListOptionsItem != null && aListOptionsItem.Count > 0 && aListConfirmButtons.Count > 0)
            {
                if (parGoNext) //DOPREDU
                {
                    if (aIndexOfSelected < aListOptionsItem.Count - 1 + aListConfirmButtons.Count)
                    {
                        aIndexOfSelected++;
                    }
                    else
                    {
                        aIndexOfSelected = 0;
                    }
                }
                else //DOZADU
                {
                    if (aIndexOfSelected > 0)
                    {
                        aIndexOfSelected--;
                    }
                    else
                    {
                        aIndexOfSelected = aListOptionsItem.Count - 1 + aListConfirmButtons.Count;
                    }
                }
            }
        }

        public void SwitchColor()
        {
            if (aBlinkingColor == Color.White)
            {
                aBlinkingColor = Color.Black;
            }
            else
            {
                aBlinkingColor = Color.White;
            }
        }

        /// <summary>
        /// Metoda, ktora vysvieti polozku ako oznacenu, pouziva sa pri tlacidlach.
        /// </summary>
        public Color ColorizeMarkedItem()
        {
            return Color.Red;
        }

        public override void Update(GameTime parGameTime)
        {
            if (aEnabled)
            {
                if (!aFocused)
                {
                    if (aGame.CheckKeyPressedOnce(aGame.DownKey)) //Dopredu
                    {
                        this.SwitchToNextOption(true);
                        aButtonPressed = false;
                    }
                    else if (aGame.CheckKeyPressedOnce(aGame.UpKey)) //Dozadu
                    {
                        this.SwitchToNextOption(false);
                        aButtonPressed = false;
                    }
                }

                if (aFocused && aGame.CurrentPressedKey.GetPressedKeyCount() > 0 && aGame.CheckKeyPressedOnce(aGame.CurrentPressedKey.GetPressedKeys()[0])) //Ak mame focus na item, spusti sa zmena tlacidla
                {
                    if (aListOptionsItem != null && aIndexOfSelected < aListOptionsItem.Count - 1)
                    { //Najprv porovname ci sa jedna o polozky nastaveni tlacidiel
                        aListOptionsItem[aIndexOfSelected].Key = aGame.CurrentPressedKey.GetPressedKeys()[0];
                        aFocused = false;
                    }

                }
                if (!aFocused && aGame.CheckKeyPressedOnce(aGame.ProceedKey) && aListOptionsItem != null && aIndexOfSelected < aListOptionsItem.Count - 1) //Potvrdenie volby
                {
                    aFocused = !aFocused; //Ked proceed key zmeni sa focus na zvoleny alebo nezvoleny item.
                }
                else if (!aFocused && aGame.CheckKeyPressedOnce(aGame.ProceedKey) && aListConfirmButtons != null && aListOptionsItem != null) //Stlacenie tlacidla
                {
                    aButtonPressed = true;
                    aBlinkCounter = 0;

                    switch (aListConfirmButtons[aIndexOfSelected % aListOptionsItem.Count].ButtonAction)
                    {
                        case MenuButtonAction.Save:
                            Debug.WriteLine("Save");
                            SaveOption(aConfigFilePath);
                            break;
                        case MenuButtonAction.ResetToDefault:
                            Debug.WriteLine("Reset");
                            ResetOptions(aConfigFilePath, aConfigDefaultFilePath);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

            }

            base.Update(parGameTime);
        }

        public override void Draw(GameTime parGameTime)
        {
            if (aFocused)
            {
                aHelperBlinkTimer += parGameTime.ElapsedGameTime.Milliseconds;

                if (aHelperBlinkTimer >= 300F)
                {
                    aHelperBlinkTimer = 0F;
                    SwitchColor();
                }
            }

            Color tmpButtonPressedColor = Color.Black;

            if (aButtonPressed && aBlinkCounter < aButtonTimesToBlink)
            {
                aHelperPressedTimer += parGameTime.ElapsedGameTime.Milliseconds;

                if (aHelperPressedTimer >= 150F)
                {
                    tmpButtonPressedColor = ColorizeMarkedItem();
                }

                if (aHelperPressedTimer >= 300F)
                {
                    aHelperPressedTimer = 0F;
                    aBlinkCounter++;
                    tmpButtonPressedColor = Color.Gold;
                }

                if (aBlinkCounter >= aButtonTimesToBlink)
                {
                    aButtonPressed = false;
                    aBlinkCounter = 0;
                }
            }


            if (aEnabled && aListOptionsItem != null && aListConfirmButtons != null)
            {
                for (int i = 0; i < aListOptionsItem.Count + aListConfirmButtons.Count; i++)
                {
                    Color tmpColorKey = Color.White;
                    Color tmpColorButton = Color.Gold;

                    if (i == aIndexOfSelected && aFocused)
                    {
                        tmpColorKey = aBlinkingColor;
                    }
                    else if (i == aIndexOfSelected && !aButtonPressed)
                    {
                        tmpColorKey = Color.Black;
                        tmpColorButton = Color.Black;
                    }
                    else if (i == aIndexOfSelected && aButtonPressed)
                    {
                        tmpColorButton = tmpButtonPressedColor;
                    }


                    if (i < aListOptionsItem.Count)
                    {
                        aGame.SpriteBatch.DrawString(aGame.Font48, aListOptionsItem[i].ToolTip, aListOptionsItem[i].ToolTipPosition, tmpColorKey, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                        aGame.SpriteBatch.DrawString(aGame.Font48, aListOptionsItem[i].Key.ToString(), aListOptionsItem[i].KeyPosition, tmpColorKey, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                    }
                    else
                    {
                        Vector2 tmpButtonCenterOffset = Vector2.Zero;
                        tmpButtonCenterOffset.X -= aGame.Font48.MeasureString(aListConfirmButtons[i % aListOptionsItem.Count].ButtonText).X / 2;

                        aGame.SpriteBatch.DrawString(aGame.Font48, aListConfirmButtons[i % aListOptionsItem.Count].ButtonText, aListConfirmButtons[i % aListOptionsItem.Count].ButtonPosition + tmpButtonCenterOffset, tmpColorButton, 0F, Vector2.Zero, 1F, SpriteEffects.None, 0F);
                        //Tu si musime davat pozor aby sme neprekrocili velkost ListConfirmButtonu, pracujeme s modulo. A pouzivame center offset, to znamena ze tento offset zarovna tlacidla ako keby do stredu.
                    }
                }
            }

            base.Draw(parGameTime);
        }

        /// <summary>
        /// Metoda, ktora sa stara o ulozenie aktualnej konfiguracie tlacidiel do suboru.
        /// Priklad cesty - Content\\Configuration\\controlsConfig.txt
        /// </summary>
        /// <param name="parConfigFilePath"></param>
        public void SaveOption(string parConfigFilePath)
        {

            string tmpFilePath = AppContext.BaseDirectory + parConfigFilePath;

            aConfigStringBuilder.Clear();

            if (aListOptionsItem != null)
            {
                for (int i = 0; i < aListOptionsItem.Count; i++)
                {
                    aConfigStringBuilder.AppendLine("#" + aListOptionsItem[i].ToolTip);
                    aConfigStringBuilder.AppendLine(aListOptionsItem[i].KeyType.ToString() + "=" + aListOptionsItem[i].Key.ToString());

                }

                File.WriteAllText(parConfigFilePath, aConfigStringBuilder.ToString());

            }

            ReadAndSetKeysFromConfig(parConfigFilePath);
        }

        /// <summary>
        /// Metoda, ktora sa stara o prepisanie aktualnej konfiguracie tlacidiel podla prednastavenej konfiguracie
        /// </summary>
        /// <param name="parConfigFilePath"></param>
        /// <param name="parDefaultConfigFilePath"></param>
        public void ResetOptions(string parConfigFilePath, string parDefaultConfigFilePath)
        {
            string tmpConfigFilePath = AppContext.BaseDirectory + parConfigFilePath;
            string tmpDefaultConfigFilePath = AppContext.BaseDirectory + parDefaultConfigFilePath;

            if (File.Exists(tmpDefaultConfigFilePath))
            {

                File.Copy(tmpDefaultConfigFilePath, tmpConfigFilePath, true);
            }
            else
            {
                CreateDefaultConfig(aConfigDefaultFilePath);
            }

            ReadAndSetKeysFromConfig(parConfigFilePath);
        }

        /// <summary>
        /// Metoda, ktora nacita ulozene tlacidla v konfiguracii a nastavi ich.
        /// </summary>
        /// <param name="parConfigFilePath">Parameter, reprezentujuci cestu k suboru s konfiguraciou - typ string.</param>
        public void ReadAndSetKeysFromConfig(string parConfigFilePath)
        {
            string tmpConfigFilePath = AppContext.BaseDirectory + parConfigFilePath;

            bool tmpFileFound = false;

            if (!File.Exists(tmpConfigFilePath))
            {

                string tmpDefaultConfigFilePath = AppContext.BaseDirectory + aConfigDefaultFilePath;
                //Ak by sa stalo, ze by nebola vytvorena nasa konfiguracia tlacidiel

                if (!File.Exists(tmpDefaultConfigFilePath)) //Ak by neexistovala ani prednastavena konfiguracia ani nasa tak nebudeme vykonavat metodu a prevezmeme konfiguraciu z triedy LogicGame
                {
                    tmpFileFound = false;
                    //Kedze neexistuje prednastavena konfiguracia, vytvorime ju
                    CreateDefaultConfig(aConfigDefaultFilePath);
                }
                else  //Skontrolujeme ci existuje prednastavena konfiguraci
                {
                    parConfigFilePath = tmpDefaultConfigFilePath; //Prevezmeme si prednastavenu konfiguraciu
                    tmpFileFound = true;

                }
            }
            else //Ak existuje konfiguracia prevezmeme ju
            {
                tmpFileFound = true;
            }

            if (tmpFileFound) 
            {

                if (aListOptionsItem != null)
                {

                    if (File.Exists(tmpConfigFilePath))
                    {
                        foreach (string tmpStringInLine in File.ReadLines(tmpConfigFilePath))
                        {
                            if (!tmpStringInLine.StartsWith("#")
                            ) //Ak nezacina riadok na znak - #, co je len popisny riadok.
                            {
                                if (!string.IsNullOrWhiteSpace(tmpStringInLine)) //Ak nie je riadok prazdny
                                {
                                    int tmpLocationOfEqualsOperator =
                                        tmpStringInLine.IndexOf("=",
                                            StringComparison.Ordinal); //Vypocitame na ktorom indexe sa nachadza znak =

                                    if (tmpLocationOfEqualsOperator >= 1
                                    ) //Este skontrolujeme ci sa nas operator nachadza aspon na prvom indexe
                                    {
                                        string tmpKeyTypeString =
                                            tmpStringInLine.Substring(0, tmpLocationOfEqualsOperator);
                                        string tmpKeyString = tmpStringInLine.Substring(tmpLocationOfEqualsOperator + 1,
                                            tmpStringInLine.Length - tmpLocationOfEqualsOperator - 1);

                                        if (Enum.TryParse(tmpKeyTypeString, out KeyTypes tmpKeyType) &&
                                            Enum.TryParse(tmpKeyString, out Keys tmpKey))
                                        {

                                            SetKey(tmpKeyType, tmpKey);
                                        }

                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metoda, ktora inicializuje tlacidla z konfiguracie - vyuzita hlavne pri zapnuti hry.
        /// </summary>
        public void InitKeysFromConfig()
        {
            ReadAndSetKeysFromConfig(aConfigFilePath);
        }

        public void CreateDefaultConfig(string parDefaultConfigFilePath)
        {
            SaveOption(parDefaultConfigFilePath);
        }


        /// <summary>
        /// Metoda, ktora nastavi tlacidla podla typu.
        /// <param name="parKeyType">Parameter, reprezentujuci typ tlacidla - typ KeyTypes.</param>
        /// <param name="parKey">Parameter, reprezentujuci znak, ktory reprezentuje tlacidlo - typ Keys.</param>
        /// </summary>
        public void SetKey(KeyTypes parKeyType, Keys parKey)
        {
            switch (parKeyType)
            {
                case KeyTypes.UpKey:
                    aGame.UpKey = parKey;
                    break;
                case KeyTypes.DownKey:
                    aGame.DownKey = parKey;
                    break;
                case KeyTypes.LeftKey:
                    aGame.LeftKey = parKey;
                    break;
                case KeyTypes.RightKey:
                    aGame.RightKey = parKey;
                    break;
                case KeyTypes.ProceedKey:
                    aGame.ProceedKey = parKey;
                    break;
                case KeyTypes.ChatWriteMessageKey:
                    aGame.ChatWriteMessageKey = parKey;
                    break;
                case KeyTypes.MusicLower:
                    aGame.MusicLower = parKey;
                    break;
                case KeyTypes.MusicHigher:
                    aGame.MusicHigher = parKey;
                    break;
                case KeyTypes.MusicStartTop:
                    aGame.MusicStartStop = parKey;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parKeyType), parKeyType, null);
            }

            aDictionaryOptionItems[parKeyType].Key = parKey;
            //Preto je potrebna tato Dictionary, aby sa nam aktualizovali nastavenia tlacidiel aj v menu a aby sme nemuseli prechadzat cely list
        }





    }
}
