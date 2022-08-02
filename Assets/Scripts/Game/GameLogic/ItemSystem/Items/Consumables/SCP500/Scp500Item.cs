using System.Collections;
using Game.GameLogic.ItemSystem.Core;
using Game.Player;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Items.Consumables.SCP500
{
    public class Scp500Item : UsableItemBase
    {
        public Animator Animator;

        public override void InitItem(NetworkGamePlayer owner)
        {
            base.InitItem(owner);
            Animator = GetComponent<Animator>();
        }

        public override void OnUse(InputAction.CallbackContext ctx)
        {
            NetworkClient.Send(new Scp500Message()
            {
                
            });
            StartCoroutine(TestAnim());
        }

        public IEnumerator TestAnim()
        {
            transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
            yield return new WaitForSeconds(3f);
            transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
        }
    }
}