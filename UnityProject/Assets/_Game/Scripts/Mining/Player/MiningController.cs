using UnityEngine;

public class MiningController : MonoBehaviour
{
    [Header("Mining Settings")]
    [SerializeField] private float miningRange = 3f;
    [SerializeField] private float miningAngle = 45f;
    [SerializeField] private LayerMask blockLayerMask = -1;
    
    [Header("Visual")]
    [SerializeField] private Transform miningPoint;
    [SerializeField] private GameObject miningIndicatorPrefab;
    [SerializeField] private LineRenderer miningRayRenderer;
    
    private MineableBlock currentTarget;
    private GameObject miningIndicator;
    private PlayerEnergy energySystem;
    private ToolStats currentTool;
    
    public ToolStats CurrentTool => currentTool;
    public MineableBlock CurrentTarget => currentTarget;
    
    private void Awake()
    {
        energySystem = GetComponent<PlayerEnergy>();
        
        // Create mining indicator if prefab exists
        if (miningIndicatorPrefab != null && miningPoint != null)
        {
            miningIndicator = Instantiate(miningIndicatorPrefab, miningPoint);
            miningIndicator.SetActive(false);
        }
        
        if (miningRayRenderer != null)
        {
            miningRayRenderer.enabled = false;
        }
    }
    
    private void Update()
    {
        HandleMiningInput();
        UpdateMiningVisuals();
    }
    
    private void HandleMiningInput()
    {
        bool isMiningInput = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
        
        if (isMiningInput && energySystem != null && energySystem.HasEnergy())
        {
            RaycastForBlock();
            
            if (currentTarget != null)
            {
                MineBlock();
            }
        }
        else
        {
            StopMining();
        }
    }
    
    private void RaycastForBlock()
    {
        if (miningPoint == null)
            miningPoint = transform;
        
        Ray ray = new Ray(miningPoint.position, miningPoint.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, miningRange, blockLayerMask))
        {
            MineableBlock block = hit.collider.GetComponent<MineableBlock>();
            
            if (block != null && block != currentTarget)
            {
                // Switch to new target
                if (currentTarget != null)
                {
                    currentTarget.StopMining();
                }
                
                currentTarget = block;
                currentTarget.StartMining(GetMiningPower());
            }
        }
        else if (currentTarget != null)
        {
            // Lost line of sight
            StopMining();
        }
    }
    
    private void MineBlock()
    {
        if (currentTarget == null || energySystem == null) return;
        
        if (!energySystem.HasEnergy())
        {
            StopMining();
            return;
        }
        
        float miningPower = GetMiningPower();
        float energyCost = miningPower * Time.deltaTime * 0.1f;
        
        if (!energySystem.UseEnergy(energyCost))
        {
            StopMining();
            return;
        }
        
        bool blockDestroyed = currentTarget.MineBlock(miningPower, Time.deltaTime);
        
        if (blockDestroyed)
        {
            currentTarget = null;
            
            // Update depth if we mined deeper
            if (GameManager.Instance != null)
            {
                // Depth calculation would go here based on player position
            }
        }
    }
    
    private void StopMining()
    {
        if (currentTarget != null)
        {
            currentTarget.StopMining();
            currentTarget = null;
        }
    }
    
    private float GetMiningPower()
    {
        if (currentTool != null)
        {
            return currentTool.miningPower;
        }
        return 1f; // Base mining power
    }
    
    private void UpdateMiningVisuals()
    {
        if (miningIndicator != null)
        {
            bool showIndicator = currentTarget != null;
            miningIndicator.SetActive(showIndicator);
            
            if (showIndicator && currentTarget != null)
            {
                miningIndicator.transform.position = currentTarget.transform.position;
            }
        }
        
        if (miningRayRenderer != null)
        {
            bool showRay = currentTarget != null && miningPoint != null;
            miningRayRenderer.enabled = showRay;
            
            if (showRay)
            {
                miningRayRenderer.SetPosition(0, miningPoint.position);
                miningRayRenderer.SetPosition(1, currentTarget.transform.position);
            }
        }
    }
    
    public void SetTool(ToolStats tool)
    {
        currentTool = tool;
    }
    
    private void OnDestroy()
    {
        StopMining();
    }
}
