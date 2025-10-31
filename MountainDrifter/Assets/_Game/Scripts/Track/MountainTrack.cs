using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Mountain track system for downhill drift runs.
/// Manages track splines, checkpoints, and environmental elements.
/// </summary>
public class MountainTrack : MonoBehaviour
{
    [Header("Track Configuration")]
    [SerializeField] private Transform[] trackSplinePoints;
    [SerializeField] private float trackWidth = 10f;
    [SerializeField] private bool isLoop = false;

    [Header("Checkpoints")]
    [SerializeField] private Transform[] checkpoints;
    [SerializeField] private bool useCheckpoints = true;

    [Header("Environment")]
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform finishPosition;
    [SerializeField] private List<GameObject> shortcuts;

    [Header("Surface Zones")]
    [SerializeField] private List<SurfaceZone> surfaceZones;

    private int currentCheckpointIndex = 0;
    private bool raceStarted = false;
    private float raceStartTime = 0f;

    public bool RaceStarted => raceStarted;
    public float RaceTime => Time.time - raceStartTime;
    public int CurrentCheckpointIndex => currentCheckpointIndex;
    public int TotalCheckpoints => checkpoints.Length;

    private void Start()
    {
        if (startPosition == null)
        {
            // Auto-create start position at first checkpoint or first spline point
            if (checkpoints != null && checkpoints.Length > 0)
            {
                startPosition = checkpoints[0];
            }
            else if (trackSplinePoints != null && trackSplinePoints.Length > 0)
            {
                GameObject startObj = new GameObject("Start Position");
                startObj.transform.position = trackSplinePoints[0].position;
                startObj.transform.rotation = trackSplinePoints[0].rotation;
                startPosition = startObj.transform;
            }
        }

        InitializeSurfaceZones();
    }

    private void InitializeSurfaceZones()
    {
        foreach (var zone in surfaceZones)
        {
            if (zone != null)
            {
                zone.Initialize();
            }
        }
    }

    public void StartRace()
    {
        raceStarted = true;
        currentCheckpointIndex = 0;
        raceStartTime = Time.time;
    }

    public void PassCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == currentCheckpointIndex)
        {
            currentCheckpointIndex++;

            // Check if race is complete
            if (currentCheckpointIndex >= checkpoints.Length)
            {
                OnRaceComplete();
            }
        }
    }

    private void OnRaceComplete()
    {
        raceStarted = false;
        // Trigger race complete event
        Debug.Log($"Race completed in {RaceTime:F2} seconds!");
    }

    public Vector3 GetPositionAtDistance(float distance)
    {
        if (trackSplinePoints == null || trackSplinePoints.Length < 2)
            return transform.position;

        // Simple linear interpolation between spline points
        float totalLength = GetTrackLength();
        float normalizedDistance = Mathf.Clamp01(distance / totalLength);

        if (isLoop)
        {
            normalizedDistance = normalizedDistance % 1f;
        }

        float pointIndex = normalizedDistance * (trackSplinePoints.Length - 1);
        int indexA = Mathf.FloorToInt(pointIndex);
        int indexB = Mathf.CeilToInt(pointIndex);

        if (indexB >= trackSplinePoints.Length)
            indexB = isLoop ? 0 : trackSplinePoints.Length - 1;

        float t = pointIndex - indexA;

        Vector3 posA = trackSplinePoints[indexA].position;
        Vector3 posB = trackSplinePoints[indexB].position;

        return Vector3.Lerp(posA, posB, t);
    }

    public float GetTrackLength()
    {
        if (trackSplinePoints == null || trackSplinePoints.Length < 2)
            return 0f;

        float length = 0f;
        for (int i = 0; i < trackSplinePoints.Length - 1; i++)
        {
            length += Vector3.Distance(trackSplinePoints[i].position, trackSplinePoints[i + 1].position);
        }

        if (isLoop && trackSplinePoints.Length > 0)
        {
            length += Vector3.Distance(trackSplinePoints[trackSplinePoints.Length - 1].position, trackSplinePoints[0].position);
        }

        return length;
    }

    public Transform GetStartPosition()
    {
        return startPosition != null ? startPosition : transform;
    }

    public void OnCheckpointTriggered(Checkpoint checkpoint)
    {
        if (!useCheckpoints) return;

        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (checkpoints[i] == checkpoint.transform)
            {
                PassCheckpoint(i);
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw track spline
        if (trackSplinePoints != null && trackSplinePoints.Length >= 2)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < trackSplinePoints.Length - 1; i++)
            {
                if (trackSplinePoints[i] != null && trackSplinePoints[i + 1] != null)
                {
                    Gizmos.DrawLine(trackSplinePoints[i].position, trackSplinePoints[i + 1].position);
                }
            }

            if (isLoop && trackSplinePoints.Length > 0 && trackSplinePoints[0] != null && trackSplinePoints[trackSplinePoints.Length - 1] != null)
            {
                Gizmos.DrawLine(trackSplinePoints[trackSplinePoints.Length - 1].position, trackSplinePoints[0].position);
            }
        }

        // Draw checkpoints
        if (checkpoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var checkpoint in checkpoints)
            {
                if (checkpoint != null)
                {
                    Gizmos.DrawWireSphere(checkpoint.position, 5f);
                }
            }
        }
    }
}

