# Shadow Duel - Implementation Guide

This guide will help you integrate the Shadow Duel scripts into Unity and get the game running.

## Quick Start Checklist

### Phase 1: Unity Project Setup

1. **Create Unity Project**
   - Open Unity Hub
   - Create new 3D (URP) project
   - Name: "ShadowDuel"
   - Unity Version: 2022.3 LTS

2. **Install Packages**
   - Package Manager → Install:
     - Input System
     - Cinemachine
     - TextMeshPro
     - Post Processing (URP)

3. **Import Scripts**
   - Create folder structure: `Assets/_Game/Scripts/`
   - Copy all C# scripts into their respective folders
   - Unity will auto-compile

### Phase 2: Scene Setup

4. **Create Test Arena**
   - New Scene: "Arena_Temple"
   - Create a ground plane (10x10m)
   - Add directional light
   - Add point lights for atmosphere

5. **Setup Player**
   ```
   Create Empty: "Player"
   - Add Character Controller
   - Add Capsule Collider
   - Add Rigidbody (for physics interactions)
   - Add Health.cs
   - Add StaminaManager.cs
   - Add ParrySystem.cs
   - Add PlayerController.cs
   - Add DirectionalCombat.cs
   - Tag: "Player"
   - Layer: Player
   ```

6. **Create Weapon**
   ```
   Create Empty: "Katana" (child of Player)
   - Add Box Collider (trigger)
   - Add Katana.cs script
   - Assign WeaponPoint in PlayerController
   ```

7. **Create Enemy**
   ```
   Create Empty: "Enemy"
   - Add NavMeshAgent
   - Add Character Controller
   - Add Health.cs
   - Add StaminaManager.cs
   - Add ParrySystem.cs
   - Add EnemyAI.cs
   - Tag: "Enemy"
   - Layer: Enemy
   ```

8. **Setup Camera**
   ```
   Add CinemachineBrain to Main Camera
   Create Cinemachine Virtual Camera
   - Follow: Player
   - Look At: Player
   - Body: Third Person Follow
   - Add to PlayerController/CameraController.cs
   ```

### Phase 3: Input System

9. **Configure Input**
   - Create: Assets → Create → Input Actions
   - Name: "ShadowDuelInput"
   - Configure actions:
     ```
     Movement Map:
     - Move (WASD)
     - Look (Mouse Delta)
     
     Combat Map:
     - Attack (Left Click)
     - Heavy Attack (Right Click)
     - Block/Parry (Space)
     - Dodge (Shift)
     - Feint (Q)
     - Special (E)
     ```

10. **Update Project Settings**
    - Edit → Project Settings → Player
    - Active Input Handling: "Input System Package (New)"

### Phase 4: Combat System Setup

11. **Create Combat Manager**
    ```
    Create Empty: "CombatManager"
    - Add CombatManager.cs
    - Assign audio clips (optional)
    ```

12. **Setup Health UI**
    ```
    Create Canvas
    - Add UI: Slider (Health Bar)
    - Add UI: Slider (Stamina Bar)
    - Add CombatHUD.cs script
    - Assign references
    ```

13. **Setup Arena Manager**
    ```
    Create Empty: "ArenaManager"
    - Add ArenaManager.cs
    - Assign hazards (optional)
    ```

### Phase 5: Testing

14. **Build Navigation**
    - Window → AI → Navigation
    - Select ground plane
    - Bake NavMesh

15. **Play Test**
    - Press Play
    - WASD to move
    - Mouse to look
    - Left Click to attack
    - Space to parry
    - Test combat mechanics

## Common Setup Issues

### Issue: "Component missing" errors
**Solution:** Make sure all required components are added to GameObjects as specified in the scripts

### Issue: "Null reference exceptions"
**Solution:** Assign all public serialized fields in the Inspector for each script

### Issue: "Input not working"
**Solution:** Check that Input System package is installed and Active Input Handling is set correctly

### Issue: "NavMeshAgent errors"
**Solution:** Bake the NavMesh for your scene

### Issue: "Cinemachine errors"
**Solution:** Make sure Cinemachine package is installed and CinemachineBrain is added to main camera

## Next Steps After Setup

1. **Add Visual Polish**
   - Create or import character models
   - Add weapon models
   - Create particle effects for parry/hit/finisher

2. **Implement Audio**
   - Create audio mixer groups
   - Add combat sound effects
   - Add background music

3. **Expand Content**
   - Create more arenas
   - Add more enemy types
   - Implement skill tree UI

4. **Tune Gameplay**
   - Adjust combat timings
   - Balance stamina costs
   - Fine-tune AI behavior

## Performance Tips

- Use object pooling for effects
- Optimize draw calls with batching
- Use LOD groups for characters
- Profile regularly with Unity Profiler

## Resources

- [Unity URP Documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/index.html)
- [Input System Documentation](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [Cinemachine Documentation](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/index.html)

## Support

If you encounter issues:
1. Check console for error messages
2. Verify all components are properly assigned
3. Review script documentation
4. Check PROJECT_STATUS.md for implementation status

Happy coding! 🎮⚔️

