using Game.GameLogic.ItemSystem.Inventory;
using Inputs;
using Mirror;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Core.UsableItems
{
    public abstract class UsableItemBase : ItemBase
    {
        public virtual void OnUse(InputAction.CallbackContext ctx)
        {
            //client stuff
            SendUseMessage();
        }

        private void SendUseMessage()
        {
            NetworkClient.Send(new UseUsableItemMessage()
            {
                item = ItemData.ItemIdentifier
            });
        }

        public virtual void OnServerReceiveUseMessage()
        {
            
        }
        
        protected override void RegisterInputEvents()
        {
            GameInputManager.Actions.Player.Fire.performed += OnUse;
        }

        protected override void UnRegisterInputEvents()
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