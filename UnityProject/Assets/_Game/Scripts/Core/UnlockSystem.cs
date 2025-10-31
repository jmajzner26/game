using UnityEngine;

public class UnlockSystem : MonoBehaviour
{
    [Header("Unlock Requirements")]
    [SerializeField] private UnlockCondition[] unlockConditions;
    
    private ProfileData profileData;
    
    [System.Serializable]
    public class UnlockCondition
    {
        public string carId;
        public int requiredWins;
        public float requiredBestTime; // On any track
        public string requiredTrackCompletion; // Track ID
    }
    
    private void Start()
    {
        profileData = ProfileData.Instance;
        if (profileData == null)
        {
            GameObject profileObj = new GameObject("ProfileData");
            profileData = profileObj.AddComponent<ProfileData>();
        }
        
        CheckUnlocks();
    }
    
    public void CheckUnlocks()
    {
        if (profileData == null || unlockConditions == null) return;
        
        PlayerProfile profile = profileData.GetProfile();
        
        foreach (var condition in unlockConditions)
        {
            if (profileData.IsCarUnlocked(condition.carId)) continue;
            
            bool unlocked = true;
            
            // Check wins requirement
            if (condition.requiredWins > 0 && profile.totalWins < condition.requiredWins)
            {
                unlocked = false;
            }
            
            // Check time requirement
            if (condition.requiredBestTime > 0)
            {
                // Check all tracks for a time better than required
                bool hasRequiredTime = false;
                foreach (var bestTime in profile.bestTimes)
                {
                    if (bestTime.Value <= condition.requiredBestTime)
                    {
                        hasRequiredTime = true;
                        break;
                    }
                }
                if (!hasRequiredTime) unlocked = false;
            }
            
            // Check track completion
            if (!string.IsNullOrEmpty(condition.requiredTrackCompletion))
            {
                if (!profile.bestTimes.ContainsKey(condition.requiredTrackCompletion))
                {
                    unlocked = false;
                }
            }
            
            if (unlocked)
            {
                profileData.UnlockCar(condition.carId);
                Debug.Log($"Unlocked car: {condition.carId}");
            }
        }
    }
    
    public void OnRaceComplete(string trackId, int position, float time)
    {
        if (profileData == null) return;
        
        PlayerProfile profile = profileData.GetProfile();
        profile.totalRaces++;
        
        if (position == 1)
        {
            profile.totalWins++;
        }
        
        profileData.UpdateBestTime(trackId, time);
        CheckUnlocks();
    }
}

