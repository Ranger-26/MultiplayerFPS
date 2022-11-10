using Game.GameLogic.ItemSystem.Core.RuntimeData;
using Game.Player;
using Inputs;
using Lobby;
using Mirror;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Core
{
    public abstract class ItemBase : MonoBehaviour
    {
        public ScriptableItemBase ItemData;

        [HideInInspector]
        public NetworkGamePlayer Owner;

        public bool IsItemOwner => Owner.hasAuthority;

        public bool IsServer => NetworkPlayerLobby.localPlayer.isServer;

        [HideInInspector]
        public Animator anim;
        
        public virtual void InitItem(NetworkGamePlayer owner)
        {
            Owner = owner;
            anim = GetComponent<Animator>();
        }

        public virtual void ServerSetRuntimeData(IRuntimeData data)
        {
            if (!ValidateRuntimeData(data))
            {
                Debug.LogError($"Problem when validating runtime data! Item {ItemData.ItemIdentifier}");
            }
        }
        
        public virtual bool OnEquip()
        {
            if (IsItemOwner)
            {
                Invoke(nameof(RegisterInputEvents), ItemData.ItemDrawTime);
                
                if (anim != null)
                    anim.SetTrigger(StringKeys.GunDrawAnimation);
            }

            return true;
        }

        public virtual bool OnDeEquip()
        {
            if (IsItemOwner)
            {
                UnRegisterInputEvents();
            }

            return true;
        }

        protected virtual void RegisterInputEvents()
        {
            
        }

        protected virtual void UnRegisterInputEvents()
        {
            
        }
        
        public virtual void ResetViewModel()
        {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.Euler(0,180,0);
        }
        
        public virtual bool ValidateRuntimeData(IRuntimeData runtimeData)
        {
            return true;
        }
    }
}