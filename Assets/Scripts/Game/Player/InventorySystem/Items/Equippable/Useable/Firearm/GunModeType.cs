using System;

namespace Game.Player.InventorySystem.Items.Equippable.Useable.Firearm
{
    [Flags]
    public enum GunModeType : byte
    {
        Semi = 1,
        SemiAuto = 2,
        Burst = 4,
        Auto = 8
    }
}