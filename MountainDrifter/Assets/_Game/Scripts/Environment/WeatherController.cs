using UnityEngine;

/// <summary>
/// Controls weather effects (rain, fog, snow) for dynamic track conditions.
/// </summary>
public class WeatherController : MonoBehaviour
{
    [Header("Weather Settings")]
    [SerializeField] private WeatherType currentWeather = WeatherType.Clear;
    [SerializeField] private bool dynamicWeather = false;
    [SerializeField] private float weatherChangeInterval = 60f;

    [Header("Rain")]
    [SerializeField] private ParticleSystem rainParticleSystem;
    [SerializeField] private float rainIntensity = 1f;
    [SerializeField] private AudioSource rainAudio;

    [Header("Fog")]
    [SerializeField] private bool useFog = true;
    [SerializeField] private Color fogColor = Color.gray;
    [SerializeField] private float fogDensity = 0.01f;

    [Header("Snow")]
    [SerializeField] private ParticleSystem snowParticleSystem;

    [Header("Lightning")]
    [SerializeField] private Light lightningLight;
    [SerializeField] private float lightningIntensity = 2f;

    private float weatherTimer = 0f;
    private float lightningTimer = 0f;

    public enum WeatherType
    {
        Clear,
        Rainy,
        Foggy,
        Snowy,
        Storm
    }

    private void Start()
    {
        ApplyWeather(currentWeather);
    }

    private void Update()
    {
        if (dynamicWeather)
        {
            weatherTimer += Time.deltaTime;
            if (weatherTimer >= weatherChangeInterval)
            {
                ChangeWeatherRandomly();
                weatherTimer = 0f;
            }
        }

        // Lightning effects for storm
        if (currentWeather == WeatherType.Storm && lightningLight != null)
        {
            lightningTimer += Time.deltaTime;
            if (lightningTimer > Random.Range(2f, 8f))
            {
                TriggerLightning();
                lightningTimer = 0f;
            }
        }
    }

    private void ApplyWeather(WeatherType weather)
    {
        currentWeather = weather;

        switch (weather)
        {
            case WeatherType.Clear:
                SetRain(false);
                SetSnow(false);
                SetFog(false);
                break;

            case WeatherType.Rainy:
                SetRain(true);
                SetSnow(false);
                SetFog(false);
                break;

            case WeatherType.Foggy:
                SetRain(false);
                SetSnow(false);
                SetFog(true);
                break;

            case WeatherType.Snowy:
                SetRain(false);
                SetSnow(true);
                SetFog(true);
                break;

            case WeatherType.Storm:
                SetRain(true);
                SetSnow(false);
                SetFog(true);
                break;
        }
    }

    private void SetRain(bool active)
    {
        if (rainParticleSystem != null)
        {
            if (active)
            {
                var emission = rainParticleSystem.emission;
                emission.rateOverTime = 100f * rainIntensity;
                rainParticleSystem.Play();
            }
            else
            {
                rainParticleSystem.Stop();
            }
        }

        if (rainAudio != null)
        {
            if (active)
            {
                rainAudio.volume = rainIntensity * 0.5f;
                rainAudio.Play();
            }
            else
            {
                rainAudio.Stop();
            }
        }
    }

    private void SetSnow(bool active)
    {
        if (snowParticleSystem != null)
        {
            if (active)
            {
                snowParticleSystem.Play();
            }
            else
            {
                snowParticleSystem.Stop();
            }
        }
    }

    private void SetFog(bool active)
    {
        if (useFog)
        {
            RenderSettings.fog = active;
            if (active)
            {
                RenderSettings.fogColor = fogColor;
                RenderSettings.fogDensity = fogDensity;
            }
        }
    }

    private void TriggerLightning()
    {
        if (lightningLight != null)
        {
            StartCoroutine(LightningFlash());
        }
    }

    private System.Collections.IEnumerator LightningFlash()
    {
        lightningLight.intensity = lightningIntensity;
        yield return new WaitForSeconds(0.1f);
        lightningLight.intensity = 0f;
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        lightningLight.intensity = lightningIntensity * 0.5f;
        yield return new WaitForSeconds(0.05f);
        lightningLight.intensity = 0f;
    }

    public void SetWeather(WeatherType weather)
    {
        ApplyWeather(weather);
    }

    public void ChangeWeatherRandomly()
    {
        WeatherType newWeather = (WeatherType)Random.Range(0, System.Enum.GetValues(typeof(WeatherType)).Length);
        ApplyWeather(newWeather);
    }

    public WeatherType GetCurrentWeather()
    {
        return currentWeather;
    }
}

