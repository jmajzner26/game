using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class GhostData
{
    public List<GhostFrame> frames = new List<GhostFrame>();
    public string trackId;
    public float lapTime;
}

[System.Serializable]
public class GhostFrame
{
    public Vector3 position;
    public Quaternion rotation;
    public float time;
}

public class GhostRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    [SerializeField] private float recordInterval = 0.1f; // Record every 0.1 seconds
    
    private float recordTimer = 0f;
    private List<GhostFrame> recordedFrames = new List<GhostFrame>();
    private string trackId;
    private bool isRecording = false;
    private float startTime = 0f;
    
    public void Initialize(string track)
    {
        trackId = track;
        isRecording = true;
        startTime = Time.time;
        recordedFrames.Clear();
    }
    
    private void Update()
    {
        if (!isRecording) return;
        
        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            RecordFrame();
            recordTimer = 0f;
        }
    }
    
    private void RecordFrame()
    {
        GhostFrame frame = new GhostFrame
        {
            position = transform.position,
            rotation = transform.rotation,
            time = Time.time - startTime
        };
        
        recordedFrames.Add(frame);
    }
    
    public void SaveGhostData()
    {
        if (recordedFrames.Count == 0) return;
        
        GhostData ghostData = new GhostData
        {
            frames = new List<GhostFrame>(recordedFrames),
            trackId = trackId,
            lapTime = Time.time - startTime
        };
        
        string fileName = $"ghost_{trackId}.json";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(ghostData, true);
        
        File.WriteAllText(path, json);
        Debug.Log($"Ghost data saved to {path}");
    }
    
    public void StopRecording()
    {
        isRecording = false;
    }
    
    public void ClearRecording()
    {
        recordedFrames.Clear();
        recordTimer = 0f;
    }
}

