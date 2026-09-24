using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Object=UnityEngine.Object;

public static class WorkshopPolishPass {
 public const string ScenePath="Assets/Scenes/WorkshopPolished.unity";
 const string Root="Assets/WorkshopRevision/Polish";
 public static Vector3 Compact(Vector3 p){p.z+=Mathf.Clamp01((-p.z-1)/2)*.9f;return p;}
 public static Vector3 Map(Vector3 p)=>WideWorkshopRestore.Map(Compact(p));
 static Material Wood(string name) {
  string path=Root+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);
  if(!m){m=new Material(Shader.Find("HDRP/Lit"));AssetDatabase.CreateAsset(m,path);}
  m.color=name=="Floor"?new Color(.83f,.77f,.68f):name.Contains("Dark")?new Color(.63f,.55f,.47f):new Color(.98f,.88f,.74f);
  m.SetTexture("_BaseColorMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/WorkshopRevision/Textures/PaintedWalnut.png"));
  m.SetTexture("_NormalMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/ThirdParty/PolyHaven/fine_grained_wood/nor_gl.png"));
  m.SetFloat("_NormalScale",name=="Floor"?.28f:.2f);m.SetFloat("_Smoothness",.23f);HDMaterial.ValidateMaterial(m);EditorUtility.SetDirty(m);return m;
 }
 static void Detail(GameObject model,Vector3 p,float yaw=0) {
  var go=(GameObject)PrefabUtility.InstantiatePrefab(model);go.transform.position=Map(p);go.transform.rotation=Quaternion.Euler(0,yaw,0);go.transform.localScale=Vector3.one*4.3f;
  foreach(var r in go.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>m.name.StartsWith("HoneyWood")?Wood("HoneyWood"):AssetDatabase.LoadAssetAtPath<Material>("Assets/WorkshopWalk/Materials/"+m.name.Split('.')[0]+".mat")??m).ToArray();
 }
 public static void Apply() {
  Directory.CreateDirectory(Root);AssetDatabase.Refresh();WideWorkshopRestore.Open();
  var shell=GameObject.Find("WorkshopShell");var frame=GameObject.Find("Approved wide workshop coordinate frame").transform;
  int meshId=0;
  foreach(var mf in Object.FindObjectsByType<MeshFilter>()) {
   var r=mf.GetComponent<Renderer>();if(!r||!mf.sharedMesh)continue;
   bool architecture=mf.transform.IsChildOf(shell.transform)||mf.name.Contains("WideWorkshopWallFinish")||mf.name.Contains("Continuous dark subfloor");
   bool wood=r.sharedMaterials.Any(m=>m&&(m.name.Contains("Wood")||m.name=="Floor"));
   if(!architecture&&!wood)continue;
   var mesh=Object.Instantiate(mf.sharedMesh);mesh.name=mf.sharedMesh.name+" detailed";
   var vertices=mesh.vertices;var normals=mesh.normals;var uv=mesh.uv;
   for(int i=0;i<vertices.Length;i++) {
    if(architecture){var metric=WideWorkshopRestore.MapInverse(mf.transform.TransformPoint(vertices[i]));vertices[i]=mf.transform.InverseTransformPoint(WideWorkshopRestore.Map(Compact(metric)));}
    if(wood){var v=vertices[i];var n=normals[i];float unit=mf.transform.lossyScale.x/4.3f;
     uv[i]=Mathf.Abs(n.y)>.6f?new Vector2(v.x*unit/.65f,v.z*unit/1.4f):Mathf.Abs(n.x)>.6f?new Vector2(v.z*unit/.65f,v.y*unit/1.4f):new Vector2(v.x*unit/.65f,v.y*unit/1.4f);}
   }
   mesh.vertices=vertices;mesh.uv=uv;mesh.RecalculateBounds();mesh.RecalculateTangents();
   string path=Root+"/Surface"+(meshId++)+".asset";AssetDatabase.DeleteAsset(path);AssetDatabase.CreateAsset(mesh,path);mf.sharedMesh=mesh;
   var mc=mf.GetComponent<MeshCollider>();if(mc)mc.sharedMesh=mesh;
   if(wood)r.sharedMaterials=r.sharedMaterials.Select(m=>m&&(m.name.Contains("Wood")||m.name=="Floor")?Wood(m.name.Contains("Dark")?"DarkWood":m.name=="Floor"?"Floor":"HoneyWood"):m).ToArray();
  }
  // Remove space from the front aisle, preserving furniture scale and the assembly bench anchor.
  foreach(Transform area in frame) {
   if(area.name.Contains("WideWorkshopWallFinish"))continue;
   foreach(Transform prop in area){if(prop==shell.transform||prop.name=="Continuous dark subfloor")continue;prop.position=Map(WideWorkshopRestore.MapInverse(prop.position));}
  }
  foreach(var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())if(root.transform!=frame && root.name!="Keyboard design and customer orders" && root.name!="Keyboard design camera")root.transform.position=Map(WideWorkshopRestore.MapInverse(root.transform.position));
  foreach(string tex in new[]{"Assets/WorkshopRevision/Textures/PaintedWalnut.png","Assets/LittleSwitch/ThirdParty/PolyHaven/fine_grained_wood/nor_gl.png","Assets/LittleSwitch/Art/CuttingMat.png"}){
   var t=(TextureImporter)AssetImporter.GetAtPath(tex);t.anisoLevel=16;t.filterMode=FilterMode.Trilinear;t.maxTextureSize=4096;t.textureCompression=TextureImporterCompression.CompressedHQ;t.mipMapBias=-.3f;t.SaveAndReimport();}
  QualitySettings.anisotropicFiltering=AnisotropicFiltering.ForceEnable;
  foreach(var name in new[]{"RoutedPartsTray","CoiledKeyboardCable"})Detail(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WorkshopRevision/Models/"+name+".fbx"),name=="RoutedPartsTray"?new Vector3(-.87f,.944f,3.17f):new Vector3(-.08f,.944f,3.12f),name=="RoutedPartsTray"?0:15);
  // Existing detailed licensed toolbox: a coherent tool cluster alongside the bench.
  var tool=AssetDatabase.FindAssets("metal_toolbox t:Model").Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault();
  if(tool!=null){var go=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(tool));go.name="Detailed workshop toolbox";var rs=go.GetComponentsInChildren<Renderer>();var b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);go.transform.localScale*=.38f*4.3f/b.size.x;b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);go.transform.position+=Map(new Vector3(-1.5f,.26f,2.8f))-new Vector3(b.center.x,b.min.y,b.center.z);}
  var profile=Object.FindObjectsByType<Volume>().First(v=>v.isGlobal).sharedProfile;
  var grain=profile.TryGet<FilmGrain>(out var g)?g:profile.Add<FilmGrain>(true);grain.type.Override(FilmGrainLookup.Thin1);grain.intensity.Override(.075f);grain.response.Override(.8f);EditorUtility.SetDirty(profile);
  MakeDust();AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),ScenePath);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
  File.WriteAllText("Logs/polish-surfaces.txt",$"{meshId} mesh surfaces remapped; physical texture scale, 16x anisotropy. Room front moved inward .9m, furniture kept at original scale. Film grain .075. Authored tray, coiled cable, sculpted keycaps. Light-reactive dust mesh particles.\n");
 }
 static void MakeDust() {
  var go=new GameObject("Sunlit drifting dust");go.transform.position=Map(new Vector3(-2.3f,1.7f,.9f));
  var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
  var main=ps.main;main.loop=true;main.prewarm=true;main.duration=18;main.startLifetime=new ParticleSystem.MinMaxCurve(14,22);main.startSpeed=.008f*4.3f;main.startSize=new ParticleSystem.MinMaxCurve(.003f*4.3f,.006f*4.3f);main.maxParticles=220;main.simulationSpace=ParticleSystemSimulationSpace.World;
  var emission=ps.emission;emission.rateOverTime=10;
  var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(3.7f,2.1f,2.4f)*4.3f;
  var noise=ps.noise;noise.enabled=true;noise.strength=.025f;noise.frequency=.15f;noise.scrollSpeed=.04f;
  var mesh=new Mesh();mesh.vertices=new[]{Vector3.up,Vector3.right,Vector3.forward,Vector3.left,Vector3.back,Vector3.down};mesh.triangles=new[]{0,2,1,0,3,2,0,4,3,0,1,4,5,1,2,5,2,3,5,3,4,5,4,1};mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Root+"/DustMesh.asset");
  var mat=new Material(Shader.Find("HDRP/Lit"));mat.color=new Color(.7f,.65f,.5f);mat.SetFloat("_Smoothness",.12f);HDMaterial.ValidateMaterial(mat);AssetDatabase.CreateAsset(mat,Root+"/Dust.mat");
  var renderer=ps.GetComponent<ParticleSystemRenderer>();renderer.renderMode=ParticleSystemRenderMode.Mesh;renderer.mesh=mesh;renderer.sharedMaterial=mat;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=true;ps.Play();
 }
 public static void Open()=>EditorSceneManager.OpenScene(ScenePath);
 public static void Finish(){Open();var dust=GameObject.Find("Sunlit drifting dust").GetComponent<ParticleSystem>();dust.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var main=dust.main;main.startSize=new ParticleSystem.MinMaxCurve(.002f*4.3f,.0035f*4.3f);main.maxParticles=90;var e=dust.emission;e.rateOverTime=4;var shape=dust.shape;shape.scale=new Vector3(2.6f,1.5f,1.8f)*4.3f;dust.transform.position=Map(new Vector3(-3.2f,1.7f,1.0f));dust.Play();var box=GameObject.Find("Detailed workshop toolbox");if(box)foreach(var r in box.GetComponentsInChildren<Renderer>())r.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/LittleSwitch/ThirdParty/PolyHaven/metal_toolbox/Workshop.mat");EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());}
 public static void BuildWindows(){Open();var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="../Builds/WalkableWorkshop/Little Switch Workshop.exe",target=BuildTarget.StandaloneWindows64});File.WriteAllText("Logs/polish-build.txt",r.summary.result+" errors="+r.summary.totalErrors);if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");}
}

