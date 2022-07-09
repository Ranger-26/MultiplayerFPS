using Game.GameLogic.ItemSystem.Core.RuntimeData;
using Game.Player;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Core
{
    public abstract class ItemBase : MonoBehaviour
    {
        public ScriptableItemBase ItemData;

        public NetworkGamePlayer Owner;

        public IRuntimeData RuntimeData;
        
        public abstract void SubscribeToInputEvents();

        public abstract void UnSubscribeFromInputEvents();

        public virtual void InitItem(NetworkGamePlayer owner, IRuntimeData runtimeData)
        {
            Owner = owner;
            RuntimeData = runtimeData;
            if (!ValidateRuntimeData(runtimeData))
            {
                Debug.LogError($"Problem when validating runtime data! Item {ItemData.ItemIdentifier}");
            }
        }
        
        public virtual bool OnEquip()
        {
            return true;
        }

        public virtual bool OnDeEquip()
        {
            return true;
        }

        public virtual void ResetViewModel()
        {
            transform.localRotation = Quaternion.Euler(0,180,0);
        }
        
        public virtual bool ValidateRuntimeData(IRuntimeData runtimeData)
        {
            return true;
        }
    }
}