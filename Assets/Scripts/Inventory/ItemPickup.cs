using UnityEngine;
using System.Collections;

namespace Game.Inventory
{
    [RequireComponent(typeof(Collider))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private string itemName = "Item";
        [SerializeField] private int quantity = 1;
        [SerializeField] private bool triggerPickupAnimation = true;

        [Header("Respawn Settings")]
        [SerializeField] private bool canRespawn = true;
        [SerializeField] private float respawnTime = 30f; // Time in seconds before item respawns

        private bool playerInRange = false;
        private GameObject playerObject = null;
        private bool isPickedUp = false;

        private void Awake()
        {
            // Ensure the collider is set to trigger
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void Update()
        {
            // Check if player is in range and presses E (and item is not picked up)
            if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isPickedUp)
            {
                PickupItem();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the colliding object is the player
            if (other.CompareTag("Player") || other.GetComponent<Supercyan.AnimalPeopleSample.SimpleSampleCharacterControl>() != null)
            {
                playerInRange = true;
                playerObject = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Check if the player is leaving
            if (other.CompareTag("Player") || other.GetComponent<Supercyan.AnimalPeopleSample.SimpleSampleCharacterControl>() != null)
            {
                playerInRange = false;
                playerObject = null;
            }
        }

        private void PickupItem()
        {
            // Add item to inventory
            if (Inventory.Instance != null)
            {
                Inventory.Instance.AddItem(itemName, quantity);

                // Show notification
                string quantityText = quantity > 1 ? $" x{quantity}" : "";
                InventoryUI.ShowNotification($"Item Collected: {itemName}{quantityText}");

                // Try to trigger pickup animation on the player
                if (triggerPickupAnimation && playerObject != null)
                {
                    Animator animator = playerObject.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetTrigger("Pickup");
                    }
                }

                // Handle respawn or destroy
                if (canRespawn)
                {
                    StartCoroutine(RespawnItem());
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogWarning("Inventory instance not found! Make sure Inventory is in the scene.");
            }
        }

        private IEnumerator RespawnItem()
        {
            isPickedUp = true;

            // Hide the item (disable renderer and collider)
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.enabled = false;
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            // Reset player range
            playerInRange = false;
            playerObject = null;

            // Wait for respawn time
            yield return new WaitForSeconds(respawnTime);

            // Re-enable the item
            foreach (Renderer r in renderers)
            {
                r.enabled = true;
            }

            if (col != null)
            {
                col.enabled = true;
            }

            isPickedUp = false;
            Debug.Log($"{itemName} has respawned!");
        }

        private void OnGUI()
        {
            // Show pickup prompt when player is in range and item is not picked up
            if (playerInRange && !isPickedUp)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

                // Check if the item is in front of the camera
                if (screenPos.z > 0)
                {
                    // Flip Y coordinate for GUI
                    screenPos.y = Screen.height - screenPos.y;

                    Color oldColor = GUI.color;
                    GUI.color = Color.yellow;

                    // Display prompt above the item
                    GUI.Label(new Rect(screenPos.x - 75, screenPos.y - 30, 150, 25),
                              $"Press E to pick up {itemName}");

                    GUI.color = oldColor;
                }
            }
        }

        // Optional: Draw a gizmo in the editor to visualize the pickup area
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}
