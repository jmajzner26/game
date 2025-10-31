using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject garagePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject leaderboardPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button startRaceButton;
    [SerializeField] private Button timeTrialButton;
    [SerializeField] private Button garageButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Track Selection")]
    [SerializeField] private Dropdown trackDropdown;
    [SerializeField] private TextMeshProUGUI trackDescriptionText;
    [SerializeField] private TrackConfig[] availableTracks;
    
    [Header("Car Display")]
    [SerializeField] private TextMeshProUGUI selectedCarText;
    [SerializeField] private CarConfig selectedCar;
    
    private int selectedTrackIndex = 0;
    
    private void Start()
    {
        SetupButtons();
        LoadAvailableTracks();
        LoadSelectedCar();
    }
    
    private void SetupButtons()
    {
        if (startRaceButton != null)
            startRaceButton.onClick.AddListener(StartRace);
        
        if (timeTrialButton != null)
            timeTrialButton.onClick.AddListener(StartTimeTrial);
        
        if (garageButton != null)
            garageButton.onClick.AddListener(OpenGarage);
        
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(OpenLeaderboard);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }
    
    private void LoadAvailableTracks()
    {
        if (trackDropdown != null && availableTracks != null && availableTracks.Length > 0)
        {
            trackDropdown.ClearOptions();
            
            var trackNames = new System.Collections.Generic.List<string>();
            foreach (var track in availableTracks)
            {
                trackNames.Add(track.displayName);
            }
            
            trackDropdown.AddOptions(trackNames);
            trackDropdown.onValueChanged.AddListener(OnTrackSelected);
            OnTrackSelected(0);
        }
    }
    
    private void OnTrackSelected(int index)
    {
        selectedTrackIndex = index;
        
        if (availableTracks != null && index < availableTracks.Length && trackDescriptionText != null)
        {
            TrackConfig track = availableTracks[index];
            trackDescriptionText.text = $"{track.displayName}\nLaps: {track.laps}\nCheckpoints: {track.checkpointCount}";
        }
    }
    
    private void LoadSelectedCar()
    {
        // Load from PlayerPrefs or ProfileData
        string carId = PlayerPrefs.GetString("SelectedCar", "");
        
        if (!string.IsNullOrEmpty(carId))
        {
            CarConfig car = Resources.Load<CarConfig>($"CarConfigs/{carId}");
            if (car != null)
            {
                selectedCar = car;
            }
        }
        
        UpdateCarDisplay();
    }
    
    private void UpdateCarDisplay()
    {
        if (selectedCarText != null)
        {
            selectedCarText.text = selectedCar != null ? selectedCar.displayName : "No Car Selected";
        }
    }
    
    public void StartRace()
    {
        if (availableTracks == null || selectedTrackIndex >= availableTracks.Length) return;
        
        TrackConfig track = availableTracks[selectedTrackIndex];
        if (selectedCar == null)
        {
            Debug.LogWarning("No car selected!");
            return;
        }
        
        // Store selection
        PlayerPrefs.SetString("SelectedTrack", track.sceneName);
        PlayerPrefs.SetString("SelectedCar", selectedCar.id);
        PlayerPrefs.SetString("GameMode", "Race");
        
        // Load race scene
        SceneManager.LoadScene(track.sceneName);
    }
    
    public void StartTimeTrial()
    {
        if (availableTracks == null || selectedTrackIndex >= availableTracks.Length) return;
        
        TrackConfig track = availableTracks[selectedTrackIndex];
        if (selectedCar == null)
        {
            Debug.LogWarning("No car selected!");
            return;
        }
        
        PlayerPrefs.SetString("SelectedTrack", track.sceneName);
        PlayerPrefs.SetString("SelectedCar", selectedCar.id);
        PlayerPrefs.SetString("GameMode", "TimeTrial");
        
        SceneManager.LoadScene(track.sceneName);
    }
    
    public void OpenGarage()
    {
        if (garagePanel != null)
        {
            garagePanel.SetActive(true);
            mainPanel.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("Garage");
        }
    }
    
    public void OpenLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            mainPanel.SetActive(false);
        }
    }
    
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            mainPanel.SetActive(false);
        }
    }
    
    public void BackToMain()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (garagePanel != null) garagePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void SetSelectedCar(CarConfig car)
    {
        selectedCar = car;
        UpdateCarDisplay();
        PlayerPrefs.SetString("SelectedCar", car.id);
    }
}

