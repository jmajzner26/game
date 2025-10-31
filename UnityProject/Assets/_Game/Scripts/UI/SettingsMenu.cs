using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    [Header("Graphics")]
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    
    [Header("Controls")]
    [SerializeField] private Button rebindButton;
    [SerializeField] private TextMeshProUGUI controlsInfoText;
    
    [Header("Other")]
    [SerializeField] private Button backButton;
    
    private Resolution[] resolutions;
    
    private void Start()
    {
        SetupAudioSliders();
        SetupQualityDropdown();
        SetupResolutionDropdown();
        SetupFullscreenToggle();
        SetupButtons();
        LoadSettings();
    }
    
    private void SetupAudioSliders()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        }
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
    }
    
    private void SetupQualityDropdown()
    {
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }
    }
    
    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown != null)
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            
            var options = new System.Collections.Generic.List<string>();
            int currentResolutionIndex = 0;
            
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
                
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }
    }
    
    private void SetupFullscreenToggle()
    {
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }
    
    private void SetupButtons()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.AddListener(OpenControlsRebind);
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMain);
        }
    }
    
    private void LoadSettings()
    {
        // Load saved settings
        if (masterVolumeSlider != null)
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
        
        if (musicVolumeSlider != null)
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
        
        if (sfxVolumeSlider != null)
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
    }
    
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    
    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }
    
    public void SetResolution(int resolutionIndex)
    {
        if (resolutions != null && resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
            PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
        }
    }
    
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
    
    private void OpenControlsRebind()
    {
        // Placeholder for controls rebinding UI
        if (controlsInfoText != null)
        {
            controlsInfoText.text = "Controls:\n" +
                                   "Cast Forward - W/Up Arrow\n" +
                                   "Brake - S/Down Arrow\n" +
                                   "Steer - A/D or Left/Right Arrows\n" +
                                   "Handbrake - Space\n" +
                                   "Boost - Shift\n" +
                                   "Pause - ESC";
        }
    }
    
    public void BackToMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

