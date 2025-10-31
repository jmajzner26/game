# Quick Start - Mining Game Setup

## 🎯 Step 1: Create the Scene

1. **Open Unity Editor**
2. **Create New Scene**: File → New Scene → Basic (Core)
3. **Save Scene**: File → Save As → Name it `MiningGame.unity`
4. **Save in**: `Assets/_Game/Scenes/` folder (create if needed)

## 🎯 Step 2: Create Scene Objects

### A. Game Managers (Create Empty GameObjects)

1. **GameManager**
   - Right-click Hierarchy → Create Empty → Name: `GameManager`
   - Add Component → Scripts → `GameManager`

2. **EconomyManager**
   - Right-click Hierarchy → Create Empty → Name: `EconomyManager`
   - Add Component → Scripts → `EconomyManager`

3. **SoundManager**
   - Right-click Hierarchy → Create Empty → Name: `SoundManager`
   - Add Component → Scripts → `SoundManager`
   - Add 2 AudioSource components (name them MusicSource and SFXSource)
   - Drag them to SoundManager script fields

### B. Terrain Generator

1. **TerrainGenerator**
   - Right-click Hierarchy → Create Empty → Name: `TerrainGenerator`
   - Add Component → Scripts → `TerrainGenerator`
   - Create a cube prefab for blocks (see Step 3)

### C. Player Setup

1. **Create Player**
   - Right-click Hierarchy → 3D Object → Capsule → Name: `Player`
   - Position: (0, 5, 0) - starts above ground
   
2. **Add Components to Player**:
   - `CharacterController` (automatically adds Rigidbody)
   - `PlayerController` script
   - `PlayerEnergy` script
   - `Inventory` script
   - `MiningController` script

3. **Create Camera Child**
   - Right-click Player → Create Empty → Name: `Camera`
   - Position: (0, 1.6, 0) - eye height
   - Add Component → Camera
   - Set Tag to "MainCamera"
   - Assign to PlayerController script's Camera Transform field

4. **Create Ground Check**
   - Right-click Player → Create Empty → Name: `GroundCheck`
   - Position: (0, -0.9, 0) - at player feet
   - Assign to PlayerController script's Ground Check field

### D. UI Setup

1. **Create Canvas**
   - Right-click Hierarchy → UI → Canvas → Name: `UI_Canvas`
   - Canvas Scaler → UI Scale Mode: Scale With Screen Size

2. **HUD Elements**
   - Right-click Canvas → Create Empty → Name: `HUD`
   - Add Component → Scripts → `MiningHUD`
   - Create TextMeshPro text objects for:
     - Money Text
     - Energy Text
     - Depth Text
     - Inventory Text
   - Create Slider for Energy Bar
   - Assign all to MiningHUD component fields

3. **Shop Panel**
   - Right-click Canvas → UI → Panel → Name: `ShopPanel`
   - Set Active: false
   - Add Component → Scripts → `ShopUI`
   - Create button prefabs for shop items

4. **Inventory Panel**
   - Right-click Canvas → UI → Panel → Name: `InventoryPanel`
   - Set Active: false
   - Add Component → Scripts → `InventoryUI`
   - Create slot prefabs for inventory

## 🎯 Step 3: Create Block Prefab

1. **Create Block**
   - Right-click Hierarchy → 3D Object → Cube → Name: `Block`
   - Scale: (1, 1, 1)
   - Add Component → Scripts → `MineableBlock`
   - Add Component → Box Collider (should auto-add)

2. **Create Material**
   - Right-click Project → Create → Material → Name: `BlockMaterial`
   - Assign color/texture

3. **Make Prefab**
   - Drag Block from Hierarchy to Project window
   - Creates `Block.prefab`
   - Assign this prefab to TerrainGenerator's Block Prefab field

## 🎯 Step 4: Create ScriptableObjects

### Resource Types

1. **Stone**
   - Right-click Project → Create → Mining → Resource Type
   - Name: `Stone`
   - Display Name: "Stone"
   - Value: 1
   - Hardness: 1
   - Rarity: 1.0
   - Min Depth: 0, Max Depth: 100
   - Spawn Chance: 0.8

