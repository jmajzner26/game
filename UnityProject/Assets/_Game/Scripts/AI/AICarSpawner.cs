using UnityEngine;
using System.Collections.Generic;

public class AICarSpawner : MonoBehaviour
{
    [Header("Car Prefab")]
    [SerializeField] private GameObject aiCarPrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private int numberOfAICars = 5;
    [SerializeField] private float spawnDistance = 10f;
    [SerializeField] private float spawnOffset = 3f; // Lateral offset per car
    
    [Header("Spline Reference")]
    [SerializeField] private TrackBuilder trackBuilder;
    [SerializeField] private List<Vector3> spawnPoints = new List<Vector3>();
    
    [Header("AI Car Configs")]
    [SerializeField] private CarConfig[] aiCarConfigs;
    
    private List<GameObject> spawnedAICars = new List<GameObject>();
    
    private void Start()
    {
        if (trackBuilder == null)
        {
            trackBuilder = FindObjectOfType<TrackBuilder>();
        }
        
        SpawnAICars();
    }
    
    public void SpawnAICars()
    {
        if (aiCarPrefab == null || trackBuilder == null)
        {
            Debug.LogWarning("AI Car Prefab or TrackBuilder is missing!");
            return;
        }
        
        // Get spline points from track builder
        List<Vector3> splinePoints = trackBuilder.SplinePoints;
        if (splinePoints == null || splinePoints.Count == 0)
        {
            Debug.LogWarning("No spline points found!");
            return;
        }
        
        // Calculate spawn positions along the track
        CalculateSpawnPositions(splinePoints);
        
        // Spawn AI cars
        for (int i = 0; i < numberOfAICars && i < spawnPoints.Count; i++)
        {
            SpawnAICar(spawnPoints[i], splinePoints, i);
        }
    }
    
    private void CalculateSpawnPositions(List<Vector3> splinePoints)
    {
        spawnPoints.Clear();
        
        // Start from a point after the finish line
        int startIndex = Mathf.Max(1, splinePoints.Count / 10);
        
        for (int i = 0; i < numberOfAICars; i++)
        {
            int pointIndex = (startIndex + i * (splinePoints.Count / numberOfAICars)) % splinePoints.Count;
            Vector3 spawnPos = splinePoints[pointIndex];
            
            // Add lateral offset
            if (pointIndex < splinePoints.Count - 1)
            {
                Vector3 direction = (splinePoints[pointIndex + 1] - splinePoints[pointIndex]).normalized;
                Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
                spawnPos += right * spawnOffset * (i % 2 == 0 ? 1f : -1f);
            }
            
            spawnPoints.Add(spawnPos);
        }
    }
    
    private void SpawnAICar(Vector3 position, List<Vector3> splinePoints, int index)
    {
        GameObject aiCar = Instantiate(aiCarPrefab, position, Quaternion.identity);
        aiCar.name = $"AICar_{index}";
        
        // Setup VehicleController
        VehicleController vehicleController = aiCar.GetComponent<VehicleController>();
        if (vehicleController != null && aiCarConfigs != null && aiCarConfigs.Length > 0)
        {
            CarConfig config = aiCarConfigs[index % aiCarConfigs.Length];
            vehicleController.Initialize(config);
        }
        
        // Setup AIDriver
        AIDriver aiDriver = aiCar.GetComponent<AIDriver>();
        if (aiDriver == null)
        {
            aiDriver = aiCar.AddComponent<AIDriver>();
        }
        
        if (aiDriver != null)
        {
            aiDriver.SetSplinePoints(splinePoints);
            
            // Vary AI difficulty by max speed
            float baseSpeed = 70f;
            float speedVariation = Random.Range(-5f, 5f);
            aiDriver.SetMaxSpeed(baseSpeed + speedVariation);
        }
        
        spawnedAICars.Add(aiCar);
    }
    
    public List<GameObject> GetAICars()
    {
        return spawnedAICars;
    }
    
    public void ClearAICars()
    {
        foreach (var car in spawnedAICars)
        {
            if (car != null)
            {
                Destroy(car);
            }
        }
        spawnedAICars.Clear();
    }
}

