using System.Collections;
using Game.GameLogic.Interactables;
using Mirror;
using UnityEngine;

namespace Game.GameLogic.Map.Doors
{
    public class Door : ServerInteractableTrigger
    {
        public NetworkAnimator Animator;

        public MeshRenderer Light;
        
        [SyncVar(hook = nameof(OnDoorStateChange))]
        public DoorState State;
        
        private void Start()
        {
            Animator = GetComponent<NetworkAnimator>();
            OnDoorStateChange(DoorState.Closed, State);
        }

        public override void ServerInteract()
        {
            Debug.Log("ServerInteract invoked.");
            if (State == DoorState.Moving)
            {
                return;
            }
            StartCoroutine(State == DoorState.Closed
                ? DoorChangeState(DoorState.Open)
                : DoorChangeState(DoorState.Closed));
        }

        IEnumerator DoorChangeState(DoorState newState)
        {
            State = DoorState.Moving;
            Debug.Log($"New State: {newState}");
            if (newState == DoorState.Closed)
            {
                Animator.animator.SetBool("IsOpen", false);
            }
            else
            {
                Animator.animator.SetBool("IsOpen", true);
            }

            //Debug.Log($"IsOpen: {Animator.animator.GetCurrentAnimatorStateInfo(0).IsName("Open")}");
            //Debug.Log($"IsOpen: {Animator.animator.GetCurrentAnimatorStateInfo(0).IsName("Close")}");
            Debug.Log($"Time: {Animator.animator.GetCurrentAnimatorStateInfo(0).normalizedTime}");
            if (newState == DoorState.Open)
            {
                yield return new WaitUntil(() => !Animator.animator.GetCurrentAnimatorStateInfo(0).IsName("Close") && Animator.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
            }
            else
            {
                yield return new WaitUntil(() => !Animator.animator.GetCurrentAnimatorStateInfo(0).IsName("Open") && Animator.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
            }
            Debug.Log($"IsOpen: {Animator.animator.GetCurrentAnimatorStateInfo(0).IsName("Open")}");
            Debug.Log($"IsOpen: {Animator.animator.GetCurrentAnimatorStateInfo(0).IsName("Close")}");
            State = newState;
        }

        public override void ClientInteract()
        {
            if (State == DoorState.Moving) return;
            CmdInteract();
        }

        public void OnDoorStateChange(DoorState _, DoorState cur)
        {
            switch (cur)
            {
                case DoorState.Moving:
                    Light.material.color = Color.yellow;
                    break;
                case DoorState.Closed:
                    Light.material.color = Color.blue;
                    break;
                case DoorState.Open:
                    Light.material.color = Color.green;
                    break;
            }
        }
    }
}