2. **Coal**
   - Create → Mining → Resource Type
   - Name: `Coal`
   - Value: 5
   - Hardness: 1.5
   - Rarity: 0.7
   - Min Depth: 5, Max Depth: 50
   - Spawn Chance: 0.3

3. **Iron**
   - Create → Mining → Resource Type
   - Name: `Iron`
   - Value: 15
   - Hardness: 2
   - Rarity: 0.5
   - Min Depth: 10, Max Depth: 80
   - Spawn Chance: 0.2

4. **Gold**
   - Create → Mining → Resource Type
   - Name: `Gold`
   - Value: 50
   - Hardness: 2.5
   - Rarity: 0.3
   - Min Depth: 20, Max Depth: 100
   - Spawn Chance: 0.1

5. **Diamond**
   - Create → Mining → Resource Type
   - Name: `Diamond`
   - Value: 200
   - Hardness: 3
   - Rarity: 0.1
   - Min Depth: 30, Max Depth: 150
   - Spawn Chance: 0.05

### Tools

1. **Basic Pickaxe**
   - Right-click Project → Create → Mining → Tool
   - Name: `BasicPickaxe`
   - Mining Power: 1.0
   - Mining Speed: 1.0

2. **Iron Pickaxe**
   - Create → Mining → Tool
   - Name: `IronPickaxe`
   - Mining Power: 2.0
   - Mining Speed: 1.5

3. **Gold Pickaxe**
   - Create → Mining → Tool
   - Name: `GoldPickaxe`
   - Mining Power: 3.0
   - Mining Speed: 2.0

## 🎯 Step 5: Configure Components

### EconomyManager
1. Open EconomyManager in Inspector
2. Add Resource Prices:
   - Stone: Base Price = 1
   - Coal: Base Price = 5
   - Iron: Base Price = 15
   - Gold: Base Price = 50
   - Diamond: Base Price = 200

### TerrainGenerator
1. Assign Block Prefab
2. Add all ResourceType ScriptableObjects to Available Resources array

### EquipmentShop
1. Create Empty GameObject → Name: `EquipmentShop`
2. Add Component → Scripts → `EquipmentShop`
3. Add all ToolStats ScriptableObjects to Available Tools array

### MiningController
1. On Player object, assign:
   - Mining Point: Camera transform
   - Block Layer Mask: Default layer

### PlayerController
1. Assign:
   - Camera Transform: Camera child
   - Ground Check: GroundCheck child
   - Ground Mask: Default layer

## 🎯 Step 6: Lighting Setup

1. **Directional Light** (should already exist)
   - Set Intensity: 1.2
   - Rotation: (50, -30, 0)

2. **Environment**
   - Window → Rendering → Lighting
   - Generate Lighting

## 🎯 Step 7: Test!

1. **Press Play**
2. **Move**: WASD keys
3. **Look**: Mouse
4. **Mine**: Left Click on blocks
5. **Shop**: Press B
6. **Inventory**: Press I

## ✅ Checklist

- [ ] Scene created and saved
- [ ] GameManager created and configured
- [ ] EconomyManager created and resources added
- [ ] TerrainGenerator created and prefab assigned
- [ ] Player created with all components
- [ ] Camera and GroundCheck created
- [ ] Canvas and UI panels created
- [ ] Block prefab created
- [ ] ResourceType ScriptableObjects created
- [ ] ToolStats ScriptableObjects created
- [ ] All references assigned in Inspector
- [ ] Scene plays without errors

## 🐛 Troubleshooting

**No blocks spawning?**
- Check TerrainGenerator has Block Prefab assigned
- Check Available Resources array has entries
- Check terrain generation is enabled

**Can't mine blocks?**
- Check MiningController's Block Layer Mask includes block layer
- Check blocks have Collider components
- Check mining range is set (default: 3)

**UI not showing?**
- Check Canvas is enabled
- Check UI scripts are assigned to panels
- Check TextMeshPro is imported (Window → TextMeshPro → Import TMP Essentials)

**Player can't move?**
- Check CharacterController is added
- Check Ground Check is assigned
- Check camera is assigned

---

**After setup, you should see blocks spawn in the terrain when you press Play! ⛏️**
