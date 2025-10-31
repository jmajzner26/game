# 3D Mining Game - Unity Implementation

A fun and addictive 3D mining game where players dig underground to mine various materials and ores, collect resources, sell them for money, and upgrade equipment.

## 🎮 Features

### Core Gameplay
- **Progressive Mining Loop**: Mine → Sell → Upgrade → Explore Deeper
- **Randomized Terrain Generation**: Procedural blocks with depth-based rarity
- **Resource System**: Multiple resource types (stone, coal, iron, gold, diamonds, gems)
- **Economy System**: Dynamic pricing and resource selling
- **Inventory Management**: Stackable resources with capacity limits

### Player Systems
- **First-Person Controls**: Smooth camera and movement
- **Energy/Stamina System**: Energy-based mining with regeneration
- **Depth Tracking**: Track how deep you've gone

### Upgrade System
- **Tool Upgrades**: Better pickaxes, drills, and mining equipment
- **Equipment Shop**: Purchase upgrades with earned money
- **Progressive Upgrades**: Energy capacity, regeneration, inventory size

### UI Systems
- **HUD**: Real-time money, energy, depth, and inventory display
- **Shop UI**: Browse and purchase equipment (Press 'B')
- **Inventory UI**: Manage and sell resources (Press 'I')
- **Mining Progress**: Visual feedback for mining progress

### Visual & Audio
- **Particle Effects**: Block breaking particles
- **Sound System**: Background music and sound effects
- **Mining Feedback**: Visual indicators and progress bars

## 📁 Project Structure

```
UnityProject/Assets/_Game/Scripts/Mining/
├── Core/
│   ├── ResourceType.cs          # ScriptableObject for resource definitions
│   ├── EconomyManager.cs        # Handles pricing and selling
│   ├── Inventory.cs             # Inventory system with slots
│   └── GameManager.cs           # Main game state manager
├── Blocks/
│   ├── MineableBlock.cs         # Individual mineable blocks
│   └── TerrainGenerator.cs      # Procedural terrain generation
├── Player/
│   ├── PlayerController.cs      # First-person movement controller
│   ├── PlayerEnergy.cs          # Energy/stamina system
│   └── MiningController.cs      # Mining technics and raycast
├── Upgrades/
│   ├── ToolStats.cs             # Tool configuration ScriptableObject
│   └── EquipmentShop.cs         # Shop system for upgrades
├── UI/
│   ├── MiningHUD.cs             # Main HUD overlay
│   ├── ShopUI.cs                # Shop interface
│   ├── ShopItemUI.cs            # Individual shop item UI
│   ├── InventoryUI.cs           # Inventory interface
│   └── InventorySlotUI.cs       # Individual inventory slot UI
└── Audio/
    └── SoundManager.cs          # Audio system manager
```

## 🚀 Setup Instructions

### 1. Create ScriptableObjects

**Resources:**
1. Right-click in Project window → Create → Mining → Resource Type
2. Create resources like:
   - **Stone** (common, depth 0-100, value: $1)
   - **Coal** (uncommon, depth 5-50, value: $5)
   - **Iron** (rare, depth 10-80, value: $15)
   - **Gold** (very rare, depth 20-100, value: $50)
   - **Diamond** (extremely rare, depth 30-150, value: $200)
   - **Rare Gems** (ultra rare, depth 50-200, value: $500)

**Tools:**
1. Right-click → Create → Mining → Tool
2. Create tools like:
   - **Basic Pickaxe** (power: 1.0, speed: 1.0)
   - **Iron Pickaxe** (power: 2.0, speed: 1.5)
   - **Gold Pickaxe** (power: 3.0, speed: 2.0)
   - **Diamond Drill** (power: 5.0, speed: 3.0)

### 2. Scene Setup

