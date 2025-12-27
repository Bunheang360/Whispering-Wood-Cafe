using UnityEngine;
using Game.Inventory;
using Game.Quests;

/// <summary>
/// Helper script to debug quest and inventory issues
/// Attach to any GameObject and it will show debug info in console
/// </summary>
public class QuestDebugHelper : MonoBehaviour
{
    [Header("Press these keys to debug")]
    [Tooltip("Press L to log current inventory")]
    public KeyCode logInventoryKey = KeyCode.L;

    [Tooltip("Press K to log active quests")]
    public KeyCode logQuestsKey = KeyCode.K;

    void Update()
    {
        // Log inventory contents
        if (Input.GetKeyDown(logInventoryKey))
        {
            LogInventory();
        }

        // Log active quests
        if (Input.GetKeyDown(logQuestsKey))
        {
            LogActiveQuests();
        }
    }

    void LogInventory()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory.Instance is NULL! Make sure Inventory GameObject is in the scene.");
            return;
        }

        var items = Inventory.Instance.GetItems();

        if (items.Count == 0)
        {
            Debug.Log("=== INVENTORY IS EMPTY ===");
            return;
        }

        Debug.Log("=== CURRENT INVENTORY ===");
        foreach (var item in items)
        {
            Debug.Log($"  Item: '{item.Key}' | Quantity: {item.Value}");
        }
        Debug.Log($"Total unique items: {items.Count}");
    }

    void LogActiveQuests()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogError("QuestManager.Instance is NULL! Make sure QuestManager GameObject is in the scene.");
            return;
        }

        var quests = QuestManager.Instance.GetActiveQuests();

        if (quests.Count == 0)
        {
            Debug.Log("=== NO ACTIVE QUESTS ===");
            return;
        }

        Debug.Log("=== ACTIVE QUESTS ===");
        foreach (var quest in quests)
        {
            Debug.Log($"\nQuest: {quest.questTitle} (ID: {quest.questID})");
            Debug.Log($"  Description: {quest.questDescription}");
            Debug.Log($"  Requirements:");

            foreach (var req in quest.requirements)
            {
                int inventoryAmount = Inventory.Instance.GetItemQuantity(req.itemName);
                bool hasEnough = inventoryAmount >= req.requiredAmount;
                string status = hasEnough ? "✓ COMPLETE" : "✗ INCOMPLETE";

                Debug.Log($"    - '{req.itemName}': {inventoryAmount}/{req.requiredAmount} {status}");
            }

            bool canComplete = QuestManager.Instance.IsQuestComplete(quest);
            Debug.Log($"  Can complete quest: {canComplete}");
        }
    }

    void OnGUI()
    {
        // Show helper text on screen
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.yellow;

        GUI.Label(new Rect(10, Screen.height - 60, 400, 50),
            $"Press {logInventoryKey} to log inventory\nPress {logQuestsKey} to log quests",
            style);
    }
}
