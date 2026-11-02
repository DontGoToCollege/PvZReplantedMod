using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppReloaded.Binders;
using Il2CppSource.TreeStateActivities;
using Il2CppTMPro;
using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(PvZReplantedMod.Mod), "PvZReplantedMod", PvZReplantedMod.Mod.Version, "DontGoToCollege")]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]

namespace PvZReplantedMod
{
    public class Mod : MelonMod
    {
        public const string Version = "0.3.0";

        internal static MelonPreferences_Entry<bool> KeepRunning;
        internal static MelonPreferences_Entry<bool> BackgroundAudio;

        float? m_savedVolume;

        public override void OnInitializeMelon()
        {
            var settings = MelonPreferences.CreateCategory("PvZReplantedMod");
            KeepRunning = settings.CreateEntry("KeepRunning", true);
            BackgroundAudio = settings.CreateEntry("BackgroundAudio", true);
            LoggerInstance.Msg($"KeepRunning={KeepRunning.Value} BackgroundAudio={BackgroundAudio.Value}");
        }

        public override void OnUpdate()
        {
            if (Application.runInBackground != KeepRunning.Value)
                Application.runInBackground = KeepRunning.Value;

            bool mute = !BackgroundAudio.Value && !Application.isFocused;
            if (mute && m_savedVolume == null)
            {
                m_savedVolume = AudioListener.volume;
                AudioListener.volume = 0;
            }
            else if (!mute && m_savedVolume != null)
            {
                AudioListener.volume = m_savedVolume.Value;
                m_savedVolume = null;
            }
        }
    }

    // The game also leaves gameplay for another state when focus is lost.
    [HarmonyPatch(typeof(TransitionWhenFocusLostActivity), nameof(TransitionWhenFocusLostActivity.OnPlatformFocusChanged))]
    static class FocusLostPatch
    {
        static bool Prefix(bool hasFocus)
        {
            Melon<Mod>.Logger.Msg($"Focus {(hasFocus ? "back" : "lost")} at frame {Time.frameCount}");
            return hasFocus || !Mod.KeepRunning.Value;
        }
    }

    // Both options screens have a V-Sync row, and it binds each time the screen opens.
    [HarmonyPatch(typeof(UnityToggleBinder), nameof(UnityToggleBinder.Bind))]
    static class OptionsMenuPatch
    {
        static void Postfix(UnityToggleBinder __instance)
        {
            if (__instance.m_key?.Path != "settings.vsyncToggle") return;

            var vsyncRow = __instance.transform.parent;
            AddOrRefreshRow(vsyncRow, "Background Audio", Mod.BackgroundAudio);
            AddOrRefreshRow(vsyncRow, "Run in Background", Mod.KeepRunning);
        }

        static void AddOrRefreshRow(Transform vsyncRow, string label, MelonPreferences_Entry<bool> setting)
        {
            var existing = vsyncRow.parent.Find(label);
            if (existing != null)
            {
                existing.GetComponentInChildren<Toggle>(true).SetIsOnWithoutNotify(setting.Value);
                return;
            }

            // Cloning under an inactive parent stops the copy's binders starting and taking over the V-Sync setting.
            var holder = new GameObject();
            holder.SetActive(false);
            var row = UnityEngine.Object.Instantiate(vsyncRow.gameObject, holder.transform);
            row.name = label;

            foreach (var component in row.GetComponentsInChildren<Component>(true))
            {
                var type = component.GetIl2CppType().Name;
                if (type == "UnityToggleBinder" || type == "BinderContainer" || type == "TextLocalizer")
                    UnityEngine.Object.DestroyImmediate(component);
            }

            row.GetComponentInChildren<TMP_Text>(true).text = label;

            var toggle = row.GetComponentInChildren<Toggle>(true);
            toggle.onValueChanged.RemoveAllListeners();
            toggle.SetIsOnWithoutNotify(setting.Value);
            toggle.onValueChanged.AddListener(DelegateSupport.ConvertDelegate<UnityAction<bool>>(new Action<bool>(value =>
            {
                setting.Value = value;
                MelonPreferences.Save();
            })));

            row.transform.SetParent(vsyncRow.parent, false);
            row.transform.SetSiblingIndex(vsyncRow.GetSiblingIndex() + 1);
            UnityEngine.Object.Destroy(holder);
        }
    }
}
