using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnvController : MonoBehaviour
{
    [Header("Time of Day")]
    [SerializeField] private TrackConfig.TimeOfDay timeOfDay = TrackConfig.TimeOfDay.Day;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Gradient sunColorGradient;
    
    [Header("Skybox")]
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material duskSkybox;
    [SerializeField] private Material nightSkybox;
    
    [Header("Fog")]
    [SerializeField] private bool enableFog = false;
    [SerializeField] private Color fogColor = Color.gray;
    [SerializeField] private float fogDensity = 0.01f;
    
    [Header("Post-Processing")]
    [SerializeField] private Volume postProcessVolume;
    
    private void Start()
    {
        ApplyTimeOfDay(timeOfDay);
    }
    
    public void ApplyTimeOfDay(TrackConfig.TimeOfDay tod)
    {
        timeOfDay = tod;
        
        switch (tod)
        {
            case TrackConfig.TimeOfDay.Day:
                SetDaySettings();
                break;
            case TrackConfig.TimeOfDay.Dusk:
                SetDuskSettings();
                break;
            case TrackConfig.TimeOfDay.Night:
                SetNightSettings();
                break;
        }
    }
    
    private void SetDaySettings()
    {
        if (directionalLight != null)
        {
            directionalLight.color = Color.white;
            directionalLight.intensity = 1f;
            directionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
        
        RenderSettings.skybox = daySkybox;
        RenderSettings.ambientSkyColor = new Color(0.5f, 0.5f, 0.6f);
    }
    
    private void SetDuskSettings()
    {
        if (directionalLight != null)
        {
            directionalLight.color = new Color(1f, 0.7f, 0.5f);
            directionalLight.intensity = 0.8f;
            directionalLight.transform.rotation = Quaternion.Euler(15f, -30f, 0f);
        }
        
        RenderSettings.skybox = duskSkybox != null ? duskSkybox : daySkybox;
        RenderSettings.ambientSkyColor = new Color(0.4f, 0.3f, 0.3f);
    }
    
    private void SetNightSettings()
    {
        if (directionalLight != null)
        {
            directionalLight.color = new Color(0.5f, 0.6f, 1f);
            directionalLight.intensity = 0.3f;
            directionalLight.transform.rotation = Quaternion.Euler(-10f, -30f, 0f);
        }
        
        RenderSettings.skybox = nightSkybox != null ? nightSkybox : daySkybox;
        RenderSettings.ambientSkyColor = new Color(0.1f, 0.1f, 0.2f);
        
        // Enable fog for night races
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.05f, 0.05f, 0.1f);
        RenderSettings.fogDensity = 0.005f;
    }
    
    public void SetWeather(TrackConfig.Weather weather)
    {
        // Weather effects would be handled by WeatherEffects component
        WeatherEffects weatherEffects = GetComponent<WeatherEffects>();
        if (weatherEffects != null)
        {
            weatherEffects.SetWeather(weather);
        }
    }
}

