# Lost Horizon - Unity Setup Guide

## Quick Start

1. **Create Unity Project**
   - Open Unity Hub
   - Click "New Project"
   - Select "3D (URP)" template
   - Name: "LostHorizon"
   - Location: Choose your project folder

2. **Import Scripts**
   - Copy the `LostHorizon/Scripts/` folder to `Assets/LostHorizon/Scripts/` in your Unity project
   - Unity will automatically compile the scripts

3. **Required Unity Packages**
   - Open Package Manager (Window → Package Manager)
   - Install:
     - **Input System** (1.7.0 or later)
     - **Cinemachine** (2.9.0 or later) - Optional but recommended
     - **TextMeshPro** (Included with Unity, may need to import)

4. **Setup Player**
   - Create an empty GameObject named "Player"
   - Add components:
     - CharacterController
     - PlayerController
     - PlayerStats
     - PlayerInventory
     - EquipmentManager
   - Add a Camera as a child of Player
   - Tag the Player GameObject as "Player"

5. **Setup GameManager**
   - Create an empty GameObject named "GameManager"
   - Add component: GameManager
   - Assign references to all systems (PlayerController, WorldManager, etc.)

6. **Setup World**
   - Create an empty GameObject named "WorldManager"
   - Add component: WorldManager
   - Add component: WeatherSystem
   - Add a Directional Light for the sun

7. **Create Resource Nodes**
   - Create a GameObject for each resource type (trees, rocks, etc.)
   - Add ResourceNode component
   - Set ResourceType, MaxResources, and HarvestTime
   - Add Collider (BoxCollider or SphereCollider)

8. **Create UI**
   - Create a Canvas in your scene
   - Create UI for GameHUD (health bars, stamina, etc.)
   - Create UI panels for Inventory, Crafting, and Building menus
   - Assign UI scripts to appropriate GameObjects

## Project Structure

```
Assets/
  LostHorizon/
    Scripts/
      Core/
        - GameManager.cs (Main game coordinator)
        - SaveSystem.cs (Save/load functionality)
      
      Player/
        - PlayerController.cs (Movement, interaction, harvesting)
        - PlayerStats.cs (Health, stamina, hunger)
        - PlayerInventory.cs (Resource storage)
      
      Resources/
        - ResourceType.cs (Resource enumeration)
        - ResourceNode.cs (Harvestable nodes)
        - ResourceManager.cs (Resource management)
      
      Crafting/
        - CraftingSystem.cs (Recipe management)
        - CraftingRecipe.cs (ScriptableObject recipes)
        - CraftingStation.cs (Crafting locations)
      
      Building/
        - BuildingSystem.cs (Building management)
        - BuildableObject.cs (ScriptableObject buildings)
        - BuildingPlacement.cs (Placement preview)
      
      World/
        - WorldManager.cs (Time of day)
        - WeatherSystem.cs (Weather effects)
      
      Equipment/
        - EquipmentType.cs (Tool types)
        - EquipmentItem.cs (ScriptableObject equipment)
        - EquipmentManager.cs (Equipment management)
      
      Lore/
        - Quest.cs (ScriptableObject quests)
        - QuestManager.cs (Quest system)
      
      UI/
        - GameHUD.cs (Main HUD)
        - InventoryUI.cs (Inventory menu)
        - CraftingUI.cs (Crafting menu)
        - BuildingUI.cs (Building menu)
```

## Key Features Implemented

✅ **Player Systems**
- First-person movement with stamina
- Health, hunger, and temperature systems
- Inventory management
- Resource harvesting

✅ **Crafting System**
- Recipe-based crafting
- Crafting stations
- Resource requirements

✅ **Building System**
- Placement preview system
- Resource-based building
- Building management

✅ **World Systems**
- Dynamic day/night cycle
- Weather system (rain, storms, fog)
- Time management

✅ **Equipment System**
- Tool types and equipment items
- Durability system
- Equipment management

✅ **Quest System**
- Quest objectives and rewards
- Quest progression tracking

✅ **UI Systems**
- Main HUD (health, stamina, hunger accounting, time)
- Inventory UI
- Crafting UI
- Building UI

## Next Steps

1. **Create ScriptableObjects**
   - Crafting Recipes (Right-click → Create → Lost Horizon/Crafting/Recipe)
   - Buildable Objects (Right-click → Create → Lost Horizon/Building/Buildable Object)
   - Equipment Items (Right-click → Create → Lost Horizon/Equipment/Equipment Item)
   - Quests (Right-click → Create → Lost Horizon/Lore/Quest)

2. **Build Prefabs**
   - Player prefab with all components
   - Resource node prefabs (trees, rocks, ore veins)
   - Building prefabs
   - UI prefabs

3. **Create Art Assets**
   - Low-poly models for resources
   - Building models
   - UI sprites and icons
   - Materials and textures

4. **Configure Input**
   - Set up Input Actions for player movement
   - Configure interaction keys

5. **Polish & Expand**
   - Add sound effects
   - Implement procedural generation
   - Add more recipes and buildings
   - Create quests and lore content
   - Add multiplayer support (co-op)

## Controls (Default)

- **WASD**: Move
- **Mouse**: Look around
- **Space**: Jump
- **Left Shift**: Run
- **E/Left Click**: Interact/Harvest
- **I**: Open Inventory
- **B**: Open Building Menu
- **Esc**: Pause/Menu

## Tips

- Use ScriptableObjects for all game data (recipes, buildings, equipment, quests)
- Set up layers for ground, resources, buildings, etc.
- Use tags for identifying interactable objects
- Test each system individually before integrating
- Start with simple prefabs and iterate

## Troubleshooting

**Scripts not compiling?**
- Check that all namespaces are correct
- Ensure Unity version is 2022.3 LTS or later
- Check Console for specific errors

**Player not moving?**
- Ensure CharacterController is added
- Check Input System package is installed
- Verify Input Actions are set up correctly

**Resources not harvesting?**
- Check ResourceNode has a Collider
- Verify player is close enough (3 units)
- Ensure correct tool type is equipped

**Buildings not placing?**
- Check BuildingPlacement is on GameManager or separate GameObject
- Verify ground layer mask is set correctly
- Ensure player has required resources

---

**Happy exploring! Welcome to Lost Horizon!** 🌴🏝️

