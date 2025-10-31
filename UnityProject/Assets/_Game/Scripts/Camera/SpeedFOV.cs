using UnityEngine;
using Cinemachine;

public class SpeedFOV : MonoBehaviour
{
    [Header("FOV Settings")]
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float maxFOV = 75f;
    [SerializeField] private float fovChangeSpeed = 2f;
    [SerializeField] private float speedThreshold = 20f; // m/s
    
    [Header("Motion Blur")]
    [SerializeField] private bool enableMotionBlur = true;
    [SerializeField] private float motionBlurIntensity = 0.5f;
    
    private VehicleController targetVehicle;
    private Camera cameraComponent;
    private CinemachineVirtualCamera cinemachineCamera;
    private float currentFOV;
    
    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
        cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        
        if (cameraComponent != null)
        {
            currentFOV = cameraComponent.fieldOfView;
        }
        else if (cinemachineCamera != null)
        {
            currentFOV = cinemachineCamera.m_Lens.FieldOfView;
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            targetVehicle = player.GetComponent<VehicleController>();
        }
    }
    
    private void Update()
    {
        if (targetVehicle == null) return;
        
        float speed = targetVehicle.CurrentSpeed;
        float normalizedSpeed = Mathf.Clamp01((speed - speedThreshold) / (targetVehicle.Config.maxSpeed - speedThreshold));
        
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, normalizedSpeed);
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovChangeSpeed);
        
        if (cameraComponent != null)
        {
            cameraComponent.fieldOfView = currentFOV;
        }
        
        if (cinemachineCamera != null)
        {
            cinemachineCamera.m_Lens.FieldOfView = currentFOV;
        }
    }
}

