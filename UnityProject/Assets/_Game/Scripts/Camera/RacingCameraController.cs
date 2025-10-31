using UnityEngine;
using Cinemachine;

public enum CameraMode
{
    Chase,
    Cockpit,
    Cinematic
}

public class RacingCameraController : MonoBehaviour
{
    [Header("Camera Modes")]
    [SerializeField] private CinemachineVirtualCamera chaseCamera;
    [SerializeField] private CinemachineVirtualCamera cockpitCamera;
    [SerializeField] private CinemachineVirtualCamera cinematicCamera;
    
    [Header("Chase Camera Settings")]
    [SerializeField] private float chaseDistance = 10f;
    [SerializeField] private float chaseHeight = 3f;
    
    [Header("Input")]
    [SerializeField] private KeyCode switchCameraKey = KeyCode.C;
    
    private CameraMode currentMode = CameraMode.Chase;
    private Transform target;
    
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            SetupCameras();
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(switchCameraKey))
        {
            SwitchCamera();
        }
    }
    
    private void SetupCameras()
    {
        if (chaseCamera != null && target != null)
        {
            chaseCamera.Follow = target;
            chaseCamera.LookAt = target;
        }
        
        if (cockpitCamera != null && target != null)
        {
            Transform cockpitPoint = target.Find("CockpitPoint");
            if (cockpitPoint != null)
            {
                cockpitCamera.Follow = cockpitPoint;
                cockpitCamera.LookAt = null;
            }
            else
            {
                cockpitCamera.Follow = target;
                cockpitCamera.LookAt = null;
            }
        }
        
        if (cinematicCamera != null && target != null)
        {
            cinematicCamera.Follow = target;
            cinematicCamera.LookAt = target;
        }
        
        SetCameraMode(CameraMode.Chase);
    }
    
    public void SetCameraMode(CameraMode mode)
    {
        currentMode = mode;
        
        if (chaseCamera != null) chaseCamera.Priority = mode == CameraMode.Chase ? 10 : 0;
        if (cockpitCamera != null) cockpitCamera.Priority = mode == CameraMode.Cockpit ? 10 : 0;
        if (cinematicCamera != null) cinematicCamera.Priority = mode == CameraMode.Cinematic ? 10 : 0;
    }
    
    public void SwitchCamera()
    {
        currentMode = (CameraMode)(((int)currentMode + 1) % 3);
        SetCameraMode(currentMode);
    }
    
    public CameraMode GetCurrentMode() => currentMode;
}

