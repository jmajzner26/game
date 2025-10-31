using UnityEngine;

namespace ShadowDuel.Core
{
    /// <summary>
    /// Handles parrying mechanics and timing windows
    /// </summary>
    public class ParrySystem : MonoBehaviour
    {
        [Header("Parry Settings")]
        [SerializeField] private float parryWindow = 0.3f;
        [SerializeField] private float perfectParryWindow = 0.1f;
        [SerializeField] private float parryCooldown = 0.5f;
        [SerializeField] private float perfectParryStaminaBoost = 20f;
        [SerializeField] private float stunDurationOnParry = 1f;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject parryEffectPrefab;
        [SerializeField] private GameObject perfectParryEffectPrefab;

        private bool canParry = true;
        private bool isParrying = false;
        private float parryTimer = 0f;

        private StaminaManager staminaManager;
        private CombatManager combatManager;

        private void Awake()
        {
            staminaManager = GetComponent<StaminaManager>();
            combatManager = CombatManager.Instance;
        }

        private void Update()
        {
            if (isParrying)
            {
                parryTimer += Time.deltaTime;
                
                if (parryTimer >= parryWindow)
                {
                    EndParry();
                }
            }
        }

        /// <summary>
        /// Attempt to start parrying
        /// </summary>
        public bool TryParry()
        {
            if (!canParry || isParrying)
                return false;

            if (staminaManager && staminaManager.CanParry())
            {
                StartParry();
                return true;
            }

            return false;
        }

        private void StartParry()
        {
            isParrying = true;
            parryTimer = 0f;
            
            // Consume stamina
            if (staminaManager)
            {
                staminaManager.ConsumeParryStamina();
            }
        }

        private void EndParry()
        {
            isParrying = false;
            parryTimer = 0f;
            canParry = false;
            
            Invoke(nameof(ResetParry), parryCooldown);
        }

        private void ResetParry()
        {
            canParry = true;
        }

        /// <summary>
        /// Check if currently parrying
        /// </summary>
        public bool IsParrying() => isParrying;

        /// <summary>
        /// Process incoming attack and attempt to parry
        /// </summary>
        public bool ProcessIncomingAttack(GameObject attacker, float damage)
        {
            if (!isParrying)
                return false;

            // Calculate if it's a perfect parry
            bool isPerfect = parryTimer <= perfectParryWindow;

            // Register the parry
            if (combatManager)
            {
                combatManager.RegisterParry(gameObject, attacker, isPerfect);
            }

            // Spawn visual effect
            SpawnParryEffect(isPerfect);

            // Stun the attacker
            if (attacker.TryGetComponent<Enemy.EnemyAI>(out var enemyAI))
            {
                enemyAI.Stun(stunDurationOnParry);
            }

            EndParry();
            return true;
        }

        /// <summary>
        /// Called when a perfect parry occurs
        /// </summary>
        public void OnPerfectParry()
        {
            if (staminaManager)
            {
                staminaManager.AddStamina(perfectParryStaminaBoost);
            }

            if (perfectParryEffectPrefab)
            {
                SpawnParryEffect(true);
            }
        }

        private void SpawnParryEffect(bool isPerfect)
        {
            GameObject effectPrefab = isPerfect ? perfectParryEffectPrefab : parryEffectPrefab;
            
            if (effectPrefab)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 1.5f;
                Instantiate(effectPrefab, spawnPos, Quaternion.identity);
            }
        }

        /// <summary>
        /// Get parry state for AI/visual feedback
        /// </summary>
        public float GetParryProgress() => isParrying ? parryTimer / parryWindow : 0f;
    }
}

