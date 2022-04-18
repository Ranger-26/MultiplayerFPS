namespace Game.Player.InventorySystem.Items.Equippable.Useable.Firearm
{
    public class FirearmBase : UseableItem
    {
        /// <summary>
        /// The number of ammo that can be loaded in the <see cref="FirearmBase"/>.
        /// </summary>
        public virtual float AmmoCapacity { get; set; }

        /// <summary>
        /// The number of ammo in magazines that are in the <see cref="FirearmBase"/> when spawned.
        /// </summary>
        public virtual float InitialMagazines { get; set; }

        /// <summary>
        /// The <see cref="GunModeTypes"/> allowed.
        /// </summary>
        /// <remarks>You can use this as a flag.</remarks>
        public virtual GunModeType GunModeTypes { get; set; }

        /// <summary>
        /// The ammunition currently loaded in the <see cref="FirearmBase"/>.
        /// </summary>
        public float Ammo { get; set; }

        /// <summary>
        /// The number of magazines left in the <see cref="FirearmBase"/>.
        /// </summary>
        public float Magazines { get; set; }

        /// <summary>
        /// The current <see cref="GunModeType"/> selected.
        /// </summary>
        public GunModeType CurrentMode { get; set; }

        public virtual void Reloaded() {}
    }
}