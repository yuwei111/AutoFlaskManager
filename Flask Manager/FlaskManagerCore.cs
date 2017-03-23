using PoeHUD.Controllers;
using PoeHUD.Poe.Components;
using PoeHUD.Plugins;
using System.Collections.Generic;
using PoeHUD.Poe;
using System.Threading;
using System;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.Elements;
using SharpDX;
using PoeHUD.Hud.Health;
using System.IO;
using Newtonsoft.Json;
using PoeHUD.Models.Enums;
using PoeHUD.Framework;
using System.Windows.Forms;
using FlaskManager.Flask_Components;
using FlaskManager.Helpers;

namespace FlaskManager.Flask_Manager
{
    class FlaskManagerCore : BaseSettingsPlugin<FlaskManagerSettings>
    {
        private readonly int logmsg_time = 3;
        private readonly int errmsg_time = 10;
        private KeyboardHelper keyboard;
        private Queue<Element> eleQueue;
        private Dictionary<string, float> debugDebuff;

        private bool isTown;
        private bool isHideout;
        private bool _WarnFlaskSpeed;
        private DebuffPanelConfig debuffInfo;
        private FlaskInformation flaskInfo;
        private FlaskKeys keyInfo;
        private List<PlayerFlask> playerFlaskList;

        private float moveCounter;
        private float lastManaUsed;
        private float lastLifeUsed;
        private float lastDefUsed;
        private float lastOffUsed;

