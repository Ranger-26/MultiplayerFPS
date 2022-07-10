using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Core.RuntimeData;

namespace Game.GameLogic.ItemSystem.Items.Firearms
{
    public struct FirearmRuntimeData : IRuntimeData
    {
        public ItemIdentifier ItemIdentifier { get; }

        public int currentAmmo;

        public int ReserveAmmo;

        public FirearmRuntimeData(ItemIdentifier id, int curAmmo, int resAmmo)
        {
            ItemIdentifier = id;
            currentAmmo = curAmmo;
            ReserveAmmo = resAmmo;
        }
    }
}