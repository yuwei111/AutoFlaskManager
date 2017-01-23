using PoeHUD.Controllers;
using PoeHUD.Poe.Components;
using PoeHUD.Plugins;
using System.Collections.Generic;
using PoeHUD.Poe;
using System.Windows.Forms;
using System.Threading;
using System;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.Elements;
using System.Runtime.InteropServices;

namespace FlaskManager
{
    public class FlaskManagerCore : BaseSettingsPlugin<FlaskManagerSettings>
    {
        private bool DEBUG = true;
        private int logmsg_time = 3;
        private int errmsg_time = 5;
        private bool isThreadEnabled;
        private Element FlasksRoot;
        private IntPtr gameHandle;
        private Entity playerInfo;
        private List<PlayerFlask> PlayerFlasks;

        #region FlaskInformations
        private FlaskAction flask_name_to_action(string flaskname)
        {
            flaskname = flaskname.ToLower();
            FlaskAction ret = FlaskAction.NONE;
            String defense_pattern = @"bismuth|jade|stibnite|granite|
                amethyst|ruby|sapphire|topaz|aquamarinequartz";
            String offense_pattern = @"silver|sulphur|basalt|diamond";
            if (flaskname.Contains("life"))
                ret = FlaskAction.LIFE;
            else if (flaskname.Contains("mana"))
                ret = FlaskAction.MANA;
            else if (flaskname.Contains("hybrid"))
                ret = FlaskAction.HYBRID;
            else if (flaskname.Contains("quicksilver"))
                ret = FlaskAction.SPEEDRUN;
            else if (System.Text.RegularExpressions.Regex.IsMatch(flaskname, defense_pattern))
                ret = FlaskAction.DEFENSE;
            else if (System.Text.RegularExpressions.Regex.IsMatch(flaskname, offense_pattern))
                ret = FlaskAction.OFFENSE;
            return ret;
        }
        private FlaskAction flask_mod_to_action(string flaskmodRawName)
        {
            flaskmodRawName = flaskmodRawName.ToLower();
            FlaskAction ret = FlaskAction.NONE;
            String defense_pattern = @"armour|evasion|lifeleech|manaleech|resistance";
            if (flaskmodRawName.Contains("poison"))
                ret = FlaskAction.POISON_IMMUNE;
            else if (flaskmodRawName.Contains("chill"))
                ret = FlaskAction.FREEZE_IMMUNE;
            else if (flaskmodRawName.Contains("burning"))
                ret = FlaskAction.IGNITE_IMMUNE;
            else if (flaskmodRawName.Contains("shock"))
                ret = FlaskAction.SHOCK_IMMUNE;
            else if (flaskmodRawName.Contains("bleeding"))
                ret = FlaskAction.BLEED_IMMUNE;
            else if (flaskmodRawName.Contains("curse"))
                ret = FlaskAction.CURSE_IMMUNE;
            else if (flaskmodRawName.Contains("knockback"))
                ret = FlaskAction.OFFENSE;
            else if (flaskmodRawName.Contains("movementspeed"))
                ret = FlaskAction.SPEEDRUN;
            else if (System.Text.RegularExpressions.Regex.IsMatch(flaskmodRawName, defense_pattern))
                ret = FlaskAction.DEFENSE;
                return ret;
        }
        #endregion
        public override void Initialise()
        {
            PlayerFlasks = new List<PlayerFlask>();
            onFlaskManagerToggle();
            GameController.Area.OnAreaChange += area => onAreaChange();
            Settings.Enable.OnValueChanged +=  onFlaskManagerToggle;
        }
        private void onFlaskManagerToggle()
        {
            try
            {
                if (Settings.Enable.Value)
                {
                    if (DEBUG)
                        LogMessage("Enabling FlaskManager.",logmsg_time);
                    isThreadEnabled = true;
                    PluginName = "Flask Manager";
                    gameHandle = GameController.Window.Process.MainWindowHandle;
                    ScanForFlaskAddress(GameController.Game.IngameState.UIRoot);
                    playerInfo = GameController.Game.IngameState.Data.LocalPlayer;
                    searchFlasksInventory();
                    //We are creating our plugin thread inside PoEHUD!
                    Thread flaskThread = new Thread(FlaskThread) { IsBackground = true };
                    flaskThread.Start();
                }
                else
                {
                    if (DEBUG)
                        LogMessage("Disabling FlaskManager.",logmsg_time);
                    FlasksRoot = null;
                    gameHandle = IntPtr.Zero;
                    PlayerFlasks.Clear();
                    isThreadEnabled = false;
                }
            }
            catch (Exception)
            {

                LogError("Error Starting FlaskManager Thread.", errmsg_time);
            }
        }
        private void onAreaChange()
        {
            if (Settings.Enable.Value)
            {
                ScanForFlaskAddress(GameController.Game.IngameState.UIRoot);
                searchFlasksInventory();
            }
        }
        private void searchFlasksInventory()
        {
            if (DEBUG)
                LogMessage("Searching for flasks in inventory.", logmsg_time);
            PlayerFlasks = new List<PlayerFlask>();
            if (FlasksRoot != null)
            {
                foreach (Element flaskElem in FlasksRoot.Children)
                {
                    Entity item = flaskElem.AsObject<InventoryItemIcon>().Item;
                    if (item != null && item.HasComponent<Flask>())
                    {
                        var flaskCharges = item.GetComponent<Charges>();
                        var mods = item.GetComponent<Mods>();
                        var flaskName = GameController.Files.BaseItemTypes.Translate(item.Path).BaseName;
                        #region UniqueFlaskNotImplemented
                        var isUnique = false;
                        foreach (var mod in mods.ItemMods)
                            if ( mod.RawName.ToLower().Contains("unique") )
                                isUnique = true;
                        if (isUnique)
                        {
                            LogError("Unique Flasks are not implemented.", errmsg_time);
                            continue;
                        }
                        #endregion
                        PlayerFlask newFLask = new PlayerFlask();
                        newFLask.FlaskName = flaskName;
                        newFLask.Slot = PlayerFlasks.Count;
                        newFLask.setSettings(Settings);
                        newFLask.EnableDisableFlask();
                        newFLask.Item = item;
                        newFLask.MaxCharges = flaskCharges.ChargesMax;
                        newFLask.UseCharges = flaskCharges.ChargesPerUse;
                        newFLask.CurrentCharges = flaskCharges.NumCharges;

                        FlaskAction action1 = flask_name_to_action(flaskName);
                        if (action1 == FlaskAction.NONE)
                            LogError("Error: " + flaskName + " not found", errmsg_time);
                        else
                            newFLask.FlaskAction1 = action1;
                        FlaskAction action2 = FlaskAction.NONE;
                        foreach (var mod in mods.ItemMods)
                        {
                            action2 = flask_mod_to_action(mod.RawName);
                            if (action2 == FlaskAction.NONE)
                                LogError("Error: " + mod.RawName + " not found", errmsg_time);
                            else
                                newFLask.FlaskAction2 = action2;
                        }

                        PlayerFlasks.Add(newFLask);
                    }
                }
            }
        }
        private void ScanForFlaskAddress(Element elm)
        {
            foreach (var child in elm.Children)
            {
                var item = child.AsObject<InventoryItemIcon>().Item;
                if (item != null)
                {
                    if (item.HasComponent<Flask>())
                    {
                        FlasksRoot = child.Parent;
                        if (DEBUG)
                            LogMessage("Found Flask Address.", logmsg_time);
                        return;
                    }
                }
                ScanForFlaskAddress(child);
            }
        }
        public void UseFlask(PlayerFlask flask)
        {
            if (flask.Slot == 1)
                KeyPressRelease(Keys.D1);
            else if (flask.Slot == 2)
                KeyPressRelease(Keys.D2);
            else if (flask.Slot == 3)
                KeyPressRelease(Keys.D3);
            else if (flask.Slot == 4)
                KeyPressRelease(Keys.D4);
            else if (flask.Slot == 5)
                KeyPressRelease(Keys.D5);
        }
        private void FlaskMain()
        {
            foreach (var flask in PlayerFlasks)
            {
                LogMessage(flask.Slot + ": " + flask.FlaskName + " ActionA=" + flask.FlaskAction1 + " ActionB=" + flask.FlaskAction2, logmsg_time);
            }
            /*            var isplayer = localPlayer.IsValid;
                        var life = localPlayer.GetComponent<Life>();
                        int hp = localPlayer.IsValid ? life.CurHP + life.CurES : 0;
                        //if(isplayer != false) LogMessage($"Our Current Health is {life.CurHP.ToString()} !!", 2); //Example code given, this is our own thread. 
                        */
            return;
        }

