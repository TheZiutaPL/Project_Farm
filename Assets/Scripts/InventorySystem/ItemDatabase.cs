using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class ItemDatabase : MonoBehaviour
    {
        public static ItemDatabase Instance;
        [SerializeField] private List<InventoryItemSO> itemScriptables;

        private Dictionary<int, InventoryItemSO> itemDictionary;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            itemDictionary = new Dictionary<int, InventoryItemSO>();
            foreach (var item in itemScriptables)
            {
                if (itemDictionary.TryAdd(item.ItemID, item))
                    continue;

                Debug.LogError($"Error: multiple items with ID {item.ItemID}! Skipping to next...");
                continue;
            }
        }

        public InventoryItemSO GetItemByID(int id)
        {
            if (itemDictionary.TryGetValue(id, out InventoryItemSO item))
            {
                return item;
            }
            Debug.LogError($"Item ID {id} not found in database!");
            return null;
        }
    }
}