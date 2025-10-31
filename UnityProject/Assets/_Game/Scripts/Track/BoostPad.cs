using UnityEngine;

public class BoostPad : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float boostForce = 30000f;
    [SerializeField] private float boostDuration = 2f;
    
    private BoxCollider trigger;
    private Renderer boostPadRenderer;
    private bool isActive = true;
    private float cooldownTimer = 0f;
    private const float COOLDOWN_TIME = 3f;
    
    private void Awake()
    {
        trigger = GetComponent<BoxCollider>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<BoxCollider>();
        }
        trigger.isTrigger = true;
        
        boostPadRenderer = GetComponent<Renderer>();
        
        // Visual indicator
        if (boostPadRenderer != null)
        {
            boostPadRenderer.material.color = Color.cyan;
        }
    }
    
    private void Update()
    {
        if (!isActive)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                Activate();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        
        VehicleController vehicle = other.GetComponent<VehicleController>();
        if (vehicle != null && vehicle.Config != null)
        {
            // Activate boost on vehicle
            ActivateBoost(vehicle);
            Deactivate();
        }
    }
    
    private void ActivateBoost(VehicleController vehicle)
    {
        // Add boost force
        Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 boostDirection = transform.forward;
            rb.AddForce(boostDirection * boostForce, ForceMode.Force);
        }
        
        // Trigger boost visual/audio effects
        // This would call a boost effect system
    }
    
    private void Activate()
    {
        isActive = true;
        if (boostPadRenderer != null)
        {
            boostPadRenderer.material.color = Color.cyan;
        }
    }
    
    private void Deactivate()
    {
        isActive = false;
        cooldownTimer = COOLDOWN_TIME;
        if (boostPadRenderer != null)
        {
            boostPadRenderer.material.color = Color.gray;
        }
    }
}

