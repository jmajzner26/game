using UnityEngine;
using System.Collections.Generic;

public class AIDriver : MonoBehaviour
{
    [Header("AI Configuration")]
    [SerializeField] private List<Vector3> splinePoints = new List<Vector3>();
    [SerializeField] private float lookAheadDistance = 15f;
    [SerializeField] private float maxSpeed = 70f;
    
    [Header("PID Steering")]
    [SerializeField] private float pGain = 1f;
    [SerializeField] private float iGain = 0.1f;
    [SerializeField] private float dGain = 0.5f;
    
    [Header("PID Throttle")]
    [SerializeField] private float throttleP = 2f;
    [SerializeField] private float throttleI = 0.1f;
    [SerializeField] private float throttleD = 0.3f;
    
    [Header("Rubber-Banding")]
    [SerializeField] private bool useRubberBanding = true;
    [SerializeField] private float rubberBandRange = 50f;
    
    [Header("Overtaking")]
    [SerializeField] private float overtakeDistance = 10f;
    [SerializeField] private float laneOffset = 3f;
    
    private VehicleController vehicleController;
    private int currentWaypointIndex = 0;
    private Vector3 targetPosition;
    
    // PID Steering
    private float steerIntegral = 0f;
    private float steerLastError = 0f;
    
    // PID Throttle
    private float throttleIntegral = 0f;
    private float throttleLastError = 0f;
    
    // Rubber-banding
    private Transform playerCar;
    private float baseEnginePower;
    
    private void Awake()
    {
        vehicleController = GetComponent<VehicleController>();
        if (vehicleController == null)
        {
            Debug.LogError("AIDriver requires VehicleController component!");
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCar = player.transform;
        }
    }
    
    private void Start()
    {
        if (vehicleController != null && vehicleController.Config != null)
        {
            baseEnginePower = vehicleController.Config.enginePower;
        }
        
        // Initialize to first waypoint
        if (splinePoints.Count > 0)
        {
            targetPosition = splinePoints[0];
        }
    }
    
    private void FixedUpdate()
    {
        if (vehicleController == null || splinePoints.Count == 0) return;
        
        UpdateTargetPosition();
        ApplySteering();
        ApplyThrottle();
        ApplyRubberBanding();
    }
    
    private void UpdateTargetPosition()
    {
        Vector3 currentPos = transform.position;
        
        // Find nearest waypoint
        float nearestDist = float.MaxValue;
        int nearestIndex = currentWaypointIndex;
        
        for (int i = 0; i < splinePoints.Count; i++)
        {
            float dist = Vector3.Distance(currentPos, splinePoints[i]);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestIndex = i;
            }
        }
        
        currentWaypointIndex = nearestIndex;
        
        // Look ahead
        int lookAheadIndex = (currentWaypointIndex + 1) % splinePoints.Count;
        targetPosition = splinePoints[lookAheadIndex];
        
        // Apply look-ahead distance offset
        Vector3 direction = (targetPosition - currentPos).normalized;
        targetPosition = currentPos + direction * lookAheadDistance;
    }
    
    private void ApplySteering()
    {
        Vector3 toTarget = targetPosition - transform.position;
        Vector3 forward = transform.forward;
        
        // Calculate cross product to get steering direction
        float cross = Vector3.Cross(forward, toTarget.normalized).y;
        
        // Calculate angle error
        float angleError = Vector3.SignedAngle(forward, toTarget, Vector3.up);
        
        // PID control
        steerIntegral += angleError * Time.fixedDeltaTime;
        float derivative = (angleError - steerLastError) / Time.fixedDeltaTime;
        float steerOutput = pGain * angleError + iGain * steerIntegral + dGain * derivative;
        
        steerLastError = angleError;
        
        // Clamp and apply
        float steer = Mathf.Clamp(steerOutput / 45f, -1f, 1f);
        
        // Use Input System callback simulation
        var context = new UnityEngine.InputSystem.InputAction.CallbackContext();
        vehicleController.OnSteer(new UnityEngine.InputSystem.InputAction.CallbackContext());
        // Direct control instead:
        vehicleController.GetComponent<VehicleController>().GetType().GetField("steerInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(vehicleController, steer);
    }
    
    private void ApplyThrottle()
    {
        float currentSpeed = vehicleController.CurrentSpeed;
        float speedError = maxSpeed - currentSpeed;
        
        // PID control
        throttleIntegral += speedError * Time.fixedDeltaTime;
        float derivative = (speedError - throttleLastError) / Time.fixedDeltaTime;
        float throttleOutput = throttleP * speedError + throttleI * throttleIntegral + throttleD * derivative;
        
        throttleLastError = speedError;
        
        // Clamp
        float throttle = Mathf.Clamp01(throttleOutput / 20f);
        
        // Apply (similar to steering - would need reflection or public method)
        // For now, assume VehicleController has public methods or we modify it
    }
    
    private void ApplyRubberBanding()
    {
        if (!useRubberBanding || playerCar == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerCar.position);
        float normalizedDistance = distanceToPlayer / rubberBandRange;
        
        // Scale engine power based on distance
        // Closer = slower (0.9x), Further = faster (1.1x)
        float powerMultiplier = Mathf.Lerp(0.9f, 1.1f, normalizedDistance);
        
        // This would need a method in VehicleController to adjust engine power
        // Or we modify CarConfig at runtime (not ideal)
    }
    
    public void SetSplinePoints(List<Vector3> points)
    {
        splinePoints = new List<Vector3>(points);
    }
    
    public void SetMaxSpeed(float speed)
    {
        maxSpeed = speed;
    }
}

