using UnityEngine;
using Cinemachine;

namespace ShadowDuel.Core
{
    /// <summary>
    /// Handles cinematic finisher sequences with camera effects
    /// </summary>
    public class FinisherSystem : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private CinemachineCamera finisherCamera;
        [SerializeField] private float finisherDuration = 3f;
        [SerializeField] private float cameraZoomDistance = 3f;
        [SerializeField] private AnimationCurve cameraZoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Visual Effects")]
        [SerializeField] private GameObject finisherParticleEffect;
        [SerializeField] private GameObject bloodEffect;
        [SerializeField] private Light finisherLight;

        [Header("Audio")]
        [SerializeField] private AudioClip finisherMusic;

        private Camera mainCamera;
        private Transform attacker;
        private Transform target;
        private bool isFinisherActive = false;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        /// <summary>
        /// Trigger a finisher sequence
        /// </summary>
        public void TriggerFinisher(GameObject attacker, GameObject target)
        {
            if (isFinisherActive) return;

            this.attacker = attacker.transform;
            this.target = target.transform;

            StartFinisherSequence();
        }

        private void StartFinisherSequence()
        {
            isFinisherActive = true;

            // Setup camera
            SetupFinisherCamera();

            // Play effects
            PlayFinisherEffects();

            // Play audio
            PlayFinisherAudio();

            // Slow time
            if (CombatManager.Instance)
            {
                Time.timeScale = 0.15f;
            }

            // End sequence after duration
            Invoke(nameof(EndFinisherSequence), finisherDuration);
        }

        private void SetupFinisherCamera()
        {
            if (finisherCamera)
            {
                finisherCamera.gameObject.SetActive(true);

                // Position camera for dramatic angle
                Vector3 direction = (target.position - attacker.position).normalized;
                Vector3 cameraPos = attacker.position + direction * cameraZoomDistance + Vector3.up * 1.5f;

                finisherCamera.transform.position = cameraPos;
                finisherCamera.transform.LookAt(target.position);

                // Blend to finisher camera
                CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
                if (brain)
                {
                    // The brain will handle the transition
                }
            }
        }

        private void PlayFinisherEffects()
        {
            if (finisherParticleEffect && target)
            {
                GameObject effect = Instantiate(finisherParticleEffect, target.position, Quaternion.identity);
                Destroy(effect, finisherDuration);
            }

            if (bloodEffect && target)
            {
                GameObject blood = Instantiate(bloodEffect, target.position, Quaternion.identity);
                Destroy(blood, finisherDuration);
            }

            if (finisherLight)
            {
                finisherLight.enabled = true;
                StartCoroutine(PulseLight());
            }
        }

        private System.Collections.IEnumerator PulseLight()
        {
            float elapsed = 0f;
            float maxIntensity = finisherLight.intensity;

            while (elapsed < finisherDuration)
            {
                float progress = elapsed / finisherDuration;
                finisherLight.intensity = maxIntensity * Mathf.Sin(progress * Mathf.PI);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            finisherLight.intensity = maxIntensity;
        }

        private void PlayFinisherAudio()
        {
            if (finisherMusic)
            {
                AudioSource musicSource = Camera.main.GetComponent<AudioSource>();
                if (musicSource)
                {
                    musicSource.clip = finisherMusic;
                    musicSource.Play();
                }
            }
        }

        private void EndFinisherSequence()
        {
            isFinisherActive = false;

            // Restore normal time
            Time.timeScale = 1f;

            // Disable finisher camera
            if (finisherCamera)
            {
                finisherCamera.gameObject.SetActive(false);
            }

            if (finisherLight)
            {
                finisherLight.enabled = false;
            }

            // Ensure target is dead
            if (target && target.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(health.MaxHealth * 2); // Ensure death
            }
        }

        /// <summary>
        /// Check if finisher is currently active
        /// </summary>
        public bool IsFinisherActive() => isFinisherActive;
    }
}

