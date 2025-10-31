using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LostHorizon.UI
{
    /// <summary>
    /// UI for displaying and managing player inventory
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject itemSlotPrefab;
        [SerializeField] private Button closeButton;

        [Header("Item Display")]
        [SerializeField] private TextMeshProUGUI selectedItemName;
        [SerializeField] private TextMeshProUGUI selectedItemCount;

        private Player.PlayerInventory playerInventory;
        private Dictionary<Resources.ResourceType, InventorySlotUI> slotUIs = new Dictionary<Resources.ResourceType, InventorySlotUI>();
        private bool isOpen = false;

        private void Start()
        {
            playerInventory = FindObjectOfType<Player.PlayerInventory>();

            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged += RefreshInventory;
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseInventory);
            }

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }
        }

        private void Update()
        {
            // Toggle inventory with I key
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }

            // Close with Escape
            if (isOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInventory();
            }
        }

        public void ToggleInventory()
        {
            if (isOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        public void OpenInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
                isOpen = true;
                RefreshInventory();
                Time.timeScale = 0f; // Pause game
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void CloseInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
                isOpen = false;
                Time.timeScale = 1f; // Resume game
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void RefreshInventory()
        {
            if (playerInventory == null || itemContainer == null) return;

            // Clear existing slots
            foreach (Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }
            slotUIs.Clear();

            // Create slots for each resource type
            var allResources = playerInventory.GetAllResources();
            foreach (var resource in allResources)
            {
                CreateSlot(resource.Key, resource.Value);
            }
        }

        private void CreateSlot(Resources.ResourceType resourceType, int amount)
        {
            if (itemSlotPrefab == null || itemContainer == null) return;

            GameObject slotObj = Instantiate(itemSlotPrefab, itemContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

            if (slotUI == null)
            {
                slotUI = slotObj.AddComponent<InventorySlotUI>();
            }

            slotUI.Initialize(resourceType, amount);
            slotUIs[resourceType] = slotUI;
        }

        private void OnDestroy()
        {
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged -= RefreshInventory;
            }
        }
    }

    /// <summary>
    /// UI component for individual inventory slots
    /// </summary>
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Button slotButton;

        private Resources.ResourceType resourceType;

        public void Initialize(Resources.ResourceType type, int count)
        {
            resourceType = type;
            
            if (nameText != null)
            {
                nameText.text = type.ToString();
            }

            if (countText != null)
            {
                countText.text = count.ToString();
            }

            // Load icon if available (would be in Resources folder)
            // if (iconImage != null) { ... }

            if (slotButton != null)
            {
                slotButton.onClick.AddListener(OnSlotClicked);
            }
        }

        public void UpdateCount(int count)
        {
            if (countText != null)
            {
                countText.text = count.ToString();
            }
        }

        private void OnSlotClicked()
        {
            // Handle slot click (show details, use item, etc.)
            Debug.Log($"Clicked on {resourceType}");
        }
    }
}

