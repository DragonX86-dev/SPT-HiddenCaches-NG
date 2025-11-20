using System.Threading.Tasks;
using UnityEngine;

namespace HiddenCaches.NG;

public class FlareComponent : MonoBehaviour
{
    private static readonly int LocalMinimalAmbientLight = Shader.PropertyToID("_LocalMinimalAmbientLight");
    private static readonly int TintColor = Shader.PropertyToID("_TintColor");

    public async void Start()
    {
        if (BundleLoader.Material is null || BundleLoader.AudioClip is null || BundleLoader.ParticleSystem is null)
        {
            await BundleLoader.PopulateComponentsAsync();
        }

        var chosenColor = Plugin.ConfigColor.Value;

        var lightObject = this.GetOrAddComponent<Light>();
        lightObject.Reset();
        lightObject.color = chosenColor;
        lightObject.range = 3f;
        lightObject.enabled = Plugin.ConfigLight.Value;

        var audioSource = this.GetOrAddComponent<AudioSource>();
        audioSource.clip = BundleLoader.AudioClip;
        audioSource.loop = true;
        audioSource.maxDistance = 8;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
        audioSource.spatialBlend = 1f;
        audioSource.volume = 0.182f;
        audioSource.enabled = Plugin.ConfigAudio.Value;

        var particleSystem = this.GetOrAddComponent<ParticleSystem>();
        var mainModule = particleSystem.main;
        var emissionModule = particleSystem.emission;
        emissionModule.rateOverTime = 15f;
        mainModule.gravityModifier = -0.01f;
        mainModule.maxParticles = 150;
        particleSystem.time = 0.4902f;
        mainModule.startColor = new Color(1, 1, 1, 0.2f);
        mainModule.startLifetime = 10;
        mainModule.startSpeed = 3;
        mainModule.startSize = 1;
        mainModule.scalingMode = ParticleSystemScalingMode.Shape;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        var colorOverLifetimeModule = particleSystem.colorOverLifetime;
        colorOverLifetimeModule.enabled = true;
        
        if (BundleLoader.ParticleSystem != null)
        {
            colorOverLifetimeModule.color = BundleLoader.ParticleSystem.colorOverLifetime.color;
        }

        var shapeModule = particleSystem.shape;
        shapeModule.radius = 0.01f;

        var textureSheetAnimationModule = particleSystem.textureSheetAnimation;
        textureSheetAnimationModule.enabled = true;
        textureSheetAnimationModule.numTilesX = 8;
        textureSheetAnimationModule.numTilesY = 8;

        var limitVelocityModule = particleSystem.limitVelocityOverLifetime;
        limitVelocityModule.enabled = true;
        limitVelocityModule.dampen = 1f;
        limitVelocityModule.limitMultiplier = 0.4f;

        mainModule.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;

        var particleSystemRenderer = this.GetOrAddComponent<ParticleSystemRenderer>();
        particleSystemRenderer.enableGPUInstancing = false;
        particleSystemRenderer.maxParticleSize = 20f;
        particleSystemRenderer.receiveShadows = true;
        particleSystemRenderer.material = BundleLoader.Material!;
        particleSystemRenderer.material.SetColor(LocalMinimalAmbientLight, new Color(1f, 1f, 1f, 1f));
        particleSystemRenderer.material.SetColor(TintColor, chosenColor);
        particleSystemRenderer.enabled = Plugin.ConfigSmoke.Value;

        audioSource.Play();
        particleSystem.Play();
    }
}