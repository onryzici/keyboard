using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using TMPro;
using LittleSwitch;
using LittleSwitch.Environment;
using Object=UnityEngine.Object;

public static class WideWorkshopRestore
{
    public const string ScenePath="Assets/Scenes/WideWorkshop.unity";
    const string Root="Assets/WorkshopRevision";
    public const float Units=4.3f;
    public static Vector3 Map(Vector3 p)=>p*Units+new Vector3(3.87f,.0672f,-12.633f);
    static Color Hex(string s){ColorUtility.TryParseHtmlString("#"+s,out var c);return c;}
    static Material Finish(string kind)
    {
        string path=Root+"/WideFinish"+kind+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!m){m=new Material(Shader.Find("HDRP/Lit"));AssetDatabase.CreateAsset(m,path);}
        m.color=Hex(kind=="Plaster"?"D0C3B4":kind=="DarkWood"?"94715B":"BEA084");m.SetFloat("_Smoothness",.18f);
        if(kind=="Plaster")
        {m.SetTexture("_NormalMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/Art/Painterly/PlasterRelief.png"));m.SetFloat("_NormalScale",.18f);}
        else
        {m.SetTexture("_BaseColorMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/PaintedWalnut.png"));m.SetTexture("_NormalMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/ThirdParty/PolyHaven/fine_grained_wood/nor_gl.png"));m.SetFloat("_NormalScale",.12f);}
        HDMaterial.ValidateMaterial(m);EditorUtility.SetDirty(m);return m;
    }
    static Bounds Bounds(GameObject go){var all=go.GetComponentsInChildren<Renderer>();var b=all[0].bounds;foreach(var r in all.Skip(1))b.Encapsulate(r.bounds);return b;}
    static GameObject Transfer(GameObject source,Scene target)
    {var go=Object.Instantiate(source);go.transform.SetParent(null);SceneManager.MoveGameObjectToScene(go,target);return go;}
    static void GroundCarton(GameObject source,Scene target,Vector3 position,float size)
    {
        var go=Transfer(source,target);go.name="Worn cardboard floor parcel";
        var b=Bounds(go);go.transform.localScale*=size*Units/Mathf.Max(b.size.x,b.size.z);b=Bounds(go);go.transform.position+=Map(position)-new Vector3(b.center.x,b.min.y,b.center.z);
        foreach(var component in go.GetComponentsInChildren<MovableDeskProp>())Object.DestroyImmediate(component);
        var solid=new GameObject("Floor parcel collision");SceneManager.MoveGameObjectToScene(solid,target);solid.layer=2;b=Bounds(go);solid.transform.position=b.center;solid.AddComponent<BoxCollider>().size=b.size;
    }
    public static void Build()
    {
        // Load the approved wide scene directly; do not regenerate or replace its furniture.
        var scene=EditorSceneManager.OpenScene("Assets/Scenes/WalkableWorkshop.unity");
        var originalTransforms=Object.FindObjectsByType<Renderer>().ToDictionary(r=>r.transform,r=>r.transform.localToWorldMatrix);
        var originalRootObjects=scene.GetRootGameObjects();
        var frame=new GameObject("Approved wide workshop coordinate frame").transform;
        foreach(var root in originalRootObjects)root.transform.SetParent(frame,true);
        frame.position=Map(Vector3.zero);frame.localScale=Vector3.one*Units;
        int preserved=0;
        foreach(var entry in originalTransforms)
        {
            var expected=frame.localToWorldMatrix*entry.Value;var actual=entry.Key.localToWorldMatrix;
            for(int i=0;i<16;i++)if(Mathf.Abs(expected[i]-actual[i])>.002f)throw new Exception("Unexpected layout change: "+entry.Key.name);
            preserved++;
        }
        var shell=GameObject.Find("WorkshopShell");
        foreach(var r in shell.GetComponentsInChildren<Renderer>())
        {
            if(r.name.EndsWith("_Floor"))continue;
            r.sharedMaterials=r.sharedMaterials.Select(m=>m.name=="Plaster"?Finish("Plaster"):m.name=="DarkWood"?Finish("DarkWood"):m.name=="HoneyWood"?Finish("HoneyWood"):m).ToArray();
        }
        var lining=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Models/WideWorkshopWallFinish.fbx"));
        lining.transform.SetParent(frame,false);foreach(var r in lining.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>Finish(m.name.Split('.')[0])).ToArray();

        // Carry forward the user's explicit small corrections to this same environment.
        foreach(var t in frame.GetComponentsInChildren<Transform>().ToArray())
        {
            if(t.name.IndexOf("stool",StringComparison.OrdinalIgnoreCase)>=0)t.gameObject.SetActive(false);
            if(t.GetComponent<TMP_Text>())t.gameObject.SetActive(false);
        }
        var mainArea=frame.GetComponentsInChildren<Transform>().First(t=>t.name.StartsWith("02 Main"));
        foreach(var t in mainArea.GetComponentsInChildren<Transform>())if(t.name=="DisplayKeyboard" && MapInverse(t.position).y<1.2f)t.gameObject.SetActive(false);
        var mat=mainArea.GetComponentsInChildren<Renderer>().First(r=>r.name=="Inset sage cutting mat");
        var blue=AssetDatabase.LoadAssetAtPath<Material>(Root+"/RestoredBlueMat.mat");
        if(!blue){blue=new Material(Shader.Find("HDRP/Lit"));AssetDatabase.CreateAsset(blue,Root+"/RestoredBlueMat.mat");}
        blue.color=Color.white;blue.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/Art/CuttingMat.png");blue.SetFloat("_Smoothness",.15f);HDMaterial.ValidateMaterial(blue);mat.sharedMaterial=blue;

        var original=EditorSceneManager.OpenScene("Assets/Scenes/Workshop.unity",OpenSceneMode.Additive);
        var originalObjects=original.GetRootGameObjects().SelectMany(o=>o.GetComponentsInChildren<Transform>(true)).ToArray();
        var sourceShop=originalObjects.Select(t=>t.GetComponent<ShopGame>()).First(g=>g);
        SceneManager.SetActiveScene(scene);
        var carton=originalObjects.First(t=>t.name=="Worn delivery carton 0").gameObject;
        var oldFloorBox=frame.GetComponentsInChildren<Transform>().FirstOrDefault(t=>t.name=="cardboardBoxOpen" && MapInverse(t.position).y<.05f);
        if(oldFloorBox)oldFloorBox.gameObject.SetActive(false);
        GroundCarton(carton,scene,new Vector3(-4.15f,0,-3.56f),.48f);
        GroundCarton(carton,scene,new Vector3(-2.1f,0,1.82f),.46f);
        GroundCarton(carton,scene,new Vector3(3.48f,0,3.42f),.42f);
        foreach(var source in originalObjects.Select(t=>t.GetComponent<PartsPackage>()).Where(p=>p))
        {
            var go=Transfer(source.gameObject,scene);go.name=source.name;var package=go.GetComponent<PartsPackage>();
            package.shelfPosition=Map(new Vector3(package.keycaps?-1.82f:-2.20f,1.43f,3.08f));
            package.deskPosition=new Vector3(-2.8f,4.09f,package.keycaps?-.55f:-1.55f);package.Present(false);
        }
        foreach(var name in new[]{"Usable switch puller","Usable pin straightening pliers"})Transfer(originalObjects.First(t=>t.name==name).gameObject,scene).name=name;
        var systems=new GameObject("Keyboard design and customer orders");var shop=systems.AddComponent<ShopGame>();shop.catalog=sourceShop.catalog;
        EditorSceneManager.CloseScene(original,true);

        var walker=Object.FindAnyObjectByType<WorkshopWalker>();walker.worldUnitsPerMetre=Units;walker.walkingSpeed=1.35f;walker.sensitivity=.045f;walker.lookSmoothTime=.09f;
        var walking=walker.playerCamera;walking.farClipPlane=430;walking.nearClipPlane=.20f;
        var work=Object.Instantiate(walking.gameObject).GetComponent<Camera>();work.name="Keyboard design camera";work.transform.SetParent(null,true);work.enabled=false;work.tag="MainCamera";
        var listener=work.GetComponent<AudioListener>();if(listener)Object.DestroyImmediate(listener);work.GetComponent<HDAdditionalCameraData>().volumeAnchorOverride=work.transform;shop.viewCamera=work;
        var computer=frame.GetComponentsInChildren<Transform>().First(t=>t.name=="RetroComputer").gameObject;
        computer.AddComponent<OrderTerminal>();var cb=Bounds(computer);var click=new GameObject("Computer interaction surface",typeof(BoxCollider));click.transform.position=cb.center;click.transform.SetParent(computer.transform,true);click.GetComponent<BoxCollider>().size=Vector3.Scale(cb.size,new Vector3(1/computer.transform.lossyScale.x,1/computer.transform.lossyScale.y,1/computer.transform.lossyScale.z));
        var visit=walker.gameObject.AddComponent<WorkshopVisit>();visit.shop=shop;visit.walker=walker;visit.walkingCamera=walking;visit.orderTerminal=computer.transform;
        foreach(var l in frame.GetComponentsInChildren<Light>())if(l.type!=LightType.Directional){l.intensity*=Units*Units;l.range*=Units;l.areaSize*=Units;}
        var sun=frame.GetComponentsInChildren<Light>().First(l=>l.type==LightType.Directional);sun.intensity=10000;sun.GetComponent<HDAdditionalLightData>().volumetricDimmer=.3f;sun.GetComponent<HDAdditionalLightData>().angularDiameter=2;
        var volume=frame.GetComponentsInChildren<Volume>().First(v=>v.isGlobal);string profilePath=Root+"/WideAtmosphere.asset";
        if(!AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath))AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(volume.sharedProfile),profilePath);volume.sharedProfile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
        var profile=volume.sharedProfile;profile.TryGet<Fog>(out var fog);fog.meanFreePath.Override(180*Units);fog.maximumHeight.Override(4*Units);fog.depthExtent.Override(24*Units);fog.anisotropy.Override(.25f);
        foreach(var local in frame.GetComponentsInChildren<LocalVolumetricFog>()){local.parameters.size*=Units;local.parameters.meanFreePath=100*Units;}
        profile.TryGet<HDShadowSettings>(out var shadows);shadows.maxShadowDistance.Override(35*Units);
        profile.TryGet<ContactShadows>(out var contact);contact.length.Override(.12f*Units);contact.maxDistance.Override(25*Units);
        profile.TryGet<ScreenSpaceAmbientOcclusion>(out var ao);ao.radius.Override(.2f*Units);
        EditorUtility.SetDirty(profile);AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene,ScenePath);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        File.WriteAllText("Logs/wide-layout-audit.txt",$"{preserved} original renderer transforms preserved by one uniform coordinate mapping.\nApproved wide scene used directly. Original furniture mesh assets retained.\nWall/ceiling finishes and three floor cartons changed; physical keyboard/order systems added.\n");
    }
    public static Vector3 MapInverse(Vector3 p)=>(p-new Vector3(3.87f,.0672f,-12.633f))/Units;
    public static void Refresh()=>AssetDatabase.Refresh();
    public static void Open()=>EditorSceneManager.OpenScene(ScenePath);
    public static void BuildWindows()
    {
        Open();var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="../Builds/WalkableWorkshop/Little Switch Workshop.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
        File.WriteAllText("Logs/wide-build.txt",report.summary.result+" errors="+report.summary.totalErrors);if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
    }
}
