using UnityEngine;

namespace LostHorizon.Building
{
    /// <summary>
    /// Defines a buildable structure/object
    /// </summary>
    [CreateAssetMenu(fileName = "New Buildable", menuName = "Lost Horizon/Building/Buildable Object")]
    public class BuildableObject : ScriptableObject
    {
        [Header("Object Info")]
        public string objectName;
        [TextArea(3, 5)]
        public string description;

        [Header("Prefab")]
        public GameObject prefab;

        [Header("Build Requirements")]
        public System.Collections.Generic.List<Crafting.RecipeIngredient> requiredResources;

        [Header("Building Settings")]
        public bool requiresFoundation = true;
        public bool canSnapToOthers = false;
        public Vector3 snapOffset = Vector3.zero;

        [Header("Size")]
        public Vector3 size = Vector3.one;

        [Header("Placement Rules")]
        public bool canPlaceOnWater = false;
        public bool canPlaceOnSlopes = true;
        public float maxSlopeAngle = 45f;

        /// <summary>
        /// Get required resources as dictionary
        /// </summary>
        public System.Collections.Generic.Dictionary<Resources.ResourceType, int> GetRequiredResources()
        {
            var result = new System.Collections.Generic.Dictionary<Resources.ResourceType, int>();
            foreach (var ingredient in requiredResources)
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
}