        #region FlaskManagerInit
        public void SplashPage()
        {
            if (Settings.about.Value)
            {
                float X = (GameController.Window.GetWindowRectangle().Width / 2) - (475 / 2);
                float Y = (GameController.Window.GetWindowRectangle().Height / 2) - (395 / 2);
                RectangleF container = new RectangleF(X, Y, 475, 395);
                if (File.Exists(PluginDirectory + @"\splash\AutoFlaskManagerCredits.png"))
                    Graphics.DrawPluginImage(PluginDirectory + @"\splash\AutoFlaskManagerCredits.png", container);
                else
                {
                    LogMessage("Cannot find splash image, disable About.", logmsg_time);
                }
            }
            return;
        }
        public void BuffUi()
        {
            if (Settings.buffUiEnable.Value && !isTown)
            {
                float X = GameController.Window.GetWindowRectangle().Width * Settings.buff_PositionX.Value * .01f;
                float Y = GameController.Window.GetWindowRectangle().Height * Settings.buff_PositionY.Value * .01f;
                Vector2 position = new Vector2(X, Y);
                float maxWidth = 0;
                float maxheight = 0;
                var textColor = Color.WhiteSmoke;
                foreach (var buff in GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>().Buffs)
                {
                    var isInfinity = float.IsInfinity(buff.Timer);
                    var isFlaskBuff = buff.Name.ToLower().Contains("flask");
                    if (!Settings.enableFlaskAuraBuff.Value && (isInfinity || isFlaskBuff))
                        continue;

                    if (isFlaskBuff)
                        textColor = Color.SpringGreen;
                    else if (isInfinity)
                        textColor = Color.Purple;
                    else
                        textColor = Color.WhiteSmoke;

                    var size = Graphics.DrawText(buff.Name + ":" + buff.Timer, Settings.buff_TextSize.Value, position, textColor);
                    position.Y += size.Height;
                    maxheight += size.Height;
                    maxWidth = Math.Max(maxWidth, size.Width);
                }
                var background = new RectangleF(X, Y, maxWidth, maxheight);
                Graphics.DrawFrame(background, 5, Color.Black);
                Graphics.DrawImage("lightBackground.png", background);
            }
            return;
        }
        public void FlaskUi()
        {
            if (Settings.flaskUiEnable.Value)
            {
                float X = GameController.Window.GetWindowRectangle().Width * Settings.flask_PositionX.Value * .01f;
                float Y = GameController.Window.GetWindowRectangle().Height * Settings.flask_PositionY.Value * .01f;
                Vector2 position = new Vector2(X, Y);
                float maxWidth = 0;
                float maxheight = 0;
                Color textColor = Color.WhiteSmoke;

                foreach (var flasks in playerFlaskList.ToArray())
                {
                    if (!flasks.isEnabled)
                        textColor = Color.Red;
                    else if (flasks.flaskRarity == ItemRarity.Magic)
                        textColor = Color.CornflowerBlue;
                    else if (flasks.flaskRarity == ItemRarity.Unique)
                        textColor = Color.Chocolate;
                    else
                        textColor = Color.WhiteSmoke;

                    var size = Graphics.DrawText(flasks.FlaskName, Settings.flask_TextSize.Value, position, textColor);
                    position.Y += size.Height;
                    maxheight += size.Height;
                    maxWidth = Math.Max(maxWidth, size.Width);
                }
                var background = new RectangleF(X, Y, maxWidth, maxheight);
                Graphics.DrawFrame(background, 5, Color.Black);
                Graphics.DrawImage("lightBackground.png", background);
            }
            return;
        }
        public override void Render()
        {
            base.Render();
            if (Settings.Enable.Value)
            {
                // Panic Quit Key.
                if (WinApi.IsKeyDown(Keys.F4))
                    PoeProcessHandler.ExitPoe("cports.exe", "/close * * * * " + GameController.Window.Process.ProcessName + ".exe");

                FlaskUi();
                BuffUi();
                SplashPage();
            }
        }
        public override void OnClose()
        {
            base.OnClose();
            if (Settings.debugMode.Value)
                foreach (var key in debugDebuff)
                {
                    File.AppendAllText("autoflaskmanagerDebug.log", DateTime.Now + " " + key.Key + " : " + key.Value + Environment.NewLine);
                }
        }
        public override void Initialise()
        {
            PluginName = "Flask Manager";
            var bindFilename = PluginDirectory + @"/config/flaskbind.json";
            var flaskFilename = PluginDirectory + @"/config/flaskinfo.json";
            var debufFilename = "config/debuffPanel.json";
            if (!File.Exists(bindFilename))
            {
                LogError("Cannot find " + bindFilename + " file. This plugin will exit.", errmsg_time);
                return;
            }
            if (!File.Exists(flaskFilename))
            {
                LogError("Cannot find " + flaskFilename + " file. This plugin will exit.", errmsg_time);
                return;
            }
            if (!File.Exists(debufFilename))
            {
                LogError("Cannot find " + debufFilename + " file. This plugin will exit.", errmsg_time);
                return;
            }
            string keyString = File.ReadAllText(bindFilename);
            string flaskString = File.ReadAllText(flaskFilename);
            string json = File.ReadAllText(debufFilename);
            keyInfo = JsonConvert.DeserializeObject<FlaskKeys>(keyString);
            debuffInfo = JsonConvert.DeserializeObject<DebuffPanelConfig>(json);
            flaskInfo = JsonConvert.DeserializeObject<FlaskInformation>(flaskString);
            playerFlaskList = new List<PlayerFlask>();
            eleQueue = new Queue<Element>();
            debugDebuff = new Dictionary<string, float>();
            OnFlaskManagerToggle();
            GameController.Area.OnAreaChange += area => OnAreaChange(area);
            Settings.Enable.OnValueChanged += OnFlaskManagerToggle;
        }
        private void OnFlaskManagerToggle()
        {
            try
            {
                if (Settings.Enable.Value)
                {
                    if (Settings.debugMode.Value)
                        LogMessage("Enabling FlaskManager.", logmsg_time);
                    moveCounter = 0f;
                    lastManaUsed = 100000f;
                    lastLifeUsed = 100000f;
                    lastDefUsed = 100000f;
                    lastOffUsed = 100000f;
                    isTown = true;
                    isHideout = false;
                    _WarnFlaskSpeed = true;
                    keyboard = new KeyboardHelper(GameController);
                    //We are creating our plugin thread inside PoEHUD!
                    Thread flaskThread = new Thread(FlaskThread) { IsBackground = true };
                    flaskThread.Start();
                }
                else
                {
                    if (Settings.debugMode.Value)
                        LogMessage("Disabling FlaskManager.", logmsg_time);
                    playerFlaskList.Clear();
                }
            }
            catch (Exception)
            {

                LogError("Error Starting FlaskManager Thread.", errmsg_time);
            }
        }
        private void OnAreaChange(AreaController area)
        {
            if (Settings.Enable.Value)
            {
                LogMessage("Area has been changed. Loading flasks info.", logmsg_time);
                isHideout = area.CurrentArea.IsHideout;
                isTown = area.CurrentArea.IsTown;
            }
        }
        #endregion
        #region Finding Flasks
        //Breath First Search for finding flask root.
        private Element FindFlaskRoot()
        {
            Element current = null;
            InventoryItemIcon itm = null;
            Entity item = null;
            while (eleQueue.Count > 0)
            {
                current = eleQueue.Dequeue();
                if (current == null)
                    continue;
                foreach (var child in current.Children)
                {
                    eleQueue.Enqueue(child);
                }

                itm = current.AsObject<InventoryItemIcon>();

                if (itm.ToolTipType == ToolTipType.InventoryItem)
                {
                    item = itm.Item;
                    if (item != null && item.HasComponent<Flask>())
                    {
                        eleQueue.Clear();
                        return current.Parent;
                    }
                }
            }
            return null;
        }
        private Element GetFlaskRoot()
        {
            try
            {
                eleQueue.Clear();
                eleQueue.Enqueue(GameController.Game.IngameState.UIRoot);
                return FindFlaskRoot();
            }
            catch (Exception e)
            {
                if (Settings.debugMode.Value)
                {
                    LogError("Warning: Cannot find Flask list.", errmsg_time);
                    LogError(e.Message + e.StackTrace, errmsg_time);
                }
                return null;
            }
        }
        private bool GettingAllFlaskInfo(Element flaskRoot)
        {
            if (Settings.debugMode.Value)
                LogMessage("Getting Inventory Flasks info.", logmsg_time);
            playerFlaskList.Clear();
            try
            {
                var flasksEquipped = flaskRoot.Children;
                for (int j = 0; j < flasksEquipped.Count; j++)
                {
                    InventoryItemIcon flask = flasksEquipped[j].AsObject<InventoryItemIcon>();
                    Entity flaskItem = flask.Item;
                    Charges flaskCharges = flaskItem.GetComponent<Charges>();
                    Mods flaskMods = flaskItem.GetComponent<Mods>();
                    PlayerFlask newFlask = new PlayerFlask();

                    newFlask.SetSettings(Settings);
                    newFlask.isInstant = false;
                    newFlask.Slot = flask.InventPosX;
                    newFlask.Item = flaskItem;
                    newFlask.MaxCharges = flaskCharges.ChargesMax;
                    newFlask.UseCharges = flaskCharges.ChargesPerUse;
                    newFlask.CurrentCharges = flaskCharges.NumCharges;
                    newFlask.flaskRarity = flaskMods.ItemRarity;
                    newFlask.FlaskName = GameController.Files.BaseItemTypes.Translate(flaskItem.Path).BaseName;
                    newFlask.FlaskAction2 = FlaskAction.NONE;
                    newFlask.FlaskAction1 = FlaskAction.NONE;

                    //Checking flask action based on flask name type.
                    if (!flaskInfo.FlaskTypes.TryGetValue(newFlask.FlaskName, out newFlask.FlaskAction1))
                        LogError("Error: " + newFlask.FlaskName + " name not found. Report this error message.", errmsg_time);

                    //Checking for unique flasks.
                    if (flaskMods.ItemRarity == ItemRarity.Unique)
                    {
                        newFlask.FlaskName = flaskMods.UniqueName;
                        if (Settings.uniqFlaskEnable.Value)
                        {
                            //Enabling Unique flask action 2.
                            if (!flaskInfo.UniqueFlaskNames.TryGetValue(newFlask.FlaskName, out newFlask.FlaskAction2))
                                LogError("Error: " + newFlask.FlaskName + " unique name not found. Report this error message.", errmsg_time);
                        }
                        else
                        {
                            //Disabling Unique Flask actions.
                            newFlask.FlaskAction1 = FlaskAction.NONE;
                            newFlask.FlaskAction2 = FlaskAction.NONE;
                        }
                    }

                    //Checking flask mods.
                    FlaskAction action2 = FlaskAction.NONE;
                    foreach (var mod in flaskMods.ItemMods)
                    {
                        if (mod.Name.ToLower().Contains("flaskchargesused"))
                            newFlask.UseCharges = (int)Math.Floor(newFlask.UseCharges + ((double)(newFlask.UseCharges) * mod.Value1 / 100));

                        if (mod.Name.ToLower().Contains("instant"))
                            newFlask.isInstant = true;

                        // We have already decided action2 for unique flasks.
                        if (flaskMods.ItemRarity == ItemRarity.Unique)
                            continue;

                        if (!flaskInfo.FlaskMods.TryGetValue(mod.Name, out action2))
                            LogError("Error: " + mod.Name + " mod not found. Is it unique flask? If not, report this error message.", errmsg_time);
                        else if (action2 != FlaskAction.IGNORE)
                            newFlask.FlaskAction2 = action2;
                    }

                    // Speedrun mod on mana/life flask wouldn't work when full mana/life is full respectively,
                    // So we will ignore speedrun mod from mana/life flask. Other mods
                    // on mana/life flasks will work.
                    if (newFlask.FlaskAction2 == FlaskAction.SPEEDRUN &&
                        (newFlask.FlaskAction1 == FlaskAction.LIFE ||
                         newFlask.FlaskAction1 == FlaskAction.MANA ||
                         newFlask.FlaskAction1 == FlaskAction.HYBRID))
                    {
                        newFlask.FlaskAction2 = FlaskAction.NONE;
                        if (_WarnFlaskSpeed)
                        {
                            LogError("Warning: Speed Run mod is ignored on mana/life/hybrid flasks. Use Alt Orbs on those flasks.", errmsg_time);
                            _WarnFlaskSpeed = false;
                        }
                    }

                    if (Settings.disableLifeSecUse.Value)
                    {
                        if (newFlask.FlaskAction1 == FlaskAction.LIFE || newFlask.FlaskAction1 == FlaskAction.HYBRID)
                            if (newFlask.FlaskAction2 == FlaskAction.OFFENSE || newFlask.FlaskAction2 == FlaskAction.DEFENSE)
                                newFlask.FlaskAction2 = FlaskAction.NONE;
                    }

                    if (Settings.treatOffenAsDef.Value)
                    {
                        if (newFlask.FlaskAction1 == FlaskAction.OFFENSE)
                            newFlask.FlaskAction1 = FlaskAction.DEFENSE;
                        if (newFlask.FlaskAction2 == FlaskAction.OFFENSE)
                            newFlask.FlaskAction2 = FlaskAction.DEFENSE;
                    }
                    newFlask.EnableDisableFlask();
                    playerFlaskList.Add(newFlask);
                }
            }
            catch (Exception e)
            {
                if (Settings.debugMode.Value)
                {
                    LogError("Warning: Error getting all flask Informations.", errmsg_time);
                    LogError(e.Message + e.StackTrace, errmsg_time);
                }
                playerFlaskList.Clear();
                return false;
            }
            playerFlaskList.Sort((x, y) => x.Slot.CompareTo(y.Slot));
            return true;
        }
        #endregion
        #region Flask Helper Functions
        //Parameters:
        // type1 and type2 define the type of flask you wanna drink.
        // reason: is just for debugging output to see where does the drinking flask request came from
        // minRequiredCharges: Min number of charges a flask must have to consider it a valid flask to drink.
        // shouldDrinkAll: if you want to drink all the flasks of type1,type2 (true) or just first in the list(false).
        private bool FindDrinkFlask(FlaskAction type1, FlaskAction type2, string reason, int minRequiredCharge = 0, bool shouldDrinkAll = false)
        {
            bool hasDrunk = false;
            var flaskList = playerFlaskList.FindAll(x => x.CurrentCharges >= minRequiredCharge &&
                                (x.FlaskAction1 == type1 || x.FlaskAction2 == type2) && x.isEnabled);
            foreach (var flask in flaskList)
            {
                if (flask.CurrentCharges >= flask.UseCharges)
                {
                    keyboard.setLatency(GameController.Game.IngameState.CurLatency);
                    if (!keyboard.KeyPressRelease(keyInfo.k[flask.Slot]))
                        LogError("Warning: High latency ( more than 1000 millisecond ), plugin will fail to work properly.", errmsg_time);
                    flask.UpdateFlaskChargesInfo();
                    if (Settings.debugMode.Value)
                        LogMessage("Just Drank Flask on key " + keyInfo.k[flask.Slot] + " cuz of " + reason, logmsg_time);
                    // if there are multiple flasks, drinking 1 of them at a time is enough.
                    hasDrunk = true;
                    if (!shouldDrinkAll)
                        return hasDrunk;
                }
                else
                {
                    flask.UpdateFlaskChargesInfo();
                }

            }
            return hasDrunk;
        }
        private bool HasDebuff(Dictionary<string, int> dictionary, string buffName, bool isHostile)
        {
            int filterId;
            if (dictionary.TryGetValue(buffName, out filterId))
            {
                return filterId == 0 || isHostile == (filterId == 1);
            }
            return false;
        }
        #endregion

