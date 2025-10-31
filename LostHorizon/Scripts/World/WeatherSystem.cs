using UnityEngine;

namespace LostHorizon.World
{
    /// <summary>
    /// Manages dynamic weather effects (rain, storms, fog, etc.)
    /// </summary>
    public class WeatherSystem : MonoBehaviour
    {
        [Header("Weather Settings")]
        [SerializeField] private WeatherType currentWeather = WeatherType.Clear;
        [SerializeField] private float weatherChangeInterval = 300f; // 5 minutes
        [SerializeField] private bool allowWeatherChanges = true;

        [Header("Rain")]
        [SerializeField] private ParticleSystem rainEffect;
        [SerializeField] private float rainIntensity = 0f;

        [Header("Fog")]
        [SerializeField] private bool useFog = true;
        [SerializeField] private float fogDensity = 0f;

        [Header("Wind")]
        [SerializeField] private float windStrength = 0f;
        [SerializeField] private Vector3 windDirection = Vector3.forward;

        // Events
        public System.Action<WeatherType> OnWeatherChanged;

        private float weatherTimer = 0f;

        public WeatherType CurrentWeather => currentWeather;
        public float RainIntensity => rainIntensity;
        public float FogDensity => fogDensity;

        private void Update()
        {
            if (allowWeatherChanges)
            {
                weatherTimer += Time.deltaTime;
                if (weatherTimer >= weatherChangeInterval)
                {
                    ChangeWeatherRandomly();
                    weatherTimer = 0f;
                }
            }

            UpdateWeatherEffects();
        }

        private void UpdateWeatherEffects()
        {
            // Update rain
            if (rainEffect != null)
            {
                var emission = rainEffect.emission;
                emission.rateOverTime = rainIntensity * 100f;
                
                if (rainIntensity > 0f && !rainEffect.isPlaying)
                {
                    rainEffect.Play();
                }
                else if (rainIntensity <= 0f && rainEffect.isPlaying)
                {
                    rainEffect.Stop();
                }
            }

            // Update fog
            if (useFog)
            {
                RenderSettings.fog = fogDensity > 0f;
                RenderSettings.fogDensity = fogDensity;
            }
        }

        public void SetWeather(WeatherType weather)
        {
            if (currentWeather == weather) return;

            currentWeather = weather;
            ApplyWeatherSettings(weather);
            OnWeatherChanged?.Invoke(weather);
        }

        private void ApplyWeatherSettings(WeatherType weather)
        {
            switch (weather)
            {
                case WeatherType.Clear:
                    rainIntensity = 0f;
                    fogDensity = 0f;
                    windStrength = 0f;
                    break;

                case WeatherType.Cloudy:
                    rainIntensity = 0f;
                    fogDensity = 0.01f;
                    windStrength = 0.1f;
                    break;

                case WeatherType.Rainy:
                    rainIntensity = 0.5f;
                    fogDensity = 0.02f;
                    windStrength = 0.3f;
                    break;

                case WeatherType.Storm:
                    rainIntensity = 1f;
                    fogDensity = 0.03f;
                    windStrength = 0.8f;
                    break;

                case WeatherType.Foggy:
                    rainIntensity = 0f;
                    fogDensity = 0.05f;
                    windStrength = 0.05f;
                    break;
            }
        }

        private void ChangeWeatherRandomly()
        {
            WeatherType[] possibleWeathers = System.Enum.GetValues(typeof(WeatherType)) as WeatherType[];
            WeatherType newWeather = possibleWeathers[Random.Range(0, possibleWeathers.Length)];
            SetWeather(newWeather);
        }

        public float GetTemperatureModifier()
        {
            // Weather affects temperature
            switch (currentWeather)
            {
                case WeatherType.Storm:
                case WeatherType.Rainy:
                    return -5f; // Colder
                case WeatherType.Clear:
                    return 2f; // Warmer
                default:
                    return 0f;
            }
        }
    }

    public enum WeatherType
    {
        Clear,
        Cloudy,
        Rainy,
        Storm,
        Foggy
    }
}

