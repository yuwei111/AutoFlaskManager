using PoeHUD.Models.Enums;

namespace FlaskManager.FlaskComponents
{
    class PlayerFlask
    {
        public string FlaskName = "";
        public readonly int Slot;
        public bool isEnabled = false;
        public bool isValid = false;
        public bool isInstant = false;
        public int UseCharges = 1000;
        public int CurrentCharges = 0;
        public long TotalTimeUsed = 0;

        public ItemRarity flaskRarity = ItemRarity.Normal;
        public FlaskAction FlaskAction1 = FlaskAction.IGNORE;
        public FlaskAction FlaskAction2 = FlaskAction.IGNORE;
        public PlayerFlask(int slot)
        {
            Slot = slot;
        }
    }
}
