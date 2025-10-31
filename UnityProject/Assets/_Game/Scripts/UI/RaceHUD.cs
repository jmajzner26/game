using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceHUD : MonoBehaviour
{
    [Header("Position Display")]
    [SerializeField] private TextMeshProUGUI positionText;
    
    [Header("Lap Display")]
    [SerializeField] private TextMeshProUGUI lapText;
    
    [Header("Timer Display")]
    [SerializeField] private TextMeshProUGUI currentLapTimeText;
    [SerializeField] private TextMeshProUGUI bestLapTimeText;
    
    [Header("Speed Display")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private RectTransform speedBar;
    [SerializeField] private float maxSpeedBar = 100f; // km/h
    
    [Header("Boost Display")]
    [SerializeField] private RectTransform boostMeter;
    [SerializeField] private float maxBoostHeight = 100f;
    
    [Header("Mini-map")]
    [SerializeField] private Camera miniMapCamera;
    [SerializeField] private RawImage miniMapImage;
    [SerializeField] private RectTransform playerDot;
    
    private VehicleController playerVehicle;
    private LapCounter lapCounter;
    private int currentPosition = 1;
    private int totalRacers = 1;
    
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerVehicle = player.GetComponent<VehicleController>();
            lapCounter = player.GetComponent<LapCounter>();
        }
        
        SetupMiniMap();
    }
    
    private void Update()
    {
        UpdatePosition();
        UpdateLap();
        UpdateTimers();
        UpdateSpeed();
        UpdateBoost();
        UpdateMiniMap();
    }
    
    private void UpdatePosition()
    {
        if (positionText != null)
        {
            positionText.text = $"P{currentPosition}/{totalRacers}";
        }
    }
    
    private void UpdateLap()
    {
        if (lapCounter != null && lapText != null)
        {
            lapText.text = $"L{lapCounter.CurrentLap}/{lapCounter.TotalLaps}";
        }
    }
    
    private void UpdateTimers()
    {
        if (lapCounter != null)
        {
            if (currentLapTimeText != null)
            {
                currentLapTimeText.text = FormatTime(lapCounter.CurrentLapTime);
            }
            
            if (bestLapTimeText != null && lapCounter.BestLapTime > 0f)
            {
                bestLapTimeText.text = $"Best: {FormatTime(lapCounter.BestLapTime)}";
            }
        }
    }
    
    private void UpdateSpeed()
    {
        if (playerVehicle != null)
        {
            float speedKmh = playerVehicle.CurrentSpeed * 3.6f; // m/s to km/h
            
            if (speedText != null)
            {
                speedText.text = $"{speedKmh:F0} km/h";
            }
            
            if (speedBar != null)
            {
                float fill = Mathf.Clamp01(speedKmh / maxSpeedBar);
                speedBar.localScale = new Vector3(1f, fill, 1f);
            }
        }
    }
    
    private void UpdateBoost()
    {
        if (playerVehicle != null && boostMeter != null)
        {
            float boostCharge = playerVehicle.BoostCharge;
            boostMeter.sizeDelta = new Vector2(boostMeter.sizeDelta.x, maxBoostHeight * boostCharge);
        }
    }
    
    private void SetupMiniMap()
    {
        if (miniMapCamera != null && miniMapImage != null)
        {
            RenderTexture rt = new RenderTexture(256, 256, 16);
            miniMapCamera.targetTexture = rt;
            miniMapImage.texture = rt;
        }
    }
    
    private void UpdateMiniMap()
    {
        if (playerDot != null && playerVehicle != null)
        {
            // Update player dot position on mini-map
            // This would require coordinate conversion from world to mini-map space
        }
    }
    
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        int ms = Mathf.FloorToInt((seconds % 1f) * 100f);
        return $"{minutes:00}:{secs:00}.{ms:00}";
    }
    
    public void SetPosition(int position, int total)
    {
        currentPosition = position;
        totalRacers = total;
    }
}

