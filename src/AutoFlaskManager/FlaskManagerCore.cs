using PoeHUD.Controllers;
using PoeHUD.Models;
using PoeHUD.Poe.Components;
using PoeHUD.Plugins;
using System.IO;
using System.Collections.Generic;
using PoeHUD.Poe;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading;
using System;
using System.Linq;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.Elements;
using System.Runtime.InteropServices;

namespace FlaskManager
{
    public class FlaskManagerCorePlugin : BaseSettingsPlugin<FlaskManagerSettings>
    {
        private const string FlaskEffectsFile = "FlaskEffects.txt";
        private bool isThreadEnabled = false;
        private FlasksConfig FlasksCfg;
        //private bool getForegroundWindow;
        //private static GameController Entity;
        //private readonly Random random;
        private long FlaskRootAddress = 0;
        private IntPtr gameHandle;
        private Element FlasksRoot;
        private List<PlayerFlask> PlayerFlasks = new List<PlayerFlask>();

        private void onFlaskManagerToggle()
        {
            try
            {
                if (Settings.Enable.Value)
                {
                    LogMessage("Enabling FlaskManager.",1);
                    //We are creating our plugin thread inside PoEHUD!
                    Thread flaskThread = new Thread(FlaskThread) { IsBackground = true };
                    isThreadEnabled = true;
                    flaskThread.Start();
                }
                else
                {
                    LogMessage("Disabling FlaskManager.",1);
                    isThreadEnabled = false;
                }
            }
            catch (Exception)
            {

                LogError("Error Starting FlaskManager Thread.", 4);
            }
        }

        public override void Initialise()
        {
            ReadConfig(); //Reading the Flask Config File
            gameHandle = GameController.Window.Process.MainWindowHandle;
            onFlaskManagerToggle();
            Settings.Enable.OnValueChanged += onFlaskManagerToggle;
        }

        private void FlaskMain()
        {
           //DebugFlasks();
            ScanForFlask(GameController.Game.IngameState.UIRoot); //We are scanning for flasks that's on our inventory tab
            InitPlayerFlasks(); //We are reading the Flask Config File etc
            Entity localPlayer = GameController.Game.IngameState.Data.LocalPlayer; //Need LocalPlayer for detection of Health,Mana,CI and such
            var isplayer = localPlayer.IsValid;
            var life = localPlayer.GetComponent<Life>();
            int hp = localPlayer.IsValid ? life.CurHP + life.CurES : 0;
            //if(isplayer != false) LogMessage($"Our Current Health is {life.CurHP.ToString()} !!", 2); //Example code given, this is our own thread. 
        }


        public void UseFlasksForAction(FlaskAction action)
        {
            if (PlayerFlasks.Count == 0)
            {
                ScanForFlask(GameController.Game.IngameState.UIRoot);
                InitPlayerFlasks();

                if (PlayerFlasks.Count == 0)
                    
                    LogMessage($"=================NO PLAYER FLASKS!!! NOT FIXED!!!!!!!!!! Addr: {FlaskRootAddress}===========================", 3);
                else
                    LogMessage("=================NO PLAYER FLASKS!!! FIXED!!!!!!!!!!===========================", 3);
            }

            var fightFlasks = PlayerFlasks.Where(x => x.FlaskAction == action);

            foreach (var flask in fightFlasks)
            {
                if (flask.FlaskCharges > flask.UseCharges && !IsFlaskInUse(flask))
                {
                    LogMessage($"Using flask in slot: {flask.Slot}", 2);
                    UseFlask(flask);
                }
                else
                {
                    LogMessage($"Ignore flask in slot: {flask.Slot}", 3);
                }
            }
            InitPlayerFlasks();
        }

        private void InitPlayerFlasks()
        {
            PlayerFlasks = new List<PlayerFlask>();

            if (FlasksRoot != null)
            {
                foreach (var flaskElem in FlasksRoot.Children)
                {
                    var flaskInventItem = flaskElem.AsObject<InventoryItemIcon>();
                    var item = flaskInventItem.Item;
                    if (item != null && item.HasComponent<Flask>())
                    {
                        var flaskCharges = item.GetComponent<Charges>();

                        var newFlask = new PlayerFlask();
                        newFlask.Slot = PlayerFlasks.Count;
                        newFlask.FlaskCharges = flaskCharges.NumCharges;
                        newFlask.UseCharges = flaskCharges.ChargesPerUse;
                        newFlask.MaxCharges = flaskCharges.ChargesMax;
  

                        newFlask.Item = item;

                        BaseItemType bit = GameController.Files.BaseItemTypes.Translate(item.Path);
                        if (bit == null)
                            continue;


                        newFlask.DebugPosX = flaskElem.X;
                        newFlask.FlaskName = bit.BaseName;

                        var flaskCfg = GetConfigForFlask(newFlask.FlaskName);

                        //This is my debug code.. I used it to test on certain flasks
                        LogMessage($"{newFlask.FlaskName} !", 20);
                        LogMessage($"{newFlask.FlaskCharges} !", 20);
                        LogMessage($"{newFlask.UseCharges} !", 20);
                        LogMessage($"{newFlask.MaxCharges} !", 20);
                        LogMessage($"{newFlask.Slot} !", 20);

                        if (flaskCfg != null)
                        {
                            newFlask.FlaskEffectName = flaskCfg.FlaskEffectName;
                            newFlask.FlaskAction = flaskCfg.FlaskAction;
                            newFlask.UseCharges = flaskCharges.ChargesPerUse;
                            newFlask.MaxCharges = flaskCharges.ChargesMax;
                            PlayerFlasks.Add(newFlask);
                        }
                        else
                        {
                            LogMessage($"=========================================Can't find cfg for flask: {newFlask.FlaskName} !", 20);
                        }

                    }
                }
            }

            PlayerFlasks = PlayerFlasks.OrderBy(x => x.DebugPosX).ToList();

            int slot = 1;
            foreach (var flask in PlayerFlasks)
            {
                flask.Slot = slot;
                slot++;
            }
        }



