namespace Game.Player.InventorySystem.Items
{
    public class ItemHandlers
    {
        public enum ItemType : byte
        {
            GunMp5K,
            GunMakarov,
            Knife,
            ArmorLight,
            ArmorHeavy,
        }

        public enum ItemCategoryType : byte
        {
            Primary,
            Secondary,
            Equipment,
            Armor,
        }
    }
}