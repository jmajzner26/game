using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ShadowDuel.Progression
{
    /// <summary>
    /// Defines a skill in the skill tree
    /// </summary>
    [System.Serializable]
    public class SkillNode
    {
        public string skillName;
        public string description;
        public int cost;
        public bool isUnlocked = false;
        public List<int> prerequisites = new List<int>();

        public SkillNode(string name, string desc, int cost)
        {
            this.skillName = name;
            this.description = desc;
            this.cost = cost;
        }
    }

    /// <summary>
    /// Manages skill tree progression
    /// </summary>
    public class SkillTree : MonoBehaviour
    {
        [Header("Skill Nodes")]
        [SerializeField] private List<SkillNode> skills = new List<SkillNode>();

        [Header("UI")]
        [SerializeField] private GameObject skillTreeUI;
        [SerializeField] private Transform skillButtonParent;
        [SerializeField] private GameObject skillButtonPrefab;

        private ProfileManager profileManager;

        private void Awake()
        {
            profileManager = ProfileManager.Instance;
            
            if (skills.Count == 0)
            {
                InitializeDefaultSkills();
            }
        }

        private void InitializeDefaultSkills()
        {
            // Combat skills
            skills.Add(new SkillNode("Improved Parry", "Increases parry window by 20%", 2));
            skills.Add(new SkillNode("Perfect Parry Master", "Perfect parries restore more stamina", 3));
            skills[1].prerequisites.Add(0);

            // Stamina skills
            skills.Add(new SkillNode("Endurance", "Increase max stamina by 25", 2));
            skills.Add(new SkillNode("Rapid Recovery", "Stamina regenerates 30% faster", 3));
            skills[3].prerequisites.Add(2);

            // Damage skills
            skills.Add(new SkillNode("Brutal Strikes", "Increase damage by 15%", 2));
            skills.Add(new SkillNode("Critical Power", "Critical hits deal 50% more damage", 4));
            skills[5].prerequisites.Add(4);

            // Mobility skills
            skills.Add(new SkillNode("Shadow Dash", "Unlock shadow dash ability", 3));
            skills.Add(new SkillNode("Improved Dash", "Dash distance increased by 50%", 2));
            skills[7].prerequisites.Add(6);

            // Special skills
            skills.Add(new SkillNode("Finisher Master", "Finishers execute 50% faster", 3));
            skills.Add(new SkillNode("Parry Disarm", "Perfect parries can disarm enemies", 5));
            skills[9].prerequisites.Add(1);
        }

        /// <summary>
        /// Try to unlock a skill
        /// </summary>
        public bool TryUnlockSkill(int skillIndex)
        {
            if (skillIndex < 0 || skillIndex >= skills.Count)
                return false;

            SkillNode skill = skills[skillIndex];

            // Check if already unlocked
            if (skill.isUnlocked)
                return false;

            // Check prerequisites
            if (!ArePrerequisitesMet(skill))
                return false;

            // Check if enough skill points
            if (profileManager && profileManager.Profile.skillPoints < skill.cost)
                return false;

            // Unlock skill
            skill.isUnlocked = true;

            if (profileManager)
            {
                profileManager.Profile.skillPoints -= skill.cost;
            }

            ApplySkillEffect(skill);
            return true;
        }

        /// <summary>
        /// Check if all prerequisites are met
        /// </summary>
        private bool ArePrerequisitesMet(SkillNode skill)
        {
            foreach (int prereq in skill.prerequisites)
            {
                if (prereq < skills.Count && !skills[prereq].isUnlocked)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Apply skill effects
        /// </summary>
        private void ApplySkillEffect(SkillNode skill)
        {
            // Find the player and apply skill effects
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (!player) return;

            var staminaManager = player.GetComponent<StaminaManager>();
            var parrySystem = player.GetComponent<ParrySystem>();

            switch (skill.skillName)
            {
                case "Endurance":
                    if (staminaManager)
                    {
                        var field = typeof(StaminaManager).GetField("maxStamina",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (field != null)
                        {
                            float currentMax = (float)field.GetValue(staminaManager);
                            field.SetValue(staminaManager, currentMax + 25f);
                        }
                    }
                    break;

                case "Rapid Recovery":
                    if (staminaManager)
                    {
                        var field = typeof(StaminaManager).GetField("staminaRegenRate",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (field != null)
                        {
                            float currentRate = (float)field.GetValue(staminaManager);
                            field.SetValue(staminaManager, currentRate * 1.3f);
                        }
                    }
                    break;

                case "Shadow Dash":
                    if (profileManager)
                    {
                        profileManager.Profile.UnlockAbility("shadowdash");
                    }
                    break;
            }
        }

        /// <summary>
        /// Get all available skills
        /// </summary>
        public List<SkillNode> GetSkills() => skills;

        /// <summary>
        /// Check if a skill is unlocked
        /// </summary>
        public bool IsSkillUnlocked(int index)
        {
            if (index < 0 || index >= skills.Count)
                return false;
            return skills[index].isUnlocked;
        }
    }
}

