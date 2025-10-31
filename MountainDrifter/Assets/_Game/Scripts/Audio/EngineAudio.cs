using UnityEngine;

/// <summary>
/// RPM-based engine sound system with realistic audio mixing.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class EngineAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource engineSource;
    [SerializeField] private AudioClip engineIdleClip;
    [SerializeField] private AudioClip engineRevClip;

    [Header("RPM Settings")]
    [SerializeField] private float minRPM = 800f;
    [SerializeField] private float maxRPM = 7000f;
    [SerializeField] private float pitchMultiplier = 0.3f;

    [Header("Volume Settings")]
    [SerializeField] private float idleVolume = 0.3f;
    [SerializeField] private float revVolume = 0.7f;
    [SerializeField] private float volumeLerpSpeed = 5f;

    private DriftCarController carController;
    private TransmissionController transmission;
    private float currentRPM = 0f;
    private float targetPitch = 1f;
    private float targetVolume = 0f;

    private void Awake()
    {
        if (engineSource == null)
            engineSource = GetComponent<AudioSource>();

        engineSource.loop = true;
        engineSource.spatialBlend = 1f; // 3D sound
    }

    private void Start()
    {
        carController = GetComponentInParent<DriftCarController>();
        if (carController != null)
        {
            transmission = carController.GetComponent<TransmissionController>();
        }

        if (engineIdleClip != null)
        {
            engineSource.clip = engineIdleClip;
            engineSource.Play();
        }
    }

    private void Update()
    {
        if (transmission != null)
        {
            currentRPM = transmission.CurrentRPM;
            UpdateEngineSound();
        }
    }

    private void UpdateEngineSound()
    {
        // Calculate normalized RPM (0-1)
        float normalizedRPM = Mathf.InverseLerp(minRPM, maxRPM, currentRPM);

        // Update pitch based on RPM
        targetPitch = 1f + (normalizedRPM * pitchMultiplier);
        engineSource.pitch = Mathf.Lerp(engineSource.pitch, targetPitch, Time.deltaTime * 10f);

        // Blend between idle and rev sounds based on RPM
        if (normalizedRPM > 0.3f && engineRevClip != null)
        {
            // Switch to rev clip at higher RPM
            if (engineSource.clip != engineRevClip)
            {
                engineSource.clip = engineRevClip;
                engineSource.Play();
            }

            targetVolume = Mathf.Lerp(idleVolume, revVolume, normalizedRPM);
        }
        else
        {
            // Use idle clip at low RPM
            if (engineSource.clip != engineIdleClip && engineIdleClip != null)
            {
                engineSource.clip = engineIdleClip;
                engineSource.Play();
            }

            targetVolume = idleVolume * (1f + normalizedRPM);
        }

        // Smooth volume transition
        engineSource.volume = Mathf.Lerp(engineSource.volume, targetVolume, Time.deltaTime * volumeLerpSpeed);

        // Add 3D positioning
        if (carController != null)
        {
            transform.position = carController.transform.position;
        }
    }

    public void SetVolume(float volume)
    {
        idleVolume = volume;
        revVolume = volume * 1.5f;
    }
}

