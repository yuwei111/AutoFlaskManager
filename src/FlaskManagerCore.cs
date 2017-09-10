using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FlaskManager.FlaskComponents;
using Newtonsoft.Json;
using PoeHUD.Controllers;
using PoeHUD.Hud.Health;
using PoeHUD.Models.Enums;
using PoeHUD.Plugins;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.EntityComponents;
using SharpDX;

namespace FlaskManager
{
    internal class FlaskManagerCore : BaseSettingsPlugin<FlaskManagerSettings>
    {
        private const int LogmsgTime = 3;
        private const int ErrmsgTime = 10;
        private KeyboardHelper _keyboard;
        private Dictionary<string, float> _debugDebuff;

        private bool _isTown;
        private bool _isHideout;
        private bool _warnFlaskSpeed;
        private DebuffPanelConfig _debuffInfo;
        private FlaskInformation _flaskInfo;
        private FlaskKeys _keyInfo;
        private List<PlayerFlask> _playerFlaskList;

        private float _moveCounter;
        private float _lastManaUsed;
        private float _lastLifeUsed;
        private float _lastDefUsed;
        private float _lastOffUsed;

        #region FlaskManagerInit
        public void SplashPage()
        {
            if (!Settings.About.Value) return;
            var x = (GameController.Window.GetWindowRectangle().Width / 2) - 237;
            var y = (GameController.Window.GetWindowRectangle().Height / 2) - 197;
            var container = new RectangleF(x, y, 475, 395);
            if (File.Exists(PluginDirectory + @"\splash\AutoFlaskManagerCredits.png"))
                Graphics.DrawPluginImage(PluginDirectory + @"\splash\AutoFlaskManagerCredits.png", container);
            else
            {
                LogMessage("Cannot find splash image, disable About.", LogmsgTime);
            }
        }
        public void BuffUi()
        {
            if (!Settings.BuffUiEnable.Value || _isTown) return;
            var x = GameController.Window.GetWindowRectangle().Width * Settings.BuffPositionX.Value * .01f;
            var y = GameController.Window.GetWindowRectangle().Height * Settings.BuffPositionY.Value * .01f;
            var position = new Vector2(x, y);
            float maxWidth = 0;
            float maxheight = 0;
            foreach (var buff in GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>().Buffs)
            {
                var isInfinity = float.IsInfinity(buff.Timer);
                var isFlaskBuff = buff.Name.ToLower().Contains("flask");
                if (!Settings.EnableFlaskAuraBuff.Value && (isInfinity || isFlaskBuff))
                    continue;

                Color textColor;
                if (isFlaskBuff)
                    textColor = Color.SpringGreen;
                else if (isInfinity)
                    textColor = Color.Purple;
                else
                    textColor = Color.WhiteSmoke;

                var size = Graphics.DrawText(buff.Name + ":" + buff.Timer, Settings.BuffTextSize.Value, position, textColor);
                position.Y += size.Height;
                maxheight += size.Height;
                maxWidth = Math.Max(maxWidth, size.Width);
            }
            var background = new RectangleF(x, y, maxWidth, maxheight);
            Graphics.DrawFrame(background, 5, Color.Black);
            Graphics.DrawImage("lightBackground.png", background);
        }
        public void FlaskUi()
        {
            if (!Settings.FlaskUiEnable.Value) return;
            var x = GameController.Window.GetWindowRectangle().Width * Settings.FlaskPositionX.Value * .01f;
            var y = GameController.Window.GetWindowRectangle().Height * Settings.FlaskPositionY.Value * .01f;
            var position = new Vector2(x, y);
            float maxWidth = 0;
            float maxheight = 0;
            var textColor = Color.WhiteSmoke;

            foreach (var flasks in _playerFlaskList.ToArray())
            {
                if (!flasks.IsValid)
                    continue;
                if (!flasks.IsEnabled)
                    textColor = Color.Red;
                else switch (flasks.FlaskRarity)
                {
                    case ItemRarity.Magic:
                        textColor = Color.CornflowerBlue;
                        break;
                    case ItemRarity.Unique:
                        textColor = Color.Chocolate;
                        break;
                    case ItemRarity.Normal:
                        break;
                    case ItemRarity.Rare:
                        break;
                    default:
                        textColor = Color.WhiteSmoke;
                        break;
                }

                var size = Graphics.DrawText(flasks.FlaskName, Settings.FlaskTextSize.Value, position, textColor);
                position.Y += size.Height;
                maxheight += size.Height;
                maxWidth = Math.Max(maxWidth, size.Width);
            }
            var background = new RectangleF(x, y, maxWidth, maxheight);
            Graphics.DrawFrame(background, 5, Color.Black);
            Graphics.DrawImage("lightBackground.png", background);
        }
        public override void Render()
        {
            base.Render();
            if (!Settings.Enable.Value) return;
            FlaskUi();
            BuffUi();
            SplashPage();
        }
        public override void OnClose()
        {
            base.OnClose();
            if (!Settings.DebugMode.Value) return;
            foreach (var key in _debugDebuff)
            {
                File.AppendAllText(PluginDirectory + @"/debug.log", DateTime.Now + " " + key.Key + " : " + key.Value + Environment.NewLine);
            }
        }
        public override void Initialise()
        {
            PluginName = "Flask Manager";
            var bindFilename = PluginDirectory + @"/config/flaskbind.json";
            var flaskFilename = PluginDirectory + @"/config/flaskinfo.json";
            const string debufFilename = "config/debuffPanel.json";
            if (!File.Exists(bindFilename))
            {
                LogError("Cannot find " + bindFilename + " file. This plugin will exit.", ErrmsgTime);
                return;
            }
            if (!File.Exists(flaskFilename))
            {
                LogError("Cannot find " + flaskFilename + " file. This plugin will exit.", ErrmsgTime);
                return;
            }
            if (!File.Exists(debufFilename))
            {
                LogError("Cannot find " + debufFilename + " file. This plugin will exit.", ErrmsgTime);
                return;
            }
            var keyString = File.ReadAllText(bindFilename);
            var flaskString = File.ReadAllText(flaskFilename);
            var json = File.ReadAllText(debufFilename);
            _keyInfo = JsonConvert.DeserializeObject<FlaskKeys>(keyString);
            _debuffInfo = JsonConvert.DeserializeObject<DebuffPanelConfig>(json);
            _flaskInfo = JsonConvert.DeserializeObject<FlaskInformation>(flaskString);
            _playerFlaskList = new List<PlayerFlask>(5);
            _debugDebuff = new Dictionary<string, float>();
            OnFlaskManagerToggle();
            Settings.Enable.OnValueChanged += OnFlaskManagerToggle;
        }
        private void OnAreaChange(AreaController area)
        {
            if (Settings.Enable.Value)
            {
                LogMessage("Area has been changed. Loading flasks info.", LogmsgTime);
                _isHideout = area.CurrentArea.IsHideout;
                _isTown = area.CurrentArea.IsTown;
                foreach (var flask in _playerFlaskList)
                {
                    flask.TotalTimeUsed = (flask.IsInstant) ? 100000 : 0;
                }
            }
        }
        private void OnFlaskManagerToggle()
        {
            try
            {
                if (Settings.Enable.Value)
                {
                    if (Settings.DebugMode.Value)
                        LogMessage("Enabling FlaskManager.", LogmsgTime);
                    GameController.Area.OnAreaChange += OnAreaChange;
                    _moveCounter = 0f;
                    _lastManaUsed = 100000f;
                    _lastLifeUsed = 100000f;
                    _lastDefUsed = 100000f;
                    _lastOffUsed = 100000f;
                    _isTown = true;
                    _isHideout = false;
                    _warnFlaskSpeed = true;
                    _keyboard = new KeyboardHelper(GameController);
                    _playerFlaskList.Clear();
                    for (var i = 0; i < 5; i++)
                        _playerFlaskList.Add(new PlayerFlask(i));

                    //We are creating our plugin thread inside PoEHUD!
                    var flaskThread = new Thread(FlaskThread) { IsBackground = true };
                    flaskThread.Start();
                }
                else
                {
                    if (Settings.DebugMode.Value)
                        LogMessage("Disabling FlaskManager.", LogmsgTime);
                    GameController.Area.OnAreaChange -= OnAreaChange;
                    _playerFlaskList.Clear();
                }
            }
            catch (Exception)
            {

                LogError("Error Starting FlaskManager Thread.", ErrmsgTime);
            }
        }
        private void FlaskThread()
        {
            while (Settings.Enable.Value)
            {
                FlaskMain();
                Thread.Sleep(100);
            }
        }
        #endregion
        #region GettingFlaskDetails
        private bool GettingAllFlaskInfo()
        {
            try
            {
                for (var j = 0; j < 5; j++)
                {
                    //InventoryItemIcon flask = flasksEquipped[j].AsObject<InventoryItemIcon>();
                    var flaskItem = GameController.Game.IngameState.IngameUi.InventoryPanel[InventoryIndex.Flask][j, 0, 0];
                    if (flaskItem == null || flaskItem.Address == 0)
                    {
                        _playerFlaskList[j].IsValid = false;
                        continue;
                    }

                    var flaskCharges = flaskItem.GetComponent<Charges>();
                    var flaskMods = flaskItem.GetComponent<Mods>();
                    _playerFlaskList[j].IsInstant = false;
                    float tmpUseCharges = flaskCharges.ChargesPerUse;
                    _playerFlaskList[j].CurrentCharges = flaskCharges.NumCharges;
                    _playerFlaskList[j].FlaskRarity = flaskMods.ItemRarity;
                    _playerFlaskList[j].FlaskName = GameController.Files.BaseItemTypes.Translate(flaskItem.Path).BaseName;
                    _playerFlaskList[j].FlaskAction2 = FlaskActions.None;
                    _playerFlaskList[j].FlaskAction1 = FlaskActions.None;

                    //Checking flask action based on flask name type.
                    if (!_flaskInfo.FlaskTypes.TryGetValue(_playerFlaskList[j].FlaskName, out _playerFlaskList[j].FlaskAction1))
                        LogError("Error: " + _playerFlaskList[j].FlaskName + " name not found. Report this error message.", ErrmsgTime);

                    //Checking for unique flasks.
                    if (flaskMods.ItemRarity == ItemRarity.Unique)
                    {
                        _playerFlaskList[j].FlaskName = flaskMods.UniqueName;
                        if (Settings.UniqFlaskEnable.Value)
                        {
                            //Enabling Unique flask action 2.
                            if (!_flaskInfo.UniqueFlaskNames.TryGetValue(_playerFlaskList[j].FlaskName, out _playerFlaskList[j].FlaskAction2))
                                LogError("Error: " + _playerFlaskList[j].FlaskName + " unique name not found. Report this error message.", ErrmsgTime);
                        }
                        else
                        {
                            //Disabling Unique Flask actions.
                            _playerFlaskList[j].FlaskAction1 = FlaskActions.None;
                            _playerFlaskList[j].FlaskAction2 = FlaskActions.None;
                        }
                    }

                    if (Settings.ChargeReduction.Value > 0)
                        tmpUseCharges = ((100 - Settings.ChargeReduction.Value) / 100) * tmpUseCharges;

                    //Checking flask mods.
                    FlaskActions action2 = FlaskActions.Ignore;
                    foreach (var mod in flaskMods.ItemMods)
                    {
                        if (mod.Name.ToLower().Contains("flaskchargesused"))
                            tmpUseCharges = ((100 + (float)mod.Value1) / 100) * tmpUseCharges;

                        if (mod.Name.ToLower().Contains("instant"))
                            _playerFlaskList[j].IsInstant = true;

                        // We have already decided action2 for unique flasks.
                        if (flaskMods.ItemRarity == ItemRarity.Unique)
                            continue;

                        if (!_flaskInfo.FlaskMods.TryGetValue(mod.Name, out action2))
                            LogError("Error: " + mod.Name + " mod not found. Is it unique flask? If not, report this error message.", ErrmsgTime);
                        else if (action2 != FlaskActions.Ignore)
                            _playerFlaskList[j].FlaskAction2 = action2;
                    }

                    // Speedrun mod on mana/life flask wouldn't work when full mana/life is full respectively,
                    // So we will ignore speedrun mod from mana/life flask. Other mods
                    // on mana/life flasks will work.
                    if (_playerFlaskList[j].FlaskAction2 == FlaskActions.Speedrun &&
                        (_playerFlaskList[j].FlaskAction1 == FlaskActions.Life ||
                         _playerFlaskList[j].FlaskAction1 == FlaskActions.Mana ||
                         _playerFlaskList[j].FlaskAction1 == FlaskActions.Hybrid))
                    {
                        _playerFlaskList[j].FlaskAction2 = FlaskActions.None;
                        if (_warnFlaskSpeed)
                        {
                            LogError("Warning: Speed Run mod is ignored on mana/life/hybrid flasks. Use Alt Orbs on those flasks.", ErrmsgTime);
                            _warnFlaskSpeed = false;
                        }
                    }

                    if (Settings.DisableLifeSecUse.Value)
                    {
                        if (_playerFlaskList[j].FlaskAction1 == FlaskActions.Life || _playerFlaskList[j].FlaskAction1 == FlaskActions.Hybrid)
                            if (_playerFlaskList[j].FlaskAction2 == FlaskActions.Offense || _playerFlaskList[j].FlaskAction2 == FlaskActions.Defense)
                                _playerFlaskList[j].FlaskAction2 = FlaskActions.None;
                    }

                    if (Settings.TreatOffenAsDef.Value)
                    {
                        if (_playerFlaskList[j].FlaskAction1 == FlaskActions.Offense)
                            _playerFlaskList[j].FlaskAction1 = FlaskActions.Defense;
                        if (_playerFlaskList[j].FlaskAction2 == FlaskActions.Offense)
                            _playerFlaskList[j].FlaskAction2 = FlaskActions.Defense;
                    }
                    _playerFlaskList[j].UseCharges = (int)Math.Floor(tmpUseCharges);
                    _playerFlaskList[j].IsValid = true;
                }
            }
            catch (Exception e)
            {
                if (Settings.DebugMode.Value)
                {
                    LogError("Warning: Error getting all flask Informations.", ErrmsgTime);
                    LogError(e.Message + e.StackTrace, ErrmsgTime);
                }
                _playerFlaskList.Clear();
                for (var i = 0; i < 5; i++)
                    _playerFlaskList.Add(new PlayerFlask(i));
                return false;
            }
            return true;
        }
        #endregion
        #region Flask Helper Functions
        public void UpdateFlaskChargesInfo(int slot)
        {
            try
            {
                _playerFlaskList[slot].CurrentCharges = GameController.Game.IngameState.
                    IngameUi.InventoryPanel[InventoryIndex.Flask][slot, 0, 0].
                    GetComponent<Charges>().NumCharges;
            }
            catch (Exception)
            {
                _playerFlaskList[slot].CurrentCharges = 0;
            }
        }
        //Parameters:
        // type1 and type2 define the type of flask you wanna drink.
        // reason: is just for debugging output to see where does the drinking flask request came from
        // minRequiredCharges: Min number of charges a flask must have to consider it a valid flask to drink.
        // shouldDrinkAll: if you want to drink all the flasks of type1,type2 (true) or just first in the list(false).
        private bool FindDrinkFlask(FlaskActions type1, FlaskActions type2, string reason, int minRequiredCharge = 0, bool shouldDrinkAll = false)
        {
            var hasDrunk = false;
            var flaskList = _playerFlaskList.FindAll(x => (x.FlaskAction1 == type1 || x.FlaskAction2 == type2 ||
            x.FlaskAction1 == type2 || x.FlaskAction2 == type1) && x.IsEnabled && x.IsValid);

            flaskList.Sort( (x,y) => x.TotalTimeUsed.CompareTo(y.TotalTimeUsed));
            foreach (var flask in flaskList)
            {
                UpdateFlaskChargesInfo(flask.Slot);
                if (flask.CurrentCharges < flask.UseCharges || flask.CurrentCharges < minRequiredCharge) continue;
                _keyboard.SetLatency(GameController.Game.IngameState.CurLatency);
                if (!_keyboard.KeyPressRelease(_keyInfo.K[flask.Slot]))
                    LogError("Warning: High latency ( more than 1000 millisecond ), plugin will fail to work properly.", ErrmsgTime);
                if (Settings.DebugMode.Value)
                    LogMessage("Just Drank Flask on key " + _keyInfo.K[flask.Slot] + " cuz of " + reason, LogmsgTime);
                flask.TotalTimeUsed = (flask.IsInstant) ? 100000 : flask.TotalTimeUsed + 1;
                // if there are multiple flasks, drinking 1 of them at a time is enough.
                hasDrunk = true;
                if (!shouldDrinkAll)
                    return true;
            }
            return hasDrunk;
        }
        private static bool HasDebuff(IReadOnlyDictionary<string, int> dictionary, string buffName, bool isHostile)
        {
            int filterId = 0;
            if (dictionary.TryGetValue(buffName, out filterId))
            {
                return filterId == 0 || isHostile == (filterId == 1);
            }
            return false;
        }
        #endregion

