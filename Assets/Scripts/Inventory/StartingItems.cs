using UnityEngine;
using Game.Inventory;

public class StartingItems : MonoBehaviour
{
    void Start()
    {
        // Water is unlimited - never runs out
        Inventory.Instance.AddItem("Water", 999999);

        // Starting ingredients for testing
        Inventory.Instance.AddItem("Berries", 10);
        Inventory.Instance.AddItem("Fish", 10);

        Debug.Log("Starting items added! Water is unlimited.");
    }
}
