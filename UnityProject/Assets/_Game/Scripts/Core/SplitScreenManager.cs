using UnityEngine;
using UnityEngine.SceneManagement;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    
    [Header("Cameras")]
    [SerializeField] private Camera player1Camera;
    [SerializeField] private Camera player2Camera;
    
    [Header("Car Configs")]
    [SerializeField] private CarConfig player1CarConfig;
    [SerializeField] private CarConfig player2CarConfig;
    
    [Header("Spawn Points")]
    [SerializeField] private Transform player1SpawnPoint;
    [SerializeField] private Transform player2SpawnPoint;
    
    private GameObject player1;
    private GameObject player2;
    private VehicleController player1Controller;
    private VehicleController player2Controller;
    private LapCounter player1LapCounter;
    private LapCounter player2LapCounter;
    
    private void Start()
    {
        SetupSplitScreen();
    }
    
    private void SetupSplitScreen()
    {
        // Setup cameras for split-screen
        if (player1Camera != null && player2Camera != null)
        {
            player1Camera.rect = new Rect(0f, 0f, 0.5f, 1f); // Left half
            player2Camera.rect = new Rect(0.5f, 0f, 0.5f, 1f); // Right half
        }
        
        // Spawn players
        if (player1Prefab != null && player1SpawnPoint != null)
        {
            player1 = Instantiate(player1Prefab, player1SpawnPoint.position, player1SpawnPoint.rotation);
            player1.tag = "Player";
            player1Controller = player1.GetComponent<VehicleController>();
            player1LapCounter = player1.GetComponent<LapCounter>();
            
            if (player1Controller != null && player1CarConfig != null)
            {
                player1Controller.Initialize(player1CarConfig);
            }
            
            // Setup camera to follow player 1
            if (player1Camera != null)
            {
                Cinemachine.CinemachineVirtualCamera vcam = player1Camera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    vcam.Follow = player1.transform;
                    vcam.LookAt = player1.transform;
                }
            }
        }
        
        if (player2Prefab != null && player2SpawnPoint != null)
        {
            player2 = Instantiate(player2Prefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
            player2.tag = "Player2";
            player2Controller = player2.GetComponent<VehicleController>();
            player2LapCounter = player2.GetComponent<LapCounter>();
            
            if (player2Controller != null && player2CarConfig != null)
            {
                player2Controller.Initialize(player2CarConfig);
            }
            
            // Setup camera to follow player 2
            if (player2Camera != null)
            {
                Cinemachine.CinemachineVirtualCamera vcam = player2Camera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    vcam.Follow = player2.transform;
                    vcam.LookAt = player2.transform;
                }
            }
        }
        
        // Setup input - would need separate input maps for each player
        SetupPlayerInputs();
    }
    
    private void SetupPlayerInputs()
    {
        // Player 1 uses default input map
        // Player 2 would need a separate input map (requires Input System setup)
        // This is a placeholder - full implementation would require proper input configuration
    }
    
    private void Update()
    {
        // Check for race completion
        if (player1LapCounter != null && player1LapCounter.RaceComplete)
        {
            OnPlayerFinish(1);
        }
        
        if (player2LapCounter != null && player2LapCounter.RaceComplete)
        {
            OnPlayerFinish(2);
        }
    }
    
    private void OnPlayerFinish(int playerNumber)
    {
        Debug.Log($"Player {playerNumber} finished!");
        // Show results and return to menu
        Invoke(nameof(ReturnToMenu), 5f);
    }
    
    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

