using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    [SerializeField] private float money = 0f;
    [SerializeField] private int depth = 0; // Current depth level
    
    public float Money => money;
    public int Depth => depth;
    
    public event System.Action<float> OnMoneyChanged;
    public event System.Action<int> OnDepthChanged;
    
    private Inventory playerInventory;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        playerInventory = FindObjectOfType<Inventory>();
        
        // Load saved game data
        LoadGame();
    }
    
    public void AddMoney(float amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke(money);
        SaveGame();
    }
    
    public bool SpendMoney(float amount)
    {
        if (money >= amount)
        {
            money -= amount;
            OnMoneyChanged?.Invoke(money);
            SaveGame();
            return true;
        }
        return false;
    }
    
    public void SetDepth(int newDepth)
    {
        depth = newDepth;
        OnDepthChanged?.Invoke(depth);
    }
    
    public Inventory GetPlayerInventory()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<Inventory>();
        return playerInventory;
    }
    
    private void SaveGame()
    {
        PlayerPrefs.SetFloat("Money", money);
        PlayerPrefs.SetInt("Depth", depth);
        PlayerPrefs.Save();
    }
    
    private void LoadGame()
    {
        money = PlayerPrefs.GetFloat("Money", 0f);
        depth = PlayerPrefs.GetInt("Depth", 0);
        OnMoneyChanged?.Invoke(money);
        OnDepthChanged?.Invoke(depth);
    }
    
    public void ResetGame()
    {
        money = 0f;
        depth = 0;
        PlayerPrefs.DeleteAll();
        OnMoneyChanged?.Invoke(money);
        OnDepthChanged?.Invoke(depth);
    }
}
