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

    public struct ItemIdentifier : IEquatable<ItemType>
    {
        public string Owner;
        public ItemType item;

        public bool Equals(ItemIdentifier other)
        {
            return Owner == other.Owner && item == other.item;
        }

        public bool Equals(ItemType other)
        {
            return other != null && item == other;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Owner != null ? Owner.GetHashCode() : 0) * 397) ^ (int) item;
            }
        }
    }
}