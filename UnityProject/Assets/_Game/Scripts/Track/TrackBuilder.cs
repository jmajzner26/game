using UnityEngine;
using System.Collections.Generic;

public class TrackBuilder : MonoBehaviour
{
    [Header(" Aspect Configuration")]
    [SerializeField] private TrackConfig trackConfig;
    
    [Header("Road Settings")]
    [SerializeField] private float roadWidth = 10f;
    [SerializeField] private Material roadMaterial;
    [SerializeField] private float segmentLength = 10f;
    
    [Header("Guardrails")]
    [SerializeField] private GameObject guardrailPrefab;
    [SerializeField] private float guardrailHeight = 1.5f;
    [SerializeField] private Material guardrailMaterial;
    
    [Header("Spline")]
    [SerializeField] private Transform splineContainer;
    
    private List<Vector3> splinePoints = new List<Vector3>();
    private List<GameObject> roadSegments = new List<GameObject>();
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    
    public List<Vector3> SplinePoints => splinePoints;
    
    public void BuildTrackFromSpline()
    {
        if (trackConfig == null)
        {
            Debug.LogError("TrackConfig is missing!");
            return;
        }
        
        ClearTrack();
        GenerateSplinePoints();
        BuildRoadSegments();
        BuildGuardrails();
        PlaceCheckpoints();
    }
    
    private void GenerateSplinePoints()
    {
        splinePoints.Clear();
        
        // If using Cinemachine Dolly Track
        if (splineContainer != null)
        {
            var dollyTrack = splineContainer.GetComponent<Cinemachine.CinemachineSmoothPath>();
            if (dollyTrack != null)
            {
                int resolution = Mathf.Max(10, trackConfig.checkpointCount * 3);
                for (int i = 0; i <= resolution; i++)
                {
                    float distance = (float)i / resolution * dollyTrack.PathLength;
                    splinePoints.Add(dollyTrack.EvaluatePositionAtUnit(distance, Cinemachine.CinemachinePathBase.PositionUnits.Distance));
                }
            }
        }
        
        // Fallback: Generate simple oval if no spline
        if (splinePoints.Count == 0)
        {
            GenerateSimpleOval();
        }
        
        trackConfig.checkpointCount = Mathf.Max(1, splinePoints.Count / 3);
    }
    
    private void GenerateSimpleOval()
    {
        int segments = 32;
        float radius = 50f;
        
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius * 0.7f;
            splinePoints.Add(new Vector3(x, 0f, z));
        }
    }
    
    private void BuildRoadSegments()
    {
        GameObject roadParent = new GameObject("RoadSegments");
        roadParent.transform.SetParent(transform);
        
        for (int i = 0; i < splinePoints.Count - 1; i++)
        {
            Vector3 start = splinePoints[i];
            Vector3 end = splinePoints[i + 1];
            Vector3 direction = (end - start).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
            
            float length = Vector3.Distance(start, end);
            
            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Plane);
            segment.name = $"RoadSegment_{i}";
            segment.transform.SetParent(roadParent.transform);
            segment.transform.position = (start + end) / 2f;
            segment.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            segment.transform.localScale = new Vector3(length / 10f, 1f, roadWidth / 10f);
            
            if (roadMaterial != null)
            {
                segment.GetComponent<Renderer>().material = roadMaterial;
            }
            
            roadSegments.Add(segment);
        }
    }
    
    private void BuildGuardrails()
    {
        if (guardrailPrefab == null) return;
        
        GameObject guardrailParent = new GameObject("Guardrails");
        guardrailParent.transform.SetParent(transform);
        
        for (int i = 0; i < splinePoints.Count - 1; i++)
        {
            Vector3 start = splinePoints[i];
            Vector3 end = splinePoints[i + 1];
            Vector3 direction = (end - start).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
            
            float length = Vector3.Distance(start, end);
            
            // Left guardrail
            GameObject leftRail = Instantiate(guardrailPrefab);
            leftRail.name = $"Guardrail_L_{i}";
            leftRail.transform.SetParent(guardrailParent.transform);
            leftRail.transform.position = (start + end) / 2f + right * (roadWidth / 2f + 0.5f);
            leftRail.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            leftRail.transform.localScale = new Vector3(1f, guardrailHeight, length);
            
            // Right guardrail
            GameObject rightRail = Instantiate(guardrailPrefab);
            rightRail.name = $"Guardrail_R_{i}";
            rightRail.transform.SetParent(guardrailParent.transform);
            rightRail.transform.position = (start + end) / 2f - right * (roadWidth / 2f + 0.5f);
            rightRail.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            rightRail.transform.localScale = new Vector3(1f, guardrailHeight, length);
            
            if (guardrailMaterial != null)
            {
                leftRail.GetComponent<Renderer>().material = guardrailMaterial;
                rightRail.GetComponent<Renderer>().material = guardrailMaterial;
            }
        }
    }
    
    private void PlaceCheckpoints()
    {
        GameObject checkpointParent = new GameObject("Checkpoints");
        checkpointParent.transform.SetParent(transform);
        
        int checkpointSpacing = Mathf.Max(1, splinePoints.Count / trackConfig.checkpointCount);
        
        for (int i = 0; i < trackConfig.checkpointCount; i++)
        {
            int pointIndex = (i * checkpointSpacing) % splinePoints.Count;
            Vector3 position = splinePoints[pointIndex];
            
            GameObject checkpointObj = new GameObject($"Checkpoint_{i}");
            checkpointObj.transform.SetParent(checkpointParent.transform);
            checkpointObj.transform.position = position + Vector3.up * 0.5f;
            
            BoxCollider trigger = checkpointObj.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(roadWidth + 5f, 3f, 5f);
            trigger.center = Vector3.zero;
            
            Checkpoint checkpoint = checkpointObj.AddComponent<Checkpoint>();
            checkpoint.SetIndex(i);
            if (i == 0)
            {
                checkpoint.SetAsFinishLine(true);
            }
            
            checkpoints.Add(checkpoint);
        }
    }
    
    private void ClearTrack()
    {
        foreach (var segment in roadSegments)
        {
            if (segment != null) DestroyImmediate(segment);
        }
        roadSegments.Clear();
        
        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint != null) DestroyImmediate(checkpoint.gameObject);
        }
        checkpoints.Clear();
        
        // Clear guardrails
        Transform guardrailParent = transform.Find("Guardrails");
        if (guardrailParent != null) DestroyImmediate(guardrailParent.gameObject);
    }
    
    void OnValidate()
    {
        if (trackConfig != null && trackConfig.checkpointCount < 1)
        {
            trackConfig.checkpointCount = 10;
        }
    }
}

