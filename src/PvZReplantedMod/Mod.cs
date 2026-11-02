using HarmonyLib;
using Il2CppSource.TreeStateActivities;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(PvZReplantedMod.Mod), "PvZReplantedMod", PvZReplantedMod.Mod.Version, "DontGoToCollege")]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]

namespace PvZReplantedMod
{
    public class Mod : MelonMod
    {
        public const string Version = "0.2.0";

        public override void OnInitializeMelon()
        {
            // Unity stops the whole game loop on focus loss unless this is on.
            Application.runInBackground = true;
            LoggerInstance.Msg($"Keep running in background: {Application.runInBackground}");
        }
    }

    // The game also leaves gameplay for another state when focus is lost.
    [HarmonyPatch(typeof(TransitionWhenFocusLostActivity), nameof(TransitionWhenFocusLostActivity.OnPlatformFocusChanged))]
    static class FocusLostPatch
    {
        static bool Prefix(bool hasFocus)
        {
            Melon<Mod>.Logger.Msg($"Focus {(hasFocus ? "back" : "lost")} at frame {Time.frameCount}");
            return hasFocus;
        }
    }
}
