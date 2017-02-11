using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;

namespace FlaskManager
{
    public class FlaskManagerSettings : SettingsBase
    {
        public FlaskManagerSettings()
        {
            #region Flask Manager Settings Var
            //plugin
            Enable = false;
            //Auto Quit
            isPercentQuit = false;
            percentHPQuit = new RangeNode<float>(35f, 0f, 100f);
            percentESQuit = new RangeNode<float>(35f, 0, 100);
            //HP/MANA
            autoFlask = false;
            perHPFlask = new RangeNode<int>(60, 0, 100);
            HPDelay = new RangeNode<float>(1f, 0f, 4f);
            ManaDelay = new RangeNode<float>(1f, 0f, 4f);
            PerManaFlask = new RangeNode<float>(25f, 0, 100);
            //Ailment Flask
            remAilment = false;
            remFrozen = false;
            remShocked = false;
            remBurning = false;
            remCurse = false;
            remPoison = false;
            remCorrupt = false;
            corrptCount = new RangeNode<int>(10, 1, 20);
            ailmentDur = new RangeNode<int>(0, 0, 5);
            //QuickSilver
            qSEnable = false;
            qSDur = new RangeNode<float>(1.5f, 0f, 10f);
            //Defensive Flask
            defFlaskEnable = false;
            hPDefensive = new RangeNode<int>(50, 0, 100);
            eSDefensive = new RangeNode<int>(50, 0, 100);
            DefensiveDelay = new RangeNode<float>(3f, 2f, 10f);
            //Offensive Flask
            offFlaskEnable = false;
            hpOffensive = new RangeNode<int>(50, 0, 100);
            esOffensive = new RangeNode<int>(50, 0, 100);
            OffensiveDelay = new RangeNode<float>(3f, 2f, 10f);
            //Unique Flask
            uniqFlaskEnable = false;
            // Settings
            // Flask UI Settings
            flaskUiEnable = false;
            flask_PositionX = new RangeNode<float>(28.0f, 0.0f, 100.0f);
            flask_PositionY = new RangeNode<float>(83.0f, 0.0f, 100.0f);
            flask_TextSize = new RangeNode<int>(15, 15, 60);
            //Buff UI Settings
            buffUiEnable = false;
            buff_PositionX = new RangeNode<float>(0.0f, 0.0f, 100.0f);
            buff_PositionY = new RangeNode<float>(10.0f, 0.0f, 100.0f);
            buff_TextSize = new RangeNode<int>(15, 15, 60);
            enableFlaskAuraBuff = true;
            //Debug
            debugMode = false;
            //Flask Slot Enable/Disable
            flaskSlot1Enable = true;
            flaskSlot2Enable = true;
            flaskSlot3Enable = true;
            flaskSlot4Enable = true;
            flaskSlot5Enable = true;
            #endregion
        }

        #region Auto Quit Menu
        [Menu("Auto % HP/ES to Quit", 1)]
        public ToggleNode isPercentQuit { get; set; }
        [Menu("Min % Life to Auto Quit", 2, 1)]
        public RangeNode<float> percentHPQuit { get; set; }
        [Menu("Min % ES Auto Quit", 3, 1)]
        public RangeNode<float> percentESQuit { get; set; }
        #endregion

        #region HP Mana Flask Menu
        [Menu("HP/MANA % Auto Flask", 10)]
        public ToggleNode autoFlask { get; set; }
        [Menu("Min Life % Auto HP Flask", 11, 10)]
        public RangeNode<int> perHPFlask { get; set; }
        [Menu("HP Flask Delay", 12, 10)]
        public RangeNode<float> HPDelay { get; set; }
        [Menu("Min Mana % Auto Mana Flask", 13, 10)]
        public RangeNode<float> PerManaFlask { get; set; }
        [Menu("Mana Flask Delay", 14, 10)]
        public RangeNode<float> ManaDelay { get; set; }
        #endregion

