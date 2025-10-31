using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to quickly set up the mining game scene.
/// Attach this to an empty GameObject and click "Setup Scene" button in Inspector.
/// </summary>
public class SceneSetupHelper : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Scene Setup")]
    [SerializeField] private bool setupOnStart = false;
    
    [ContextMenu("Setup Mining Game Scene")]
    public void SetupMiningScene()
    {
        Debug.Log("Setting up Mining Game Scene...");
        
        // Create GameManager
        GameObject gameManager = CreateGameObject("GameManager");
        gameManager.AddComponent<GameManager>();
        
        // Create EconomyManager
        GameObject economyManager = CreateGameObject("EconomyManager");
        economyManager.AddComponent<EconomyManager>();
        
        // Create SoundManager
        GameObject soundManager = CreateGameObject("SoundManager");
        soundManager.AddComponent<SoundManager>();
        AudioSource musicSource = soundManager.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        AudioSource sfxSource = soundManager.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        
        // Set references using reflection
        var soundManagerComponent = soundManager.GetComponent<SoundManager>();
        var musicField = typeof(SoundManager).GetField("musicSource", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var sfxField = typeof(SoundManager).GetField("sfxSource", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (musicField != null) musicField.SetValue(soundManagerComponent, musicSource);
        if (sfxField != null) sfxField.SetValue(soundManagerComponent, sfxSource);
        
        // Create TerrainGenerator
        GameObject terrainGen = CreateGameObject("TerrainGenerator");
        terrainGen.AddComponent<TerrainGenerator>();
        terrainGen.transform.position = Vector3.zero;
        
        // Create EquipmentShop
        GameObject equipmentShop = CreateGameObject("EquipmentShop");
        equipmentShop.AddComponent<EquipmentShop>();
        
        // Create Player
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0, 5, 0);
        Destroy(player.GetComponent<CapsuleCollider>()); // Remove primitive collider
        
        // Add CharacterController
        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.5f;
        controller.center = new Vector3(0, 1, 0);
        
        // Add Player Components
        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerEnergy>();
        player.AddComponent<Inventory>();
        player.AddComponent<MiningController>();
        
        // Create Camera
        GameObject camera = new GameObject("Camera");
        camera.transform.SetParent(player.transform);
        camera.transform.localPosition = new Vector3(0, 1.6f, 0);
        Camera cam = camera.AddComponent<Camera>();
        cam.fieldOfView = 75f;
        camera.AddComponent<AudioListener>();
        camera.tag = "MainCamera";
        
        // Create Ground Check
        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(player.transform);
        groundCheck.transform.localPosition = new Vector3(0, -0.9f, 0);
        
        // Create Canvas
        GameObject canvas = new GameObject("UI_Canvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Create HUD
        GameObject hud = new GameObject("HUD");
        hud.transform.SetParent(canvas.transform);
        hud.transform.localPosition = Vector3.zero;
        hud.transform.localScale = Vector3.one;
        hud.AddComponent<MiningHUD>();
        
        // Create Shop Panel
        GameObject shopPanel = new GameObject("ShopPanel");
        shopPanel.transform.SetParent(canvas.transform);
        shopPanel.AddComponent<RectTransform>();
        shopPanel.AddComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.8f);
        shopPanel.SetActive(false);
        shopPanel.AddComponent<ShopUI>();
        
        // Create Inventory Panel
        GameObject inventoryPanel = new GameObject("InventoryPanel");
        inventoryPanel.transform.SetParent(canvas.transform);
        inventoryPanel.AddComponent<RectTransform>();
        inventoryPanel.AddComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.8f);
        inventoryPanel.SetActive(false);
        inventoryPanel.AddComponent<InventoryUI>();
        
        // Create Directional Light if doesn't exist
        if (FindObjectOfType<Light>() == null)
        {
            GameObject light = new GameObject("Directional Light");
            Light lightComponent = light.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
        
        Debug.Log("Mining Game Scene setup complete!");
        Debug.LogWarning("NOTE: You still need to:");
        Debug.LogWarning("1. Create ResourceType ScriptableObjects (Stone, Coal, Iron, Gold, Diamond)");
        Debug.LogWarning("2. Create ToolStats ScriptableObjects (BasicPickaxe, IronPickaxe, etc.)");
        Debug.LogWarning("3. Create Block Prefab and assign to TerrainGenerator");
        Debug.LogWarning("4. Assign ResourceTypes to TerrainGenerator's Available Resources");
        Debug.LogWarning("5. Assign Resource Prices to EconomyManager");
        Debug.LogWarning("6. Assign Tools to EquipmentShop");
        Debug.LogWarning("7. Set up UI elements (texts, buttons, sliders)");
    }
    
    private GameObject CreateGameObject(string name)
    {
        GameObject obj = new GameObject(name);
        return obj;
    }
    
    private void Start()
    {
        if (setupOnStart)
        {
            SetupMiningScene();
        }
    }
#endif
}
