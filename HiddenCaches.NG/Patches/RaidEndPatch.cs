using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace HiddenCaches.NG.Patches;

public class RaidEndPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.DeclaredMethod(typeof(Player), "OnDestroy");
    }

    [PatchPrefix]
    private static void Cleanup(Player instance)
    {
        if (Singleton<GameWorld>.Instance.MainPlayer.Id != instance.Id) return;
        
        BundleLoader.Material = null;
        BundleLoader.AudioClip = null;
        BundleLoader.ParticleSystem = null;

        CachePatch.HiddenCacheList = [];
    }
}
