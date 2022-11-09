using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Core.RuntimeData;
using Game.GameLogic.ItemSystem.Core.RuntimeData.DefaultRuntimeData;
using Game.GameLogic.ItemSystem.Items.Firearms;
using Game.Player;
using Inputs;
using Menu;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Inventory
{
    public class PlayerInventory : NetworkBehaviour
    {
        public static PlayerInventory Local;

        public NetworkGamePlayer Player;

        public readonly SyncDictionary<int, ItemIdentifier> allItems = new();

        public Dictionary<int, ItemBase> allItemBases = new();

        [SerializeField]
        private Transform ItemParent;

        [SyncVar]
        public ItemIdentifier currentItem;

        [SyncVar(hook = nameof(OnIndexChange))]
        public int heldItemIndex = -1;

        public ItemBase CurrentItemBase
        {
            get
            {
                return cur;
            }
            set
            {
                OnEquip(value);
                cur = value;
            }
        }

        ItemBase cur;

        private void Start()
        {
            Player = GetComponent<NetworkGamePlayer>();
            if (isServer)
            {
                Invoke(nameof(Test0), 0.05f);
            }

            if (hasAuthority)
            {
                GameInputManager.Actions.Player.Num1.performed += Slot1;
                GameInputManager.Actions.Player.Num2.performed += Slot2;
                GameInputManager.Actions.Player.Num3.performed += Slot3;
                GameInputManager.Actions.Player.Num4.performed += Slot4;
                GameInputManager.Actions.Player.Num5.performed += Slot5;
                GameInputManager.Actions.Player.DropItem.performed += Drop;

                Local = this;
            }
        }

        public void Test0()
        {
            // ServerAddItem(ItemIdentifier.DebugGun, new FirearmRuntimeData(ItemIdentifier.DebugGun, -1, -1));

            #if UNITY_STANDALONE && !DEBUG && !UNITY_EDITOR

            ServerAddItem(ItemIdentifier.MP5K, new FirearmRuntimeData(ItemIdentifier.DebugGun, -1, -1));
            ServerAddItem(ItemIdentifier.Vityaz, new FirearmRuntimeData(ItemIdentifier.DebugGun, -1, -1));
            ServerAddItem(ItemIdentifier.Mossberg, new FirearmRuntimeData(ItemIdentifier.DebugGun, -1, -1));

            #elif UNITY_EDITOR || DEBUG
            
            switch (Settings.Current.StartingWeapon)
            {
                case 0:
                    ServerAddItem(ItemIdentifier.DebugGun, new FirearmRuntimeData(ItemIdentifier.DebugGun, -1, -1));
                    break;
                case 1:
                    ServerAddItem(ItemIdentifier.MP5K, new FirearmRuntimeData(ItemIdentifier.DebugGun, -1, -1));
                    break;
                case 2:
                    ServerAddItem(ItemIdentifier.Vityaz, new FirearmRuntimeData(ItemIdentifier.DebugGun, -1, -1));
                    break;
                case 3:
                    ServerAddItem(ItemIdentifier.Mossberg, new FirearmRuntimeData(ItemIdentifier.DebugGun, -1, -1));
                    break;
            }

            #endif

            ServerAddItem(ItemIdentifier.Makarov, new FirearmRuntimeData(ItemIdentifier.DebugGun, -1, -1));
            ServerAddItem(ItemIdentifier.Knife, new DefaultRuntimeData());
        }

        public void Slot1(InputAction.CallbackContext ctx) => EquipItem(0);

        public void Slot2(InputAction.CallbackContext ctx) => EquipItem(1);

        public void Slot3(InputAction.CallbackContext ctx) => EquipItem(2);

        public void Slot4(InputAction.CallbackContext ctx) => EquipItem(3);

        public void Slot5(InputAction.CallbackContext ctx) => EquipItem(4);

        public void Drop(InputAction.CallbackContext ctx) => RemoveItem(heldItemIndex);

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
            SetPlayer();
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
                heldItemIndex = -1;
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
            if (!allItems.ContainsKey(id) || allItems[id] == currentItem) return;

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
                heldItemIndex = id;
            }
        }

        public void OnIndexChange(int _, int cur)
        {
            OtherClientEquipItem(cur);
        }

        public void OtherClientEquipItem(int id)
        {
            if (isServer || hasAuthority) return;
            if (allItemBases.ContainsKey(id))
            {
                Debug.Log("Found item base, equiping.");
                CurrentItemBase = allItemBases[id];
            }
            else
            {
                if (allItems.ContainsKey(id))
                {
                    Debug.Log("Couldnt find item base but item exists, creating new base.");
                    GameObject obj = Instantiate(ItemDatabase.TryGetItem(allItems[id]).gameObject, ItemParent);
                    ItemBase baseItem = obj.GetComponent<ItemBase>();
                    CurrentItemBase = baseItem;
                    CurrentItemBase.InitItem(Player);
                    allItemBases.Add(id, CurrentItemBase);
                }
                else
                {
                    Debug.Log($"No item found for item id {id}, skipping.");
                    return;
                }
            }
            CurrentItemBase.gameObject.SetActive(true);
            CurrentItemBase.ResetViewModel();
        }

        public void RemoveItem(int id)
        {
            if (!allItems.ContainsKey(id) || !allItemBases.ContainsKey(id)) return;

            if (heldItemIndex == id)
            {
                if (isServer)
                {
                    ServerDestroyItem(id);
                }
                else
                {
                    if (DeEquipHeldItem())
                    {
                        CmdDestroyItem(id);
                    }
                }

            }
            else
            {
                if (isServer)
                {
                    ServerDestroyItem(id);
                }
                else
                {
                    CmdDestroyItem(id);
                }
            }
        }

        [Command]
        public void CmdDestroyItem(int id)
        {
            ServerDestroyItem(id);
        }

        [Server]
        public void ServerDestroyItem(int id)
        {
            if (!allItems.ContainsKey(id) || !allItemBases.ContainsKey(id)) return;
            if (id == heldItemIndex)
            {
                if (DeEquipHeldItem())
                {
                    allItems.Remove(id);
                    RpcDestroyItem(id);
                }
            }
            else
            {
                allItems.Remove(id);
                RpcDestroyItem(id);
            }
        }

        [Server]
        public void ServerDestroyHeldItem()
        {
            ServerDestroyItem(heldItemIndex);
        }


        [ClientRpc]
        public void RpcDestroyItem(int id)
        {
            if (!allItemBases.ContainsKey(id)) return;
            Destroy(allItemBases[id].gameObject);
        }

        private void OnDestroy()
        {
            if (hasAuthority)
            {
                GameInputManager.Actions.Player.Num1.performed -= Slot1;
                GameInputManager.Actions.Player.Num2.performed -= Slot2;
                GameInputManager.Actions.Player.Num3.performed -= Slot3;
                GameInputManager.Actions.Player.Num4.performed -= Slot4;
                GameInputManager.Actions.Player.Num5.performed -= Slot5;
                GameInputManager.Actions.Player.DropItem.performed += Drop;
            }
        }

        public void SetPlayer()
        {
            if (Player == null)
                Player = GetComponent<NetworkGamePlayer>();
        }

        #endregion

        #region Events

        public event Action<ItemBase> onEquip;
        public void OnEquip(ItemBase id)
        {
            if (onEquip != null)
                onEquip(id);
        }

        public event Action onInventoryUpdate;
        public void OnInventoryUpdate()
        {
            if (onInventoryUpdate != null)
                onInventoryUpdate();
        }

        #endregion
    }
}