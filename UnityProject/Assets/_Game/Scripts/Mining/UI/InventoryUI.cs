using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventorySlotContainer;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private Button sellAllButton;
    
    private Inventory inventory;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    
    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventory);
        
        if (sellAllButton != null)
            sellAllButton.onClick.AddListener(SellAllResources);
        
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }
    
    private void Start()
    {
        if (inventory != null)
        {
            inventory.OnSlotChanged += OnSlotChanged;
            PopulateInventorySlots();
        }
    }
    
    public void OpenInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            UpdateInventoryDisplay();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    public void ToggleInventory()
    {
        if (inventoryPanel != null && inventoryPanel.activeSelf)
            CloseInventory();
        else
            OpenInventory();
    }
    
    private void PopulateInventorySlots()
    {
        if (inventory == null || inventorySlotContainer == null || inventorySlotPrefab == null) return;
        
        // Clear existing slots
        foreach (var slotUI in slotUIs)
        {
            if (slotUI != null)
                Destroy(slotUI.gameObject);
        }
        slotUIs.Clear();
        
        // Create slots
        var slots = inventory.GetSlots();
        foreach (var slot in slots)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventorySlotContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            
            if (slotUI != null)
            {
                slotUI.Initialize(slot, this);
                slotUIs.Add(slotUI);
            }
        }
    }
    
    private void OnSlotChanged(InventorySlot slot)
    {
        UpdateInventoryDisplay();
    }
    
    private void UpdateInventoryDisplay()
    {
        if (inventory == null) return;
        
        if (capacityText != null)
        {
            capacityText.text = $"{inventory.UsedCapacity}/{inventory.CurrentCapacity}";
        }
        
        // Update slot UIs
        var slots = inventory.GetSlots();
        for (int i = 0; i < slotUIs.Count && i < slots.Count; i++)
        {
            if (slotUIs[i] != null)
                slotUIs[i].UpdateDisplay(slots[i]);
        }
    }
    
    public void SellResource(ResourceType resource, int amount)
    {
        if (EconomyManager.Instance == null || inventory == null) return;
        
        int available = inventory.GetResourceCount(resource);
        int toSell = Mathf.Min(amount, available);
        
        if (toSell > 0)
        {
            EconomyManager.Instance.SellResource(resource, toSell);
            inventory.RemoveResource(resource, toSell);
        }
    }
    
    private void SellAllResources()
    {
        if (EconomyManager.Instance == null || inventory == null) return;
        
        var slots = inventory.GetSlots();
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.resource != null)
            {
                EconomyManager.Instance.SellResource(slot.resource, slot.quantity);
                inventory.RemoveResource(slot.resource, slot.quantity);
            }
        }
    }
    
    private void Update()
    {
        // Toggle inventory with 'I' key
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }
    
    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnSlotChanged -= OnSlotChanged;
        }
    }
}
