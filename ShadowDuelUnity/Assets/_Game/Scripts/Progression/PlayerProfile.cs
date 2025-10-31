using UnityEngine;
using System;

namespace ShadowDuel.Progression
{
    /// <summary>
    /// Stores and manages player progression data
    /// </summary>
    [Serializable]
    public class PlayerProfile
    {
        [Header("Progression")]
        public int playerLevel = 1;
        public int experiencePoints = 0;
        public int skillPoints = 0;

        [Header("Stats")]
        public float maxHealth = 100f;
        public float maxStamina = 100f;
        public float baseDamage = 1f;
        public float defenseRating = 0f;

        [Header("Unlocks")]
        public bool katanaUnlocked = true;
        public bool twinBladesUnlocked = false;
        public bool claymoreUnlocked = false;

        public bool shadowDashUnlocked = false;
        public bool disarmUnlocked = false;
        public bool criticalStrikeUnlocked = false;

        [Header("Customization")]
        public int selectedArmor = 0;
        public int selectedMaterial = 0;
        public Color armorColor = Color.white;

        [Header("Combat Stats")]
        public int totalDuelsWon = 0;
        public int totalDuelsLost = 0;
        public int perfectParries = 0;
        public int finishersExecuted = 0;

        /// <summary>
        /// Calculate experience required for next level
        /// </summary>
        public int GetExpForNextLevel()
        {
            return 1000 + (playerLevel * 500);
        }

        /// <summary>
        /// Add experience points
        /// </summary>
        public void AddExperience(int exp)
        {
            experiencePoints += exp;
            CheckLevelUp();
        }

        /// <summary>
        /// Check if player levels up
        /// </summary>
        private void CheckLevelUp()
        {
            int expNeeded = GetExpForNextLevel();

            while (experiencePoints >= expNeeded)
            {
                experiencePoints -= expNeeded;
                playerLevel++;
                skillPoints += 3;
                OnLevelUp();
                expNeeded = GetExpForNextLevel();
            }
        }

        private void OnLevelUp()
        {
            // Scale stats with level
            maxHealth += 10f;
            maxStamina += 5f;
        }

        /// <summary>
        /// Unlock a weapon
        /// </summary>
        public void UnlockWeapon(string weaponName)
        {
            switch (weaponName.ToLower())
            {
                case "katana":
                    katanaUnlocked = true;
                    break;
                case "twinblades":
                    twinBladesUnlocked = true;
                    break;
                case "claymore":
                    claymoreUnlocked = true;
                    break;
            }
        }

        /// <summary>
        /// Unlock a special ability
        /// </summary>
        public void UnlockAbility(string abilityName)
        {
            switch (abilityName.ToLower())
            {
                case "shadowdash":
                    shadowDashUnlocked = true;
                    break;
                case "disarm":
                    disarmUnlocked = true;
                    break;
                case "criticalstrike":
                    criticalStrikeUnlocked = true;
                    break;
            }
        }
    }

    /// <summary>
    /// Manages player profile loading and saving
    /// </summary>
    public class ProfileManager : MonoBehaviour
    {
        private static ProfileManager instance;
        public static ProfileManager Instance => instance;

        [SerializeField] private PlayerProfile currentProfile = new PlayerProfile();
        private string saveFileName = "playerprofile.json";

        public PlayerProfile Profile => currentProfile;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                LoadProfile();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Save profile to disk
        /// </summary>
        public void SaveProfile()
        {
            string json = JsonUtility.ToJson(currentProfile, true);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
        }

        /// <summary>
        /// Load profile from disk
        /// </summary>
        public void LoadProfile()
        {
            string path = Application.persistentDataPath + "/" + saveFileName;

            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                currentProfile = JsonUtility.FromJson<PlayerProfile>(json);
            }
            else
            {
                currentProfile = new PlayerProfile();
            }
        }

        /// <summary>
        /// Reset profile to defaults
        /// </summary>
        public void ResetProfile()
        {
            currentProfile = new PlayerProfile();
            SaveProfile();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveProfile();
            }
        }

        private void OnApplicationQuit()
        {
            SaveProfile();
        }
    }
}

