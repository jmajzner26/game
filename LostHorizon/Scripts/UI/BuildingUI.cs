using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LostHorizon.UI
{
    /// <summary>
    /// UI for the building system
    /// </summary>
    public class BuildingUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject buildingPanel;
        [SerializeField] private Transform buildingListContainer;
        [SerializeField] private GameObject buildingButtonPrefab;
        [SerializeField] private Button closeButton;

        [Header("Building Info")]
        [SerializeField] private TextMeshProUGUI buildingNameText;
        [SerializeField] private TextMeshProUGUI buildingDescriptionText;
        [SerializeField] private Transform requirementContainer;
        [SerializeField] private GameObject requirementSlotPrefab;

        private Building.BuildingSystem buildingSystem;
        private Player.PlayerInventory playerInventory;
        private Building.BuildableObject selectedBuilding;

        private void Start()
        {
            buildingSystem = Building.BuildingSystem.Instance;
            playerInventory = FindObjectOfType<Player.PlayerInventory>();

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseBuildingMenu);
            }

            if (buildingPanel != null)
            {
                buildingPanel.SetActive(false);
            }
        }

        private void Update()
        {
            // Toggle building menu with B key
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleBuildingMenu();
            }

            if (buildingPanel != null && buildingPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseBuildingMenu();
            }
        }

        public void ToggleBuildingMenu()
        {
            if (buildingPanel != null && buildingPanel.activeSelf)
            {
                CloseBuildingMenu();
            }
            else
            {
                OpenBuildingMenu();
            }
        }

        public void OpenBuildingMenu()
        {
            if (buildingPanel != null)
            {
                buildingPanel.SetActive(true);
                RefreshBuildingList();
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void CloseBuildingMenu()
        {
            if (buildingPanel != null)
            {
                buildingPanel.SetActive(false);
                selectedBuilding = null;
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void RefreshBuildingList()
        {
            if (buildingListContainer == null || buildingSystem == null) return;

            foreach (Transform child in buildingListContainer)
            {
                Destroy(child.gameObject);
            }

            var buildings = buildingSystem.GetAvailableBuildings();
            foreach (var building in buildings)
            {
                CreateBuildingButton(building);
            }
        }

        private void CreateBuildingButton(Building.BuildableObject building)
        {
            if (buildingButtonPrefab == null || buildingListContainer == null) return;

            GameObject buttonObj = Instantiate(buildingButtonPrefab, buildingListContainer);
            Button button = buttonObj.GetComponent<Button>();

            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = building.objectName;
            }

            bool canBuild = HasRequiredResources(building);
            if (button != null)
            {
                button.interactable = canBuild;
                button.onClick.AddListener(() => SelectBuilding(building));
            }
        }

        private bool HasRequiredResources(Building.BuildableObject building)
        {
            if (playerInventory == null) return false;

            var required = building.GetRequiredResources();
            foreach (var req in required)
            {
                if (!playerInventory.HasResource(req.Key, req.Value))
                {
                    return false;
                }
            }
            return true;
        }

        private void SelectBuilding(Building.BuildableObject building)
        {
            selectedBuilding = building;
            DisplayBuildingDetails(building);

            // Start placement mode
            if (buildingSystem != null)
            {
                CloseBuildingMenu();
                buildingSystem.StartBuilding(building);
            }
        }

        private void DisplayBuildingDetails(Building.BuildableObject building)
        {
            if (buildingNameText != null)
            {
                buildingNameText.text = building.objectName;
            }

            if (buildingDescriptionText != null)
            {
                buildingDescriptionText.text = building.description;
            }

            // Display requirements
            if (requirementContainer != null)
            {
                foreach (Transform child in requirementContainer)
                {
                    Destroy(child.gameObject);
                }

                foreach (var ingredient in building.requiredResources)
                {
                    GameObject reqObj = Instantiate(requirementSlotPrefab, requirementContainer);
                    TextMeshProUGUI text = reqObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null)
                    {
                        int currentAmount = playerInventory != null ? playerInventory.GetResourceCount(ingredient.resourceType) : 0;
                        bool hasEnough = currentAmount >= ingredient.amount;
                        text.text = $"{ingredient.resourceType}: {currentAmount}/{ingredient.amount}";
                        text.color = hasEnough ? Color.white : Color.red;
                    }
                }
            }
        }
    }
}

