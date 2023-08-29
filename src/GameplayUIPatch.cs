using HarmonyLib;
using Reptile;

namespace moe.taw.BRC.FlyMode;

[HarmonyPatch(typeof(GameplayUI))]
public class GameplayUIPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("Init")]
    public static void Init(GameplayUI __instance) {
        __instance.gameplayScreen.gameObject.AddComponent<UI>();
    }
}