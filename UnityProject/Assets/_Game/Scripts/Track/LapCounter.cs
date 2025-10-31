using UnityEngine;
using System;

public class LapCounter : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private TrackConfig trackConfig;
    
    private int currentLap = 0;
    private int lastCheckpoint = -1;
    private float raceStartTime;
    private float lastLapStartTime;
    private float bestLapTime = float.MaxValue;
    private float lastLapTime = 0f;
    private int targetLaps;
    
    public int CurrentLap => currentLap;
    public float CurrentLapTime => Time.time - lastLapStartTime;
    public float BestLapTime => bestLapTime == float.MaxValue ? 0f : bestLapTime;
    public float LastLapTime => lastLapTime;
    public float TotalRaceTime => Time.time - raceStartTime;
    public bool RaceComplete => currentLap > targetLaps;
    
    public event Action<int> OnLapCompleted;
    public event Action OnRaceComplete;
    
    private void Start()
    {
        if (trackConfig != null)
        {
            targetLaps = trackConfig.laps;
        }
        else
        {
            targetLaps = 3; // Default
        }
        
        raceStartTime = Time.time;
        lastLapStartTime = Time.time;
    }
    
    public void OnCheckpointPassed(int checkpointIndex, bool isFinishLine)
    {
        // Prevent skipping checkpoints
        int expectedNext = (lastCheckpoint + 1) % (trackConfig?.checkpointCount ?? 10);
        
        if (checkpointIndex == expectedNext || isFinishLine)
        {
            lastCheckpoint = checkpointIndex;
            
            if (isFinishLine && checkpointIndex == 0)
            {
                CompleteLap();
            }
        }
    }
    
    private void CompleteLap()
    {
        lastLapTime = CurrentLapTime;
        
        if (lastLapTime < bestLapTime)
        {
            bestLapTime = lastLapTime;
        }
        
        currentLap++;
        lastLapStartTime = Time.time;
        
        OnLapCompleted?.Invoke(currentLap);
        
        if (RaceComplete)
        {
            OnRaceComplete?.Invoke();
        }
    }
    
    public void Initialize(TrackConfig config)
    {
        trackConfig = config;
        targetLaps = config.laps;
        currentLap = 0;
        lastCheckpoint = -1;
        raceStartTime = Time.time;
        lastLapStartTime = Time.time;
        bestLapTime = float.MaxValue;
        lastLapTime = 0f;
    }
}

