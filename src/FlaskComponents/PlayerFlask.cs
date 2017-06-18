using PoeHUD.Models.Enums;

namespace FlaskManager.FlaskComponents
{
    internal class PlayerFlask
    {
        public string FlaskName = "";
        public readonly int Slot;
        public bool IsEnabled = false;
        public bool IsValid = false;
        public bool IsInstant = false;
        public int UseCharges = 1000;
        public int CurrentCharges = 0;
        public long TotalTimeUsed = 0;

        public ItemRarity FlaskRarity = ItemRarity.Normal;
        public FlaskActions FlaskAction1 = FlaskActions.Ignore;
        public FlaskActions FlaskAction2 = FlaskActions.Ignore;
        public PlayerFlask(int slot)
        {
            Slot = slot;
        }
    }
}
