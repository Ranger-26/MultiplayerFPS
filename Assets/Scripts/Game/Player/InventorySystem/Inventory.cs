using System;
using System.Linq;
using Game.GameLogic;
using Game.ItemSystem.Core;
using Mirror;
using UnityEngine;

namespace Game.Player.InventorySystem
{
    public class Inventory : NetworkBehaviour
    {
        private readonly SyncList<ItemType> _curItems = new SyncList<ItemType>();

        private NetworkGamePlayer player;

        [SerializeField]
        private ItemBase _curItemBase;
        private void Start()
        {
            player = GetComponent<NetworkGamePlayer>();

            if (hasAuthority)
            {
                CmdAddItem(ItemType.Medkit);
            }
        }

        [SyncVar]
        private ItemType _curItem = ItemType.None;

        [Server]
        public void ServerAddItem(ItemType item)
        {
            if (_curItems.Count < 8)
            {
                _curItems.Add(item);
            }
            TargetPrintOutItems();
        }
        
        
        [Command]
        public void CmdAddItem(ItemType item) => ServerAddItem(item);
        
        [Command]
        public void CmdEquiptItem(int index) => ServerEquiptItem(index);

        [Command]
        public void CmdDequiptItem() => ServerDequiptItem();
        
        [Server]
        private void ServerEquiptItem(int index)
        {
            if (_curItems[index] == 0) return;
            _curItem = _curItems[index];
            RpcEqupitItem(_curItem, player.playerId);
            if (ItemDatabase.Instance.TryGetItem(_curItem, out ItemBase item))
            {
                _curItemBase = item;
                _curItemBase.OnItemEquipt(player);
            }
        }

        [Server]
        private void ServerDequiptItem()
        {
            Debug.Log("Trying to dequipt item...");
            if (_curItem == ItemType.None) return;
            _curItem = ItemType.None;
            _curItemBase = null;
            RpcDequiptItem(player.playerId);
            Debug.Log("Item dequipted successfully...");
        }

        [ClientRpc]
        private void RpcDequiptItem(int playerId)
        {
            NetworkGamePlayer player = GetPlayerById(playerId);
            ItemViewModel model = player.transform.GetComponentInChildren<ItemViewModel>();
            Destroy(model.gameObject);
        }
        
        [ClientRpc]
        public void RpcEqupitItem(ItemType item, int playerId)
        {
            if (ItemDatabase.Instance.TryGetItem(item, out ItemViewModel model))
            {
                NetworkGamePlayer owner = GetPlayerById(playerId);
                GameObject thing = Instantiate(model.gameObject, owner.transform.position, Quaternion.identity);
                thing.transform.SetParent(owner.transform);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && hasAuthority)
            {
                CmdEquiptItem(0);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && hasAuthority)
            {
                CmdUseHeldItem();
            }
            
            if (Input.GetKeyDown(KeyCode.T) && hasAuthority)
            {
                CmdDequiptItem();
            }
            _curItemBase?.OnUpdate(player);
        }

        [Command]
        private void CmdUseHeldItem()
        {
            Debug.Log("Using held item...");
            if (_curItem == ItemType.None || _curItemBase == null) return;
            Debug.Log("Using held item 2...");
            _curItemBase.OnUse(player);
        }
        
        
        [TargetRpc]
        private void TargetPrintOutItems()
        {
            for (int i = 0; i < _curItems.Count; i++)
            {
               Debug.Log($"Item: {_curItems[i]}"); 
            }
        }

        public NetworkGamePlayer GetPlayerById(int id)
        {
            NetworkGamePlayer[] players = FindObjectsOfType<NetworkGamePlayer>();
            return players.Where(x => x.playerId == id).ToList()[0];
        }
    }
}