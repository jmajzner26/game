using System.Collections.Generic;
using UnityEngine;

namespace LostHorizon.Equipment
{
    /// <summary>
    /// Manages player equipment and tools
    /// </summary>
    public class EquipmentManager : MonoBehaviour
    {
        [Header("Equipment Slots")]
        [SerializeField] private EquipmentItem currentTool;
        [SerializeField] private EquipmentItem currentWeapon;
        [SerializeField] private EquipmentItem currentGlider;
        [SerializeField] private EquipmentItem currentBoat;

        [Header("Owned Equipment")]
        [SerializeField] private List<EquipmentItem> ownedEquipment = new List<EquipmentItem>();

        // Events
        public System.Action<EquipmentItem> OnToolEquipped;
        public System.Action<EquipmentItem> OnWeaponEquipped;
        public System.Action<EquipmentItem> OnEquipmentBroken;

        public EquipmentItem CurrentTool => currentTool;
        public EquipmentItem CurrentWeapon => currentWeapon;
        public EquipmentType CurrentToolType => currentTool != null ? currentTool.equipmentType : EquipmentType.Hand;

        /// <summary>
        /// Equip a tool
        /// </summary>
        public bool EquipTool(EquipmentItem tool)
        {
            if (tool == null || tool.IsBroken())
            {
                return false;
            }

            currentTool = tool;
            OnToolEquipped?.Invoke(tool);
            return true;
        }

        /// <summary>
        /// Equip a weapon
        /// </summary>
        public bool EquipWeapon(EquipmentItem weapon)
        {
            if (weapon == null || weapon.IsBroken())
            {
                return false;
            }

            currentWeapon = weapon;
            OnWeaponEquipped?.Invoke(weapon);
            return true;
        }

        /// <summary>
        /// Use current tool (decreases durability)
        /// </summary>
        public void UseTool(float durabilityCost = 1f)
        {
            if (currentTool != null)
            {
                currentTool.Use(durabilityCost);
                if (currentTool.IsBroken())
                {
                    OnEquipmentBroken?.Invoke(currentTool);
                    currentTool = null; // Unequip broken tool
                }
            }
        }

        /// <summary>
        /// Add equipment to owned list
        /// </summary>
        public void AddEquipment(EquipmentItem equipment)
        {
            if (equipment != null && !ownedEquipment.Contains(equipment))
            {
                ownedEquipment.Add(equipment);
            }
        }

        /// <summary>
        /// Get all owned equipment
        /// </summary>
        public List<EquipmentItem> GetOwnedEquipment()
        {
            return new List<EquipmentItem>(ownedEquipment);
        }

        /// <summary>
        /// Repair equipment
        /// </summary>
        public void RepairEquipment(EquipmentItem equipment, float amount)
        {
            if (equipment != null)
            {
                equipment.Repair(amount);
            }
        }
    }
}

