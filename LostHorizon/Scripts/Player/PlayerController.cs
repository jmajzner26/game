using UnityEngine;
using UnityEngine.InputSystem;

namespace LostHorizon.Player
{
    /// <summary>
    /// Main player controller handling movement, interaction, and harvesting
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerStats))]
    [RequireComponent(typeof(PlayerInventory))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform playerModel;

        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundCheckDistance = 0.1f;

        [Header("Mouse Look")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float verticalLookLimit = 80f;

        [Header("Interaction")]
        [SerializeField] private float interactionRange = 3f;
        [SerializeField] private LayerMask interactionLayerMask = -1;

        [Header("Harvesting")]
        [SerializeField] private Equipment.EquipmentType currentToolType = Equipment.EquipmentType.Hand;
        private Resources.ResourceNode currentHarvestNode = null;

        // Input
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool isRunning = false;
        private bool isJumping = false;
        private bool isInteracting = false;

        // Movement
        private Vector3 velocity;
        private float verticalRotation = 0f;
        private bool isGrounded;

        // Stamina
        private float runningStaminaDrain = 20f;

        public void Initialize()
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();
            if (playerStats == null)
                playerStats = GetComponent<PlayerStats>();
            if (playerInventory == null)
                playerInventory = GetComponent<PlayerInventory>();
            if (playerCamera == null)
                playerCamera = Camera.main;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleMovement();
            HandleLook();
            HandleInteraction();
            HandleHarvesting();
        }

        private void HandleMovement()
        {
            // Ground check
            isGrounded = Physics.CheckSphere(
                transform.position + Vector3.down * (characterController.height / 2f - characterController.radius),
                characterController.radius + groundCheckDistance
            );

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Calculate movement direction
            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            float speed = walkSpeed;

            // Running logic
            if (isRunning && moveInput.magnitude > 0.1f && !playerStats.IsExhausted)
            {
                if (playerStats.UseStamina(runningStaminaDrain * Time.deltaTime))
                {
                    speed = runSpeed;
                }
                else
                {
                    isRunning = false;
                }
            }

            // Apply movement
            characterController.Move(moveDirection * speed * Time.deltaTime);

            // Jumping
            if (isJumping && isGrounded && playerStats.UseStamina(10f))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            isJumping = false;

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            // Horizontal rotation
            transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

            // Vertical rotation
            verticalRotation -= lookInput.y * mouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
            if (playerCamera != null)
            {
                playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            }
        }

        private void HandleInteraction()
        {
            if (!isInteracting) return;

            RaycastHit hit;
            Vector3 rayOrigin = playerCamera != null ? playerCamera.transform.position : transform.position + Vector3.up;
            Vector3 rayDirection = playerCamera != null ? playerCamera.transform.forward : transform.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionRange, interactionLayerMask))
            {
                // Try to interact with various interactables
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                }
            }

            isInteracting = false;
        }

        private void HandleHarvesting()
        {
            if (currentHarvestNode == null) return;

            // Check if still in range
            float distance = Vector3.Distance(transform.position, currentHarvestNode.transform.position);
            if (distance > interactionRange)
            {
                currentHarvestNode.StopHarvesting();
                currentHarvestNode = null;
                return;
            }

            // Attempt harvest
            int harvested = currentHarvestNode.Harvest(transform, currentToolType, Time.deltaTime);
            if (harvested > 0)
            {
                playerInventory.AddResource(currentHarvestNode.ResourceType, harvested);
                playerStats.UseStamina(5f * Time.deltaTime); // Harvesting costs stamina
            }
        }

        // Input callbacks (using Unity's new Input System)
        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            lookInput = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            isJumping = value.isPressed;
        }

        public void OnRun(InputValue value)
        {
            isRunning = value.isPressed;
        }

        public void OnInteract(InputValue value)
        {
            isInteracting = value.isPressed;
            
            // Also try to find resource nodes to harvest
            RaycastHit hit;
            Vector3 rayOrigin = playerCamera != null ? playerCamera.transform.position : transform.position + Vector3.up;
            Vector3 rayDirection = playerCamera != null ? playerCamera.transform.forward : transform.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionRange, interactionLayerMask))
            {
                var resourceNode = hit.collider.GetComponent<Resources.ResourceNode>();
                if (resourceNode != null && !resourceNode.IsDepleted)
                {
                    currentHarvestNode = resourceNode;
                }
            }
        }

        public void SetTool(Equipment.EquipmentType toolType)
        {
            currentToolType = toolType;
        }
    }

    /// <summary>
    /// Interface for interactable objects
    /// </summary>
    public interface IInteractable
    {
        void Interact(PlayerController player);
    }
}

