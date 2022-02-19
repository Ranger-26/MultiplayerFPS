using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ItemSystem.Core
{
    public class ItemDatabase : MonoBehaviour
    {
        private Dictionary<ItemType, ItemBase> _idsToItems = new Dictionary<ItemType, ItemBase>();

        private Dictionary<ItemType, ItemViewModel> _idsToViewModels = new Dictionary<ItemType, ItemViewModel>();

        [SerializeField] private List<ItemBase> _items = new List<ItemBase>();

        public static ItemDatabase Instance;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            foreach (var item in _items)
            {
                _idsToItems.Add(item.ItemId, item);
                _idsToViewModels.Add(item.ItemId, item.clientModel);
            }
        }

        public ItemBase TryGetItem(ItemType item)
        {
            try
            {
                return _idsToItems[item];
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occured when trying to get the item prefab for item id {item}: {e}");
                return null;
            }
        }
    }
}