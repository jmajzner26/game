using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private int chunkSize = 16;
    [SerializeField] private int chunksPerDimension = 4;
    [SerializeField] private float blockSpacing = 1f;
    
    [Header("Resources")]
    [SerializeField] private ResourceType[] availableResources;
    [SerializeField] private GameObject blockPrefab;
    
    [Header("Depth Layers")]
    [SerializeField] private int layerHeight = 10; // Blocks per depth layer
    
    private Dictionary<Vector3Int, MineableBlock> activeBlocks = new Dictionary<Vector3Int, MineableBlock>();
    private HashSet<Vector3Int> generatedChunks = new HashSet<Vector3Int>();
    
    private void Start()
    {
        GenerateInitialTerrain();
    }
    
    public void GenerateInitialTerrain()
    {
        // Generate chunks around origin
        for (int x = -chunksPerDimension; x < chunksPerDimension; x++)
        {
            for (int z = -chunksPerDimension; z < chunksPerDimension; z++)
            {
                GenerateChunk(x, 0, z);
            }
        }
    }
    
    public void GenerateChunk(int chunkX, int chunkY, int chunkZ)
    {
        Vector3Int chunkKey = new Vector3Int(chunkX, chunkY, chunkZ);
        if (generatedChunks.Contains(chunkKey)) return;
        
        generatedChunks.Add(chunkKey);
        
        Vector3 chunkOrigin = new Vector3(
            chunkX * chunkSize * blockSpacing,
            chunkY * chunkSize * blockSpacing,
            chunkZ * chunkSize * blockSpacing
        );
        
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    Vector3 worldPos = chunkOrigin + new Vector3(
                        x * blockSpacing,
                        -y * blockSpacing, // Negative Y for going down
                        z * blockSpacing
                    );
                    
                    GenerateBlock(worldPos);
                }
            }
        }
    }
    
    private void GenerateBlock(Vector3 position)
    {
        // Calculate depth level
        int depthLevel = Mathf.FloorToInt(-position.y / (layerHeight * blockSpacing));
        
        // Skip if already exists
        Vector3Int blockKey = Vector3Int.RoundToInt(position);
        if (activeBlocks.ContainsKey(blockKey)) return;
        
        // Determine resource type based on depth and probability
        ResourceType selectedResource = SelectResourceForDepth(depthLevel);
        
        if (selectedResource == null) return;
        
        // Spawn block
        GameObject blockObj = Instantiate(blockPrefab, position, Quaternion.identity, transform);
        MineableBlock block = blockObj.GetComponent<MineableBlock>();
        
        if (block != null)
        {
            block.Initialize(selectedResource, depthLevel);
            block.OnBlockDestroyed += OnBlockDestroyed;
        }
        
        activeBlocks[blockKey] = block;
    }
    
    private ResourceType SelectResourceForDepth(int depthLevel)
    {
        if (availableResources == null || availableResources.Length == 0)
            return null;
        
        List<ResourceType> validResources = new List<ResourceType>();
        
        // Filter resources that can spawn at this depth
        foreach (var resource in availableResources)
        {
            if (resource == null) continue;
            
            if (depthLevel >= resource.minDepth && depthLevel <= resource.maxDepth)
            {
                // Calculate spawn probability based on rarity and depth
                float spawnChance = resource.spawnChance * (1f - resource.rarity * 0.5f);
                
                // Increase rarity as depth increases (makes sense for ores)
                if (depthLevel > resource.minDepth)
                {
                    float depthBonus = Mathf.Clamp01((depthLevel - resource.minDepth) / 20f);
                    spawnChance *= (1f + depthBonus);
                }
                
                if (Random.Range(0f, 1f) <= spawnChance)
                {
                    validResources.Add(resource);
                }
            }
        }
        
        // If no valid resources found, return default (stone/dirt)
        if (validResources.Count == 0)
        {
            // Return first resource as default (should be stone)
            return availableResources[0];
        }
        
        // Weighted random selection based on rarity
        float totalWeight = 0f;
        foreach (var resource in validResources)
        {
            totalWeight += 1f / resource.rarity;
        }
        
        float random = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (var resource in validResources)
        {
            currentWeight += 1f / resource.rarity;
            if (random <= currentWeight)
            {
                return resource;
            }
        }
        
        return validResources[validResources.Count - 1];
    }
    
    private void OnBlockDestroyed(MineableBlock block)
    {
        // Remove from active blocks
        Vector3Int key = Vector3Int.RoundToInt(block.transform.position);
        activeBlocks.Remove(key);
        
        // Optionally regenerate after delay or when player moves away
        // For now, blocks stay destroyed
    }
    
    public MineableBlock GetBlockAt(Vector3 position)
    {
        Vector3Int key = Vector3Int.RoundToInt(position);
        activeBlocks.TryGetValue(key, out MineableBlock block);
        return block;
    }
    
    public void GenerateChunkAroundPosition(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / (chunkSize * blockSpacing));
        int chunkY = Mathf.FloorToInt(position.y / (chunkSize * blockSpacing));
        int chunkZ = Mathf.FloorToInt(position.z / (chunkSize * blockSpacing));
        
        // Generate surrounding chunks
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                GenerateChunk(chunkX + x, chunkY, chunkZ + z);
            }
        }
    }
}
