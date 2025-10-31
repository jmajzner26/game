using UnityEngine;
using UnityEngine.AI;

namespace ShadowDuel.Enemy
{
    /// <summary>
    /// Base AI controller for enemies with basic combat behavior
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(StaminaManager))]
    [RequireComponent(typeof(ParrySystem))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float detectionRange = 15f;
        [SerializeField] private float attackRange = 2.5f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float decisionInterval = 0.5f;

        [Header("Combat")]
        [SerializeField] private float staggerDuration = 0.5f;
        [SerializeField] private Transform weaponPoint;
        [SerializeField] private WeaponBase weapon;

        [Header("States")]
        [SerializeField] private AIState currentState = AIState.Patrol;
        [SerializeField] private bool isStunned = false;

        protected NavMeshAgent agent;
        protected Health health;
        protected StaminaManager staminaManager;
        protected ParrySystem parrySystem;
        protected GameObject target;
        protected Vector3 lastKnownPosition;

        protected float lastAttackTime = 0f;
        protected float lastDecisionTime = 0f;
        protected float stunTime = 0f;

        public enum AIState
        {
            Patrol,
            Chase,
            Attack,
            Block,
            Dodge,
            Stunned,
            Dead
        }

        public AIState CurrentState => currentState;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            staminaManager = GetComponent<StaminaManager>();
            parrySystem = GetComponent<ParrySystem>();

            if (weapon)
            {
                weapon.Equip(gameObject);
            }
        }

        protected virtual void Start()
        {
            // Find player as default target
            target = GameObject.FindGameObjectWithTag("Player");
            
            if (agent)
            {
                agent.speed = Random.Range(3f, 5f);
            }
        }

        protected virtual void Update()
        {
            if (isStunned)
            {
                HandleStun();
                return;
            }

            if (health.IsDead)
            {
                currentState = AIState.Dead;
                OnDeath();
                return;
            }

            UpdateAI();
            MakeDecision();
        }

        protected virtual void UpdateAI()
        {
            // Update target knowledge
            if (target && IsTargetInRange(detectionRange))
            {
                lastKnownPosition = target.transform.position;
                
                if (currentState == AIState.Patrol)
                {
                    currentState = AIState.Chase;
                }
            }
        }

        protected virtual void MakeDecision()
        {
            if (Time.time - lastDecisionTime < decisionInterval)
                return;

            lastDecisionTime = Time.time;

            // State machine
            switch (currentState)
            {
                case AIState.Patrol:
                    HandlePatrol();
                    break;
                case AIState.Chase:
                    HandleChase();
                    break;
                case AIState.Attack:
                    HandleAttack();
                    break;
                case AIState.Block:
                    HandleBlock();
                    break;
                case AIState.Dodge:
                    HandleDodge();
                    break;
            }
        }

        protected virtual void HandlePatrol()
        {
            if (!agent) return;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                // Move to random nearby point
                Vector3 randomDirection = Random.insideUnitSphere * 10f;
                randomDirection += transform.position;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
        }

        protected virtual void HandleChase()
        {
            if (!agent || !target) return;

            agent.SetDestination(target.transform.position);

            // Check if in attack range
            if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
            {
                currentState = AIState.Attack;
                agent.ResetPath();
            }

            // Check if lost target
            if (!IsTargetInRange(detectionRange * 2f))
            {
                currentState = AIState.Patrol;
            }
        }

        protected virtual void HandleAttack()
        {
            if (!target || !weapon) return;

            // Face target
            Vector3 direction = (target.transform.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

            // Attack if cooldown is over
            if (Time.time - lastAttackTime > attackCooldown)
            {
                PerformAttack(direction);
                lastAttackTime = Time.time;
            }

            // Check if out of range
            if (Vector3.Distance(transform.position, target.transform.position) > attackRange * 1.5f)
            {
                currentState = AIState.Chase;
            }
        }

        protected virtual void HandleBlock()
        {
            parrySystem.TryParry();
            
            // After short block, transition to attack or dodge
            if (Time.time - lastDecisionTime > 1f)
            {
                currentState = AIState.Attack;
            }
        }

        protected virtual void HandleDodge()
        {
            if (staminaManager.CanDodge())
            {
                Vector3 dodgeDirection = transform.right * (Random.value > 0.5f ? 1f : -1f);
                transform.position += dodgeDirection * 3f;
                staminaManager.ConsumeDodge();
            }

            currentState = AIState.Attack;
        }

        protected virtual void PerformAttack(Vector3 direction)
        {
            if (Random.value > 0.5f)
            {
                weapon.PerformLightAttack(direction);
            }
            else
            {
                weapon.PerformHeavyAttack(direction);
            }
        }

        protected virtual void HandleStun()
        {
            stunTime -= Time.deltaTime;

            if (stunTime <= 0f)
            {
                isStunned = false;
                currentState = AIState.Chase;
            }
        }

        public virtual void Stun(float duration)
        {
            isStunned = true;
            stunTime = duration;
            currentState = AIState.Stunned;

            if (agent)
            {
                agent.ResetPath();
            }
        }

        protected virtual void OnDeath()
        {
            if (agent)
            {
                agent.enabled = false;
            }

            // Disable components
            enabled = false;
        }

        protected bool IsTargetInRange(float range)
        {
            if (!target) return false;
            return Vector3.Distance(transform.position, target.transform.position) <= range;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw detection and attack ranges
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

