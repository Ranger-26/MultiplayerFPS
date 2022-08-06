using Game.GameLogic.Interactables;
using Mirror;
using UnityEngine;

namespace Game.GameLogic.Map.Doors
{
    public class Door : ServerInteractableTrigger
    {
        public Animator Animator;

        [SyncVar]
        public DoorState State;
        
        private void Start()
        {
            Animator = GetComponent<Animator>();
        }

        public override void ServerInteract()
        {
            if (State == DoorState.Moving)
            {
                return;
            }
            
        }
    }
}