using UnityEngine;

/// <summary>
/// ScriptableObject containing car specifications and stats.
/// Create car data assets for different vehicles in the game.
/// </summary>
[CreateAssetMenu(fileName = "New Car Data", menuName = "Mountain Drifter/Car Data")]
public class CarData : ScriptableObject
{
    [Header("Car Info")]
    public string carID;
    public string carName;
    public string manufacturer;
    [TextArea(2, 4)]
    public string description;
    public Sprite carIcon;

    [Header("Price & Unlock")]
    public float purchasePrice;
    public int reputationRequired;

    [Header("Performance Stats")]
    [Range(500f, 5000f)]
    public float maxMotorTorque = 1500f;
    [Range(1000f, 5000f)]
    public float maxBrakeTorque = 3000f;
    [Range(20f, 50f)]
    public float maxSteerAngle = 35f;
    [Range(50f, 200f)]
    public float downforce = 100f;

    [Header("Drift Characteristics")]
    [Range(0.5f, 1f)]
    public float baseGrip = 0.95f;
    [Range(0.3f, 0.9f)]
    public float driftGripMultiplier = 0.7f;
    [Range(0.1f, 0.5f)]
    public float handbrakeGripMultiplier = 0.3f;

    [Header("Tuning Limits")]
    public float minTractionControl = 0.3f;
    public float maxTractionControl = 1f;
    public float minStabilityControl = 0f;
    public float maxStabilityControl = 1f;
    public float minGripMultiplier = 0.7f;
    public float maxGripMultiplier = 1f;

    [Header("Transmission")]
    public bool defaultAutomatic = false;
    public float[] gearRatios = { 0f, 2.66f, 1.78f, 1.30f, 1.0f, 0.74f };
    public float finalDriveRatio = 3.42f;
    public float maxRPM = 7000f;

    [Header("Visual")]
    public GameObject carPrefab;
    public Material[] defaultPaintMaterials;
    public Vector3 previewRotation = Vector3.zero;
}

