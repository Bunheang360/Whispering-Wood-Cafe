using UnityEngine;
using Game.Inventory;

public class CheatCodes : MonoBehaviour
{
  [Header("Cheat Settings")]
  [Tooltip("How many berries to give when cheat is activated")]
  public int berryAmount = 10;

  [Tooltip("How many water to give when cheat is activated")]
  public int waterAmount = 10;

  void Update()
  {
    // Press F1 to get berries and water
    if (Input.GetKeyDown(KeyCode.F1))
    {
      GiveIngredients();
    }

    // Press F2 to give even more (for testing)
    if (Input.GetKeyDown(KeyCode.F2))
    {
      GiveLargeAmount();
    }

    // Press F3 to clear all inventory
    if (Input.GetKeyDown(KeyCode.F3))
    {
      ClearInventory();
    }
  }

  void GiveIngredients()
  {
    if (Inventory.Instance == null)
    {
      Debug.LogWarning("Inventory not found! Make sure Inventory GameObject is in the scene.");
      return;
    }

    Inventory.Instance.AddItem("Berry", berryAmount);
    Inventory.Instance.AddItem("Water", waterAmount);

    Debug.Log($"<color=yellow>CHEAT ACTIVATED!</color> Added {berryAmount} Berries and {waterAmount} Water");

    // Show notification
    InventoryUI.ShowNotification($"Cheat: +{berryAmount} Berry, +{waterAmount} Water");
  }

  void GiveLargeAmount()
  {
    if (Inventory.Instance == null)
    {
      Debug.LogWarning("Inventory not found!");
      return;
    }

    Inventory.Instance.AddItem("Berry", 99);
    Inventory.Instance.AddItem("Water", 99);
    Inventory.Instance.AddItem("Fish", 99);

    Debug.Log("<color=yellow>MEGA CHEAT!</color> Added 99 of everything!");

    InventoryUI.ShowNotification("Mega Cheat: +99 Everything!");
  }

  void ClearInventory()
  {
    if (Inventory.Instance == null)
    {
      Debug.LogWarning("Inventory not found!");
      return;
    }

    // This would require a method in Inventory to clear all items
    Debug.Log("<color=red>Clear inventory cheat activated</color>");

    InventoryUI.ShowNotification("Inventory cleared!");
  }
}
