using UnityEngine;
using System.Collections;

public class AdaptiveMusic : MonoBehaviour
{
    [Header("Music Tracks")]
    [SerializeField] private AudioClip lowIntensityTrack;
    [SerializeField] private AudioClip mediumIntensityTrack;
    [SerializeField] private AudioClip highIntensityTrack;
    
    [Header("Transition Settings")]
    [SerializeField] private float crossfadeDuration = 2f;
    [SerializeField] private float intensityChangeThreshold = 5f;
    
    [Header("Intensity Calculation")]
    [SerializeField] private float speedWeight = 0.5f;
    [SerializeField] private float lapProgressWeight = 0.3f;
    [SerializeField] private float timeInRaceWeight = 0.2f;
    
    private AudioSource[] audioSources = new AudioSource[2];
    private int activeSourceIndex = 0;
    private VehicleController playerVehicle;
    private LapCounter lapCounter;
    
    private enum IntensityLevel { Low, Medium, High }
    private IntensityLevel currentIntensity = IntensityLevel.Low;
    
    private void Start()
    {
        // Create two audio sources for crossfading
        for (int i = 0; i < audioSources.Length; i++)
        {
            GameObject sourceObj = new GameObject($"MusicSource_{i}");
            sourceObj.transform.SetParent(transform);
            audioSources[i] = sourceObj.AddComponent<AudioSource>();
            audioSources[i].loop = true;
        }
        
        // Find player vehicle
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerVehicle = player.GetComponent<VehicleController>();
            lapCounter = player.GetComponent<LapCounter>();
        }
        
        // Start with low intensity
        PlayTrack(lowIntensityTrack, audioSources[0], false);
    }
    
    private void Update()
    {
        if (playerVehicle == null) return;
        
        // Calculate current intensity
        float intensity = CalculateIntensity();
        IntensityLevel newIntensity = DetermineIntensityLevel(intensity);
        
        if (newIntensity != currentIntensity)
        {
            TransitionToIntensity(newIntensity);
        }
    }
    
    private float CalculateIntensity()
    {
        float intensity = 0f;
        
        // Speed component
        if (playerVehicle != null)
        {
            float speedNormalized = playerVehicle.CurrentSpeed / playerVehicle.Config.maxSpeed;
            intensity += speedNormalized * speedWeight;
        }
        
        // Lap progress component
        if (lapCounter != null)
        {
            float lapProgress = (float)lapCounter.CurrentLap / Mathf.Max(1, 3); // Assuming 3 laps default
            intensity += lapProgress * lapProgressWeight;
        }
        
        // Time in race component
        float timeNormalized = Mathf.Clamp01(Time.time / 300f); // 5 minute race
        intensity += timeNormalized * timeInRaceWeight;
        
        return Mathf.Clamp01(intensity);
    }
    
    private IntensityLevel DetermineIntensityLevel(float intensity)
    {
        if (intensity < 0.33f)
            return IntensityLevel.Low;
        else if (intensity < 0.66f)
            return IntensityLevel.Medium;
        else
            return IntensityLevel.High;
    }
    
    private void TransitionToIntensity(IntensityLevel newIntensity)
    {
        AudioClip targetClip = GetClipForIntensity(newIntensity);
        if (targetClip != null)
        {
            StartCoroutine(CrossfadeToTrack(targetClip));
        }
        currentIntensity = newIntensity;
    }
    
    private AudioClip GetClipForIntensity(IntensityLevel intensity)
    {
        switch (intensity)
        {
            case IntensityLevel.Low:
                return lowIntensityTrack;
            case IntensityLevel.Medium:
                return mediumIntensityTrack;
            case IntensityLevel.High:
                return highIntensityTrack;
            default:
                return lowIntensityTrack;
        }
    }
    
    private IEnumerator CrossfadeToTrack(AudioClip newClip)
    {
        int nextSourceIndex = (activeSourceIndex + 1) % audioSources.Length;
        
        // Start playing new track on next source
        PlayTrack(newClip, audioSources[nextSourceIndex], true);
        
        // Crossfade
        float elapsed = 0f;
        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / crossfadeDuration;
            
            audioSources[activeSourceIndex].volume = 1f - t;
            audioSources[nextSourceIndex].volume = t;
            
            yield return null;
        }
        
        // Stop old source
        audioSources[activeSourceIndex].Stop();
        activeSourceIndex = nextSourceIndex;
    }
    
    private void PlayTrack(AudioClip clip, AudioSource source, bool play)
    {
        if (clip != null && source != null)
        {
            source.clip = clip;
            if (play)
            {
                source.Play();
            }
        }
    }
}

