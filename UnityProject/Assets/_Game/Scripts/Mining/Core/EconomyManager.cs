using UnityEngine;
using System.Collections.Generic;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }
    
    [Header("Market Prices")]
    [SerializeField] private List<ResourcePrice> resourcePrices = new List<ResourcePrice>();
    
    private Dictionary<string, float> priceDictionary = new Dictionary<string, float>();
    
    [System.Serializable]
    public class ResourcePrice
    {
        public ResourceType resource;
        public float basePrice;
        public float priceVariation = 0.1f; // ±10% variation
        
        public float GetCurrentPrice()
        {
            float variation = Random.Range(-priceVariation, priceVariation);
            return basePrice * (1f + variation);
        }
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePrices();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializePrices()
    {
        priceDictionary.Clear();
        foreach (var price in resourcePrices)
        {
            if (price.resource != null)
            {
                priceDictionary[price.resource.id] = price.basePrice;
            }
        }
    }
    
    public float GetResourcePrice(string resourceId)
    {
        foreach (var price in resourcePrices)
        {
            if (price.resource != null && price.resource.id == resourceId)
            {
                return price.GetCurrentPrice();
            }
        }
        
        // Fallback to dictionary
        if (priceDictionary.ContainsKey(resourceId))
        {
            return priceDictionary[resourceId];
        }
        
        Debug.LogWarning($"Price not found for resource: {resourceId}");
        return 0f;
    }
    
    public float GetResourcePrice(ResourceType resource)
    {
        if (resource == null) return 0f;
        return GetResourcePrice(resource.id);
    }
    
    public float SellResource(ResourceType resource, int amount)
    {
        float price = GetResourcePrice(resource);
        float totalValue = price * amount;
        
        GameManager.Instance.AddMoney(totalValue);
        
        return totalValue;
    }
    
    public void AddResourcePrice(ResourceType resource, float basePrice)
    {
        resourcePrices.Add(new ResourcePrice
        {
            resource = resource,
            basePrice = basePrice,
            priceVariation = 0.1f
        });
        
        if (priceDictionary != null)
        {
            priceDictionary[resource.id] = basePrice;
        }
    }
}
