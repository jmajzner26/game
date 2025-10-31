using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main menu controller for Mountain Drifter.
/// Handles navigation to race, garage, settings, etc.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button startRaceButton;
    [SerializeField] private Button garageButton;
    [SerializeField] private Button freeRoamButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject trackSelectionPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Music")]
    [SerializeField] private MusicController musicController;

    private void Start()
    {
        SetupButtons();

        if (musicController != null)
        {
            musicController.PlayMenuMusic();
        }

        // Show main panel
        if (mainPanel != null)
            mainPanel.SetActive(true);
        
        if (trackSelectionPanel != null)
            trackSelectionPanel.SetActive(false);
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        if (startRaceButton != null)
            startRaceButton.onClick.AddListener(OnStartRaceClicked);

        if (garageButton != null)
            garageButton.onClick.AddListener(OnGarageClicked);

        if (freeRoamButton != null)
            freeRoamButton.onClick.AddListener(OnFreeRoamClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnStartRaceClicked()
    {
        // Show track selection
        if (trackSelectionPanel != null)
        {
            trackSelectionPanel.SetActive(true);
            if (mainPanel != null)
                mainPanel.SetActive(false);
        }
        else
        {
            // Direct to default track
            StartRace("Track_MountainPass");
        }
    }

    private void OnGarageClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnterGarage();
        }
        else
        {
            SceneManager.LoadScene("Garage");
        }
    }

    private void OnFreeRoamClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartFreeRoam("Track_MountainPass");
        }
        else
        {
            SceneManager.LoadScene("Track_MountainPass");
        }
    }

    private void OnSettingsClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            if (mainPanel != null)
                mainPanel.SetActive(!settingsPanel.activeSelf);
        }
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void StartRace(string trackName)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartRace(trackName);
        }
        else
        {
            SceneManager.LoadScene(trackName);
        }
    }
}