        #region Ailment Flask Menu
        [Menu("Remove Ailment Flask", 20)]
        public ToggleNode remAilment { get; set; }
        [Menu("Remove Frozen Ailment", 21, 20)]
        public ToggleNode remFrozen { get; set; }
        [Menu("Remove Burning Ailment", 22, 20)]
        public ToggleNode remBurning { get; set; }
        [Menu("Remove Shocked Ailment", 23, 20)]
        public ToggleNode remShocked { get; set; }
        [Menu("Remove Curse Ailment", 24, 20)]
        public ToggleNode remCurse { get; set; }
        [Menu("Remove Poison Ailment", 25, 20)]
        public ToggleNode remPoison { get; set; }
        [Menu("Remove Corrupting/Bleed Ailment", 26, 20)]
        public ToggleNode remCorrupt { get; set; }
        [Menu("Corrupting Blood Stacks", 27, 20)]
        public RangeNode<int> corrptCount { get; set; }
        [Menu("Remove Ailment Post Duration (s)", 28, 20)]
        public RangeNode<int> ailmentDur { get; set; } 
        #endregion

        #region Quick Sivler Flask Menu
        [Menu("QuickSilver Flask", 30)]
        public ToggleNode qSEnable { get; set; }
        [Menu("Use After Moving Post (s)", 31, 30)]
        public RangeNode<float> qSDur { get; set; } 
        #endregion

        #region Defensive Flask Menu
        [Menu("Defensive Flask", 40)]
        public ToggleNode defFlaskEnable { get; set; }
        [Menu("Min Life % Auto Defensive Flask", 41, 40)]
        public RangeNode<int> hPDefensive { get; set; }
        [Menu("Min ES % Auto Defensive Flask", 42, 40)]
        public RangeNode<int> eSDefensive { get; set; }
        [Menu("Defensive Flask Delay", 43, 40)]
        public RangeNode<float> DefensiveDelay { get; set; }
        #endregion

        #region Offensive Flask Menu
        [Menu("Offensive Flask", 50)]
        public ToggleNode offFlaskEnable { get; set; }
        [Menu("Min Life % Auto Offensive Flask", 51, 50)]
        public RangeNode<int> hpOffensive { get; set; }
        [Menu("Min ES % Auto Offensive Flask", 52, 50)]
        public RangeNode<int> esOffensive { get; set; }
        [Menu("Offensive Flask Delay", 53, 50)]
        public RangeNode<float> OffensiveDelay { get; set; }
        #endregion

        #region Unnique Flask Menu
        [Menu("Unique Flask", 60)]
        public ToggleNode uniqFlaskEnable { get; set; } 
        #endregion

        #region Settings Menu
        [Menu("UI Settings", 100)]
        public EmptyNode uiSesettingsHolder { get; set; }
        [Menu("Flask Slot UI", 101, 100)]

        public ToggleNode flaskUiEnable { get; set; }
        [Menu("Position: X", 102, 101)]
        public RangeNode<float> flask_PositionX { get; set; }
        [Menu("Position: Y", 103, 101)]
        public RangeNode<float> flask_PositionY { get; set; }
        [Menu("Text Size", 104, 101)]
        public RangeNode<int> flask_TextSize { get; set; }

        [Menu("Buff Bar UI",105,100)]
        public ToggleNode buffUiEnable { get; set; }
        [Menu("Position: X", 106, 105)]
        public RangeNode<float> buff_PositionX { get; set; }
        [Menu("Position: Y", 107, 105)]
        public RangeNode<float> buff_PositionY { get; set; }
        [Menu("Text Size", 108, 105)]
        public RangeNode<int> buff_TextSize { get; set; }
        [Menu("Enable Flask Or Aura Debuff/Buff",109,105)]
        public ToggleNode enableFlaskAuraBuff { get; set; }

        [Menu("Debug Mode", 110,100)]
        public ToggleNode debugMode { get; set; }

        [Menu("Enable/Disable Flasks", 120)]
        public EmptyNode flasksettingsHolder { get; set; }
        [Menu("Use Flask Slot 1", 121, 120)]
        public ToggleNode flaskSlot1Enable { get; set; }
        [Menu("Use Flask Slot 2", 122, 120)]
        public ToggleNode flaskSlot2Enable { get; set; }
        [Menu("Use Flask Slot 3", 123, 120)]
        public ToggleNode flaskSlot3Enable { get; set; }
        [Menu("Use Flask Slot 4", 124, 120)]
        public ToggleNode flaskSlot4Enable { get; set; }
        [Menu("Use Flask Slot 5", 125, 120)]
        public ToggleNode flaskSlot5Enable { get; set; }
        #endregion
    }
}