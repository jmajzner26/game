using UnityEngine;
using Unity.Collections;
using ShadowDuel.Weapons;

namespace ShadowDuel.Player
{
    /// <summary>
    /// Main player controller with movement and combat
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(StaminaManager))]
    [RequireComponent(typeof(ParrySystem))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Combat")]
        [SerializeField] private Transform weaponPoint;
        [SerializeField] private WeaponBase currentWeapon;

        [Header("Dodge")]
        [SerializeField] private float dodgeDistance = 5f;
        [SerializeField] private float dodgeDuration = 0.5f;
        [SerializeField] private AnimationCurve dodgeSpeedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private CharacterController controller;
        private StaminaManager staminaManager;
        private ParrySystem parrySystem;
        private DirectionalCombat directionalCombat;
        private Camera playerCamera;

        private Vector3 velocity;
        private bool isDodging = false;
        private Vector3 dodgeDirection;
        private float dodgeTime = 0f;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            staminaManager = GetComponent<StaminaManager>();
            parrySystem = GetComponent<ParrySystem>();
            directionalCombat = GetComponent<DirectionalCombat>();
            playerCamera = Camera.main;

            // Equip starting weapon if assigned
            if (currentWeapon)
            {
                currentWeapon.Equip(gameObject);
            }
        }

        private void Update()
        {
            HandleMovement();
            HandleCombat();
            HandleDodge();

            ApplyGravity();
            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleMovement()
        {
            if (isDodging) return;

            // Get input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            // Calculate direction relative to camera
            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            move.Normalize();

            // Apply speed
            float currentSpeed = isRunning && staminaManager.CanRun() ? runSpeed : moveSpeed;
            move *= currentSpeed;

            velocity.x = move.x;
            velocity.z = move.z;

            // Face movement direction
            if (move.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(move),
                    Time.deltaTime * 10f
                );
            }
        }

        private void HandleCombat()
        {
            if (isDodging || !currentWeapon) return;

            // Attack
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 attackDirection = GetMouseDirection();
                currentWeapon.PerformLightAttack(attackDirection);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 attackDirection = GetMouseDirection();
                currentWeapon.PerformHeavyAttack(attackDirection);
            }

            // Parry
            if (Input.GetKeyDown(KeyCode.Space))
            {
                parrySystem.TryParry();
            }

            // Feint (requires directional combat)
            if (Input.GetKeyDown(KeyCode.Q) && directionalCombat)
            {
                directionalCombat.TryFeint();
            }

            // Special ability
            if (Input.GetKeyDown(KeyCode.E) && staminaManager.CanUseSpecialAbility())
            {
                UseSpecialAbility();
            }
        }

        private void HandleDodge()
        {
            if (isDodging)
            {
                dodgeTime += Time.deltaTime;
                float progress = dodgeTime / dodgeDuration;
                
                float speed = dodgeSpeedCurve.Evaluate(progress) * dodgeDistance / dodgeDuration;
                velocity.x = dodgeDirection.x * speed;
                velocity.z = dodgeDirection.z * speed;

                if (dodgeTime >= dodgeDuration)
                {
                    isDodging = false;
                    dodgeTime = 0f;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && 
                     !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                TryDodge();
            }
        }

        private bool TryDodge()
        {
            if (!staminaManager.CanDodge() || isDodging)
                return false;

            if (staminaManager.ConsumeDodge())
            {
                // Dodge in camera direction or backward
                dodgeDirection = GetMouseDirection();
                isDodging = true;
                dodgeTime = 0f;
                return true;
            }

            return false;
        }

        private Vector3 GetMouseDirection()
        {
            if (!playerCamera) return transform.forward;

            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, transform.position);
            
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                return (point - transform.position).normalized;
            }

            return transform.forward;
        }

        private void UseSpecialAbility()
        {
            // Example: Shadow Dash ability
            staminaManager.ConsumeSpecialAbility();
            
            Vector3 dashDirection = transform.forward;
            StartCoroutine(ShadowDash(dashDirection));
        }

        private System.Collections.IEnumerator ShadowDash(Vector3 direction)
        {
            float dashDistance = 10f;
            float dashTime = 0.3f;
            float elapsed = 0f;

            while (elapsed < dashTime)
            {
                elapsed += Time.deltaTime;
                float speed = (dashDistance / dashTime) * (1f - elapsed / dashTime);
                controller.Move(direction * speed * Time.deltaTime);
                yield return null;
            }
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            velocity.y += gravity * Time.deltaTime;
        }

        /// <summary>
        /// Switch to a different weapon
        /// </summary>
        public void EquipWeapon(WeaponBase newWeapon)
        {
            if (currentWeapon)
            {
                Destroy(currentWeapon.gameObject);
            }

            currentWeapon = Instantiate(newWeapon, weaponPoint);
            currentWeapon.Equip(gameObject);
        }

        /// <summary>
        /// Get current weapon
        /// </summary>
        public WeaponBase GetCurrentWeapon() => currentWeapon;

        private void OnDrawGizmos()
        {
            if (currentWeapon)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, currentWeapon.Range);
            }
        }
    }
}

