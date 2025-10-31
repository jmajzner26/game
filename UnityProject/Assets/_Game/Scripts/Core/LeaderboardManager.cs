using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float time;
    public string trackId;
    
    public LeaderboardEntry(string name, float time, string track)
    {
        playerName = name;
        this.time = time;
        trackId = track;
    }
}

public class LeaderboardManager : MonoBehaviour
{
    private const string LEADERBOARD_FILE = "leaderboards.json";
    private Dictionary<string, List<LeaderboardEntry>> leaderboards = new Dictionary<string, List<LeaderboardEntry>>();
    
    private void Awake()
    {
        LoadLeaderboards();
    }
    
    public void AddEntry(string trackId, string playerName, float time)
    {
        if (!leaderboards.ContainsKey(trackId))
        {
            leaderboards[trackId] = new List<LeaderboardEntry>();
        }
        
        leaderboards[trackId].Add(new LeaderboardEntry(playerName, time, trackId));
        leaderboards[trackId] = leaderboards[trackId].OrderBy(e => e.time).Take(10).ToList();
        
        SaveLeaderboards();
    }
    
    public List<LeaderboardEntry> GetTopTimes(string trackId, int count = 10)
    {
        if (!leaderboards.ContainsKey(trackId))
        {
            return new List<LeaderboardEntry>();
        }
        
        return leaderboards[trackId].Take(count).ToList();
    }
    
    private void SaveLeaderboards()
    {
        string path = Path.Combine(Application.persistentDataPath, LEADERBOARD_FILE);
        string json = JsonUtility.ToJson(new SerializableDict(leaderboards));
        File.WriteAllText(path, json);
    }
    
    private void LoadLeaderboards()
    {
        string path = Path.Combine(Application.persistentDataPath, LEADERBOARD_FILE);
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SerializableDict data = JsonUtility.FromJson<SerializableDict>(json);
            leaderboards = data.ToDictionary();
        }
    }
    
    [System.Serializable]
    private class SerializableDict
    {
        public List<EntryData> entries = new List<EntryData>();
        
        public SerializableDict(Dictionary<string, List<LeaderboardEntry>> dict)
        {
            foreach (var kvp in dict)
            {
                entries.Add(new EntryData { trackId = kvp.Key, leaderboardEntries = kvp.Value });
            }
        }
        
        public Dictionary<string, List<LeaderboardEntry>> ToDictionary()
        {
            Dictionary<string, List<LeaderboardEntry>> dict = new Dictionary<string, List<LeaderboardEntry>>();
            foreach (var entry in entries)
            {
                dict[entry.trackId] = entry.leaderboardEntries;
            }
            return dict;
        }
    }
    
    [System.Serializable]
    private class EntryData
    {
        public string trackId;
        public List<LeaderboardEntry> leaderboardEntries;
    }
}

