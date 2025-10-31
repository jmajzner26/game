using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Leaderboard Display")]
    [SerializeField] private Transform leaderboardContent;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Dropdown trackDropdown;
    [SerializeField] private TextMeshProUGUI trackNameText;
    
    [Header("Track Selection")]
    [SerializeField] private TrackConfig[] availableTracks;
    
    private LeaderboardManager leaderboardManager;
    private string currentTrackId = "";
    
    private void Start()
    {
        leaderboardManager = FindObjectOfType<LeaderboardManager>();
        if (leaderboardManager == null)
        {
            GameObject managerObj = new GameObject("LeaderboardManager");
            leaderboardManager = managerObj.AddComponent<LeaderboardManager>();
        }
        
        SetupTrackDropdown();
    }
    
    private void SetupTrackDropdown()
    {
        if (trackDropdown != null && availableTracks != null && availableTracks.Length > 0)
        {
            trackDropdown.ClearOptions();
            var trackNames = new List<string>();
            foreach (var track in availableTracks)
            {
                trackNames.Add(track.displayName);
            }
            trackDropdown.AddOptions(trackNames);
            trackDropdown.onValueChanged.AddListener(OnTrackSelected);
            
            if (availableTracks.Length > 0)
            {
                OnTrackSelected(0);
            }
        }
    }
    
    private void OnTrackSelected(int index)
    {
        if (availableTracks == null || index < 0 || index >= availableTracks.Length) return;
        
        currentTrackId = availableTracks[index].id;
        
        if (trackNameText != null)
        {
            trackNameText.text = availableTracks[index].displayName;
        }
        
        RefreshLeaderboard();
    }
    
    private void RefreshLeaderboard()
    {
        if (leaderboardContent == null || entryPrefab == null || leaderboardManager == null) return;
        
        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
        
        // Get top times
        List<LeaderboardEntry> entries = leaderboardManager.GetTopTimes(currentTrackId, 10);
        
        if (entries.Count == 0)
        {
            GameObject emptyEntry = Instantiate(entryPrefab, leaderboardContent);
            TextMeshProUGUI text = emptyEntry.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = "No times recorded yet!";
            }
            return;
        }
        
        // Create entry UI for each leaderboard entry
        for (int i = 0; i < entries.Count; i++)
        {
            LeaderboardEntry entry = entries[i];
            GameObject entryObj = Instantiate(entryPrefab, leaderboardContent);
            
            // Format: "#1. PlayerName - 1:23.45"
            TextMeshProUGUI[] texts = entryObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0 && texts[0] != null)
            {
                string position = (i + 1).ToString();
                string name = entry.playerName;
                string time = FormatTime(entry.time);
                texts[0].text = $"#{position}. {name} - {time}";
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
    
    public void BackToMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

