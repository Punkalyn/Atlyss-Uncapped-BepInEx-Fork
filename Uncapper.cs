using System;
using HarmonyLib;
using MelonLoader;
using Mirror;
using UnityEngine;

namespace Uncapped_Players_And_Parties
{
    public class Uncapper : MelonMod
    {
        PartyObjectBehavior partyPre = null;
        public override void OnLateInitializeMelon()
        {
            uncapParties();
        }

        void uncapParties()
        {
            try
            {
                if (partyPre == null)
                {
                    foreach (PartyObjectBehavior go in Resources.FindObjectsOfTypeAll(typeof(PartyObjectBehavior)) as PartyObjectBehavior[])
                    {
                        go.Network_maxPartyLimit = 1000;
                        go._maxPartyLimit = 1000;
                    }
                }
            }
            catch (Exception e)
            {
                MelonLogger.Msg(e);
            }
        }

        [HarmonyPatch(typeof(AtlyssNetworkManager), "OnStartServer")]
        public static class uncapPlayers
        {
            private static void Prefix(ref AtlyssNetworkManager __instance)
            {
                try
                {
                    __instance.maxConnections = 1000;
                    NetworkServer.maxConnections = 1000;
                }
                catch (Exception e)
                {
                    MelonLogger.Msg(e);
                }
            }
        }

        /*[HarmonyPatch(typeof(NetworkServer), "Listen")]
        public static class uncapPlayers2
        {
            private static void Prefix(ref int maxConns)
            {
                try
                {
                    maxConns = 500;
                }
                catch (Exception obj)
                {
                    MelonLogger.Msg(obj);
                }
            }
        }*/
    }
}