        private bool IsFlaskInUse(PlayerFlask flask)
        {
            var life = GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();

            foreach (var buff in life.Buffs)
            {
                if (buff.Name == flask.FlaskEffectName)
                {
                    return true;
                }
            }
            return false;
        }

        private void ScanForFlask(Element elm)
        {
            FlaskRootAddress = 0;
            foreach (var child in elm.Children)
            {
                var flaskInventItem = child.AsObject<InventoryItemIcon>();
                var item = flaskInventItem.Item;
                if (item != null)
                {
                    if (item.HasComponent<Flask>())
                    {
                        FlasksRoot = child.Parent;
                        FlaskRootAddress = elm.Address;
                        LogMessage("================================Flasks Found!!=============================", 10);
                        return;
                    }
                }

                ScanForFlask(child);
                if (FlaskRootAddress != 0)
                    return;
            }
        }

        public void UseFlask(PlayerFlask flask)
        {
            if (flask.Slot == 1)
                UseFlaskByKey(Keys.D1);
            else if (flask.Slot == 2)
                UseFlaskByKey(Keys.D2);
            else if (flask.Slot == 3)
                UseFlaskByKey(Keys.D3);
            else if (flask.Slot == 4)
                UseFlaskByKey(Keys.D4);
            else if (flask.Slot == 5)
                UseFlaskByKey(Keys.D5);
        }

        private void UseFlaskByKey(Keys key)
        {
            KeyPressRelease(key);
        }



        private string FlaskCFGPath => LocalPluginDirectory + @"\" + FlaskEffectsFile;

        private void ReadConfig()
        {
            if (File.Exists(FlaskCFGPath))
            {
                string json = File.ReadAllText(FlaskCFGPath);
                FlasksCfg = JsonConvert.DeserializeObject<FlasksConfig>(json);//, SettingsHub.jsonSettings
            }
            else
            {
                FlasksCfg = new FlasksConfig();


                FlasksCfg.Flasks.Add(new FlaskConfig()
                {
                    FlaskName = "Quicksilver Flask",
                    FlaskEffectName = "",
                    FlaskAction = FlaskAction.BeforeFight
                });

                using (var stream = new StreamWriter(File.Create(FlaskCFGPath)))
                {
                    string json = JsonConvert.SerializeObject(FlasksCfg, Formatting.Indented);//, SettingsHub.jsonSettings
                    stream.Write(json);
                }

                LogMessage($"Can't find flask effects file! {FlaskCFGPath} !", 10);
            }
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

        private FlaskConfig GetConfigForFlask(string flaskName)
        {
            return FlasksCfg.Flasks.Find(x => x.FlaskName == flaskName);
        }
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
        public int Slot;
        public string FlaskName;
        public string FlaskEffectName;
        public int FlaskCharges;
        public FlaskAction FlaskAction;
        public float DebugPosX;
        public Entity Item;
        public int UseCharges;
        public int MaxCharges;
    }




    public class FlasksConfig
    {
        public List<FlaskConfig> Flasks = new List<FlaskConfig>();
    }


    public class FlaskConfig
    {
        public string FlaskName;
        public string FlaskEffectName;
        public FlaskAction FlaskAction;
        public int UseCharges;
        public int MaxCharges;
    }


    public enum FlaskAction : int
    {
        LIFE = 0, //life, hybrid
        MANA, //mana, hybrid
        DEFENSE, //bismuth, jade, stibnite, granite,
                 //amethyst, ruby, sapphire, topaz,
                 // aquamarine, quartz
                 //MODS: iron skin, reflexes, gluttony,
                 // craving, resistance
        SPEEDRUN, //quicksilver, adrenaline,
        OFFENSE, //silver, sulphur, basalt, diamond
        POISON_IMMUNE,// MOD: curing
        FREEZE_IMMUNE,// MOD: heat
        IGNITE_IMMUNE,// MOD: dousing
        SHOCK_IMMUNE,// MOD: grounding
        BLEED_IMMUNE,// MOD: staunching
        CURSE_IMMUNE, // MOD: warding
        UNIQUE_FLASK
    }
    }