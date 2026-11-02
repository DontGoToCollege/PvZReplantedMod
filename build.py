"""Builds the mod, keeps a copy in Versions/Archive and installs it into the game's Mods folder.

Refuses to rebuild a version that's already archived: raise Version in Mod.cs first.
"""
import os
import re
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.abspath(__file__))
SOURCE = os.path.join(ROOT, "src", "PvZReplantedMod")


def main():
    with open(os.path.join(SOURCE, "Mod.cs"), encoding="utf-8") as f:
        version = re.search(r'Version = "([^"]+)"', f.read()).group(1)
    archived = os.path.join(ROOT, "Versions", "Archive", f"PvZReplantedMod-{version}.dll")
    if os.path.exists(archived):
        sys.exit(f"{version} is already archived. Raise Version in Mod.cs first.")

    with open(os.path.join(ROOT, "local.props"), encoding="utf-8") as f:
        game_dir = re.search(r"<GameDir>(.+?)</GameDir>", f.read()).group(1)

    subprocess.run(["dotnet", "build", SOURCE, "-c", "Release", "-v", "q", "--nologo"], check=True)
    built = os.path.join(SOURCE, "bin", "Release", "net6.0", "PvZReplantedMod.dll")

    os.makedirs(os.path.dirname(archived), exist_ok=True)
    shutil.copy2(built, archived)
    shutil.copy2(built, os.path.join(ROOT, "PvZReplantedMod.dll"))
    os.makedirs(os.path.join(game_dir, "Mods"), exist_ok=True)
    shutil.copy2(built, os.path.join(game_dir, "Mods", "PvZReplantedMod.dll"))
    print(f"Built and installed {version}")


if __name__ == "__main__":
    main()
