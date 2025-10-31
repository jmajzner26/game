# Mountain Drifter - Unity Setup Guide

## Prerequisites

- Unity 2022.3 LTS or later
- Universal Render Pipeline (URP) package
- Cinemachine package
- Input System package
- TextMeshPro (usually included with Unity)

## Installation Steps

### 1. Create Unity Project

1. Open Unity Hub
2. Create new project with **3D (URP)** template
3. Name it "Mountain Drifter"

### 2. Install Required Packages

Open **Window → Package Manager** and install:

- **Cinemachine** (Virtual Cameras for cinematic effects)
- **Input System** (Modern input handling)
- **TextMeshPro** (If not already installed)

### 3. Import Project Files

1. Copy the entire `Assets/_Game` folder from this repository into your Unity project's `Assets` folder
2. Unity will automatically compile the scripts

### 4. Project Structure Setup

Create the following folder structure in `Assets`:

```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Vehicles/
│   │   ├── Drift/
│   │   ├── Camera/
│   │   ├── Track/
│   │   ├── Environment/
│   │   ├── UI/
│   │   ├── Audio/
│   │   ├── Customization/
│   │   └── Replay/
│   ├── Resources/
│   │   ├── CarData/
│   │   └── Input/
│   ├── Prefabs/
│   │   ├── Cars/
│   │   ├── Tracks/
│   │   └── UI/
│   └── Materials/
└── ...
```

### 5. Input System Setup

1. Create an Input Actions asset:
   - Right-click in `Assets/_Game/Resources/Input/`
   - Create → Input Actions
   - Name it "DrivingControls"

2. Configure Input Actions:
   - Create Action Map: "Driving"
   - Add Actions:
     - **Throttle** (Button) → Bound to W key / Gamepad Right Trigger
     - **Brake** (Button) → Bound to S key / Gamepad Left Trigger
     - **Steer** (Axis) → Bound to A/D keys / Gamepad Left Stick X
     - **Handbrake** (Button) → Bound to Space / Gamepad Right Bumper
     - **Clutch** (Button) → Bound to Left Shift / Gamepad Left Bumper
     - **Shift Up** (Button) → Bound to E key / Gamepad D-Pad Up
     - **Shift Down** (Button) → Bound to Q key / Gamepad D-Pad Down

### 6. Create GameManager GameObject

1. Create empty GameObject in scene
2. Name it "GameManager"
3. Add components:
   - `GameManager.cs`
   - `ProgressionManager.cs`
   - `SaveSystem.cs`

### 7. Setup Car Prefab

1. Create a car GameObject with:
   - Empty GameObject (Car Root)
     - `DriftCarController.cs`
     - `TransmissionController.cs`
     - `DriftScoring.cs`
     - `CarCustomizer.cs`
     - `Rigidbody` component
     - `WheelCollider` components (4 wheels: FL, FR, RL, RR)
   - Wheel mesh GameObjects (4)
   - Audio sources:
     - `EngineAudio.cs` on engine source
     - `TireAudio.cs` on tire source

2. Configure WheelColliders:
   - Position at wheel locations
   - Set radius: 0.3m
   - Configure suspension

3. Drag to `Assets/_Game/Prefabs/Cars/` as prefab

### 8. Create Car Data ScriptableObject

1. Right-click in `Assets/_Game/Resources/CarData/`
2. Create → Mountain Drifter → Car Data
3. Configure car stats:
   - Name, price, performance values
   - Assign car prefab
   - Set gear ratios

### 9. Setup Camera System

1. Add Cinemachine Virtual Camera:
   - Create → Cinemachine → Virtual Camera
   - Add `CinematicCameraController.cs`
   - Add `CameraShake.cs`
   - Assign follow target (car)
   - Add second Virtual Camera for drift view

2. Configure camera settings:
   - Field of View: 60-75
   - Add noise profile for shake effects

### 10. Create Track

1. Create empty GameObject: "MountainTrack"
2. Add `MountainTrack.cs` component
3. Add spline points (empty GameObjects) along track path
4. Add Checkpoints:
   - Create GameObjects with `Checkpoint.cs`
   - Place triggers along track
   - Assign checkpoint indices

5. Add Surface Zones:
   - Create GameObjects with `SurfaceZone.cs`
   - Configure surface types (asphalt, dirt, etc.)
   - Add colliders as triggers

### 11. Setup Environment

1. Add Weather Controller:
   - Create GameObject: "WeatherController"
   - Add `WeatherController.cs`
   - Setup particle systems for rain/snow

2. Add Time of Day Controller:
   - Create GameObject: "TimeOfDayController"
   - Add `TimeOfDayController.cs`
   - Assign sun light reference

### 12. Setup UI

1. Create Canvas (UI → Canvas)
2. Add HUD elements:
   - `DriftHUD.cs` on Canvas
   - Speedometer Text (TMP)
   - RPM gauge (Image)
   - Score display
   - Gear display

3. Create Menu scenes:
   - MainMenu scene with `MainMenu.cs`
   - Garage scene with `GarageMenu.cs`
   - Tuning scene with `TuningMenu.cs`

### 13. Audio Setup

1. Create Music Controller:
   - GameObject: "MusicController"
   - Add `MusicController.cs`
   - Assign music clips (menu, drift, intense)

2. Engine Audio:
   - Add to car prefab
   - Assign engine sound clips
   - Configure RPM range

### 14. Configure Project Settings

1. **Quality Settings:**
   - Enable MSAA (4x)
   - Enable shadows
   - Configure post-processing

2. **URP Settings:**
   - Enable Bloom
   - Enable Depth of Field
   - Enable Motion Blur (optional)

3. **Input System:**
   - Edit → Project Settings → Player
   - Active Input Handling: "Both" or "Input System Package (New)"

### 15. Testing

1. Create test scene:
   - Add GameManager
   - Add car prefab
   - Add track
   - Add camera system
   - Add HUD

2. Play mode testing:
   - Test driving controls
   - Test drift mechanics
   - Test scoring system
   - Test camera transitions

## Common Issues & Solutions

### Issue: Input System not working
**Solution:** Enable Input System in Project Settings → Player → Active Input Handling

### Issue: Cinemachine camera not following
**Solution:** Ensure Follow and LookAt targets are assigned in Virtual Camera

### Issue: Car not drifting
**Solution:** Check wheel collider settings, adjust grip values, verify drift threshold

### Issue: Audio not playing
**Solution:** Check Audio Source settings, verify clips are assigned, check volume levels

### Issue: Save system not working
**Solution:** Check file permissions, verify save path, ensure JSON serialization is working

## Next Steps

1. Create additional car models
2. Design more mountain tracks
3. Add visual effects (particles, trails)
4. Implement car unlock system
5. Create garage UI artwork
6. Record and implement sound effects
7. Add music tracks
8. Polish camera angles
9. Add post-processing effects
10. Optimize performance

## Performance Tips

- Use object pooling for particles
- LOD groups for car models
- Occlusion culling for tracks
- Optimize wheel mesh updates
- Cache component references

## Resources Needed

- Car models (low-poly style)
- Track models/environments
- UI artwork
- Sound effects (engine, tires, environment)
- Music tracks (synthwave/chill electronic)
- Skybox textures (dawn, day, sunset, night)

