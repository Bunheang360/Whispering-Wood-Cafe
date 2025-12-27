using System.Collections.Generic;
using UnityEngine;

namespace Game.Quests
{
    public class QuestUI : MonoBehaviour
    {
        private bool showQuestLog = false; // Already hidden by default
        private Vector2 scrollPosition = Vector2.zero;

        // GUI Styles
        private GUIStyle panelStyle;
        private GUIStyle headerStyle;
        private GUIStyle questTitleStyle;
        private GUIStyle questDescStyle;
        private GUIStyle requirementStyle;
        private GUIStyle requirementCompleteStyle;
        private GUIStyle categoryStyle;
        private GUIStyle footerStyle;
        private GUIStyle emptyStyle;
        private GUIStyle separatorStyle;
        private bool stylesInitialized = false;

        private void Update()
        {
            // Toggle quest log visibility with 'Q' key
            if (Input.GetKeyDown(KeyCode.Q))
            {
                showQuestLog = !showQuestLog;
            }
        }

        private void OnGUI()
        {
            // Initialize styles on first run
            if (!stylesInitialized)
            {
                InitializeStyles();
                stylesInitialized = true;
            }

            // Return early if quest log is hidden
            if (!showQuestLog) return;

            // Check if QuestManager instance exists
            if (QuestManager.Instance == null)
            {
                GUI.Label(new Rect(Screen.width - 220, 20, 200, 30), "QuestManager not initialized");
                return;
            }

            // Get active quests
            List<Quest> activeQuests = QuestManager.Instance.GetActiveQuests();
            int questCount = activeQuests.Count;

            // Quest log panel positioned on the right side of screen
            GUILayout.BeginArea(new Rect(Screen.width - 620, 20, 600, 450), panelStyle);

            // Header
            GUILayout.Space(10);
            GUILayout.Label($"QUEST LOG ({questCount} quests)", headerStyle);
            GUILayout.Space(15);

            // Separator line
            GUILayout.Box("", separatorStyle, GUILayout.Height(2), GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            // Check if quest log is empty
            if (questCount == 0)
            {
                GUILayout.Space(20);
                GUILayout.Label("No Active Quests", emptyStyle);
                GUILayout.Label("Talk to NPCs to receive quests", emptyStyle);
            }
            else
            {
                // Category header
                GUILayout.Label("Active Quests", categoryStyle);
                GUILayout.Space(10);

                // Begin scroll view for quests
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));

                // Display each quest
                foreach (Quest quest in activeQuests)
                {
                    DisplayQuest(quest);
                    GUILayout.Space(15);
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
            GUILayout.Label("Press 'Q' to toggle quest log", footerStyle);
            GUILayout.Space(10);

            GUILayout.EndArea();
        }

        private void DisplayQuest(Quest quest)
        {
            // Update progress from inventory
            QuestManager.Instance.UpdateQuestProgress(quest);

            // Quest Title
            GUILayout.Label(quest.questTitle, questTitleStyle);
            GUILayout.Space(3);

            // Time remaining (if quest has time limit)
            if (quest.hasTimeLimit)
            {
                string timeText = QuestManager.Instance.GetTimeRemainingText(quest);
                Color oldColor = GUI.color;

                // Change color based on time remaining
                if (quest.timeRemaining < 30f)
                    GUI.color = new Color(1f, 0.3f, 0.3f); // Red warning
                else if (quest.timeRemaining < 60f)
                    GUI.color = new Color(1f, 0.8f, 0.2f); // Yellow warning
                else
                    GUI.color = new Color(0.7f, 0.9f, 1f); // Light blue

                GUILayout.Label($"⏱ Time Remaining: {timeText}", requirementStyle);
                GUI.color = oldColor;
                GUILayout.Space(5);
            }

            // Quest Description
            GUILayout.Label(quest.questDescription, questDescStyle);
            GUILayout.Space(8);

            // Requirements with progress
            foreach (QuestRequirement req in quest.requirements)
            {
                bool isComplete = req.currentAmount >= req.requiredAmount;
                GUIStyle style = isComplete ? requirementCompleteStyle : requirementStyle;

                string checkmark = isComplete ? " ✓" : "";
                GUILayout.Label($"  • {req.itemName}: {req.currentAmount}/{req.requiredAmount}{checkmark}", style);
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

            // Quest title style
            questTitleStyle = new GUIStyle(GUI.skin.label);
            questTitleStyle.fontSize = 16;
            questTitleStyle.fontStyle = FontStyle.Bold;
            questTitleStyle.normal.textColor = new Color(1f, 0.84f, 0f); // Gold color
            questTitleStyle.padding = new RectOffset(5, 5, 3, 3);

            // Quest description style
            questDescStyle = new GUIStyle(GUI.skin.label);
            questDescStyle.fontSize = 13;
            questDescStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f); // Light gray
            questDescStyle.padding = new RectOffset(5, 5, 2, 2);
            questDescStyle.wordWrap = true;

            // Requirement style (incomplete)
            requirementStyle = new GUIStyle(GUI.skin.label);
            requirementStyle.fontSize = 14;
            requirementStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f); // White-ish
            requirementStyle.padding = new RectOffset(5, 5, 2, 2);

            // Requirement style (complete)
            requirementCompleteStyle = new GUIStyle(GUI.skin.label);
            requirementCompleteStyle.fontSize = 14;
            requirementCompleteStyle.fontStyle = FontStyle.Bold;
            requirementCompleteStyle.normal.textColor = new Color(0.4f, 1f, 0.6f); // Light green
            requirementCompleteStyle.padding = new RectOffset(5, 5, 2, 2);

            // Category header style
            categoryStyle = new GUIStyle(GUI.skin.label);
            categoryStyle.fontSize = 16;
            categoryStyle.fontStyle = FontStyle.Bold;
            categoryStyle.normal.textColor = new Color(0.7f, 0.9f, 1f); // Light blue
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
