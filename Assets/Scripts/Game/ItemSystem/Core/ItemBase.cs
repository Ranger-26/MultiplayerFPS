using Game.Player;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.ItemSystem.Core
{
    public abstract class ItemBase : MonoBehaviour
    {
        public abstract ItemType ItemId { get; }

        public ItemViewModel clientModel;

        protected NetworkGamePlayer Owner;
        public virtual void InitalizePlayer(NetworkGamePlayer ply)
        {
            Owner = ply;
        }
        
        [Server]
        public virtual void OnItemEquipt()
        {
            
        }
        
        [Server]
        public virtual void OnItemDequipt()
        {
            
        }

        [Server]
        public virtual void OnHolsterItem()
        {
            
        }
        
        [Server]
        public virtual void OnUpdate()
        {
            
        }
        
        [Server]
        public virtual void OnUse()
        {
            
        }
    }

    public enum ItemType : byte
    {
        None,
        Medkit
    }
}