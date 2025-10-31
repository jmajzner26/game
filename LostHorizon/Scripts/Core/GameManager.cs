using UnityEngine;
using UnityEngine.SceneManagement;

namespace LostHorizon.Core
{
    /// <summary>
    /// Main game manager that coordinates all core systems
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Systems")]
        [SerializeField] private Player.PlayerController playerController;
        [SerializeField] private World.WorldManager worldManager;
        [SerializeField] private Crafting.CraftingSystem craftingSystem;
        [SerializeField] private Building.BuildingSystem buildingSystem;
        [SerializeField] private Lore.QuestManager questManager;
        [SerializeField] private Resources.ResourceManager resourceManager;
        [SerializeField] private SaveSystem saveSystem;

        [Header("Game State")]
        [SerializeField] private bool isPaused = false;
        [SerializeField] private float gameTimeScale = 1f;

        [Header("Settings")]
        [SerializeField] private bool enableDebugLogs = true;

        public bool IsPaused => isPaused;
        public float GameTimeScale => gameTimeScale;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeSystems();
        }

        private void InitializeSystems()
        {
            if (enableDebugLogs) Debug.Log("Lost Horizon: Initializing game systems...");

            // Initialize all systems in order
            if (resourceManager != null) resourceManager.Initialize();
            if (worldManager != null) worldManager.Initialize();
            if (craftingSystem != null) craftingSystem.Initialize();
            if (buildingSystem != null) buildingSystem.Initialize();
            if (questManager != null) questManager.Initialize();
            if (playerController != null) playerController.Initialize();

            if (enableDebugLogs) Debug.Log("Lost Horizon: All systems initialized!");
        }

        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = gameTimeScale;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void TogglePause()
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        public void SetTimeScale(float scale)
        {
            gameTimeScale = Mathf.Clamp(scale, 0f, 2f);
            if (!isPaused)
            {
                Time.timeScale = gameTimeScale;
            }
        }

        public void SaveGame()
        {
            if (saveSystem != null)
            {
                saveSystem.SaveGame();
            }
        }

        public void LoadGame()
        {
            if (saveSystem != null)
            {
                saveSystem.LoadGame();
            }
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && !isPaused)
            {
                PauseGame();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}

