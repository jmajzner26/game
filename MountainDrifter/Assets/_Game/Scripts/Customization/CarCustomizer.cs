using UnityEngine;

/// <summary>
/// Handles car customization: paint, decals, spoilers, wheels, etc.
/// Provides preview and application of visual modifications.
/// </summary>
public class CarCustomizer : MonoBehaviour
{
    [Header("Car Parts")]
    [SerializeField] private Renderer[] bodyRenderers;
    [SerializeField] private Renderer[] wheelRenderers;
    [SerializeField] private GameObject[] spoilerOptions;
    [SerializeField] private GameObject[] wheelMeshOptions;

    [Header("Customization Data")]
    [SerializeField] private CustomizationData currentCustomization;

    private Material[] bodyMaterials;
    private Material[] wheelMaterials;

    [System.Serializable]
    public class CustomizationData
    {
        public Color primaryColor = Color.red;
        public Color secondaryColor = Color.white;
        public int spoilerIndex = -1;
        public int wheelIndex = 0;
        public bool hasDecals = false;
        public Texture2D decalTexture;
    }

    private void Awake()
    {
        // Store original materials
        if (bodyRenderers != null && bodyRenderers.Length > 0)
        {
            bodyMaterials = new Material[bodyRenderers.Length];
            for (int i = 0; i < bodyRenderers.Length; i++)
            {
                if (bodyRenderers[i] != null)
                {
                    bodyMaterials[i] = bodyRenderers[i].material;
                }
            }
        }

        if (wheelRenderers != null && wheelRenderers.Length > 0)
        {
            wheelMaterials = new Material[wheelRenderers.Length];
            for (int i = 0; i < wheelRenderers.Length; i++)
            {
                if (wheelRenderers[i] != null)
                {
                    wheelMaterials[i] = wheelRenderers[i].material;
                }
            }
        }

        if (currentCustomization == null)
        {
            currentCustomization = new CustomizationData();
        }
    }

    public void ApplyPrimaryColor(Color color)
    {
        currentCustomization.primaryColor = color;
        ApplyColorToRenderers(bodyRenderers, color);
    }

    public void ApplySecondaryColor(Color color)
    {
        currentCustomization.secondaryColor = color;
        // Apply to secondary parts if available
    }

    private void ApplyColorToRenderers(Renderer[] renderers, Color color)
    {
        if (renderers == null) return;

        foreach (var renderer in renderers)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = color;
            }
        }
    }

    public void SetSpoiler(int index)
    {
        // Disable all spoilers
        if (spoilerOptions != null)
        {
            foreach (var spoiler in spoilerOptions)
            {
                if (spoiler != null)
                    spoiler.SetActive(false);
            }

            // Enable selected spoiler
            if (index >= 0 && index < spoilerOptions.Length && spoilerOptions[index] != null)
            {
                spoilerOptions[index].SetActive(true);
                currentCustomization.spoilerIndex = index;
            }
            else
            {
                currentCustomization.spoilerIndex = -1;
            }
        }
    }

    public void SetWheelMesh(int index)
    {
        currentCustomization.wheelIndex = index;
        // Wheel mesh swapping would be handled by the wheel system
    }

    public void ApplyDecal(Texture2D decal)
    {
        currentCustomization.decalTexture = decal;
        currentCustomization.hasDecals = decal != null;

        if (decal != null && bodyRenderers != null)
        {
            foreach (var renderer in bodyRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    // Apply decal texture to material
                    renderer.material.SetTexture("_DecalTex", decal);
                }
            }
        }
    }

    public CustomizationData GetCurrentCustomization()
    {
        return currentCustomization;
    }

    public void LoadCustomization(CustomizationData data)
    {
        if (data == null) return;

        currentCustomization = data;
        ApplyPrimaryColor(data.primaryColor);
        ApplySecondaryColor(data.secondaryColor);
        SetSpoiler(data.spoilerIndex);
        SetWheelMesh(data.wheelIndex);
        
        if (data.hasDecals && data.decalTexture != null)
        {
            ApplyDecal(data.decalTexture);
        }
    }

    public void ResetToDefault()
    {
        currentCustomization = new CustomizationData();
        ApplyPrimaryColor(Color.red);
        ApplySecondaryColor(Color.white);
        SetSpoiler(-1);
        SetWheelMesh(0);
        ApplyDecal(null);
    }
}

