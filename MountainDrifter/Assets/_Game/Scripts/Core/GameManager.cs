using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main game manager for Mountain Drifter. Handles game state, scene transitions, and core systems.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Menu;

    [Header("References")]
    [SerializeField] private ProgressionManager progressionManager;
    [SerializeField] private SaveSystem saveSystem;

    private DriftCarController playerCar;
    private DriftScoring driftScoring;

    public GameState CurrentState => currentState;
    public ProgressionManager Progression => progressionManager;
    public DriftCarController PlayerCar => playerCar;

    public enum GameState
    {
        Menu,
        Garage,
        Racing,
        Paused,
        Replay,
        FreeRoam
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize systems
        if (progressionManager == null)
            progressionManager = GetComponent<ProgressionManager>();
        
        if (saveSystem == null)
            saveSystem = GetComponent<SaveSystem>();

        // Load saved data
        saveSystem?.LoadGame();
    }

    private void Start()
    {
        if (progressionManager != null)
        {
            progressionManager.Initialize();
        }
    }

    public void SetPlayerCar(DriftCarController car)
    {
        playerCar = car;
        driftScoring = car.GetComponent<DriftScoring>();
    }

    public void StartRace(string trackName)
    {
        currentState = GameState.Racing;
        SceneManager.LoadScene(trackName);
    }

    public void EnterGarage()
    {
        currentState = GameState.Garage;
        SceneManager.LoadScene("Garage");
    }

    public void StartFreeRoam(string trackName)
    {
        currentState = GameState.FreeRoam;
        SceneManager.LoadScene(trackName);
    }

    public void PauseGame()
    {
        if (currentState == GameState.Racing || currentState == GameState.FreeRoam)
        {
            currentState = GameState.Paused;
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = GameState.Racing;
            Time.timeScale = 1f;
        }
    }

    public void EndRace(float finalScore)
    {
        if (driftScoring != null)
        {
            float moneyEarned = driftScoring.GetMoneyEarned();
            progressionManager?.AddMoney(moneyEarned);
            progressionManager?.AddReputation(Mathf.FloorToInt(finalScore / 100f));
        }

        saveSystem?.SaveGame();
        currentState = GameState.Menu;
    }

    public void ReturnToMenu()
    {
        currentState = GameState.Menu;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && (currentState == GameState.Racing || currentState == GameState.FreeRoam))
        {
            PauseGame();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && (currentState == GameState.Racing || currentState == GameState.FreeRoam))
        {
            PauseGame();
        }
    }
}

