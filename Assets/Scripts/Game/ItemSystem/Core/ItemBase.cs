using System;
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
        
        [Server]
        public virtual void OnItemEquipt(NetworkGamePlayer ply)
        {
            
        }
        
        [Server]
        public virtual void OnItemDequipt(NetworkGamePlayer ply)
        {
            
        }

        [Server]
        public virtual void OnHolsterItem(NetworkGamePlayer ply)
        {
            
        }
        
        [Server]
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