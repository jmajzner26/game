using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ShadowDuel.UI
{
    /// <summary>
    /// Main menu UI controller
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button tutorialButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [Header("Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Button backToMenuButton;

        private void Start()
        {
            // Setup button listeners
            if (playButton) playButton.onClick.AddListener(OnPlayClicked);
            if (tutorialButton) tutorialButton.onClick.AddListener(OnTutorialClicked);
            if (settingsButton) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (creditsButton) creditsButton.onClick.AddListener(OnCreditsClicked);
            if (quitButton) quitButton.onClick.AddListener(OnQuitClicked);
            if (backToMenuButton) backToMenuButton.onClick.AddListener(OnBackToMenuClicked);

            // Load volume settings
            LoadSettings();
        }

        private void OnPlayClicked()
        {
            // Load game scene
            SceneManager.LoadScene("Arena_Temple");
        }

        private void OnTutorialClicked()
        {
            // Load tutorial scene
            SceneManager.LoadScene("Tutorial");
        }

        private void OnSettingsClicked()
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(false);
            if (settingsPanel) settingsPanel.SetActive(true);
        }

        private void OnCreditsClicked()
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(false);
            if (creditsPanel) creditsPanel.SetActive(true);
        }

        private void OnQuitClicked()
        {
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        private void OnBackToMenuClicked()
        {
            if (settingsPanel) settingsPanel.SetActive(false);
            if (creditsPanel) creditsPanel.SetActive(false);
            if (mainMenuPanel) mainMenuPanel.SetActive(true);
            
            SaveSettings();
        }

        private void LoadSettings()
        {
            // Load player preferences
            if (masterVolumeSlider)
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            
            if (musicVolumeSlider)
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            
            if (sfxVolumeSlider)
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }

        private void SaveSettings()
        {
            // Save player preferences
            if (masterVolumeSlider)
                PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
            
            if (musicVolumeSlider)
                PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
            
            if (sfxVolumeSlider)
                PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
            
            PlayerPrefs.Save();
        }

        private void Update()
        {
            // Press ESC to return to main menu from settings
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (settingsPanel && settingsPanel.activeSelf)
                {
                    OnBackToMenuClicked();
                }
                else if (creditsPanel && creditsPanel.activeSelf)
                {
                    OnBackToMenuClicked();
                }
            }
        }
    }
}

