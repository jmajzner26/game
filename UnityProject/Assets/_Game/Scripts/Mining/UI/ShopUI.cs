using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform shopItemContainer;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private Button closeButton;
    
    [Header("Item Display")]
    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemDescription;
    [SerializeField] private TextMeshProUGUI selectedItemPrice;
    [SerializeField] private Button purchaseButton;
    
    private EquipmentShop shop;
    private List<ShopItemUI> itemUIElements = new List<ShopItemUI>();
    private ShopItem selectedItem;
    
    private void Awake()
    {
        shop = FindObjectOfType<EquipmentShop>();
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);
        
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(PurchaseSelectedItem);
        
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }
    
    private void Start()
    {
        PopulateShopItems();
    }
    
    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            UpdateShopDisplay();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    public void ToggleShop()
    {
        if (shopPanel != null && shopPanel.activeSelf)
            CloseShop();
        else
            OpenShop();
    }
    
    private void PopulateShopItems()
    {
        if (shop == null || shopItemContainer == null || shopItemPrefab == null) return;
        
        // Clear existing items
        foreach (var item in item创建的UIElements)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        item创建的UIElements.Clear();
        
        // Create items
        var items = shop.GetAvailableItems();
        foreach (var item in items)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, shopItemContainer);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(item, this);
                itemUIElements.Add(itemUI);
            }
        }
    }
    
    public void SelectItem(ShopItem item)
    {
        selectedItem = item;
        UpdateSelectedItemDisplay();
    }
    
    private void UpdateSelectedItemDisplay()
    {
        if (selectedItem == null) return;
        
        if (selectedItemName != null)
            selectedItemName.text = selectedItem.displayName;
        
        if (selectedItemDescription != null)
            selectedItemDescription.text = selectedItem.description;
        
        if (selectedItemPrice != null)
        {
            float price = shop.GetItemPrice(selectedItem.id);
            bool canAfford = GameManager.Instance != null && GameManager.Instance.Money >= price;
            selectedItemPrice.text = $"${price:F2}";
            selectedItemPrice.color = canAfford ? Color.green : Color.red;
        }
        
        if (purchaseButton != null)
        {
            float price = shop.GetItemPrice(selectedItem.id);
            bool canAfford = GameManager.Instance != null && GameManager.Instance.Money >= price;
            purchaseButton.interactable = canAfford;
        }
    }
    
    private void PurchaseSelectedItem()
    {
        if (selectedItem == null || shop == null) return;
        
        if (shop.PurchaseItem(selectedItem.id))
        {
            // Update displays
            UpdateShopDisplay();
            UpdateSelectedItemDisplay();
            
            // Refresh item UIs
            foreach (var itemUI in itemUIElements)
            {
                if (itemUI != null)
                    itemUI.UpdateDisplay();
            }
        }
    }
    
    private void UpdateShopDisplay()
    {
        UpdateSelectedItemDisplay();
    }
    
    private void Update()
    {
        // Toggle shop with 'B' key
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleShop();
        }
    }
}
