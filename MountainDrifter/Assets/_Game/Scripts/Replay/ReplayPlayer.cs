using UnityEngine;

/// <summary>
/// Plays back recorded replay data for highlight videos and review.
/// </summary>
public class ReplayPlayer : MonoBehaviour
{
    [Header("Replay Settings")]
    [SerializeField] private float playbackSpeed = 1f;
    [SerializeField] private bool loopReplay = false;
    [SerializeField] private bool autoPlay = false;

    [Header("Visual Effects")]
    [SerializeField] private GameObject replayCarPrefab;
    [SerializeField] private Material replayMaterial;

    private ReplayData replayData;
    private GameObject replayCarInstance;
    private int currentFrameIndex = 0;
    private float playbackTimer = 0f;
    private bool isPlaying = false;

    public bool IsPlaying => isPlaying;
    public float PlaybackProgress => replayData != null && replayData.frames != null && replayData.frames.Length > 0
        ? (float)currentFrameIndex / replayData.frames.Length
        : 0f;

    public void LoadReplay(ReplayData data)
    {
        replayData = data;
        currentFrameIndex = 0;
        playbackTimer = 0f;

        if (replayCarPrefab != null && replayData != null && replayData.frames != null && replayData.frames.Length > 0)
        {
            SpawnReplayCar();
        }

        if (autoPlay)
        {
            Play();
        }
    }

    private void SpawnReplayCar()
    {
        if (replayCarInstance != null)
        {
            Destroy(replayCarInstance);
        }

        if (replayCarPrefab != null && replayData.frames != null && replayData.frames.Length > 0)
        {
            ReplayRecorder.ReplayFrame firstFrame = replayData.frames[0];
            replayCarInstance = Instantiate(replayCarPrefab, firstFrame.position, firstFrame.rotation);

            // Disable physics and input
            Rigidbody rb = replayCarInstance.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            DriftCarController controller = replayCarInstance.GetComponent<DriftCarController>();
            if (controller != null) controller.enabled = false;

            // Apply replay material if specified
            if (replayMaterial != null)
            {
                Renderer[] renderers = replayCarInstance.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.material = replayMaterial;
                }
            }
        }
    }

    private void Update()
    {
        if (isPlaying && replayData != null && replayData.frames != null && currentFrameIndex < replayData.frames.Length)
        {
            playbackTimer += Time.deltaTime * playbackSpeed;

            // Find current frame based on timestamp
            float currentTime = replayData.frames[0].timestamp + playbackTimer;

            // Advance to next frame
            while (currentFrameIndex < replayData.frames.Length - 1 &&
                   currentTime >= replayData.frames[currentFrameIndex + 1].timestamp)
            {
                currentFrameIndex++;
            }

            // Interpolate between frames for smooth playback
            if (currentFrameIndex < replayData.frames.Length - 1)
            {
                ReplayRecorder.ReplayFrame frameA = replayData.frames[currentFrameIndex];
                ReplayRecorder.ReplayFrame frameB = replayData.frames[currentFrameIndex + 1];

                float t = (currentTime - frameA.timestamp) / (frameB.timestamp - frameA.timestamp);
                t = Mathf.Clamp01(t);

                if (replayCarInstance != null)
                {
                    replayCarInstance.transform.position = Vector3.Lerp(frameA.position, frameB.position, t);
                    replayCarInstance.transform.rotation = Quaternion.Lerp(frameA.rotation, frameB.rotation, t);
                }
            }
            else
            {
                // End of replay
                if (loopReplay)
                {
                    Restart();
                }
                else
                {
                    Stop();
                }
            }
        }
    }

    public void Play()
    {
        if (replayData != null && replayData.frames != null && replayData.frames.Length > 0)
        {
            isPlaying = true;
        }
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Stop()
    {
        isPlaying = false;
        currentFrameIndex = 0;
        playbackTimer = 0f;
    }

    public void Restart()
    {
        Stop();
        Play();
    }

    public void SetPlaybackSpeed(float speed)
    {
        playbackSpeed = Mathf.Clamp(speed, 0.1f, 5f);
    }
}

