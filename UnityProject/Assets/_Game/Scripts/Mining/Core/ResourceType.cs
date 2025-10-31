using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceType", menuName = "Mining/Resource Type", order = 1)]
public class ResourceType : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string displayName;
    public string description;
    
    [Header("Visual")]
    public Color resourceColor = Color.white;
    public Material blockMaterial;
    public GameObject breakEffectPrefab;
    public Sprite icon;
    
    [Header("Properties")]
    public float baseValue = 1f;
    public float hardness = 1f; // How difficult to mine (affects mining time)
    public float rarity = 1f; // Lower = rarer (0.1 to 1.0)
    
    [Header("Generation")]
    public int minDepth = 0; // Minimum depth to spawn
    public int maxDepth = 100; // Maximum depth to spawn
    public float spawnChance = 0.1f; // Chance per block at appropriate depth
    
    [Header("Stacking")]
    public int maxStackSize = 64;
    
    void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
            id = name.ToLower().Replace(" ", "_");
    }
}
