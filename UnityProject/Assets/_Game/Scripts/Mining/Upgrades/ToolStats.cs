using UnityEngine;

[CreateAssetMenu(fileName = "NewTool", menuName = "Mining/Tool", order = 2)]
public class ToolStats : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string displayName;
    public string description;
    
    [Header("Stats")]
    [Range(0.5f, 10f)]
    public float miningPower = 1f;
    [Range(0.5f, 5f)]
    public float miningSpeed = 1f;
    [Range(1f, 100f)]
    public float durability = 100f;
    
    [Header("Visual")]
    public GameObject toolModelPrefab;
    public Sprite icon;
    
    [Header("Energy")]
    public float energyCostMultiplier = 1f; // Lower is better
    
    void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
            id = name.ToLower().Replace(" ", "_");
    }
}
