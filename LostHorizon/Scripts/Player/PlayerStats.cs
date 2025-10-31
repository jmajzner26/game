using UnityEngine;

namespace LostHorizon.Player
{
    /// <summary>
    /// Manages player health, stamina, hunger, and other stats
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float currentStamina;
        [SerializeField] private float staminaRegenRate = 10f;
        [SerializeField] private float staminaDrainRate = 20f;

        [Header("Hunger")]
        [SerializeField] private float maxHunger = 100f;
        [SerializeField] private float currentHunger;
        [SerializeField] private float hungerDecayRate = 0.5f; // Per minute

        [Header("Temperature")]
        [SerializeField] private float baseTemperature = 20f; // Celsius
        [SerializeField] private float currentTemperature;

        [Header("Status Effects")]
        [SerializeField] private bool isExhausted = false;
        [SerializeField] private bool isCold = false;
        [SerializeField] private bool isHungry = false;

        // Events
        public System.Action<float> OnHealthChanged;
        public System.Action<float> OnStaminaChanged;
        public System.Action<float> OnHungerChanged;
        public System.Action OnDeath;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float HealthPercent => currentHealth / maxHealth;

        public float MaxStamina => maxStamina;
        public float CurrentStamina => currentStamina;
        public float StaminaPercent => currentStamina / maxStamina;

        public float MaxHunger => maxHunger;
        public float CurrentHunger => currentHunger;
        public float HungerPercent => currentHunger / maxHunger;

        public bool IsExhausted => isExhausted;
        public bool IsCold => isCold;
        public bool IsHungry => isHungry;

        private void Start()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            currentHunger = maxHunger;
            currentTemperature = baseTemperature;
        }

        private void Update()
        {
            UpdateStamina();
            UpdateHunger();
            UpdateStatusEffects();
        }

        private void UpdateStamina()
        {
            // Regenerate stamina if not being used
            if (currentStamina < maxStamina && !isExhausted)
            {
                currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
                OnStaminaChanged?.Invoke(currentStamina);
            }
        }

        private void UpdateHunger()
        {
            // Decay hunger over time
            currentHunger = Mathf.Max(0f, currentHunger - (hungerDecayRate / 60f) * Time.deltaTime);
            OnHungerChanged?.Invoke(currentHunger);

            if (currentHunger <= 0f)
            {
                isHungry = true;
            }
        }

        private void UpdateStatusEffects()
        {
            isExhausted = currentStamina <= 0f;
            isCold = currentTemperature < 10f;
            isHungry = currentHunger < 20f;

            // Health damage from negative status effects
            if (isHungry)
            {
                TakeDamage(hungerDecayRate / 120f * Time.deltaTime);
            }

            if (isCold)
            {
                TakeDamage(1f * Time.deltaTime);
            }
        }

        public void TakeDamage(float amount)
        {
            currentHealth = Mathf.Max(0f, currentHealth - amount);
            OnHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth);
        }

        public bool UseStamina(float amount)
        {
            if (currentStamina >= amount)
            {
                currentStamina -= amount;
                OnStaminaChanged?.Invoke(currentStamina);
                return true;
            }
            return false;
        }

        public void RestoreStamina(float amount)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
            OnStaminaChanged?.Invoke(currentStamina);
        }

        public void Eat(float hungerRestore)
        {
            currentHunger = Mathf.Min(maxHunger, currentHunger + hungerRestore);
            OnHungerChanged?.Invoke(currentHunger);
            isHungry = false;
        }

        public void SetTemperature(float temperature)
        {
            currentTemperature = temperature;
            isCold = currentTemperature < 10f;
        }

        private void Die()
        {
            OnDeath?.Invoke();
            Debug.Log("Player has died!");
            // Handle death logic (respawn, game over screen, etc.)
        }

        public void ResetStats()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            currentHunger = maxHunger;
            currentTemperature = baseTemperature;
            isExhausted = false;
            isCold = false;
            isHungry = false;
        }
    }
}

