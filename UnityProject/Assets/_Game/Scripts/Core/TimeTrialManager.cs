using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTrialManager : MonoBehaviour
{
    [Header("Track Configuration")]
    [SerializeField] private TrackConfig trackConfig;
    
    [Header("UI")]
    [SerializeField] private RaceHUD raceHUD;
    
    [Header("Ghost System")]
    [SerializeField] private bool loadBestLapGhost = true;
    
    private VehicleController playerVehicle;
    private LapCounter lapCounter;
    private GhostRecorder ghostRecorder;
    private GhostRunner ghostRunner;
    
    private void Start()
    {
        InitializeTimeTrial();
    }
    
    private void InitializeTimeTrial()
    {
        // Find player vehicle
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerVehicle = player.GetComponent<VehicleController>();
            lapCounter = player.GetComponent<LapCounter>();
            
            if (lapCounter != null && trackConfig != null)
            {
                lapCounter.Initialize(trackConfig);
                lapCounter.OnLapCompleted += OnLapCompleted;
            }
            
            // Setup ghost recorder
            ghostRecorder = player.GetComponent<GhostRecorder>();
            if (ghostRecorder == null)
            {
                ghostRecorder = player.AddComponent<GhostRecorder>();
            }
            
            if (ghostRecorder != null && trackConfig != null)
            {
                ghostRecorder.Initialize(trackConfig.id);
            }
            
            // Load and run best lap ghost if available
            if (loadBestLapGhost)
            {
                ghostRunner = FindObjectOfType<GhostRunner>();
                if (ghostRunner != null)
                {
                    ghostRunner.LoadGhost(trackConfig.id);
                }
            }
        }
    }
    
    private void OnLapCompleted(int lapNumber)
    {
        if (lapCounter == null) return;
        
        float lapTime = lapCounter.LastLapTime;
        float bestTime = lapCounter.BestLapTime;
        
        // Check if this is a new best time
        if (Mathf.Approximately(lapTime, bestTime))
        {
            Debug.Log($"New Best Lap! Time: {FormatTime(lapTime)}");
            
            // Save ghost data
            if (ghostRecorder != null)
            {
                ghostRecorder.SaveGhostData();
            }
            
            // Save to leaderboard
            LeaderboardManager leaderboard = FindObjectOfType<LeaderboardManager>();
            if (leaderboard != null && trackConfig != null)
            {
                string playerName = PlayerPrefs.GetString("PlayerName", "Player");
                leaderboard.AddEntry(trackConfig.id, playerName, bestTime);
            }
        }
    }
    
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        int ms = Mathf.FloorToInt((seconds % 1f) * 100f);
        return $"{minutes}:{secs:00}.{ms:00}";
    }
}

