using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tuning menu with sliders for handling, drift balance, and performance adjustments.
/// </summary>
public class TuningMenu : MonoBehaviour
{
    [Header("Tuning Sliders")]
    [SerializeField] private Slider tractionControlSlider;
    [SerializeField] private Slider stabilityControlSlider;
    [SerializeField] private Slider gripMultiplierSlider;
    [SerializeField] private Slider downforceSlider;
    [SerializeField] private Slider weightShiftSlider;

    [Header("Value Displays")]
    [SerializeField] private TextMeshProUGUI tractionControlValue;
    [SerializeField] private TextMeshProUGUI stabilityControlValue;
    [SerializeField] private TextMeshProUGUI gripMultiplierValue;
    [SerializeField] private TextMeshProUGUI downforceValue;
    [SerializeField] private TextMeshProUGUI weightShiftValue;

    [Header("Car Reference")]
    [SerializeField] private DriftCarController carController;

    [Header("Apply Button")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;

    private float originalTractionControl;
    private float originalStabilityControl;
    private float originalGripMultiplier;
    private float originalDownforce;

    private void Start()
    {
        if (carController == null)
            carController = FindObjectOfType<DriftCarController>();

        if (carController == null)
        {
            Debug.LogError("TuningMenu: No DriftCarController found!");
            return;
        }

        InitializeSliders();
        SetupButtons();
    }

    private void InitializeSliders()
    {
        // Set slider ranges
        if (tractionControlSlider != null)
        {
            tractionControlSlider.minValue = 0f;
            tractionControlSlider.maxValue = 1f;
            tractionControlSlider.value = 0.8f;
            tractionControlSlider.onValueChanged.AddListener(OnTractionControlChanged);
        }

        if (stabilityControlSlider != null)
        {
            stabilityControlSlider.minValue = 0f;
            stabilityControlSlider.maxValue = 1f;
            stabilityControlSlider.value = 0.5f;
            stabilityControlSlider.onValueChanged.AddListener(OnStabilityControlChanged);
        }

        if (gripMultiplierSlider != null)
        {
            gripMultiplierSlider.minValue = 0.7f;
            gripMultiplierSlider.maxValue = 1f;
            gripMultiplierSlider.value = 0.95f;
            gripMultiplierSlider.onValueChanged.AddListener(OnGripMultiplierChanged);
        }

        if (downforceSlider != null)
        {
            downforceSlider.minValue = 50f;
            downforceSlider.maxValue = 200f;
            downforceSlider.value = 100f;
            downforceSlider.onValueChanged.AddListener(OnDownforceChanged);
        }

        // Store original values
        originalTractionControl = tractionControlSlider != null ? tractionControlSlider.value : 0.8f;
        originalStabilityControl = stabilityControlSlider != null ? stabilityControlSlider.value : 0.5f;
        originalGripMultiplier = gripMultiplierSlider != null ? gripMultiplierSlider.value : 0.95f;
        originalDownforce = downforceSlider != null ? downforceSlider.value : 100f;

        // Update initial displays
        UpdateAllDisplays();
    }

    private void SetupButtons()
    {
        if (applyButton != null)
            applyButton.onClick.AddListener(OnApplyClicked);

        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetClicked);
    }

    private void OnTractionControlChanged(float value)
    {
        UpdateDisplay(tractionControlValue, value);
    }

    private void OnStabilityControlChanged(float value)
    {
        UpdateDisplay(stabilityControlValue, value);
    }

    private void OnGripMultiplierChanged(float value)
    {
        UpdateDisplay(gripMultiplierValue, value);
    }

    private void OnDownforceChanged(float value)
    {
        UpdateDisplay(downforceValue, value);
    }

    private void UpdateDisplay(TextMeshProUGUI text, float value)
    {
        if (text != null)
        {
            text.text = value.ToString("F2");
        }
    }

    private void UpdateAllDisplays()
    {
        if (tractionControlSlider != null)
            OnTractionControlChanged(tractionControlSlider.value);
        if (stabilityControlSlider != null)
            OnStabilityControlChanged(stabilityControlSlider.value);
        if (gripMultiplierSlider != null)
            OnGripMultiplierChanged(gripMultiplierSlider.value);
        if (downforceSlider != null)
            OnDownforceChanged(downforceSlider.value);
    }

    private void OnApplyClicked()
    {
        if (carController == null) return;

        float tractionControl = tractionControlSlider != null ? tractionControlSlider.value : 0.8f;
        float stabilityControl = stabilityControlSlider != null ? stabilityControlSlider.value : 0.5f;
        float gripMultiplier = gripMultiplierSlider != null ? gripMultiplierSlider.value : 0.95f;

        carController.SetTuning(tractionControl, stabilityControl, gripMultiplier);

        if (downforceSlider != null)
        {
            // Downforce would need to be set separately if DriftCarController exposes it
            // carController.SetDownforce(downforceSlider.value);
        }

        Debug.Log("Tuning applied!");
    }

    private void OnResetClicked()
    {
        if (tractionControlSlider != null)
            tractionControlSlider.value = originalTractionControl;
        if (stabilityControlSlider != null)
            stabilityControlSlider.value = originalStabilityControl;
        if (gripMultiplierSlider != null)
            gripMultiplierSlider.value = originalGripMultiplier;
        if (downforceSlider != null)
            downforceSlider.value = originalDownforce;

        UpdateAllDisplays();
        OnApplyClicked(); // Apply reset values
    }
}

