using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName = "Village Elder";

    [Header("Dialogue Lines")]
    [TextArea(3, 10)]
    public string[] dialogueLines = new string[]
    {
        "Hello traveler! Welcome to our village.",
        "We've been expecting you.",
        "Feel free to explore around!"
    };

    [Header("Quest Settings")]
    public bool hasQuest = false;
    public Quest npcQuest;

    [Header("Quest Reset Settings")]
    [Tooltip("Enable quest reset after completion")]
    public bool canQuestReset = false;
    [Tooltip("Time in seconds before quest can be done again (e.g., 3600 = 1 hour, 86400 = 1 day)")]
    public float questResetTime = 3600f; // Default: 1 hour
    private float questCompletionTime = -1f; // Time when quest was completed

    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    public GameObject interactionPrompt; // The "Press E" UI

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        // Check distance to player
        float distance = Vector3.Distance(transform.position, player.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionRange;

        // Show/hide interaction prompt
        if (playerInRange != wasInRange && interactionPrompt != null)
        {
            interactionPrompt.SetActive(playerInRange);
        }

        // Trigger dialogue when E is pressed
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Don't start new dialogue if one is already active
            if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive())
            {
                TriggerDialogue();
            }
        }
    }

    void TriggerDialogue()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // Handle quest logic if NPC has quest
        if (hasQuest && Game.Quests.QuestManager.Instance != null)
        {
            HandleQuestInteraction();
            return;
        }

        // Original dialogue if no quest
        if (DialogueManager.Instance != null && dialogueLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(npcName, dialogueLines);
        }
    }

    void HandleQuestInteraction()
    {
        // Check if quest can be reset
        if (canQuestReset && npcQuest.isCompleted && CanResetQuest())
        {
            ResetQuest();
        }

        Quest activeQuest = Game.Quests.QuestManager.Instance.GetQuest(npcQuest.questID);

        if (activeQuest == null && !npcQuest.isCompleted)
        {
            // Quest not started - offer quest
            OfferQuest();
        }
        else if (activeQuest != null)
        {
            // Quest in progress - check if complete
            if (Game.Quests.QuestManager.Instance.IsQuestComplete(activeQuest))
            {
                SubmitQuest(activeQuest);
            }
            else
            {
                ShowQuestProgress(activeQuest);
            }
        }
        else
        {
            // Quest already completed
            if (canQuestReset)
            {
                ShowQuestOnCooldown();
            }
            else
            {
                ShowThankYouMessage();
            }
        }
    }

    void OfferQuest()
    {
        if (!Game.Quests.QuestManager.Instance.CanAcceptQuest())
        {
            string[] fullDialogue = new string[]
            {
                "You have too many quests already!",
                "Come back when you've completed some."
            };
            DialogueManager.Instance.StartDialogue(npcName, fullDialogue);
            return;
        }

        string[] questDialogue = new string[]
        {
            $"I have a task for you: {npcQuest.questTitle}",
            npcQuest.questDescription,
            "Will you help me?"
        };

        DialogueManager.Instance.StartDialogue(npcName, questDialogue);

        // Set quest giver name and add quest
        npcQuest.questGiverName = npcName;
        Game.Quests.QuestManager.Instance.AddQuest(npcQuest);
    }

    void ShowQuestProgress(Quest quest)
    {
        Game.Quests.QuestManager.Instance.UpdateQuestProgress(quest);

        string progress = "How's the quest going?\n\n";
        foreach (QuestRequirement req in quest.requirements)
        {
            progress += $"{req.itemName}: {req.currentAmount}/{req.requiredAmount}\n";
        }

        string[] dialogues = new string[]
        {
            progress,
            "Keep going! Come back when you have everything."
        };

        DialogueManager.Instance.StartDialogue(npcName, dialogues);
    }

    void SubmitQuest(Quest quest)
    {
        string rewards = "Your reward:\n\n";
        foreach (QuestReward reward in npcQuest.rewards)
        {
            rewards += $"{reward.itemName} x{reward.quantity}\n";
        }

        string[] dialogues = new string[]
        {
            "Excellent! You did it!",
            rewards,
            "Thank you so much for your help!"
        };

        DialogueManager.Instance.StartDialogue(npcName, dialogues);

        // Complete the quest and record completion time
        Game.Quests.QuestManager.Instance.CompleteQuest(quest);
        npcQuest.isCompleted = true;
        questCompletionTime = Time.time; // Record when quest was completed
    }

    bool CanResetQuest()
    {
        if (questCompletionTime < 0) return false;
        return (Time.time - questCompletionTime) >= questResetTime;
    }

    void ResetQuest()
    {
        npcQuest.isCompleted = false;
        questCompletionTime = -1f;

        // Reset all quest requirements
        foreach (QuestRequirement req in npcQuest.requirements)
        {
            req.currentAmount = 0;
        }
    }

    void ShowQuestOnCooldown()
    {
        float timeRemaining = questResetTime - (Time.time - questCompletionTime);
        int hoursRemaining = Mathf.FloorToInt(timeRemaining / 3600f);
        int minutesRemaining = Mathf.FloorToInt((timeRemaining % 3600f) / 60f);

        string cooldownMessage;
        if (hoursRemaining > 0)
        {
            cooldownMessage = $"I'll have another task for you in {hoursRemaining} hour(s) and {minutesRemaining} minute(s).";
        }
        else
        {
            cooldownMessage = $"I'll have another task for you in {minutesRemaining} minute(s).";
        }

        string[] dialogues = new string[]
        {
            "Thank you again for your help!",
            cooldownMessage
        };

        DialogueManager.Instance.StartDialogue(npcName, dialogues);
    }

    void ShowThankYouMessage()
    {
        string[] dialogues = new string[]
        {
            "Thank you again for your help!",
            "You're a true friend to our village."
        };

        DialogueManager.Instance.StartDialogue(npcName, dialogues);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}