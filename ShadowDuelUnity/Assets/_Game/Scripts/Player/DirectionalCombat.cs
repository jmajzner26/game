using UnityEngine;

namespace ShadowDuel.Player
{
    /// <summary>
    /// Handles directional input for directional combat and feinting
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class DirectionalCombat : MonoBehaviour
    {
        [Header("Combat Directions")]
        [SerializeField] private float directionThreshold = 0.3f;

        [Header("Feint System")]
        [SerializeField] private float feintWindow = 0.2f;
        [SerializeField] private float feintStaminaCost = 5f;
        [SerializeField] private float feintBonus = 0.3f; // Bonus damage on successful feint

        private StaminaManager staminaManager;
        private Vector2 lastInputDirection;
        private AttackDirection currentDirection = AttackDirection.Neutral;
        private bool isFeinting = false;

        public enum AttackDirection
        {
            Neutral,
            Up,
            Down,
            Left,
            Right,
            UpLeft,
            UpRight,
            DownLeft,
            DownRight
        }

        private void Awake()
        {
            staminaManager = GetComponent<StaminaManager>();
        }

        private void Update()
        {
            ReadDirectionalInput();
        }

        /// <summary>
        /// Read mouse/controller input for combat directions
        /// </summary>
        private void ReadDirectionalInput()
        {
            // Get mouse delta for directional input
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector2 inputDirection = new Vector2(mouseX, mouseY);

            // Determine cardinal direction
            if (inputDirection.magnitude > directionThreshold)
            {
                currentDirection = DetermineDirection(inputDirection);
                lastInputDirection = inputDirection;
            }
            else
            {
                currentDirection = AttackDirection.Neutral;
            }
        }

        private AttackDirection DetermineDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Normalize angle to 0-360
            if (angle < 0) angle += 360;

            // Map to 8 directions
            if (angle >= 337.5f || angle < 22.5f)
                return AttackDirection.Right;
            else if (angle >= 22.5f && angle < 67.5f)
                return AttackDirection.UpRight;
            else if (angle >= 67.5f && angle < 112.5f)
                return AttackDirection.Up;
            else if (angle >= 112.5f && angle < 157.5f)
                return AttackDirection.UpLeft;
            else if (angle >= 157.5f && angle < 202.5f)
                return AttackDirection.Left;
            else if (angle >= 202.5f && angle < 247.5f)
                return AttackDirection.DownLeft;
            else if (angle >= 247.5f && angle < 292.5f)
                return AttackDirection.Down;
            else // angle >= 292.5f && angle < 337.5f
                return AttackDirection.DownRight;
        }

        /// <summary>
        /// Try to perform a feint (fake attack to trick opponent)
        /// </summary>
        public bool TryFeint()
        {
            if (isFeinting || !staminaManager)
                return false;

            if (staminaManager.CurrentStamina >= feintStaminaCost)
            {
                staminaManager.ConsumeStamina(feintStaminaCost);
                StartCoroutine(FeintSequence());
                return true;
            }

            return false;
        }

        private System.Collections.IEnumerator FeintSequence()
        {
            isFeinting = true;

            // Brief animation/effect
            yield return new WaitForSeconds(feintWindow);

            isFeinting = false;
        }

        /// <summary>
        /// Check if currently feinting
        /// </summary>
        public bool IsFeinting() => isFeinting;

        /// <summary>
        /// Get current attack direction
        /// </summary>
        public AttackDirection GetCurrentDirection() => currentDirection;

        /// <summary>
        /// Get direction vector
        /// </summary>
        public Vector3 GetDirectionVector()
        {
            return currentDirection switch
            {
                AttackDirection.Up => Vector3.forward,
                AttackDirection.Down => Vector3.back,
                AttackDirection.Left => Vector3.left,
                AttackDirection.Right => Vector3.right,
                AttackDirection.UpLeft => (Vector3.forward + Vector3.left).normalized,
                AttackDirection.UpRight => (Vector3.forward + Vector3.right).normalized,
                AttackDirection.DownLeft => (Vector3.back + Vector3.left).normalized,
                AttackDirection.DownRight => (Vector3.back + Vector3.right).normalized,
                _ => Vector3.zero
            };
        }

        /// <summary>
        /// Apply feint bonus if successful (call from combat system)
        /// </summary>
        public float GetFeintBonus()
        {
            return isFeinting ? feintBonus : 0f;
        }

        private void OnDrawGizmos()
        {
            Vector3 dir = GetDirectionVector();
            if (dir != Vector3.zero)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position + Vector3.up * 1.5f, dir * 2f);
            }
        }
    }
}

