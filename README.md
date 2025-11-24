# Board Defense – Case Study (Unity 6.2, URP, C#)


## Tech Stack
- **Engine:** Unity **6.2**
- **Render Pipeline:** **URP**
- **Language:** **C#**

## How to Play
1. Open the **Bootstrap** scene (main entry).
2. **Play** ▶️ in the Editor.
3. In the main menu, **select a level**.
4. Use the bottom **Inventory** to **drag & drop** towers onto valid grid cells (bottom half). Survive all waves.

---

## Folder Structure
```
Scripts/
  Core/                 # Engine‑agnostic utilities & cross‑cutting systems
    Interfaces/         # Small contracts (IDamageable, IMovable, ITargetSelector, …)
    Pooling/            # PoolManager (singleton), IPoolable
    Signals/            # SignalBus, GameSignals
    Targeting/          # Target selectors (Closest, FirstInColumn, …)
  Gameplay/             # High‑level game systems (data + logic)
    Base/               # BaseHealthSystem (win/lose condition input)
    Board/              # BoardManager, GridCoords (grid authority & helpers)
    Data/               # DefenseItemType, EnemyType, enums
    Enemy/              # EnemyController, EnemyHealth, Registry, Pathing providers
    Goal/               # GoalLine (breach → base damage + cleanup)
    Inventory/          # TowerInventory (per‑level allowances)
    LevelFlow/          # LevelManager, LevelWinLoseManager
    Levels/             # LevelConfig, WaveConfig, DefenseAllowance, EnemySpawnEntry
    Placement/          # TowerPlacementControllerV2, PlacedEntity
    Projectile/         # ProjectileController, PooledProjectileLifetime
    Spawning/           # SpawnManager (grid‑aware top‑row spawns)
    Tower/              # TowerController (fires via ITargetSelector)
  Content/              # Prefab catalogs (EnemyPrefabCatalog, DefensePrefabCatalog)
  UI/                   # HUDs (BaseHealthHUD, WaveHUD, ResultBanner), drag UI
  Util/                 # GhostMaterialFactory (editor/runtime helpers)
```
> Scenes, prefabs, and ScriptableObjects live in their own folders; catalogs bridge data→prefab references.

---

## Architecture – Key Classes & Where They Fit
- **BoardManager** (`Gameplay/Board`) — **Grid authority**. Converts **grid↔world**, generates cells from a prefab, exposes tile sizing. All spatial rules (rows/columns, bottom‑half validation via helpers) rely on this.
- **SpawnManager** (`Gameplay/Spawning`) — **Wave runner.** Spawns enemies at the **top row** in random columns per `LevelConfig/WaveConfig`, signals wave events (start/finish), and ends with “all waves completed.”
- **EnemyController + Pathing** (`Gameplay/Enemy`) — Column‑locked movement using `IPathProvider` (default: `StraightDownPathProvider`). `EnemyHealth` implements `IDamageable`; `EnemyRegistry` tracks alive counts.
- **GoalLine** (`Gameplay/Goal`) — Trigger at the bottom edge: applies **base damage** and **releases** enemies back to pool on breach.
- **TowerController** (`Gameplay/Tower`) — Fires at interval (seconds) and range (**blocks**), asks an `ITargetSelector` (e.g., `ClosestSelector`, `FirstInColumnSelector`) for the best target (Strategy Pattern), spawns **pooled** projectiles.
- **ProjectileController** (`Gameplay/Projectile`) — Lightweight forward motion + **distance check** hit; calls `IDamageable.TakeDamage(float)`; returns to pool on hit/lifetime.
- **TowerPlacementController & TowerPaletteDragUI** (`Gameplay/Placement`, `UI`) — Mobile‑friendly drag‑drop flow using `BoardManager` snapping + **tower inventory** gating (via `TowerInventory` allowances per level).
- **LevelManager / LevelWinLoseManager** (`Gameplay/LevelFlow`) — Orchestrates level lifecycle and resolves **win/lose** from waves + base health + enemy registry.
- **SignalBus + GameSignals** (`Core/Signals`) — Minimal, **type‑safe event hub** for decoupled UI↔gameplay communication (no UnityEvents in hot paths).
- **PoolManager** (`Core/Pooling`) — **Singleton** pooling with **lazy growth + hard cap + warnings**. All transient objects (enemies, projectiles) are pooled; `IPoolable` lets objects react to spawn/despawn.

---

## Design Patterns & Practices
- **Composition over inheritance** (Unity‑friendly): Small, focused MonoBehaviours (e.g., `EnemyHealth`, `EnemyController`, `TowerController`) compose behavior without deep trees.
- **Interfaces as seams** (`IDamageable`, `ITargetSelector`, `IMovable`, `IPathProvider`): Enables interchangeable implementations (selectors, pathing) and easy unit testing/mocking.
- **Centralized authority** (BoardManager): One source of truth for grid metrics & conversions prevents drift between systems and keeps math consistent.
- **Event bus (SignalBus)**: Decouples systems and UI without heavy dependencies. Type‑safe C# delegates keep it lean in hot paths.
- **Object Pooling** (PoolManager singleton): Eliminates allocation spikes for better stability.
- **Data‑driven content** (`LevelConfig`, `WaveConfig`, `DefenseItemType`, `EnemyType`): Designers change values in SOs; code stays generic. Prefab catalogs separate **data** from **presentation**.
- **Zero‑alloc targeting**: Selectors use physics non‑alloc APIs and squared distance checks to minimize GC.
- **Separation of gameplay vs. presentation**: Projectiles apply damage via `IDamageable`, towers query selectors — neither depends on specific enemy components.

---

## Future Improvements
- **Content polish / art:** Deliberately **no art assets** to keep this case study lightweight and focused on **architecture + mobile performance**. Visual polish (models, VFX, SFX, UI skin) can layer on without touching gameplay code.
- **Tower modifiers / upgrades:** With a larger scope, introduce a **Decorator** (or **Buff**/**Aura**) system to dynamically compose effects (range/damage/status, etc.) at runtime, plus a proper upgrade graph.
- **Richer pathing:** If the grid grows and includes obstacles/lanes, swap `StraightDownPathProvider` with **A\*** or flow‑field pathing; `IPathProvider` already isolates that decision.
- **Save/Progression:** Per‑level stars, difficulty tiers, loadouts, and a meta‑progress loop.
- **UX:** Haptics, color palettes, better touch affordances, and onboarding tips.

---
## AI NOTICE
This README file is generated by AI using disorganized notes and comments I take during development.
AI assistance was used to create tables, tags, titles and general structure of this documentation. 