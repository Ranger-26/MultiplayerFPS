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
        public virtual void OnItemEquipt(NetworkGamePlayer ply)
        {
            Debug.Log("Initalizing item....");
        }
        
        [Server]
        public virtual void OnItemDequipt(NetworkGamePlayer ply)
        {
            
        }

        [Server]
        public virtual void OnHolsterItem(NetworkGamePlayer ply)
        {
            
        }
        
        [ServerCallback]
        public virtual void OnUpdate(NetworkGamePlayer ply)
        {
            
        }
        
        [Server]
        public virtual void OnUse(NetworkGamePlayer ply)
        {
            
        }
    }

    public enum ItemType : byte
    {
        None,
        Medkit
    }
}