using UnityEngine;

namespace ShadowDuel.Weapons
{
    /// <summary>
    /// Dual-wielded fast weapons with multi-hit combos
    /// </summary>
    public class TwinBlades : WeaponBase
    {
        [Header("Twin Blades Specific")]
        [SerializeField] private float multiHitInterval = 0.15f;
        [SerializeField] private int multiHitCount = 3;

        protected override void Awake()
        {
            base.Awake();
            
            weaponName = "Twin Blades";
            baseDamage = 15f;
            attackSpeed = 2f;
            range = 1.8f;
        }

        public override bool PerformLightAttack(Vector3 direction)
        {
            if (!CanAttack(false))
                return false;

            isAttacking = true;
            lastAttackTime = Time.time;

            PlayAttackSound();
            StartCoroutine(MultiHitCombo(direction, multiHitCount, false));

            return true;
        }

        public override bool PerformHeavyAttack(Vector3 direction)
        {
            if (!CanAttack(true))
                return false;

            isAttacking = true;
            lastAttackTime = Time.time;

            PlayAttackSound();
            StartCoroutine(PerformWhirlwind(direction));

            return true;
        }

        private System.Collections.IEnumerator MultiHitCombo(Vector3 direction, int hitCount, bool isHeavy)
        {
            for (int i = 0; i < hitCount; i++)
            {
                PerformDualSlash(direction);
                DetectHits(direction, isHeavy ? 1.5f : 1f);
                
                if (i < hitCount - 1)
                    yield return new WaitForSeconds(multiHitInterval);
            }

            EndAttack();
        }

        private System.Collections.IEnumerator PerformWhirlwind(Vector3 direction)
        {
            // Continuous spinning attack
            for (int i = 0; i < 6; i++)
            {
                PerformDualSlash(direction);
                DetectHits(direction, 0.8f);
                
                // Rotate for next slash
                direction = Quaternion.Euler(0, 60f, 0) * direction;
                yield return new WaitForSeconds(0.1f);
            }

            EndAttack();
        }

        private void PerformDualSlash(Vector3 direction)
        {
            // Alternate between left and right blade
            StartCoroutine(AlternatingSlash(0.2f));
        }

        private System.Collections.IEnumerator AlternatingSlash(float duration)
        {
            float elapsed = 0f;
            bool leftBlade = true;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                
                // Alternate slashing motion
                float angle = leftBlade ? 45f : -45f;
                weaponTransform.localRotation = Quaternion.Euler(0, angle, 0);
                
                yield return null;
            }

            weaponTransform.localRotation = Quaternion.identity;
        }

        private void EndAttack()
        {
            isAttacking = false;
        }

        public override WeaponStats GetWeaponStats()
        {
            var stats = base.GetWeaponStats();
            stats.name = "Twin Blades";
            return stats;
        }
    }
}

