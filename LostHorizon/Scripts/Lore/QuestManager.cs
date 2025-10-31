using System.Collections.Generic;
using UnityEngine;

namespace LostHorizon.Lore
{
    /// <summary>
    /// Manages quests and lore progression
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quests")]
        [SerializeField] private List<Quest> allQuests = new List<Quest>();
        [SerializeField] private List<Quest> activeQuests = new List<Quest>();

        // Events
        public System.Action<Quest> OnQuestStarted;
        public System.Action<Quest> OnQuestCompleted;
        public System.Action<Quest, QuestObjective> OnObjectiveCompleted;

        public void Initialize()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Start a quest
        /// </summary>
        public bool StartQuest(Quest quest)
        {
            if (quest == null)
            {
                return false;
            }

            // Check prerequisites
            foreach (var prereq in quest.prerequisiteQuests)
            {
                if (prereq == null || prereq.status != QuestStatus.Completed)
                {
                    Debug.Log($"Cannot start {quest.questName}: Prerequisites not met");
                    return false;
                }
            }

            quest.status = QuestStatus.InProgress;
            if (!activeQuests.Contains(quest))
            {
                activeQuests.Add(quest);
            }

            OnQuestStarted?.Invoke(quest);
            Debug.Log($"Quest started: {quest.questName}");
            return true;
        }

        /// <summary>
        /// Update quest objective progress
        /// </summary>
        public void UpdateObjective(Quest quest, QuestObjective objective, int amount)
        {
            if (quest == null || objective == null)
            {
                return;
            }

            if (quest.status != QuestStatus.InProgress)
            {
                return;
            }

            objective.currentAmount = Mathf.Min(objective.currentAmount + amount, objective.targetAmount);
            
            if (objective.currentAmount >= objective.targetAmount && !objective.isCompleted)
            {
                objective.isCompleted = true;
                OnObjectiveCompleted?.Invoke(quest, objective);

                // Check if quest is complete
                if (quest.IsCompleted())
                {
                    CompleteQuest(quest);
                }
            }
        }

        /// <summary>
        /// Complete a quest
        /// </summary>
        private void CompleteQuest(Quest quest)
        {
            quest.status = QuestStatus.Completed;

            // Give rewards
            foreach (var reward in quest.rewards)
            {
                GiveReward(reward);
            }

            activeQuests.Remove(quest);
            OnQuestCompleted?.Invoke(quest);
            Debug.Log($"Quest completed: {quest.questName}");
        }

        private void GiveReward(QuestReward reward)
        {
            var playerInventory = FindObjectOfType<Player.PlayerInventory>();
            
            switch (reward.rewardType)
            {
                case RewardType.Resources:
                    if (playerInventory != null)
                    {
                        playerInventory.AddResource(reward.resourceType, reward.resourceAmount);
                    }
                    break;

                case RewardType.Equipment:
                    // Would add to equipment inventory
                    break;

                case RewardType.Experience:
                    // Would add experience points
                    break;
            }
        }

        /// <summary>
        /// Get all active quests
        /// </summary>
        public List<Quest> GetActiveQuests()
        {
            return new List<Quest>(activeQuests);
        }

        /// <summary>
        /// Get all completed quests
        /// </summary>
        public List<Quest> GetCompletedQuests()
        {
            List<Quest> completed = new List<Quest>();
            foreach (var quest in allQuests)
            {
                if (quest.status == QuestStatus.Completed)
                {
                    completed.Add(quest);
                }
            }
            return completed;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}