**Main Scene:**
1. Create an empty GameObject named "GameManager" and add `GameManager` component
2. Create "EconomyManager" GameObject with `EconomyManager` component
3. Create "TerrainGenerator" GameObject with `TerrainGenerator` component
4. Assign ResourceType ScriptableObjects to TerrainGenerator's Available Resources array

**Player Setup:**
1. Create a Capsule or FirstPersonController
2. Add `CharacterController` component
3. Add `PlayerController`, `PlayerEnergy`, and `Inventory` components
4. Add `MiningController` component
5. Create a child GameObject for the camera (Mining Point)
6. Set up Ground Check point (empty GameObject at player feet)

**UI Setup:**
1. Create Canvas for UI
2. Add `MiningHUD` component to a GameObject
3. Create Shop UI panel with `ShopUI` component
 experimental4. Create Inventory UI panel with `InventoryUI` component
5. Link all UI references in Inspector

**Terrain:**
1. Create a Block Prefab:
   - Cube GameObject
   - Add `MineableBlock` component
   - Add Collider (Box Collider)
   - Add Renderer with material
2. Assign Block Prefab to TerrainGenerator

### 3. Configure Systems

**EconomyManager:**
- Add ResourcePrice entries for producer resource types
- Set base prices for each resource

**EquipmentShop:**
- Add ToolStats ScriptableObjects to Available Tools array
- Create ShopItem entries for upgrades (energy, inventory, slots)

**Inventory:**
- Set Max Slots (default: 20)
- Set Base Capacity (default: 100)

### 4. Audio Setup

**SoundManager:**
1. Create "SoundManager" GameObject
2. Add `SoundManager` component
3. Add two AudioSource components (one for music, one for SFX)
4. Add sound effects:
   - "BlockBreak" - sound when block is mined
   - "Pickup" - sound when resource collected
   - "Purchase" - sound when buying from shop
5. Assign background music clip

## 🎯 Controls

- **WASD** - Move
- **Mouse** - Look around
- **Space** - Jump / Mine (when looking at block)
- **Left Click** - Mine blocks
- **Left Shift** - Sprint (uses energy)
- **I** - Toggle Inventory
- **B** - Toggle Shop
- **ESC** - Pause (if implemented)

## 🔧 Customization

### Adding New Resources
1. Create ResourceType ScriptableObject
2. Configure: color, material, hardness, rarity, depth range
3. Add to TerrainGenerator's Available Resources
4. Atoms EconomyManager's ResourcePrices

### Adding New Tools
1. Create ToolStats ScriptableObject
2. Configure: mining power, speed, durability
3. Add to EquipmentShop's Available Tools
4. Create tool model prefab (optional)

### Adjusting Difficulty
- **Resource Rarity**: Lower `rarity` value = rarer
- **Mining Speed**: Adjust `hardness` in ResourceType
- **Energy Costs**: Modify `energyRegenRate` in PlayerEnergy
- **Tool Prices**: Adjust `CalculateToolPrice` in EquipmentShop

## 📝 Notes

- Blocks are generated procedurally based on depth
- Deeper blocks have higher chance of rare resources
- Inventory capacity can be upgraded at the shop
- Energy regenerates when not mining
- Game progress is saved automatically (money, depth)

## 🐛 Troubleshooting

**Blocks not mining:**
- Check block layer mask in MiningController
- Ensure blocks have Collider components
- Verify mining range and angle settings

**Resources not collecting:**
- Check Inventory capacity
- Verify GameManager and Inventory references
- Ensure signaling is working

**UI not showing:**
- Check Canvas render mode
- Verify UI prefabs are assigned
- Check event subscriptions in Start()

**Terrain not generating:**
- Verify Block Prefab is assigned
- Check TerrainGenerator chunk settings
- Ensure resources are assigned

## 🚧 Future Enhancements

- Quest system
- Crafting system
- Base building mechanics
- Multiplayer support
- Advanced particle effects
- More tool variety
- Vehicles (mining carts, drills)
- Biome system
- Weather effects

---

**Enjoy mining! ⛏️💎**
