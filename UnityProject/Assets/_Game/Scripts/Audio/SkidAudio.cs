using UnityEngine;

public class SkidAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] wheelSkidSources = new AudioSource[4];
    
    [Header("Skid Audio Clip")]
    [SerializeField] private AudioClip skidClip;
    
    [Header("Skid Settings")]
    [SerializeField] private float skidThreshold = 8f; // slip angle in degrees
    [SerializeField] private float maxVolume = 0.8f;
    [SerializeField] private float volumeFadeSpeed = 5f;
    
    private VehicleController vehicleController;
    private float[] wheelSlipAmounts = new float[4];
    private float currentSkidVolume = 0f;
    
    private void Start()
    {
        vehicleController = GetComponent<VehicleController>();
        
        // Create audio sources for each wheel if needed
        for (int i = 0; i < wheelSkidSources.Length; i++)
        {
            if (wheelSkidSources[i] == null)
            {
                GameObject wheelAudioObj = new GameObject($"WheelSkidAudio_{i}");
                wheelAudioObj.transform.SetParent(transform);
                wheelSkidSources[i] = wheelAudioObj.AddComponent<AudioSource>();
                wheelSkidSources[i].clip = skidClip;
                wheelSkidSources[i].loop = true;
                wheelSkidSources[i].playOnAwake = false;
                wheelSkidSources[i].spatialBlend = 1f; // 3D sound
            }
        }
    }
    
    private void Update()
    {
        if (vehicleController == null) return;
        
        bool isDrifting = vehicleController.IsDrifting;
        float targetVolume = 0f;
        
        if (isDrifting)
        {
            // Calculate skid volume based on drift intensity
            float speed = vehicleController.CurrentSpeed;
            float speedNormalized = Mathf.Clamp01(speed / vehicleController.Config.maxSpeed);
            targetVolume = maxVolume * speedNormalized;
        }
        
        // Smooth volume transitions
        currentSkidVolume = Mathf.Lerp(currentSkidVolume, targetVolume, Time.deltaTime * volumeFadeSpeed);
        
        // Apply to all wheel audio sources
        foreach (var source in wheelSkidSources)
        {
            if (source != null)
            {
                source.volume = currentSkidVolume;
                
                if (currentSkidVolume > 0.1f && !source.isPlaying)
                {
                    source.Play();
                }
                else if (currentSkidVolume < 0.1f && source.isPlaying)
                {
                    source.Stop();
                }
            }
        }
    }
}

