# PvZ Replanted Mod

A mod project for Plants vs. Zombies: Replanted. So far it's a test mod that writes a line to MelonLoader's log when it loads and when a level starts. It doesn't change the game.

## Install

1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) 0.7.3 into the game folder, then start the game once so it can set itself up.
2. Download `PvZReplantedMod.dll` from the [Releases page](https://github.com/DontGoToCollege/PvZReplantedMod/releases).
3. Put it in the game's `Mods` folder.

Tested on game version 1.2.1173. Mods can break when the game updates. Stay away from online play while using them.

## For developers

[FINDINGS.md](FINDINGS.md) explains how the game is built and which parts of it a mod can change. `tools/make_dump.py` builds a searchable list of the game's classes and methods from a REA export of your own copy of the game.

This is a fan project, not made by or connected to EA or PopCap. It doesn't include any game files, and you need your own copy of the game.

## License

MIT: free for anyone to use, change and share. See [LICENSE](LICENSE).
