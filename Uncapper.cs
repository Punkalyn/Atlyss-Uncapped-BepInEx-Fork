using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using static MelonLoader.MelonLogger;

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
                    foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                    {
                        if (go.name == "_entity_partyObject")
                        {
                            partyPre = go.GetComponent<PartyObjectBehavior>();
                            //MelonLogger.Msg(go.name);
                            //MelonLogger.Msg(partyPre.Network_maxPartyLimit);
                            partyPre.Network_maxPartyLimit = 1000;
                            //MelonLogger.Msg(partyPre.Network_maxPartyLimit);
                            //MelonLogger.Msg(partyPre._maxPartyLimit);
                            partyPre._maxPartyLimit = 1000;
                            //MelonLogger.Msg(partyPre._maxPartyLimit);
                            break;
                        }
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
                    //MelonLogger.Msg(__instance.maxConnections);
                    __instance.maxConnections = 500;
                    //MelonLogger.Msg(__instance.maxConnections);
                }
                catch (Exception e)
                {
                    MelonLogger.Msg(e);
                }
            }
        }
    }
}
