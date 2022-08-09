using System;
using System.Collections.Generic;
using Game.Player;
using Mirror;
using UnityEngine;

namespace Game.GameLogic.Interactables
{
    public abstract class ServerInteractableTrigger : NetworkBehaviour, IInteractable
    {
        public List<NetworkGamePlayer> allowedInteractions = new();

        public virtual void ServerInteract()
        {
            
        }
        
        [Command(requiresAuthority = false)]
        public void CmdInteract(NetworkConnectionToClient sender = null)
        {
            if (allowedInteractions.Contains(sender.identity.GetComponent<NetworkGamePlayer>()))
            {
                ServerInteract();
            }
        }

        [ServerCallback]
        public void OnTriggerEnter(Collider other)
        {
            if (isServer)
            {
                if (other.TryGetComponent(out NetworkGamePlayer player))
                {
                    allowedInteractions.Add(player);
                }
            }
        }
        
        [ServerCallback]
        public void OnTriggerExit(Collider other)
        {
            if (isServer)
            {
                if (other.TryGetComponent(out NetworkGamePlayer player))
                {
                    allowedInteractions.Remove(player);
                }
            }
        }

        [Client]
        public virtual void ClientInteract()
        {
            if (!isServer)
            {
                CmdInteract();
            }
            else
            {
                ServerInteract();
            }
        }

        public virtual void Highlight()
        {
            
        }

        public virtual void UnHighlight()
        {
            
        }
    }
}