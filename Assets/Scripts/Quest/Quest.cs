using UnityEngine;

[System.Serializable]
public class Quest
{
    [Header("Quest Identity")]
    public string questID;
    public string questTitle;

    [TextArea(2, 4)]
    public string questDescription;

    [Header("Requirements")]
    public QuestRequirement[] requirements;

    [Header("Rewards")]
    public QuestReward[] rewards;

    [Header("Time Constraint")]
    public bool hasTimeLimit = false;
    public float timeLimitInSeconds = 300f; // Default 5 minutes

    [Header("State (Runtime)")]
    public bool isActive;
    public bool isCompleted;
    public bool isFailed;
    public float timeRemaining;
    public string questGiverName;

    public Quest()
    {
        requirements = new QuestRequirement[1];
        rewards = new QuestReward[1];
        isActive = false;
        isCompleted = false;
        isFailed = false;
        hasTimeLimit = false;
        timeLimitInSeconds = 300f;
        timeRemaining = 0f;
    }
}

[System.Serializable]
public class QuestRequirement
{
    public string itemName;
    public int requiredAmount;

    [Header("Progress (Runtime)")]
    public int currentAmount;
}

[System.Serializable]
public class QuestReward
{
    public string itemName;
    public int quantity;
}
