using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using System.Windows.Forms;

namespace FlaskManager
{
    internal class FlaskManagerSettings : SettingsBase
    {
        public FlaskManagerSettings()
        {
            #region Default Settings
            //plugin
            Enable = false;
            //HP/MANA
            AutoFlask = false;
            PerHpFlask = new RangeNode<int>(60, 0, 100);
            InstantPerHpFlask = new RangeNode<int>(35, 0, 100);
            PerManaFlask = new RangeNode<float>(25f, 0, 100);
			InstantPerMpFlask = new RangeNode<int>(35, 0, 100);
            MinManaFlask = new RangeNode<float>(50, 0, 100);
            DisableLifeSecUse = false;
            //Ailment Flask
            RemAilment = false;
            RemFrozen = false;
            RemShocked = false;
            RemBurning = false;
            RemCurse = false;
            RemPoison = false;
            RemBleed = false;
            CorrptCount = new RangeNode<int>(10, 1, 20);
            AilmentDur = new RangeNode<int>(0, 0, 5000);
            //Quicksilver
			SpeedFlaskEnable = false;
            ShouldDrinkSilverQuickSilverTogether = true;
            QuicksilverEnable = false;
            QuicksilverDurration = new RangeNode<float>(500f, 0f, 5000f);
            QuicksilverUseWhenCharges = new RangeNode<int>(0, 0, 100);
			//SilverFlask
            SilverFlaskEnable = false;
            SilverFlaskDurration = new RangeNode<float>(500f, 0f, 6000f);
            SilverFlaskUseWhenCharges = new RangeNode<int>(0, 0, 100);
            //Defensive Flask
            DefFlaskEnable = false;
            HpDefensive = new RangeNode<int>(50, 0, 100);
            EsDefensive = new RangeNode<int>(50, 0, 100);
            DefensiveDelay = new RangeNode<float>(3000f, 0f, 10000f);
            DefensiveDrinkAll = true;
            //Offensive Flask
            OffFlaskEnable = false;
            HpOffensive = new RangeNode<int>(50, 0, 100);
            EsOffensive = new RangeNode<int>(50, 0, 100);
            OffensiveDelay = new RangeNode<float>(3000f, 0f, 10000f);
            UseWhileKeyPressed = false;
            KeyPressed = Keys.T;
            OffensiveDrinkAll = true;
            OffensiveWhenAttacking = false;
            OffensiveWhenLifeEs = true;
            TreatOffenAsDef = false;
            OffensiveUseWhenCharges = new RangeNode<int>(0, 0, 100);
            //Unique Flask
            UniqFlaskEnable = false;
            // Settings
            // Flask UI Settings
            FlaskUiEnable = false;
            FlaskPositionX = new RangeNode<float>(28.0f, 0.0f, 100.0f);
            FlaskPositionY = new RangeNode<float>(83.0f, 0.0f, 100.0f);
            FlaskTextSize = new RangeNode<int>(15, 15, 60);
            //Buff UI Settings
            BuffUiEnable = false;
            BuffPositionX = new RangeNode<float>(0.0f, 0.0f, 100.0f);
            BuffPositionY = new RangeNode<float>(10.0f, 0.0f, 100.0f);
            BuffTextSize = new RangeNode<int>(15, 15, 60);
            EnableFlaskAuraBuff = true;
            //Debug
            DebugMode = false;
            //Flask Slot Enable/Disable
            FlaskSlot1Enable = true;
            FlaskSlot2Enable = true;
            FlaskSlot3Enable = true;
            FlaskSlot4Enable = true;
            FlaskSlot5Enable = true;

            //Charges Reduction from Items/Tree (0 to disable it)
            ChargeReduction = new RangeNode<float>(0f, 0f, 100f);
            //About
            About = true;
            #endregion
        }

