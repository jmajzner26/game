using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class InventorySlot
{
    public ResourceType resource;
    public int quantity;
    
    public InventorySlot(ResourceType resource, int quantity = 0)
    {
        this.resource = resource;
        this.quantity = quantity;
    }
    
    public bool IsEmpty => resource == null || quantity <= 0;
    public bool IsFull => resource != null && quantity >= resource.maxStackSize;
    public int SpaceRemaining => resource != null ? resource.maxStackSize - quantity : 0;
}

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 20;
    [SerializeField] private int baseCapacity = 100;
    
    private List<InventorySlot> slots = new List<InventorySlot>();
    private int currentCapacity;
    
    public int MaxSlots => maxSlots;
    public int CurrentCapacity => currentCapacity;
    public int UsedCapacity
    {
        get
        {
            int total = 0;
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty)
                    total += slot.quantity;
            }
            return total;
        }
    }
    
    public event System.Action<InventorySlot> OnSlotChanged;
    public event System.Action OnInventoryFull;
    
    private void Awake()
    {
        currentCapacity = baseCapacity;
        InitializeSlots();
    }
    
    private void InitializeSlots()
    {
        slots.Clear();
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlot(null, 0));
        }
    }
    
    public bool AddResource(ResourceType resource, int quantity)
    {
        if (resource == null || quantity <= 0) return false;
        
        // Check if we have space
        if (UsedCapacity + quantity > currentCapacity)
        {
            OnInventoryFull?.Invoke();
            return false;
        }
        
        // Try to add to existing stack first
        var existingSlot = slots.FirstOrDefault(s => 
            s.resource == resource && !s.IsFull);
        
        if (existingSlot != null)
        {
            int canAdd = Mathf.Min(quantity, existingSlot.SpaceRemaining);
            existingSlot.quantity += canAdd;
            OnSlotChanged?.Invoke(existingSlot);
            quantity -= canAdd;
        }
        
        // If still have items, try new slots
        while (quantity > 0)
        {
            var emptySlot = slots.FirstOrDefault(s => s.IsEmpty);
            if (emptySlot == null) break;
            
            int canAdd = Mathf.Min(quantity, resource.maxStackSize);
            emptySlot.resource = resource;
            emptySlot.quantity = canAdd;
            OnSlotChanged?.Invoke(emptySlot);
            quantity -= canAdd;
        }
        
        // If couldn't add all, trigger inventory full
        if (quantity > 0)
        {
            OnInventoryFull?.Invoke();
            return false;
        }
        
        return true;
    }
    
    public bool RemoveResource(ResourceType resource, int quantity)
    {
        if (resource == null || quantity <= 0) return false;
        
        int remaining = quantity;
        var slotsWithResource = slots.Where(s => s.resource == resource && !s.IsEmpty)
                                    .OrderByDescending(s => s.quantity)
                                    .ToList();
        
        foreach (var slot in slotsWithResource)
        {
            if (remaining <= 0) break;
            
            int toRemove = Mathf.Min(remaining, slot.quantity);
            slot.quantity -= toRemove;
            remaining -= toRemove;
            
            if (slot.IsEmpty)
            {
                slot.resource = null;
            }
            
            OnSlotChanged?.Invoke(slot);
        }
        
        return remaining == 0;
    }
    
    public int GetResourceCount(ResourceType resource)
    {
        if (resource == null) return 0;
        return slots.Where(s => s.resource == resource)
                   .Sum(s => s.quantity);
    }
    
    public List<InventorySlot> GetSlots()
    {
        return new List<InventorySlot>(slots);
    }
    
    public void SetCapacity(int newCapacity)
    {
        currentCapacity = newCapacity;
    }
    
    public void ExpandSlots(int additionalSlots)
    {
        maxSlots += additionalSlots;
        for (int i = 0; i < additionalSlots; i++)
        {
            slots.Add(new InventorySlot(null, 0));
        }
    }
    
    public void Clear()
    {
        foreach (var slot in slots)
        {
            slot.resource = null;
            slot.quantity = 0;
            OnSlotChanged?.Invoke(slot);
        }
    }
}
