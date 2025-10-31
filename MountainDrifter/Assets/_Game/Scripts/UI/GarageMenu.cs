using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Garage menu for car selection, purchase, and preview.
/// </summary>
public class GarageMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform carListContainer;
    [SerializeField] private GameObject carItemPrefab;
    [SerializeField] private TextMeshProUGUI carNameText;
    [SerializeField] private TextMeshProUGUI carDescriptionText;
    [SerializeField] private TextMeshProUGUI carPriceText;
    [SerializeField] private Image carIcon;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button customizeButton;

    [Header("Car Preview")]
    [SerializeField] private Transform carPreviewPosition;
    [SerializeField] private Camera previewCamera;
    [SerializeField] private float rotationSpeed = 30f;

    private List<CarData> availableCars = new List<CarData>();
    private CarData selectedCar;
    private GameObject previewCarInstance;
    private ProgressionManager progressionManager;

    private void Start()
    {
        progressionManager = GameManager.Instance?.Progression;

        // Load available cars (could be from Resources or ScriptableObjects)
        LoadAvailableCars();

        // Populate car list
        PopulateCarList();

        // Setup buttons
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        
        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelectClicked);
        
        if (customizeButton != null)
            customizeButton.onClick.AddListener(OnCustomizeClicked);
    }

    private void Update()
    {
        // Rotate preview car
        if (previewCarInstance != null)
        {
            previewCarInstance.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
    }

    private void LoadAvailableCars()
    {
        // Load all CarData ScriptableObjects from Resources
        CarData[] cars = Resources.LoadAll<CarData>("CarData");
        availableCars.AddRange(cars);
    }

    private void PopulateCarList()
    {
        if (carListContainer == null || carItemPrefab == null)
            return;

        foreach (var car in availableCars)
        {
            GameObject item = Instantiate(carItemPrefab, carListContainer);
            
            // Setup car item UI
            CarListItemUI itemUI = item.GetComponent<CarListItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(car, OnCarSelected);
            }
            else
            {
                // Fallback: setup manually
                Button button = item.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnCarSelected(car));
                }

                TextMeshProUGUI nameText = item.GetComponentInChildren<TextMeshProUGUI>();
                if (nameText != null)
                {
                    nameText.text = car.carName;
                }
            }
        }
    }

    private void OnCarSelected(CarData car)
    {
        selectedCar = car;
        UpdateCarDisplay();
        UpdateButtons();
        SpawnPreviewCar(car);
    }

    private void UpdateCarDisplay()
    {
        if (selectedCar == null) return;

        if (carNameText != null)
            carNameText.text = selectedCar.carName;

        if (carDescriptionText != null)
            carDescriptionText.text = selectedCar.description;

        if (carPriceText != null)
        {
            if (progressionManager != null && progressionManager.IsCarUnlocked(selectedCar.carID))
            {
                carPriceText.text = "OWNED";
            }
            else
            {
                carPriceText.text = $"${selectedCar.purchasePrice:F0}";
            }
        }

        if (carIcon != null && selectedCar.carIcon != null)
            carIcon.sprite = selectedCar.carIcon;
    }

    private void UpdateButtons()
    {
        if (selectedCar == null || progressionManager == null) return;

        bool isUnlocked = progressionManager.IsCarUnlocked(selectedCar.carID);
        bool canAfford = progressionManager.CurrentMoney >= selectedCar.purchasePrice;

        if (purchaseButton != null)
        {
            purchaseButton.gameObject.SetActive(!isUnlocked);
            purchaseButton.interactable = canAfford;
        }

        if (selectButton != null)
        {
            selectButton.gameObject.SetActive(isUnlocked);
            selectButton.interactable = isUnlocked;
        }

        if (customizeButton != null)
        {
            customizeButton.interactable = isUnlocked;
        }
    }

    private void SpawnPreviewCar(CarData car)
    {
        if (carPreviewPosition == null || car.carPrefab == null)
            return;

        // Destroy existing preview
        if (previewCarInstance != null)
        {
            Destroy(previewCarInstance);
        }

        // Spawn new preview
        previewCarInstance = Instantiate(car.carPrefab, carPreviewPosition.position, carPreviewPosition.rotation);
        
        // Disable physics and unnecessary components
        Rigidbody rb = previewCarInstance.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        DriftCarController controller = previewCarInstance.GetComponent<DriftCarController>();
        if (controller != null) controller.enabled = false;

        // Apply preview rotation
        previewCarInstance.transform.rotation = Quaternion.Euler(car.previewRotation);
    }

    private void OnPurchaseClicked()
    {
        if (selectedCar == null || progressionManager == null)
            return;

        if (progressionManager.SpendMoney(selectedCar.purchasePrice))
        {
            progressionManager.UnlockCar(selectedCar.carID);
            progressionManager.SetCurrentCar(selectedCar.carID);
            UpdateButtons();
            UpdateCarDisplay();
            
            Debug.Log($"Purchased {selectedCar.carName}!");
        }
    }

    private void OnSelectClicked()
    {
        if (selectedCar == null || progressionManager == null)
            return;

        progressionManager.SetCurrentCar(selectedCar.carID);
        Debug.Log($"Selected {selectedCar.carName}");
    }

    private void OnCustomizeClicked()
    {
        if (selectedCar == null)
            return;

        // Open customization menu
        // TODO: Open customization scene or menu
        Debug.Log($"Opening customization for {selectedCar.carName}");
    }
}

// Helper class for car list item UI
public class CarListItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image iconImage;
    
    private CarData carData;
    private System.Action<CarData> onSelected;

    public void Setup(CarData car, System.Action<CarData> onSelectedCallback)
    {
        carData = car;
        onSelected = onSelectedCallback;

        if (nameText != null)
            nameText.text = car.carName;

        if (iconImage != null && car.carIcon != null)
            iconImage.sprite = car.carIcon;

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => onSelected?.Invoke(car));
        }
    }
}

