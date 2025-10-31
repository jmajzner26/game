using System;
using System.IO;
using UnityEngine;

namespace LostHorizon.Core
{
    /// <summary>
    /// Handles saving and loading game data
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string saveFileName = "losthorizon_save.json";
        [SerializeField] private bool autoSaveEnabled = true;
        [SerializeField] private float autoSaveInterval = 300f; // 5 minutes

        private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);
        private float lastAutoSaveTime;

        private void Update()
        {
            if (autoSaveEnabled && Time.time - lastAutoSaveTime >= autoSaveInterval)
            {
                SaveGame();
                lastAutoSaveTime = Time.time;
            }
        }

        public void SaveGame()
        {
            try
            {
                GameSaveData saveData = new GameSaveData
                {
                    // Player data
                    playerPosition = GameManager.Instance != null && 
                                   GameManager.Instance.GetComponent<Player.PlayerController>() != null 
                                   ? GameManager.Instance.GetComponent<Player.PlayerController>().transform.position 
                                   : Vector3.zero,
                    
                    // World data
                    worldTime = World.WorldManager.Instance != null 
                               ? World.WorldManager.Instance.GetCurrentTime() 
                               : 0f,
                    
                    // Inventory data would be saved here
                    // Building data would be saved here
                    // Quest progress would be saved here
                    
                    saveTimestamp = DateTime.Now.ToBinary()
                };

                string jsonData = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SavePath, jsonData);
                
                Debug.Log($"Game saved successfully to: {SavePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        public void LoadGame()
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    Debug.LogWarning("No save file found!");
                    return;
                }

                string jsonData = File.ReadAllText(SavePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);

                // Load player position
                if (GameManager.Instance != null)
                {
                    var playerController = GameManager.Instance.GetComponent<Player.PlayerController>();
                    if (playerController != null)
                    {
                        playerController.transform.position = saveData.playerPosition;
                    }
                }

                // Load world time
                if (World.WorldManager.Instance != null)
                {
                    World.WorldManager.Instance.SetTime(saveData.worldTime);
                }

                // Load inventory, buildings, quests, etc.

                Debug.Log("Game loaded successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
            }
        }

        public bool SaveFileExists()
        {
            return File.Exists(SavePath);
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save file deleted.");
            }
        }
    }

    [Serializable]
    public class GameSaveData
    {
        public Vector3 playerPosition;
        public float worldTime;
        public long saveTimestamp;
        // Add more save data fields as needed
        // public InventoryData inventory;
        // public BuildingData[] buildings;
        // public QuestData[] quests;
    }
}

