using UnityEngine;

namespace LostHorizon.World
{
    /// <summary>
    /// Manages the game world, including time, weather, and world state
    /// </summary>
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance { get; private set; }

        [Header("Time Settings")]
        [SerializeField] private float timeOfDay = 12f; // 0-24 hours
        [SerializeField] private float timeScale = 60f; // Real seconds per game hour
        [SerializeField] private bool pauseTime = false;

        [Header("Weather")]
        [SerializeField] private WeatherSystem weatherSystem;

        [Header("Lighting")]
        [SerializeField] private Light sun;
        [SerializeField] private Light moon;
        [SerializeField] private Gradient skyColorGradient;
        [SerializeField] private AnimationCurve sunIntensityCurve;

        // Events
        public System.Action<float> OnTimeChanged;
        public System.Action<int> OnDayChanged;

        private int currentDay = 1;

        public float GetCurrentTime() => timeOfDay;
        public int GetCurrentDay() => currentDay;
        public bool IsDaytime() => timeOfDay >= 6f && timeOfDay < 18f;
        public bool IsNighttime() => !IsDaytime();

        public void Initialize()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (weatherSystem == null)
                weatherSystem = GetComponent<WeatherSystem>();
            if (weatherSystem == null)
                weatherSystem = gameObject.AddComponent<WeatherSystem>();

            if (sun == null)
                sun = RenderSettings.sun;

            // Initialize sky gradient if not set
            if (skyColorGradient == null)
            {
                skyColorGradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(new Color(0.3f, 0.4f, 0.6f), 0f), // Night (midnight)
                    new GradientColorKey(new Color(0.5f, 0.6f, 0.8f), 0.25f), // Dawn
                    new GradientColorKey(new Color(0.7f, 0.8f, 1f), 0.5f), // Noon
                    new GradientColorKey(new Color(0.5f, 0.6f, 0.8f), 0.75f), // Dusk
                    new GradientColorKey(new Color(0.3f, 0.4f, 0.6f), 1f) // Night (midnight)
                };
                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                };
                skyColorGradient.SetKeys(colorKeys, alphaKeys);
            }

            UpdateLighting();
        }

        private void Update()
        {
            if (!pauseTime)
            {
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            float timeDelta = Time.deltaTime / timeScale;
            timeOfDay += timeDelta;

            if (timeOfDay >= 24f)
            {
                timeOfDay = 0f;
                currentDay++;
                OnDayChanged?.Invoke(currentDay);
            }

            OnTimeChanged?.Invoke(timeOfDay);
            UpdateLighting();
        }

        private void UpdateLighting()
        {
            // Calculate sun rotation based on time of day
            float sunAngle = ((timeOfDay / 24f) * 360f) - 90f;
            if (sun != null)
            {
                sun.transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);
                
                // Adjust sun intensity
                float normalizedTime = timeOfDay / 24f;
                if (sunIntensityCurve != null && sunIntensityCurve.length > 0)
                {
                    sun.intensity = sunIntensityCurve.Evaluate(normalizedTime);
                }
                else
                {
                    sun.intensity = IsDaytime() ? 1f : 0.1f;
                }
            }

            // Update sky color
            float normalizedTime = timeOfDay / 24f;
            if (skyColorGradient != null)
            {
                Color skyColor = skyColorGradient.Evaluate(normalizedTime);
                RenderSettings.ambientSkyColor = skyColor;
                RenderSettings.fogColor = skyColor;
            }
        }

        public void SetTime(float hours)
        {
            timeOfDay = Mathf.Clamp(hours, 0f, 24f);
            UpdateLighting();
        }

        public void SetTimeScale(float scale)
        {
            timeScale = Mathf.Max(1f, scale);
        }

        public void PauseTime(bool pause)
        {
            pauseTime = pause;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}

