using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class PlayerProfile
{
    public string playerName = "Player";
    public int totalRaces = 0;
    public int totalWins = 0;
    public float totalPlayTime = 0f;
    public List<string> unlockedCars = new List<string>();
    public Dictionary<string, float> bestTimes = new Dictionary<string, float>();
    
    public PlayerProfile()
    {
        // Default unlocked car
        unlockedCars.Add("car_default");
    }
}

public class ProfileData : MonoBehaviour
{
    private const string PROFILE_FILE = "player_profile.json";
    private PlayerProfile currentProfile;
    
    public static ProfileData Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProfile();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadProfile()
    {
        string path = Path.Combine(Application.persistentDataPath, PROFILE_FILE);
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentProfile = JsonUtility.FromJson<PlayerProfile>(json);
        }
        else
        {
            currentProfile = new PlayerProfile();
            SaveProfile();
        }
    }
    
    public void SaveProfile()
    {
        string path = Path.Combine(Application.persistentDataPath, PROFILE_FILE);
        string json = JsonUtility.ToJson(currentProfile, true);
        File.WriteAllText(path, json);
    }
    
    public PlayerProfile GetProfile()
    {
        return currentProfile;
    }
    
    public void SetPlayerName(string name)
    {
        currentProfile.playerName = name;
        SaveProfile();
    }
    
    public void UnlockCar(string carId)
    {
        if (!currentProfile.unlockedCars.Contains(carId))
        {
            currentProfile.unlockedCars.Add(carId);
            SaveProfile();
        }
    }
    
    public bool IsCarUnlocked(string carId)
    {
        return currentProfile.unlockedCars.Contains(carId);
    }
    
    public void UpdateBestTime(string trackId, float time)
    {
        if (!currentProfile.bestTimes.ContainsKey(trackId) || time < currentProfile.bestTimes[trackId])
        {
            currentProfile.bestTimes[trackId] = time;
            SaveProfile();
        }
    }
    
    public float GetBestTime(string trackId)
    {
        return currentProfile.bestTimes.ContainsKey(trackId) ? currentProfile.bestTimes[trackId] : float.MaxValue;
    }
}

