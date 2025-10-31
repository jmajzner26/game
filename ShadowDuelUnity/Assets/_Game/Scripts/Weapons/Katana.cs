using UnityEngine;

namespace ShadowDuel.Weapons
{
    /// <summary>
    /// Fast, precise weapon with quick slashes
    /// </summary>
    public class Katana : WeaponBase
    {
        [Header("Katana Specific")]
        [SerializeField] private float comboWindow = 1.5f;
        [SerializeField] private float comboBonusDamage = 0.15f;

        private int comboCount = 0;
        private float lastComboTime = 0f;

        protected override void Awake()
        {
            base.Awake();
            
            weaponName = "Katana";
            baseDamage = 20f;
            attackSpeed = 1.5f;
            range = 2f;
        }

        public override bool PerformLightAttack(Vector3 direction)
        {
            if (!CanAttack(false))
                return false;

            isAttacking = true;
            lastAttackTime = Time.time;

            // Update combo
            if (Time.time - lastComboTime > comboWindow)
            {
                comboCount = 0;
            }
            comboCount++;
            lastComboTime = Time.time;

            // Play animation/effect
            PlayAttackSound();
            PerformQuickSlash(direction);

            // Detect hits
            float damageBonus = 1f + (comboCount - 1) * comboBonusDamage;
            DetectHits(direction, damageBonus);

            Invoke(nameof(EndAttack), 0.5f);
            return true;
        }

        public override bool PerformHeavyAttack(Vector3 direction)
        {
            if (!CanAttack(true))
                return false;

            isAttacking = true;
            lastAttackTime = Time.time;
            comboCount = 0; // Reset combo on heavy

            PlayAttackSound();
            PerformIaiSlash(direction);

            DetectHits(direction, 2f);

            Invoke(nameof(EndAttack), 1.2f);
            return true;
        }

        private void PerformQuickSlash(Vector3 direction)
        {
            // Quick horizontal slash
            StartCoroutine(SlashMotion(0.3f, 90f));
        }

        private void PerformIaiSlash(Vector3 direction)
        {
            // Iai slash - draw and strike
            StartCoroutine(SlashMotion(1f, 120f));
        }

        private System.Collections.IEnumerator SlashMotion(float duration, float arcAngle)
        {
            float elapsed = 0f;
            Quaternion startRot = weaponTransform.localRotation;
            Quaternion endRot = startRot * Quaternion.Euler(0, arcAngle, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                weaponTransform.localRotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }

            weaponTransform.localRotation = startRot;
        }

        private void EndAttack()
        {
            isAttacking = false;
        }

        public override WeaponStats GetWeaponStats()
        {
            var stats = base.GetWeaponStats();
            stats.name = "Katana";
            return stats;
        }
    }
}

