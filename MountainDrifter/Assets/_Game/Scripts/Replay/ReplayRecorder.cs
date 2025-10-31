using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Records car position, rotation, and state for replay playback.
/// Captures data at fixed intervals for smooth playback.
/// </summary>
public class ReplayRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    [SerializeField] private float recordingInterval = 0.02f; // 50 FPS for smooth replay
    [SerializeField] private bool autoStartRecording = true;

    private DriftCarController carController;
    private bool isRecording = false;
    private float recordingTimer = 0f;
    private List<ReplayFrame> replayFrames = new List<ReplayFrame>();

    public bool IsRecording => isRecording;
    public int FrameCount => replayFrames.Count;

    [System.Serializable]
    public class ReplayFrame
    {
        public float timestamp;
        public Vector3 position;
        public Quaternion rotation;
        public float speed;
        public float driftAngle;
        public bool isDrifting;
    }

    private void Awake()
    {
        carController = GetComponent<DriftCarController>();
        if (carController == null)
        {
            carController = GetComponentInParent<DriftCarController>();
        }
    }

    private void Start()
    {
        if (autoStartRecording)
        {
            StartRecording();
        }
    }

    private void Update()
    {
        if (isRecording && carController != null)
        {
            recordingTimer += Time.deltaTime;
            if (recordingTimer >= recordingInterval)
            {
                RecordFrame();
                recordingTimer = 0f;
            }
        }
    }

    public void StartRecording()
    {
        isRecording = true;
        replayFrames.Clear();
        recordingTimer = 0f;
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    private void RecordFrame()
    {
        if (carController == null) return;

        ReplayFrame frame = new ReplayFrame
        {
            timestamp = Time.time,
            position = carController.transform.position,
            rotation = carController.transform.rotation,
            speed = carController.CurrentSpeed,
            driftAngle = carController.DriftAngle,
            isDrifting = carController.IsDrifting
        };

        replayFrames.Add(frame);
    }

    public ReplayData GetReplayData()
    {
        return new ReplayData
        {
            frames = replayFrames.ToArray(),
            duration = replayFrames.Count > 0 ? replayFrames[replayFrames.Count - 1].timestamp - replayFrames[0].timestamp : 0f
        };
    }

    public void ClearRecording()
    {
        replayFrames.Clear();
        recordingTimer = 0f;
    }
}

[System.Serializable]
public class ReplayData
{
    public ReplayRecorder.ReplayFrame[] frames;
    public float duration;
}

