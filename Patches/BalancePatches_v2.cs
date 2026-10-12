using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using WesleyMoonScripts;

// BALANCING PATCHES

namespace WesleysMoonsHQModule.Patches;

/// <summary>
/// Balancing patches for v81
/// </summary>
[HarmonyPatch(typeof(StartOfRound))]
internal class BalancePatches_v2
{
    internal static bool Patched = false;
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    internal static void Start_Postfix(StartOfRound __instance)
    {
        if (Patched) return; // Only patch once.

        if (WesleyScripts.LockMoons.Value) // Only apply when moons are locked - HQ Mode
        {
            LungProp[] lungProps = Resources.FindObjectsOfTypeAll<LungProp>();

            foreach (LungProp lp in lungProps)
            {
                if (lp.name == "CosmicLungApparatus" || lp.name == "CosmicLungApparatusDisabled")
                {
                    lp.scrapValue = 214; // Change Cosmocos outside apparatus value
                }
            }

            List<Item> items = StartOfRound.Instance.allItemsList.itemsList;

            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                // -- HYX -- item value rebalance
                switch (item.itemName)
                {
                    case "Assimilated maneater":
                    case "Assimilated bracken arm":
                    case "Assimilated jester skull":
                    case "Assimilated spore lizard leg":
                    case "Assimilated hoarding bug head":
                        item.maxValue = 220;
                        item.minValue = 160;
                        break;

                    case "Assimilated concrete torso":
                        item.maxValue = 360;
                        item.minValue = 300;
                        break;

                    case "The seed of assimilation":
                        item.maxValue = 580;
                        item.minValue = 500;
                        break;
                }
            }

            TerminalKeyword[] keywords = Resources.FindObjectsOfTypeAll<TerminalKeyword>();

            for (int i = 0; i < keywords.Length; i++)
            {
                if (keywords[i].name == "Route")
                {
                    CompatibleNoun[] nouns = keywords[i].compatibleNouns;
                    for (int j = 0; j < nouns.Length; j++)
                    {
                        switch (nouns[j].result.name)
                        {
                            case "XenphirRoute":
                                nouns[j].result.itemCost = 2500;
                                break;
                            case "HyxRoute":
                                nouns[j].result.itemCost = 4000;
                                break;
                            case "CarnateRoute":
                                nouns[j].result.itemCost = 2200;
                                break;
                        }
                    }
                    break;
                }
            }

        }

        foreach (SelectableLevel s in __instance.levels)
        {
            s.DaySpeedMultiplier = Mathf.Max(s.DaySpeedMultiplier, 1f); // Make default daytime speed the minimum

            // -- COSMOCOS --
            if (s.name == "CosmocosLevel")
            {
                s.DaySpeedMultiplier = 0.959f; // Re-ajust daytime speed - Landing cutscene
            }

            // -- EMPRA --
            else if (s.name == "EmpraLevel")
            {
                s.DaySpeedMultiplier = 0.875f; // Re-adjust daytime speed - Cart ride

                if (!WesleyScripts.LockMoons.Value) continue; // patch only in HQ Mode

                s.maxEnemyPowerCount = 11;
            }

            // -- HYX --
            else if (s.name == "HyxLevel")
            {
                if (!WesleyScripts.LockMoons.Value) continue; // patch only in HQ Mode

                s.maxScrap = 54;
                s.minScrap = 42;
                s.maxOutsideEnemyPowerCount = 14;
                s.maxEnemyPowerCount = 20;
                s.spawnProbabilityRange = 3.04f;
                s.enemySpawnChanceThroughoutDay = new AnimationCurve([
                    new Keyframe(0.0f, -3.0f, 2f, 2f),
                    new Keyframe(0.2f, -1.0f, 15f, 15f),
                    new Keyframe(0.4f, 6.0f, 0f, 0f),
                    new Keyframe(0.6f, 4.0f, 4f, 4f),
                    new Keyframe(0.8f, 9.0f, 15f, 15f),
                    new Keyframe(1.0f, 50.0f, 120f, 120f)
                    ]);
                s.outsideEnemySpawnChanceThroughDay = AddToKeyFramesOverTime(s.outsideEnemySpawnChanceThroughDay, 2f);
                s.indoorMapHazards[0].numberToSpawn = AddToKeyFramesOverTime(s.indoorMapHazards[0].numberToSpawn, 20f); // landmines
                s.indoorMapHazards[1].numberToSpawn = AddToKeyFramesOverTime(s.indoorMapHazards[1].numberToSpawn, 12f); // turrets
                s.indoorMapHazards[2].numberToSpawn = AddToKeyFramesOverTime(s.indoorMapHazards[2].numberToSpawn, 6f); // spiketraps
                s.randomWeathers[1].weatherVariable = 4; // Eclipsed
            }

            else if (s.name == "CarnateLevel")
            {
                if (!WesleyScripts.LockMoons.Value) continue; // patch only in HQ mode

                s.maxScrap = 50;
                s.minScrap = 47;
                s.factorySizeMultiplier = 1.7f;
            }

            else if (s.name == "XenphirLevel")
            {
                if (!WesleyScripts.LockMoons.Value) continue; // patch only in HQ Mode

                s.maxScrap = 56;
                s.minScrap = 48;
                s.factorySizeMultiplier = 1.7f;
            }
        }

        Patched = true;
    }

    internal static AnimationCurve AddToKeyFramesOverTime(AnimationCurve curve, float value)
    {
        Keyframe[] keyFrames = curve.keys;
        for (int i = 0; i < keyFrames.Length; i++)
        {
            if (keyFrames[i].value > 0)
            {
                keyFrames[i].value += keyFrames[i].time * value;
            }
        }
        return new AnimationCurve(keyFrames);
    }
}
