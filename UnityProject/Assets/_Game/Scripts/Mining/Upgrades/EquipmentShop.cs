using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShopItem
{
    public string id;
    public string displayName;
    public string description;
    public float price;
    public UpgradeType upgradeType;
    public ToolStats tool; // For tool purchases
    public UpgradeData upgradeData; // For stat upgrades
    
    public enum UpgradeType
    {
        Tool,
        EnergyCapacity,
        EnergyRegen,
        InventoryCapacity,
        InventorySlots,
        MiningRange,
        Speed
    }
}

[System.Serializable]
public class UpgradeData
{
    public float value;
    public float priceMultiplier = 1.5f; // Each upgrade costs more
}

public class EquipmentShop : MonoBehaviour
{
    [Header("Shop Items")]
    [SerializeField] private List<ShopItem> availableItems = new List<ShopItem>();
    
    [Header("Default Tools")]
    [SerializeField] private ToolStats[] availableTools;
    
    public static EquipmentShop Instance { get; private set; }
    
    private Dictionary<string, int> purchaseCount = new Dictionary<string, int>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeShop();
    }
    
    private void InitializeShop()
    {
        // Auto-populate with tools if available
        if (availableTools != null && availableTools.Length > 0)
        {
            foreach (var tool in availableTools)
            {
                if (tool == null) continue;
                
                ShopItem item = new ShopItem
                {
                    id = tool.id,
                    displayName = tool.displayName,
                    description = tool.description,
                    price = CalculateToolPrice(tool),
                    upgradeType = ShopItem.UpgradeType.Tool,
                    tool = tool
                };
                
                if (!availableItems.Exists(i => i.id == item.id))
                {
                    availableItems.Add(item);
                }
            }
        }
    }
    
    public bool PurchaseItem(string itemId)
    {
        ShopItem item = availableItems.Find(i => i.id == itemId);
        if (item == null) return false;
        
        float cost = GetItemPrice(itemId);
        
        if (GameManager.Instance == null || !GameManager.Instance.SpendMoney(cost))
        {
            return false;
        }
        
        // Apply upgrade
        ApplyUpgrade(item);
        
        // Track purchase count for dynamic pricing
        if (!purchaseCount.ContainsKey(itemId))
            purchaseCount[itemId] = 0;
        purchaseCount[itemId]++;
        
        return true;
    }
    
    private void ApplyUpgrade(ShopItem item)
    {
        switch (item.upgradeType)
        {
            case ShopItem.UpgradeType.Tool:
                ApplyToolUpgrade(item.tool);
                break;
                
            case ShopItem.UpgradeType.EnergyCapacity:
                if (item.upgradeData != null)
                {
                    var energy = FindObjectOfType<PlayerEnergy>();
                    if (energy != null)
                    {
                        energy.SetMaxEnergy(energy.MaxEnergy + item.upgradeData.value);
                    }
                }
                break;
                
            case ShopItem.UpgradeType.EnergyRegen:
                if (item.upgradeData != null)
                {
                    var energy = FindObjectOfType<PlayerEnergy>();
                    if (energy != null)
                    {
                        energy.UpgradeEnergy(0f, item.upgradeData.value);
                    }
                }
                break;
                
            case ShopItem.UpgradeType.InventoryCapacity:
                if (item.upgradeData != null)
                {
                    var inventory = GameManager.Instance?.GetPlayerInventory();
                    if (inventory != null)
                    {
                        inventory.SetCapacity(inventory.CurrentCapacity + (int)item.upgradeData.value);
                    }
                }
                break;
                
            case ShopItem.UpgradeType.InventorySlots:
                if (item.upgradeData != null)
                {
                    var inventory = GameManager.Instance?.GetPlayerInventory();
                    if (inventory != null)
                    {
                        inventory.ExpandSlots((int)item.upgradeData.value);
                    }
                }
                break;
                
            case ShopItem.UpgradeType.MiningRange:
                var miningController = FindObjectOfType<MiningController>();
                if (miningController != null && item.upgradeData != null)
                {
                    // This would require a SetMiningRange method on MiningController
                    // For now, we'll handle it through tool upgrades
                }
                break;
        }
    }
    
    private void ApplyToolUpgrade(ToolStats tool)
    {
        if (tool == null) return;
        
        var miningController = FindObjectOfType<MiningController>();
        if (miningController != null)
        {
            miningController.SetTool(tool);
        }
    }
    
    public float GetItemPrice(string itemId)
    {
        ShopItem item = availableItems.Find(i => i.id == itemId);
        if (item == null) return 0f;
        
        int purchases = purchaseCount.ContainsKey(itemId) ? purchaseCount[itemId] : 0;
        
        if (item.upgradeType == ShopItem.UpgradeType.Tool)
        {
            // One-time purchase
            return item.price;
        }
        else
        {
            // Progressive pricing
            float basePrice = item.price;
            float multiplier = item.upgradeData != null ? item.upgradeData.priceMultiplier : 1.5f;
            return basePrice * Mathf.Pow(multiplier, purchases);
        }
    }
    
    private float CalculateToolPrice(ToolStats tool)
    {
        // Price based on tool stats
        float basePrice = 100f;
        float powerMultiplier = tool.miningPower * 50f;
        float speedMultiplier = tool.miningSpeed * 30f;
        return basePrice + powerMultiplier + speedMultiplier;
    }
    
    public List<ShopItem> GetAvailableItems()
    {
        return new List<ShopItem>(availableItems);
    }
    
    public void AddShopItem(ShopItem item)
    {
        if (!availableItems.Exists(i => i.id == item.id))
        {
            availableItems.Add(item);
        }
    }
}
