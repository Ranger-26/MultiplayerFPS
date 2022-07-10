using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Core.RuntimeData;
using Game.GameLogic.ItemSystem.Core.RuntimeData.DefaultRuntimeData;
using Game.Player;
using Inputs;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

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
            if (isServer)
            {
                Invoke(nameof(Test0), 1f);
            }

            if (hasAuthority)
            {
                GameInputManager.Actions.Player.Num1.performed += Test1;
                GameInputManager.Actions.Player.Num2.performed += Test2;
            }
        }

        public void Test0() => ServerAddItem(ItemIdentifier.Knife, new DefaultRuntimeData());
        
        public void Test1(InputAction.CallbackContext ctx) => EquipItem(0);

        public void Test2(InputAction.CallbackContext ctx) => DeEquipHeldItem();
        
        #region AddItem
        [Server]
        public void ServerAddItem(ItemIdentifier id, IRuntimeData data)
        {
            allItems.Add(allItems.Count, id);
            CreateItemInstance(id, data);
            RpcAddItem(id);
        }

        [ClientRpc]
        public void RpcAddItem(ItemIdentifier id)
        {
            if (!isServer)
            {
                CreateItemInstance(id, null);
            }
        }
        public void CreateItemInstance(ItemIdentifier item, IRuntimeData data)
        {
            GameObject obj = Instantiate(ItemDatabase.TryGetItem(item).gameObject, ItemParent);
            ItemBase baseItem = obj.GetComponent<ItemBase>();
            baseItem.InitItem(Player);
            if (isServer)
            {
                baseItem.ServerSetRuntimeData(data);
            }
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
                if (isServer)
                {
                    ServerDeEquipHeldItem();
                }
                else
                {
                    CurrentItemBase.gameObject.SetActive(false);
                    CurrentItemBase = null;
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
            if (CurrentItemBase.OnDeEquip())
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
                if (CurrentItemBase != null)
                {
                    CurrentItemBase.gameObject.SetActive(false);
                    CurrentItemBase = null;
                }
            }
        }
        #endregion

        #region Equip

        public void EquipItem(int id)
        {
            if (allItems[id] == currentItem) return;
            
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
            if (allItems[id] == currentItem) return;

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

        private void OnDestroy()
        {
            GameInputManager.Actions.Player.Num1.performed -= Test1;
            GameInputManager.Actions.Player.Num2.performed -= Test2;
        }

        #endregion
    }
}