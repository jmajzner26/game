using System.Collections.Generic;
using UnityEngine;

namespace LostHorizon.Crafting
{
    /// <summary>
    /// Manages the crafting system and available recipes
    /// </summary>
    public class CraftingSystem : MonoBehaviour
    {
        public static CraftingSystem Instance { get; private set; }

        [Header("Recipes")]
        [SerializeField] private List<CraftingRecipe> availableRecipes = new List<CraftingRecipe>();

        [Header("Component References")]
        [SerializeField] private Player.PlayerInventory playerInventory;

        private Dictionary<string, CraftingRecipe> recipeDictionary = new Dictionary<string, CraftingRecipe>();

        // Events
        public System.Action<CraftingRecipe> OnRecipeCrafted;
        public System.Action OnCraftingStarted;
        public System.Action OnCraftingCompleted;

        public void Initialize()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (playerInventory == null)
            {
                playerInventory = FindObjectOfType<Player.PlayerInventory>();
            }

            // Build recipe dictionary
            foreach (var recipe in availableRecipes)
            {
                if (recipe != null && !string.IsNullOrEmpty(recipe.recipeName))
                {
                    recipeDictionary[recipe.recipeName] = recipe;
                }
            }

            Debug.Log($"Crafting System initialized with {recipeDictionary.Count} recipes");
        }

        /// <summary>
        /// Get all available recipes
        /// </summary>
        public List<CraftingRecipe> GetAvailableRecipes()
        {
            return new List<CraftingRecipe>(availableRecipes);
        }

        /// <summary>
        /// Get recipes that can be crafted with current inventory
        /// </summary>
        public List<CraftingRecipe> GetCraftableRecipes()
        {
            List<CraftingRecipe> craftable = new List<CraftingRecipe>();

            foreach (var recipe in availableRecipes)
            {
                if (recipe != null && recipe.CanCraft(playerInventory))
                {
                    craftable.Add(recipe);
                }
            }

            return craftable;
        }

        /// <summary>
        /// Attempt to craft an item from a recipe
        /// </summary>
        public bool CraftRecipe(CraftingRecipe recipe)
        {
            if (recipe == null)
            {
                Debug.LogWarning("Cannot craft: Recipe is null");
                return false;
            }

            if (playerInventory == null)
            {
                Debug.LogWarning("Cannot craft: Player inventory not found");
                return false;
            }

            if (!recipe.CanCraft(playerInventory))
            {
                Debug.Log($"Cannot craft {recipe.recipeName}: Missing ingredients");
                return false;
            }

            // Remove ingredients
            Dictionary<Resources.ResourceType, int> ingredients = recipe.GetIngredientDictionary();
            if (!playerInventory.RemoveResources(ingredients))
            {
                Debug.LogWarning($"Failed to remove ingredients for {recipe.recipeName}");
                return false;
            }

            // Add result
            playerInventory.AddResource(recipe.output.resourceType, recipe.output.amount);

            OnRecipeCrafted?.Invoke(recipe);
            Debug.Log($"Crafted {recipe.output.amount}x {recipe.output.resourceType} from {recipe.recipeName}");

            return true;
        }

        /// <summary>
        /// Craft recipe by name
        /// </summary>
        public bool CraftRecipe(string recipeName)
        {
            if (recipeDictionary.ContainsKey(recipeName))
            {
                return CraftRecipe(recipeDictionary[recipeName]);
            }

            Debug.LogWarning($"Recipe not found: {recipeName}");
            return false;
        }

        /// <summary>
        /// Add a recipe to available recipes
        /// </summary>
        public void AddRecipe(CraftingRecipe recipe)
        {
            if (recipe != null && !availableRecipes.Contains(recipe))
            {
                availableRecipes.Add(recipe);
                if (!string.IsNullOrEmpty(recipe.recipeName))
                {
                    recipeDictionary[recipe.recipeName] = recipe;
                }
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}

