using HarmonyLib;
using UnityEngine;
using WesleyMoonScripts;

// BALANCING PATCHES

namespace WesleysMoonsHQModule.Patches;

/// <summary>
/// Balancing patches pre v81
/// </summary>
[HarmonyPatch(typeof(StartOfRound))]
internal class BalancePatches_v1
{
    internal static bool Patched = false;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    internal static void Start_Postfix(StartOfRound __instance)
    {
        if (Patched) return; // Only patch once.

        if (WesleyScripts.LockMoons.Value) // Only apply when moons are locked
        {
            LungProp[] lungProps = Resources.FindObjectsOfTypeAll<LungProp>();

            foreach (LungProp lp in lungProps)
            {
                if (lp.name == "CosmicLungApparatus" || lp.name == "CosmicLungApparatusDisabled")
                {
                    lp.scrapValue = 214; // Change Cosmocos outside apparatus value
                }
            }
        }

        foreach (SelectableLevel s in __instance.levels)
        {
            s.DaySpeedMultiplier = Mathf.Max(s.DaySpeedMultiplier, 1f); // Make default daytime speed the minimum.

            if (s.name == "CosmocosLevel")
            {
                s.DaySpeedMultiplier = 0.959f; // Re-ajust daytime speed - Landing cutscene

                if (WesleyScripts.LockMoons.Value)
                {
                    s.maxScrap = 55; // Change max scrap on Cosmocos, only apply when moons are locked
                }
            }

            else if (s.name == "EmpraLevel")
            {
                s.DaySpeedMultiplier = 0.875f; // Re-adjust daytime speed - Cart ride
            }
        }

        Patched = true;
    }
}
