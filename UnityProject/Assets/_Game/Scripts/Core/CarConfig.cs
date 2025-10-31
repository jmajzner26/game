using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewCarConfig", menuName = "Polytrack/Car Config", order = 1)]
public class CarConfig : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string displayName;
    
    [Header("Visual")]
    public GameObject mesh;
    public Material material;
    
    [Header("Physics")]
    [Range(800f, 2000f)]
    public float mass = 1200f;
    [Range(0.01f, 0.1f)]
    public float drag = 0.03f;
    [Range(0.01f, 0.1f)]
    public float angularDrag = 0.05f;
    public Vector3 centerOfMassOffset = new Vector3(0, -0.3f, 0);
    
    [Header("Engine")]
    [Range(200f, 800f)]
    public float enginePower = 420f;
    [Range(50f, 100f)]
    public float maxSpeed = 78f; // m/s
    public AnimationCurve accelCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.8f);
    
    [Header("Braking")]
    [Range(3000f, 10000f)]
    public float brakeForce = 6000f;
    
    [Header("Steering")]
    [Range(1f, 5f)]
    public float steerSensitivity = 2.1f;
    
    [Header("Drift")]
    [Range(0.3f, 1f)]
    public float driftGripMultiplier = 0.55f;
    public AnimationCurve gripCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.4f);
    
    [Header("Boost")]
    [Range(10000f, 50000f)]
    public float boostForce = 30000f; // Newtons
    [Range(1f, 5f)]
    public float boostDuration = 2f;
    [Range(3f, 15f)]
    public float boostRecharge = 8f;
    
    void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
            id = name.ToLower().Replace(" ", "_");
    }
}

