using UnityEngine;

namespace ShadowDuel.Arena
{
    /// <summary>
    /// Represents an environmental hazard in the arena
    /// </summary>
    public class EnvironmentalHazard : MonoBehaviour
    {
        [Header("Hazard Settings")]
        [SerializeField] private HazardType hazardType = HazardType.Fire;
        [SerializeField] private float damagePerSecond = 10f;
        [SerializeField] private float duration = -1f; // -1 for permanent
        [SerializeField] private bool isActive = true;

        [Header("Visuals")]
        [SerializeField] private ParticleSystem hazardEffect;
        [SerializeField] private Light hazardLight;

        [Header("Audio")]
        [SerializeField] private AudioSource hazardAudio;

        private float activeTime = 0f;
        private BoxCollider triggerZone;

        public enum HazardType
        {
            Fire,
            Poison,
            Electricity,
            Spikes,
            Acid
        }

        private void Awake()
        {
            triggerZone = GetComponent<BoxCollider>();
            if (!triggerZone)
            {
                triggerZone = gameObject.AddComponent<BoxCollider>();
                triggerZone.isTrigger = true;
                triggerZone.size = Vector3.one * 2f;
            }
        }

        private void Update()
        {
            if (!isActive) return;

            // Handle duration
            if (duration > 0)
            {
                activeTime += Time.deltaTime;
                if (activeTime >= duration)
                {
                    Deactivate();
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!isActive) return;

            // Damage player or enemies
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                Health health = other.GetComponent<Health>();
                if (health)
                {
                    health.TakeDamage(damagePerSecond * Time.deltaTime);
                    ApplyHazardEffect(other.gameObject);
                }
            }
        }

        private void ApplyHazardEffect(GameObject target)
        {
            switch (hazardType)
            {
                case HazardType.Fire:
                    ApplyFireEffect(target);
                    break;
                case HazardType.Poison:
                    ApplyPoisonEffect(target);
                    break;
                case HazardType.Electricity:
                    ApplyElectricEffect(target);
                    break;
            }
        }

        private void ApplyFireEffect(GameObject target)
        {
            // Could add burning status effect
            target.GetComponent<Renderer>()?.material.SetColor("_EmissionColor", Color.red * 2f);
        }

        private void ApplyPoisonEffect(GameObject target)
        {
            // Could add poison DoT effect
        }

        private void ApplyElectricEffect(GameObject target)
        {
            // Could add stun effect
        }

        /// <summary>
        /// Activate the hazard
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;

            if (hazardEffect)
            {
                if (active)
                    hazardEffect.Play();
                else
                    hazardEffect.Stop();
            }

            if (hazardLight)
            {
                hazardLight.enabled = active;
            }

            if (hazardAudio)
            {
                if (active)
                    hazardAudio.Play();
                else
                    hazardAudio.Stop();
            }
        }

        /// <summary>
        /// Set damage value
        /// </summary>
        public void SetDamage(float damage)
        {
            damagePerSecond = damage;
        }

        private void Deactivate()
        {
            SetActive(false);
            Destroy(gameObject, 2f);
        }
    }
}

