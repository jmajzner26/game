# Polytrack Racer - 3D Racing Game

A visually stunning and highly engaging 3D racing game built with Unity, featuring smooth physics, fast-paced gameplay, and immersive sound design. The game features a modern, sleek, and slightly futuristic low-poly aesthetic similar to *Polytrack* or *Hotshot Racing*.

## 🎮 Features

### Core Racing
- **Realistic but fun driving physics** with drifting, boosts, and impactful collisions
- **Lap system** with checkpoints, countdown timers, and best-lap saving
- **Multiple camera options** (chase cam, cockpit view, cinematic replay)
- **Dynamic FOV** that adapts to speed for enhanced sensation of velocity

### Tracks & Cars
- **Multiple unique tracks**: City circuit, mountain pass, desert canyon, neon night track
- **Distinct cars** with different stats for speed, acceleration, and handling
- **Track builder** system using splines for easy track creation
- **Surface zones** with varying grip (asphalt, dirt, snow, neon-wet)

### Game Modes
- **Single Race** - Race against AI opponents
- **Time Trial** - Beat your best lap time with ghost replay
- **Split-Screen** - Local 2-player multiplayer

### Audio
- **Dynamic soundtrack** that adapts to race intensity
- **Engine audio** based on RPM and throttle
- **Tire skid sounds** during drifting
- **Environmental sounds** (wind, impacts)

### UI & Progression
- **Main menu** with car and track selection
- **Garage** for viewing and selecting cars
- **Leaderboard system** with local best times
- **Settings menu** for audio, graphics, and controls
- **Progression system** with car unlocks

### AI System
- **PID-based AI drivers** with intelligent waypoint following
- **Rubber-banding** for competitive racing
- **Overtaking behavior** for dynamic races

### Visual Effects
- **Motion blur** and speed lines
- **Dynamic lighting** with time of day system
- **Weather effects** (rain, snow)
- **Camera shake** on impacts
- **Chromatic aberration** on crashes

## 🛠️ Technical Details

### Engine & Requirements
- **Unity Version**: 2022.3 LTS
- **Render Pipeline**: Universal Render Pipeline (URP) 14
- **Required Packages**:
  - Input System 1.7+
  - Cinemachine 2.9+
  - TextMeshPro (included)

### Project Structure
```
Assets/_Game/
├── Scripts/
│   ├── Core/          # Game managers, configs, progression
│   ├── Vehicles/      # Car physics and control
│   ├── Track/         # Track building and checkpoints
│   ├── AI/            # AI driver behavior
│   ├── Camera/        # Camera controllers and effects
│   ├── UI/            # Menu systems and HUD
│   └── Audio/         # Sound and music systems
├── Resources/
│   ├── CarConfigs/    # ScriptableObject car configurations
│   └── TrackConfigs/  # ScriptableObject track configurations
└── Prefabs/
    ├── Cars/          # Car prefabs
    └── Tracks/        # Track elements (checkpoints, boost pads)
```

## 🚀 Setup Instructions

### 1. Create Unity Project
1. Open Unity Hub
2. Create New Project → **3D (URP)** template
3. Name: "PolytrackRacer" (or your preferred name)

### 2. Install Required Packages
1. Open **Window → Package Manager**
2. Install the following packages:
   - **Input System** (1.7+)
   - **Cinemachine** (2.9+)
   - **TextMeshPro** (usually already included)

### 3. Import Scripts
1. Copy the `Assets/_Game/Scripts/` folder into your Unity project
2. Ensure all script folders match the structure above

### 4. Create Folder Structure
Create the following folders in `Assets/_Game/`:
- `Resources/CarConfigs/`
- `Resources/TrackConfigs/`
- `Prefabs/Cars/`
- `Prefabs/Tracks/`

