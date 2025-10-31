using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages player progression: money, reputation, unlocked cars and tracks.
/// </summary>
public class ProgressionManager : MonoBehaviour
{
    [Header("Starting Values")]
    [SerializeField] private float startingMoney = 5000f;
    [SerializeField] private int startingReputation = 0;

    private float currentMoney;
    private int currentReputation;
    private HashSet<string> unlockedCars = new HashSet<string>();
    private HashSet<string> unlockedTracks = new HashSet<string>();
    private string currentCarID;

    public float CurrentMoney => currentMoney;
    public int CurrentReputation => currentReputation;
    public string CurrentCarID => currentCarID;

    // Events
    public event System.Action<float> OnMoneyChanged;
    public event System.Action<int> OnReputationChanged;
    public event System.Action<string> OnCarUnlocked;
    public event System.Action<string> OnTrackUnlocked;

    private void Awake()
    {
        // Initialize with default car unlocked
        unlockedCars.Add("car_starter");
        currentCarID = "car_starter";
    }

    public void Initialize()
    {
        if (currentMoney == 0f)
        {
            currentMoney = startingMoney;
        }
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public bool SpendMoney(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }
        return false;
    }

    public void AddReputation(int amount)
    {
        currentReputation += amount;
        OnReputationChanged?.Invoke(currentReputation);
        
        // Check for unlocks based on reputation
        CheckReputationUnlocks();
    }

    private void CheckReputationUnlocks()
    {
        // Example unlock thresholds
        if (currentReputation >= 100 && !unlockedTracks.Contains("track_mountain_pass"))
        {
            UnlockTrack("track_mountain_pass");
        }
        if (currentReputation >= 250 && !unlockedTracks.Contains("track_alpine_ridge"))
        {
            UnlockTrack("track_alpine_ridge");
        }
        if (currentReputation >= 500 && !unlockedCars.Contains("car_tuned"))
        {
            UnlockCar("car_tuned");
        }
    }

    public void UnlockCar(string carID)
    {
        if (!unlockedCars.Contains(carID))
        {
            unlockedCars.Add(carID);
            OnCarUnlocked?.Invoke(carID);
        }
    }

    public void UnlockTrack(string trackID)
    {
        if (!unlockedTracks.Contains(trackID))
        {
            unlockedTracks.Add(trackID);
            OnTrackUnlocked?.Invoke(trackID);
        }
    }

    public bool IsCarUnlocked(string carID)
    {
        return unlockedCars.Contains(carID);
    }

    public bool IsTrackUnlocked(string trackID)
    {
        return unlockedTracks.Contains(trackID);
    }

    public void SetCurrentCar(string carID)
    {
        if (unlockedCars.Contains(carID))
        {
            currentCarID = carID;
        }
    }

    public HashSet<string> GetUnlockedCars()
    {
        return new HashSet<string>(unlockedCars);
    }

    public HashSet<string> GetUnlockedTracks()
    {
        return new HashSet<string>(unlockedTracks);
    }

    // Save/Load data
    public ProgressionData GetSaveData()
    {
        return new ProgressionData
        {
            money = currentMoney,
            reputation = currentReputation,
            currentCarID = currentCarID,
            unlockedCars = new List<string>(unlockedCars),
            unlockedTracks = new List<string>(unlockedTracks)
        };
    }

    public void LoadSaveData(ProgressionData data)
    {
        currentMoney = data.money;
        currentReputation = data.reputation;
        currentCarID = data.currentCarID;
        unlockedCars = new HashSet<string>(data.unlockedCars);
        unlockedTracks = new HashSet<string>(data.unlockedTracks);

        OnMoneyChanged?.Invoke(currentMoney);
        OnReputationChanged?.Invoke(currentReputation);
    }
}

[System.Serializable]
public class ProgressionData
{
    public float money;
    public int reputation;
    public string currentCarID;
    public List<string> unlockedCars;
    public List<string> unlockedTracks;
}

