using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        private bool showInventory = false;
        private Vector2 scrollPosition = Vector2.zero;

        // Notification system
        private static string notificationMessage = "";
        private static float notificationTimer = 0f;
        private static float notificationDuration = 3f;

        // GUI Styles
        private GUIStyle panelStyle;
        private GUIStyle headerStyle;
        private GUIStyle itemNameStyle;
        private GUIStyle quantityStyle;
        private GUIStyle categoryStyle;
        private GUIStyle footerStyle;
        private GUIStyle emptyStyle;
        private GUIStyle separatorStyle;
        private GUIStyle notificationStyle;
        private bool stylesInitialized = false;

        private void Update()
        {
            // Toggle inventory visibility with 'I' key
            if (Input.GetKeyDown(KeyCode.I))
            {
                showInventory = !showInventory;
            }

            // Update notification timer
            if (notificationTimer > 0)
            {
                notificationTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Show a notification message that fades after a few seconds
        /// </summary>
        public static void ShowNotification(string message)
        {
            notificationMessage = message;
            notificationTimer = notificationDuration;
        }

        private void OnGUI()
        {
            // Initialize styles on first run
            if (!stylesInitialized)
            {
                InitializeStyles();
                stylesInitialized = true;
            }

            // Always display notification regardless of inventory visibility
            DisplayNotification();

            // Return early if inventory is hidden
            if (!showInventory) return;

            // Check if Inventory instance exists
            if (Inventory.Instance == null)
            {
                GUI.Label(new Rect(20, 20, 200, 30), "Inventory not initialized");
                return;
            }

            // Get items from inventory
            Dictionary<string, int> items = Inventory.Instance.GetItems();
            int itemCount = items.Count;

            // Main inventory panel with fixed position and size (rectangle shape)
            GUILayout.BeginArea(new Rect(20, 20, 600, 350), panelStyle);

            // Header
            GUILayout.Space(10);
            GUILayout.Label($"INVENTORY ({itemCount} items)", headerStyle);
            GUILayout.Space(15);

            // Separator line
            GUILayout.Box("", separatorStyle, GUILayout.Height(2), GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            // Check if inventory is empty
            if (items.Count == 0)
            {
                GUILayout.Space(20);
                GUILayout.Label("Empty Inventory", emptyStyle);
                GUILayout.Label("Collect items to see them here", emptyStyle);
            }
            else
            {
                // Category header
                GUILayout.Label("Collected Items", categoryStyle);
                GUILayout.Space(10);

                // Begin scroll view for items
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

                // Display items
                List<string> itemNames = new List<string>(items.Keys);

                foreach (string itemName in itemNames)
                {
                    int quantity = items[itemName];

                    GUILayout.BeginHorizontal();

                    // Item name
                    GUILayout.Label(itemName, itemNameStyle, GUILayout.Width(470));

                    // Flexible space to push quantity to the right
                    GUILayout.FlexibleSpace();

                    // Quantity
                    GUILayout.Label($"x{quantity}", quantityStyle, GUILayout.Width(60));

                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                }

                GUILayout.EndScrollView();
            }

            // Flexible space to push footer to bottom
            GUILayout.FlexibleSpace();

            // Footer separator
            GUILayout.Space(10);
            GUILayout.Box("", separatorStyle, GUILayout.Height(2), GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            // Footer instructions
            GUILayout.Label("Press 'I' to toggle inventory", footerStyle);
            GUILayout.Space(10);

            GUILayout.EndArea();
        }

        private void DisplayNotification()
        {
            // Display notification if active
            if (notificationTimer > 0 && !string.IsNullOrEmpty(notificationMessage))
            {
                // Calculate fade alpha based on timer
                float alpha = Mathf.Clamp01(notificationTimer / notificationDuration);
                Color notifColor = new Color(1f, 1f, 1f, alpha);

                // Position notification at top center of screen
                float notifWidth = 400;
                float notifHeight = 60;
                float notifX = (Screen.width - notifWidth) / 2;
                float notifY = 80;

                // Draw notification background
                GUI.backgroundColor = new Color(0.2f, 0.8f, 0.3f, alpha * 0.9f);
                GUI.Box(new Rect(notifX, notifY, notifWidth, notifHeight), "");
                GUI.backgroundColor = Color.white;

                // Draw notification text
                GUI.color = notifColor;
                if (notificationStyle != null)
                {
                    GUI.Label(new Rect(notifX, notifY, notifWidth, notifHeight), notificationMessage, notificationStyle);
                }
                GUI.color = Color.white;
            }
        }

        private void InitializeStyles()
        {
            // Panel background style
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.95f));
            panelStyle.border = new RectOffset(12, 12, 12, 12);
            panelStyle.padding = new RectOffset(20, 20, 15, 15);

            // Header style
            headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 20;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = Color.white;
            headerStyle.alignment = TextAnchor.MiddleCenter;

            // Item name style
            itemNameStyle = new GUIStyle(GUI.skin.label);
            itemNameStyle.fontSize = 15;
            itemNameStyle.normal.textColor = new Color(0.95f, 0.95f, 0.95f);
            itemNameStyle.padding = new RectOffset(5, 5, 3, 3);

            // Quantity style
            quantityStyle = new GUIStyle(GUI.skin.label);
            quantityStyle.fontSize = 15;
            quantityStyle.fontStyle = FontStyle.Bold;
            quantityStyle.normal.textColor = new Color(0.4f, 1f, 0.6f); // Light green
            quantityStyle.alignment = TextAnchor.MiddleRight;

            // Category header style
            categoryStyle = new GUIStyle(GUI.skin.label);
            categoryStyle.fontSize = 16;
            categoryStyle.fontStyle = FontStyle.Bold;
            categoryStyle.normal.textColor = new Color(1f, 0.84f, 0f); // Gold
            categoryStyle.padding = new RectOffset(5, 5, 5, 5);

            // Footer style
            footerStyle = new GUIStyle(GUI.skin.label);
            footerStyle.fontSize = 12;
            footerStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f); // Gray
            footerStyle.alignment = TextAnchor.MiddleCenter;
            footerStyle.fontStyle = FontStyle.Italic;

            // Empty state style
            emptyStyle = new GUIStyle(GUI.skin.label);
            emptyStyle.fontSize = 14;
            emptyStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f); // Gray
            emptyStyle.alignment = TextAnchor.MiddleCenter;
            emptyStyle.fontStyle = FontStyle.Italic;

            // Separator style
            separatorStyle = new GUIStyle(GUI.skin.box);
            separatorStyle.normal.background = MakeTex(2, 2, new Color(0.4f, 0.4f, 0.4f, 0.8f));

            // Notification style
            notificationStyle = new GUIStyle(GUI.skin.label);
            notificationStyle.fontSize = 18;
            notificationStyle.fontStyle = FontStyle.Bold;
            notificationStyle.normal.textColor = Color.white;
            notificationStyle.alignment = TextAnchor.MiddleCenter;
        }

        private Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
