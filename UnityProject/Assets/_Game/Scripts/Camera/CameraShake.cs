using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    
    private Vector3 originalPosition;
    private float shakeTimer = 0f;
    private bool isShaking = false;
    
    private void Start()
    {
        originalPosition = transform.localPosition;
    }
    
    private void Update()
    {
        if (isShaking)
        {
            shakeTimer += Time.deltaTime;
            
            if (shakeTimer >= shakeDuration)
            {
                isShaking = false;
                shakeTimer = 0f;
                transform.localPosition = originalPosition;
            }
            else
            {
                float curveValue = shakeCurve.Evaluate(shakeTimer / shakeDuration);
                Vector3 offset = Random.insideUnitSphere * shakeMagnitude * curveValue;
                transform.localPosition = originalPosition + offset;
            }
        }
    }
    
    public void Shake(float duration = -1f, float magnitude = -1f)
    {
        if (duration > 0f) shakeDuration = duration;
        if (magnitude > 0f) shakeMagnitude = magnitude;
        
        isShaking = true;
        shakeTimer = 0f;
    }
}

