using System;
using System.Threading.Tasks;
using Comfort.Common;
using UnityEngine;

namespace HiddenCaches.NG;

public static class BundleLoader
{
    internal static AudioClip? AudioClip;
    internal static Material? Material;
    internal static ParticleSystem? ParticleSystem;
    
    private const string Path = "assets/content/location_objects/lootable/prefab/scontainer_crate.bundle";

    public static async Task PopulateComponentsAsync()
    {
        var easyAssets = Singleton<PoolManagerClass>.Instance.EasyAssets;
        await easyAssets.Retain(Path, null, null).LoadingJob;
        
        try
        {
            var allComponents = easyAssets.GetAsset<GameObject>(Path)!.GetComponentsInChildren<Component>();

            foreach (var component in allComponents)
            {
                switch (component.name)
                {
                    case "Flare_Smoke" when component.GetType().Name == "ParticleSystemRenderer":
                    {
                        var renderer = component.GetComponent<ParticleSystemRenderer>();
                        Material = renderer.material;
                        continue;
                    }
                    case "Flare_Audio" when component.GetType().Name == "AudioSource":
                    {
                        var audioSource = component.GetComponent<AudioSource>();
                        AudioClip = audioSource.clip;
                        continue;
                    }
                    case "Flare_Smoke" when component.GetType().Name == "ParticleSystem":
                        ParticleSystem = component.GetComponent<ParticleSystem>();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError(ex);
        }
    }
}