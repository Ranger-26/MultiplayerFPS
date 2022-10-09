using Game.GameLogic.ItemSystem.Inventory;
using Inputs;
using Mirror;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Core.UsableItems
{
    public abstract class UsableItemBase : ItemBase
    {
        public virtual void OnClientUse(InputAction.CallbackContext ctx)
        {
            //client stuff
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
        
        protected override void RegisterInputEvents()
        {
            GameInputManager.Actions.Player.Fire.performed += OnClientUse;
        }

        protected override void UnRegisterInputEvents()
        {
            GameInputManager.Actions.Player.Fire.performed -= OnClientUse;
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