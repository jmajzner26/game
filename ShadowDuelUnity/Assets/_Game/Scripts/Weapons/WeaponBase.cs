using UnityEngine;

namespace ShadowDuel.Weapons
{
    /// <summary>
    /// Base class for all weapons in Shadow Duel
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Weapon Stats")]
        [SerializeField] protected string weaponName;
        [SerializeField] protected float baseDamage = 25f;
        [SerializeField] protected float attackSpeed = 1f;
        [SerializeField] protected float range = 2.5f;
        [SerializeField] protected float staminaCost = 0f;

        [Header("Combat")]
        [SerializeField] protected LayerMask hitLayers;
        [SerializeField] protected float hitBoxWidth = 1f;
        [SerializeField] protected float hitBoxHeight = 1f;

        [Header("Effects")]
        [SerializeField] protected GameObject trailEffect;
        [SerializeField] protected AudioClip attackSound;
        [SerializeField] protected AudioClip hitSound;
        [SerializeField] protected ParticleSystem impactEffect;

        protected Transform weaponTransform;
        protected GameObject owner;
        protected StaminaManager ownerStamina;
        protected bool isAttacking = false;
        protected float lastAttackTime = 0f;

        private AudioSource audioSource;

        public string WeaponName => weaponName;
        public float Damage => baseDamage;
        public float Range => range;
        public bool IsAttacking => isAttacking;

        protected virtual void Awake()
        {
            weaponTransform = transform;
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        /// <summary>
        /// Equip this weapon to an owner
        /// </summary>
        public virtual void Equip(GameObject newOwner)
        {
            owner = newOwner;
            if (owner)
            {
                ownerStamina = owner.GetComponent<StaminaManager>();
            }
        }

        /// <summary>
        /// Perform a light attack
        /// </summary>
        public abstract bool PerformLightAttack(Vector3 direction);

        /// <summary>
        /// Perform a heavy attack
        /// </summary>
        public abstract bool PerformHeavyAttack(Vector3 direction);

        /// <summary>
        /// Check if can attack (cooldown and stamina)
        /// </summary>
        protected bool CanAttack(bool isHeavy = false)
        {
            // Check cooldown
            float cooldownTime = 1f / attackSpeed;
            if (Time.time - lastAttackTime < cooldownTime)
                return false;

            // Check stamina
            if (ownerStamina)
            {
                if (isHeavy)
                    return ownerStamina.ConsumeHeavyAttack();
                else
                    return ownerStamina.ConsumeLightAttack();
            }

            return true;
        }

        /// <summary>
        /// Detect hits in an area
        /// </summary>
        protected void DetectHits(Vector3 attackDirection, float damageModifier = 1f)
        {
            Vector3 attackOrigin = weaponTransform.position + weaponTransform.forward * range * 0.5f;
            Vector3 halfExtents = new Vector3(hitBoxWidth * 0.5f, hitBoxHeight * 0.5f, range * 0.5f);

            Collider[] hits = Physics.OverlapBox(attackOrigin, halfExtents, weaponTransform.rotation, hitLayers);

            foreach (Collider hit in hits)
            {
                if (hit.gameObject == owner)
                    continue;

                ProcessHit(hit.gameObject, damageModifier);
            }
        }

        protected virtual void ProcessHit(GameObject hitTarget, float damageModifier)
        {
            float totalDamage = baseDamage * damageModifier;

            // Apply damage
            if (hitTarget.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(totalDamage);
            }

            // Register hit with combat manager
            if (CombatManager.Instance)
            {
                CombatManager.Instance.RegisterHit(owner, hitTarget, totalDamage, hitTarget.transform.position);
            }

            // Play hit sound
            if (audioSource && hitSound)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // Play impact effect
            if (impactEffect)
            {
                impactEffect.transform.position = hitTarget.transform.position;
                impactEffect.Play();
            }
        }

        /// <summary>
        /// Play attack sound
        /// </summary>
        protected void PlayAttackSound()
        {
            if (audioSource && attackSound)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }

        /// <summary>
        /// Get weapon stats for UI display
        /// </summary>
        public virtual WeaponStats GetWeaponStats()
        {
            return new WeaponStats
            {
                name = weaponName,
                damage = baseDamage,
                attackSpeed = attackSpeed,
                range = range
            };
        }

        public struct WeaponStats
        {
            public string name;
            public float damage;
            public float attackSpeed;
            public float range;
        }
    }
}

