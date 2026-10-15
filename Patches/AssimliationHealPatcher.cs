using HarmonyLib;
using WesleysInteriorTools.Items;

namespace WesleysMoonsHQModule.Patches;

[HarmonyPatch(typeof(StimItem))]
internal class AssimilationHealPatcher
{
    [HarmonyPatch(nameof(StimItem.UseStim))]
    [HarmonyPostfix]
    internal static void UseStim_Post(StimItem __instance)
    {
        AssimilatedGrowth superCavader = UnityEngine.Object.FindAnyObjectByType<AssimilatedGrowth>();
        if (!__instance.HealthStim || superCavader == null)
            return;

        superCavader.HealInfection((int)GameNetworkManager.Instance.localPlayerController.playerClientId, 0.6f);
    }
}
