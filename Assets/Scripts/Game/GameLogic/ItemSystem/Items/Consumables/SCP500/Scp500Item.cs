using System.Collections;
using Game.GameLogic.ItemSystem.Core.UsableItems;
using Game.Player.Damage;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Items.Consumables.SCP500
{
    public class Scp500Item : UsableItemBase
    {
        public IEnumerator Test()
        {
            transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
            yield return new WaitForSeconds(3f);
            transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
            SendUseMessage();
        }

        public override void OnClientUse(InputAction.CallbackContext ctx)
        {
            if (gameObject.activeSelf)
                StartCoroutine(Test());
        }

        public override void OnServerReceiveUseMessage()
        {
            Owner.GetComponent<HealthController>().ServerHealPlayer(100);
        }
    }
}