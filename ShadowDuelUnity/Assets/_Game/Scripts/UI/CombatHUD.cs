using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ShadowDuel.UI
{
    /// <summary>
    /// In-game combat HUD display
    /// </summary>
    public class CombatHUD : MonoBehaviour
    {
        [Header("Health Display")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Stamina Display")]
        [SerializeField] private Slider staminaBar;
        [SerializeField] private Image staminaFill;

        [Header("Enemy Health")]
        [SerializeField] private Slider enemyHealthBar;
        [SerializeField] private GameObject enemyHealthPanel;

        [Header("Combat Info")]
        [SerializeField] private TextMeshProUGUI comboCounter;
        [SerializeField] private TextMeshProUGUI parryIndicator;
        [SerializeField] private GameObject finisherIndicator;

        [Header("Weapon Display")]
        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private Image weaponIcon;

        private GameObject player;
        private GameObject currentEnemy;
        private Health playerHealth;
        private StaminaManager playerStamina;
        private ParrySystem playerParry;

        private float comboDisplayTime = 0f;
        private int currentCombo = 0;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            
            if (player)
            {
                playerHealth = player.GetComponent<Health>();
                playerStamina = player.GetComponent<StaminaManager>();
                playerParry = player.GetComponent<ParrySystem>();
            }
        }

        private void Update()
        {
            UpdateHealthBar();
            UpdateStaminaBar();
            UpdateEnemyHealth();
            UpdateCombatInfo();
            UpdateWeaponDisplay();
        }

        private void UpdateHealthBar()
        {
            if (healthBar && playerHealth)
            {
                healthBar.value = playerHealth.CurrentHealth / playerHealth.MaxHealth;
            }

            if (healthText && playerHealth)
            {
                healthText.text = $"{playerHealth.CurrentHealth:F0} / {playerHealth.MaxHealth:F0}";
            }
        }

        private void UpdateStaminaBar()
        {
            if (staminaBar && playerStamina)
            {
                staminaBar.value = playerStamina.StaminaPercentage;
            }
        }

        private void UpdateEnemyHealth()
        {
            if (!currentEnemy)
            {
                // Find nearest enemy
                currentEnemy = FindNearestEnemy();
            }

            if (enemyHealthPanel && currentEnemy)
            {
                Health enemyHealth = currentEnemy.GetComponent<Health>();

                if (enemyHealth && !enemyHealth.IsDead)
                {
                    enemyHealthPanel.SetActive(true);
                    enemyHealthBar.value = enemyHealth.CurrentHealth / enemyHealth.MaxHealth;
                }
                else
                {
                    enemyHealthPanel.SetActive(false);
                    currentEnemy = null;
                }
            }
            else if (enemyHealthPanel)
            {
                enemyHealthPanel.SetActive(false);
            }
        }

        private void UpdateCombatInfo()
        {
            // Update combo counter
            if (comboCounter)
            {
                if (comboDisplayTime > 0)
                {
                    comboCounter.text = $"COMBO x{currentCombo}";
                    comboDisplayTime -= Time.deltaTime;
                }
                else
                {
                    comboCounter.text = "";
                    currentCombo = 0;
                }
            }

            // Update parry indicator
            if (parryIndicator && playerParry)
            {
                float parryProgress = playerParry.GetParryProgress();
                if (parryProgress > 0)
                {
                    parryIndicator.gameObject.SetActive(true);
                    parryIndicator.text = "PARRY";
                    if (parryProgress < 0.1f / 0.3f) // Perfect parry window
                    {
                        parryIndicator.color = Color.yellow;
                        parryIndicator.text = "PERFECT!";
                    }
                    else
                    {
                        parryIndicator.color = Color.cyan;
                    }
                }
                else
                {
                    parryIndicator.gameObject.SetActive(false);
                }
            }

            // Update finisher indicator
            if (finisherIndicator)
            {
                // Check if finisher conditions are met
                bool canFinisher = CheckFinisherCondition();
                finisherIndicator.SetActive(canFinisher);
            }
        }

        private void UpdateWeaponDisplay()
        {
            if (player)
            {
                var playerController = player.GetComponent<Player.PlayerController>();
                if (playerController)
                {
                    var weapon = playerController.GetCurrentWeapon();
                    if (weapon && weaponName)
                    {
                        weaponName.text = weapon.WeaponName;
                    }
                }
            }
        }

        private GameObject FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return null;

            GameObject nearest = null;
            float minDistance = float.MaxValue;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        private bool CheckFinisherCondition()
        {
            // Check if enemy is low on health
            if (currentEnemy)
            {
                Health enemyHealth = currentEnemy.GetComponent<Health>();
                if (enemyHealth && enemyHealth.CurrentHealth < enemyHealth.MaxHealth * 0.2f)
                {
                    return true;
                }
            }
            return false;
        }

        public void ShowCombo(int combo)
        {
            currentCombo = combo;
            comboDisplayTime = 3f;
        }

        public void ShowPerfectParry()
        {
            if (parryIndicator)
            {
                parryIndicator.text = "PERFECT PARRY!";
                parryIndicator.color = Color.yellow;
                comboDisplayTime = 2f;
            }
        }
    }
}

