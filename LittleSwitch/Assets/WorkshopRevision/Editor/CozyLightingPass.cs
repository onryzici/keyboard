using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Object=UnityEngine.Object;

public static class CozyLightingPass
{
    static void Practical(string name,Vector3 p,float lumens,Vector2 size,float kelvin)
    {
        var go=GameObject.Find(name)??new GameObject(name);
        var l=go.GetComponent<Light>();if(!l)l=go.AddComponent<Light>();
        var hd=go.GetComponent<HDAdditionalLightData>();if(!hd)hd=go.AddComponent<HDAdditionalLightData>();
        l.transform.position=WideWorkshopRestore.Map(p);l.transform.rotation=Quaternion.Euler(90,0,0);
        l.type=LightType.Rectangle;l.areaSize=size*4.3f;l.lightUnit=LightUnit.Lumen;l.intensity=lumens*4.3f*4.3f;
        l.useColorTemperature=true;l.colorTemperature=kelvin;l.range=30;l.shadows=LightShadows.Soft;
        hd.volumetricDimmer=.02f;hd.SetShadowResolution(1024);
    }
    public static void Apply()
    {
        WideWorkshopRestore.Open();
        var p=Object.FindObjectsByType<Volume>().First(v=>v.isGlobal).sharedProfile;
        p.TryGet<Exposure>(out var exp);exp.mode.Override(ExposureMode.Fixed);exp.fixedExposure.Override(7.8f);
        p.TryGet<GradientSky>(out var sky);sky.exposure.Override(7.6f);
        sky.top.Override(new Color(.42f,.52f,.64f));sky.middle.Override(new Color(.64f,.69f,.74f));sky.bottom.Override(new Color(.35f,.39f,.43f));
        var sun=Object.FindObjectsByType<Light>().First(l=>l.type==LightType.Directional);
        sun.intensity=4500;sun.colorTemperature=4600;sun.transform.rotation=Quaternion.Euler(23,76,0);
        var hd=sun.GetComponent<HDAdditionalLightData>();hd.angularDiameter=4f;hd.volumetricDimmer=.5f;hd.SetShadowResolution(4096);
        p.TryGet<Fog>(out var fog);fog.meanFreePath.Override(1000);fog.anisotropy.Override(.35f);fog.globalLightProbeDimmer.Override(.15f);
        foreach(var f in Object.FindObjectsByType<LocalVolumetricFog>())f.parameters.meanFreePath=500;
        p.TryGet<GlobalIllumination>(out var gi);gi.ambientProbeDimmer.Override(.25f);gi.depthBufferThickness.Override(.4f);
        p.TryGet<ScreenSpaceAmbientOcclusion>(out var ao);ao.intensity.Override(.35f);ao.directLightingStrength.Override(0);
        p.TryGet<Bloom>(out var bloom);bloom.intensity.Override(.06f);bloom.threshold.Override(1.5f);
        Practical("Main bench pendant",new Vector3(-.9f,2.70f,2.3f),1700,new Vector2(.55f,.55f),3400);
        Practical("Orders ceiling pendant",new Vector3(.6f,2.76f,-1.70f),1700,new Vector2(.55f,.55f),3500);
        Practical("Main bench task lamp",new Vector3(.13f,1.49f,3.0f),350,new Vector2(.18f,.12f),3700);
        Practical("Electronics task lamp",new Vector3(4.84f,1.50f,-.61f),500,new Vector2(.18f,.12f),3800);
        Practical("Orders task lamp",new Vector3(-.2f,1.30f,-3.65f),400,new Vector2(.18f,.12f),3600);
        const string diffuserPath="Assets/WorkshopRevision/IlluminatedDiffuser.mat";
        var diffuser=AssetDatabase.LoadAssetAtPath<Material>(diffuserPath);
        if(!diffuser){diffuser=new Material(Shader.Find("HDRP/Lit"));AssetDatabase.CreateAsset(diffuser,diffuserPath);}
        diffuser.color=new Color(1,.83f,.6f);diffuser.SetColor("_EmissiveColor",new Color(1,.72f,.4f)*1200);diffuser.SetFloat("_UseEmissiveIntensity",0);HDMaterial.ValidateMaterial(diffuser);EditorUtility.SetDirty(diffuser);
        foreach(var t in Object.FindObjectsByType<Transform>().Where(t=>t.name=="PendantLamp"))
        foreach(var r in t.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>m.name=="Glow"||m.name=="IlluminatedDiffuser"?diffuser:m).ToArray();
        EditorUtility.SetDirty(p);AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        File.WriteAllText("Logs/cozy-lighting-settings.txt","Sun 4500 lux / 4600 K / angle 23,76 / angular diameter 4 / volume .5. Fixed EV7.8. Gradient sky EV7.6. Fog MFP1000, local500, anisotropy .35. SSGI ambient fallback .25, thickness .4. SSAO .35. ACES, bloom .06 threshold1.5. One directional + five rectangle lights located at the five existing lamp fixtures. Room geometry/gameplay untouched.");
    }
}