        #region Chicken Auto Quit
        private void AutoChicken()
        {
            var LocalPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var PlayerHealth = LocalPlayer.GetComponent<Life>();
            if (Settings.isPercentQuit.Value && LocalPlayer.IsValid)
            {
                if (Math.Round(PlayerHealth.HPPercentage, 3) * 100 < (Settings.percentHPQuit.Value))
                {
                    try
                    {
                        PoeProcessHandler.ExitPoe("cports.exe", "/close * * * * " + GameController.Window.Process.ProcessName + ".exe");
                        if (Settings.debugMode.Value)
                            File.AppendAllText("autoflaskmanagerDebug.log", DateTime.Now +
                                " AUTO QUIT: Your Health was at: " + (Math.Round(PlayerHealth.HPPercentage, 3) * 100 +
                                "%" + Environment.NewLine));
                    }
                    catch (Exception)
                    {
                        LogError("Error: Cannot find cports.exe, you must die now!", errmsg_time);
                    }
                }
                if (PlayerHealth.MaxES > 0 && (Math.Round(PlayerHealth.ESPercentage, 3) * 100 < (Settings.percentESQuit.Value)))
                {
                    try
                    {

                        PoeProcessHandler.ExitPoe("cports.exe", "/close * * * * " + GameController.Window.Process.ProcessName + ".exe");
                        if (Settings.debugMode.Value)
                            File.AppendAllText("autoflaskmanagerDebug.log", DateTime.Now +
                                " AUTO QUIT: Your Energy Shield was at: " + (Math.Round(PlayerHealth.ESPercentage, 3) * 100 +
                                "%" + Environment.NewLine));
                    }
                    catch (Exception)
                    {
                        LogError("Error: Cannot find cports.exe, you must die now!", errmsg_time);
                    }
                }
            }
            return;
        }
        #endregion 
        #region Auto Health Flasks
        private bool InstantLifeFlask(Life PlayerHealth)
        {
            bool ret = false;
            if (PlayerHealth.HPPercentage * 100 < Settings.instantPerHPFlask.Value)
            {
                var flaskList = playerFlaskList.FindAll(x => x.isInstant == x.isEnabled == true &&
                          (x.FlaskAction1 == FlaskAction.LIFE || x.FlaskAction1 == FlaskAction.HYBRID));
                foreach (PlayerFlask flask in flaskList)
                {
                    if (flask.CurrentCharges >= flask.UseCharges)
                    {
                        keyboard.setLatency(GameController.Game.IngameState.CurLatency);
                        if (!keyboard.KeyPressRelease(keyInfo.k[flask.Slot]))
                            LogError("Warning: High latency ( more than 1000 millisecond ), plugin will fail to work properly.", errmsg_time);
                        flask.UpdateFlaskChargesInfo();
                        if (Settings.debugMode.Value)
                            LogMessage("Just Drank Instant Flask on key " + keyInfo.k[flask.Slot] + " cuz of Very Low Life", logmsg_time);
                        ret = true;
                    }
                    else
                    {
                        flask.UpdateFlaskChargesInfo();
                    }
                }
            }
            return ret;
        }
        private void LifeLogic()
        {
            if (!GameController.Game.IngameState.Data.LocalPlayer.IsValid || !Settings.autoFlask.Value)
                return;

            Entity LocalPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            Life PlayerHealth = LocalPlayer.GetComponent<Life>();
            lastLifeUsed += 100f;
            if (InstantLifeFlask(PlayerHealth))
                return;
            if (lastLifeUsed < Settings.HPDelay.Value)
                return;
            if (PlayerHealth.HPPercentage * 100 < Settings.perHPFlask.Value)
            {
                if (FindDrinkFlask(FlaskAction.LIFE, FlaskAction.IGNORE, "Low life"))
                    lastLifeUsed = 0f;
                else if (FindDrinkFlask(FlaskAction.HYBRID, FlaskAction.IGNORE, "Low life"))
                    lastLifeUsed = 0f;
            }
        }
        #endregion
        #region Auto Mana Flasks
        private void ManaLogic()
        {
            if (!Settings.autoFlask.Value || !GameController.Game.IngameState.Data.LocalPlayer.IsValid)
                return;

            Entity LocalPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            Life PlayerHealth = LocalPlayer.GetComponent<Life>();
            lastManaUsed += 100f;
            if (lastManaUsed < Settings.ManaDelay.Value)
                return;
            if (PlayerHealth.MPPercentage * 100 < Settings.PerManaFlask.Value)
            {
                if (FindDrinkFlask(FlaskAction.MANA, FlaskAction.IGNORE, "Low Mana"))
                    lastManaUsed = 0f;
                else if (FindDrinkFlask(FlaskAction.HYBRID, FlaskAction.IGNORE, "Low Mana"))
                    lastManaUsed = 0f;
            }
        }
        #endregion
        #region Auto Ailment Flasks
        private void AilmentLogic()
        {
            var LocalPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var PlayerHealth = LocalPlayer.GetComponent<Life>();
            foreach (var buff in PlayerHealth.Buffs)
            {
                if (float.IsInfinity(buff.Timer))
                    continue;

                var buffName = buff.Name;

                if (Settings.debugMode.Value)
                    if (debugDebuff.ContainsKey(buffName))
                        debugDebuff[buffName] = Math.Max(buff.Timer, debugDebuff[buffName]);
                    else
                        debugDebuff[buffName] = buff.Timer;

                if (!Settings.remAilment.Value)
                    return;

                var tmpResult = false;
                if (Settings.remPoison.Value && HasDebuff(debuffInfo.Poisoned, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskAction.IGNORE, FlaskAction.POISON_IMMUNE, "Poisoned");
                    if (Settings.debugMode.Value)
                        LogMessage("Poison -> hasDrunkFlask:" + tmpResult, logmsg_time);
                }
                else if (Settings.remFrozen.Value && HasDebuff(debuffInfo.ChilledFrozen, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskAction.IGNORE, FlaskAction.FREEZE_IMMUNE, "Frozen");
                    if (Settings.debugMode.Value)
                        LogMessage("Frozen -> hasDrunkFlask:" + tmpResult, logmsg_time);
                }
                else if (Settings.remBurning.Value && HasDebuff(debuffInfo.Burning, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskAction.IGNORE, FlaskAction.IGNITE_IMMUNE, "Burning");
                    if (Settings.debugMode.Value)
                        LogMessage("Burning -> hasDrunkFlask:" + tmpResult, logmsg_time);
                }
                else if (Settings.remShocked.Value && HasDebuff(debuffInfo.Shocked, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskAction.IGNORE, FlaskAction.SHOCK_IMMUNE, "Shocked");
                    if (Settings.debugMode.Value)
                        LogMessage("Shock -> hasDrunkFlask:" + tmpResult, logmsg_time);
                }
                else if (Settings.remCurse.Value && HasDebuff(debuffInfo.WeakenedSlowed, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskAction.IGNORE, FlaskAction.CURSE_IMMUNE, "Cursed");
                    if (Settings.debugMode.Value)
                        LogMessage("Curse -> hasDrunkFlask:" + tmpResult, logmsg_time);
                }
                else if (Settings.remBleed.Value)
                {
                    if (HasDebuff(debuffInfo.Bleeding, buffName, false))
                    {
                        tmpResult = FindDrinkFlask(FlaskAction.IGNORE, FlaskAction.BLEED_IMMUNE, "Bleeding");
                        if (Settings.debugMode.Value)
                            LogMessage("Bleeding -> hasDrunkFlask:" + tmpResult, logmsg_time);
                    }
                    else if (HasDebuff(debuffInfo.Corruption, buffName, false) && buff.Charges >= Settings.corrptCount)
                    {
                        tmpResult = FindDrinkFlask(FlaskAction.IGNORE, FlaskAction.BLEED_IMMUNE, "Corruption");
                        if (Settings.debugMode.Value)
                            LogMessage("Corruption -> hasDrunkFlask:" + tmpResult, logmsg_time);
                    }
                }
            }
        }
        #endregion
        #region Auto Quicksilver Flasks
        private void SpeedFlaskLogic()
        {
            var LocalPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var PlayerHealth = LocalPlayer.GetComponent<Life>();
            var PlayerMovement = LocalPlayer.GetComponent<Actor>();
            moveCounter = PlayerMovement.isMoving ? moveCounter += 100f : 0;
            if (LocalPlayer.IsValid && Settings.quicksilverEnable.Value && moveCounter >= Settings.quicksilverDurration.Value &&
                !PlayerHealth.HasBuff("flask_bonus_movement_speed") &&
                !PlayerHealth.HasBuff("flask_utility_sprint"))
            {
                FindDrinkFlask(FlaskAction.SPEEDRUN, FlaskAction.SPEEDRUN, "Moving Around", Settings.quicksilverUseWhenCharges.Value);
            }
        }
        #endregion
        #region Defensive Flasks
        private void DefensiveFlask()
        {
            var LocalPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var PlayerHealth = LocalPlayer.GetComponent<Life>();
            lastDefUsed += 100f;
            if (lastDefUsed < Settings.DefensiveDelay.Value)
                return;
            if (Settings.defFlaskEnable.Value && LocalPlayer.IsValid)
            {
                if (PlayerHealth.HPPercentage * 100 < Settings.hPDefensive.Value ||
                    (PlayerHealth.MaxES > 0 && PlayerHealth.ESPercentage * 100 < Settings.eSDefensive.Value))
                {
                    if (FindDrinkFlask(FlaskAction.DEFENSE, FlaskAction.DEFENSE, "Defensive Action", 0, Settings.defensiveDrinkAll.Value))
                        lastDefUsed = 0f;
                }
            }
        }
        #endregion
        #region Offensive Flasks
        private void OffensiveFlask()
        {
            var LocalPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var PlayerHealth = LocalPlayer.GetComponent<Life>();
            var IsAttacking = (LocalPlayer.GetComponent<Actor>().ActionId & 2) > 0;
            lastOffUsed += 100f;
            if (!Settings.offFlaskEnable.Value || !LocalPlayer.IsValid)
                return;
            if (!Settings.offensiveWhenAttacking.Value && !Settings.offensiveWhenLifeES.Value)
            {
                LogError("Atleast Select 1 offensive flask Method Life/ES OR When Attacking. OR Disable offensive flask.", errmsg_time);
                return;
            }
            if (lastOffUsed < Settings.OffensiveDelay.Value)
                return;
            if (Settings.offensiveWhenAttacking.Value && !IsAttacking)
                return;
            if (Settings.offensiveWhenLifeES.Value && (PlayerHealth.HPPercentage * 100 > Settings.hpOffensive.Value &&
                    (PlayerHealth.MaxES <= 0 || PlayerHealth.ESPercentage * 100 > Settings.esOffensive.Value)))
                return;

            if (FindDrinkFlask(FlaskAction.OFFENSE, FlaskAction.OFFENSE, "Offensive Action", Settings.OffensiveUseWhenCharges.Value, Settings.offensiveDrinkAll.Value))
                lastOffUsed = 0f;
        }
        #endregion

