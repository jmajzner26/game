# Lost Horizon - Unity Project

## Game Overview

**Lost Horizon** is an exploration/survival/open-world adventure game where players wake up on a vast island chain after a mysterious storm. Explore, gather resources, craft tools, build bases, and uncover the secrets of a lost civilization.

## Project Structure

```
Assets/
  LostHorizon/
    Scripts/
      Player/
        PlayerController.cs
        PlayerStats.cs
        PlayerInventory.cs
      Crafting/
        CraftingSystem.cs
        CraftingRecipe.cs
        CraftingStation.cs
      Building/
        BuildingSystem.cs
        BuildableObject.cs
        BuildingPlacement.cs
      Resources/
        ResourceManager.cs
        ResourceNode.cs
        ResourceType.cs
      World/
        WorldManager.cs
        TimeOfDay.cs
        WeatherSystem.cs
        ProceduralGenerator.cs
      Lore/
        QuestManager.cs
        Quest.cs
        Relic.cs
        AncientCarving.cs
      Equipment/
        EquipmentManager.cs
        EquipmentItem.cs
      UI/
        GameHUD.cs
        InventoryUI.cs
        CraftingUI.cs
        BuildingUI.cs
      Core/
        GameManager.cs
        SaveSystem.cs
    Prefabs/
      Player/
      Buildings/
      Resources/
      Effects/
    Scenes/
      MainMenu.unity
      GameWorld.unity
    Materials/
    Models/
    Audio/
```

## Unity Setup Requirements

- **Unity Version**: 2022.3 LTS or later
- **Render Pipeline**: URP (Universal Render Pipeline)
- **Required Packages**:
  - Input System
  - Cinemachine
  - TextMeshPro
  - ProBuilder (optional, for level design)

## Core Features

1. **Exploration System**: Procedural island generation with forests, beaches, caves, mountains, and ruins
2. **Crafting System**: Gather materials and craft tools, weapons, and items
3. **Base Building**: Construct and upgrade camps and permanent bases
4. **Lore System**: Discover ancient carvings, relics, and quests
5. **Weather & Time**: Dynamic day/night cycle, weather effects, temperature
6. **Equipment System**: Upgradeable tools and equipment
7. **Co-op Support**: Multiplayer framework (optional)

## Getting Started

1. Create a new Unity 3D (URP) project
2. Import required packages via Package Manager
3. Create the folder structure above
4. Import all scripts from the Scripts/ folder
5. Set up a player prefab with PlayerController component
6. Create a GameManager in your scene
7. Build and test!

## Art Style

Semi-realistic low-poly with bright lighting, soft color palette, and atmospheric fog.

我們的冒險即將開始... (Our adventure begins...)

