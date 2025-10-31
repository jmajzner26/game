using UnityEngine;
using UnityEngine.Rendering;

namespace ShadowDuel.Arena
{
    /// <summary>
    /// Controls weather effects in the arena
    /// </summary>
    public class WeatherController : MonoBehaviour
    {
        [Header("Weather Settings")]
        [SerializeField] private WeatherType currentWeather = WeatherType.Clear;
        [SerializeField] private bool randomizeWeather = false;

        [Header("Rain")]
        [SerializeField] private ParticleSystem rainEffect;
        [SerializeField] private AudioSource rainAudio;
        [SerializeField] private GameObject puddles;

        [Header("Fog")]
        [SerializeField] private FogMode fogMode = FogMode.ExponentialSquared;
        [SerializeField] private Color fogColor = Color.gray;
        [SerializeField] private float fogDensity = 0.01f;

        [Header("Lightning")]
        [SerializeField] private Light directionalLight;
        [SerializeField] private float lightningInterval = 10f;
        [SerializeField] private float lightningDuration = 0.5f;
        [SerializeField] private AudioClip lightningSound;

        [Header("Wind")]
        [SerializeField] private ParticleSystem windEffect;
        [SerializeField] private float windStrength = 0.5f;

        private float lastLightningTime = 0f;
        private bool isActive = false;

        public enum WeatherType
        {
            Clear,
            Rain,
            Fog,
            Storm,
            Mist
        }

        private void Start()
        {
            if (randomizeWeather)
            {
                SetRandomWeather();
            }
            else
            {
                SetWeather(currentWeather);
            }
        }

        private void Update()
        {
            if (!isActive) return;

            // Lightning effects during storms
            if (currentWeather == WeatherType.Storm && Time.time - lastLightningTime > lightningInterval)
            {
                TriggerLightning();
                lastLightningTime = Time.time;
            }
        }

        /// <summary>
        /// Initialize weather system
        /// </summary>
        public void InitializeWeather()
        {
            SetWeather(currentWeather);
        }

        /// <summary>
        /// Set active state
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;

            if (!active)
            {
                DeactivateAllWeather();
            }
        }

        private void SetRandomWeather()
        {
            System.Array weatherTypes = System.Enum.GetValues(typeof(WeatherType));
            WeatherType randomWeather = (WeatherType)weatherTypes.GetValue(Random.Range(0, weatherTypes.Length));
            SetWeather(randomWeather);
        }

        /// <summary>
        /// Set specific weather type
        /// </summary>
        public void SetWeather(WeatherType weather)
        {
            currentWeather = weather;
            DeactivateAllWeather();

            switch (weather)
            {
                case WeatherType.Clear:
                    SetClearWeather();
                    break;
                case WeatherType.Rain:
                    SetRainWeather();
                    break;
                case WeatherType.Fog:
                    SetFogWeather();
                    break;
                case WeatherType.Storm:
                    SetStormWeather();
                    break;
                case WeatherType.Mist:
                    SetMistWeather();
                    break;
            }
        }

        private void SetClearWeather()
        {
            RenderSettings.fog = false;
        }

        private void SetRainWeather()
        {
            if (rainEffect) rainEffect.Play();
            if (rainAudio) rainAudio.Play();
            if (puddles) puddles.SetActive(true);

            RenderSettings.fog = true;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity * 2f;

            // Dim ambient light
            if (directionalLight)
            {
                directionalLight.intensity = 0.6f;
                directionalLight.color = Color.Lerp(Color.white, Color.gray, 0.3f);
            }
        }

        private void SetFogWeather()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;

            if (directionalLight)
            {
                directionalLight.intensity = 0.5f;
            }
        }

        private void SetStormWeather()
        {
            SetRainWeather();
            SetFogWeather();

            if (windEffect) windEffect.Play();

            // Storm-specific effects
            if (directionalLight)
            {
                directionalLight.intensity = 0.4f;
                directionalLight.color = Color.Lerp(Color.white, Color.blue, 0.2f);
            }
        }

        private void SetMistWeather()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogColor = Color.Lerp(fogColor, Color.white, 0.5f);
            RenderSettings.fogDensity = fogDensity * 0.5f;

            if (windEffect) windEffect.Play();

            if (directionalLight)
            {
                directionalLight.intensity = 0.7f;
            }
        }

        private void DeactivateAllWeather()
        {
            if (rainEffect) rainEffect.Stop();
            if (rainAudio) rainAudio.Stop();
            if (windEffect) windEffect.Stop();
            if (puddles) puddles.SetActive(false);
        }

        private void TriggerLightning()
        {
            if (directionalLight)
            {
                StartCoroutine(LightningFlash());
            }

            if (lightningSound)
            {
                AudioSource.PlayClipAtPoint(lightningSound, transform.position);
            }
        }

        private System.Collections.IEnumerator LightningFlash()
        {
            float originalIntensity = directionalLight.intensity;
            
            directionalLight.intensity = 2f;
            yield return new WaitForSeconds(lightningDuration);
            
            directionalLight.intensity = originalIntensity;
        }
    }

    /// <summary>
    /// Breakable prop in the arena
    /// </summary>
    public class BreakableProp : MonoBehaviour
    {
        [Header("Prop Settings")]
        [SerializeField] private float health = 50f;
        [SerializeField] private GameObject debrisPrefab;
        [SerializeField] private AudioClip breakSound;

        private bool isBroken = false;

        private void OnTriggerEnter(Collider other)
        {
            if (isBroken) return;

            // Check if hit by weapon
            if (other.CompareTag("Weapon"))
            {
                TakeDamage(health * 0.5f);
            }
        }

        public void TakeDamage(float damage)
        {
            if (isBroken) return;

            health -= damage;

            if (health <= 0)
            {
                Break();
            }
        }

        private void Break()
        {
            isBroken = true;

            // Spawn debris
            if (debrisPrefab)
            {
                Instantiate(debrisPrefab, transform.position, Quaternion.identity);
            }

            // Play sound
            if (breakSound)
            {
                AudioSource.PlayClipAtPoint(breakSound, transform.position);
            }

            // Destroy prop
            Destroy(gameObject, 0.1f);
        }
    }
}