        #region Auto Health Flasks
        private bool InstantLifeFlask(Life playerHealth)
        {
            if (playerHealth.HPPercentage * 100 < Settings.InstantPerHpFlask.Value)
            {
                var flaskList = _playerFlaskList.FindAll(x => x.IsInstant == x.IsEnabled == x.IsValid &&
                          (x.FlaskAction1 == FlaskActions.Life || x.FlaskAction1 == FlaskActions.Hybrid));
                foreach (var flask in flaskList)
                {
                    if (flask.CurrentCharges >= flask.UseCharges)
                    {
                        _keyboard.SetLatency(GameController.Game.IngameState.CurLatency);
                        if (!_keyboard.KeyPressRelease(_keyInfo.K[flask.Slot]))
                            LogError("Warning: High latency ( more than 1000 millisecond ), plugin will fail to work properly.", ErrmsgTime);
                        UpdateFlaskChargesInfo(flask.Slot);
                        if (Settings.DebugMode.Value)
                            LogMessage("Just Drank Instant Flask on key " + _keyInfo.K[flask.Slot] + " cuz of Very Low Life", LogmsgTime);
                        return true;
                    }
                    else
                    {
                        UpdateFlaskChargesInfo(flask.Slot);
                    }
                }
            }
            return false;
        }
        private void LifeLogic()
        {
            if (!GameController.Game.IngameState.Data.LocalPlayer.IsValid || !Settings.AutoFlask.Value)
                return;

            var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var playerHealth = localPlayer.GetComponent<Life>();
            _lastLifeUsed += 100f;
            if (InstantLifeFlask(playerHealth))
                return;
            if (playerHealth.HasBuff("flask_effect_life"))
                return;
            if (playerHealth.HPPercentage * 100 < Settings.PerHpFlask.Value)
            {
                if (FindDrinkFlask(FlaskActions.Life, FlaskActions.Ignore, "Low life"))
                    _lastLifeUsed = 0f;
                else if (FindDrinkFlask(FlaskActions.Hybrid, FlaskActions.Ignore, "Low life"))
                    _lastLifeUsed = 0f;
            }
        }
        #endregion
        #region Auto Mana Flasks
        private bool InstantManaFlask(Life playerHealth)
        {
            if (playerHealth.MPPercentage * 100 < Settings.InstantPerMpFlask.Value)
            {
                var flaskList = _playerFlaskList.FindAll(x => x.IsInstant == x.IsEnabled == x.IsValid &&
                          (x.FlaskAction1 == FlaskActions.Mana || x.FlaskAction1 == FlaskActions.Hybrid));
                foreach (var flask in flaskList)
                {
                    if (flask.CurrentCharges >= flask.UseCharges)
                    {
                        _keyboard.SetLatency(GameController.Game.IngameState.CurLatency);
                        if (!_keyboard.KeyPressRelease(_keyInfo.K[flask.Slot]))
                            LogError("Warning: High latency ( more than 1000 millisecond ), plugin will fail to work properly.", ErrmsgTime);
                        UpdateFlaskChargesInfo(flask.Slot);
                        if (Settings.DebugMode.Value)
                            LogMessage("Just Drank Instant Flask on key " + _keyInfo.K[flask.Slot] + " cuz of Very Low Mana", LogmsgTime);
                        return true;
                    }
                    else
                    {
                        UpdateFlaskChargesInfo(flask.Slot);
                    }
                }
            }
            return false;
        }
        private void ManaLogic()
        {
            if (!Settings.AutoFlask.Value || !GameController.Game.IngameState.Data.LocalPlayer.IsValid)
                return;

            var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var playerHealth = localPlayer.GetComponent<Life>();
            _lastManaUsed += 100f;
            if (InstantManaFlask(playerHealth))
                return;
            if (playerHealth.HasBuff("flask_effect_mana"))
                return;
            if (playerHealth.MPPercentage * 100 < Settings.PerManaFlask.Value || playerHealth.CurMana < Settings.MinManaFlask.Value)
            {
                if (FindDrinkFlask(FlaskActions.Mana, FlaskActions.Ignore, "Low Mana"))
                    _lastManaUsed = 0f;
                else if (FindDrinkFlask(FlaskActions.Hybrid, FlaskActions.Ignore, "Low Mana"))
                    _lastManaUsed = 0f;
            }
        }
        #endregion
        #region Auto Ailment Flasks
        private void AilmentLogic()
        {
            var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var playerHealth = localPlayer.GetComponent<Life>();
            foreach (var buff in playerHealth.Buffs)
            {
                if (float.IsInfinity(buff.Timer))
                    continue;

                var buffName = buff.Name;

                if (Settings.DebugMode.Value)
                    if (_debugDebuff.ContainsKey(buffName))
                        _debugDebuff[buffName] = Math.Max(buff.Timer, _debugDebuff[buffName]);
                    else
                        _debugDebuff[buffName] = buff.Timer;

                if (!Settings.RemAilment.Value)
                    return;

                bool tmpResult;
                if (Settings.RemPoison.Value && HasDebuff(_debuffInfo.Poisoned, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskActions.Ignore, FlaskActions.PoisonImmune, "Poisoned");
                    if (Settings.DebugMode.Value)
                        LogMessage("Poison -> hasDrunkFlask:" + tmpResult + " For Buff:" + buffName, LogmsgTime);
                }
                else if (Settings.RemFrozen.Value && HasDebuff(_debuffInfo.Frozen, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskActions.Ignore, FlaskActions.FreezeImmune, "Frozen");
                    if (Settings.DebugMode.Value)
                        LogMessage("Frozen -> hasDrunkFlask:" + tmpResult + " For Buff:" + buffName, LogmsgTime);
                }
                else if (Settings.RemBurning.Value && HasDebuff(_debuffInfo.Burning, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskActions.Ignore, FlaskActions.IgniteImmune, "Burning");
                    if (Settings.DebugMode.Value)
                        LogMessage("Burning -> hasDrunkFlask:" + tmpResult + " For Buff:" + buffName, LogmsgTime);
                }
                else if (Settings.RemShocked.Value && HasDebuff(_debuffInfo.Shocked, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskActions.Ignore, FlaskActions.ShockImmune, "Shocked");
                    if (Settings.DebugMode.Value)
                        LogMessage("Shock -> hasDrunkFlask:" + tmpResult + " For Buff:" + buffName, LogmsgTime);
                }
                else if (Settings.RemCurse.Value && HasDebuff(_debuffInfo.WeakenedSlowed, buffName, false))
                {
                    tmpResult = FindDrinkFlask(FlaskActions.Ignore, FlaskActions.CurseImmune, "Cursed");
                    if (Settings.DebugMode.Value)
                        LogMessage("Curse -> hasDrunkFlask:" + tmpResult + " For Buff:" + buffName, LogmsgTime);
                }
                else if (Settings.RemBleed.Value)
                {
                    if (HasDebuff(_debuffInfo.Bleeding, buffName, false))
                    {
                        tmpResult = FindDrinkFlask(FlaskActions.Ignore, FlaskActions.BleedImmune, "Bleeding");
                        if (Settings.DebugMode.Value)
                            LogMessage("Bleeding -> hasDrunkFlask:" + tmpResult + " For Buff:" + buffName, LogmsgTime);
                    }
                    else if (HasDebuff(_debuffInfo.Corruption, buffName, false) && buff.Charges >= Settings.CorrptCount)
                    {
                        tmpResult = FindDrinkFlask(FlaskActions.Ignore, FlaskActions.BleedImmune, "Corruption");
                        if (Settings.DebugMode.Value)
                            LogMessage("Corruption -> hasDrunkFlask:" + tmpResult + " For Buff:" + buffName, LogmsgTime);
                    } else if(HasDebuff(_debuffInfo.Corruption, buffName, false))
                    {
                        if (Settings.DebugMode.Value)
                            LogMessage("For Buff:" + buffName + " Corruption detected -> Current Stack=" + buff.Charges +
                                " Required Stack=" + Settings.CorrptCount, LogmsgTime);
                    }
                }
            }
        }
        #endregion
        #region Auto Speed Flasks
        private void SpeedFlaskLogic()
        {
            if (!Settings.SpeedFlaskEnable.Value)
                return;

            var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var playerHealth = localPlayer.GetComponent<Life>();
            var playerMovement = localPlayer.GetComponent<Actor>();
            _moveCounter = playerMovement.isMoving ? _moveCounter += 100f : 0;
            var hasDrunkQuickSilver = false;

            if (localPlayer.IsValid && Settings.QuicksilverEnable.Value && _moveCounter >= Settings.QuicksilverDurration.Value &&
                !playerHealth.HasBuff("flask_bonus_movement_speed") &&
                !playerHealth.HasBuff("flask_utility_sprint"))
            {
                hasDrunkQuickSilver = FindDrinkFlask(FlaskActions.Speedrun, FlaskActions.Speedrun, "Moving Around", Settings.QuicksilverUseWhenCharges.Value);
            }

            // Given preference to QuickSilver cuz it give +40 and Silver give +20
            if (!Settings.ShouldDrinkSilverQuickSilverTogether.Value &&
                (hasDrunkQuickSilver || playerHealth.HasBuff("flask_bonus_movement_speed")
                || playerHealth.HasBuff("flask_utility_sprint")))
                return;

            if (localPlayer.IsValid && Settings.SilverFlaskEnable.Value && _moveCounter >= Settings.SilverFlaskDurration.Value &&
                !playerHealth.HasBuff("flask_utility_haste"))
            {
                FindDrinkFlask(FlaskActions.OFFENSE_AND_SPEEDRUN, FlaskActions.OFFENSE_AND_SPEEDRUN, "Moving Around", Settings.SilverFlaskUseWhenCharges.Value);
            }
        }
        #endregion
        #region Defensive Flasks
        private void DefensiveFlask()
        {
            var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var playerHealth = localPlayer.GetComponent<Life>();
            _lastDefUsed += 100f;
            var secondAction = FlaskActions.Ignore;
            if (Settings.TreatOffenAsDef.Value)
                secondAction = FlaskActions.OFFENSE_AND_SPEEDRUN;
            if (_lastDefUsed < Settings.DefensiveDelay.Value)
                return;
            if (Settings.DefFlaskEnable.Value && localPlayer.IsValid)
            {
                if (playerHealth.HPPercentage * 100 < Settings.HpDefensive.Value ||
                    (playerHealth.MaxES > 0 && playerHealth.ESPercentage * 100 < Settings.EsDefensive.Value))
                {
                    if (FindDrinkFlask(FlaskActions.Defense, secondAction, "Defensive Action", 0, Settings.DefensiveDrinkAll.Value))
                        _lastDefUsed = 0f;
                }
            }
        }
        #endregion
        #region Offensive Flasks
        private void OffensiveFlask()
        {
            var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            var playerHealth = localPlayer.GetComponent<Life>();
            var isAttacking = (localPlayer.GetComponent<Actor>().ActionId & 2) > 0;
            _lastOffUsed += 100f;
            if (!Settings.OffFlaskEnable.Value || !localPlayer.IsValid)
                return;
            if (!Settings.OffensiveWhenAttacking.Value && !Settings.OffensiveWhenLifeEs.Value && !Settings.UseWhileKeyPressed)
            {
                LogError("Atleast Select 1 offensive flask Method Life/ES OR When Attacking OR When Use While Key Pressed. OR Disable offensive flask.", ErrmsgTime);
                return;
            }
            if (_lastOffUsed < Settings.OffensiveDelay.Value)
                return;

            //if (Settings.offensiveWhenAttacking.Value && Settings.debugMode.Value)
            //    LogMessage("isAttacking: " + IsAttacking + "ActionId: " + LocalPlayer.GetComponent<Actor>().ActionId, logmsg_time);

            if (Settings.OffensiveWhenAttacking.Value && !isAttacking)
                return;

            if (Settings.UseWhileKeyPressed.Value && !KeyboardHelper.IsKeyDown((int)Settings.KeyPressed.Value))
                return;

            if (Settings.OffensiveWhenLifeEs.Value && (playerHealth.HPPercentage * 100 > Settings.HpOffensive.Value &&
                    (playerHealth.MaxES <= 0 || playerHealth.ESPercentage * 100 > Settings.EsOffensive.Value)))
                return;

            if (FindDrinkFlask(FlaskActions.Offense, FlaskActions.Offense, "Offensive Action", Settings.OffensiveUseWhenCharges.Value, Settings.OffensiveDrinkAll.Value))
                _lastOffUsed = 0f;

            if (!playerHealth.HasBuff("flask_utility_haste"))
                if (FindDrinkFlask(FlaskActions.OFFENSE_AND_SPEEDRUN, FlaskActions.OFFENSE_AND_SPEEDRUN, "Offensive Action", Settings.OffensiveUseWhenCharges.Value, Settings.OffensiveDrinkAll.Value))
                    _lastOffUsed = 0f;

        }
        #endregion

        private void FlaskMain()
        {
            if (!GameController.Game.IngameState.Data.LocalPlayer.IsValid)
                return;

            _playerFlaskList[0].IsEnabled = Settings.FlaskSlot1Enable.Value;
            _playerFlaskList[1].IsEnabled = Settings.FlaskSlot2Enable.Value;
            _playerFlaskList[2].IsEnabled = Settings.FlaskSlot3Enable.Value;
            _playerFlaskList[3].IsEnabled = Settings.FlaskSlot4Enable.Value;
            _playerFlaskList[4].IsEnabled = Settings.FlaskSlot5Enable.Value;
            if (!GettingAllFlaskInfo())
            {
                if (Settings.DebugMode.Value)
                    LogMessage("Error getting Flask Details, trying again.", LogmsgTime);
                return;
            }

            var playerLife = GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            if (playerLife == null || _isTown || _isHideout || playerLife.CurHP <= 0 || playerLife.HasBuff("grace_period"))
                return;

            SpeedFlaskLogic();
            ManaLogic();
            LifeLogic();
            AilmentLogic();
            DefensiveFlask();
            OffensiveFlask();
        }
    }
}
