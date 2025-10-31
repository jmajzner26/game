# Shadow Duel

## Overview
A cinematic sword-fighting game where precision, timing, and reflexes determine victory. Players face off in intense 1v1 duels set in atmospheric environments.

**Genre:** Action / Combat / Arena Fighter  
**Engine:** Unity 2022.3 LTS (URP)  
**Platform:** PC/Console

## Core Gameplay Loop
Train → Battle → Earn XP → Upgrade → Unlock new skills

## Key Features

- **Skill-based combat:** Parry, dodge, feint, and counter using directional input and stamina management
- **Slow-motion finishers:** When landing a perfect counter or final blow, time slows for a dramatic kill sequence
- **Weapon mastery:** Unlock new weapon types — katanas, twin blades, claymores — each with unique move sets
- **AI progression:** Enemies get smarter, learn your patterns, and adapt over time
- **Arena variety:** Dynamic arenas with breakable props, environmental hazards, and weather effects
- **Customization:** Upgrade armor, learn special abilities (shadow dash, disarm, critical strike), and personalize your fighter's look
- **Optional multiplayer:** Duel friends online or locally in ranked or custom matches

## Art Style
Stylized realistic or cel-shaded — something between Ghost of Tsushima and Aragami, with heavy use of lighting and atmosphere (glowing blades, mist, dust)

## Installation & Setup

See [SETUP_GUIDE.md](SETUP_GUIDE.md) for detailed Unity setup instructions.

## Project Structure

```
Assets/_Game/
├── Scripts/
│   ├── Core/
│   │   ├── CombatManager.cs
│   │   ├── ParrySystem.cs
│   │   ├── StaminaManager.cs
│   │   └── FinisherSystem.cs
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── DirectionalCombat.cs
│   │   └── CameraController.cs
│   ├── Weapons/
│   │   ├── WeaponBase.cs
│   │   ├── Katana.cs
│   │   ├── TwinBlades.cs
│   │   └── Claymore.cs
│   ├── Enemy/
│   │   ├── EnemyAI.cs
│   │   ├── AdaptiveAI.cs
│   │   └── EnemySpawner.cs
│   ├── Arena/
│   │   ├── ArenaManager.cs
│   │   ├── EnvironmentalHazard.cs
│   │   └── WeatherController.cs
│   ├── Progression/
│   │   ├── PlayerProfile.cs
│   │   ├── SkillTree.cs
│   │   └── UnlockManager.cs
│   ├── UI/
│   │   ├── MainMenu.cs
│   │   ├── CombatHUD.cs
│   │   ├── CustomizationUI.cs
│   │   └── PauseMenu.cs
│   └── Audio/
│       ├── CombatAudio.cs
│       └── MusicController.cs
├── Prefabs/
│   ├── Player/
│   ├── Enemies/
│   ├── Weapons/
│   └── Effects/
└── Resources/
    ├── WeaponConfigs/
    └── SkillTrees/
```

## Development Roadmap

### Phase 1: Core Combat (MVP)
- ✅ Project structure and documentation
- ⏳ Player controller with movement
- ⏳ Basic combat mechanics (attack, block, parry)
- ⏳ Stamina system
- ⏳ Single enemy type with basic AI

### Phase 2: Combat Polish
- ⏳ Slow-motion finishers
- ⏳ Directional combat system
- ⏳ Feint system
- ⏳ Multiple attack combos
- ⏳ Camera shake and screen effects

### Phase 3: Weapons & Abilities
- ⏳ Weapon switching system
- ⏳ Three unique weapon types
- ⏳ Special abilities (shadow dash, disarm, etc.)
- ⏳ Weapon unlock system

### Phase 4: AI & Arenas
- ⏳ Adaptive AI with pattern learning
- ⏳ Multiple enemy types
- ⏳ Dynamic arenas
- ⏳ Environmental hazards
- ⏳ Weather system

### Phase 5: Progression
- ⏳ Skill tree system
- ⏳ Player progression and XP
- ⏳ Armor customization
- ⏳ Unlock system

### Phase 6: Polish
- ⏳ UI/UX refinement
- ⏳ Audio system
- ⏳ Visual effects
- ⏳ Performance optimization

### Phase 7: Multiplayer (Optional)
- ⏳ Local multiplayer
- ⏳ Online matchmaking
- ⏳ Ranked system

## Controls

### Combat
- **WASD** - Movement
- **Mouse** - Directional combat input
- **Left Click** - Light attack
- **Right Click** - Heavy attack
- **Space** - Parry
- **Shift** - Dodge
- **Q** - Feint
- **E** - Special ability
- **R** - Finisher (when available)

### UI
- **ESC** - Pause menu
- **Tab** - Open skill tree
- **C** - Customization menu

## Credits

Developed in Unity using Universal Render Pipeline.

## License

All rights reserved.

