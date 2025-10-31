using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CrashResponder : MonoBehaviour
{
    [Header("Impact Settings")]
    [SerializeField] private float impactThreshold = 5f;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 0.15f;
    
    [Header("Chromatic Aberration")]
    [SerializeField] private float chromaIntensity = 0.5f;
    [SerializeField] private float chromaDuration = 0.2f;
    
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private CameraShake cameraShake;
    private AudioSource audioSource;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraShake = Camera.main?.GetComponent<CameraShake>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    private void FixedUpdate()
    {
        if (rb != null)
        {
            lastVelocity = rb.velocity;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;
        
        if (impactForce > impactThreshold)
        {
            HandleImpact(impactForce);
        }
    }
    
    private void HandleImpact(float force)
    {
        // Play sound
        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound, Mathf.Clamp01(force / 20f));
        }
        
        // Camera shake
        if (cameraShake != null)
        {
            cameraShake.Shake(shakeDuration, shakeMagnitude * (force / impactThreshold));
        }
        
        // Chromatic aberration (if using URP)
        // This would require accessing URP volume profile
        // Placeholder for visual effect
    }
}

