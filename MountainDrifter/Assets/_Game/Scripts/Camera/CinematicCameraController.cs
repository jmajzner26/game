using UnityEngine;
using Cinemachine;

/// <summary>
/// Cinematic camera controller that dynamically adjusts during drifts.
/// Provides smooth following, shake effects, and drift angle tracking.
/// </summary>
public class CinematicCameraController : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private CinemachineVirtualCamera followCamera;
    [SerializeField] private CinemachineVirtualCamera driftCamera;
    [SerializeField] private CameraShake cameraShake;

    [Header("Camera Settings")]
    [SerializeField] private float driftAngleThreshold = 20f;
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private float driftCameraOffset = 5f;

    [Header("Dynamic FOV")]
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float maxFOV = 75f;
    [SerializeField] private float fovSpeedMultiplier = 0.3f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeIntensity = 0.5f;
    [SerializeField] private float shakeSpeed = 10f;

    private DriftCarController carController;
    private bool isInDrift = false;
    private float currentFOV;

    private void Awake()
    {
        if (followCamera == null)
            followCamera = GetComponent<CinemachineVirtualCamera>();
        
        if (cameraShake == null)
            cameraShake = GetComponent<CameraShake>();

        currentFOV = baseFOV;
    }

    private void Start()
    {
        // Find player car
        carController = FindObjectOfType<DriftCarController>();
        
        if (carController != null)
        {
            carController.OnDriftStateChanged += OnDriftStateChanged;
            carController.OnDriftAngleChanged += OnDriftAngleChanged;
            carController.OnSpeedChanged += OnSpeedChanged;

            // Set follow target
            if (followCamera != null)
            {
                followCamera.Follow = carController.transform;
                followCamera.LookAt = carController.transform;
            }
        }
    }

    private void Update()
    {
        UpdateCameraTransition();
        UpdateFOV();
        UpdateCameraShake();
    }

    private void UpdateCameraTransition()
    {
        if (followCamera == null || driftCamera == null || carController == null)
            return;

        // Switch to drift camera when drifting
        if (isInDrift && Mathf.Abs(carController.DriftAngle) > driftAngleThreshold)
        {
            // Activate drift camera
            driftCamera.Priority = 20;
            followCamera.Priority = 10;

            // Position drift camera to show drift angle better
            Vector3 offset = -carController.transform.right * driftCameraOffset * Mathf.Sign(carController.SlipAngle);
            driftCamera.transform.position = carController.transform.position + offset;
        }
        else
        {
            // Use normal follow camera
            driftCamera.Priority = 10;
            followCamera.Priority = 20;
        }
    }

    private void UpdateFOV()
    {
        if (carController == null || followCamera == null)
            return;

        float targetFOV = baseFOV;

        // Increase FOV based on speed
        float speedFactor = carController.CurrentSpeed / 200f; // Assuming max speed ~200 km/h
        targetFOV = Mathf.Lerp(baseFOV, maxFOV, speedFactor * fovSpeedMultiplier);

        // Increase FOV during drifts
        if (isInDrift)
        {
            targetFOV += 5f;
        }

        // Smooth FOV transition
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * 5f);
        
        if (followCamera.m_Lens.FieldOfView != currentFOV)
        {
            followCamera.m_Lens.FieldOfView = currentFOV;
        }
        
        if (driftCamera != null)
        {
            driftCamera.m_Lens.FieldOfView = currentFOV + 5f; // Slightly wider for drift cam
        }
    }

    private void UpdateCameraShake()
    {
        if (cameraShake == null || carController == null)
            return;

        // Add subtle shake during drifts
        if (isInDrift)
        {
            float intensity = shakeIntensity * (Mathf.Abs(carController.DriftAngle) / 45f);
            cameraShake.AddShake(intensity, shakeSpeed);
        }

        // Shake on high speed
        float speedShake = carController.CurrentSpeed / 300f;
        if (speedShake > 0.3f)
        {
            cameraShake.AddShake(shakeIntensity * 0.3f * speedShake, shakeSpeed * 0.5f);
        }
    }

    private void OnDriftStateChanged(bool drifting)
    {
        isInDrift = drifting;
    }

    private void OnDriftAngleChanged(float angle)
    {
        // Camera can respond to drift angle changes here if needed
    }

    private void OnSpeedChanged(float speed)
    {
        // Speed changes handled in UpdateFOV
    }

    private void OnDestroy()
    {
        if (carController != null)
        {
            carController.OnDriftStateChanged -= OnDriftStateChanged;
            carController.OnDriftAngleChanged -= OnDriftAngleChanged;
            carController.OnSpeedChanged -= OnSpeedChanged;
        }
    }
}

