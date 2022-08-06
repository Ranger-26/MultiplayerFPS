using System;
using Mirror;
using UnityEngine;

namespace Game.GameLogic.Interactables
{
    public abstract class ServerInteractableTrigger : NetworkBehaviour, IInteractable
    {
        public bool CanInteract;

        public virtual void ServerInteract()
        {
            
        }

        [Command(requiresAuthority = false)]
        public void CmdInteract()
        {
            if (CanInteract)
            {
                ServerInteract();
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            CanInteract = true;
        }

        public void OnTriggerExit(Collider other)
        {
            CanInteract = false;
        }

        [Client]
        public virtual void ClientInteract()
        {
            CmdInteract();
        }
    }
}