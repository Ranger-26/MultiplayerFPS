using System.Collections;
using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Core.UsableItems;
using Game.GameLogic.ItemSystem.Inventory;
using Game.Player;
using Inputs;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Items.Consumables.GenericConsumables
{
    public class ConsumableBase : UsableItemBase
    {
        [HideInInspector]
        public ConsumableData ConsumableData;

        public override void InitItem(NetworkGamePlayer owner)
        {
            base.InitItem(owner);
            ConsumableData = (ConsumableData) ItemData;
        }

        public override bool OnEquip()
        {
            SubscribeToInputEvents();
            return true;
        }

        public override bool OnDeEquip()
        {
            UnSubscribeFromInputEvents();
            return true;
        }

        public virtual void SubscribeToInputEvents()
        {
            GameInputManager.Actions.Player.Fire.performed += OnClientUse;
        }

        public virtual void UnSubscribeFromInputEvents()
        {
            GameInputManager.Actions.Player.Fire.performed -= OnClientUse;
        }
        
        
        public virtual void ServerOnConsume()
        {
            Owner.GetComponent<PlayerInventory>().ServerDestroyHeldItem();
        }

        public virtual void OnClientUse(InputAction.CallbackContext ctx)
        {
            NetworkClient.Send(new GenericConsumableMessage());
        }
    }
}