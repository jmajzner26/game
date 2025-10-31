# Unity Polytrack Racer - Setup Guide

## Prerequisites
- Unity 2022.3 LTS
- Universal Render Pipeline (URP) 14
- Input System Package 1.7+
- Cinemachine 2.9+
- TextMeshPro (included with Unity)

## Project Structure
```
Assets/
  _Game/
    Art/
      Materials/
      Models/
    Audio/
    Prefabs/
      Cars/
      Tracks/
    Scenes/
      Boot.unity
      MainMenu.unity
      Garage.unity
      Track_Desert.unity
      Track_City.unity
      Track_Snow.unity
      Track_Neon.unity
    Scripts/
      Core/
      Vehicles/
      Track/
      UI/
      Audio/
      Net/
    Resources/
      CarConfigs/
      TrackConfigs/
    Shaders/
```

## Unity Setup Steps

1. **Create New Project**
   - Open Unity Hub
   - New Project → 3D (URP) template
   - Name: "PolytrackRacer"

2. **Import Packages**
   - Window → Package Manager
   - Install: Input System, Cinemachine, TextMeshPro

3. **Create Folder Structure**
   - Create folders as listed above
   - Copy all C# scripts to appropriate Scripts/ subfolders

4. **URP Settings** (Edit → Project Settings → Graphics)
   - Assign URP Asset to Scriptable Render Pipeline Settings
   - MSAA: 2 (Desktop), 0 (WebGL)
   - Shadows: Medium
   - Enable Bloom, Vignette, Motion Blur (light)
   - Dynamic Batching: ON
   - GPU Instancing: ON

5. **Quality Settings**
   - Target 60 FPS desktop, 50+ FPS WebGL
   - VSync: OFF
   - Fixed Timestep: 0.0167

6. **Create ScriptableObjects**
   - Right-click in Resources/CarConfigs → Create → CarConfig
   - Right-click in Resources/TrackConfigs → Create → TrackConfig
   - Configure values as needed

7. **Build Car Prefab**
   - Create empty GameObject: "Car_Base"
   - Add Rigidbody (Interpolate, ContinuousDynamic collision)
   - Add BoxCollider (coarse)
   - Add 4 child GameObjects with WheelColliders
   - Add VehicleController script
   - Save as Prefab in Prefabs/Cars/

8. **Setup Input System**
   - Create Input Actions asset
   - Add "Driving" action map with actions: Throttle, Brake, Steer, Handbrake, Boost, Pause
   - Assign keys as specified

9. **Build Tracks**
   - Use TrackBuilder script in track scenes
   - Assign TrackConfig ScriptableObject
   - Create spline (use Cinemachine Dolly Track or custom spline)

10. **Build Settings**
    - Add all scenes to Build Settings
    - Boot scene = index 0
    - Configure Desktop and WebGL platforms

## File Organization
All C# scripts are organized by functionality. Import them into Unity following the folder structure.

