using System;
using Game.GameLogic.Interactables;
using Inputs;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class InteractionScript : NetworkBehaviour
    {
        public IInteractable CurrentInteractable;
        private void Start()
        {
            if (!hasAuthority)
            {
                enabled = false;
            }
            else
            {
                GameInputManager.Actions.Player.Interact.performed += InteractWithCurrentInteractable;
            }
        }

        public void InteractWithCurrentInteractable(InputAction.CallbackContext ctx)
        {
            if (CurrentInteractable != null)
            {
                CurrentInteractable.ClientInteract();
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                CurrentInteractable = interactable;
            }
            CurrentInteractable.Highlight();
        }

        public void OnTriggerExit(Collider other)
        {
            CurrentInteractable.UnHighlight();
            if (other.TryGetComponent(out IInteractable interactable))
            {
                CurrentInteractable = null;
            }
        }

        private void OnDestroy()
        {
            if (hasAuthority)
            {
                GameInputManager.Actions.Player.Interact.performed -= InteractWithCurrentInteractable;
            }
        }
    }
}