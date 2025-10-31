using System.Collections.Generic;
using UnityEngine;

namespace LostHorizon.Lore
{
    /// <summary>
    /// ScriptableObject defining a quest or objective
    /// </summary>
    [CreateAssetMenu(fileName = "New Quest", menuName = "Lost Horizon/Lore/Quest")]
    public class Quest : ScriptableObject
    {
        [Header("Quest Info")]
        public string questName;
        [TextArea(5, 10)]
        public string description;
        public string questGiverName = "Unknown";

        [Header("Objectives")]
        public List<QuestObjective> objectives = new List<QuestObjective>();

        [Header("Rewards")]
        public List<QuestReward> rewards = new List<QuestReward>();

        [Header("Quest Type")]
        public QuestType questType = QuestType.Main;

        [Header("Prerequisites")]
        public List<Quest> prerequisiteQuests = new List<Quest>();

        [Header("Status")]
        public QuestStatus status = QuestStatus.NotStarted;

        /// <summary>
        /// Check if all objectives are completed
        /// </summary>
        public bool IsCompleted()
        {
            foreach (var objective in objectives)
            {
                if (!objective.isCompleted)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get completion percentage
        /// </summary>
        public float GetCompletionPercent()
        {
            if (objectives.Count == 0) return 1f;

            int completed = 0;
            foreach (var objective in objectives)
            {
                if (objective.isCompleted)
                {
                    completed++;
                }
            }

            return (float)completed / objectives.Count;
        }
    }

    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveText;
        public ObjectiveType objectiveType;
        public int targetAmount = 1;
        public int currentAmount = 0;
        public bool isCompleted = false;

        // For specific target objectives
        public string targetObjectName = "";
        public Resources.ResourceType targetResource = Resources.ResourceType.None;
        public Equipment.EquipmentType targetEquipment = Equipment.EquipmentType.Hand;
    }

    [System.Serializable]
    public class QuestReward
    {
        public RewardType rewardType;
        public Resources.ResourceType resourceType = Resources.ResourceType.None;
        public int resourceAmount = 0;
        public int experiencePoints = 0;
        public Equipment.EquipmentItem equipmentReward;
    }

    public enum QuestType
    {
        Main,
        Side,
        Discovery,
        Collection
    }

    public enum QuestStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Failed
    }

    public enum ObjectiveType
    {
        GatherResource,
        CraftItem,
        BuildStructure,
        DiscoverLocation,
        FindRelic,
        TalkToNPC,
        KillEnemy
    }

    public enum RewardType
    {
        Resources,
        Experience,
        Equipment,
        Recipe
    }
}

