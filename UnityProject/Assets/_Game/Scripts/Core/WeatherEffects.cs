using UnityEngine;

public class WeatherEffects : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem rainParticleSystem;
    [SerializeField] private ParticleSystem snowParticleSystem;
    
    [Header("Wind Settings")]
    [SerializeField] private float windStrength = 1f;
    [SerializeField] private Vector3 windDirection = Vector3.forward;
    
    [Header("Surface Effects")]
    [SerializeField] private float wetGripMultiplier = 0.7f;
    [SerializeField] private float snowGripMultiplier = 0.5f;
    
    private TrackConfig.Weather currentWeather = TrackConfig.Weather.Clear;
    
    public void SetWeather(TrackConfig.Weather weather)
    {
        currentWeather = weather;
        
        // Stop all particle systems first
        if (rainParticleSystem != null) rainParticleSystem.Stop();
        if (snowParticleSystem != null) snowParticleSystem.Stop();
        
        switch (weather)
        {
            case TrackConfig.Weather.Clear:
                // No particles
                break;
                
            case TrackConfig.Weather.Rain:
                if (rainParticleSystem != null)
                {
                    rainParticleSystem.Play();
                }
                ApplyWetSurface();
                break;
                
            case TrackConfig.Weather.Snow:
                if (snowParticleSystem != null)
                {
                    snowParticleSystem.Play();
                }
                ApplySnowSurface();
                break;
        }
    }
    
    private void ApplyWetSurface()
    {
        // Find all surface zones and apply wet grip
        SurfaceZone[] surfaceZones = FindObjectsOfType<SurfaceZone>();
        foreach (var zone in surfaceZones)
        {
            // This would modify grip multiplier
            // Would need a method in SurfaceZone to set grip
        }
        
        // Apply to all vehicles
        VehicleController[] vehicles = FindObjectsOfType<VehicleController>();
        foreach (var vehicle in vehicles)
        {
            vehicle.SetGripMultiplier(wetGripMultiplier);
        }
    }
    
    private void ApplySnowSurface()
    {
        VehicleController[] vehicles = FindObjectsOfType<VehicleController>();
        foreach (var vehicle in vehicles)
        {
            vehicle.SetGripMultiplier(snowGripMultiplier);
        }
    }
    
    private void Update()
    {
        // Apply wind force to vehicles if weather is active
        if (currentWeather != TrackConfig.Weather.Clear)
        {
            VehicleController[] vehicles = FindObjectsOfType<VehicleController>();
            foreach (var vehicle in vehicles)
            {
                Rigidbody rb = vehicle.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 windForce = windDirection.normalized * windStrength * rb.mass;
                    rb.AddForce(windForce * Time.deltaTime);
                }
            }
        }
    }
}

