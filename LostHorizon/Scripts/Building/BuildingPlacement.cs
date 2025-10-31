using UnityEngine;

namespace LostHorizon.Building
{
    /// <summary>
    /// Handles placement preview and validation for buildings
    /// </summary>
    public class BuildingPlacement : MonoBehaviour
    {
        [Header("Placement Settings")]
        [SerializeField] private LayerMask groundLayerMask = 1; // Default layer
        [SerializeField] private float maxPlacementDistance = 5f;
        [SerializeField] private Color validPlacementColor = Color.green;
        [SerializeField] private Color invalidPlacementColor = Color.red;

        [Header("Preview")]
        [SerializeField] private Material previewMaterial;
        private GameObject previewObject;
        private BuildableObject currentBuildable;
        private bool isValidPlacement = false;
        private Vector3 targetPosition;
        private Quaternion targetRotation;

        private Player.PlayerController player;
        private Camera playerCamera;

        public bool IsPlacing => currentBuildable != null;

        private void Start()
        {
            player = FindObjectOfType<Player.PlayerController>();
            if (player != null)
            {
                playerCamera = player.GetComponentInChildren<Camera>();
            }

            if (previewMaterial == null)
            {
                // Create a default transparent material
                previewMaterial = new Material(Shader.Find("Standard"));
                previewMaterial.SetFloat("_Mode", 3); // Transparent mode
                previewMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                previewMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                previewMaterial.SetInt("_ZWrite", 0);
                previewMaterial.DisableKeyword("_ALPHATEST_ON");
                previewMaterial.EnableKeyword("_ALPHABLEND_ON");
                previewMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                previewMaterial.renderQueue = 3000;
            }
        }

        public void StartPlacement(BuildableObject buildable)
        {
            if (buildable == null || buildable.prefab == null)
            {
                Debug.LogWarning("Cannot start placement: Invalid buildable object");
                return;
            }

            currentBuildable = buildable;

            // Create preview object
            if (previewObject != null)
            {
                Destroy(previewObject);
            }

            previewObject = Instantiate(buildable.prefab);
            previewObject.name = buildable.objectName + "_Preview";

            // Make preview semi-transparent
            Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = new Material[renderer.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = previewMaterial;
                }
                renderer.materials = materials;
            }

            // Disable colliders on preview
            Collider[] colliders = previewObject.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }

        public void CancelPlacement()
        {
            if (previewObject != null)
            {
                Destroy(previewObject);
                previewObject = null;
            }
            currentBuildable = null;
        }

        private void Update()
        {
            if (!IsPlacing || previewObject == null) return;

            UpdatePlacementPreview();
        }

        private void UpdatePlacementPreview()
        {
            if (playerCamera == null) return;

            // Raycast from camera to find placement position
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxPlacementDistance, groundLayerMask))
            {
                targetPosition = hit.point;
                targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;

                // Validate placement
                isValidPlacement = ValidatePlacement(hit);

                // Update preview position and color
                previewObject.transform.position = targetPosition;
                previewObject.transform.rotation = targetRotation;

                UpdatePreviewColor();
            }
            else
            {
                isValidPlacement = false;
                UpdatePreviewColor();
            }
        }

        private bool ValidatePlacement(RaycastHit hit)
        {
            // Check distance from player
            if (player != null)
            {
                float distance = Vector3.Distance(player.transform.position, hit.point);
                if (distance > maxPlacementDistance)
                {
                    return false;
                }
            }

            // Check slope angle
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (!currentBuildable.canPlaceOnSlopes && slopeAngle > 5f)
            {
                return false;
            }
            if (slopeAngle > currentBuildable.maxSlopeAngle)
            {
                return false;
            }

            // Check if on water (simplified - would need actual water detection)
            // This would require checking if hit point is below water level

            return true;
        }

        private void UpdatePreviewColor()
        {
            if (previewObject == null) return;

            Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
            Color targetColor = isValidPlacement ? validPlacementColor : invalidPlacementColor;
            targetColor.a = 0.5f;

            foreach (Renderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        mat.color = targetColor;
                    }
                }
            }
        }

        public bool TryPlace()
        {
            if (!IsPlacing || !isValidPlacement || currentBuildable == null)
            {
                return false;
            }

            // Check if player has required resources
            var buildingSystem = FindObjectOfType<BuildingSystem>();
            if (buildingSystem != null)
            {
                bool placed = buildingSystem.PlaceBuilding(currentBuildable, targetPosition, targetRotation);
                if (placed)
                {
                    CancelPlacement();
                    return true;
                }
            }

            return false;
        }
    }
}

