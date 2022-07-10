using Game.GameLogic.ItemSystem.Core.RuntimeData;
using Game.Player;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Core
{
    public abstract class ItemBase : MonoBehaviour
    {
        public ScriptableItemBase ItemData;

        public NetworkGamePlayer Owner;
        
        public virtual void InitItem(NetworkGamePlayer owner)
        {
            Owner = owner;
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
            return true;
        }

        public virtual bool OnDeEquip()
        {
            return true;
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