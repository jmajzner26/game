using System.Collections.Generic;
using UnityEngine;

namespace LostHorizon.Player
{
    /// <summary>
    /// Manages player inventory and resource storage
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private int maxSlots = 30;
        [SerializeField] private int maxStackSize = 99;

        private Dictionary<Resources.ResourceType, int> resources = new Dictionary<Resources.ResourceType, int>();

        // Events
        public System.Action<Resources.ResourceType, int> OnResourceChanged;
        public System.Action OnInventoryChanged;

        public int MaxSlots => maxSlots;
        public int CurrentItemCount => resources.Count;

        /// <summary>
        /// Add resources to inventory
        /// </summary>
        public bool AddResource(Resources.ResourceType type, int amount)
        {
            if (amount <= 0) return false;

            if (resources.ContainsKey(type))
            {
                resources[type] += amount;
            }
            else
            {
                resources[type] = amount;
            }

            // Enforce stack size limit
            if (resources[type] > maxStackSize)
            {
                resources[type] = maxStackSize;
            }

            OnResourceChanged?.Invoke(type, resources[type]);
            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Remove resources from inventory
        /// </summary>
        public bool RemoveResource(Resources.ResourceType type, int amount)
        {
            if (!resources.ContainsKey(type) || resources[type] < amount)
            {
                return false;
            }

            resources[type] -= amount;

            if (resources[type] <= 0)
            {
                resources.Remove(type);
            }

            OnResourceChanged?.Invoke(type, resources.ContainsKey(type) ? resources[type] : 0);
            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Get the amount of a specific resource
        /// </summary>
        public int GetResourceCount(Resources.ResourceType type)
        {
            return resources.ContainsKey(type) ? resources[type] : 0;
        }

        /// <summary>
        /// Check if player has enough of a resource
        /// </summary>
        public bool HasResource(Resources.ResourceType type, int amount)
        {
            return GetResourceCount(type) >= amount;
        }

        /// <summary>
        /// Check if player has all required resources
        /// </summary>
        public bool HasResources(Dictionary<Resources.ResourceType, int> requiredResources)
        {
            foreach (var requirement in requiredResources)
            {
                if (!HasResource(requirement.Key, requirement.Value))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Remove multiple resources at once
        /// </summary>
        public bool RemoveResources(Dictionary<Resources.ResourceType, int> resourcesToRemove)
        {
            // First check if we have all resources
            if (!HasResources(resourcesToRemove))
            {
                return false;
            }

            // Then remove them
            foreach (var resource in resourcesToRemove)
            {
                RemoveResource(resource.Key, resource.Value);
            }

            return true;
        }

        /// <summary>
        /// Get all resources in inventory
        /// </summary>
        public Dictionary<Resources.ResourceType, int> GetAllResources()
        {
            return new Dictionary<Resources.ResourceType, int>(resources);
        }

        /// <summary>
        /// Clear entire inventory
        /// </summary>
        public void ClearInventory()
        {
            resources.Clear();
            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Get total item count across all stacks
        /// </summary>
        public int GetTotalItemCount()
        {
            int total = 0;
            foreach (var resource in resources.Values)
            {
                total += resource;
            }
            return total;
        }
    }
}

