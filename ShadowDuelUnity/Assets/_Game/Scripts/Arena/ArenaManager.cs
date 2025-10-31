using UnityEngine;
using ShadowDuel.Core;

namespace ShadowDuel.Arena
{
    /// <summary>
    /// Manages arena state, hazards, and environment
    /// </summary>
    public class ArenaManager : MonoBehaviour
    {
        [Header("Arena Settings")]
        [SerializeField] private string arenaName = "Unknown Arena";
        [SerializeField] private float arenaRadius = 20f;
        [SerializeField] private bool hasBoundary = true;

        [Header("Environmental Hazards")]
        [SerializeField] private EnvironmentalHazard[] hazards;
        [SerializeField] private float hazardDamage = 10f;

        [Header("Weather")]
        [SerializeField] private WeatherController weatherController;

        [Header("Breakable Props")]
        [SerializeField] private BreakableProp[] breakableProps;

        private CombatManager combatManager;
        private bool combatInProgress = false;

        public string ArenaName => arenaName;
        public bool CombatInProgress => combatInProgress;

        private void Awake()
        {
            combatManager = FindObjectOfType<CombatManager>();
        }

        private void Start()
        {
            InitializeArena();
        }

        private void InitializeArena()
        {
            // Enable/disable hazards
            if (hazards != null)
            {
                foreach (var hazard in hazards)
                {
                    if (hazard)
                    {
                        hazard.SetDamage(hazardDamage);
                    }
                }
            }

            // Initialize weather
            if (weatherController)
            {
                weatherController.InitializeWeather();
            }
        }

        /// <summary>
        /// Start combat in this arena
        /// </summary>
        public void StartCombat()
        {
            combatInProgress = true;

            if (combatManager)
            {
                combatManager.StartCombat();
            }

            // Activate arena systems
            ActivateHazards(true);
            ActivateWeather(true);
        }

        /// <summary>
        /// End combat in this arena
        /// </summary>
        public void EndCombat()
        {
            combatInProgress = false;

            if (combatManager)
            {
                combatManager.EndCombat();
            }

            // Deactivate arena systems
            ActivateHazards(false);
            ActivateWeather(false);
        }

        private void ActivateHazards(bool active)
        {
            if (hazards == null) return;

            foreach (var hazard in hazards)
            {
                if (hazard)
                {
                    hazard.SetActive(active);
                }
            }
        }

        private void ActivateWeather(bool active)
        {
            if (weatherController)
            {
                weatherController.SetActive(active);
            }
        }

        /// <summary>
        /// Check if position is within arena bounds
        /// </summary>
        public bool IsWithinBounds(Vector3 position)
        {
            if (!hasBoundary) return true;

            float distanceFromCenter = Vector3.Distance(transform.position, position);
            return distanceFromCenter <= arenaRadius;
        }

        /// <summary>
        /// Get the push-back direction from arena edge
        /// </summary>
        public Vector3 GetBoundaryDirection(Vector3 position)
        {
            return (position - transform.position).normalized;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw arena boundary
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, arenaRadius);
        }
    }
}

