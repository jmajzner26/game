using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private CarConfig carConfig;
    
    [Header("Wheels")]
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider rearLeft;
    [SerializeField] private WheelCollider rearRight;
    
    [Header("Wheel Meshes (Optional)")]
    [SerializeField] private Transform frontLeftMesh;
    [SerializeField] private Transform frontRightMesh;
    [SerializeField] private Transform rearLeftMesh;
    [SerializeField] private Transform rearRightMesh;
    
    // Input
    private InputActionMap drivingMap;
    private float throttleInput;
    private float brakeInput;
    private float steerInput;
    private bool handbrakeInput;
    private bool boostInput;
    
    // Physics
    private Rigidbody rb;
    private float currentSpeed;
    private float motorTorque;
    private float steerAngle;
    
    // Drift & Traction
    private float slipAngle;
    private bool isDrifting;
    private const float DRIFT_THRESHOLD = 8f; // degrees
    private const float TRACTION_CONTROL_SLIP = 0.25f;
    private const float ABS_SLIP = -0.2f;
    
    // Boost
    private bool boostActive = false;
    private float boostTimer = 0f;
    private float boostRechargeTimer = 0f;
    
    // SpeedSafety-sensitive steering
    private const float STEER_REDUCE_SPEED = 40f; // m/s
    
    // Surface grip
    private float currentGripMultiplier = 1f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Setup Rigidbody
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    
    public void Initialize(CarConfig cfg)
    {
        carConfig = cfg;
        
        if (carConfig == null)
        {
            Debug.LogError("CarConfig is null!");
            return;
        }
        
        // Apply config to Rigidbody
        rb.mass = carConfig.mass;
        rb.drag = carConfig.drag;
        rb.angularDrag = carConfig.angularDrag;
        rb.centerOfMass = carConfig.centerOfMassOffset;
        
        // Setup wheel colliders
        ConfigureWheelCollider(frontLeft);
        ConfigureWheelCollider(frontRight);
        ConfigureWheelCollider(rearLeft);
        ConfigureWheelCollider(rearRight);
    }
    
    private void ConfigureWheelCollider(WheelCollider wheel)
    {
        wheel.radius = 0.3f;
        wheel.suspensionDistance = 0.3f;
        wheel.forceAppPointDistance = 0f;
        
        JointSpring spring = wheel.suspensionSpring;
        spring.spring = 35000f;
        spring.damper = 4500f;
        spring.targetPosition = 0.5f;
        wheel.suspensionSpring = spring;
        
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        forwardFriction.extremumSlip = 0.4f;
        forwardFriction.extremumValue = 1f;
        forwardFriction.asymptoteSlip = 0.8f;
        forwardFriction.asymptoteValue = 0.5f;
        forwardFriction.stiffness = carConfig.gripCurve.Evaluate(0f);
        wheel.forwardFriction = forwardFriction;
        
        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.2f;
        sidewaysFriction.extremumValue = 1f;
        sidewaysFriction.asymptoteSlip = 0.5f;
        sidewaysFriction.asymptoteValue = 0.75f;
        sidewaysFriction.stiffness = carConfig.gripCurve.Evaluate(0f);
        wheel.sidewaysFriction = sidewaysFriction;
    }
    
    private void Update()
    {
        // Update wheel meshes
        UpdateWheelMesh(frontLeft, frontLeftMesh);
        UpdateWheelMesh(frontRight, frontRightMesh);
        UpdateWheelMesh(rearLeft, rearLeftMesh);
        UpdateWheelMesh(rearRight, rearRightMesh);
        
        // Calculate current speed
        currentSpeed = rb.velocity.magnitude;
        
        // Calculate slip angle for drift detection
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        slipAngle = Mathf.Atan2(localVelocity.x, localVelocity.z) * Mathf.Rad2Deg;
        isDrifting = Mathf.Abs(slipAngle) > DRIFT_THRESHOLD;
        
        // Update boost timers
        if (boostActive)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                boostActive = false;
                boostRechargeTimer = carConfig.boostRecharge;
            }
        }
        else
        {
            if (boostRechargeTimer > 0f)
            {
                boostRechargeTimer -= Time.deltaTime;
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (carConfig == null) return;
        
        // Apply motor torque
        float motor = CalculateMotorTorque();
        frontLeft.motorTorque = motor;
        frontRight.motorTorque = motor;
        
        // Apply brake
        float brake = CalculateBrakeForce();
        ApplyBraking(brake);
        
        // Apply steering (speed-sensitive)
        steerAngle = CalculateSteerAngle();
        frontLeft.steerAngle = steerAngle;
        frontRight.steerAngle = steerAngle;
        
        // Apply handbrake
        if (handbrakeInput)
        {
            ApplyHandbrake();
        }
        
        // Apply boost
        if (boostActive)
        {
            ApplyBoost();
        }
        
        // Update grip based on drift
        UpdateGrip();
    }
    
    private float CalculateMotorTorque()
    {
        float accelCurveValue = carConfig.accelCurve.Evaluate(currentSpeed / carConfig.maxSpeed);
        float torque = throttleInput * carConfig.enginePower * accelCurveValue;
        
        // Traction control
        float wheelSlip = GetAverageWheelSlip();
        if (wheelSlip > TRACTION_CONTROL_SLIP)
        {
            torque *= 0.5f; // Reduce power
        }
        
        // Limit by max speed
        if (currentSpeed >= carConfig.maxSpeed && throttleInput > 0)
        {
            torque = 0f;
        }
        
        return torque;
    }
    
    private float CalculateBrakeForce()
    {
        if (brakeInput > 0f)
        {
            // ABS - check for wheel lockup
            if (GetAverageWheelSlip() < ABS_SLIP)
            {
                return 0f; // Pulse brake (simplified)
            }
            return brakeInput * carConfig.brakeForce;
        }
        return 0f;
    }
    
    private void ApplyBraking(float brake)
    {
        frontLeft.brakeTorque = brake;
        frontRight.brakeTorque = brake;
        rearLeft.brakeTorque = brake;
        rearRight.brakeTorque = brake;
    }
    
    private float CalculateSteerAngle()
    {
        float steer = steerInput * carConfig.steerSensitivity;
        
        // Speed-sensitive steering reduction
        if (currentSpeed > STEER_REDUCE_SPEED)
        {
            float speedFactor = Mathf.Lerp(1f, 0.6f, (currentSpeed - STEER_REDUCE_SPEED) / 20f);
            steer *= speedFactor;
        }
        
        return steer;
    }
    
    private void ApplyHandbrake()
    {
        rearLeft.brakeTorque = carConfig.brakeForce * 2f;
        rearRight.brakeTorque = carConfig.brakeForce * 2f;
        
        // Reduce rear grip for drift
        WheelFrictionCurve rearLeftSideways = rearLeft.sidewaysFriction;
        WheelFrictionCurve rearRightSideways = rearRight.sidewaysFriction;
        rearLeftSideways.stiffness = 0.3f;
        rearRightSideways.stiffness = 0.3f;
        rearLeft.sidewaysFriction = rearLeftSideways;
        rearRight.sidewaysFriction = rearRightSideways;
    }
    
    private void ApplyBoost()
    {
        if (currentSpeed < carConfig.maxSpeed)
        {
            Vector3 boostForce = transform.forward * carConfig.boostForce;
            rb.AddForce(boostForce, ForceMode.Force);
        }
    }
    
    private void UpdateGrip()
    {
        float gripMultiplier = currentGripMultiplier;
        
        // Reduce lateral grip when drifting
        if (isDrifting)
        {
            gripMultiplier *= carConfig.driftGripMultiplier;
        }
        
        // Apply grip to all wheels
        float slipNormalized = Mathf.Clamp01(Mathf.Abs(slipAngle) / 30f);
        float gripValue = carConfig.gripCurve.Evaluate(slipNormalized) * gripMultiplier;
        
        ApplyGripToWheel(frontLeft, gripValue);
        ApplyGripToWheel(frontRight, gripValue);
        ApplyGripToWheel(rearLeft, gripValue);
        ApplyGripToWheel(rearRight, gripValue);
    }
    
    private void ApplyGripToWheel(WheelCollider wheel, float grip)
    {
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
        forwardFriction.stiffness = grip;
        sidewaysFriction.stiffness = grip;
        wheel.forwardFriction = forwardFriction;
        wheel.sidewaysFriction = sidewaysFriction;
    }
    
    private float GetAverageWheelSlip()
    {
        WheelHit hit;
        float totalSlip = 0f;
        int count = 0;
        
        if (frontLeft.GetGroundHit(out hit)) { totalSlip += hit.forwardSlip; count++; }
        if (frontRight.GetGroundHit(out hit)) { totalSlip += hit.forwardSlip; count++; }
        if (rearLeft.GetGroundHit(out hit)) { totalSlip += hit.forwardSlip; count++; }
        if (rearRight.GetGroundHit(out hit)) { totalSlip += hit.forwardSlip; count++; }
        
        return count > 0 ? totalSlip / count : 0f;
    }
    
    private void UpdateWheelMesh(WheelCollider wheelCollider, Transform wheelMesh)
    {
        if (wheelMesh == null) return;
        
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelMesh.position = position;
        wheelMesh.rotation = rotation;
    }
    
    // Input handlers
    public void OnThrottle(InputAction.CallbackContext context)
    {
        throttleInput = context.ReadValue<float>();
    }
    
    public void OnBrake(InputAction.CallbackContext context)
    {
        brakeInput = context.ReadValue<float>();
    }
    
    public void OnSteer(InputAction.CallbackContext context)
    {
        steerInput = context.ReadValue<float>();
    }
    
    public void OnHandbrake(InputAction.CallbackContext context)
    {
        handbrakeInput = context.performed;
    }
    
    public void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed && boostRechargeTimer <= 0f)
        {
            boostActive = true;
            boostTimer = carConfig.boostDuration;
        }
    }
    
    // Public properties
    public float CurrentSpeed => currentSpeed;
    public bool IsDrifting => isDrifting;
    public bool BoostActive => boostActive;
    public float BoostCharge => Mathf.Clamp01(1f - (boostRechargeTimer / carConfig.boostRecharge));
    public CarConfig Config => carConfig;
    
    public void SetGripMultiplier(float multiplier)
    {
        currentGripMultiplier = multiplier;
    }
    
    // AI Control Methods
    public void SetAIInput(float throttle, float steer, bool handbrake, bool boost)
    {
        throttleInput = throttle;
        steerInput = steer;
        handbrakeInput = handbrake;
        if (boost && boostRechargeTimer <= 0f && !boostActive)
        {
            boostActive = true;
            boostTimer = carConfig.boostDuration;
        }
    }
}

