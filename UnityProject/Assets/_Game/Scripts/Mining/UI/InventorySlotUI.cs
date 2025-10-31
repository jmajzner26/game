using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button slotButton;
    [SerializeField] private Button sellButton;
    
    private InventorySlot slot;
    private InventoryUI inventoryUI;
    
    public void Initialize(InventorySlot slot, InventoryUI inventoryUI)
    {
        this.slot = slot;
        this.inventoryUI = inventoryUI;
        
        if (slotButton != null)
            slotButton.onClick.AddListener(OnSlotClicked);
        
        if (sellButton != null)
            sellButton.onClick.AddListener(OnSellClicked);
        
        UpdateDisplay(slot);
    }
    
    public void UpdateDisplay(InventorySlot newSlot)
    {
        slot = newSlot;
        
        bool hasItem = !slot.IsEmpty;
        
        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(hasItem);
            if (hasItem && slot.resource != null && slot.resource.icon != null)
            {
                iconImage.sprite = slot.resource.icon;
            }
        }
        
        if (quantityText != null)
        {
            quantityText.gameObject.SetActive(hasItem);
            if (hasItem)
            {
                quantityText.text = slot.quantity.ToString();
            }
        }
        
        if (sellButton != null)
        {
            sellButton.gameObject.SetActive(hasItem);
        }
    }
    
    private void OnSlotClicked()
    {
        // Could show item details or options
    }
    
    private void OnSellClicked()
    {
        if (!slot.IsEmpty && slot.resource != null && inventoryUI != null)
        {
            inventoryUI.SellResource(slot.resource, slot.quantity);
        }
    }
}
