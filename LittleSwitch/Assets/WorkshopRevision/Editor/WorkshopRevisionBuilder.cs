using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using LittleSwitch;
using LittleSwitch.Environment;
using Object=UnityEngine.Object;

public static class WorkshopRevisionBuilder
{
    public const string ScenePath="Assets/Scenes/CraftedWorkshop.unity";
    const string Root="Assets/WorkshopRevision";
    public static void Inspect()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Workshop.unity");
        File.WriteAllLines("Logs/revision-hierarchy.txt",Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).Where(t=>t.gameObject.activeInHierarchy && (t.parent==null || t.parent.parent==null)).Select(t=>$"{t.name}|parent={t.parent?.name}|{t.position}"));
    }
    public static void Refresh()=>AssetDatabase.Refresh();
    static Color Hex(string s){ColorUtility.TryParseHtmlString("#"+s,out var c);return c;}
    static Material Surface(string name)
    {
        string path=Root+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!m){m=new Material(Shader.Find("HDRP/Lit"));AssetDatabase.CreateAsset(m,path);}
        m.color=Hex(name=="Plaster"?"D7CABC":name=="DarkWood"?"997357":name=="HoneyWood"?"CDB194":name=="Sage"?"74877A":name=="Metal"?"424643":name=="Steel"?"92978C":name=="Screw"?"B39A70":"D7CEB6");
        m.SetFloat("_Smoothness",.22f);
        if(name=="DarkWood"||name=="HoneyWood")
        {
            m.SetTexture("_BaseColorMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/PaintedWalnut.png"));
            m.SetTexture("_NormalMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/ThirdParty/PolyHaven/fine_grained_wood/nor_gl.png"));m.SetFloat("_NormalScale",.16f);
            m.SetTexture("_MaskMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/ThirdParty/PolyHaven/fine_grained_wood/Mask.png"));
            m.SetFloat("_SmoothnessRemapMin",.12f);m.SetFloat("_SmoothnessRemapMax",.27f);m.SetFloat("_AORemapMin",.8f);
        }
        if(name=="Plaster"){m.SetTexture("_NormalMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/Art/Painterly/PlasterRelief.png"));m.SetFloat("_NormalScale",.12f);m.SetFloat("_Smoothness",.10f);}
        if(name=="Glow")m.SetColor("_EmissiveColor",Hex("FFE1B4")*150);
        HDMaterial.ValidateMaterial(m);EditorUtility.SetDirty(m);return m;
    }
    static GameObject Model(string name,Vector3 position,float yaw=0)
    {
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Models/"+name+".fbx");
        var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab);go.transform.SetPositionAndRotation(position,Quaternion.Euler(0,yaw,0));go.transform.localScale=Vector3.one*4;
        foreach(var renderer in go.GetComponentsInChildren<Renderer>())
        {renderer.sharedMaterials=renderer.sharedMaterials.Select(m=>Surface(m.name.Split('.')[0])).ToArray();renderer.gameObject.isStatic=true;}
        return go;
    }
    static Bounds Bounds(GameObject go){var all=go.GetComponentsInChildren<Renderer>();var b=all[0].bounds;foreach(var r in all.Skip(1))b.Encapsulate(r.bounds);return b;}
    static void Solid(GameObject go)
    {
        var b=Bounds(go);var wall=new GameObject(go.name+" walking boundary");wall.layer=2;wall.transform.position=b.center;wall.AddComponent<BoxCollider>().size=b.size;
    }
    static GameObject Copy(GameObject source,string name,Vector3 bottom,float scale=1,float yaw=0)
    {
        var go=Object.Instantiate(source);go.name=name;go.transform.SetParent(null);go.transform.Rotate(0,yaw,0,Space.World);go.transform.localScale*=scale;
        var b=Bounds(go);go.transform.position+=bottom-new Vector3(b.center.x,b.min.y,b.center.z);
        // Decorative duplicates must not share gameplay identities or saved desk positions.
        foreach(var c in go.GetComponentsInChildren<MonoBehaviour>())if(c is MovableDeskProp || c is PartsPackage || c is OrderTerminal)Object.DestroyImmediate(c);
        return go;
    }
    public static void Build()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Workshop.unity");
        var oldRoom=GameObject.Find("Spacious textured workshop");oldRoom.SetActive(false);
        foreach(var t in Object.FindObjectsByType<Transform>())if(t.name.Contains("Stool")||t.name.StartsWith("Sunlit drifting dust"))t.gameObject.SetActive(false);
        foreach(var n in new[]{"CraftedFloor","CraftedPlaster","CraftedTimber"})
        {
            var go=Model(n,Vector3.zero);
            foreach(var f in go.GetComponentsInChildren<MeshFilter>())f.gameObject.AddComponent<MeshCollider>().sharedMesh=f.sharedMesh;
        }
        var bench=GameObject.Find("KayKit · table_medium_long");Solid(bench);
        var carton=GameObject.Find("Worn delivery carton 0");
        var receiving=Copy(bench,"Receiving oak table",new Vector3(-8.1f,0,-12.5f),.65f,90);Solid(receiving);
        Copy(carton,"Original worn receiving carton",new Vector3(-8.1f,2.60f,-12.0f),.72f);
        Copy(carton,"Original parcel awaiting dispatch",new Vector3(-8.2f,0,-14.8f),.8f,12);
        var shelf=GameObject.Find("Imported component shelf");
        for(int level=0;level<3;level++)
        {
            Copy(shelf,"Oak archive side shelf "+level,new Vector3(9.35f,3.4f+level*1.7f,-10),1.5f,90);
            for(int j=0;j<3;j++)Copy(carton,"Original archive parcel "+level+" "+j,new Vector3(9.25f,3.80f+level*1.7f,-11.9f+j*1.7f),.54f,j*7);
        }
        var hooks=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WorkshopWalk/Models/CoatHooks.fbx");
        var coat=(GameObject)PrefabUtility.InstantiatePrefab(hooks);coat.transform.position=new Vector3(-7.5f,6.4f,-21.9f);coat.transform.rotation=Quaternion.Euler(0,180,0);coat.transform.localScale=Vector3.one*4;
        foreach(var r in coat.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>Surface(m.name.Split('.')[0])).ToArray();
        Model("CorrectTaskLamp",new Vector3(3.5f,4.03f,1.15f),180);
        Model("CorrectTaskLamp",new Vector3(-8.2f,2.62f,-13.8f),0);
        for(int i=0;i<3;i++)
        {
            var tree=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WorkshopWalk/Models/CourtyardTree.fbx"));tree.transform.position=new Vector3(-20-i*3,0,-14+i*9);tree.transform.localScale=Vector3.one*4;
            foreach(var r in tree.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>Surface(m.name.Split('.')[0]=="Leaf"?"Sage":"DarkWood")).ToArray();
        }
        Lighting();
        var shop=Object.FindAnyObjectByType<ShopGame>();
        var visitor=new GameObject("Workshop walking player",typeof(CharacterController),typeof(WorkshopWalker));visitor.transform.position=new Vector3(1.2f,.15f,-17.5f);
        var body=visitor.GetComponent<CharacterController>();body.height=7.2f;body.center=new Vector3(0,3.6f,0);body.radius=.8f;body.stepOffset=.7f;body.skinWidth=.06f;
        var walkingCamera=Object.Instantiate(shop.viewCamera.gameObject).GetComponent<Camera>();walkingCamera.name="Walking player camera";walkingCamera.transform.SetParent(visitor.transform);walkingCamera.transform.localPosition=new Vector3(0,6.6f,0);walkingCamera.transform.localRotation=Quaternion.identity;walkingCamera.fieldOfView=60;
        walkingCamera.nearClipPlane=.18f;walkingCamera.farClipPlane=180;walkingCamera.tag="MainCamera";shop.viewCamera.tag="Untagged";
        var walker=visitor.GetComponent<WorkshopWalker>();walker.playerCamera=walkingCamera;walker.worldUnitsPerMetre=4;walker.walkingSpeed=1.35f;walker.sensitivity=.045f;walker.lookSmoothTime=.09f;
        var visit=visitor.AddComponent<WorkshopVisit>();visit.shop=shop;visit.walker=walker;visit.walkingCamera=walkingCamera;
        var listener=shop.viewCamera.GetComponent<AudioListener>();if(listener)listener.enabled=false;
        AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),ScenePath);
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        File.WriteAllText("Logs/revision-built.txt",DateTime.UtcNow.ToString("O"));
    }
    static void Area(string name,Vector3 p,float lumens,Vector2 size)
    {
        var l=new GameObject(name).AddComponent<Light>();l.type=LightType.Rectangle;l.transform.SetPositionAndRotation(p,Quaternion.Euler(90,0,0));l.lightUnit=LightUnit.Lumen;l.intensity=lumens;l.areaSize=size;l.range=24;l.useColorTemperature=true;l.colorTemperature=3800;l.shadows=LightShadows.Soft;
        var hd=l.gameObject.AddComponent<HDAdditionalLightData>();hd.volumetricDimmer=0;hd.SetShadowResolution(1024);
    }
    static void Lighting()
    {
        foreach(var l in Object.FindObjectsByType<Light>())l.enabled=false;
        foreach(var flicker in Object.FindObjectsByType<LanternFlicker>())flicker.enabled=false;
        var sun=GameObject.Find("Late afternoon").GetComponent<Light>();sun.enabled=true;sun.intensity=6500;sun.colorTemperature=5100;sun.transform.rotation=Quaternion.Euler(24,80,0);
        var hd=sun.GetComponent<HDAdditionalLightData>();hd.volumetricDimmer=.30f;hd.angularDiameter=2.2f;
        var volume=GameObject.Find("Little Switch HDRP atmosphere").GetComponent<Volume>();
        string path=Root+"/CraftedAtmosphere.asset";if(!AssetDatabase.LoadAssetAtPath<VolumeProfile>(path))AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(volume.sharedProfile),path);
        volume.sharedProfile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);var profile=volume.sharedProfile;
        profile.TryGet<Exposure>(out var exposure);exposure.fixedExposure.Override(9.7f);
        profile.TryGet<GradientSky>(out var sky);sky.exposure.Override(9.4f);sky.top.Override(Hex("90A8BC"));sky.middle.Override(Hex("BFC4C2"));sky.bottom.Override(Hex("8F9997"));
        profile.TryGet<Fog>(out var fog);fog.meanFreePath.Override(400);fog.anisotropy.Override(.25f);fog.globalLightProbeDimmer.Override(.1f);
        foreach(var local in Object.FindObjectsByType<LocalVolumetricFog>())local.parameters.meanFreePath=180;
        profile.TryGet<Bloom>(out var bloom);bloom.intensity.Override(.025f);
        Area("Warm main workbench practical",new Vector3(.5f,10.3f,-.4f),9000,new Vector2(4.4f,2.8f));
        Area("Warm receiving practical",new Vector3(-7.5f,7.8f,-12.5f),2800,new Vector2(2.0f,1.4f));
        Area("Downward desk lamp",new Vector3(2.7f,5.23f,1.15f),650,new Vector2(.65f,.45f));
        // Modeled ceiling fixture for each overhead practical.
        foreach(var p in new[]{new Vector3(.5f,10.4f,-.4f),new Vector3(-7.5f,7.9f,-12.5f)})
        {
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WorkshopWalk/Models/PendantLamp.fbx");var fixture=(GameObject)PrefabUtility.InstantiatePrefab(prefab);fixture.transform.position=p;fixture.transform.localScale=Vector3.one*4;
            foreach(var r in fixture.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>Surface(m.name.Split('.')[0])).ToArray();
        }
        EditorUtility.SetDirty(profile);
    }
}
