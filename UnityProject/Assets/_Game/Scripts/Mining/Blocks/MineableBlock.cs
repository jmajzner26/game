using UnityEngine;

public class MineableBlock : MonoBehaviour
{
    [Header("Block Data")]
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int depthLevel = 0;
    
    [Header("Visual")]
    [SerializeField] private Renderer blockRenderer;
    [SerializeField] private Collider blockCollider;
    [SerializeField] private GameObject breakParticlesPrefab;
    
    [Header("State")]
    private float currentHealth;
    private bool isMining = false;
    private float miningProgress = 0f;
    
    private Vector3 originalScale;
    
    public ResourceType ResourceType => resourceType;
    public int DepthLevel => depthLevel;
    public float MiningProgress => miningProgress;
    public bool IsBeingMined => isMining;
    
    public event System.Action<MineableBlock> OnBlockDestroyed;
    public event System.Action<MineableBlock, float> OnMiningProgressChanged;
    
    private void Awake()
    {
        originalScale = transform.localScale;
        
        if (blockRenderer == null)
            blockRenderer = GetComponent<Renderer>();
        
        if (blockCollider == null)
            blockCollider = GetComponent<Collider>();
        
        InitializeBlock();
    }
    
    public void Initialize(ResourceType resource, int depth)
    {
        resourceType = resource;
        depthLevel = depth;
        InitializeBlock();
    }
    
    private void InitializeBlock()
    {
        if (resourceType != null)
        {
            currentHealth = resourceType.hardness;
            
            if (blockRenderer != null && resourceType.blockMaterial != null)
            {
                blockRenderer.material = resourceType.blockMaterial;
            }
            
            // Add slight color variation
            if (blockRenderer != null)
            {
                var material = blockRenderer.material;
                Color baseColor = resourceType.resourceColor;
                baseColor *= Random.Range(0.9f, 1.1f);
                material.color = baseColor;
            }
        }
    }
    
    public void StartMining(float miningPower)
    {
        if (isMining) return;
        
        isMining = true;
        miningProgress = 0f;
    }
    
    public bool MineBlock(float miningPower, float deltaTime)
    {
        if (!isMining || resourceType == null) return false;
        
        // Calculate damage based on mining power vs block hardness
        float damage = miningPower / resourceType.hardness * deltaTime;
        miningProgress += damage;
        
        // Visual feedback - shake/scale
        float shakeAmount = Mathf.Sin(miningProgress * 20f) * 0.02f;
        transform.localScale = originalScale * (1f + shakeAmount);
        
        OnMiningProgressChanged?.Invoke(this, miningProgress);
        
        // Check if block is destroyed
        if (miningProgress >= 1f)
        {
            DestroyBlock();
            return true;
        }
        
        return false;
    }
    
    public void StopMining()
    {
        if (!isMining) return;
        
        isMining = false;
        transform.localScale = originalScale;
        
        // Reset progress over time if not fully mined
        if (miningProgress < 1f)
        {
            miningProgress = Mathf.Max(0f, miningProgress - Time.deltaTime * 0.5f);
            OnMiningProgressChanged?.Invoke(this, miningProgress);
        }
    }
    
    private void DestroyBlock()
    {
        // Play break sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound("BlockBreak");
        }
        
        // Spawn break particles
        if (breakParticlesPrefab != null || (resourceType != null && resourceType.breakEffectPrefab != null))
        {
            GameObject particles = Instantiate(
                resourceType?.breakEffectPrefab ?? breakParticlesPrefab,
                transform.position,
                Quaternion.identity
            );
            
            // Auto-destroy particles after 3 seconds
            Destroy(particles, 3f);
        }
        
        // Drop resource
        if (resourceType != null)
        {
            var inventory = GameManager.Instance?.GetPlayerInventory();
            if (inventory != null)
            {
                inventory.AddResource(resourceType, 1);
                
                // Play pickup sound
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySound("Pickup");
                }
            }
        }
        
        OnBlockDestroyed?.Invoke(this);
        
        // Destroy the block
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        OnBlockDestroyed?.Invoke(this);
    }
}
