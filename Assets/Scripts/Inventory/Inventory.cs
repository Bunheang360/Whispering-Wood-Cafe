using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        private Dictionary<string, int> items = new Dictionary<string, int>();

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Add an item to the inventory or increase its quantity if it already exists
        /// </summary>
        public void AddItem(string itemName, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning("Cannot add item with empty name");
                return;
            }

            if (quantity <= 0)
            {
                Debug.LogWarning("Quantity must be greater than 0");
                return;
            }

            if (items.ContainsKey(itemName))
            {
                items[itemName] += quantity;
            }
            else
            {
                items.Add(itemName, quantity);
            }

            Debug.Log($"Added {quantity} {itemName}(s). Total: {items[itemName]}");
        }

        /// <summary>
        /// Remove a specific quantity of an item. If quantity reaches 0, removes the item entirely
        /// </summary>
        public void RemoveItem(string itemName, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning("Cannot remove item with empty name");
                return;
            }

            if (!items.ContainsKey(itemName))
            {
                Debug.LogWarning($"Item '{itemName}' not found in inventory");
                return;
            }

            items[itemName] -= quantity;

            if (items[itemName] <= 0)
            {
                items.Remove(itemName);
                Debug.Log($"Removed all {itemName}(s) from inventory");
            }
            else
            {
                Debug.Log($"Removed {quantity} {itemName}(s). Remaining: {items[itemName]}");
            }
        }

        /// <summary>
        /// Remove all of a specific item from inventory
        /// </summary>
        public void RemoveAllOfItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning("Cannot remove item with empty name");
                return;
            }

            if (items.ContainsKey(itemName))
            {
                items.Remove(itemName);
                Debug.Log($"Removed all {itemName}(s) from inventory");
            }
            else
            {
                Debug.LogWarning($"Item '{itemName}' not found in inventory");
            }
        }

        /// <summary>
        /// Get the items dictionary
        /// </summary>
        public Dictionary<string, int> GetItems()
        {
            return items;
        }

        /// <summary>
        /// Clear all items from inventory
        /// </summary>
        public void ClearInventory()
        {
            items.Clear();
            Debug.Log("Inventory cleared");
        }

        /// <summary>
        /// Get the number of unique items in inventory
        /// </summary>
        public int GetUniqueItemCount()
        {
            return items.Count;
        }

        /// <summary>
        /// Get the quantity of a specific item
        /// </summary>
        public int GetItemQuantity(string itemName)
        {
            if (items.ContainsKey(itemName))
            {
                return items[itemName];
            }
            return 0;
        }
    }
}