        #region HP Mana Flask Menu
        [Menu("HP/MANA % Auto Flask", 10)]
        public ToggleNode AutoFlask { get; set; }
        [Menu("Min Life % Auto HP Flask", 11, 10)]
        public RangeNode<int> PerHpFlask { get; set; }
        [Menu("Min Life % Auto Instant HP Flask (put instant flask in last slot)", 12, 10)]
        public RangeNode<int> InstantPerHpFlask { get; set; }

        [Menu("Min Mana % Auto Mana Flask", 14, 10)]
        public RangeNode<float> PerManaFlask { get; set; }
        [Menu("Min Mana Auto Mana Flask", 15, 10)]
        public RangeNode<float> MinManaFlask { get; set; }
		[Menu("Min Mana % Auto Instant HP Flask (put instant flask in last slot)", 16, 10)]
        public RangeNode<int> InstantPerMpFlask { get; set; }
        [Menu("Disable Life/Hybrid Flask Offensive/Defensive Usage", 18, 10)]
        public ToggleNode DisableLifeSecUse { get; set; }
        #endregion

        #region Ailment Flask Menu
        [Menu("Remove Ailment Flask", 20)]
        public ToggleNode RemAilment { get; set; }
        [Menu("Remove Frozen", 21, 20)]
        public ToggleNode RemFrozen { get; set; }
        [Menu("Remove Burning", 22, 20)]
        public ToggleNode RemBurning { get; set; }
        [Menu("Remove Shocked", 23, 20)]
        public ToggleNode RemShocked { get; set; }
        [Menu("Remove Curse", 24, 20)]
        public ToggleNode RemCurse { get; set; }
        [Menu("Remove Poison", 25, 20)]
        public ToggleNode RemPoison { get; set; }
        [Menu("Remove Corrupting/Bleed", 26, 20)]
        public ToggleNode RemBleed { get; set; }
        [Menu("Corrupting Blood Stacks", 27, 20)]
        public RangeNode<int> CorrptCount { get; set; }
        [Menu("Remove Ailment Post Duration (millisecond)", 28, 20)]
        public RangeNode<int> AilmentDur { get; set; }
        #endregion

        #region Speed Flask Menu
		[Menu("Speed Flask", 30)]
        public ToggleNode SpeedFlaskEnable { get; set; }

        [Menu("QuickSilver Flask", 31, 30)]
        public ToggleNode QuicksilverEnable { get; set; }
        [Menu("Use QuickSilver After Moving Post (millisecond)", 32, 30)]
        public RangeNode<float> QuicksilverDurration { get; set; }
        [Menu("Use QuickSilver When Charges Greater than X (0 to disable it)", 33, 30)]
        public RangeNode<int> QuicksilverUseWhenCharges { get; set; }

        [Menu("Silver Flask", 34, 30)]
        public ToggleNode SilverFlaskEnable { get; set; }
        [Menu("Use Silver Flask After Moving Post (millisecond)", 35, 30)]
        public RangeNode<float> SilverFlaskDurration { get; set; }
        [Menu("Use Silver Flask When Charges Greater than X (0 to disable it)", 36, 30)]
        public RangeNode<int> SilverFlaskUseWhenCharges { get; set; }
        [Menu("Drink Silver/QuickSilver Together", 37, 30)]
        public ToggleNode ShouldDrinkSilverQuickSilverTogether { get; set; }
        #endregion
			
        #region Defensive Flask Menu
        [Menu("Defensive Flask", 40)]
        public ToggleNode DefFlaskEnable { get; set; }
        [Menu("Min Life %", 41, 40)]
        public RangeNode<int> HpDefensive { get; set; }
        [Menu("Min ES %", 42, 40)]
        public RangeNode<int> EsDefensive { get; set; }
        [Menu("Delay (millisecond)", 43, 40)]
        public RangeNode<float> DefensiveDelay { get; set; }
        [Menu("Treat Offensive Flasks As Defensive", 44, 40)]
        public ToggleNode TreatOffenAsDef { get; set; }
        [Menu("Drink All Flasks Together", 45, 40)]
        public ToggleNode DefensiveDrinkAll { get; set; }
        #endregion

