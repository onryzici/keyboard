using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Object = UnityEngine.Object;

// Migration is explicit and repeatable. It never rebuilds workshop geometry or gameplay.
public static class HDRPWorkshopLighting
{
    const string Folder = "Assets/Settings/LittleSwitchHDRP";
    const string ScenePath = "Assets/Scenes/Workshop.unity";
    static readonly Dictionary<Material, Material> Materials = new();

    [MenuItem("Little Switch/HDRP/Migrate existing workshop")]
    public static void Migrate()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Exit Play Mode first.");
        if (!Application.dataPath.Replace('\\','/').EndsWith("/keyboard/LittleSwitch/Assets", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("This migration belongs to keyboard/LittleSwitch only.");
        EditorSceneManager.OpenScene(ScenePath);
        Directory.CreateDirectory(Folder + "/Materials"); AssetDatabase.Refresh();
        ConfigurePipeline();
        Materials.Clear();
        // Include authored materials used by the runtime upgrade prefab, retaining texture references.
        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets/LittleSwitch/Art" }))
            Convert(AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid)));
        foreach (var renderer in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            ConvertRenderer(renderer);
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/LittleSwitch/Resources" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var root = PrefabUtility.LoadPrefabContents(path);
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true)) ConvertRenderer(renderer);
            PrefabUtility.SaveAsPrefabAsset(root, path); PrefabUtility.UnloadPrefabContents(root);
        }
        Apply();
        Debug.Log("LITTLE_SWITCH_HDRP_MIGRATION_COMPLETE materials=" + Materials.Count);
    }

    static void ConfigurePipeline()
    {
        var asset = AssetDatabase.LoadAssetAtPath<HDRenderPipelineAsset>(Folder + "/WorkshopPipeline.asset");
        if (!asset) { asset = ScriptableObject.CreateInstance<HDRenderPipelineAsset>(); AssetDatabase.CreateAsset(asset, Folder + "/WorkshopPipeline.asset"); }
        var settings = asset.currentPlatformRenderPipelineSettings;
        settings.supportVolumetrics = true; settings.supportSSGI = true; settings.supportSSAO = true;
        settings.supportSSR = true; settings.supportMotionVectors = true;
        settings.supportRayTracing = false; settings.supportWater = false;
        settings.supportVolumetricClouds = false; settings.supportDistortion = false;
        settings.supportedLitShaderMode = RenderPipelineSettings.SupportedLitShaderMode.DeferredOnly;
        asset.currentPlatformRenderPipelineSettings = settings;
        GraphicsSettings.defaultRenderPipeline = asset;
        int original = QualitySettings.GetQualityLevel();
        for (int i = 0; i < QualitySettings.names.Length; i++) { QualitySettings.SetQualityLevel(i, false); QualitySettings.renderPipeline = asset; }
        QualitySettings.SetQualityLevel(original, false);
        PlayerSettings.colorSpace = ColorSpace.Linear;
        var globalType = typeof(HDRenderPipelineAsset).Assembly.GetType("UnityEngine.Rendering.HighDefinition.HDRenderPipelineGlobalSettings");
        globalType.GetMethod("Ensure", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { true });
        EditorUtility.SetDirty(asset);
    }

    static void ConvertRenderer(Renderer renderer)
    {
        if (renderer.GetComponent<TMPro.TMP_Text>()) return;
        var original = renderer.sharedMaterials;
        var converted = original.Select(Convert).ToArray();
        if (!original.SequenceEqual(converted)) { renderer.sharedMaterials = converted; EditorUtility.SetDirty(renderer); }
    }
    static Material Convert(Material source)
    {
        if (!source || !source.shader) return source;
        if (Materials.TryGetValue(source, out var existing)) return existing;
        string shaderName = source.shader.name;
        if (!(shaderName.StartsWith("Universal Render Pipeline/") || shaderName == "Standard" || shaderName.StartsWith("Unlit/"))) return source;
        bool unlit = shaderName.Contains("Unlit");
        var color = source.HasProperty("_BaseColor") ? source.GetColor("_BaseColor") : source.color;
        var texture = source.mainTexture; var tiling = source.mainTextureScale; var offset = source.mainTextureOffset;
        float smoothness = source.HasProperty("_Smoothness") ? source.GetFloat("_Smoothness") : .18f;
        float metallic = source.HasProperty("_Metallic") ? source.GetFloat("_Metallic") : 0;
        var normal = source.HasProperty("_BumpMap") ? source.GetTexture("_BumpMap") : null;
        float normalScale = source.HasProperty("_BumpScale") ? source.GetFloat("_BumpScale") : 1;
        var mask = source.HasProperty("_MetallicGlossMap") ? source.GetTexture("_MetallicGlossMap") : null;
        var emission = source.HasProperty("_EmissionColor") && source.IsKeywordEnabled("_EMISSION") ? source.GetColor("_EmissionColor") : Color.black;
        bool alphaClip = source.HasProperty("_AlphaClip") && source.GetFloat("_AlphaClip") > .5f;
        float cutoff = source.HasProperty("_Cutoff") ? source.GetFloat("_Cutoff") : .5f;
        bool transparent = source.HasProperty("_Surface") && source.GetFloat("_Surface") > .5f;
        string path = AssetDatabase.GetAssetPath(source);
        // Imported originals stay intact; reference a derived HDRP material instead.
        Material result = source;
        if (!path.EndsWith(".mat") || path.Contains("/ThirdParty/"))
        {
            result = new Material(source);
            string safe = string.Concat(source.name.Select(c => char.IsLetterOrDigit(c) ? c : '_'));
            AssetDatabase.CreateAsset(result, AssetDatabase.GenerateUniqueAssetPath(Folder + "/Materials/" + safe + ".mat"));
        }
        Materials[source] = result;
        result.shader = Shader.Find(unlit ? "HDRP/Unlit" : "HDRP/Lit"); result.shaderKeywords = Array.Empty<string>();
        result.SetColor(unlit ? "_UnlitColor" : "_BaseColor", color);
        string mapName = unlit ? "_UnlitColorMap" : "_BaseColorMap";
        result.SetTexture(mapName, texture); result.SetTextureScale(mapName, tiling); result.SetTextureOffset(mapName, offset);
        if (!unlit)
        {
            result.SetFloat("_Smoothness", smoothness); result.SetFloat("_Metallic", metallic);
            result.SetTexture("_NormalMap", normal); result.SetFloat("_NormalScale", normalScale);
            // Authored workshop masks already pack metal/AO/smoothness in R/G/A.
            if (mask) { result.SetTexture("_MaskMap", mask); result.SetFloat("_SmoothnessRemapMax", smoothness); result.SetFloat("_AORemapMin", .65f); }
        }
        result.SetFloat("_SurfaceType", transparent ? 1 : 0);
        result.SetFloat("_AlphaCutoffEnable", alphaClip ? 1 : 0); result.SetFloat("_AlphaCutoff", cutoff);
        result.SetColor("_EmissiveColor", emission * 250f);
        result.SetFloat("_EmissiveExposureWeight", 1);
        HDMaterial.ValidateMaterial(result); EditorUtility.SetDirty(result);
        return result;
    }

    [MenuItem("Little Switch/HDRP/Apply workshop lighting")]
    public static void Apply()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Exit Play Mode first.");
        if (!(GraphicsSettings.defaultRenderPipeline is HDRenderPipelineAsset)) throw new InvalidOperationException("Migrate the pipeline first.");
        var pipeline = (HDRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
        var pipelineSettings = pipeline.currentPlatformRenderPipelineSettings;
        pipelineSettings.hdShadowInitParams.directionalShadowFilteringQuality = HDShadowFilteringQuality.High;
        pipeline.currentPlatformRenderPipelineSettings = pipelineSettings;
        EditorUtility.SetDirty(pipeline);
        foreach (var camera in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var urp = camera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            if (urp) Object.DestroyImmediate(urp);
            var data = camera.GetComponent<HDAdditionalCameraData>() ?? camera.gameObject.AddComponent<HDAdditionalCameraData>();
            data.volumeLayerMask = 1; data.volumeAnchorOverride = camera.transform;
            data.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
            data.clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;
            data.customRenderingSettings = true;
            foreach (var field in new[] { FrameSettingsField.Volumetrics, FrameSettingsField.SSAO, FrameSettingsField.SSGI, FrameSettingsField.ContactShadows, FrameSettingsField.Postprocess })
            {
                data.renderingPathCustomFrameSettings.SetEnabled(field, true);
                data.renderingPathCustomFrameSettingsOverrideMask.mask[(uint)field] = true;
            }
            camera.allowHDR = true; camera.allowMSAA = false;
        }
        // Retire the former mesh beams. HDRP light scattering replaces their entire visual role.
        foreach (var renderer in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (renderer.sharedMaterials.Any(m => m && m.shader && m.shader.name == "LittleSwitch/SoftWindowRay")) renderer.enabled = false;
        var skyBackdrop = GameObject.Find("Distant evening sky").GetComponent<Renderer>();
        skyBackdrop.shadowCastingMode = ShadowCastingMode.Off; skyBackdrop.receiveShadows = false;
        foreach (var light in Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None)) light.enabled = false;

        var sun = GameObject.Find("Late afternoon").GetComponent<Light>();
        sun.enabled = true; sun.type = LightType.Directional; sun.lightUnit = LightUnit.Lux; sun.intensity = 16000;
        sun.color = Color.white; sun.useColorTemperature = true; sun.colorTemperature = 5250;
        sun.transform.rotation = Quaternion.Euler(26, 81, 0); sun.shadows = LightShadows.Soft;
        var sunData = sun.GetComponent<HDAdditionalLightData>() ?? sun.gameObject.AddComponent<HDAdditionalLightData>();
        sunData.angularDiameter = 1.2f; sunData.volumetricDimmer = 2.2f; sunData.SetShadowDimmer(1,1);
        sunData.SetShadowResolution(2048); sunData.SetShadowResolutionOverride(true);
        sunData.useContactShadow.useOverride = true; sunData.useContactShadow.@override = true;
        RenderSettings.sun = sun;

        var lantern = GameObject.Find("Shelf warm pool").GetComponent<Light>();
        lantern.enabled = true; lantern.type = LightType.Point; lantern.lightUnit = LightUnit.Lumen;
        lantern.intensity = 900; lantern.range = 4; lantern.color = Color.white;
        lantern.useColorTemperature = true; lantern.colorTemperature = 3600; lantern.shadows = LightShadows.Soft;
        var lanternData = lantern.GetComponent<HDAdditionalLightData>() ?? lantern.gameObject.AddComponent<HDAdditionalLightData>();
        lanternData.volumetricDimmer = .03f; lanternData.SetShadowResolution(512);
        foreach (var flicker in Object.FindObjectsByType<LittleSwitch.LanternFlicker>(FindObjectsSortMode.None)) flicker.intensity = 900;

        foreach (var volume in Object.FindObjectsByType<Volume>(FindObjectsSortMode.None))
            if (volume.name != "Little Switch HDRP atmosphere") volume.enabled = false;
        var volumeObject = GameObject.Find("Little Switch HDRP atmosphere") ?? new GameObject("Little Switch HDRP atmosphere");
        var global = volumeObject.GetComponent<Volume>() ?? volumeObject.AddComponent<Volume>();
        global.isGlobal = true; global.priority = 10; global.enabled = true;
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(Folder + "/WorkshopAtmosphere.asset");
        if (!profile) { profile = ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(profile, Folder + "/WorkshopAtmosphere.asset"); }
        global.sharedProfile = profile;
        var skySetup = Get<VisualEnvironment>(profile); skySetup.skyType.Override(3); skySetup.skyAmbientMode.Override(SkyAmbientMode.Dynamic);
        var sky = Get<GradientSky>(profile); sky.top.Override(new Color(.20f,.29f,.43f)); sky.middle.Override(new Color(.40f,.46f,.54f)); sky.bottom.Override(new Color(.23f,.27f,.33f));
        sky.exposure.Override(10.5f); sky.multiplier.Override(.5f);
        var exposure = Get<Exposure>(profile); exposure.mode.Override(ExposureMode.Fixed); exposure.fixedExposure.Override(10); exposure.compensation.Override(0);
        var tone = Get<Tonemapping>(profile); tone.mode.Override(TonemappingMode.ACES);
        var bloom = Get<Bloom>(profile); bloom.intensity.Override(.06f); bloom.threshold.Override(2); bloom.scatter.Override(.6f);
        var ao = Get<ScreenSpaceAmbientOcclusion>(profile); ao.intensity.Override(.5f); ao.radius.Override(.28f); ao.directLightingStrength.Override(.1f);
        var contact = Get<ContactShadows>(profile); contact.enable.Override(true); contact.directionalOnly.Override(true); contact.length.Override(.12f); contact.opacity.Override(.65f); contact.maxDistance.Override(25); contact.fadeDistance.Override(5);
        var shadows = Get<HDShadowSettings>(profile); shadows.maxShadowDistance.Override(45); shadows.cascadeShadowSplitCount.Override(4);
        var gi = Get<GlobalIllumination>(profile); gi.enable.Override(true); gi.tracing.Override(RayCastingMode.RayMarching); gi.quality.Override(3); gi.fullResolutionSS.Override(true); gi.maxRaySteps = 64; gi.denoiseSS = true;
        var fog = Get<Fog>(profile); fog.enabled.Override(true); fog.enableVolumetricFog.Override(true);
        fog.meanFreePath.Override(100); fog.baseHeight.Override(0); fog.maximumHeight.Override(14);
        fog.albedo.Override(new Color(.87f,.9f,.94f)); fog.anisotropy.Override(.58f); fog.depthExtent.Override(40);
        fog.globalLightProbeDimmer.Override(.3f); fog.multipleScatteringIntensity.Override(.15f);
        fog.quality.Override(3); fog.fogControlMode = FogControl.Manual;
        fog.volumeSliceCount.Override(128); fog.screenResolutionPercentage.Override(25); fog.sliceDistributionUniformity.Override(.6f);
        var fogObject = GameObject.Find("HDRP window scattering") ?? new GameObject("HDRP window scattering");
        var local = fogObject.GetComponent<LocalVolumetricFog>() ?? fogObject.AddComponent<LocalVolumetricFog>();
        fogObject.transform.position = new Vector3(-3.8f,6.15f,-.55f); fogObject.transform.rotation = Quaternion.Euler(0,0,-26);
        local.parameters.size = new Vector3(15,4.8f,6.4f); local.parameters.meanFreePath = 35;
        local.parameters.albedo = new Color(.88f,.90f,.94f);
        local.parameters.positiveFade = new Vector3(.2f,.25f,.2f); local.parameters.negativeFade = new Vector3(.2f,.25f,.2f);
        local.parameters.distanceFadeStart = 35; local.parameters.distanceFadeEnd = 50;
        EditorUtility.SetDirty(profile); AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene()); EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        if (File.Exists("Logs/hdrp-before-layout.json") && HDRPWorkshopReview.Layout() != File.ReadAllText("Logs/hdrp-before-layout.json")) throw new Exception("Workshop model/transform preservation check failed.");
        Debug.Log("LITTLE_SWITCH_HDRP_LIGHTING_APPLIED; active lights=2; original model transforms preserved");
    }
    static T Get<T>(VolumeProfile profile) where T : VolumeComponent
    {
        if (profile.TryGet<T>(out var component)) return component;
        component = profile.Add<T>(true); AssetDatabase.AddObjectToAsset(component, profile); return component;
    }
}
