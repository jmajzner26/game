using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Garage : MonoBehaviour
{
    [Header("Car Display")]
    [SerializeField] private Transform carDisplayParent;
    [SerializeField] private TextMeshProUGUI carNameText;
    [SerializeField] private TextMeshProUGUI carStatsText;
    
    [Header("Car Stats Bars")]
    [SerializeField] private Image speedBar;
    [SerializeField] private Image accelerationBar;
    [SerializeField] private Image handlingBar;
    
    [Header("Car Selection")]
    [SerializeField] private Button previousCarButton;
    [SerializeField] private Button nextCarButton;
    [SerializeField] private Button selectCarButton;
    [SerializeField] private Button backButton;
    
    [Header("Available Universe")]
    [SerializeField] private CarConfig[] availableCars;
    
    private int currentCarIndex = 0;
    private GameObject currentCarInstance;
    private CarConfig selectedCar;
    
    private void Start()
    {
        LoadCarsFromResources();
        SetupButtons();
        DisplayCar(0);
        
        // Load previously selected car
        string savedCarId = PlayerPrefs.GetString("SelectedCar", "");
        if (!string.IsNullOrEmpty(savedCarId))
        {
            for (int i = 0; i < availableCars.Length; i++)
            {
                if (availableCars[i].id == savedCarId)
                {
                    currentCarIndex = i;
                    DisplayCar(i);
                    break;
                }
            }
        }
    }
    
    private void LoadCarsFromResources()
    {
        availableCars = Resources.LoadAll<CarConfig>("CarConfigs");
        if (availableCars == null || availableCars.Length == 0)
        {
            Debug.LogWarning("No car configs found in Resources/CarConfigs!");
            availableCars = new CarConfig[0];
        }
    }
    
    private void SetupButtons()
    {
        if (previousCarButton != null)
            previousCarButton.onClick.AddListener(() => NavigateCar(-1));
        
        if (nextCarButton != null)
            nextCarButton.onClick.AddListener(() => NavigateCar(1));
        
        if (selectCarButton != null)
            selectCarButton.onClick.AddListener(SelectCurrentCar);
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
    }
    
    private void NavigateCar(int direction)
    {
        if (availableCars == null || availableCars.Length == 0) return;
        
        currentCarIndex += direction;
        
        if (currentCarIndex < 0)
            currentCarIndex = availableCars.Length - 1;
        else if (currentCarIndex >= availableCars.Length)
            currentCarIndex = 0;
        
        DisplayCar(currentCarIndex);
    }
    
    private void DisplayCar(int index)
    {
        if (availableCars == null || index < 0 || index >= availableCars.Length) return;
        
        selectedCar = availableCars[index];
        
        // Clear previous car instance
        if (currentCarInstance != null)
        {
            Destroy(currentCarInstance);
        }
        
        // Instantiate car mesh if available
        if (selectedCar.mesh != null && carDisplayParent != null)
        {
            currentCarInstance = Instantiate(selectedCar.mesh, carDisplayParent);
            currentCarInstance.transform.localPosition = Vector3.zero;
            currentCarInstance.transform.localRotation = Quaternion.identity;
            
            // Apply material if available
            if (selectedCar.material != null)
            {
                Renderer[] renderers = currentCarInstance.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.material = selectedCar.material;
                }
            }
        }
        
        // Update UI
        if (carNameText != null)
        {
            carNameText.text = selectedCar.displayName;
        }
        
        UpdateStatsDisplay();
    }
    
    private void UpdateStatsDisplay()
    {
        if (selectedCar == null) return;
        
        // Normalize stats (assuming max values)
        float maxSpeed = 100f; // m/s
        float maxEnginePower = 800f;
        float maxSteerSensitivity = 5f;
        
        float speedNormalized = selectedCar.maxSpeed / maxSpeed;
        float accelNormalized = selectedCar.enginePower / maxEnginePower;
        float handlingNormalized = selectedCar.steerSensitivity / maxSteerSensitivity;
        
        if (speedBar != null)
            speedBar.fillAmount = speedNormalized;
        
        if (accelerationBar != null)
            accelerationBar.fillAmount = accelNormalized;
        
        if (handlingBar != null)
            handlingBar.fillAmount = handlingNormalized;
        
        if (carStatsText != null)
        {
            carStatsText.text = $"Speed: {selectedCar.maxSpeed * 3.6f:F0} km/h\n" +
                               $"Power: {selectedCar.enginePower:F0} HP\n" +
                               $"Handling: {selectedCar.steerSensitivity:F1}";
        }
    }
    
    private void SelectCurrentCar()
    {
        if (selectedCar == null) return;
        
        PlayerPrefs.SetString("SelectedCar", selectedCar.id);
        
        // Notify MainMenu if it exists
        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        if (mainMenu != null)
        {
            mainMenu.SetSelectedCar(selectedCar);
        }
        
        GoBack();
    }
    
    private void GoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

