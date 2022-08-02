using System.Collections;
using Game.GameLogic.ItemSystem.Core;
using Game.Player;
using Inputs;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Items.Consumables.SCP500
{
    public class Scp500Item : UsableItemBase
    {
        public Animator Animator;

        public bool InUse;
        
        public override void InitItem(NetworkGamePlayer owner)
        {
            base.InitItem(owner);
            Animator = GetComponent<Animator>();
        }

        public override void OnUse(InputAction.CallbackContext ctx)
        {
            StartCoroutine(Test());
            InUse = true;
        }

        public IEnumerator Test()
        {
            transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
            yield return new WaitForSeconds(3f);
            transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
            NetworkClient.Send(new Scp500Message());
        }

        public void CancelUse(InputAction.CallbackContext ctx)
        {
            StopCoroutine(Test());
        }

        public override void ResetViewModel()
        {
            transform.localPosition = new Vector3(0.22f, 0, 2.32f);
        }

        protected override void RegisterInputEvents()
        {
            base.RegisterInputEvents();
            GameInputManager.Actions.Player.AltFire.performed += CancelUse;
        }

        protected override void UnRegisterInputEvents()
        {
            base.UnRegisterInputEvents();
            GameInputManager.Actions.Player.AltFire.performed -= CancelUse;
        }
    }
}