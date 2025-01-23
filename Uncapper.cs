using System;
using System.IO;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using BepInEx.Logging;


namespace Uncapped_Players_And_Parties
{
    [BepInPlugin("lilly.uncappedplayers", "Lilly's Uncapped Players", "1.0.0")]

    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public void Awake()
        {
            Logger = base.Logger;
            Debug.Log("Uncapped Players and Parties Plugin Initialized.");
            var harmony = new Harmony("lilly.uncappedplayers");
            try
            {
                harmony.PatchAll();
            }
            catch (Exception value)
            {
                Console.WriteLine(value);
                throw;
            }
        }

        [HarmonyPatch(typeof(PartyObjectBehavior), "Update")]
        public static class PatchPartyObjectBehavior
        {
            public static void Postfix(PartyObjectBehavior __instance)
            {
                if (__instance.Network_maxPartyLimit < 1000)
                {
                    __instance.Network_maxPartyLimit = 1000;
                    __instance._maxPartyLimit = 1000;
                    Debug.Log($"Patched PartyObjectBehavior: max limit set to 1000.");
                }
            }
        }

        [HarmonyPatch(typeof(LobbyListManager), "Update")]
        public static class PatchLobbySlider
        {
            public static void Postfix(LobbyListManager __instance)
            {
                if (__instance._lobbyMaxConnectionSlider == null)
                {
                    Debug.LogError("_lobbyMaxConnectionSlider is null. Ensure the UI is loaded.");
                    return;
                }
                else if (__instance._lobbyMaxConnectionSlider.maxValue < 250)
                {
                    __instance._lobbyMaxConnectionSlider.maxValue = 250f;
                    __instance._lobbyMaxConnectionSlider.minValue = 2f;
                    Debug.Log("Slider values patched successfully.");
                }
            }
        }

        [HarmonyPatch(typeof(ProfileDataManager), "Load_HostSettingsData")]
        public static class PatchHostSettingsData
        {
            public static bool Prefix(ref string ____dataPath)
            {
                Debug.Log($"Patching HostSettingsData Load from {____dataPath}");
                Load_HostSettingsData(____dataPath);
                return false;
            }

            private static void Load_HostSettingsData(string dataPath)
            {
                try
                {
                    string filePath = Path.Combine(dataPath, "hostSettings.json");
                    if (File.Exists(filePath))
                    {
                        string json = File.ReadAllText(filePath);
                        var profile = JsonUtility.FromJson<ServerHostSettings_Profile>(json);

                        if (string.IsNullOrEmpty(profile._serverName) || profile._serverName.Contains("<") || profile._serverName.Contains(">"))
                        {
                            profile._serverName = "Atlyss Server";
                        }

                        profile._maxAllowedConnections = Mathf.Clamp(profile._maxAllowedConnections, 2, 250);

                        ProfileDataManager._current._hostSettingsProfile = profile;
                        MainMenuManager._current.Load_HostSettings();
                        AtlyssNetworkManager._current._serverName = profile._serverName;
                        AtlyssNetworkManager._current._serverPassword = profile._serverPassword;
                        AtlyssNetworkManager._current._serverMotd = profile._serverMotd;
                        AtlyssNetworkManager._current.maxConnections = profile._maxAllowedConnections;

                        Debug.Log("Host settings successfully loaded and patched.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading host settings: {ex.Message}");
                }
            }
        }
    }
}