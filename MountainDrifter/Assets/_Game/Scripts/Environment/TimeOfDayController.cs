using UnityEngine;

/// <summary>
/// Controls time of day with dynamic lighting and skybox changes.
/// Supports dawn, day, sunset, and night cycles.
/// </summary>
public class TimeOfDayController : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float timeOfDay = 12f; // 0-24 hours
    [SerializeField] private float timeSpeed = 1f; // Real-time seconds per game hour
    [SerializeField] private bool useRealTimeCycle = true;

    [Header("Sun Light")]
    [SerializeField] private Light sunLight;
    [SerializeField] private Gradient sunColorGradient;
    [SerializeField] private AnimationCurve sunIntensityCurve;

    [Header("Skybox")]
    [SerializeField] private Material[] skyboxMaterials; // Dawn, Day, Sunset, Night

    [Header("Ambient Light")]
    [SerializeField] private Gradient ambientLightGradient;
    [SerializeField] private Color fogColorDawn = new Color(0.8f, 0.7f, 0.6f);
    [SerializeField] private Color fogColorDay = Color.white;
    [SerializeField] private Color fogColorSunset = new Color(1f, 0.7f, 0.5f);
    [SerializeField] private Color fogColorNight = new Color(0.2f, 0.2f, 0.3f);

    private void Start()
    {
        if (sunLight == null)
            sunLight = RenderSettings.sun;

        UpdateTimeOfDay();
    }

    private void Update()
    {
        if (useRealTimeCycle)
        {
            timeOfDay += Time.deltaTime / timeSpeed;
            if (timeOfDay >= 24f)
                timeOfDay -= 24f;

            UpdateTimeOfDay();
        }
    }

    private void UpdateTimeOfDay()
    {
        float normalizedTime = timeOfDay / 24f;

        // Rotate sun
        if (sunLight != null)
        {
            sunLight.transform.rotation = Quaternion.Euler((normalizedTime * 360f) - 90f, 0f, 0f);

            // Update sun color and intensity
            if (sunColorGradient != null)
            {
                sunLight.color = sunColorGradient.Evaluate(normalizedTime);
            }

            if (sunIntensityCurve != null)
            {
                sunLight.intensity = sunIntensityCurve.Evaluate(normalizedTime);
            }
        }

        // Update skybox based on time
        UpdateSkybox(normalizedTime);

        // Update fog color
        UpdateFogColor(normalizedTime);

        // Update ambient light
        if (ambientLightGradient != null)
        {
            RenderSettings.ambientLight = ambientLightGradient.Evaluate(normalizedTime);
        }
    }

    private void UpdateSkybox(float normalizedTime)
    {
        if (skyboxMaterials == null || skyboxMaterials.Length == 0)
            return;

        Material skybox = null;

        // Determine skybox based on time
        if (normalizedTime >= 0.2f && normalizedTime < 0.25f) // Dawn (5-6 AM)
        {
            skybox = skyboxMaterials[0];
        }
        else if (normalizedTime >= 0.25f && normalizedTime < 0.75f) // Day (6 AM - 6 PM)
        {
            skybox = skyboxMaterials[1];
        }
        else if (normalizedTime >= 0.75f && normalizedTime < 0.8f) // Sunset (6-7 PM)
        {
            skybox = skyboxMaterials[2];
        }
        else // Night (7 PM - 5 AM)
        {
            skybox = skyboxMaterials[3];
        }

        if (skybox != null)
        {
            RenderSettings.skybox = skybox;
        }
    }

    private void UpdateFogColor(float normalizedTime)
    {
        Color targetFogColor;

        if (normalizedTime >= 0.2f && normalizedTime < 0.25f) // Dawn
        {
            targetFogColor = fogColorDawn;
        }
        else if (normalizedTime >= 0.25f && normalizedTime < 0.75f) // Day
        {
            targetFogColor = fogColorDay;
        }
        else if (normalizedTime >= 0.75f && normalizedTime < 0.8f) // Sunset
        {
            targetFogColor = fogColorSunset;
        }
        else // Night
        {
            targetFogColor = fogColorNight;
        }

        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetFogColor, Time.deltaTime);
    }

    public void SetTimeOfDay(float hours)
    {
        timeOfDay = Mathf.Clamp(hours, 0f, 24f);
        UpdateTimeOfDay();
    }

    public float GetTimeOfDay()
    {
        return timeOfDay;
    }

    public string GetTimeOfDayString()
    {
        int hours = Mathf.FloorToInt(timeOfDay);
        int minutes = Mathf.FloorToInt((timeOfDay - hours) * 60f);
        return $"{hours:00}:{minutes:00}";
    }
}

