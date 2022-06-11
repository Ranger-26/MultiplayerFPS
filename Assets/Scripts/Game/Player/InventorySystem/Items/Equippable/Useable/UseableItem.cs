namespace Game.Player.InventorySystem.Items.Equippable.Useable
{
    public class UseableItem : EquippableItem
    {
        /// <summary>
        /// The number in seconds before this <see cref="UseableItem"/> can be used again after usage, or cancelled.
        /// </summary>
        public virtual float Cooldown { get; set; } = 0.125f;

        /// <summary>
        /// The number in seconds before the usage of this <see cref="UseableItem"/> cannot be cancelled.
        /// </summary>
        /// <remarks>If this is set to -1, the item will not be able to be canceled.</remarks>
        public virtual float TimeBeforeUncancellable { get; set; } = -1;

        public virtual void Used(ref bool allowed) {}
        public virtual void Cancelled() {}
    }
}