        #region Keyboard Input
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, UIntPtr wParam, UIntPtr lParam);
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        public void KeyDown(Keys Key)
        {
            SendMessage(gameHandle, 0x100, (int)Key, 0);
        }
        public void KeyUp(Keys Key)
        {
            SendMessage(gameHandle, 0x101, (int)Key, 0);
        }
        public void KeyPressRelease(Keys key, int delay = 50)
        {
            KeyDown(key);
            Thread.Sleep(delay);
            KeyUp(key);
        }
        private void Write(string text, params object[] args)
        {
            foreach (var character in string.Format(text, args))
            {
                PostMessage(gameHandle, 0x0102, new UIntPtr(character), UIntPtr.Zero);
            }
        }
        #endregion
        #region Threading, do not touch
        private void FlaskThread()
        {
            while (isThreadEnabled)
            {
                    FlaskMain();
                    Thread.Sleep(1000);
            }
       }
        #endregion
    }

    public class AutoHPManaFlask
    {

    }
    public class RemoveAilmentsFlask
    {

    }
    public class QuickSilverFlask
    {

    }
    public class DefenssiveFlask
    {

    }
    public class OffensiveFlask
    {

    }
    public class UniqueFlask
    {

    }
    public class AutoQuit
    {
    }

    public class PlayerFlask
    {
        public string FlaskName;
        public int Slot;
        public bool isEnabled;

        public Entity Item;
        public FlaskAction FlaskAction1;
        public FlaskAction FlaskAction2;

        public int CurrentCharges;
        public int UseCharges;
        public int MaxCharges;

        private FlaskManagerSettings Settings;
        public void setSettings(FlaskManagerSettings s)
        {
            Settings = s;
        }
        public void EnableDisableFlask()
        {
            switch (Slot)
            {
                case 0:
                    isEnabled = Settings.flaskSlot1Enable.Value;
                    Settings.flaskSlot1Enable.OnValueChanged += EnableDisableFlask;
                    break;
                case 1:
                    isEnabled = Settings.flaskSlot2Enable.Value;
                    Settings.flaskSlot2Enable.OnValueChanged += EnableDisableFlask;
                    break;
                case 2:
                    isEnabled = Settings.flaskSlot3Enable.Value;
                    Settings.flaskSlot3Enable.OnValueChanged += EnableDisableFlask;
                    break;
                case 3:
                    isEnabled = Settings.flaskSlot4Enable.Value;
                    Settings.flaskSlot4Enable.OnValueChanged += EnableDisableFlask;
                    break;
                case 4:
                    isEnabled = Settings.flaskSlot5Enable.Value;
                    Settings.flaskSlot5Enable.OnValueChanged += EnableDisableFlask;
                    break;
                default:
                    break;
            }
        }
        ~PlayerFlask()
        {
            switch (Slot)
            {
                case 0:
                    Settings.flaskSlot1Enable.OnValueChanged -= EnableDisableFlask;
                    break;
                case 1:
                    Settings.flaskSlot2Enable.OnValueChanged -= EnableDisableFlask;
                    break;
                case 2:
                    Settings.flaskSlot3Enable.OnValueChanged -= EnableDisableFlask;
                    break;
                case 3:
                    Settings.flaskSlot4Enable.OnValueChanged -= EnableDisableFlask;
                    break;
                case 4:
                    Settings.flaskSlot5Enable.OnValueChanged -= EnableDisableFlask;
                    break;
                default:
                    break;
            }
        }
    }
    public enum FlaskAction : int
    {
        NONE = 0,
        LIFE, //life
        MANA, //mana
        HYBRID,
        DEFENSE, //bismuth, jade, stibnite, granite,
                 //amethyst, ruby, sapphire, topaz,
                 // aquamarine, quartz
                 //MODS: iron skin, reflexes, gluttony,
                 // craving, resistance
        SPEEDRUN, //quicksilver, MOD: adrenaline,
        OFFENSE, //silver, sulphur, basalt, diamond, MOD: Fending
        POISON_IMMUNE,// MOD: curing
        FREEZE_IMMUNE,// MOD: heat
        IGNITE_IMMUNE,// MOD: dousing
        SHOCK_IMMUNE,// MOD: grounding
        BLEED_IMMUNE,// MOD: staunching
        CURSE_IMMUNE, // MOD: warding
        UNIQUE_FLASK
    }
    }