using System;
using BepInEx;
using System.Collections;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using RaiRai.HiddenCaches.Patches;

namespace HiddenCaches.NG;

[BepInPlugin("com.dragonx86.hiddencaches-ng", "Hidden Caches NG", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    private const bool DefaultEnabled = true;
    
    internal static ManualLogSource Log = null!;

    internal static ConfigEntry<Color> ConfigColor = null!;
    internal static ConfigEntry<bool> ConfigAudio = null!;
    internal static ConfigEntry<bool> ConfigLight = null!;
    internal static ConfigEntry<bool> ConfigSmoke = null!;
    
    private static readonly int TintColor = Shader.PropertyToID("_TintColor");

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("Loading RaiRai.HiddenCaches plugin!");
        
        try
        {
            InitConfig();

            new CachePatch().Enable();
            new RaidEndPatch().Enable();
        }
        catch (Exception ex)
        {
            Log.LogError(ex.ToString());
        }
        
        Log.LogInfo("Loaded RaiRai.HiddenCaches plugin!");
    }

    private void InitConfig()
    {
        ConfigAudio = Config.Bind("", "Audio", enabled, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 4 }));
        ConfigLight = Config.Bind("", "Light", enabled, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 3 }));
        ConfigSmoke = Config.Bind("", "Smoke", enabled, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2 }));
        ConfigColor = Config.Bind("Color", "", new Color(1.0f, 0.75f / 2, 0.0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            
        Config.Bind("Color", "Apply", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 0, HideDefaultButton = true, CustomDrawer = new Action<ConfigEntryBase>(ApplyDrawer) }));
    }
    
    private void ApplyDrawer(ConfigEntryBase configEntry)
    {
        if (!GUILayout.Button("Apply", GUILayout.ExpandWidth(true))) return;
        
        var chosenColor = ConfigColor.Value * 2f;
        StartCoroutine(UpdateColors(chosenColor));
    }
    
    private static IEnumerator UpdateColors(Color chosenColor)
    {
        foreach (var container in CachePatch.HiddenCacheList)
        {
            // Audio
            if (container.TryGetComponent<AudioSource>(out var audioSource))
            {
                var shouldEnableAudio = ConfigAudio.Value;
                audioSource.enabled = shouldEnableAudio;
                if (shouldEnableAudio && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            
            // Light
            if (container.TryGetComponent<Light>(out var light))
            {
                // Only assign if different to avoid unnecessary work
                if (light.color != chosenColor)
                    light.color = chosenColor;

                light.enabled = ConfigLight.Value;
            }
            
            // ReSharper disable once InvertIf
            if (container.TryGetComponent<ParticleSystem>(out var particleSystem) && container.TryGetComponent<ParticleSystemRenderer>(out var componentParticleSys))
            {
                componentParticleSys.enabled = ConfigSmoke.Value;
                componentParticleSys.material.SetColor(TintColor, chosenColor);

                if (ConfigSmoke.Value && !particleSystem.isPlaying)
                    particleSystem.Play();
            }
        }
        
        yield return null;
    }
}