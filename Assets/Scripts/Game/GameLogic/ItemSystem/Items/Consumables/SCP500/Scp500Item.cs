using System.Collections;
using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Inventory;
using Game.GameLogic.ItemSystem.Items.Consumables.GenericConsumables;
using Game.Player;
using Game.Player.Damage;
using Inputs;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Items.Consumables.SCP500
{
    public class Scp500Item : ConsumableBase
    {
        public IEnumerator Test()
        {
            transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
            yield return new WaitForSeconds(3f);
            transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
            NetworkClient.Send(new GenericConsumableMessage());
        }

        public override void OnClientUse(InputAction.CallbackContext ctx)
        {
            StartCoroutine(Test());
        }

        public override void ServerOnConsume()
        {
            Owner.GetComponent<HealthController>().ServerHealPlayer(100);
            base.ServerOnConsume();
        }
    }
}