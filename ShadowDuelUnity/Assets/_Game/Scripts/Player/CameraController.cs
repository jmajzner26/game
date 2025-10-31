using UnityEngine;
using Cinemachine;

namespace ShadowDuel.Player
{
    /// <summary>
    /// Manages camera behavior for combat and cinematics
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private CinemachineVirtualCamera combatCamera;
        [SerializeField] private CinemachineVirtualCamera finisherCamera;

        [Header("Camera Shake")]
        [SerializeField] private float shakeDuration = 0.3f;
        [SerializeField] private float shakeAmplitude = 1f;
        [SerializeField] private float shakeFrequency = 2f;

        [Header("Combat Feedback")]
        [SerializeField] private float hitFeedbackDuration = 0.1f;

        private CinemachineVirtualCamera currentCamera;
        private CinemachineCameraOffset offset;

        private void Awake()
        {
            currentCamera = combatCamera;
            
            if (combatCamera)
            {
                offset = combatCamera.GetCinemachineComponent<CinemachineCameraOffset>();
            }
        }

        private void Start()
        {
            // Activate combat camera by default
            if (combatCamera)
            {
                combatCamera.Priority = 10;
            }

            if (finisherCamera)
            {
                finisherCamera.Priority = 0;
            }
        }

        /// <summary>
        /// Trigger camera shake on hit/impact
        /// </summary>
        public void ShakeCamera(float intensity = 1f)
        {
            if (offset)
            {
                StartCoroutine(ShakeSequence(intensity));
            }
        }

        private System.Collections.IEnumerator ShakeSequence(float intensity)
        {
            float elapsed = 0f;
            float actualShake = shakeAmplitude * intensity;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;

                float x = Random.Range(-actualShake, actualShake) * intensity;
                float y = Random.Range(-actualShake, actualShake) * intensity;

                offset.m_Offset = new Vector3(x, y, 0f);

                yield return null;
            }

            offset.m_Offset = Vector3.zero;
        }

        /// <summary>
        /// Switch to finisher camera
        /// </summary>
        public void SwitchToFinisherCamera()
        {
            if (finisherCamera)
            {
                finisherCamera.Priority = 20;
                currentCamera = finisherCamera;
            }
        }

        /// <summary>
        /// Switch back to combat camera
        /// </summary>
        public void SwitchToCombatCamera()
        {
            if (combatCamera)
            {
                combatCamera.Priority = 20;
                if (finisherCamera)
                {
                    finisherCamera.Priority = 0;
                }
                currentCamera = combatCamera;
            }
        }

        /// <summary>
        /// Play hit feedback effect
        /// </summary>
        public void PlayHitFeedback()
        {
            if (currentCamera)
            {
                StartCoroutine(HitFeedbackSequence());
            }
        }

        private System.Collections.IEnumerator HitFeedbackSequence()
        {
            // Slight camera zoom/zoom out
            CinemachineComponentBase component = currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            
            if (component is Cinemachine3rdPersonFollow follow)
            {
                float originalDistance = follow.CameraDistance;
                float targetDistance = originalDistance * 0.9f;

                float elapsed = 0f;
                while (elapsed < hitFeedbackDuration * 0.5f)
                {
                    elapsed += Time.deltaTime;
                    follow.CameraDistance = Mathf.Lerp(originalDistance, targetDistance, elapsed / (hitFeedbackDuration * 0.5f));
                    yield return null;
                }

                elapsed = 0f;
                while (elapsed < hitFeedbackDuration * 0.5f)
                {
                    elapsed += Time.deltaTime;
                    follow.CameraDistance = Mathf.Lerp(targetDistance, originalDistance, elapsed / (hitFeedbackDuration * 0.5f));
                    yield return null;
                }

                follow.CameraDistance = originalDistance;
            }
        }
    }
}

