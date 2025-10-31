using UnityEngine;

namespace LostHorizon.Resources
{
    /// <summary>
    /// Represents a harvestable resource node in the world (tree, rock, ore vein, etc.)
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ResourceNode : MonoBehaviour
    {
        [Header("Resource Settings")]
        [SerializeField] private ResourceType resourceType = ResourceType.Wood;
        [SerializeField] private int maxResources = 10;
        [SerializeField] private int currentResources;
        [SerializeField] private float harvestTime = 2f;
        [SerializeField] private float respawnTime = 300f; // 5 minutes default

        [Header("Visual")]
        [SerializeField] private GameObject visualModel;
        [SerializeField] private ParticleSystem harvestEffect;
        [SerializeField] private AudioClip harvestSound;

        [Header("Requirements")]
        [SerializeField] private bool requiresTool = true;
        [SerializeField] private Equipment.EquipmentType requiredToolType = Equipment.EquipmentType.Pickaxe;

        private bool isHarvesting = false;
        private float harvestTimer = 0f;
        private float respawnTimer = 0f;
        private bool isDepleted = false;
        private Vector3 originalScale;

        public ResourceType ResourceType => resourceType;
        public int CurrentResources => currentResources;
        public bool IsDepleted => isDepleted;
        public bool RequiresTool => requiresTool;
        public Equipment.EquipmentType RequiredToolType => requiredToolType;

        private void Start()
        {
            currentResources = maxResources;
            if (visualModel != null)
            {
                originalScale = visualModel.transform.localScale;
            }
        }

        private void Update()
        {
            // Handle respawn timer
            if (isDepleted)
            {
                respawnTimer += Time.deltaTime;
                if (respawnTimer >= respawnTime)
                {
                    Respawn();
                }
            }
        }

        /// <summary>
        /// Attempts to harvest resources from this node
        /// </summary>
        /// <param name="harvester">The player or entity harvesting</param>
        /// <param name="toolType">Type of tool being used</param>
        /// <param name="deltaTime">Time passed since last harvest attempt</param>
        /// <returns>Amount of resources harvested (0 if failed or incomplete)</returns>
        public int Harvest(Transform harvester, Equipment.EquipmentType toolType, float deltaTime)
        {
            if (isDepleted || currentResources <= 0)
                return 0;

            if (requiresTool && toolType != requiredToolType)
            {
                Debug.Log($"This resource requires a {requiredToolType} to harvest!");
                return 0;
            }

            // Check if harvester is close enough
            float distance = Vector3.Distance(transform.position, harvester.position);
            if (distance > 3f)
            {
                isHarvesting = false;
                harvestTimer = 0f;
                return 0;
            }

            isHarvesting = true;
            harvestTimer += deltaTime;

            if (harvestTimer >= harvestTime)
            {
                // Successful harvest
                int amountHarvested = Mathf.Min(1, currentResources); // Harvest 1 at a time
                currentResources -= amountHarvested;
                harvestTimer = 0f;

                // Play effects
                if (harvestEffect != null && !harvestEffect.isPlaying)
                {
                    harvestEffect.Play();
                }

                if (harvestSound != null)
                {
                    AudioSource.PlayClipAtPoint(harvestSound, transform.position);
                }

                // Update visuals
                UpdateVisuals();

                // Check if depleted
                if (currentResources <= 0)
                {
                    Deplete();
                }

                return amountHarvested;
            }

            return 0;
        }

        private void UpdateVisuals()
        {
            if (visualModel != null && originalScale != Vector3.zero)
            {
                float scaleFactor = (float)currentResources / maxResources;
                visualModel.transform.localScale = originalScale * Mathf.Max(0.3f, scaleFactor);
            }
        }

        private void Deplete()
        {
            isDepleted = true;
            isHarvesting = false;
            respawnTimer = 0f;

            if (visualModel != null)
            {
                visualModel.SetActive(false);
            }

            // Disable collider
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }

        private void Respawn()
        {
            isDepleted = false;
            currentResources = maxResources;
            respawnTimer = 0f;

            if (visualModel != null)
            {
                visualModel.SetActive(true);
                visualModel.transform.localScale = originalScale;
            }

            // Re-enable collider
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }
        }

        public void StopHarvesting()
        {
            isHarvesting = false;
            harvestTimer = 0f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isDepleted ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, 3f);
        }
    }
}

