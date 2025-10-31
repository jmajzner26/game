using UnityEngine;
using Cinemachine;

/// <summary>
/// Camera shake system for impact effects and cinematic feel.
/// Works with Cinemachine cameras.
/// </summary>
public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float defaultIntensity = 1f;
    [SerializeField] private float defaultDuration = 0.3f;
    [SerializeField] private float defaultFrequency = 1f;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer = 0f;
    private float currentIntensity = 0f;

    private void Awake()
    {
        // Get noise component from Cinemachine camera
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise == null)
            {
                noise = vcam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                noise.m_NoiseProfile = Resources.Load<NoiseSettings>("Noise/DefaultNoise");
            }
        }
    }

    private void Update()
    {
        if (noise == null) return;

        // Update shake timer
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            // Decrease intensity over time
            currentIntensity = Mathf.Lerp(currentIntensity, 0f, Time.deltaTime * 5f);
            noise.m_AmplitudeGain = currentIntensity;

            if (shakeTimer <= 0f)
            {
                currentIntensity = 0f;
                noise.m_AmplitudeGain = 0f;
            }
        }
    }

    public void AddShake(float intensity, float frequency, float duration = -1f)
    {
        if (noise == null) return;

        // If new shake is stronger, update
        if (intensity > currentIntensity)
        {
            currentIntensity = intensity;
            noise.m_AmplitudeGain = currentIntensity;
        }

        if (frequency > 0f)
        {
            noise.m_FrequencyGain = frequency;
        }

        shakeTimer = duration > 0f ? duration : defaultDuration;
    }

    public void Shake(float intensity = -1f, float duration = -1f, float frequency = -1f)
    {
        AddShake(
            intensity > 0f ? intensity : defaultIntensity,
            frequency > 0f ? frequency : defaultFrequency,
            duration > 0f ? duration : defaultDuration
        );
    }

    public void StopShake()
    {
        shakeTimer = 0f;
        currentIntensity = 0f;
        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
        }
    }
}

