using UnityEngine;

/// <summary>
/// Handles manual and automatic transmission systems for drift cars.
/// Provides RPM calculation and gear shifting logic.
/// </summary>
public class TransmissionController : MonoBehaviour
{
    [Header("Transmission Settings")]
    [SerializeField] private bool automatic = false;
    [SerializeField] private float[] gearRatios = { 0f, 2.66f, 1.78f, 1.30f, 1.0f, 0.74f };
    [SerializeField] private float finalDriveRatio = 3.42f;
    [SerializeField] private float engineRPM = 1000f;
    [SerializeField] private float maxRPM = 7000f;
    [SerializeField] private float minRPM = 800f;
    [SerializeField] private float idleRPM = 1000f;

    [Header("Shift Settings")]
    [SerializeField] private float shiftUpRPM = 6000f;
    [SerializeField] private float shiftDownRPM = 2500f;
    [SerializeField] private float shiftDelay = 0.2f;

    private DriftCarController carController;
    private int currentGear = 1;
    private float shiftTimer = 0f;
    private bool isShifting = false;

    public int CurrentGear => currentGear;
    public float CurrentRPM => engineRPM;
    public bool IsAutomatic => automatic;
    public bool IsShifting => isShifting;

    public void Initialize(DriftCarController controller)
    {
        carController = controller;
        engineRPM = idleRPM;
    }

    private void Update()
    {
        if (carController == null) return;

        float currentSpeed = carController.CurrentSpeed / 3.6f; // Convert km/h to m/s
        
        UpdateRPM(currentSpeed);

        if (automatic)
        {
            UpdateAutomaticShift(currentSpeed);
        }

        if (isShifting)
        {
            shiftTimer -= Time.deltaTime;
            if (shiftTimer <= 0f)
            {
                isShifting = false;
            }
        }
    }

    private void UpdateRPM(float speed)
    {
        if (currentGear == 0) // Neutral
        {
            engineRPM = Mathf.Lerp(engineRPM, idleRPM, Time.deltaTime * 5f);
            return;
        }

        // Calculate RPM based on speed and gear ratio
        float wheelRPM = (speed / (2f * Mathf.PI * 0.3f)) * 60f; // Assuming wheel radius of 0.3m
        float targetRPM = wheelRPM * gearRatios[currentGear] * finalDriveRatio;

        // Smooth RPM transition
        engineRPM = Mathf.Lerp(engineRPM, targetRPM, Time.deltaTime * 10f);

        // Clamp RPM
        engineRPM = Mathf.Clamp(engineRPM, minRPM, maxRPM);
    }

    private void UpdateAutomaticShift(float speed)
    {
        if (isShifting) return;

        // Shift up
        if (currentGear < gearRatios.Length - 1 && engineRPM > shiftUpRPM)
        {
            ShiftUp();
        }
        // Shift down
        else if (currentGear > 1 && engineRPM < shiftDownRPM)
        {
            ShiftDown();
        }
    }

    public float GetMotorTorque(float throttleInput, float clutchInput, float currentSpeed)
    {
        if (currentGear == 0 || isShifting || clutchInput > 0.5f)
        {
            return 0f; // No torque in neutral, during shift, or with clutch engaged
        }

        // Calculate torque based on RPM and throttle
        float maxTorque = carController != null ? carController.MaxMotorTorque : 1500f;
        float rpmFactor = GetRPMTorqueFactor();
        float torque = maxTorque * throttleInput * rpmFactor;

        // Apply gear ratio
        if (currentGear > 0 && currentGear < gearRatios.Length)
        {
            torque *= gearRatios[currentGear] * finalDriveRatio;
        }

        return torque;
    }

    private float GetRPMTorqueFactor()
    {
        // Torque curve: low at idle, peaks around mid-RPM, drops at high RPM
        float normalizedRPM = (engineRPM - minRPM) / (maxRPM - minRPM);
        
        // Parabolic curve peaking at 0.6
        float peakPosition = 0.6f;
        float distanceFromPeak = Mathf.Abs(normalizedRPM - peakPosition);
        float torqueFactor = 1f - (distanceFromPeak / peakPosition);
        torqueFactor = Mathf.Clamp01(torqueFactor);
        
        // Minimum torque factor to prevent stalling
        return Mathf.Max(0.3f, torqueFactor);
    }

    public void ShiftUp()
    {
        if (currentGear < gearRatios.Length - 1 && !isShifting)
        {
            currentGear++;
            isShifting = true;
            shiftTimer = shiftDelay;
            engineRPM *= 0.7f; // RPM drop on shift
        }
    }

    public void ShiftDown()
    {
        if (currentGear > 1 && !isShifting)
        {
            currentGear--;
            isShifting = true;
            shiftTimer = shiftDelay;
            engineRPM *= 1.3f; // RPM increase on downshift
        }
    }

    public void SetGear(int gear)
    {
        if (gear >= 0 && gear < gearRatios.Length)
        {
            currentGear = gear;
        }
    }

    public void SetAutomatic(bool auto)
    {
        automatic = auto;
    }

    // Input methods for manual transmission
    public void OnShiftUpInput()
    {
        if (!automatic)
        {
            ShiftUp();
        }
    }

    public void OnShiftDownInput()
    {
        if (!automatic)
        {
            ShiftDown();
        }
    }
}

