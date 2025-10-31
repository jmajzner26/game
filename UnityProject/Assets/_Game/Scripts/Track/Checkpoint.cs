using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointIndex;
    [SerializeField] private bool isFinishLine = false;
    
    public int Index => checkpointIndex;
    public bool IsFinishLine => isFinishLine;
    
    private void OnTriggerEnter(Collider other)
    {
        VehicleController vehicle = other.GetComponent<VehicleController>();
        if (vehicle != null)
        {
            LapCounter lapCounter = vehicle.GetComponent<LapCounter>();
            if (lapCounter != null)
            {
                lapCounter.OnCheckpointPassed(checkpointIndex, isFinishLine);
            }
        }
    }
    
    public void SetIndex(int index)
    {
        checkpointIndex = index;
    }
    
    public void SetAsFinishLine(bool finish)
    {
        isFinishLine = finish;
    }
}

