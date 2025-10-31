using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private AudioSource exhaustAudioSource;
    
    [Header("Engine Audio Clips")]
    [SerializeField] private AudioClip engineIdleClip;
    [SerializeField] private AudioClip engineRunningClip;
    
    [Header("Audio Settings")]
    [SerializeField] private float minRPM = 0f;
    [SerializeField] private float maxRPM = 7000f;
    [SerializeField] private float pitchRange = 1.5f;
    [SerializeField] private float volumeMultiplier = 1f;
    
    private VehicleController vehicleController;
    private float currentRPM = 0f;
    
    private void Start()
    {
        vehicleController = GetComponent<VehicleController>();
        
        if (engineAudioSource == null)
        {
            engineAudioSource = gameObject.AddComponent<AudioSource>();
            engineAudioSource.loop = true;
            engineAudioSource.playOnAwake = true;
        }
        
        if (exhaustAudioSource == null)
        {
            exhaustAudioSource = gameObject.AddComponent<AudioSource>();
            exhaustAudioSource.loop = true;
        }
        
        if (engineRunningClip != null)
        {
            engineAudioSource.clip = engineRunningClip;
            engineAudioSource.Play();
        }
    }
    
    private void Update()
    {
        if (vehicleController == null || vehicleController.Config == null) return;
        
        // Calculate RPM based on speed and throttle
        float speed = vehicleController.CurrentSpeed;
        float maxSpeed = vehicleController.Config.maxSpeed;
        float speedRatio = Mathf.Clamp01(speed / maxSpeed);
        
        // RPM increases with speed and throttle input
        float targetRPM = Mathf.Lerp(minRPM, maxRPM, speedRatio);
        currentRPM = Mathf.Lerp(currentRPM, targetRPM, Time.deltaTime * 5f);
        
        // Update audio pitch and volume
        if (engineAudioSource != null && engineAudioSource.clip != null)
        {
            float rpmNormalized = currentRPM / maxRPM;
            engineAudioSource.pitch = Mathf.Lerp(1f, 1f + pitchRange, rpmNormalized);
            engineAudioSource.volume = Mathf.Lerp(0.3f, 1f, rpmNormalized) * volumeMultiplier;
        }
        
        // Exhaust sound (louder when accelerating)
        if (exhaustAudioSource != null)
        {
            float acceleration = Mathf.Abs(vehicleController.CurrentSpeed - (vehicleController.CurrentSpeed - Time.deltaTime * 10f));
            exhaustAudioSource.volume = Mathf.Clamp01(acceleration / 5f) * volumeMultiplier * 0.5f;
        }
    }
}

