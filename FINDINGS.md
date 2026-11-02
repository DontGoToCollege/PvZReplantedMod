# What we know about the game

Studied: Plants vs. Zombies: Replanted, game version 1.2.1173, using REA on the game's files. The game was not run, so nothing here has been seen working live yet.

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

## Inferred, not proven

- The code looks like a port of the original game's C++: the names match its style (`mBoard`, `mSunMoney`, `ZombieInitialize`). Knowledge of the original game should mostly carry over.
- `PlantDefinition` and friends are probably stored in the game's asset bundles and loaded by `DataService`, so changing them after load should change the game's numbers.
- `ZombieDefinition` only has health values for Versus, so normal zombie health is probably set in code inside `ZombieInitialize`.
- Enum numbers (for example `SeedType.Peashooter` being 0) are assumed from their order in the dump. REA's export doesn't include the real values.

## Not known yet

- Field offsets. The dump has none, but a mod loader's generated assemblies handle them.
- What any method's code does. Only names and signatures were read, nothing was decompiled.
- Whether MelonLoader runs on this exact copy of the game.

## How to build mods

Use **MelonLoader**. Published Replanted mods are built on it, and a [BepInEx plugin template](https://www.nuget.org/packages/Replanted.PluginTemplate) for the game itself recommends it. [RaptorRush135.PvZR.Sdk](https://www.nuget.org/packages/RaptorRush135.PvZR.Sdk) is a ready-made build setup for it, and [More Yetis](https://github.com/kedzie/MoreYetis) is a working example. A mod would be a small C# DLL that patches the methods above with Harmony.

## Regenerating the dump

`dump/` is the game's own names and is not in git. To rebuild it, run `inspect_il2cpp_metadata` in REA with `binary_path` set to `GameAssembly.dll`, export the evidence bundle, then:

```bash
python tools/make_dump.py il2cpp-bundle.json dump
```

Method addresses in the dump only match this game version.
