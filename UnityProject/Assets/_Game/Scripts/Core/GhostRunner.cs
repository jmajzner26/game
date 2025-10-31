using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GhostRunner : MonoBehaviour
{
    [Header("Ghost Display")]
    [SerializeField] private GameObject ghostCarPrefab;
    [SerializeField] private Material ghostMaterial;
    
    private GameObject ghostCarInstance;
    private GhostData ghostData;
    private int currentFrameIndex = 0;
    private float playbackTimer = 0f;
    private bool isPlaying = false;
    
    public void LoadGhost(string trackId)
    {
        string fileName = $"ghost_{trackId}.json";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ghostData = JsonUtility.FromJson<GhostData>(json);
            
            if (ghostData != null && ghostData.frames.Count > 0)
            {
                CreateGhostCar();
                StartPlayback();
            }
        }
        else
        {
            Debug.Log($"No ghost data found for track: {trackId}");
        }
    }
    
    private void CreateGhostCar()
    {
        if (ghostCarPrefab == null) return;
        
        ghostCarInstance = Instantiate(ghostCarPrefab);
        ghostCarInstance.name = "GhostCar";
        
        // Make it semi-transparent
        Renderer[] renderers = ghostCarInstance.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (ghostMaterial != null)
            {
                renderer.material = ghostMaterial;
            }
            
            // Set transparency
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    color.a = 0.5f;
                    mat.color = color;
                }
            }
        }
        
        // Disable collisions
        Collider[] colliders = ghostCarInstance.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.isTrigger = true;
        }
    }
    
    private void StartPlayback()
    {
        if (ghostData == null || ghostData.frames.Count == 0) return;
        
        isPlaying = true;
        playbackTimer = 0f;
        currentFrameIndex = 0;
    }
    
    private void Update()
    {
        if (!isPlaying || ghostData == null || ghostCarInstance == null) return;
        
        playbackTimer += Time.deltaTime;
        
        // Find the appropriate frame
        while (currentFrameIndex < ghostData.frames.Count - 1 && 
               ghostData.frames[currentFrameIndex + 1].time <= playbackTimer)
        {
            currentFrameIndex++;
        }
        
        // Interpolate between frames
        if (currentFrameIndex < ghostData.frames.Count - 1)
        {
            GhostFrame currentFrame = ghostData.frames[currentFrameIndex];
            GhostFrame nextFrame = ghostData.frames[currentFrameIndex + 1];
            
            float frameDelta = nextFrame.time - currentFrame.time;
            float t = frameDelta > 0 ? (playbackTimer - currentFrame.time) / frameDelta : 0f;
            
            ghostCarInstance.transform.position = Vector3.Lerp(currentFrame.position, nextFrame.position, t);
            ghostCarInstance.transform.rotation = Quaternion.Lerp(currentFrame.rotation, nextFrame.rotation, t);
        }
        else if (currentFrameIndex < ghostData.frames.Count)
        {
            // Final frame
            GhostFrame frame = ghostData.frames[currentFrameIndex];
            ghostCarInstance.transform.position = frame.position;
            ghostCarInstance.transform.rotation = frame.rotation;
        }
        
        // Check if playback finished
        if (playbackTimer >= ghostData.lapTime)
        {
            StopPlayback();
        }
    }
    
    public void StopPlayback()
    {
        isPlaying = false;
    }
    
    public void ResetPlayback()
    {
        playbackTimer = 0f;
        currentFrameIndex = 0;
        isPlaying = true;
    }
}

