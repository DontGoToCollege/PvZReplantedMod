# PvZ Replanted Mod

A mod for Plants vs. Zombies: Replanted that keeps the game running when you alt-tab. Normally the game pauses as soon as you click away.

It adds two checkboxes to the game's Options menu, under V-Sync:

- **Run in Background:** keeps the game playing while you're alt-tabbed.
- **Background Audio:** keeps the sound on while you're alt-tabbed. Uncheck it to keep the game running silently.

Both are on by default and are remembered between launches. They work with a mouse, and may not be reachable with a gamepad.

Alt-tab may still minimize the game in exclusive fullscreen, so windowed or borderless is safest.

## Install

1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) 0.7.3 into the game folder, then start the game once so it can set itself up.
2. Download `PvZReplantedMod.dll` from the [Releases page](https://github.com/DontGoToCollege/PvZReplantedMod/releases).
3. Put it in the game's `Mods` folder.
4. Optional: set `hide_console = true` in `UserData\Loader.cfg`. Clicking inside MelonLoader's console window can freeze the game.

Tested on game version 1.2.1173. Mods can break when the game updates. Stay away from online play while using them.

## For developers

[FINDINGS.md](FINDINGS.md) explains how the game is built and which parts of it a mod can change. `tools/make_dump.py` builds a searchable list of the game's classes and methods from a REA export of your own copy of the game.

This is a fan project, not made by or connected to EA or PopCap. It doesn't include any game files, and you need your own copy of the game.

## License

MIT: free for anyone to use, change and share. See [LICENSE](LICENSE).
