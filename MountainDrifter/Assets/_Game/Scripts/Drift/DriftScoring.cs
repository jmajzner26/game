using UnityEngine;
using System;

/// <summary>
/// Calculates and tracks drift scores based on slip angle, speed, combo duration, and style.
/// Provides real-time scoring feedback for Mountain Drifter.
/// </summary>
public class DriftScoring : MonoBehaviour
{
    [Header("Scoring Parameters")]
    [SerializeField] private float basePointsPerSecond = 10f;
    [SerializeField] private float speedMultiplier = 0.5f;
    [SerializeField] private float angleMultiplier = 2f;
    [SerializeField] private float comboMultiplier = 1.5f;
    [SerializeField] private float comboDecayTime = 2f;

    [Header("Bonus Thresholds")]
    [SerializeField] private float perfectAngleMin = 20f;
    [SerializeField] private float perfectAngleMax = 45f;
    [SerializeField] private float speedBonusThreshold = 60f; // km/h

    private DriftCarController carController;
    private float currentScore = 0f;
    private float comboMultiplierCurrent = 1f;
    private float comboTimer = 0f;
    private float currentAngle = 0f;
    private float currentSpeed = 0f;
    private bool isInDrift = false;
    private float driftDuration = 0f;

    // Style bonuses
    private float longestDrift = 0f;
    private float highestAngle = 0f;
    private float fastestSpeed = 0f;

    // Events
    public event Action<float> OnScoreUpdated;
    public event Action<float> OnComboMultiplierChanged;
    public event Action<string> OnBonusEarned; // Bonus name for UI

    public float CurrentScore => currentScore;
    public float ComboMultiplier => comboMultiplierCurrent;
    public float DriftDuration => driftDuration;
    public float CurrentAngle => currentAngle;
    public float CurrentSpeed => currentSpeed;

    private void Awake()
    {
        carController = GetComponent<DriftCarController>();
        if (carController == null)
        {
            Debug.LogError("DriftScoring requires DriftCarController component!");
        }
    }

    private void Start()
    {
        if (carController != null)
        {
            carController.OnDriftStateChanged += OnDriftStateChanged;
            carController.OnDriftAngleChanged += OnDriftAngleChanged;
            carController.OnSpeedChanged += OnSpeedChanged;
        }
    }

    private void Update()
    {
        if (isInDrift)
        {
            UpdateDriftScore();
            UpdateCombo();
            CheckForBonuses();
        }
        else
        {
            UpdateComboDecay();
        }
    }

    private void UpdateDriftScore()
    {
        driftDuration += Time.deltaTime;

        // Base points per second
        float pointsThisFrame = basePointsPerSecond * Time.deltaTime;

        // Speed multiplier (more speed = more points)
        float speedBonus = (currentSpeed / 100f) * speedMultiplier;
        pointsThisFrame *= (1f + speedBonus);

        // Angle multiplier (optimal angle range = max points)
        float angleBonus = CalculateAngleBonus(currentAngle);
        pointsThisFrame *= (1f + angleBonus * angleMultiplier);

        // Combo multiplier
        pointsThisFrame *= comboMultiplierCurrent;

        // Add to total score
        currentScore += pointsThisFrame;
        OnScoreUpdated?.Invoke(currentScore);
    }

    private float CalculateAngleBonus(float angle)
    {
        float absAngle = Mathf.Abs(angle);

        // Perfect angle range bonus
        if (absAngle >= perfectAngleMin && absAngle <= perfectAngleMax)
        {
            // Maximum bonus when angle is in the middle of perfect range
            float perfectCenter = (perfectAngleMin + perfectAngleMax) / 2f;
            float distanceFromPerfect = Mathf.Abs(absAngle - perfectCenter);
            float maxDistance = (perfectAngleMax - perfectAngleMin) / 2f;
            return 1f - (distanceFromPerfect / maxDistance);
        }
        // Too shallow
        else if (absAngle < perfectAngleMin)
        {
            return absAngle / perfectAngleMin;
        }
        // Too steep
        else
        {
            // Exponential decay for angles beyond perfect
            float excess = absAngle - perfectAngleMax;
            return Mathf.Exp(-excess / 20f);
        }
    }

    private void UpdateCombo()
    {
        // Increase combo multiplier while drifting
        if (driftDuration > comboDecayTime)
        {
            comboMultiplierCurrent = Mathf.Lerp(comboMultiplierCurrent, comboMultiplier, Time.deltaTime);
            comboMultiplierCurrent = Mathf.Min(comboMultiplierCurrent, comboMultiplier * 3f); // Max 3x combo
        }

        comboTimer = comboDecayTime;
        OnComboMultiplierChanged?.Invoke(comboMultiplierCurrent);
    }

    private void UpdateComboDecay()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            
            if (comboTimer <= 0f)
            {
                comboMultiplierCurrent = 1f;
                OnComboMultiplierChanged?.Invoke(comboMultiplierCurrent);
            }
        }
    }

    private void CheckForBonuses()
    {
        // Long drift bonus
        if (driftDuration > longestDrift)
        {
            longestDrift = driftDuration;
            if (driftDuration > 5f && Mathf.FloorToInt(driftDuration) % 5 == 0)
            {
                OnBonusEarned?.Invoke($"Long Drift! {Mathf.FloorToInt(driftDuration)}s");
            }
        }

        // High angle bonus
        if (Mathf.Abs(currentAngle) > highestAngle)
        {
            highestAngle = Mathf.Abs(currentAngle);
            if (highestAngle > 50f)
            {
                OnBonusEarned?.Invoke("Extreme Angle!");
            }
        }

        // Speed bonus
        if (currentSpeed > fastestSpeed)
        {
            fastestSpeed = currentSpeed;
            if (currentSpeed > speedBonusThreshold && Mathf.FloorToInt(currentSpeed) % 10 == 0)
            {
                OnBonusEarned?.Invoke($"Speed Bonus! {Mathf.FloorToInt(currentSpeed)} km/h");
            }
        }

        // Perfect angle bonus
        float absAngle = Mathf.Abs(currentAngle);
        if (absAngle >= perfectAngleMin && absAngle <= perfectAngleMax && currentSpeed > speedBonusThreshold)
        {
            if (UnityEngine.Random.value < 0.1f) // Random chance to avoid spam
            {
                OnBonusEarned?.Invoke("Perfect Angle!");
            }
        }
    }

    private void OnDriftStateChanged(bool drifting)
    {
        isInDrift = drifting;

        if (!drifting)
        {
            // Reset drift duration when not drifting
            if (driftDuration > 1f)
            {
                // Award bonus for drift length
                float driftBonus = driftDuration * 50f;
                currentScore += driftBonus;
                OnScoreUpdated?.Invoke(currentScore);
            }

            driftDuration = 0f;
        }
    }

    private void OnDriftAngleChanged(float angle)
    {
        currentAngle = angle;
    }

    private void OnSpeedChanged(float speed)
    {
        currentSpeed = speed;
    }

    public void ResetScore()
    {
        currentScore = 0f;
        comboMultiplierCurrent = 1f;
        comboTimer = 0f;
        driftDuration = 0f;
        longestDrift = 0f;
        highestAngle = 0f;
        fastestSpeed = 0f;
        OnScoreUpdated?.Invoke(currentScore);
        OnComboMultiplierChanged?.Invoke(comboMultiplierCurrent);
    }

    public float GetMoneyEarned()
    {
        // Convert score to money (1 point = 0.1 money)
        return currentScore * 0.1f;
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