        #region Plugin Thread
        private void FlaskMain()
        {
            if (!GameController.Game.IngameState.Data.LocalPlayer.IsValid)
                return;

            Element flaskRoot = GetFlaskRoot();

            if (flaskRoot == null)
                return;

            if (flaskRoot.Children.Count > 0 && flaskRoot.Children.Count != playerFlaskList.Count)
            {
                if (Settings.debugMode.Value)
                    LogMessage("Invalid Flask Count, Recalculating it.", logmsg_time);
                if (!GettingAllFlaskInfo(flaskRoot))
                    return;
            }

            for (int j = 0; j < playerFlaskList.Count; j++)
                if (!playerFlaskList[j].Item.IsValid)
                {
                    if (Settings.debugMode.Value)
                        LogMessage("Invalid Flask Data, Recalculating it.", logmsg_time);
                    GettingAllFlaskInfo(flaskRoot);
                    return;
                }

            var playerLife = GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            if (playerLife == null || isTown || isHideout || playerLife.CurHP <= 0 || playerLife.HasBuff("grace_period"))
                return;

            SpeedFlaskLogic();
            ManaLogic();
            LifeLogic();
            AilmentLogic();
            DefensiveFlask();
            OffensiveFlask();
            return;
        }
        private void FlaskThread()
        {
            while (Settings.Enable.Value)
            {
                FlaskMain();
                for (int j = 0; j < 10; j++)
                {
                    AutoChicken();
                    Thread.Sleep(10);
                }
            }
        }
        #endregion
    }
}
