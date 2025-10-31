# Mountain Drifter

**Genre:** Racing / Physics / Driving Simulation

**Core Gameplay Loop:** Drive → Drift → Earn points → Upgrade → Unlock new cars & tracks

## Concept

Race and drift through breathtaking mountain roads with realistic physics, cinematic camera angles, and rewarding progression. Players start with a simple car and work their way up by mastering challenging downhill tracks, earning reputation, and unlocking high-performance drift machines.

Think of it as a fusion between Initial D, Art of Rally, and CarX Drift Racing — fast, stylish, and skill-based.

## Key Features

### 🎮 Driving & Physics
- Realistic but fun drift physics with adjustable traction, tire grip, and weight shift
- Manual or automatic transmission options
- Handbrake drifting, clutch kick, and feint drift mechanics
- Dynamic camera that shifts and shakes during drifts for cinematic feel
- Replay mode for highlight videos and social sharing

### 🌄 Tracks & Environments
- Beautiful mountain passes inspired by Japan, the Alps, and California canyons
- Weather and time-of-day effects (foggy dawns, sunsets, night runs, rain)
- Procedural skid marks, drifting smoke, and environmental soundscapes
- Shortcuts and scenic overlooks hidden along the route

### 🚗 Progression System
- Earn drift points and money for style, speed, and combos
- Buy and tune new cars: upgrade engine, suspension, tires, weight, and cosmetics
- Unlock new tracks and events as skill increases
- Optional free roam mode for practice and exploration

### 💰 Customization & Tuning
- Deep car customization (paint jobs, decals, spoilers, wheels)
- Adjustable tuning sliders for handling and drift balance
- Garage menu to preview and test settings

### 🎧 Sound & Style
- Synthwave / chill electronic soundtrack
- Realistic sound mixing (engine roars, tire squeals, echo in tunnels)
- Minimalist UI with speedometer, RPM, gear, and drift score combo meter

## Art Direction

Stylized low-poly realism — crisp geometry, soft fog layers, and warm sunlight for immersive depth.

Think Art of Rally meets Polytrack: clean yet atmospheric.

Use post-processing (bloom, depth of field, motion blur) for cinematic immersion.

## Project Structure

```
MountainDrifter/
├── Assets/
│   └── _Game/
│       ├── Scripts/
│       │   ├── Core/
│       │   │   ├── GameManager.cs
│       │   │   ├── SaveSystem.cs
│       │   │   ├── CarData.cs
│       │   │   └── ProgressionManager.cs
│       │   ├── Vehicles/
│       │   │   ├── DriftCarController.cs
│       │   │   └── TransmissionController.cs
│       │   ├── Drift/
│       │   │   ├── DriftScoring.cs
│       │   │   └── DriftMechanics.cs
│       │   ├── Camera/
│       │   │   ├── CinematicCameraController.cs
│       │   │   ├── CameraShake.cs
│       │   │   └── ReplayCamera.cs
│       │   ├── Track/
│       │   │   ├── MountainTrack.cs
│       │   │   ├── TrackGenerator.cs
│       │   │   └── SkidMarkGenerator.cs
│       │   ├── Environment/
│       │   │   ├── WeatherController.cs
│       │   │   └── TimeOfDayController.cs
│       │   ├── UI/
│       │   │   ├── DriftHUD.cs
│       │   │   ├── GarageMenu.cs
│       │   │   ├── TuningMenu.cs
│       │   │   └── MainMenu.cs
│       │   ├── Audio/
│       │   │   ├── EngineAudio.cs
│       │   │   ├── TireAudio.cs
│       │   │   └── MusicController.cs
│       │   ├── Customization/
│       │   │   ├── CarCustomizer.cs
│       │   │   └── PaintSystem.cs
│       │   └── Replay/
│       │       ├── ReplayRecorder.cs
│       │       └── ReplayPlayer.cs
│       ├── Resources/
│       ├── Prefabs/
│       └── Materials/
└── README.md
```

