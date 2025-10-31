# How to Access the Mining Game in Unity

## 🎮 Quick Answer

**You need to create a Unity Scene file** - the scripts are ready, but you need to set up the scene in Unity Editor.

## 📍 Where to Find It

1. **Open Unity Editor**
2. **Create New Scene**: File → New Scene → Basic (Core)
3. **Save Scene**: File → Save As → `MiningGame.unity`
4. **Save location**: Create folder `Assets/_Game/Scenes/` and save there

## ⚡ Quick Setup Method

### Option 1: Use Scene Setup Helper (Easiest)

1. **Open Unity Editor**
2. **Create Empty GameObject** in your scene:
   - Right-click Hierarchy → Create Empty
   - Name it: `SceneSetupHelper`
3. **Add Component**:
   - Select SceneSetupHelper
   - Inspector → Add Component → Scripts → `SceneSetupHelper`
4. **Run Setup**:
   - In Inspector, you'll see "Setup Mining Game Scene" button
   - Click it, or right-click component → "Setup Mining Game Scene"
   - This automatically creates all GameObjects and basic structure!

5. **Manual Steps Still Needed**:
   - Create ResourceType ScriptableObjects
   - Create ToolStats ScriptableObjects  
   - Create Block Prefab
   - Assign references in Inspector

### Option 2: Manual Setup (Step by Step)

Follow the **QUICK_START.md** guide for detailed instructions.

## 🎯 What You'll See After Setup

When you press **Play** in Unity:
- ✅ Blocks will spawn in the terrain (procedurally generated)
- ✅ Player can move with WASD
- ✅ Player can look around with mouse
- ✅ Player can mine blocks with left click
- ✅ Resources collect in inventory
- ✅ Press 'I' to open Inventory
- ✅ Press 'B' to open Shop

## 🔍 Finding Scripts in Unity

All scripts are located in:
```
Assets/_Game/Scripts/Mining/
```

To see them:
1. In Unity Project window
2. Navigate to: `Assets/_Game/Scripts/Mining/`
3. You'll see folders:
   - `Core/` - GameManager, EconomyManager, Inventory, ResourceType
   - `Blocks/` - MineableBlock, TerrainGenerator
   - `Player/` - PlayerController, PlayerEnergy, MiningController
   - `Upgrades/` - ToolStats, EquipmentShop
   - `UI/` - MiningHUD, ShopUI, InventoryUI
   - `Audio/` - SoundManager

## 📝 ScriptableObjects Menu

After scripts compile, you'll find new menu options:
- **Create → Mining → Resource Type**
- **Create → Mining → Tool**

Use these to create your resources and tools!

## ✅ Checklist

- [ ] Unity Editor is open
- [ ] New scene created and saved
- [ ] SceneSetupHelper used OR manual setup completed
- [ ] ResourceType ScriptableObjects created
- [ ] ToolStats ScriptableObjects created
- [ ] Block Prefab created
- [ ] All references assigned in Inspector
- [ ] Press Play - game should run!

## 🐛 Still Not Working?

**If you don't see the scripts:**
- Make sure Unity compiled successfully (check Console for errors)
- Refresh: Assets → Refresh (or Ctrl+R)
- Check that scripts are in `Assets/_Game/Scripts/Mining/` folder

**If scene setup doesn't work:**
- Check Console for errors
- Make sure all scripts compiled without errors
- Try manual setup from QUICK_START.md

**If game doesn't run:**
- Check that Player has all components
- Check that Camera is set up correctly
- Check that GameManager, EconomyManager exist in scene
- Check Console for runtime errors

---

**The game is ready - you just need to set up the Unity scene!** ⛏️
