using System.Collections.Generic;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Core
{
    public static class ItemDatabase
    {
        public static Dictionary<ItemIdentifier, ItemBase> idsToBases = new();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Load()
        {
            ItemBase[] items = Resources.LoadAll<ItemBase>("Prefabs/Items");
            
            Debug.Log($"Found {items.Length} items!");

            foreach (var item in items)
            {
                idsToBases.Add(item.ItemData.ItemIdentifier, item);
            }
            
            Debug.Log($"Loaded {idsToBases.Count} items!");
        }

        public static ItemBase TryGetItem(ItemIdentifier id)
        {
            if (idsToBases[id] == null)
            {
                Debug.LogError($"Could not find a matching item for item id {id}");
                return null;
            }
            
            return idsToBases[id];
        }
    }
}