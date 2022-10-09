using Game.GameLogic.ItemSystem.Inventory;
using Inputs;
using Mirror;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Core.UsableItems
{
    public abstract class UsableItemBase : ItemBase
    {
        protected void RemoveItemFromOwner()
        {
            PlayerInventory plr = Owner.GetComponent<PlayerInventory>();
            if (plr.allItems.ContainsKey(plr.heldItemIndex) &&
                plr.allItems[plr.heldItemIndex] == ItemData.ItemIdentifier)
            {
                plr.ServerDestroyHeldItem();
            }
        }

        public override bool OnEquip()
        {
            if (IsItemOwner)
            {
                Invoke(nameof(RegisterInputEvents), ItemData.ItemDrawTime);
                //call item draw animation
            }

            return true;
        }

        public override bool OnDeEquip()
        {
            if (IsItemOwner)
            {
                UnRegisterInputEvents();
            }

            return true;
        }

        public virtual void OnUse(InputAction.CallbackContext ctx)
        {
            SendUseMessage();
        }

        protected void SendUseMessage()
        {
            NetworkClient.Send(new UseUsableItemMessage()
            {
                item = ItemData.ItemIdentifier
            });
        }

        public virtual void OnServerReceiveUseMessage()
        {
            
        }
        
        protected virtual void RegisterInputEvents()
        {
            GameInputManager.Actions.Player.Fire.performed += OnUse;
        }

        protected virtual void UnRegisterInputEvents()
        {
            GameInputManager.Actions.Player.Fire.performed -= OnUse;
        }

        public void OnDestroy()
        {
            if (IsItemOwner)
            {
                UnRegisterInputEvents();
            }
        }
    }
}