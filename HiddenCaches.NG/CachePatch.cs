using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using EFT.Interactive;
using SPT.Reflection.Patching;

namespace HiddenCaches.NG;

public class CachePatch : ModulePatch
{
    internal static IEnumerable<LootableContainer> HiddenCacheList = null!;
    
    private const string BarrelCacheTemplateId = "5d6d2bb386f774785b07a77a";
    private const string GroundCacheTemplateId = "5d6d2b5486f774785c2ba8ea";

    protected override MethodBase GetTargetMethod()
    {
        return typeof(EFT.GameWorld).GetMethod("OnGameStarted", BindingFlags.Public | BindingFlags.Instance)!;
    }
        
    [PatchPostfix]
    private static void AddComponentToCaches()
    {
        // clear old cache if it exists
        HiddenCacheList = [];

        HiddenCacheList = Object.FindObjectsOfType<LootableContainer>()
            .Where(container => container.Template is BarrelCacheTemplateId or GroundCacheTemplateId)
            .ToList();
            
        foreach (var lootableContainer in HiddenCacheList)
        {
            lootableContainer.GetOrAddComponent<FlareComponent>();
        }
    }
}
