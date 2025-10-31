using UnityEngine;

/// <summary>
/// Tire skid and drift sound effects based on slip angle and surface type.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TireAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip tireSquealClip;
    [SerializeField] private AudioClip dirtSkidClip;
    [SerializeField] private AudioClip gravelSkidClip;

    [Header("Settings")]
    [SerializeField] private float skidThreshold = 5f; // Slip angle threshold
    [SerializeField] private float maxVolume = 0.8f;
    [SerializeField] private float pitchRange = 0.5f;

    private AudioSource audioSource;
    private DriftCarController carController;
    private float currentSlipAngle = 0f;
    private bool isDrifting = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.spatialBlend = 1f; // 3D sound
    }

    private void Start()
    {
        carController = GetComponentInParent<DriftCarController>();
        
        if (carController != null)
        {
            carController.OnDriftStateChanged += OnDriftStateChanged;
        }

        if (tireSquealClip != null)
        {
            audioSource.clip = tireSquealClip;
        }
    }

    private void Update()
    {
        if (carController != null)
        {
            currentSlipAngle = Mathf.Abs(carController.SlipAngle);
            UpdateTireSound();
        }
    }

    private void UpdateTireSound()
    {
        if (!isDrifting || currentSlipAngle < skidThreshold)
        {
            audioSource.volume = 0f;
            return;
        }

        // Calculate volume based on slip angle
        float normalizedAngle = Mathf.Clamp01((currentSlipAngle - skidThreshold) / 30f);
        audioSource.volume = normalizedAngle * maxVolume;

        // Adjust pitch based on speed
        if (carController != null)
        {
            float speedFactor = carController.CurrentSpeed / 150f; // Normalize to ~150 km/h
            audioSource.pitch = 1f + (speedFactor * pitchRange);
        }

        // Play sound if not already playing
        if (!audioSource.isPlaying && audioSource.volume > 0.1f)
        {
            audioSource.Play();
        }
    }

    private void OnDriftStateChanged(bool drifting)
    {
        isDrifting = drifting;

        if (!drifting)
        {
            audioSource.volume = 0f;
            audioSource.Stop();
        }
    }

    public void SetSurfaceType(SurfaceZone.SurfaceType surfaceType)
    {
        // Switch audio clip based on surface
        AudioClip clipToUse = tireSquealClip;

        switch (surfaceType)
        {
            case SurfaceZone.SurfaceType.Dirt:
                clipToUse = dirtSkidClip != null ? dirtSkidClip : tireSquealClip;
                break;
            case SurfaceZone.SurfaceType.Gravel:
                clipToUse = gravelSkidClip != null ? gravelSkidClip : tireSquealClip;
                break;
            default:
                clipToUse = tireSquealClip;
                break;
        }

        if (clipToUse != null && audioSource.clip != clipToUse)
        {
            audioSource.clip = clipToUse;
            if (audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnDestroy()
    {
        if (carController != null)
        {
            carController.OnDriftStateChanged -= OnDriftStateChanged;
        }
    }
}

