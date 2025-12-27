using System.Collections.Generic;
using UnityEngine;
using Game.Inventory;

namespace Game.Quests
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quest Settings")]
        public int maxActiveQuests = 5;

        private List<Quest> activeQuests = new List<Quest>();

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

        private void Update()
        {
            // Update quest timers
            UpdateQuestTimers();
        }

        /// <summary>
        /// Add a quest to the active quest list
        /// </summary>
        public bool AddQuest(Quest quest)
        {
            if (quest == null)
            {
                Debug.LogWarning("Cannot add null quest");
                return false;
            }

            // Check if quest limit reached
            if (activeQuests.Count >= maxActiveQuests)
            {
                Debug.LogWarning($"Cannot accept quest. Maximum of {maxActiveQuests} active quests reached.");
                return false;
            }

            // Check if quest already active
            if (GetQuest(quest.questID) != null)
            {
                Debug.LogWarning($"Quest '{quest.questTitle}' is already active");
                return false;
            }

            // Create a copy of the quest to avoid modifying the original
            Quest questCopy = CopyQuest(quest);
            questCopy.isActive = true;
            questCopy.isCompleted = false;
            questCopy.isFailed = false;

            // Initialize timer if quest has time limit
            if (questCopy.hasTimeLimit)
            {
                questCopy.timeRemaining = questCopy.timeLimitInSeconds;
            }

            activeQuests.Add(questCopy);

            Debug.Log($"Quest accepted: {questCopy.questTitle}");
            InventoryUI.ShowNotification($"New Quest: {questCopy.questTitle}");

            return true;
        }

        /// <summary>
        /// Remove a quest from the active list
        /// </summary>
        public void RemoveQuest(string questID)
        {
            Quest quest = GetQuest(questID);
            if (quest != null)
            {
                activeQuests.Remove(quest);
                Debug.Log($"Quest removed: {quest.questTitle}");
            }
        }

        /// <summary>
        /// Get a quest by its ID
        /// </summary>
        public Quest GetQuest(string questID)
        {
            return activeQuests.Find(q => q.questID == questID);
        }

        /// <summary>
        /// Get all active quests
        /// </summary>
        public List<Quest> GetActiveQuests()
        {
            return activeQuests;
        }

        /// <summary>
        /// Check if player can accept more quests
        /// </summary>
        public bool CanAcceptQuest()
        {
            return activeQuests.Count < maxActiveQuests;
        }

        /// <summary>
        /// Check if a quest's requirements are met
        /// </summary>
        public bool IsQuestComplete(Quest quest)
        {
            if (quest == null || Inventory.Inventory.Instance == null)
                return false;

            foreach (QuestRequirement req in quest.requirements)
            {
                int inventoryAmount = Inventory.Inventory.Instance.GetItemQuantity(req.itemName);
                if (inventoryAmount < req.requiredAmount)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update quest progress from inventory
        /// </summary>
        public void UpdateQuestProgress(Quest quest)
        {
            if (quest == null || Inventory.Inventory.Instance == null)
                return;

            foreach (QuestRequirement req in quest.requirements)
            {
                req.currentAmount = Inventory.Inventory.Instance.GetItemQuantity(req.itemName);
            }
        }

        /// <summary>
        /// Complete a quest: remove required items, give rewards
        /// </summary>
        public void CompleteQuest(Quest quest)
        {
            if (quest == null || Inventory.Inventory.Instance == null)
            {
                Debug.LogWarning("Cannot complete quest: Quest or Inventory is null");
                return;
            }

            // Remove required items from inventory
            foreach (QuestRequirement req in quest.requirements)
            {
                Inventory.Inventory.Instance.RemoveItem(req.itemName, req.requiredAmount);
            }

            // Add rewards to inventory
            string rewardText = "";
            foreach (QuestReward reward in quest.rewards)
            {
                Inventory.Inventory.Instance.AddItem(reward.itemName, reward.quantity);
                rewardText += $"{reward.itemName} x{reward.quantity}, ";
            }

            // Mark as completed and remove from active quests
            quest.isCompleted = true;
            RemoveQuest(quest.questID);

            // Show completion notification
            Debug.Log($"Quest completed: {quest.questTitle}");
            InventoryUI.ShowNotification($"Quest Complete: {quest.questTitle}!");
        }

        /// <summary>
        /// Create a deep copy of a quest
        /// </summary>
        private Quest CopyQuest(Quest original)
        {
            Quest copy = new Quest
            {
                questID = original.questID,
                questTitle = original.questTitle,
                questDescription = original.questDescription,
                questGiverName = original.questGiverName,
                isActive = original.isActive,
                isCompleted = original.isCompleted,
                isFailed = original.isFailed,
                hasTimeLimit = original.hasTimeLimit,
                timeLimitInSeconds = original.timeLimitInSeconds,
                timeRemaining = original.timeRemaining
            };

            // Copy requirements
            copy.requirements = new QuestRequirement[original.requirements.Length];
            for (int i = 0; i < original.requirements.Length; i++)
            {
                copy.requirements[i] = new QuestRequirement
                {
                    itemName = original.requirements[i].itemName,
                    requiredAmount = original.requirements[i].requiredAmount,
                    currentAmount = 0
                };
            }

            // Copy rewards
            copy.rewards = new QuestReward[original.rewards.Length];
            for (int i = 0; i < original.rewards.Length; i++)
            {
                copy.rewards[i] = new QuestReward
                {
                    itemName = original.rewards[i].itemName,
                    quantity = original.rewards[i].quantity
                };
            }

            return copy;
        }

        /// <summary>
        /// Get total number of active quests
        /// </summary>
        public int GetActiveQuestCount()
        {
            return activeQuests.Count;
        }

        /// <summary>
        /// Update timers for all active quests with time limits
        /// </summary>
        private void UpdateQuestTimers()
        {
            List<Quest> questsToFail = new List<Quest>();

            foreach (Quest quest in activeQuests)
            {
                if (quest.hasTimeLimit && !quest.isCompleted && !quest.isFailed)
                {
                    quest.timeRemaining -= Time.deltaTime;

                    // Check if time expired
                    if (quest.timeRemaining <= 0f)
                    {
                        quest.timeRemaining = 0f;
                        questsToFail.Add(quest);
                    }
                }
            }

            // Fail expired quests
            foreach (Quest quest in questsToFail)
            {
                FailQuest(quest);
            }
        }

        /// <summary>
        /// Fail a quest due to time expiration
        /// </summary>
        public void FailQuest(Quest quest)
        {
            if (quest == null)
                return;

            quest.isFailed = true;
            quest.isActive = false;

            Debug.Log($"Quest failed: {quest.questTitle} - Time expired!");
            InventoryUI.ShowNotification($"Quest Failed: {quest.questTitle} (Time Expired)");

            RemoveQuest(quest.questID);
        }

        /// <summary>
        /// Get formatted time remaining for a quest
        /// </summary>
        public string GetTimeRemainingText(Quest quest)
        {
            if (quest == null || !quest.hasTimeLimit)
                return "";

            float seconds = quest.timeRemaining;
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);

            return $"{minutes:00}:{secs:00}";
        }
    }
}
