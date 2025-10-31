using UnityEngine;

namespace LostHorizon.Crafting
{
    /// <summary>
    /// Represents a crafting station in the world (workbench, forge, etc.)
    /// </summary>
    public class CraftingStation : MonoBehaviour, Player.IInteractable
    {
        [Header("Station Info")]
        [SerializeField] private string stationType = "Workbench"; // Workbench, Forge, Anvil, etc.
        [SerializeField] private string stationName = "Crafting Station";

        [Header("Visual")]
        [SerializeField] private GameObject interactionPrompt;
        [SerializeField] private ParticleSystem activeEffect;

        [Header("Range")]
        [SerializeField] private float interactionRange = 3f;

        private bool isPlayerNearby = false;
        private Player.PlayerController nearbyPlayer = null;

        public string StationType => stationType;
        public string StationName => stationName;

        private void Update()
        {
            // Check for nearby players
            CheckForPlayers();
        }

        private void CheckForPlayers()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
            bool foundPlayer = false;

            foreach (Collider col in colliders)
            {
                Player.PlayerController player = col.GetComponent<Player.PlayerController>();
                if (player != null)
                {
                    foundPlayer = true;
                    nearbyPlayer = player;
                    break;
                }
            }

            if (!foundPlayer)
            {
                nearbyPlayer = null;
            }

            // Update visual state
            if (foundPlayer != isPlayerNearby)
            {
                isPlayerNearby = foundPlayer;
                UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(isPlayerNearby);
            }

            if (activeEffect != null)
            {
                if (isPlayerNearby && !activeEffect.isPlaying)
                {
                    activeEffect.Play();
                }
                else if (!isPlayerNearby && activeEffect.isPlaying)
                {
                    activeEffect.Stop();
                }
            }
        }

        public void Interact(Player.PlayerController player)
        {
            // Open crafting UI
            var craftingUI = FindObjectOfType<UI.CraftingUI>();
            if (craftingUI != null)
            {
                craftingUI.OpenCraftingMenu(this);
            }
            else
            {
                Debug.LogWarning("CraftingUI not found! Cannot open crafting menu.");
            }
        }

        /// <summary>
        /// Check if this station can craft a specific recipe
        /// </summary>
        public bool CanCraftRecipe(CraftingRecipe recipe)
        {
            if (recipe == null) return false;

            // Check if recipe requires a crafting station
            if (recipe.requiresCraftingStation)
            {
                return recipe.requiredStationType == stationType;
            }

            // Recipes that don't require a station can be crafted anywhere
            return true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}

