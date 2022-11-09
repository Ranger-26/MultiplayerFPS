using Game.GameLogic.ItemSystem.Inventory;
using Inputs;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Core.UsableItems
{
    public abstract class UsableItemBase : ItemBase
    {
        /// <summary>
        /// Called when the client wants to use the item.
        /// </summary>
        /// <param name="ctx">Input Context</param>
        public virtual void OnClientUse(InputAction.CallbackContext ctx)
        {
            //client stuff
            SendUseMessage();
        }

        /// <summary>
        /// Tells tell server that the client wants to use the item.
        /// </summary>
        protected void SendUseMessage()
        {
            NetworkClient.Send(new UseUsableItemMessage()
            {
                item = ItemData.ItemIdentifier
            });
        }

        /// <summary>
        /// Called on the server when it receives a message to use an item.
        /// </summary>
        public virtual void OnServerReceiveUseMessage()
        {
            StartCoroutine(Use());
        }

        public IEnumerator Use()
        {
            if (ItemData.GetType() == typeof(UsableItemData))
            {
                UsableItemData uid = (UsableItemData)ItemData;

                if (anim != null)
                    anim.SetTrigger(StringKeys.ItemUseAnimation);

                yield return new WaitForSeconds(uid.Usetime);

                Effect();
            }
        }

        public virtual void Effect()
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