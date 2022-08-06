using System;
using UnityEngine;

namespace Game.GameLogic.Interactables
{
    public class Interactable : MonoBehaviour
    {
        public bool CanInteract;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CanInteract = true;
                OnUpdateInteraction(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CanInteract = false;
                OnUpdateInteraction(false);
            }
        }

        public void Interact()
        {
            if (!CanInteract)
                return;

            OnInteract();
        }

        public event Action onInteract;
        public void OnInteract() { if (onInteract != null) onInteract(); }

        public event Action<bool> onUpdateInteraction;
        public void OnUpdateInteraction(bool state) { if (onUpdateInteraction != null) onUpdateInteraction(state); }
    }
}
