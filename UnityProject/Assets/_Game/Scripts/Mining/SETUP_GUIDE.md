# Quick Setup Guide - Mining Game

## 🎯 Quick Start (5 Steps)

### Step 1: Create Resources
1. Right-click in Project → **Create → Mining → Resource Type**
2. Create these resources:
   - **Stone**: value=$1, hardness=1, rarity=1.0, depth=0-100
   - **Coal**: value=$5, hardness=1.5, rarity=0.7, depth=5-50
   - **Iron**: value=$15, hardness=2, rarity=0.5, depth=10-80
   - **Gold**: value=$50, hardness=2.5, rarity=0.3, depth=20-100
   - **Diamond**: value=$200, hardness=3, rarity=0.1, depth=30-150

### Step 2: Create Tools
1. Right-click → **Create → Mining → Tool**
2. Create tools:
   - **Basic Pickaxe**: power=1.0, speed=1.0
   - **Iron Pickaxe**: power=2.0, speed=1.5
   - **Gold Pickaxe**: power=3.0, speed=2.0

### Step 3: Scene Setup
1. **Create GameManager**: Empty GameObject + `GameManager` component
2. **Create EconomyManager**: Empty GameObject + `EconomyManager` component
   - Add your resources to ResourcePrices list
3. **Create TerrainGenerator**: Empty GameObject + `TerrainGenerator` component
   - Assign block prefab (cube with MineableBlock)
   - Assign all ResourceType ScriptableObjects

### Step 4: Player Setup
1. Create Capsule (or import First Person Controller)
2. Add components:
   - `CharacterController`
   - `PlayerController`
   - `PlayerEnergy`
   - `Inventory`
   - `MiningController`
3. Create child "Camera" for first-person view
4. Create child "GroundCheck" at player feet

### Step 5: UI Setup
1. Create Canvas
2. Add **MiningHUD** component
3. Create Shop Panel with **ShopUI** component
4. Create Inventory Panel with **InventoryUI** component
5. Wire up all references in Inspector

## ✅ Test Checklist
- [ ] Can move with WASD
- [ ] Can look around with mouse
- [ ] Blocks spawn in terrain
- [ ] Can mine blocks (left click)
- [ ] Resources go to inventory
- [ ] Can open shop (B key)
- [ ] Can open inventory (I key)
- [ ] Can sell resources
- [ ] Money increases when selling
- [ ] Energy depletes when mining
- [ ] Energy regenerates when idle

## 🎮 Controls Reminder
- **WASD**: Move
- **Mouse**: Look
- **Left Click**: Mine
- **I**: Inventory
- **B**: Shop
- **Shift**: Sprint

---

**That's it! Start mining! ⛏️**
