# Shadow Duel - Unity Setup Guide

## Prerequisites
- Unity 2022.3 LTS
- Universal Render Pipeline (URP) 14
- Input System Package 1.7+
- Cinemachine 2.9+
- TextMeshPro (included with Unity)

## Project Setup

### 1. Create New Project
1. Open Unity Hub
2. New Project → 3D (URP) template
3. Name: "ShadowDuel"
4. Location: Choose your preferred directory

### 2. Import Required Packages
Open **Window → Package Manager** and install:
- **Input System** (1.7+)
- **Cinemachine** (2.9+)
- **TextMeshPro** (should be pre-installed)
- **Post Processing** (URP)
- **Particle System** (should be pre-installed)
- **Timeline** (optional, for cinematic sequences)
- **ProBuilder** (optional, for quick level prototyping)

### 3. Create Folder Structure
In the **Assets** folder, create the following structure:

```
Assets/
├── _Game/
│   ├── Art/
│   │   ├── Materials/
│   │   ├── Models/
│   │   ├── Textures/
│   │   └── Shaders/
│   ├── Audio/
│   │   ├── Music/
│   │   ├── SFX/
│   │   └── Voice/
│   ├── Prefabs/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Weapons/
│   │   └── Effects/
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Player/
│   │   ├── Weapons/
│   │   ├── Enemy/
│   │   ├── Arena/
│   │   ├── Progression/
│   │   ├── UI/
│   │   └── Audio/
│   ├── Scenes/
│   │   ├── Boot.unity
│   │   ├── MainMenu.unity
│   │   ├── Arena_Temple.unity
│   │   ├── Arena_Alley.unity
│   │   └── Arena_Battlefield.unity
│   └── Resources/
│       ├── WeaponConfigs/
│       ├── SkillTrees/
│       └── ArenaConfigs/
├── ThirdParty/
└── StreamingAssets/
```

### 4. Configure URP Settings
1. Go to **Edit → Project Settings → Graphics**
2. Assign URP Asset to **Scriptable Render Pipeline Settings**
3. Configure renderer features:
   - **Bloom** (for glowing effects)
   - **Volumetric Fog**
   - **Depth of Field**
   - **Color Grading**
   - **Screen Space Shadows**
   - **Motion Blur** (light)

### 5. Quality Settings
Configure for cinematic performance:
- **Target:** 60 FPS (Desktop), 30 FPS (Consoles)
- **VSync:** ON (to prevent tearing)
- **Fixed Timestep:** 0.0167 (60 FPS)
- **Render Scale:** 1.0
- **Particle Ray Budget:** 512
- **Shadow Distance:** 100
- **Shadow Cascades:** 4

### 6. Set Up Input System
1. Create new **Input Actions** asset:
   - Right-click in Assets → Create → Input Actions
   - Name it: "ShadowDuelInput"

2. Create the following action maps:
   - **Combat** - Main combat controls
     - Attack (Left Click)
     - Heavy Attack (Right Click)
     - Block/Parry (Space)
     - Dodge (Shift)
     - Feint (Q)
     - Special Ability (E)
     - Finisher (R)
   - **Movement**
     - Move (WASD)
     - Look (Mouse Delta)
     - Run (Hold Shift while moving)
   - **UI**
     - Navigate (Arrow Keys/WASD)
     - Submit (Enter)
     - Cancel (ESC)
     - Pause (ESC)

3. Go to **Edit → Project Settings → Player**
   - Under **Other Settings**, set **Active Input Handling** to **Input System Package (New)**

### 7. Configure Lighting
1. Open **Window → Rendering → Lighting**
2. Set up lighting for atmospheric combat:
   - **Environment Lighting:** Skybox
   - **Realtime/Global Illumination:** Baked
   - **Ambient Mode:** Skybox
   - **Reflection Mode:** Skybox
3. Create a custom skybox material for each arena theme

### 8. Import Scripts
1. Copy all C# scripts from the Scripts folder into the appropriate subdirectories
2. Unity will compile them automatically

### 9. Create ScriptableObjects
Create configuration assets for weapons and skills:

**Weapon Configs:**
1. Right-click in Resources/WeaponConfigs
2. Create → WeaponConfig
3. Configure each weapon type (Katana, Twin Blades, Claymore)

**Arena Configs:**
1. Right-click in Resources/ArenaConfigs
2. Create → ArenaConfig
3. Set up arena parameters (hazards, weather, props)

### 10. Build Settings
1. Go to **File → Build Settings**
2. Add scenes in order:
   - 0: Boot
   - 1: MainMenu
   - 2+: Arena scenes
3. Configure platform settings:
   - **Windows/Mac/Linux:** Default
   - **Consoles:** Platform-specific settings

### 11. Layer Setup
Go to **Edit → Project Settings → Tags and Layers**:
- Layer 3: Player
- Layer 4: Enemy
- Layer 5: Weapon
- Layer 6: Environment
- Layer 7: UI
- Layer 8: Ignore Raycast

### 12. Physics Settings
Go to **Edit → Project Settings → Physics**:
- **Gravity:** -9.81 (standard)
- **Default Solver Iterations:** 6
- **Default Solver Velocity Iterations:** 1
- Enable **Bounce Threshold** for impact effects

### 13. Audio Settings
Go to **Edit → Project Settings → Audio**:
- **DSP Buffer Size:** Good latency (recommended)
- **Sample Rate:** 48000 Hz
- Create audio mixer groups:
  - Master
  - Music
  - Combat SFX
  - Ambience
  - UI

## Testing Setup

### Test Scene Structure
1. Create a basic test arena (10x10m platform)
2. Add basic lighting (Directional Light + Point Light)
3. Add a player character with all combat scripts
4. Add a single enemy for testing
5. Add basic UI canvas for HUD

### Camera Setup
1. Add a Cinemachine camera with **CinemachineBrain**
2. Configure camera as **Third Person** with:
   - Smooth follow
   - Collision avoidance
   - Camera shake support
   - Dynamic framing for finishers

## Troubleshooting

**Issue:** Input System errors  
**Solution:** Make sure Active Input Handling is set to Input System Package (New)

**Issue:** URP materials appear wrong  
**Solution:** Update materials to URP shaders, or create new URP materials

**Issue:** Performance issues  
**Solution:** Optimize draw calls, enable GPU instancing, reduce shadow quality

**Issue:** Scripts not compiling  
**Solution:** Check for missing references, ensure all namespaces are correct

## Next Steps

1. Set up version control (Git)
2. Import starter assets (if available)
3. Build basic player controller
4. Create first arena
5. Implement core combat loop
6. Add audio placeholder assets
7. Build and test

## Resources

- [Unity URP Documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/index.html)
- [Input System Documentation](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [Cinemachine Documentation](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/index.html)

