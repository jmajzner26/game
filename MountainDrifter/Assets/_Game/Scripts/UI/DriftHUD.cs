using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// In-game HUD displaying speedometer, RPM, gear, drift score, and combo multiplier.
/// Minimalist design with clean UI elements.
/// </summary>
public class DriftHUD : MonoBehaviour
{
    [Header("Speed Display")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private float maxSpeedDisplay = 200f; // km/h

    [Header("RPM Display")]
    [SerializeField] private TextMeshProUGUI rpmText;
    [SerializeField] private Image rpmGauge;
    [SerializeField] private Color rpmNormalColor = Color.white;
    [SerializeField] private Color rpmHighColor = Color.red;
    [SerializeField] private float maxRPMDisplay = 7000f;
    [SerializeField] private float redlineRPM = 6000f;

    [Header("Gear Display")]
    [SerializeField] private TextMeshProUGUI gearText;

    [Header("Drift Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboMultiplierText;
    [SerializeField] private GameObject comboMultiplierPanel;

    [Header("Drift Angle")]
    [SerializeField] private TextMeshProUGUI driftAngleText;
    [SerializeField] private Image driftAngleIndicator;

    private DriftCarController carController;
    private DriftScoring driftScoring;
    private TransmissionController transmission;

    private void Start()
    {
        // Find player car
        carController = FindObjectOfType<DriftCarController>();
        
        if (carController != null)
        {
            transmission = carController.GetComponent<TransmissionController>();
            
            carController.OnSpeedChanged += OnSpeedChanged;
            carController.OnDriftStateChanged += OnDriftStateChanged;

            driftScoring = carController.GetComponent<DriftScoring>();
            if (driftScoring != null)
            {
                driftScoring.OnScoreUpdated += OnScoreUpdated;
                driftScoring.OnComboMultiplierChanged += OnComboMultiplierChanged;
            }
        }
    }

    private void Update()
    {
        UpdateRPM();
        UpdateGear();
        UpdateDriftAngle();
    }

    private void OnSpeedChanged(float speed)
    {
        if (speedText != null)
        {
            speedText.text = Mathf.FloorToInt(speed).ToString();
        }

        if (speedSlider != null)
        {
            speedSlider.value = Mathf.Clamp01(speed / maxSpeedDisplay);
        }
    }

    private void UpdateRPM()
    {
        if (transmission == null || rpmText == null || rpmGauge == null)
            return;

        float rpm = transmission.CurrentRPM;

        // Update RPM text
        rpmText.text = Mathf.FloorToInt(rpm).ToString();

        // Update RPM gauge fill
        float rpmNormalized = Mathf.Clamp01(rpm / maxRPMDisplay);
        rpmGauge.fillAmount = rpmNormalized;

        // Change color near redline
        if (rpm > redlineRPM)
        {
            rpmGauge.color = Color.Lerp(rpmGauge.color, rpmHighColor, Time.deltaTime * 5f);
        }
        else
        {
            rpmGauge.color = Color.Lerp(rpmGauge.color, rpmNormalColor, Time.deltaTime * 5f);
        }
    }

    private void UpdateGear()
    {
        if (transmission == null || gearText == null)
            return;

        int gear = transmission.CurrentGear;
        string gearDisplay = gear == 0 ? "N" : (gear == -1 ? "R" : gear.ToString());
        gearText.text = gearDisplay;
    }

    private void UpdateDriftAngle()
    {
        if (carController == null)
            return;

        float driftAngle = carController.DriftAngle;
        bool isDrifting = carController.IsDrifting;

        if (driftAngleText != null)
        {
            if (isDrifting)
            {
                driftAngleText.text = $"{Mathf.FloorToInt(Mathf.Abs(driftAngle))}°";
                driftAngleText.color = Color.white;
            }
            else
            {
                driftAngleText.text = "";
            }
        }

        // Update drift angle indicator (could be a visual gauge)
        if (driftAngleIndicator != null)
        {
            float normalizedAngle = Mathf.Clamp01(Mathf.Abs(driftAngle) / 45f);
            driftAngleIndicator.fillAmount = normalizedAngle;
        }
    }

    private void OnScoreUpdated(float score)
    {
        if (scoreText != null)
        {
            scoreText.text = Mathf.FloorToInt(score).ToString();
        }
    }

    private void OnComboMultiplierChanged(float multiplier)
    {
        if (comboMultiplierText != null)
        {
            if (multiplier > 1f)
            {
                comboMultiplierText.text = $"x{multiplier:F1}";
                if (comboMultiplierPanel != null)
                {
                    comboMultiplierPanel.SetActive(true);
                }
            }
            else
            {
                if (comboMultiplierPanel != null)
                {
                    comboMultiplierPanel.SetActive(false);
                }
            }
        }
    }

    private void OnDriftStateChanged(bool drifting)
    {
        // Could add visual effects when entering/exiting drift
    }

    private void OnDestroy()
    {
        if (carController != null)
        {
            carController.OnSpeedChanged -= OnSpeedChanged;
            carController.OnDriftStateChanged -= OnDriftStateChanged;
        }

        if (driftScoring != null)
        {
            driftScoring.OnScoreUpdated -= OnScoreUpdated;
            driftScoring.OnComboMultiplierChanged -= OnComboMultiplierChanged;
        }
    }
}

