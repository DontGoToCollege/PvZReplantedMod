using HarmonyLib;
using Il2CppReloaded.Gameplay;
using MelonLoader;

[assembly: MelonInfo(typeof(PvZReplantedMod.Mod), "PvZReplantedMod", PvZReplantedMod.Mod.Version, "DontGoToCollege")]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]

namespace PvZReplantedMod
{
    public class Mod : MelonMod
    {
        public const string Version = "0.1.0";

        public override void OnInitializeMelon() => LoggerInstance.Msg("Loaded.");
    }

    // Log-only proof that patches reach gameplay code; changes nothing in the game.
    [HarmonyPatch(typeof(Board), nameof(Board.StartLevel))]
    static class BoardStartLevelPatch
    {
        static void Postfix(Board __instance) => Melon<Mod>.Logger.Msg($"Level started: mLevel={__instance.mLevel}");
    }
}
