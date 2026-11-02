# What we know about the game

Studied: Plants vs. Zombies: Replanted, game version 1.2.1173, using REA on the game's files. The hook points below were read from the files. `Board.StartLevel` is confirmed working in the running game (the log showed `Level started: mLevel=1`); the others haven't been tried yet.

## Observed in the files

- **Engine:** Unity 6000.0.52f1, built with IL2CPP, 64-bit. The game's C# was converted to native code in `GameAssembly.dll` (83 MB).
- **Names survive:** `global-metadata.dat` is version 31 and not encrypted, so all 18,169 classes and 145,510 methods keep their real names. 127,554 of those methods have code addresses in the DLL.
- **Game code:** lives in `Assembly-CSharp` (999 classes), mostly under `Reloaded.Gameplay`, `Reloaded.Services`, `Reloaded.Data` and `Source.Controllers`.
- **No anti-tamper classes:** a name search found no anti-cheat or integrity-check classes in the game's code. That is a name search only, so it doesn't prove there are none.
- **Crash reporting:** Sentry is built in, so crashes may be reported to the developers. Keep this in mind when a mod crashes the game.
- **Online code:** EA account code (`Eadp.Nexus`, 116 classes) and gRPC are present. Mods should stay away from anything online.

## Where to hook

| To change | Look at |
|---|---|
| Sun | `Board.AddSunMoney`, `TakeSunMoney`, `CanTakeSunMoney`; the value is `Board.mSunMoney` |
| Plant cost and recharge | `Plant.GetCost`, `Plant.GetRefreshTime`; base values in `PlantDefinition.m_seedCost`, `m_refreshTime` |
| Plant behaviour | `Plant.Update`, `UpdateShooter`, `Fire`, `DoSpecial`, plus one `Update...` method per plant type |
| Plant health | `Plant.mPlantHealth`, `mPlantMaxHealth`, set around `PlantInitialize` |
| Placing plants | `Board.AddPlant`, `Board.CanPlantAt` |
| Zombie stats | `Zombie.mBodyHealth`, `mHelmHealth`, `mShieldHealth`, `mFlyingHealth`; `ZombieInitialize` |
| Damage and death | `Zombie.TakeDamage`, `DieWithLoot`, `DieNoLoot`, `EatPlant` |
| Status effects | `Zombie.ApplyChill`, `ApplyBurn`, `ApplyButter` |
| Waves and spawning | `Board.SpawnZombieWave`, `PickZombieType`, `AddZombie`, `AddZombieAtCell`, `mZombiesInWave` |
| Projectile damage | `ProjectileDefinition.m_damage`, `Projectile.DoImpact`, `DoSplashDamage` |
| All game data | `IDataService`: `PlantDefinitions`, `ZombieDefinitions`, `ProjectileDefinitions`, `AllLevelsData`, `GetPlantDefinition`, `GetZombieDefinition` |
| Game speed | `GameplaySpeed` enum (`_Half`, `_Normal`, `_2X`, `_4x`, `_8X`) |

## What stops the game on alt-tab

- `Application.runInBackground` is off by default in this game. Turning it on keeps the game loop running: with another app in front, the game still used about 1.6 CPU cores.
- `TransitionWhenFocusLostActivity.OnPlatformFocusChanged` is the game's own focus-lost handler. The mod blocks it when focus is lost, but it hasn't been seen firing yet, so it may not matter. Its log line (`Focus lost at frame ...`) will show if it ever does.

## Inferred, not proven

- The code looks like a port of the original game's C++: the names match its style (`mBoard`, `mSunMoney`, `ZombieInitialize`). Knowledge of the original game should mostly carry over.
- `PlantDefinition` and friends are probably stored in the game's asset bundles and loaded by `DataService`, so changing them after load should change the game's numbers.
- `ZombieDefinition` only has health values for Versus, so normal zombie health is probably set in code inside `ZombieInitialize`.
- Enum numbers (for example `SeedType.Peashooter` being 0) are assumed from their order in the dump. REA's export doesn't include the real values.

## Not known yet

- Field offsets. The dump has none, but a mod loader's generated assemblies handle them.
- What any method's code does. Only names and signatures were read, nothing was decompiled.

## How to build mods

Use **MelonLoader**. Version 0.7.3 runs on this game: it generated its assemblies in about 40 seconds on first launch and loaded a test mod. Published Replanted mods are built on it too, and a [BepInEx plugin template](https://www.nuget.org/packages/Replanted.PluginTemplate) for the game itself recommends it. [More Yetis](https://github.com/kedzie/MoreYetis) is a working example.

A mod is a small C# DLL that patches the methods above with Harmony. Generated classes get an `Il2Cpp` prefix, so `Reloaded.Gameplay.Board` becomes `Il2CppReloaded.Gameplay.Board`.

To build, add a `local.props` file holding your game folder as `<GameDir>` (it stays out of git), then run `python build.py`. Settings and the game's own logs are in `AppData\LocalLow\PopCap Games\PvZ Replanted`. A progress save hasn't appeared there yet, so its location is still unconfirmed.

## Regenerating the dump

`dump/` is the game's own names and is not in git. To rebuild it, run `inspect_il2cpp_metadata` in REA with `binary_path` set to `GameAssembly.dll`, export the evidence bundle, then:

```bash
python tools/make_dump.py il2cpp-bundle.json dump
```

Method addresses in the dump only match this game version.
