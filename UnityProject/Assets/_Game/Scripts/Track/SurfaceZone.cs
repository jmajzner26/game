using UnityEngine;

public class SurfaceZone : MonoBehaviour
{
    [Header("Surface Configuration")]
    [SerializeField] private TrackConfig.SurfaceType surfaceType = TrackConfig.SurfaceType.Asphalt;
    
    [Header("Grip Settings")]
    [Range(0.1f, 2f)]
    [SerializeField] private float gripMultiplier = 1f;
    [Range(0f, 0.1f)]
    [SerializeField] private float rollingResistance = 0.02f;
    
    private BoxCollider triggerZone;
    
    private void Awake()
    {
        triggerZone = GetComponent<BoxCollider>();
        if (triggerZone == null)
        {
            triggerZone = gameObject.AddComponent<BoxCollider>();
            triggerZone.isTrigger = true;
            triggerZone.size = new Vector3(20f, 5f, 20f);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        VehicleController vehicle = other.GetComponent<VehicleController>();
        if (vehicle != null)
        {
            vehicle.SetGripMultiplier(gripMultiplier);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        VehicleController vehicle = other.GetComponent<VehicleController>();
        if (vehicle != null)
        {
            vehicle.SetGripMultiplier(1f); // Reset to default
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        VehicleController vehicle = other.GetComponent<VehicleController>();
        if (vehicle != null)
        {
            // Apply rolling resistance
            Rigidbody rb = vehicle.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 resistance = -rb.velocity.normalized * rollingResistance * rb.mass;
                rb.AddForce(resistance, ForceMode.Force);
            }
        }
    }
    
    public float GetGripMultiplier()
    {
        // Surface type defaults
        switch (surfaceType)
        {
            case TrackConfig.SurfaceType.Asphalt:
                return gripMultiplier != 1f ? gripMultiplier : 1f;
            case TrackConfig.SurfaceType.Dirt:
                return gripMultiplier != 1f ? gripMultiplier : 0.7f;
            case TrackConfig.SurfaceType.Snow:
                return gripMultiplier != 1f ? gripMultiplier : 0.5f;
            case TrackConfig.SurfaceType.NeonWet:
                return gripMultiplier != 1f ? gripMultiplier : 0.85f;
            default:
                return gripMultiplier;
        }
    }
}

