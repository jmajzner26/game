using UnityEngine;
using Cinemachine;

public class SpeedFOV : MonoBehaviour
{
    [Header("FOV Settings")]
    [SerializeField] private float baseFOV = 70f;
    [SerializeField] private float maxFOV = 85f;
    [SerializeField] private float speedThreshold = 60f; // m/s
    [SerializeField] private float fovIncrease = 10f;
    
    private Camera cam;
    private CinemachineVirtualCamera vcam;
    private VehicleController vehicle;
    
    private void Start()
    {
        cam = GetComponent<Camera>();
        vcam = GetComponent<CinemachineVirtualCamera>();
        
        // Find player vehicle
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            vehicle = player.GetComponent<VehicleController>();
        }
    }
    
    private void Update()
    {
        if (vehicle == null) return;
        
        float currentSpeed = vehicle.CurrentSpeed;
        float targetFOV = baseFOV;
        
        if (currentSpeed > speedThreshold)
        {
            float speedFactor = Mathf.Clamp01((currentSpeed - speedThreshold) / 30f);
            targetFOV = baseFOV + fovIncrease * speedFactor;
            targetFOV = Mathf.Min(targetFOV, maxFOV);
        }
        
        // Apply to camera
        if (cam != null)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 2f);
        }
        
        // Apply to Cinemachine
        if (vcam != null)
        {
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * 2f);
        }
    }
}

