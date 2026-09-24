using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using LittleSwitch;
using Object=UnityEngine.Object;
public static class WorkshopFocusPass {
 public const string ScenePath="Assets/Scenes/WorkshopFocused.unity";
 const string Root="Assets/WorkshopRevision/Focus";
 public static Vector3 Narrow(Vector3 p){p.x-=Mathf.Clamp01((p.x-1)/3)*1.2f;return p;}
 public static Vector3 Map(Vector3 p)=>WideWorkshopRestore.Map(Narrow(WorkshopPolishPass.Compact(p)));
 static Color Hex(string h){ColorUtility.TryParseHtmlString("#"+h,out var c);return c;}
 static Material Finish(string name,string color,string texture,float smooth=.2f,float bump=.16f){
  var path=Root+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(Shader.Find("HDRP/Lit"));AssetDatabase.CreateAsset(m,path);}
  m.color=Hex(color);if(texture!=null)m.SetTexture("_BaseColorMap",AssetDatabase.LoadAssetAtPath<Texture2D>(texture));m.SetFloat("_Smoothness",smooth);
  m.SetTexture("_NormalMap",AssetDatabase.LoadAssetAtPath<Texture2D>(name.Contains("Paint")?"Assets/LittleSwitch/Art/Painterly/PlasterRelief.png":"Assets/LittleSwitch/ThirdParty/PolyHaven/fine_grained_wood/nor_gl.png"));m.SetFloat("_NormalScale",bump);HDMaterial.ValidateMaterial(m);EditorUtility.SetDirty(m);return m;
 }
 static GameObject Model(string name,Vector3 position,float scale=4.3f){var go=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WorkshopRevision/Models/"+name+".fbx"));go.transform.position=position;go.transform.localScale=Vector3.one*scale;return go;}
 public static void Apply(){
  Directory.CreateDirectory(Root);AssetDatabase.Refresh();WorkshopPolishPass.Open();
  var floor=Finish("SmokedOakFloor","75665A","Assets/LittleSwitch/ThirdParty/PolyHaven/fine_grained_wood/Diffuse.png",.12f,.28f);
  var bench=Finish("OiledWorkbench","C4B9A1","Assets/LittleSwitch/Art/Painterly/WalnutPainted.png",.22f,.18f);
  var timber=Finish("AgedStructuralTimber","75685B","Assets/WorkshopRevision/Textures/PaintedWalnut.png",.12f);
  var ceiling=Finish("CeilingBoards","9D9385","Assets/WorkshopRevision/Textures/PaintedWalnut.png",.13f);
  var paint=Finish("WarmGraphitePaint","515455",null,.24f,.07f);
  var panel=Finish("PuttyPanelPaint","999083",null,.18f,.08f);
  var texture=new Texture2D(128,128,TextureFormat.RGBA32,false);texture.name="Subtle powder coat variation";
  for(int y=0;y<128;y++)for(int x=0;x<128;x++){float v=.89f+.11f*Mathf.PerlinNoise(x*.09f,y*.09f);texture.SetPixel(x,y,new Color(v,v,v,1));}texture.Apply();AssetDatabase.CreateAsset(texture,Root+"/PowderCoat.asset");paint.SetTexture("_BaseColorMap",texture);panel.SetTexture("_BaseColorMap",texture);
  var shell=GameObject.Find("WorkshopShell");var frame=GameObject.Find("Approved wide workshop coordinate frame").transform;int n=0;
  foreach(var mf in Object.FindObjectsByType<MeshFilter>()){
   bool architecture=mf.transform.IsChildOf(shell.transform)||mf.name.Contains("WideWorkshopWallFinish")||mf.name.Contains("Continuous dark subfloor");
   if(architecture){var mesh=Object.Instantiate(mf.sharedMesh);var v=mesh.vertices;for(int i=0;i<v.Length;i++){var p=WideWorkshopRestore.MapInverse(mf.transform.TransformPoint(v[i]));v[i]=mf.transform.InverseTransformPoint(WideWorkshopRestore.Map(Narrow(p)));}mesh.vertices=v;mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Root+"/Architecture"+(n++)+".asset");mf.sharedMesh=mesh;var mc=mf.GetComponent<MeshCollider>();if(mc)mc.sharedMesh=mesh;}
  }
  foreach(Transform area in frame){if(area.name.Contains("WideWorkshopWallFinish"))continue;foreach(Transform prop in area){if(prop==shell.transform||prop.name=="Continuous dark subfloor")continue;prop.position=WideWorkshopRestore.Map(Narrow(WideWorkshopRestore.MapInverse(prop.position)));}}
  foreach(var go in EditorSceneManager.GetActiveScene().GetRootGameObjects())if(go.transform!=frame)go.transform.position=WideWorkshopRestore.Map(Narrow(WideWorkshopRestore.MapInverse(go.transform.position)));
  foreach(var r in Object.FindObjectsByType<Renderer>()){
   r.sharedMaterials=r.sharedMaterials.Select(m=>{
    if(!m)return m;if(m.name=="Floor")return floor;
    if(m.name.Contains("Wood"))return r.name.Contains("WideWorkshopWallFinish")?ceiling:r.transform.IsChildOf(shell.transform)?timber:r.name.Contains("MainWorkbench")?bench:timber;
    if(m.name=="Sage"||m.name=="Teal")return r.name.Contains("Pegboard")||r.name.Contains("ComponentDrawer")?panel:paint;
    return m;
   }).ToArray();
  }
  foreach(var p in Object.FindObjectsByType<PartsPackage>()){p.deskPosition=new Vector3(p.keycaps?1.75f:-1.75f,4.09f,-.55f);p.Present(false);}
  foreach(var t in Object.FindObjectsByType<WorkbenchTool>())if(t.kind==WorkbenchTool.Kind.Puller)t.heldVisualPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WorkshopRevision/Models/WireKeycapPuller.fbx");
  var cabinet=Model("BenchLowerCabinet",Map(new Vector3(-.45f,.30f,2.96f)));foreach(var r in cabinet.GetComponentsInChildren<Renderer>())r.sharedMaterial=r.name.Contains("Sage")?panel:paint;
  var box=GameObject.Find("Detailed workshop toolbox");if(box)box.SetActive(false);
  var blinds=Model("FittedVenetianBlinds",WideWorkshopRestore.Map(Vector3.zero));
  foreach(var r in blinds.GetComponentsInChildren<Renderer>())r.sharedMaterial=Finish("BlindPaint","C9C0AA",null,.16f,.045f);
  // The southern window only crosses a small part of the previous depth reduction.
  foreach(var mf in blinds.GetComponentsInChildren<MeshFilter>()){var mesh=Object.Instantiate(mf.sharedMesh);var v=mesh.vertices;for(int i=0;i<v.Length;i++){var p=WideWorkshopRestore.MapInverse(mf.transform.TransformPoint(v[i]));v[i]=mf.transform.InverseTransformPoint(WideWorkshopRestore.Map(WorkshopPolishPass.Compact(p)));}mesh.vertices=v;mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Root+"/Blind"+(n++)+".asset");mf.sharedMesh=mesh;}
  var sun=Object.FindObjectsByType<Light>().First(l=>l.type==LightType.Directional);sun.GetComponent<HDAdditionalLightData>().angularDiameter=1.2f;sun.GetComponent<HDAdditionalLightData>().volumetricDimmer=.7f;
  AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),ScenePath);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
  File.WriteAllText("Logs/focus-pass.txt","Room width reduced1.2m by moving east areas without scaling furniture. Dark smoked floor, oiled bench, aged beams, muted textured graphite/putty metal. Fitted tilted real blind slats, under-bench cabinet. Centered board and two-sided supply packages.\n");
 }
 public static void Open()=>EditorSceneManager.OpenScene(ScenePath);
 public static void AlignBlinds(){Open();var old=GameObject.Find("FittedVenetianBlinds");if(old)Object.DestroyImmediate(old);var blinds=Model("FittedVenetianBlinds",WideWorkshopRestore.Map(Vector3.zero));int i=0;foreach(var mf in blinds.GetComponentsInChildren<MeshFilter>()){var mesh=Object.Instantiate(mf.sharedMesh);var v=mesh.vertices;for(int j=0;j<v.Length;j++){var p=WideWorkshopRestore.MapInverse(mf.transform.TransformPoint(v[j]));v[j]=mf.transform.InverseTransformPoint(WideWorkshopRestore.Map(WorkshopPolishPass.Compact(p)));}mesh.vertices=v;mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Root+"/AlignedBlind"+(i++)+".asset");mf.sharedMesh=mesh;mf.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Root+"/BlindPaint.mat");}foreach(var fog in Object.FindObjectsByType<LocalVolumetricFog>())fog.parameters.meanFreePath=180;EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());}
 public static void Refine(){Open();foreach(var p in Object.FindObjectsByType<PartsPackage>())p.deskPosition=new Vector3(p.keycaps?1.60f:-1.60f,4.09f,-.55f);var sun=Object.FindObjectsByType<Light>().First(l=>l.type==LightType.Directional).GetComponent<HDAdditionalLightData>();sun.angularDiameter=.6f;sun.volumetricDimmer=1;foreach(var fog in Object.FindObjectsByType<LocalVolumetricFog>())fog.parameters.meanFreePath=260;EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());}
 public static void BuildWindows(){Open();var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="../Builds/WalkableWorkshop/Little Switch Workshop.exe",target=BuildTarget.StandaloneWindows64});File.WriteAllText("Logs/focus-build.txt",r.summary.result+" errors="+r.summary.totalErrors);if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");}
}
