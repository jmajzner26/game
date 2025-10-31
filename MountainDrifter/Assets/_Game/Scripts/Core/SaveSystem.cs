using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Handles saving and loading game data using JSON serialization.
/// Saves progression, car unlocks, and player settings.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "mountain_drifter_save.json";
    [SerializeField] private bool autoSave = true;
    [SerializeField] private float autoSaveInterval = 60f; // seconds

    private string savePath;
    private float autoSaveTimer;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
    }

    private void Update()
    {
        if (autoSave && GameManager.Instance != null)
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                SaveGame();
                autoSaveTimer = 0f;
            }
        }
    }

    public void SaveGame()
    {
        try
        {
            GameSaveData saveData = new GameSaveData();

            // Save progression
            if (GameManager.Instance != null && GameManager.Instance.Progression != null)
            {
                saveData.progression = GameManager.Instance.Progression.GetSaveData();
            }

            // Save car customization data
            // TODO: Add car customization save data

            // Convert to JSON
            string json = JsonUtility.ToJson(saveData, true);

            // Write to file
            File.WriteAllText(savePath, json);

            Debug.Log($"Game saved to: {savePath}");
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
            if (!File.Exists(savePath))
            {
                Debug.Log("No save file found. Starting new game.");
                return;
            }

            // Read from file
            string json = File.ReadAllText(savePath);

            // Parse JSON
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            // Load progression
            if (GameManager.Instance != null && GameManager.Instance.Progression != null)
            {
                GameManager.Instance.Progression.LoadSaveData(saveData.progression);
            }

            Debug.Log("Game loaded successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }

    public void DeleteSave()
    {
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Save file deleted.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete save: {e.Message}");
        }
    }

    public bool SaveFileExists()
    {
        return File.Exists(savePath);
    }
}

[System.Serializable]
public class GameSaveData
{
    public ProgressionData progression;
    // Add more save data classes here as needed
}