        #region Offensive Flask Menu
        [Menu("Offensive Flask", 50)]
        public ToggleNode OffFlaskEnable { get; set; }
        [Menu("Drink On Life/ES", 51, 50)]
        public ToggleNode OffensiveWhenLifeEs { get; set; }
        [Menu("Min Life %", 52, 51)]
        public RangeNode<int> HpOffensive { get; set; }
        [Menu("Min ES %", 53, 51)]
        public RangeNode<int> EsOffensive { get; set; }
        [Menu("Drink On Skill Use", 54, 50)]
        public ToggleNode OffensiveWhenAttacking { get; set; }
        [Menu("Drink On Key Use", 55, 50)]
        public ToggleNode UseWhileKeyPressed { get; set; }
        [Menu("Skill Key Hotkey", 56, 50)]
        public HotkeyNode KeyPressed { get; set; }
        [Menu("Delay (millisecond)", 57, 50)]
        public RangeNode<float> OffensiveDelay { get; set; }
        [Menu("Use When Charges Greater than X (0 to disable it)", 58, 50)]
        public RangeNode<int> OffensiveUseWhenCharges { get; set; }
        [Menu("Drink All Flasks Together", 59, 50)]
        public ToggleNode OffensiveDrinkAll { get; set; }
        #endregion

        #region Unnique Flask Menu
        [Menu("Unique Flask", 60)]
        public ToggleNode UniqFlaskEnable { get; set; }
        #endregion

        #region Settings Menu
        [Menu("UI Settings", 100)]
        public EmptyNode UiSesettingsHolder { get; set; }
        [Menu("Flask Slot UI", 101, 100)]

        public ToggleNode FlaskUiEnable { get; set; }
        [Menu("Position: X", 102, 101)]
        public RangeNode<float> FlaskPositionX { get; set; }
        [Menu("Position: Y", 103, 101)]
        public RangeNode<float> FlaskPositionY { get; set; }
        [Menu("Text Size", 104, 101)]
        public RangeNode<int> FlaskTextSize { get; set; }

        [Menu("Buff Bar UI", 105, 100)]
        public ToggleNode BuffUiEnable { get; set; }
        [Menu("Position: X", 106, 105)]
        public RangeNode<float> BuffPositionX { get; set; }
        [Menu("Position: Y", 107, 105)]
        public RangeNode<float> BuffPositionY { get; set; }
        [Menu("Text Size", 108, 105)]
        public RangeNode<int> BuffTextSize { get; set; }
        [Menu("Enable Flask Or Aura Debuff/Buff", 109, 105)]
        public ToggleNode EnableFlaskAuraBuff { get; set; }

        [Menu("Debug Mode", 110, 100)]
        public ToggleNode DebugMode { get; set; }

        [Menu("Enable/Disable Flasks", 120)]
        public EmptyNode FlasksettingsHolder { get; set; }
        [Menu("Use Flask Slot 1", 121, 120)]
        public ToggleNode FlaskSlot1Enable { get; set; }
        [Menu("Use Flask Slot 2", 122, 120)]
        public ToggleNode FlaskSlot2Enable { get; set; }
        [Menu("Use Flask Slot 3", 123, 120)]
        public ToggleNode FlaskSlot3Enable { get; set; }
        [Menu("Use Flask Slot 4", 124, 120)]
        public ToggleNode FlaskSlot4Enable { get; set; }
        [Menu("Use Flask Slot 5", 125, 120)]
        public ToggleNode FlaskSlot5Enable { get; set; }

        [Menu("About", 126)]
        public ToggleNode About { get; set; }

        [Menu("Total Charges Reduction %", 127)]
        public RangeNode<float> ChargeReduction { get; set; }
        #endregion

    }
}
