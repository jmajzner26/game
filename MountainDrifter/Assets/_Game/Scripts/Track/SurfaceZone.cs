using UnityEngine;

/// <summary>
/// Defines surface zones with different grip properties (asphalt, dirt, wet, etc.).
/// </summary>
[RequireComponent(typeof(Collider))]
public class SurfaceZone : MonoBehaviour
{
    [Header("Surface Properties")]
    [SerializeField] private SurfaceType surfaceType = SurfaceType.Asphalt;
    [SerializeField] [Range(0f, 1f)] private float gripMultiplier = 1f;
    [SerializeField] private bool affectsVisuals = true;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem surfaceParticles;
    [SerializeField] private AudioClip surfaceSoundEffect;

    private Collider zoneCollider;

    public enum SurfaceType
    {
        Asphalt,
        Dirt,
        Gravel,
        Wet,
        Snow,
        Ice
    }

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
        if (zoneCollider != null && !zoneCollider.isTrigger)
        {
            zoneCollider.isTrigger = true;
        }
    }

    public void Initialize()
    {
        // Initialize surface-specific properties
        switch (surfaceType)
        {
            case SurfaceType.Asphalt:
                gripMultiplier = 1f;
                break;
            case SurfaceType.Dirt:
                gripMultiplier = 0.7f;
                break;
            case SurfaceType.Gravel:
                gripMultiplier = 0.6f;
                break;
            case SurfaceType.Wet:
                gripMultiplier = 0.5f;
                break;
            case SurfaceType.Snow:
                gripMultiplier = 0.4f;
                break;
            case SurfaceType.Ice:
                gripMultiplier = 0.2f;
                break;
        }
    }

    public float GetGripMultiplier()
    {
        return gripMultiplier;
    }

    public SurfaceType GetSurfaceType()
    {
        return surfaceType;
    }

    private void OnTriggerEnter(Collider other)
    {
        DriftCarController car = other.GetComponent<DriftCarController>();
        if (car != null)
        {
            car.SetSurfaceGrip(gripMultiplier);
            
            if (affectsVisuals && surfaceParticles != null)
            {
                surfaceParticles.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DriftCarController car = other.GetComponent<DriftCarController>();
        if (car != null)
        {
            // Reset to default grip (asphalt)
            car.SetSurfaceGrip(1f);
        }
    }
}

