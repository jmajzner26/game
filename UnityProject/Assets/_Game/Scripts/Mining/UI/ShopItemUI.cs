using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button itemButton;
    
    private ShopItem shopItem;
    private ShopUI shopUI;
    
    public void Initialize(ShopItem item, ShopUI shop)
    {
        shopItem = item;
        shopUI = shop;
        
        if (itemButton != null)
            itemButton.onClick.AddListener(() => shopUI.SelectItem(shopItem));
        
        UpdateDisplay();
    }
    
    public void UpdateDisplay()
    {
        if (shopItem == null) return;
        
        if (nameText != null)
            nameText.text = shopItem.displayName;
        
        if (priceText != null && EquipmentShop.Instance != null)
        {
            float price = EquipmentShop.Instance.GetItemPrice(shopItem.id);
            bool canAfford = GameManager.Instance != null && GameManager.Instance.Money >= price;
            priceText.text = $"${price:F2}";
            priceText.color = canAfford ? Color.white : Color.red;
        }
        
        if (iconImage != null && shopItem.tool != null && shopItem.tool.icon != null)
            iconImage.sprite = shopItem.tool.icon;
    }
}
