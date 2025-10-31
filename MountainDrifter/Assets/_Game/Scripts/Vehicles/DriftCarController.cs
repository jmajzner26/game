using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Advanced drift car controller with handbrake, clutch kick, feint drift, and realistic physics.
/// Designed for Mountain Drifter game with emphasis on drift mechanics.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DriftCarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheel;
    [SerializeField] private WheelCollider frontRightWheel;
    [SerializeField] private WheelCollider rearLeftWheel;
    [SerializeField] private WheelCollider rearRightWheel;

    [Header("Wheel Meshes")]
    [SerializeField] private Transform frontLeftMesh;
    [SerializeField] private Transform frontRightMesh;
    [SerializeField] private Transform rearLeftMesh;
    [SerializeField] private Transform rearRightMesh;

    [Header("Car Settings")]
    [SerializeField] private float maxMotorTorque = 1500f;
    [SerializeField] private float maxBrakeTorque = 3000f;
    [SerializeField] private float maxSteerAngle = 35f;
    [SerializeField] private float downforce = 100f;

    [Header("Drift Settings")]
    [SerializeField] private float baseGrip = 0.95f;
    [SerializeField] private float driftGripMultiplier = 0.7f;
    [SerializeField] private float handbrakeGripMultiplier = 0.3f;
    [SerializeField] private float clutchKickPower = 2000f;
    [SerializeField] private float feintDriftPower = 1500f;
    [SerializeField] private float weightShiftSpeed = 5f;

    [Header("Transmission")]
    [SerializeField] private TransmissionController transmission;
    [SerializeField] private bool useAutomaticTransmission = false;

    [Header("Physics Tuning")]
    [SerializeField] private float tractionControl = 0.8f;
    [SerializeField] private float antiLockBrakes = 0.8f;
    [SerializeField] private float stabilityControl = 0.5f;

    // Input
    private InputActionMap drivingMap;
    private float throttleInput;
    private float brakeInput;
    private float steerInput;
    private bool handbrakeInput;
    private bool clutchInput;
    private float clutchValue;

    // Physics
    private Rigidbody rb;
    private float currentSpeed;
    private float slipAngle;
    private Vector3 localVelocity;

    // Drift State
    private bool isDrifting;
    private bool isHandbraking;
    private float driftAngle;
    private float lateralGForce;

    // Advanced Drift Mechanics
    private bool clutchKickActive;
    private float clutchKickTimer;
    private bool feintDriftActive;
    private float feintDriftTimer;

    // Weight Shift (for feint drift)
    private float weightShift = 0f; // -1 to 1, -1 = left, 1 = right

    // Surface grip
    private float surfaceGrip = 1f;

    // Events
    public System.Action<float> OnSpeedChanged;
    public System.Action<float> OnDriftAngleChanged;
    public System.Action<bool> OnDriftStateChanged;

    public float CurrentSpeed => currentSpeed;
    public float SlipAngle => slipAngle;
    public float DriftAngle => driftAngle;
    public bool IsDrifting => isDrifting;
    public float RPM => transmission != null ? transmission.CurrentRPM : 0f;
    public int CurrentGear => transmission != null ? transmission.CurrentGear : 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Lower center of mass for stability

        // Setup transmission if not assigned
        if (transmission == null)
            transmission = GetComponent<TransmissionController>();
        
        if (transmission != null)
            transmission.Initialize(this);
    }

    private void Start()
    {
        SetupInput();
    }

    private void SetupInput()
    {
        var inputActionAsset = Resources.Load<InputActionAsset>("Input/DrivingControls");
        if (inputActionAsset != null)
        {
            drivingMap = inputActionAsset.FindActionMap("Driving");
            if (drivingMap != null)
            {
                drivingMap["Throttle"].performed += ctx => throttleInput = ctx.ReadValue<float>();
                drivingMap["Throttle"].canceled += _ => throttleInput = 0f;
                drivingMap["Brake"].performed += ctx => brakeInput = ctx.ReadValue<float>();
                drivingMap["Brake"].canceled += _ => brakeInput = 0f;
                drivingMap["Steer"].performed += ctx => steerInput = ctx.ReadValue<float>();
                drivingMap["Steer"].canceled += _ => steerInput = 0f;
                drivingMap["Handbrake"].performed += _ => handbrakeInput = true;
                drivingMap["Handbrake"].canceled += _ => handbrakeInput = false;
                drivingMap["Clutch"].performed += ctx => clutchInput = true;
                drivingMap["Clutch"].canceled += _ => clutchInput = false;
                drivingMap.Enable();
            }
        }
    }

    private void FixedUpdate()
    {
        UpdatePhysics();
        UpdateWheels();
        UpdateDriftMechanics();
        CalculateDriftAngle();
        ApplyDownforce();
    }

    private void UpdatePhysics()
    {
        localVelocity = transform.InverseTransformDirection(rb.velocity);
        currentSpeed = rb.velocity.magnitude * 3.6f; // Convert to km/h

        OnSpeedChanged?.Invoke(currentSpeed);
    }

    private void UpdateDriftMechanics()
    {
        // Update clutch kick
        if (clutchKickActive)
        {
            clutchKickTimer -= Time.fixedDeltaTime;
            if (clutchKickTimer <= 0f)
                clutchKickActive = false;
        }

        // Update feint drift
        if (feintDriftActive)
        {
            feintDriftTimer -= Time.fixedDeltaTime;
            if (feintDriftTimer <= 0f)
            {
                feintDriftActive = false;
                weightShift = 0f;
            }
        }

        // Clutch kick activation
        if (clutchInput && !clutchKickActive && currentSpeed > 20f)
        {
            ActivateClutchKick();
        }

        // Feint drift activation (counter-steer technique)
        if (Mathf.Abs(steerInput) > 0.5f && Mathf.Sign(steerInput) != Mathf.Sign(localVelocity.x))
        {
            if (!feintDriftActive && currentSpeed > 30f)
            {
                ActivateFeintDrift();
            }
        }

        // Weight shift for feint drift
        if (feintDriftActive)
        {
            weightShift = Mathf.MoveTowards(weightShift, -Mathf.Sign(steerInput), weightShiftSpeed * Time.fixedDeltaTime);
        }

        // Update clutch value for transmission
        clutchValue = clutchInput ? 1f : 0f;
    }

    private void ActivateClutchKick()
    {
        clutchKickActive = true;
        clutchKickTimer = 0.3f;
        
        // Apply sudden torque to break traction
        float kickPower = clutchKickPower * (1f + currentSpeed / 100f);
        rb.AddRelativeForce(Vector3.forward * kickPower, ForceMode.Acceleration);
    }

    private void ActivateFeintDrift()
    {
        feintDriftActive = true;
        feintDriftTimer = 0.5f;
        
        // Apply sideways force to initiate drift
        float feintPower = feintDriftPower * (1f + currentSpeed / 100f);
        Vector3 feintDirection = transform.right * Mathf.Sign(steerInput);
        rb.AddForce(feintDirection * feintPower, ForceMode.Acceleration);
    }

    private void UpdateWheels()
    {
        // Calculate steering
        float steerAngle = steerInput * maxSteerAngle;
        
        // Speed-sensitive steering
        float speedFactor = Mathf.Clamp01(1f - (currentSpeed / 150f));
        steerAngle *= speedFactor;

        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;

        // Calculate motor torque
        float motorTorque = 0f;
        if (transmission != null)
        {
            motorTorque = transmission.GetMotorTorque(throttleInput, clutchValue, currentSpeed);
        }
        else
        {
            motorTorque = throttleInput * maxMotorTorque;
        }

        // Apply clutch kick
        if (clutchKickActive)
        {
            motorTorque *= 1.5f;
        }

        // Traction control
        float wheelSlip = GetWheelSlip();
        if (wheelSlip > tractionControl)
        {
            motorTorque *= (1f - (wheelSlip - tractionControl));
        }

        // Distribute torque to wheels
        float torquePerWheel = motorTorque / 2f;
        frontLeftWheel.motorTorque = torquePerWheel;
        frontRightWheel.motorTorque = torquePerWheel;

        // Calculate brake torque
        float brakeTorque = brakeInput * maxBrakeTorque;

        // Anti-lock brakes
        if (brakeInput > 0.1f)
        {
            float brakeSlip = GetWheelSlip();
            if (Mathf.Abs(brakeSlip) > antiLockBrakes)
            {
                brakeTorque *= 0.3f; // Reduce brake pressure
            }
        }

        // Apply brakes
        frontLeftWheel.brakeTorque = brakeTorque;
        frontRightWheel.brakeTorque = brakeTorque;
        rearLeftWheel.brakeTorque = brakeTorque;
        rearRightWheel.brakeTorque = brakeTorque;

        // Handbrake
        isHandbraking = handbrakeInput;
        if (handbrakeInput)
        {
            rearLeftWheel.brakeTorque = maxBrakeTorque * 2f;
            rearRightWheel.brakeTorque = maxBrakeTorque * 2f;
        }

        // Update grip based on drift state
        UpdateWheelGrip();

        // Update wheel meshes
        UpdateWheelMeshes();
    }

    private void UpdateWheelGrip()
    {
        float currentGrip = baseGrip * surfaceGrip;

        // Reduce grip when drifting
        if (isDrifting)
        {
            currentGrip *= driftGripMultiplier;
        }

        // Further reduce grip with handbrake
        if (isHandbraking)
        {
            currentGrip *= handbrakeGripMultiplier;
        }

        // Apply grip to wheels
        WheelFrictionCurve forwardFriction = frontLeftWheel.forwardFriction;
        WheelFrictionCurve sidewaysFriction = frontLeftWheel.sidewaysFriction;

        forwardFriction.stiffness = currentGrip;
        sidewaysFriction.stiffness = currentGrip;

        frontLeftWheel.forwardFriction = forwardFriction;
        frontLeftWheel.sidewaysFriction = sidewaysFriction;
        frontRightWheel.forwardFriction = forwardFriction;
        frontRightWheel.sidewaysFriction = sidewaysFriction;

        // Rear wheels get less grip for drifting
        float rearGrip = currentGrip * 0.85f;
        forwardFriction.stiffness = rearGrip;
        sidewaysFriction.stiffness = rearGrip;

        rearLeftWheel.forwardFriction = forwardFriction;
        rearLeftWheel.sidewaysFriction = sidewaysFriction;
        rearRightWheel.forwardFriction = forwardFriction;
        rearRightWheel.sidewaysFriction = sidewaysFriction;
    }

    private float GetWheelSlip()
    {
        WheelHit hit;
        float maxSlip = 0f;

        if (frontLeftWheel.GetGroundHit(out hit))
            maxSlip = Mathf.Max(maxSlip, Mathf.Abs(hit.forwardSlip));
        if (frontRightWheel.GetGroundHit(out hit))
            maxSlip = Mathf.Max(maxSlip, Mathf.Abs(hit.forwardSlip));
        if (rearLeftWheel.GetGroundHit(out hit))
            maxSlip = Mathf.Max(maxSlip, Mathf.Abs(hit.forwardSlip));
        if (rearRightWheel.GetGroundHit(out hit))
            maxSlip = Mathf.Max(maxSlip, Mathf.Abs(hit.forwardSlip));

        return maxSlip;
    }

    private void CalculateDriftAngle()
    {
        // Calculate slip angle (angle between velocity and forward direction)
        float forwardVelocity = localVelocity.z;
        float lateralVelocity = localVelocity.x;
        
        if (Mathf.Abs(forwardVelocity) > 0.1f)
        {
            slipAngle = Mathf.Atan2(lateralVelocity, Mathf.Abs(forwardVelocity)) * Mathf.Rad2Deg;
        }
        else
        {
            slipAngle = 0f;
        }

        // Calculate drift angle (angle between forward and velocity direction)
        driftAngle = Vector3.Angle(transform.forward, rb.velocity.normalized);

        // Determine if drifting (slip angle > threshold)
        bool wasDrifting = isDrifting;
        isDrifting = Mathf.Abs(slipAngle) > 15f && currentSpeed > 20f;

        if (wasDrifting != isDrifting)
        {
            OnDriftStateChanged?.Invoke(isDrifting);
        }

        OnDriftAngleChanged?.Invoke(driftAngle);
    }

    private void ApplyDownforce()
    {
        // Apply downforce proportional to speed squared
        float speedFactor = rb.velocity.sqrMagnitude / 100f;
        rb.AddForce(-transform.up * downforce * speedFactor);
    }

    private void UpdateWheelMeshes()
    {
        UpdateWheelMesh(frontLeftWheel, frontLeftMesh);
        UpdateWheelMesh(frontRightWheel, frontRightMesh);
        UpdateWheelMesh(rearLeftWheel, rearLeftMesh);
        UpdateWheelMesh(rearRightWheel, rearRightMesh);
    }

    private void UpdateWheelMesh(WheelCollider collider, Transform mesh)
    {
        if (mesh == null) return;

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        mesh.position = position;
        mesh.rotation = rotation;
    }

    public void SetSurfaceGrip(float grip)
    {
        surfaceGrip = Mathf.Clamp01(grip);
    }

    public void SetTuning(float tractionControlValue, float stabilityControlValue, float gripMultiplier)
    {
        tractionControl = Mathf.Clamp01(tractionControlValue);
        stabilityControl = Mathf.Clamp01(stabilityControlValue);
        baseGrip = Mathf.Clamp01(gripMultiplier);
    }

    private void OnDisable()
    {
        if (drivingMap != null)
        {
            drivingMap.Disable();
        }
    }

    private void OnDestroy()
    {
        if (drivingMap != null)
        {
            drivingMap.Disable();
        }
    }
}