### 5. Configure URP Settings
1. Go to **Edit → Project Settings → Graphics**
2. Assign URP Asset to **Scriptable Render Pipeline Settings**
3. Configure URP Asset:
   - MSAA: 2 (Desktop), 0 (WebGL)
   - Shadows: Medium
   - Enable Bloom, Vignette, Motion Blur
   - Dynamic Batching: ON
   - GPU Instancing: ON

### 6. Create ScriptableObjects
1. Right-click in `Resources/CarConfigs/` → **Create → Polytrack → Car Config**
2. Create multiple car configs with different stats
3. Right-click in `Resources/TrackConfigs/` → **Create → Polytrack → Track Config**
4. Configure track settings

### 7. Build Car Prefab
1. Create empty GameObject: "Car_Base"
2. Add **Rigidbody** (Interpolate, ContinuousDynamic collision)
3. Add **BoxCollider** (coarse collision)
4. Add 4 child GameObjects with **WheelColliders**
5. Add **VehicleController** script
6. Assign CarConfig ScriptableObject
7. Save as Prefab in `Prefabs/Cars/`

### 8. Setup Input System
1. Create **Input Actions** asset
2. Create "Driving" action map with:
   - **Throttle** (W/Up Arrow)
   - **Brake** (S/Down Arrow)
   - **Steer** (A/D or Left/Right Arrows)
   - **Handbrake** (Space)
   - **Boost** (Shift)
   - **Pause** (ESC)

### 9. Build Tracks
1. Create a new scene for each track
2. Add **TrackBuilder** component to empty GameObject
3. Create a **Cinemachine Dolly Track** or custom spline
4. Assign TrackConfig ScriptableObject
5. Click "Build Track From Spline" in TrackBuilder

### 10. Create Scenes
Create the following scenes and add them to Build Settings:
- `Boot.unity` (index 0 - initialization)
- `MainMenu.unity`
- `Garage.unity`
- `Track_City.unity`
- `Track_Desert.unity`
- `Track_Mountain.unity`
- `Track_Neon.unity`

## 🎨 Customization

### Adding New Cars
1. Create a new **CarConfig** ScriptableObject
2. Configure stats (speed, acceleration, handling)
3. Assign car mesh and material
4. The car will appear in the garage automatically

### Adding New Tracks
1. Create a new **TrackConfig** ScriptableObject
2. Set track name, scene name, and number of laps
3. Build the track using **TrackBuilder**
4. The track will appear in the main menu

### Adjusting Physics
Modify values in `VehicleController.cs` or in `CarConfig` ScriptableObjects:
- **Engine Power**: Acceleration strength
- **Max Speed**: Top speed in m/s
- **Steer Sensitivity**: Handling responsiveness
- **Drift Grip**: How much grip is lost while drifting

## 🎮 Controls

### Default Controls
- **Throttle**: W / Up Arrow
- **Brake**: S / Down Arrow
- **Steer**: A/D or Left/Right Arrows
- **Handbrake**: Space
- **Boost**: Shift
- **Switch Camera**: C
- **Pause**: ESC

## 📝 Notes

- All scripts follow standard C# naming conventions
- Profile data and leaderboards are saved in `Application.persistentDataPath`
- Ghost data is saved per track in JSON format
- The game supports both single-player and local split-screen multiplayer
- AI difficulty can be adjusted via `AIDriver` settings

## 🔧 Troubleshooting

### Car doesn't move
- Check that WheelColliders are properly configured
- Ensure CarConfig is assigned to VehicleController
- Verify Input Actions are set up correctly

### Track doesn't build
- Ensure TrackConfig is assigned to TrackBuilder
- Check that spline points exist (use Cinemachine Dolly Track)
- Verify checkpoint count is set correctly

### AI cars not spawning
- Ensure AICarSpawner has a car prefab assigned
- Check that TrackBuilder has spline points
- Verify AIDriver component is on AI car prefab

## 📄 License

This project is provided as-is for educational and development purposes.

## 🙏 Credits

Built with Unity Engine and inspired by classic arcade racers like Polytrack and Hotshot Racing.

---

**Happy Racing! 🏎️💨**

