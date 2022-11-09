using System.Collections;
using Game.GameLogic.ItemSystem.Core.UsableItems;
using Game.Player.Damage;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Items.Consumables.SCP500
{
    public class Scp500Item : UsableItemBase
    {
        public override void OnClientUse(InputAction.CallbackContext ctx)
        {
            if (gameObject.activeSelf)
                SendUseMessage();
        }

        public override void Effect()
        {
            Owner.GetComponent<HealthController>().ServerHealPlayer(100);
        }
    }
}