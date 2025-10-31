using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiningHUD : MonoBehaviour
{
    [Header("Money Display")]
    [SerializeField] private TextMeshProUGUI moneyText;
    
    [Header("Energy Bar")]
    [SerializeField] private Slider energySlider;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private Image energyFill;
    [SerializeField] private Color energyLowColor = Color.red;
    [SerializeField] private Color energyHighColor = Color.green;
    
    [Header("Depth Display")]
    [SerializeField] private TextMeshProUGUI depthText;
    
    [Header("Inventory")]
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField] private GameObject inventoryFullWarning;
    
    [Header("Mining Progress")]
    [SerializeField] private Slider miningProgressSlider;
    [SerializeField] private GameObject miningProgressPanel;
    
    private PlayerEnergy energySystem;
    private Inventory inventory;
    private MiningController miningController;
    
    private void Start()
    {
        energySystem = FindObjectOfType<PlayerEnergy>();
        inventory = FindObjectOfType<Inventory>();
        miningController = FindObjectOfType<MiningController>();
        
        // Subscribe to events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMoneyChanged += UpdateMoneyDisplay;
            GameManager.Instance.OnDepthChanged += UpdateDepthDisplay;
        }
        
        if (energySystem != null)
        {
            energySystem.OnEnergyChanged += UpdateEnergyDisplay;
        }
        
        if (inventory != null)
        {
            inventory.OnInventoryFull += OnInventoryFull;
        }
        
        if (miningController != null && miningController.CurrentTarget != null)
        {
            miningController.CurrentTarget.OnMiningProgressChanged += UpdateMiningProgress;
        }
        
        UpdateAllDisplays();
    }
    
    private void Update()
    {
        UpdateInventoryDisplay();
        UpdateMiningProgress();
    }
    
    private void UpdateAllDisplays()
    {
        UpdateMoneyDisplay(GameManager.Instance?.Money ?? 0f);
        UpdateDepthDisplay(GameManager.Instance?.Depth ?? 0);
        if (energySystem != null)
            UpdateEnergyDisplay(energySystem.CurrentEnergy);
    }
    
    private void UpdateMoneyDisplay(float money)
    {
        if (moneyText != null)
        {
            moneyText.text = $"${money:F2}";
        }
    }
    
    private void UpdateEnergyDisplay(float energy)
    {
        if (energySystem == null) return;
        
        if (energySlider != null)
        {
            energySlider.value = energySystem.EnergyPercentage;
        }
        
        if (energyText != null)
        {
            energyText.text = $"{energy:F0}/{energySystem.MaxEnergy:F0}";
        }
        
        if (energyFill != null)
        {
            float percentage = energySystem.EnergyPercentage;
            energyFill.color = Color.Lerp(energyLowColor, energyHighColor, percentage);
        }
    }
    
    private void UpdateDepthDisplay(int depth)
    {
        if (depthText != null)
        {
            depthText.text = $"Depth: {depth}m";
        }
    }
    
    private void UpdateInventoryDisplay()
    {
        if (inventory == null || inventoryText == null) return;
        
        int used = inventory.UsedCapacity;
        int total = inventory.CurrentCapacity;
        inventoryText.text = $"Inventory: {used}/{total}";
    }
    
    private void OnInventoryFull()
    {
        if (inventoryFullWarning != null)
        {
            inventoryFullWarning.SetActive(true);
            Invoke(nameof(HideInventoryWarning), 3f);
        }
    }
    
    private void HideInventoryWarning()
    {
        if (inventoryFullWarning != null)
        {
            inventoryFullWarning.SetActive(false);
        }
    }
    
    private void UpdateMiningProgress()
    {
        if (miningController == null || miningProgressPanel == null) return;
        
        var target = miningController.CurrentTarget;
        bool isMining = target != null;
        
        miningProgressPanel.SetActive(isMining);
        
        if (isMining && miningProgressSlider != null)
        {
            miningProgressSlider.value = target.MiningProgress;
        }
    }
    
    private void UpdateMiningProgress(MineableBlock block, float progress)
    {
        if (miningProgressSlider != null && block == miningController?.CurrentTarget)
        {
            miningProgressSlider.value = progress;
        }
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMoneyChanged -= UpdateMoneyDisplay;
            GameManager.Instance.OnDepthChanged -= UpdateDepthDisplay;
        }
        
        if (energySystem != null)
        {
            energySystem.OnEnergyChanged -= UpdateEnergyDisplay;
        }
        
        if (inventory != null)
        {
            inventory.OnInventoryFull -= OnInventoryFull;
        }
    }
}
