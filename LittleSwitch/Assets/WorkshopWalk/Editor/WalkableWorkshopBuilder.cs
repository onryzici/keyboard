using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using TMPro;
using LittleSwitch.Environment;
using Object = UnityEngine.Object;

public static class WalkableWorkshopBuilder
{
    public const string ScenePath = "Assets/Scenes/WalkableWorkshop.unity";
    const string Root = "Assets/WorkshopWalk";
    static Transform area;
    static void Subfloor()
    {
        var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Subfloor.asset");
        if(!mesh){mesh=new Mesh{name="Continuous subfloor beneath plank joints"};mesh.vertices=new[]{new Vector3(-5.4f,-.075f,-4.2f),new Vector3(-5.4f,-.075f,4.2f),new Vector3(5.4f,-.075f,4.2f),new Vector3(5.4f,-.075f,-4.2f)};mesh.triangles=new[]{0,1,2,0,2,3};mesh.RecalculateNormals();AssetDatabase.CreateAsset(mesh,Root+"/Subfloor.asset");}
        var go=new GameObject("Continuous dark subfloor",typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(area);go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=Material("Rubber");go.isStatic=true;
    }
    static Dictionary<string,Material> materials = new();
    static readonly Dictionary<string,string> Colors = new() {
        {"HoneyWood","CBA77A"},{"DarkWood","66503B"},{"Plaster","CDC5AF"},{"Sage","788D79"},
        {"Metal","343F3D"},{"Steel","8C9993"},{"Cream","E4DCC4"},{"Terracotta","B97856"},
        {"Paper","D8C29A"},{"Rubber","25312D"},{"Glass","C8E0D6"},{"Glow","FFE2AF"},
        {"Teal","547F79"},{"Floor","A79C87"},{"Screw","AB946C"},{"Leaf","657D4E"}
    };
    static Color Hex(string text) { ColorUtility.TryParseHtmlString("#"+text,out var color); return color; }
    static Material Material(string name)
    {
        if (materials.TryGetValue(name,out var cached)) return cached;
        string path = Root+"/Materials/"+name+".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (!mat) { mat = new Material(Shader.Find("HDRP/Lit")); AssetDatabase.CreateAsset(mat,path); }
        mat.color = Hex(Colors.TryGetValue(name,out var color) ? color : "CBC2AB");
        mat.SetFloat("_Smoothness", name=="Steel" ? .43f : .24f);
        mat.SetFloat("_Metallic", name=="Steel" || name=="Screw" ? .7f : 0);
        if (name=="HoneyWood" || name=="DarkWood" || name=="Floor")
        {
            mat.SetTexture("_BaseColorMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/ThirdParty/PolyHaven/fine_grained_wood/Diffuse.png"));
            if(name=="HoneyWood")mat.color=Hex("FFE2B7")*1.65f;
            mat.SetFloat("_Smoothness",.20f);
        }
        if (name=="Plaster")
        {
            mat.SetTexture("_NormalMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/Art/Painterly/PlasterRelief.png"));
            mat.SetFloat("_NormalScale",.14f); mat.SetFloat("_Smoothness",.07f);
        }
        if (name=="Glass")
        {
            mat.SetFloat("_SurfaceType",1); var c=mat.color;c.a=.17f;mat.color=c;
            mat.SetFloat("_Smoothness",.55f);mat.SetFloat("_EnableBlendModePreserveSpecularLighting",0);
        }
        if (name=="Glow") mat.SetColor("_EmissiveColor",Hex("FFE5B9")*180);
        HDMaterial.ValidateMaterial(mat); EditorUtility.SetDirty(mat); materials[name]=mat;return mat;
    }
    static void Area(string name) { area = new GameObject(name).transform; }
    static GameObject Kit(string name,Vector3 position,float yaw=0, bool collide=false,Vector3? scale=null)
    {
        var asset=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Models/"+name+".fbx");
        if (!asset) throw new Exception("Missing authored asset "+name);
        var obj=(GameObject)PrefabUtility.InstantiatePrefab(asset,area);
        obj.transform.SetPositionAndRotation(position,Quaternion.Euler(0,yaw,0));
        if (scale.HasValue) obj.transform.localScale=scale.Value;
        foreach(var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.sharedMaterials=renderer.sharedMaterials.Select(m=>Material(m.name.Split('.')[0])).ToArray();
            if(name=="SwitchJar" && renderer.sharedMaterial.name=="Glass") renderer.shadowCastingMode=ShadowCastingMode.Off;
            renderer.gameObject.isStatic=true;
        }
        if(collide) Collider(obj,name=="WorkshopShell");
        return obj;
    }
    static Bounds Bounds(GameObject obj)
    {
        var rs=obj.GetComponentsInChildren<Renderer>(); var bounds=rs[0].bounds;
        foreach(var r in rs.Skip(1)) bounds.Encapsulate(r.bounds); return bounds;
    }
    static void Collider(GameObject obj,bool exact=false)
    {
        if(exact)
        {
            foreach(var mf in obj.GetComponentsInChildren<MeshFilter>()) mf.gameObject.AddComponent<MeshCollider>().sharedMesh=mf.sharedMesh;
        }
        else
        {
            var bounds=Bounds(obj); var go=new GameObject(obj.name+" walking collision"); go.transform.SetParent(area); go.transform.position=bounds.center;
            go.AddComponent<BoxCollider>().size=bounds.size;
        }
    }
    static GameObject Prop(string path,Vector3 bottom,float height,float yaw=0,bool collide=false)
    {
        var asset=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LittleSwitch/"+path+".fbx");
        if(!asset)throw new Exception("Missing project asset "+path);
        var holder=new GameObject(Path.GetFileName(path));holder.transform.SetParent(area);
        var obj=(GameObject)PrefabUtility.InstantiatePrefab(asset,holder.transform);
        obj.transform.localRotation=Quaternion.Euler(0,yaw,0)*asset.transform.localRotation;
        var bounds=Bounds(obj);
        if(path.Contains("Sjolle/") && height>.1f)
        {
            var s=bounds.size;var axis=s.x>s.y && s.x>s.z?Vector3.right:s.z>s.y?Vector3.forward:Vector3.up;
            obj.transform.rotation=Quaternion.FromToRotation(axis,Vector3.up)*obj.transform.rotation;
            bounds=Bounds(obj);
        }
        holder.transform.localScale=Vector3.one*(height/Mathf.Max(bounds.size.y,.01f));
        bounds=Bounds(obj); holder.transform.position=bottom-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
        foreach(var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.sharedMaterials=renderer.sharedMaterials.Select(source=>ImportedMaterial(source,path)).ToArray();
            renderer.gameObject.isStatic=true;
        }
        if(collide)Collider(holder);return holder;
    }
    static Material ImportedMaterial(Material source,string model)
    {
        string key="Imported_"+Path.GetFileName(model)+"_"+source.name.Replace('/','_');
        if(materials.TryGetValue(key,out var existing))return existing;
        string path=Root+"/Materials/"+key+".mat";
        var m=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!m){m=new Material(Shader.Find("HDRP/Lit"));AssetDatabase.CreateAsset(m,path);}
        m.color=source.HasProperty("_BaseColor")?source.GetColor("_BaseColor"):source.HasProperty("_Color")?source.GetColor("_Color"):Hex("C7C0A8");
        m.mainTexture=source.mainTexture;
        var solidColors=new Dictionary<string,string>{{"Black","303B37"},{"Black_glossy","29332F"},{"Grey","66746D"},{"GreyLight","A3ABA3"},{"White","D7D3BE"},{"Gold","AA8A54"},{"metal","65716B"},{"metalMedium","737C71"},{"wood","957B54"},{"woodDark","5E4936"},{"Tray blue rubber","57766B"},{"Tray cream enamel","D0CDB7"}};
        if(solidColors.TryGetValue(source.name,out var solid))m.color=Hex(solid);
        if(model.Contains("RetroComputer"))
        {
            string n=source.name.ToLowerInvariant();
            m.color=Hex(n.Contains("glass")?"344B49":n.Contains("vent")?"293634":n.Contains("bezel")?"968E75":n.Contains("function")?"B5815C":"D5CFB8");
            if(n.Contains("glass"))m.SetColor("_EmissiveColor",Hex("739C89")*35);
        }
        if(model.Contains("KayKit/")) {m.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/ThirdParty/KayKit/furniturebits_texture.png");m.color=Color.white;}
        if(model.Contains("KenneyCity/")) {m.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/ThirdParty/KenneyCity/colormap.png");m.color=Hex("CAD0C4");}
        m.SetFloat("_Smoothness",.22f);
        if(source.name.ToLowerInvariant().Contains("bulb"))m.SetColor("_EmissiveColor",Hex("FFE4B4")*180);
        HDMaterial.ValidateMaterial(m);EditorUtility.SetDirty(m);materials[key]=m;return m;
    }
    static void Text(string text,Vector3 position,float width=1.6f,float size=1.4f,float yaw=0,string color="E7DDC6")
    {
        var go=new GameObject(text.Replace('\n',' '));go.transform.SetParent(area);go.transform.SetPositionAndRotation(position,Quaternion.Euler(0,yaw,0));
        var tmp=go.AddComponent<TextMeshPro>();tmp.text=text;tmp.fontSize=size;tmp.color=Hex(color);tmp.alignment=TextAlignmentOptions.Center;
        tmp.rectTransform.sizeDelta=new Vector2(width,.6f); tmp.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
    }
    static void MatSurface(Vector3 position,Vector2 size,float yaw=0)
    {
        var mat=AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/SageCuttingMat.mat");
        if(!mat)
        {
            var tex=new Texture2D(512,256,TextureFormat.RGB24,false);var pixels=new Color[512*256];
            for(int y=0;y<256;y++)for(int x=0;x<512;x++) pixels[y*512+x]=(x%16==0||y%16==0)?Hex("7B9983"):Hex("3E6558");
            tex.SetPixels(pixels);tex.Apply();File.WriteAllBytes(Root+"/Materials/SageCuttingMat.png",tex.EncodeToPNG());Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(Root+"/Materials/SageCuttingMat.png");
            mat=new Material(Shader.Find("HDRP/Lit"));mat.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Materials/SageCuttingMat.png");mat.SetFloat("_Smoothness",.1f);HDMaterial.ValidateMaterial(mat);AssetDatabase.CreateAsset(mat,Root+"/Materials/SageCuttingMat.mat");
        }
        var mesh=new Mesh{name="Tailored cutting mat"};float w=size.x/2,d=size.y/2;
        mesh.vertices=new[]{new Vector3(-w,0,-d),new Vector3(-w,0,d),new Vector3(w,0,d),new Vector3(w,0,-d)};
        mesh.triangles=new[]{0,1,2,0,2,3};mesh.uv=new[]{Vector2.zero,Vector2.up,Vector2.one,Vector2.right};mesh.RecalculateNormals();mesh.RecalculateTangents();
        string path=AssetDatabase.GenerateUniqueAssetPath(Root+"/Materials/CuttingMatMesh.asset");AssetDatabase.CreateAsset(mesh,path);
        var go=new GameObject("Inset sage cutting mat",typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(area);go.transform.SetPositionAndRotation(position,Quaternion.Euler(0,yaw,0));
        go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=mat;
    }

    [MenuItem("Little Switch/Walkable Workshop/Build environment scene")]
    public static void Build()
    {
        if(EditorApplication.isPlaying)throw new Exception("Exit Play first");
        if(!Application.dataPath.Replace('\\','/').EndsWith("/keyboard/LittleSwitch/Assets",StringComparison.OrdinalIgnoreCase))throw new Exception("Wrong project");
        Directory.CreateDirectory(Root+"/Materials");AssetDatabase.Refresh();materials.Clear();
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        Area("01 Architecture — timber and plaster");Kit("WorkshopShell",Vector3.zero,0,true);Subfloor();

        Area("02 Main keyboard workbench");
        Kit("MainWorkbench",new Vector3(-.9f,0,2.82f),0,true);
        Kit("ToolWall",new Vector3(-.9f,1.17f,4.09f));
        MatSurface(new Vector3(-.9f,.942f,2.72f),new Vector2(1.5f,.62f));
        Kit("DisplayKeyboard",new Vector3(-.9f,.946f,2.81f));
        Kit("ComponentDrawers",new Vector3(-2.00f,.942f,3.07f));
        Prop("Art/BenchProps/CompartmentTray",new Vector3(.32f,.944f,2.82f),.065f);
        Prop("ThirdParty/Refined/ArtisanLamp",new Vector3(.40f,.944f,3.02f),.60f,0);
        Prop("ThirdParty/QuinGS/cozy_Stool_Wooden",new Vector3(-1.05f,0,1.81f),.52f,10,true);
        Prop("ThirdParty/MrEliptik/pencil_holder",new Vector3(-2.26f,.944f,2.65f),.16f);
        Prop("ThirdParty/Refined/GlazedWorkshopMug",new Vector3(.43f,.944f,2.46f),.10f);
        Kit("SwitchJar",new Vector3(-1.99f,.944f,2.50f));
        var tools=new[]{"screwdriver01","screwdriver02","plier01","cutter01","vernier01","ruler01"};
        for(int i=0;i<tools.Length;i++)Prop("ThirdParty/Sjolle/"+tools[i],new Vector3(-1.92f+i*.37f,1.65f,3.97f),.25f,180);
        // Bespoke curved cable loops and shelf are already project assets, reused as authored meshes.
        Kit("WallShelf",new Vector3(-.95f,2.64f,3.96f));
        Kit("DisplayKeyboard",new Vector3(-1.28f,2.645f,3.93f),0, false,new Vector3(1.2f,1.2f,1.2f));
        Prop("ThirdParty/KayKit/book_set",new Vector3(-.40f,2.645f,3.92f),.19f,0);
        Kit("CableLoops",new Vector3(.10f,1.23f,3.95f));
        Kit("BrushSet",new Vector3(-1.23f,1.29f,3.93f));
        Kit("NoticeBoard",new Vector3(-3.68f,1.57f,4.10f));
        Text("LITTLE SWITCH",new Vector3(-.9f,3.13f,4.15f),2.8f,2.2f);
        Text("ONAR • ÜRET • ÖZEN GÖSTER",new Vector3(-.9f,2.94f,4.14f),2.6f,.6f);

        Area("03 Parts archive");
        Kit("StorageRack",new Vector3(2.30f,0,3.85f),0,true);
        Kit("StorageRack",new Vector3(5.06f,0,2.40f),90,true);
        for(int shelf=0;shelf<4;shelf++)for(int j=0;j<3;j++)
        {
            float x=1.83f+j*.46f,y=.19f+shelf*.44f;
            if(shelf==1 || shelf==2)Kit("OpenPartsBin",new Vector3(x,y,3.79f));
            else Prop("ThirdParty/Kenney/cardboardBoxClosed",new Vector3(x,y,3.83f),.29f, j*7-6);
        }
        Kit("ComponentDrawers",new Vector3(5.03f,.63f,2.10f),90);
        for(int i=0;i<6;i++)Kit("SwitchJar",new Vector3(4.98f,1.52f,1.89f+i*.18f));
        for(int i=0;i<3;i++)Prop("ThirdParty/Kenney/cardboardBoxClosed",new Vector3(5.0f,.19f,2.01f+i*.42f),.30f,90);
        Text("PARÇA ARŞİVİ",new Vector3(2.30f,2.34f,4.16f),1.8f,1.2f);
        for(int i=0;i<4;i++)Text(new[]{"KASALAR","SWITCH / STAB","KEYCAP / KABLO","PCB / ELEKTRONİK"}[i],new Vector3(2.30f,.15f+i*.44f,3.54f),1.2f,.32f);

        Area("04 Electronics and diagnostics");
        Kit("ElectronicsBench",new Vector3(4.97f,0,.05f),90,true);
        MatSurface(new Vector3(4.86f,.912f,.05f),new Vector2(.85f,.49f),90);
        Kit("SolderingStation",new Vector3(4.93f,.912f,.61f),90);
        Kit("Multimeter",new Vector3(4.82f,.912f,-.17f),90);
        Kit("ComponentDrawers",new Vector3(5.17f,1.36f,.13f),90,false,new Vector3(.8f,.8f,.8f));
        Prop("ThirdParty/Refined/ArtisanLamp",new Vector3(5.07f,.92f,-.67f),.47f,90);
        Prop("ThirdParty/Sjolle/plier01",new Vector3(4.76f,.914f,-.48f),.045f,20);
        Text("ELEKTRONİK",new Vector3(5.36f,2.25f,.05f),1.7f,1.15f,90);
        Text("ÖLÇ • TANI • ONAR",new Vector3(5.35f,2.04f,.05f),1.5f,.43f,90);

        Area("05 Color studio");
        Kit("PaintStation",new Vector3(5.0f,0,-2.55f),90,true);
        Kit("PaintBottles",new Vector3(4.86f,.92f,-2.22f));
        Kit("DryingRack",new Vector3(4.89f,.92f,-3.0f),90,false,new Vector3(.68f,.68f,.68f));
        Prop("ThirdParty/MrEliptik/pencil_holder",new Vector3(4.78f,.92f,-2.73f),.14f,0);
        Kit("BrushSet",new Vector3(5.12f,.92f,-2.71f),90);
        Text("RENK STÜDYOSU",new Vector3(5.37f,2.64f,-2.55f),1.8f,1.1f,90);

        Area("06 Orders and design desk");
        Kit("OrderDesk",new Vector3(.40f,0,-3.62f),180,true);
        Prop("ThirdParty/Refined/RetroComputer",new Vector3(.54f,.775f,-3.62f),.55f,0);
        Prop("ThirdParty/KayKit/chair_stool_wood",new Vector3(.40f,0,-2.69f),.53f,180,true);
        Prop("ThirdParty/Refined/ArtisanLamp",new Vector3(-.22f,.78f,-3.80f),.47f,180);
        Prop("ThirdParty/MrEliptik/speaker_bookshelf",new Vector3(1.02f,.78f,-3.80f),.19f,180);
        Prop("ThirdParty/Refined/GlazedWorkshopMug",new Vector3(-.15f,.78f,-3.36f),.10f,0);
        Prop("ThirdParty/KayKit/book_set",new Vector3(1.00f,.78f,-3.48f),.10f,0);
        Kit("WallShelf",new Vector3(.36f,2.03f,-4.00f),180);
        Prop("ThirdParty/KayKit/pictureframe_standing_A",new Vector3(.75f,2.035f,-3.94f),.18f,180);
        Prop("ThirdParty/Kenney/radio",new Vector3(-.15f,2.035f,-3.98f),.19f,180);
        Kit("Paperwork",new Vector3(-.16f,.781f,-3.54f),18);
        Text("SİPARİŞ & TASARIM",new Vector3(.4f,2.55f,-4.17f),2f,1.15f,180);
        Text("küçük atölye / büyük özen",new Vector3(.4f,2.36f,-4.16f),2f,.49f,180);

        Area("07 Receiving and shipping");
        Kit("PackingBench",new Vector3(-4.92f,0,-.1f),-90,true);
        Prop("ThirdParty/Refined/ComponentPackageBody",new Vector3(-4.84f,.92f,-.1f),.22f,90);
        Prop("ThirdParty/Kenney/cardboardBoxClosed",new Vector3(-4.86f,.29f,-.36f),.34f,0);
        Prop("ThirdParty/Kenney/cardboardBoxClosed",new Vector3(-4.86f,.29f,.28f),.28f,0);
        Kit("StorageRack",new Vector3(-5.05f,0,-2.68f),-90,true);
        for(int i=0;i<4;i++)
        {
            Prop("ThirdParty/Kenney/cardboardBoxClosed",new Vector3(-5.02f,.19f+i*.44f,-2.9f),.28f,i*11);
            Prop("ThirdParty/Kenney/cardboardBoxOpen",new Vector3(-5.02f,.19f+i*.44f,-2.4f),.25f,90);
        }
        Prop("ThirdParty/Kenney/cardboardBoxOpen",new Vector3(-4.15f,0,-3.56f),.40f,12,true);
        Kit("WorkApron",new Vector3(-2.23f,.93f,-4.15f),180);
        Kit("CoatHooks",new Vector3(-2.23f,1.73f,-4.13f),180);
        Kit("OrderDesk",new Vector3(-1.87f,0,-3.7f),180,true,new Vector3(.65f,1.05f,.85f));
        Kit("Paperwork",new Vector3(-1.78f,.815f,-3.59f));
        Prop("ThirdParty/Kenney/cardboardBoxClosed",new Vector3(-2.08f,.815f,-3.72f),.20f,8);
        Kit("PackingTape",new Vector3(-4.85f,.915f,.44f));
        Kit("NoticeBoard",new Vector3(-5.35f,1.52f,-3.76f),-90);
        Text("TESLİM / KARGO",new Vector3(-5.35f,2.38f,-2.68f),1.85f,1.1f,-90);
        Text("MERHABA!",new Vector3(-3.40f,2.60f,-4.14f),1.35f,.8f,180);

        Area("08 Interior layers and workshop details");
        Kit("ServiceCart",new Vector3(-2.7f,0,-.55f),-12,true);
        Kit("OpenPartsBin",new Vector3(-2.85f,.834f,-.57f),-12);
        Kit("SwitchJar",new Vector3(-2.43f,.834f,-.53f));
        Prop("ThirdParty/Kenney/cardboardBoxClosed",new Vector3(-2.72f,.50f,-.55f),.22f);
        Prop("ThirdParty/QuinGS/cozy_PottedFlowers",new Vector3(-5.19f,1.17f,2.41f),.45f);
        Prop("ThirdParty/KayKit/cactus_medium_A",new Vector3(-5.15f,1.17f,-1.31f),.30f);
        Prop("ThirdParty/KayKit/cactus_small_A",new Vector3(2.85f,1.98f,3.85f),.24f);
        Kit("PendantLamp",new Vector3(-.9f,2.78f,2.3f));
        Kit("PendantLamp",new Vector3(.6f,2.84f,-1.70f));

        Area("09 Window courtyard — not playable");
        Kit("CourtyardGround",new Vector3(-17,-.08f,0));
        for(int i=0;i<4;i++)Kit("CourtyardTree",new Vector3(-9-i%2*3,0,-5+i*3.1f),i*43,false,Vector3.one*(.85f+i*.09f));
        Prop("ThirdParty/KenneyCity/building-a",new Vector3(-18,0,-7),6f,90);
        Prop("ThirdParty/KenneyCity/building-b",new Vector3(-22,0,2),7f,90);
        Prop("ThirdParty/KenneyCity/building-d",new Vector3(-17,0,10),5.5f,90);

        ConfigureLighting();
        Area("10 Walk-only player");
        var player=new GameObject("Workshop visitor",typeof(CharacterController),typeof(WorkshopWalker));player.transform.SetParent(area);player.transform.position=new Vector3(-3.35f,.06f,-3.3f);
        player.transform.rotation=Quaternion.Euler(0,22,0);
        var cc=player.GetComponent<CharacterController>();cc.height=1.8f;cc.radius=.24f;cc.center=new Vector3(0,.9f,0);cc.stepOffset=.24f;cc.skinWidth=.025f;
        var cameraObject=new GameObject("Player camera",typeof(Camera),typeof(AudioListener),typeof(HDAdditionalCameraData));cameraObject.transform.SetParent(player.transform,false);cameraObject.transform.localPosition=new Vector3(0,1.65f,0);
        var camera=cameraObject.GetComponent<Camera>();camera.tag="MainCamera";camera.nearClipPlane=.06f;camera.farClipPlane=100;camera.fieldOfView=66;
        player.GetComponent<WorkshopWalker>().playerCamera=camera;
        var hd=camera.GetComponent<HDAdditionalCameraData>();hd.antialiasing=HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
        hd.volumeLayerMask=1;hd.volumeAnchorOverride=camera.transform;
        hd.customRenderingSettings=true;
        foreach(var field in new[]{FrameSettingsField.Volumetrics,FrameSettingsField.SSGI,FrameSettingsField.SSAO,FrameSettingsField.ContactShadows})
        {hd.renderingPathCustomFrameSettings.SetEnabled(field,true);hd.renderingPathCustomFrameSettingsOverrideMask.mask[(uint)field]=true;}
        AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),ScenePath);
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        File.WriteAllText("Logs/walk-environment-created.txt","Created "+DateTime.UtcNow.ToString("O")+"\n"+Application.dataPath+"\n"+Bounds(GameObject.Find("WorkshopShell")));
        Debug.Log("WALKABLE_WORKSHOP_CREATED");
    }
    public static void ConfigureLighting()
    {
        Area("HDRP daylight and practical fixtures");
        var old=GameObject.Find("Walkable workshop global volume");if(old)Object.DestroyImmediate(old);
        var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Root+"/WorkshopAtmosphere.asset");
        if(!profile)
        {
            AssetDatabase.CopyAsset("Assets/Settings/LittleSwitchHDRP/WorkshopAtmosphere.asset",Root+"/WorkshopAtmosphere.asset");
            profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Root+"/WorkshopAtmosphere.asset");
        }
        profile.TryGet<Exposure>(out var exposure);exposure.fixedExposure.Override(10f);
        profile.TryGet<GradientSky>(out var sky);sky.exposure.Override(9.8f);sky.top.Override(Hex("7B9EBA"));sky.middle.Override(Hex("C0CFD0"));sky.bottom.Override(Hex("76867B"));
        profile.TryGet<Fog>(out var fog);fog.meanFreePath.Override(90);fog.maximumHeight.Override(4);fog.depthExtent.Override(24);fog.anisotropy.Override(.48f);fog.globalLightProbeDimmer.Override(.2f);fog.volumeSliceCount.Override(96);fog.screenResolutionPercentage.Override(20);
        profile.TryGet<HDShadowSettings>(out var shadow);shadow.maxShadowDistance.Override(35);
        profile.TryGet<ScreenSpaceAmbientOcclusion>(out var ao);ao.intensity.Override(.45f);ao.radius.Override(.20f);
        profile.TryGet<Bloom>(out var bloom);bloom.intensity.Override(.035f);
        var global=new GameObject("Walkable workshop global volume").AddComponent<Volume>();global.transform.SetParent(area);global.isGlobal=true;global.priority=20;global.sharedProfile=profile;
        foreach(var light in Object.FindObjectsByType<Light>())Object.DestroyImmediate(light.gameObject);
        var sun=new GameObject("Golden afternoon sun").AddComponent<Light>();sun.transform.SetParent(area);sun.type=LightType.Directional;sun.transform.rotation=Quaternion.Euler(18.5f,72,0);
        var sunlight=sun.gameObject.AddComponent<HDAdditionalLightData>();sun.lightUnit=LightUnit.Lux;sun.intensity=30000;sun.color=Color.white;sun.useColorTemperature=true;sun.colorTemperature=5100;sun.shadows=LightShadows.Soft;
        sunlight.angularDiameter=1.25f;sunlight.volumetricDimmer=1.6f;sunlight.SetShadowResolution(2048);sunlight.useContactShadow.useOverride=true;sunlight.useContactShadow.@override=true;RenderSettings.sun=sun;
        AreaLight("Main bench pendant",new Vector3(-.9f,2.72f,2.3f),650,new Vector2(.40f,.40f));
        AreaLight("Electronics task lamp",new Vector3(4.84f,1.50f,-.61f),180,new Vector2(.18f,.12f));
        AreaLight("Orders task lamp",new Vector3(-.2f,1.30f,-3.65f),150,new Vector2(.18f,.12f));
        var atmosphere=GameObject.Find("Window air")??new GameObject("Window air");atmosphere.transform.SetParent(area);atmosphere.transform.position=new Vector3(-2.1f,1.9f,.4f);
        var local=atmosphere.GetComponent<LocalVolumetricFog>()??atmosphere.AddComponent<LocalVolumetricFog>();local.parameters.size=new Vector3(6.6f,2.5f,5.0f);local.parameters.meanFreePath=28;local.parameters.albedo=new Color(.9f,.92f,.95f);local.parameters.positiveFade=Vector3.one*.25f;local.parameters.negativeFade=Vector3.one*.25f;
        EditorUtility.SetDirty(profile);AssetDatabase.SaveAssets();
    }
    static void AreaLight(string name,Vector3 position,float lumens,Vector2 size)
    {
        var light=new GameObject(name).AddComponent<Light>();light.transform.SetParent(area);light.transform.SetPositionAndRotation(position,Quaternion.Euler(90,0,0));light.type=LightType.Rectangle;
        var hd=light.gameObject.AddComponent<HDAdditionalLightData>();light.areaSize=size;light.lightUnit=LightUnit.Lumen;light.intensity=lumens;light.useColorTemperature=true;light.colorTemperature=3800;light.range=4;light.shadows=LightShadows.Soft;hd.volumetricDimmer=.03f;hd.SetShadowResolution(512);
    }
}
