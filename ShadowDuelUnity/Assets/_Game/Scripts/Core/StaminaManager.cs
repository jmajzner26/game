using UnityEngine;
using UnityEngine.UI;

namespace ShadowDuel.Core
{
    /// <summary>
    /// Manages stamina resource for combat actions
    /// </summary>
    public class StaminaManager : MonoBehaviour
    {
        [Header("Stamina Settings")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaRegenRate = 20f;
        [SerializeField] private float staminaRegenDelay = 1f;

        [Header("Action Costs")]
        [SerializeField] private float lightAttackCost = 10f;
        [SerializeField] private float heavyAttackCost = 25f;
        [SerializeField] private float dodgeCost = 15f;
        [SerializeField] private float parryCost = 20f;
        [SerializeField] private float specialAbilityCost = 50f;

        [Header("UI")]
        [SerializeField] private Slider staminaBar;
        [SerializeField] private Image staminaFill;

        private float currentStamina;
        private float lastStaminaUseTime;
        private bool isRegenerating = true;

        public float MaxStamina => maxStamina;
        public float CurrentStamina => currentStamina;
        public float StaminaPercentage => maxStamina > 0 ? currentStamina / maxStamina : 0f;
        public bool IsExhausted => currentStamina <= 0;

        private void Awake()
        {
            currentStamina = maxStamina;
        }

        private void Update()
        {
            // Start regeneration after delay
            if (Time.time - lastStaminaUseTime > staminaRegenDelay)
            {
                isRegenerating = true;
            }

            if (isRegenerating && currentStamina < maxStamina)
            {
                RegenerateStamina();
            }

            UpdateUI();
        }

        /// <summary>
        /// Consume stamina for a light attack
        /// </summary>
        public bool ConsumeLightAttack()
        {
            return ConsumeStamina(lightAttackCost);
        }

        /// <summary>
        /// Consume stamina for a heavy attack
        /// </summary>
        public bool ConsumeHeavyAttack()
        {
            return ConsumeStamina(heavyAttackCost);
        }

        /// <summary>
        /// Consume stamina for a dodge
        /// </summary>
        public bool ConsumeDodge()
        {
            return ConsumeStamina(dodgeCost);
        }

        /// <summary>
        /// Consume stamina for a parry
        /// </summary>
        public bool ConsumeParryStamina()
        {
            return ConsumeStamina(parryCost);
        }

        /// <summary>
        /// Consume stamina for special ability
        /// </summary>
        public bool ConsumeSpecialAbility()
        {
            return ConsumeStamina(specialAbilityCost);
        }

        /// <summary>
        /// Check if can parry (enough stamina)
        /// </summary>
        public bool CanParry()
        {
            return currentStamina >= parryCost;
        }

        /// <summary>
        /// Check if can dodge
        /// </summary>
        public bool CanDodge()
        {
            return currentStamina >= dodgeCost;
        }

        /// <summary>
        /// Check if can use special ability
        /// </summary>
        public bool CanUseSpecialAbility()
        {
            return currentStamina >= specialAbilityCost;
        }

        /// <summary>
        /// Consume stamina for an action
        /// </summary>
        private bool ConsumeStamina(float amount)
        {
            if (currentStamina < amount)
                return false;

            currentStamina -= amount;
            lastStaminaUseTime = Time.time;
            isRegenerating = false;
            return true;
        }

        /// <summary>
        /// Add stamina (for bonuses/perks)
        /// </summary>
        public void AddStamina(float amount)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        }

        /// <summary>
        /// Set stamina to full
        /// </summary>
        public void RestoreStamina()
        {
            currentStamina = maxStamina;
        }

        /// <summary>
        /// Regenerate stamina over time
        /// </summary>
        private void RegenerateStamina()
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
        }

        /// <summary>
        /// Update stamina UI
        /// </summary>
        private void UpdateUI()
        {
            if (staminaBar)
            {
                staminaBar.value = StaminaPercentage;
            }

            if (staminaFill)
            {
                // Change color based on stamina level
                if (currentStamina < maxStamina * 0.2f)
                {
                    staminaFill.color = Color.red;
                }
                else if (currentStamina < maxStamina * 0.5f)
                {
                    staminaFill.color = Color.yellow;
                }
                else
                {
                    staminaFill.color = Color.green;
                }
            }
        }

        /// <summary>
        /// Set custom stamina costs
        /// </summary>
        public void SetStaminaCosts(float lightAttack, float heavyAttack, float dodge, float parry)
        {
            lightAttackCost = lightAttack;
            heavyAttackCost = heavyAttack;
            dodgeCost = dodge;
            parryCost = parry;
        }

        /// <summary>
        /// Set stamina regen rate
        /// </summary>
        public void SetRegenRate(float rate)
        {
            staminaRegenRate = rate;
        }
    }
}

