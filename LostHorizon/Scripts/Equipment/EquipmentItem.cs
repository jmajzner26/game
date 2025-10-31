using UnityEngine;

namespace LostHorizon.Equipment
{
    /// <summary>
    /// ScriptableObject defining an equipment item
    /// </summary>
    [CreateAssetMenu(fileName = "New Equipment", menuName = "Lost Horizon/Equipment/Equipment Item")]
    public class EquipmentItem : ScriptableObject
    {
        [Header("Equipment Info")]
        public string itemName;
        [TextArea(3, 5)]
        public string description;

        [Header("Equipment Type")]
        public EquipmentType equipmentType = EquipmentType.Hand;

        [Header("Stats")]
        public float durability = 100f;
        public float maxDurability = 100f;
        public float damageMultiplier = 1f;
        public float harvestSpeed = 1f;
        public float efficiency = 1f; // Resource yield multiplier

        [Header("Tier/Level")]
        public int tier = 1; // Higher tier = better stats

        [Header("Requirements")]
        public bool requiresResources = true;
        public System.Collections.Generic.List<Crafting.RecipeIngredient> requiredResources;

        [Header("Visual")]
        public GameObject modelPrefab;
        public Sprite icon;

        /// <summary>
        /// Use the equipment (decreases durability)
        /// </summary>
        public void Use(float durabilityCost = 1f)
        {
            durability = Mathf.Max(0f, durability - durabilityCost);
        }

        /// <summary>
        /// Repair the equipment
        /// </summary>
        public void Repair(float amount)
        {
            durability = Mathf.Min(maxDurability, durability + amount);
        }

        /// <summary>
        /// Check if equipment is broken
        /// </summary>
        public bool IsBroken()
        {
            return durability <= 0f;
        }

        /// <summary>
        /// Get durability percentage
        /// </summary>
        public float GetDurabilityPercent()
        {
            return durability / maxDurability;
        }
    }
}

