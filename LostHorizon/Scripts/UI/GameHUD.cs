using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LostHorizon.UI
{
    /// <summary>
    /// Main game HUD displaying health, stamina, hunger, time, etc.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Health Bar")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Stamina Bar")]
        [SerializeField] private Slider staminaBar;
        [SerializeField] private TextMeshProUGUI staminaText;

        [Header("Hunger Bar")]
        [SerializeField] private Slider hungerBar;
        [SerializeField] private TextMeshProUGUI hungerText;

        [Header("Time Display")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dayText;

        [Header("Crosshair")]
        [SerializeField] private Image crosshair;

        [Header("Interaction Prompt")]
        [SerializeField] private GameObject interactionPrompt;
        [SerializeField] private TextMeshProUGUI interactionText;

        private Player.PlayerStats playerStats;
        private World.WorldManager worldManager;

        private void Start()
        {
            playerStats = FindObjectOfType<Player.PlayerStats>();
            worldManager = World.WorldManager.Instance;

            // Subscribe to events
            if (playerStats != null)
            {
                playerStats.OnHealthChanged += UpdateHealth;
                playerStats.OnStaminaChanged += UpdateStamina;
                playerStats.OnHungerChanged += UpdateHunger;
            }

            if (worldManager != null)
            {
                worldManager.OnTimeChanged += UpdateTime;
                worldManager.OnDayChanged += UpdateDay;
            }

            UpdateAll();
        }

        private void Update()
        {
            // Continuously update (in case events aren't firing)
            UpdateAll();
        }

        private void UpdateAll()
        {
            if (playerStats != null)
            {
                UpdateHealth(playerStats.CurrentHealth);
                UpdateStamina(playerStats.CurrentStamina);
                UpdateHunger(playerStats.CurrentHunger);
            }

            if (worldManager != null)
            {
                UpdateTime(worldManager.GetCurrentTime());
                UpdateDay(worldManager.GetCurrentDay());
            }
        }

        private void UpdateHealth(float health)
        {
            if (healthBar != null && playerStats != null)
            {
                healthBar.value = playerStats.HealthPercent;
            }

            if (healthText != null && playerStats != null)
            {
                healthText.text = $"{(int)playerStats.CurrentHealth}/{(int)playerStats.MaxHealth}";
            }
        }

        private void UpdateStamina(float stamina)
        {
            if (staminaBar != null && playerStats != null)
            {
                staminaBar.value = playerStats.StaminaPercent;
            }

            if (staminaText != null && playerStats != null)
            {
                staminaText.text = $"{(int)playerStats.CurrentStamina}/{(int)playerStats.MaxStamina}";
            }
        }

        private void UpdateHunger(float hunger)
        {
            if (hungerBar != null && playerStats != null)
            {
                hungerBar.value = playerStats.HungerPercent;
            }

            if (hungerText != null && playerStats != null)
            {
                hungerText.text = $"{(int)playerStats.CurrentHunger}/{(int)playerStats.MaxHunger}";
            }
        }

        private void UpdateTime(float time)
        {
            if (timeText != null)
            {
                int hours = Mathf.FloorToInt(time);
                int minutes = Mathf.FloorToInt((time - hours) * 60f);
                timeText.text = $"{hours:00}:{minutes:00}";
            }
        }

        private void UpdateDay(int day)
        {
            if (dayText != null)
            {
                dayText.text = $"Day {day}";
            }
        }

        public void ShowInteractionPrompt(string text)
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
                if (interactionText != null)
                {
                    interactionText.text = text;
                }
            }
        }

        public void HideInteractionPrompt()
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (playerStats != null)
            {
                playerStats.OnHealthChanged -= UpdateHealth;
                playerStats.OnStaminaChanged -= UpdateStamina;
                playerStats.OnHungerChanged -= UpdateHunger;
            }

            if (worldManager != null)
            {
                worldManager.OnTimeChanged -= UpdateTime;
                worldManager.OnDayChanged -= UpdateDay;
            }
        }
    }
}

