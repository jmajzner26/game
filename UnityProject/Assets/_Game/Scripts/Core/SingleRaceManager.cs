using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SingleRaceManager : MonoBehaviour
{
    [Header("Race Settings")]
    [SerializeField] private TrackConfig trackConfig;
    [SerializeField] private int numberOfLaps = 3;
    
    [Header("Countdown")]
    [SerializeField] private GameObject countdownUI;
    [SerializeField] private UnityEngine.UI.Text countdownText;
    
    [Header("Race State")]
    [SerializeField] private bool raceStarted = false;
    [SerializeField] private bool raceFinished = false;
    
    private VehicleController playerVehicle;
    private LapCounter playerLapCounter;
    private List<GameObject> allRacers = new List<GameObject>();
    private Dictionary<GameObject, int> racerPositions = new Dictionary<GameObject, int>();
    
    private void Start()
    {
        InitializeRace();
        StartCoroutine(CountdownSequence());
    }
    
    private void InitializeRace()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerVehicle = player.GetComponent<VehicleController>();
            playerLapCounter = player.GetComponent<LapCounter>();
            
            if (playerLapCounter != null && trackConfig != null)
            {
                playerLapCounter.Initialize(trackConfig);
                playerLapCounter.OnRaceComplete += OnRaceComplete;
            }
            
            allRacers.Add(player);
        }
        
        // Find AI cars
        AICarSpawner aiSpawner = FindObjectOfType<AICarSpawner>();
        if (aiSpawner != null)
        {
            List<GameObject> aiCars = aiSpawner.GetAICars();
            allRacers.AddRange(aiCars);
        }
        
        // Initialize positions
        UpdatePositions();
    }
    
    private System.Collections.IEnumerator CountdownSequence()
    {
        if (countdownUI != null) countdownUI.SetActive(true);
        
        if (countdownText != null)
        {
            countdownText.text = "3";
            AudioManager.Instance?.PlayCountdownBeep();
        }
        yield return new WaitForSeconds(1f);
        
        if (countdownText != null)
        {
            countdownText.text = "2";
            AudioManager.Instance?.PlayCountdownBeep();
        }
        yield return new WaitForSeconds(1f);
        
        if (countdownText != null)
        {
            countdownText.text = "1";
            AudioManager.Instance?.PlayCountdownBeep();
        }
        yield return new WaitForSeconds(1f);
        
        if (countdownText != null)
        {
            countdownText.text = "GO!";
            AudioManager.Instance?.PlayRaceStart();
        }
        
        raceStarted = true;
        
        yield return new WaitForSeconds(0.5f);
        
        if (countdownUI != null) countdownUI.SetActive(false);
    }
    
    private void Update()
    {
        if (raceStarted && !raceFinished)
        {
            UpdatePositions();
        }
    }
    
    private void UpdatePositions()
    {
        if (trackConfig == null || allRacers.Count == 0) return;
        
        // Sort racers by lap and checkpoint progress
        var sortedRacers = allRacers
            .Where(r => r != null)
            .OrderByDescending(r => GetLapProgress(r))
            .ThenByDescending(r => GetCheckpointProgress(r))
            .ToList();
        
        for (int i = 0; i < sortedRacers.Count; i++)
        {
            racerPositions[sortedRacers[i]] = i + 1;
        }
        
        // Update HUD
        RaceHUD hud = FindObjectOfType<RaceHUD>();
        if (hud != null && playerVehicle != null)
        {
            int playerPosition = racerPositions.ContainsKey(playerVehicle.gameObject) 
                ? racerPositions[playerVehicle.gameObject] 
                : allRacers.Count;
            hud.SetPosition(playerPosition, allRacers.Count);
        }
    }
    
    private float GetLapProgress(GameObject racer)
    {
        LapCounter lapCounter = racer.GetComponent<LapCounter>();
        if (lapCounter != null)
        {
            return lapCounter.CurrentLap;
        }
        return 0f;
    }
    
    private int GetCheckpointProgress(GameObject racer)
    {
        // This would require tracking checkpoint progress per racer
        // For now, return 0 as placeholder
        return 0;
    }
    
    private void OnRaceComplete()
    {
        raceFinished = true;
        
        // Determine finish position
        int finishPosition = racerPositions.ContainsKey(playerVehicle.gameObject) 
            ? racerPositions[playerVehicle.gameObject] 
            : allRacers.Count;
        
        // Show results screen
        ShowResultsScreen(finishPosition);
        
        // Save to leaderboard
        if (playerLapCounter != null && trackConfig != null)
        {
            LeaderboardManager leaderboard = FindObjectOfType<LeaderboardManager>();
            if (leaderboard != null)
            {
                string playerName = PlayerPrefs.GetString("PlayerName", "Player");
                leaderboard.AddEntry(trackConfig.id, playerName, playerLapCounter.TotalRaceTime);
            }
        }
    }
    
    private void ShowResultsScreen(int position)
    {
        // Placeholder - would show UI with results
        Debug.Log($"Race Complete! Finished in position: {position}");
        
        // Load back to menu after delay
        Invoke(nameof(ReturnToMenu), 5f);
    }
    
    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

