using System.Collections.Generic;
using UnityEngine;

namespace LostHorizon.Building
{
    /// <summary>
    /// Manages building placement and construction in the world
    /// </summary>
    public class BuildingSystem : MonoBehaviour
    {
        public static BuildingSystem Instance { get; private set; }

        [Header("Building Settings")]
        [SerializeField] private Transform buildingParent;
        [SerializeField] private List<BuildableObject> availableBuildings = new List<BuildableObject>();

        [Header("Component References")]
        [SerializeField] private BuildingPlacement placementController;
        [SerializeField] private Player.PlayerInventory playerInventory;

        private List<PlacedBuilding> placedBuildings = new List<PlacedBuilding>();

        public void Initialize()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (placementController == null)
                placementController = GetComponent<BuildingPlacement>();
            if (placementController == null)
                placementController = gameObject.AddComponent<BuildingPlacement>();

            if (playerInventory == null)
                playerInventory = FindObjectOfType<Player.PlayerInventory>();

            if (buildingParent == null)
            {
                GameObject parent = new GameObject("Buildings");
                buildingParent = parent.transform;
            }
        }

        /// <summary>
        /// Start building placement mode
        /// </summary>
        public void StartBuilding(BuildableObject buildable)
        {
            if (buildable == null)
            {
                Debug.LogWarning("Cannot start building: Buildable object is null");
                return;
            }

            if (placementController != null)
            {
                placementController.StartPlacement(buildable);
            }
        }

        /// <summary>
        /// Place a building at the specified position
        /// </summary>
        public bool PlaceBuilding(BuildableObject buildable, Vector3 position, Quaternion rotation)
        {
            if (buildable == null || buildable.prefab == null)
            {
                Debug.LogWarning("Cannot place building: Invalid buildable object");
                return false;
            }

            // Check if player has required resources
            var requiredResources = buildable.GetRequiredResources();
            if (playerInventory != null && !playerInventory.HasResources(requiredResources))
            {
                Debug.Log($"Cannot place {buildable.objectName}: Missing required resources");
                return false;
            }

            // Remove resources
            if (playerInventory != null)
            {
                playerInventory.RemoveResources(requiredResources);
            }

            // Instantiate building
            GameObject buildingInstance = Instantiate(buildable.prefab, position, rotation, buildingParent);
            buildingInstance.name = buildable.objectName;

            // Register building
            PlacedBuilding placedBuilding = new PlacedBuilding
            {
                buildingObject = buildingInstance,
                buildableData = buildable,
                position = position,
                rotation = rotation
            };
            placedBuildings.Add(placedBuilding);

            Debug.Log($"Placed {buildable.objectName} at {position}");
            return true;
        }

        /// <summary>
        /// Remove/destroy a building
        /// </summary>
        public bool RemoveBuilding(GameObject buildingObject)
        {
            PlacedBuilding building = placedBuildings.Find(b => b.buildingObject == buildingObject);
            if (building == null)
            {
                return false;
            }

            // Optionally refund some resources (50% for example)
            // This would be implemented based on game design

            placedBuildings.Remove(building);
            Destroy(buildingObject);
            return true;
        }

        /// <summary>
        /// Get all available buildings
        /// </summary>
        public List<BuildableObject> GetAvailableBuildings()
        {
            return new List<BuildableObject>(availableBuildings);
        }

        /// <summary>
        /// Get buildings that can be built with current resources
        /// </summary>
        public List<BuildableObject> GetBuildableObjects()
        {
            List<BuildableObject> buildable = new List<BuildableObject>();

            foreach (var building in availableBuildings)
            {
                var requiredResources = building.GetRequiredResources();
                if (playerInventory != null && playerInventory.HasResources(requiredResources))
                {
                    buildable.Add(building);
                }
            }

            return buildable;
        }

        /// <summary>
        /// Update - handle input for placing buildings
        /// </summary>
        private void Update()
        {
            if (placementController != null && placementController.IsPlacing)
            {
                // Check for placement input (left click typically)
                if (Input.GetMouseButtonDown(0))
                {
                    placementController.TryPlace();
                }

                // Cancel placement (right click or escape)
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    placementController.CancelPlacement();
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

    [System.Serializable]
    public class PlacedBuilding
    {
        public GameObject buildingObject;
        public BuildableObject buildableData;
        public Vector3 position;
        public Quaternion rotation;
    }
}

