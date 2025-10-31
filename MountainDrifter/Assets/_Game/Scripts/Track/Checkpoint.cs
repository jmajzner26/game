using UnityEngine;

/// <summary>
/// Checkpoint trigger for track progression and race timing.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private int checkpointIndex = 0;
    [SerializeField] private bool isFinishLine = false;

    [Header("Visual")]
    [SerializeField] private GameObject checkpointMarker;
    [SerializeField] private ParticleSystem passEffect;

    private MountainTrack track;
    private bool hasBeenPassed = false;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }

        // Find parent track
        track = GetComponentInParent<MountainTrack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenPassed) return;

        DriftCarController car = other.GetComponent<DriftCarController>();
        if (car != null && track != null)
        {
            track.OnCheckpointTriggered(this);
            hasBeenPassed = true;

            // Visual effect
            if (passEffect != null)
            {
                passEffect.Play();
            }

            // Disable marker
            if (checkpointMarker != null)
            {
                checkpointMarker.SetActive(false);
            }
        }
    }

    public void ResetCheckpoint()
    {
        hasBeenPassed = false;
        if (checkpointMarker != null)
        {
            checkpointMarker.SetActive(true);
        }
    }

    public int GetCheckpointIndex()
    {
        return checkpointIndex;
    }

    public bool IsFinishLine()
    {
        return isFinishLine;
    }
}

