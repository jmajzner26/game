using UnityEngine;

namespace ShadowDuel.Weapons
{
    /// <summary>
    /// Slow, powerful weapon with wide arcs and knockback
    /// </summary>
    public class Claymore : WeaponBase
    {
        [Header("Claymore Specific")]
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private float chargeDuration = 1.5f;
        [SerializeField] private float maxChargeDamage = 3f;

        private bool isCharging = false;
        private float chargeTime = 0f;

        protected override void Awake()
        {
            base.Awake();
            
            weaponName = "Claymore";
            baseDamage = 40f;
            attackSpeed = 0.8f;
            range = 3f;
        }

        public override bool PerformLightAttack(Vector3 direction)
        {
            if (!CanAttack(false))
                return false;

            isAttacking = true;
            lastAttackTime = Time.time;

            PlayAttackSound();
            StartCoroutine(PerformHeavySlash(direction, 1f));

            return true;
        }

        public override bool PerformHeavyAttack(Vector3 direction)
        {
            if (!CanAttack(true))
                return false;

            isCharging = true;
            StartCoroutine(ChargeAndStrike(direction));

            return true;
        }

        private System.Collections.IEnumerator ChargeAndStrike(Vector3 direction)
        {
            // Charge phase
            chargeTime = 0f;
            while (chargeTime < chargeDuration && isCharging)
            {
                chargeTime += Time.deltaTime;
                // Visual/audio feedback for charging
                yield return null;
            }

            isCharging = false;
            isAttacking = true;
            lastAttackTime = Time.time;

            PlayAttackSound();
            
            // Damage scales with charge
            float chargeMultiplier = 1f + (chargeTime / chargeDuration) * maxChargeDamage;
            StartCoroutine(PerformPowerSlash(direction, chargeMultiplier));
        }

        private System.Collections.IEnumerator PerformPowerSlash(Vector3 direction, float damageMultiplier)
        {
            float duration = 0.8f;
            float elapsed = 0f;

            Quaternion startRot = weaponTransform.localRotation;
            Quaternion endRot = startRot * Quaternion.Euler(0, 180f, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                weaponTransform.localRotation = Quaternion.Slerp(startRot, endRot, t);
                
                // Detect hits throughout the swing
                if (t > 0.3f && t < 0.7f)
                {
                    DetectHits(direction, damageMultiplier);
                }
                
                yield return null;
            }

            weaponTransform.localRotation = startRot;
            EndAttack();
        }

        private System.Collections.IEnumerator PerformHeavySlash(Vector3 direction, float damageModifier)
        {
            float duration = 0.6f;
            float elapsed = 0f;

            Quaternion startRot = weaponTransform.localRotation;
            Quaternion endRot = startRot * Quaternion.Euler(0, 120f, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                weaponTransform.localRotation = Quaternion.Slerp(startRot, endRot, t);
                
                if (t > 0.4f && t < 0.8f)
                {
                    DetectHits(direction, damageModifier);
                }
                
                yield return null;
            }

            weaponTransform.localRotation = startRot;
            EndAttack();
        }

        protected override void ProcessHit(GameObject hitTarget, float damageModifier)
        {
            base.ProcessHit(hitTarget, damageModifier);

            // Apply knockback
            if (hitTarget.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 knockbackDirection = (hitTarget.transform.position - weaponTransform.position).normalized;
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }

        private void EndAttack()
        {
            isAttacking = false;
        }

        public override WeaponStats GetWeaponStats()
        {
            var stats = base.GetWeaponStats();
            stats.name = "Claymore";
            return stats;
        }
    }
}

