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
            instHPFlask = new RangeNode<int>(35, 0, 100);
            instHPDelay = new RangeNode<float>(.3f, 0f, 1f);
            PerManaFlask = new RangeNode<float>(25f, 0, 100);
            //Ailment Flask
            remAilment = false;
            remFrozen = false;
            remShocked = false;
            remBurning = false;
            remCurse = false;
            remCorrupt = false;
            corrptCount = new RangeNode<int>(10, 1, 20);
            ailmentDur = new RangeNode<int>(0, 0, 5);
            //QuickSilver
            qSEnable = false;
            qS20 = false;
            qS40 = false;
            qSDur = new RangeNode<float>(1.5f, 0f, 10f);
            //Defensive Flask
            defFlaskEnable = false;
            hPDefensive = new RangeNode<int>(0, 0, 100);
            hPElement = new RangeNode<int>(0, 0, 100);
            eSDefensive = new RangeNode<int>(0, 0, 100);
            eSElement = new RangeNode<int>(0, 0, 100);
            //Offensive Flask
            offFlaskEnable = false;
            offFlaskDur = new RangeNode<float>(4f, 0f, 10f);
            //Unique Flask
            uniqFlaskEnable = false;
            // Settings
            uiEnable = false;
            positionX = new RangeNode<float>(28.0f, 0.0f, 100.0f);
            positionY = new RangeNode<float>(83.0f, 0.0f, 100.0f);
            textSize = new RangeNode<int>(10, 1, 30);
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
        [Menu("Min % ES Auto Quit", 4, 1)]
        public RangeNode<float> percentESQuit { get; set; }
        #endregion

        #region HP Mana Flask Menu
        [Menu("HP/MANA % Auto Flask", 6)]
        public ToggleNode autoFlask { get; set; }
        [Menu("Min Life % Auto HP Flask", 7, 6)]
        public RangeNode<int> perHPFlask { get; set; }
        [Menu("Min Life % Auto Instant HP Flask", 8, 6)]
        public RangeNode<int> instHPFlask { get; set; }
        [Menu("Instant HP Flask Delay", 9, 6)]
        public RangeNode<float> instHPDelay { get; set; }
        [Menu("Min Mana % Auto Mana Flask", 10, 6)]
        public RangeNode<float> PerManaFlask { get; set; } 
        #endregion

        #region Ailment Flask Menu
        [Menu("Remove Ailment Flask", 11)]
        public ToggleNode remAilment { get; set; }
        [Menu("Remove Frozen Ailment", 12, 11)]
        public ToggleNode remFrozen { get; set; }
        [Menu("Remove Burning Ailment", 13, 11)]
        public ToggleNode remShocked { get; set; }
        [Menu("Remove Shocked Ailment", 14, 11)]
        public ToggleNode remBurning { get; set; }
        [Menu("Remove Curse Ailment", 15, 11)]
        public ToggleNode remCurse { get; set; }
        [Menu("Remove Corrupting Ailment", 16, 11)]
        public ToggleNode remCorrupt { get; set; }
        [Menu("Corrupting Blood Stacks", 17, 11)]
        public RangeNode<int> corrptCount { get; set; }
        [Menu("Remove Ailment Post Duration (s)", 18, 11)]
        public RangeNode<int> ailmentDur { get; set; } 
        #endregion

        #region Quick Sivler Flask Menu
        [Menu("QuickSilver Flask", 19)]
        public ToggleNode qSEnable { get; set; }
        [Menu("Use QickSilver @20+ Charges", 20, 19)]
        public ToggleNode qS20 { get; set; }
        [Menu("Use QickSilver @40+ Charges", 21, 19)]
        public ToggleNode qS40 { get; set; }
        [Menu("Use After Moving Post (s)", 22, 19)]
        public RangeNode<float> qSDur { get; set; } 
        #endregion

        #region Defensive Flask Menu
        [Menu("Defensive Flask", 23)]
        public ToggleNode defFlaskEnable { get; set; }
        [Menu("Min Life % Auto Defensive Flask", 24, 23)]
        public RangeNode<int> hPDefensive { get; set; }
        [Menu("Min Life % Auto Elemental Flask", 25, 23)]
        public RangeNode<int> hPElement { get; set; }
        [Menu("Min ES % Auto Defensive Flask", 26, 23)]
        public RangeNode<int> eSDefensive { get; set; }
        [Menu("Min ES % Auto Elemental Flask", 27, 23)]
        public RangeNode<int> eSElement { get; set; }
        #endregion

        #region Offensive Flask Menu
        [Menu("Offensive Flask", 28)]
        public ToggleNode offFlaskEnable { get; set; }
        [Menu("Use Offensive Flask Post (s)", 29, 28)]
        public RangeNode<float> offFlaskDur { get; set; } 
        #endregion

        #region Unnique Flask Menu
        [Menu("Unique Flask", 30)]
        public ToggleNode uniqFlaskEnable { get; set; } 
        #endregion

        #region Settings Menu
        [Menu("Flask Manager Settings", 100)]
        public EmptyNode settingsHolder { get; set; }
        [Menu("UI Enable", 101, 100)]
        public ToggleNode uiEnable { get; set; }
        [Menu("Position: X", 102, 100)]
        public RangeNode<float> positionX { get; set; }
        [Menu("Position: Y", 103, 100)]
        public RangeNode<float> positionY { get; set; }
        [Menu("Text Size", 104, 100)]
        public RangeNode<int> textSize { get; set; }
        [Menu("Use Flask Slot 1", 105, 100)]
        public ToggleNode flaskSlot1Enable { get; set; }
        [Menu("Use Flask Slot 2", 106, 100)]
        public ToggleNode flaskSlot2Enable { get; set; }
        [Menu("Use Flask Slot 3", 107, 100)]
        public ToggleNode flaskSlot3Enable { get; set; }
        [Menu("Use Flask Slot 4", 108, 100)]
        public ToggleNode flaskSlot4Enable { get; set; }
        [Menu("Use Flask Slot 5", 109, 100)]
        public ToggleNode flaskSlot5Enable { get; set; } 
        #endregion
    }
}