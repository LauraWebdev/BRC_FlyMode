using System.Linq;
using BepInEx.Logging;
using HarmonyLib;

namespace moe.taw.BRC.FlyMode;
using BepInEx;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Bomb Rush Cyberfunk.exe")]
public class FlyMode : BaseUnityPlugin
{
    public static ManualLogSource Log = null!;
    public static Harmony Harmony = null!;
    
    internal static FlyMode Instance { get; private set; } = null;

    private void Awake()
    {
        // Setting up Harmony
        Harmony = new Harmony("moe.taw.BRC.FlyMode");

        var patches = typeof(FlyMode)
            .Assembly
            .GetTypes()
            .Where(x => x.GetCustomAttributes(typeof(HarmonyPatch), false).Length > 0)
            .ToArray();
        
        foreach (var patch in patches)
        {
            Harmony.PatchAll(patch);
        }

        // Create Instance
        Instance = this;
    }
}
