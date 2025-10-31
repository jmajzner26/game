using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float currentEnergy = 100f;
    [SerializeField] private float energyRegenRate = 10f; // Per second
    [SerializeField] private float energyRegenDelay = 2f; // Delay before regen starts
    
    private float timeSinceLastUse = 0f;
    
    public float MaxEnergy => maxEnergy;
    public float CurrentEnergy => currentEnergy;
    public float EnergyPercentage => maxEnergy > 0 ? currentEnergy / maxEnergy : 0f;
    
    public event System.Action<float> OnEnergyChanged;
    
    private void Update()
    {
        RegenerateEnergy();
    }
    
    public bool HasEnergy()
    {
        return currentEnergy > 0f;
    }
    
    public bool UseEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
            timeSinceLastUse = 0f;
            OnEnergyChanged?.Invoke(currentEnergy);
            return true;
        }
        return false;
    }
    
    public void RestoreEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy);
    }
    
    public void RestoreEnergyFull()
    {
        currentEnergy = maxEnergy;
        OnEnergyChanged?.Invoke(currentEnergy);
    }
    
    private void RegenerateEnergy()
    {
        if (currentEnergy >= maxEnergy) return;
        
        timeSinceLastUse += Time.deltaTime;
        
        if (timeSinceLastUse >= energyRegenDelay)
        {
            float roomForRegen = maxEnergy - currentEnergy;
            float regenAmount = energyRegenRate * Time.deltaTime;
            
            if (regenAmount > roomForRegen)
                regenAmount = roomForRegen;
            
            currentEnergy += regenAmount;
            OnEnergyChanged?.Invoke(currentEnergy);
        }
    }
    
    public void SetMaxEnergy(float newMax)
    {
        float ratio = EnergyPercentage;
        maxEnergy = newMax;
        currentEnergy = maxEnergy * ratio;
        OnEnergyChanged?.Invoke(currentEnergy);
    }
    
    public void UpgradeEnergy(float additionalMax, float additionalRegen)
    {
        maxEnergy += additionalMax;
        energyRegenRate += additionalRegen;
        OnEnergyChanged?.Invoke(currentEnergy);
    }
}
