using System.Collections.Generic;
using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Core.RuntimeData;
using Game.Player;
using Mirror;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Inventory
{
    public class PlayerInventory : NetworkBehaviour
    {
        public NetworkGamePlayer Player;
        
        public readonly SyncDictionary<int, ItemIdentifier> allItems = new();

        public Dictionary<int, ItemBase> allItemBases = new();

        [SerializeField]
        private Transform ItemParent;
        
        [SyncVar]
        public ItemIdentifier currentItem;

        public ItemBase CurrentItemBase;
        private void Start()
        {
            Player = GetComponent<NetworkGamePlayer>();
        }

        #region AddItem
        [Server]
        public void ServerAddItem(ItemIdentifier id, IRuntimeData data)
        {
            allItems.Add(allItems.Count, id);
            CreateItemInstance(id, data);
        }

        [ClientRpc]
        public void RpcAddItem(ItemIdentifier id, IRuntimeData data)
        {
            if (!isServer)
            {
                CreateItemInstance(id, data);
            }
        }
        public void CreateItemInstance(ItemIdentifier item, IRuntimeData data)
        {
            GameObject obj = Instantiate(ItemDatabase.TryGetItem(item).gameObject, ItemParent);
            ItemBase baseItem = obj.GetComponent<ItemBase>();
            baseItem.InitItem(Player, data);
            obj.SetActive(false);
            allItemBases.Add(allItemBases.Count, baseItem);
        }
        #endregion

        #region DeEquip

        public bool DeEquipHeldItem()
        {
            if (CurrentItemBase == null) return true;
            if (CurrentItemBase.OnDeEquip())
            {
                CurrentItemBase.gameObject.SetActive(false);
                CurrentItemBase = null;
                if (isServer)
                {
                    ServerDeEquipHeldItem();
                }
                else
                {
                    CmdDeEquipHeldItem();
                }
                return true;
            }

            return false;
        }

        [Command]
        public void CmdDeEquipHeldItem() => ServerDeEquipHeldItem();
        
        [Server]
        public bool ServerDeEquipHeldItem()
        {
            if (CurrentItemBase == null) return true;
            if ( CurrentItemBase.OnDeEquip())
            {
                CurrentItemBase.gameObject.SetActive(false);
                CurrentItemBase = null;
                currentItem = ItemIdentifier.None;
                RpcDeEquipHeldItem();
                return true;
            }

            return false;
        }

        [ClientRpc(includeOwner = false)]
        public void RpcDeEquipHeldItem()
        {
            if (!isServer)
            {
                CurrentItemBase.gameObject.SetActive(false);
                CurrentItemBase = null;
            }
        }
        #endregion

        #region Equip

        public void EquipItem(int id)
        {
            if (DeEquipHeldItem() && allItems.ContainsKey(id) && allItemBases[id].OnEquip())
            {
                CurrentItemBase = allItemBases[id];
                CurrentItemBase.gameObject.SetActive(true);
                CurrentItemBase.ResetViewModel();
                if (isServer)
                {
                    ServerEquipItem(id);
                }
                else
                {
                    CmdEquipItem(id);
                }
            }
        }

        [Command]
        public void CmdEquipItem(int id) => ServerEquipItem(id);
        
        [Server]
        public void ServerEquipItem(int id)
        {
            if (DeEquipHeldItem() && allItems.ContainsKey(id) && allItemBases[id].OnEquip())
            {
                currentItem = allItems[id];
                CurrentItemBase = allItemBases[id];
                CurrentItemBase.gameObject.SetActive(true);
                CurrentItemBase.ResetViewModel();
                RpcEquipItem(id);
            }
        }

        [ClientRpc(includeOwner = false)]
        public void RpcEquipItem(int id)
        {
            CurrentItemBase = allItemBases[id];
            CurrentItemBase.gameObject.SetActive(true);
            CurrentItemBase.ResetViewModel();
        }
        #endregion
    }
}