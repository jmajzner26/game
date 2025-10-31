using UnityEngine;

namespace ShadowDuel.Core
{
    /// <summary>
    /// Manages the overall combat state and flow between player and enemies
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] private float combatRange = 5f;
        [SerializeField] private float hitStunDuration = 0.5f;
        [SerializeField] private float perfectParryWindow = 0.2f;
        [SerializeField] private float slowMotionScale = 0.25f;
        [SerializeField] private float slowMotionDuration = 2f;

        [Header("Audio")]
        [SerializeField] private AudioClip parrySound;
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioClip finisherSound;

        private static CombatManager instance;
        public static CombatManager Instance => instance;

        private AudioSource audioSource;
        private bool isSlowMotion = false;
        private bool combatActive = false;

        public bool CombatActive => combatActive;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            audioSource = gameObject.AddComponent<AudioSource>();
        }

        /// <summary>
        /// Register a hit between combatants
        /// </summary>
        public void RegisterHit(GameObject attacker, GameObject defender, float damage, Vector3 hitPoint)
        {
            if (!combatActive) return;

            // Play hit sound
            if (audioSource && hitSound)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // Check for finisher condition
            if (ShouldTriggerFinisher(attacker, defender))
            {
                TriggerFinisher(attacker, defender);
            }
        }

        /// <summary>
        /// Register a parry event
        /// </summary>
        public void RegisterParry(GameObject parrier, GameObject attacker, bool isPerfect = false)
        {
            if (!combatActive) return;

            // Play parry sound
            if (audioSource && parrySound)
            {
                audioSource.PlayOneShot(parrySound);
            }

            if (isPerfect)
            {
                // Perfect parry rewards
                if (parrier.TryGetComponent<ParrySystem>(out var parrySystem))
                {
                    parrySystem.OnPerfectParry();
                }
            }
        }

        /// <summary>
        /// Trigger slow-motion finisher sequence
        /// </summary>
        private void TriggerFinisher(GameObject attacker, GameObject defender)
        {
            if (isSlowMotion) return;

            isSlowMotion = true;
            Time.timeScale = slowMotionScale;

            if (audioSource && finisherSound)
            {
                audioSource.PlayOneShot(finisherSound);
            }

            Invoke(nameof(EndSlowMotion), slowMotionDuration);
        }

        private void EndSlowMotion()
        {
            Time.timeScale = 1f;
            isSlowMotion = false;
        }

        /// <summary>
        /// Check if finisher conditions are met
        /// </summary>
        private bool ShouldTriggerFinisher(GameObject attacker, GameObject defender)
        {
            // Check if defender is low on health or stunned
            if (defender.TryGetComponent<Health>(out var health))
            {
                return health.IsDead || health.CurrentHealth < health.MaxHealth * 0.2f;
            }
            return false;
        }

        /// <summary>
        /// Start combat mode
        /// </summary>
        public void StartCombat()
        {
            combatActive = true;
        }

        /// <summary>
        /// End combat mode
        /// </summary>
        public void EndCombat()
        {
            combatActive = false;
            if (isSlowMotion)
            {
                EndSlowMotion();
            }
        }

        /// <summary>
        /// Get if slow motion is active
        /// </summary>
        public bool IsSlowMotion() => isSlowMotion;

        private void OnDestroy()
        {
            Time.timeScale = 1f; // Reset time scale
        }
    }

    /// <summary>
    /// Health component for combatants
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public bool IsDead => currentHealth <= 0;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);

            if (IsDead)
            {
                OnDeath();
            }
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        protected virtual void OnDeath()
        {
            // Override in derived classes for death behavior
        }
    }
}

