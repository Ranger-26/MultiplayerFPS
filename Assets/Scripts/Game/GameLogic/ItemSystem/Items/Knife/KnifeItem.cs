using System;
using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Core.RuntimeData;
using Game.Player;
using Inputs;
using Lobby;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Knife
{
    public class KnifeItem : ItemBase
    {
        public KnifeComponent KnifeComponent;
        public LayerMask Mask;
        public override void InitItem(NetworkGamePlayer owner)
        {
            base.InitItem(owner);
            KnifeComponent = GetComponent<KnifeComponent>();
        }

        public void SubscribeToInputEvents()
        {
            GameInputManager.Actions.Player.Fire.performed += KnifeComponent.LightAttack;
            GameInputManager.Actions.Player.AltFire.performed += KnifeComponent.HeavyAttack;
            GameInputManager.Actions.Player.Inspect.performed += KnifeComponent.Inspect;
        }

        public void UnSubscribeFromInputEvents()
        {
            GameInputManager.Actions.Player.Fire.performed -= KnifeComponent.LightAttack;
            GameInputManager.Actions.Player.AltFire.performed -= KnifeComponent.HeavyAttack;
            GameInputManager.Actions.Player.Inspect.performed -= KnifeComponent.Inspect;
        }

        public override bool OnEquip()
        {
            Debug.Log("Layer Mask:"+Mask.value);
            if (IsItemOwner)
            {
                Invoke(nameof(SubscribeToInputEvents), ItemData.ItemDrawTime);
            }

            return true;
        }

        public override bool OnDeEquip()
        {
            if (IsItemOwner)
            {
                Debug.Log("Unsubscribing from events!");
                UnSubscribeFromInputEvents();
            }

            return true;
        }
        
        public void OnDestroy()
        {
            if (IsItemOwner)
            {
                UnSubscribeFromInputEvents();
            }
        }
    }
}