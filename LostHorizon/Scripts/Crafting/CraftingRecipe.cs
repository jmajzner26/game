using System.Collections.Generic;
using UnityEngine;

namespace LostHorizon.Crafting
{
    /// <summary>
    /// ScriptableObject defining a crafting recipe
    /// </summary>
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Lost Horizon/Crafting/Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        [Header("Recipe Info")]
        public string recipeName;
        [TextArea(3, 5)]
        public string description;

        [Header("Ingredients")]
        public List<RecipeIngredient> ingredients = new List<RecipeIngredient>();

        [Header("Result")]
        public RecipeOutput output;

        [Header("Requirements")]
        public bool requiresCraftingStation = false;
        public string requiredStationType = ""; // e.g., "Workbench", "Forge"

        [Header("Crafting Time")]
        public float craftingTime = 2f;

        /// <summary>
        /// Check if player has all required ingredients
        /// </summary>
        public bool CanCraft(Player.PlayerInventory inventory)
        {
            foreach (var ingredient in ingredients)
            {
                if (!inventory.HasResource(ingredient.resourceType, ingredient.amount))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get all required ingredients as a dictionary
        /// </summary>
        public Dictionary<Resources.ResourceType, int> GetIngredientDictionary()
        {
            Dictionary<Resources.ResourceType, int> result = new Dictionary<Resources.ResourceType, int>();
            foreach (var ingredient in ingredients)
            {
                if (result.ContainsKey(ingredient.resourceType))
                {
                    result[ingredient.resourceType] += ingredient.amount;
                }
                else
                {
                    result[ingredient.resourceType] = ingredient.amount;
                }
            }
            return result;
        }
    }

    [System.Serializable]
    public class RecipeIngredient
    {
        public Resources.ResourceType resourceType;
        public int amount = 1;
    }

    [System.Serializable]
    public class RecipeOutput
    {
        public Resources.ResourceType resourceType;
        public int amount = 1;
        // Could also be an EquipmentItem or other item type
    }
}

