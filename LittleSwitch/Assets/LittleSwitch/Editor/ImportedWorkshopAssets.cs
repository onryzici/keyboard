using System;using System.IO;using System.Linq;using System.Collections.Generic;using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;
public static class ImportedWorkshopAssets {
 const string Root="Assets/LittleSwitch/ThirdParty";
 static readonly string[] Kay={"table_medium_long","shelf_A_big","shelf_B_large","book_set","cactus_small_A","cactus_medium_A","pictureframe_standing_A"};
 static readonly string[] Tools={"screwdriver01","screwdriver02","plier01","cutter01","vernier01","ruler01"};
 public static void Import(){
 CopyGroup("KayKit","/tmp/little-switch-import/kaykit",Kay);CopyGroup("Sjolle","/tmp/little-switch-import/tools",Tools);
 CopyGroup("MrEliptik","/tmp/little-switch-import/converted",new[]{"lamp_architect","mug","pencil_holder","pencil","speaker_bookshelf","camera"});
 CopyGroup("QuinGS","/tmp/little-switch-import/converted",new[]{"cozy_TableA","cozy_KitchenCabinetB","cozy_Stool_Wooden","cozy_PottedFlowers"});
 File.Copy("/tmp/little-switch-import/kaykit/furniturebits_texture.png",Root+"/KayKit/furniturebits_texture.png",true);
 File.Copy("/tmp/little-switch-import/kaykit/License.txt",Root+"/KayKit/License.txt",true);File.Copy("/tmp/little-switch-import/office/LICENSE.txt",Root+"/MrEliptik/LICENSE.txt",true);
 AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
 foreach(var path in AssetDatabase.FindAssets("t:Model",new[]{Root}).Select(AssetDatabase.GUIDToAssetPath)){
 var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.isReadable=false;importer.importCameras=false;importer.importLights=false;importer.importAnimation=false;importer.SaveAndReimport();
 string folder=Path.GetDirectoryName(path)+"/Textures";Directory.CreateDirectory(folder);importer.ExtractTextures(folder);
 }
 AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);Debug.Log("SELECTED_MODELS_IMPORTED");
 }
 static void CopyGroup(string group,string source,string[] names){Directory.CreateDirectory(Root+"/"+group);foreach(string n in names)File.Copy(source+"/"+n+".fbx",Root+"/"+group+"/"+n+".fbx",true);}
 static Bounds BoundsOf(GameObject go){var rs=go.GetComponentsInChildren<Renderer>();var b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);return b;}
 static Dictionary<string,Material> materialCache=new Dictionary<string,Material>();
 static void Materials(GameObject go,string group){foreach(var r in go.GetComponentsInChildren<Renderer>()){var arr=r.sharedMaterials;for(int i=0;i<arr.Length;i++){
 var src=arr[i];string key=group+"_"+(src?src.name:"Default").Replace("/","_");
 if(!materialCache.TryGetValue(key,out var mat)){
 string path=Root+"/Materials/"+key+".mat";mat=AssetDatabase.LoadAssetAtPath<Material>(path);
 if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));mat.name=key;Color c=src&&src.HasProperty("_Color")?src.GetColor("_Color"):Color.white;
 Texture texture=src?src.mainTexture:null;
 if(group=="KayKit"){texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/KayKit/furniturebits_texture.png");c=new Color(.88f,.86f,.77f);}
 if(group=="Sjolle"&&!texture){var tex=AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"/Sjolle"});if(tex.Length>0)texture=AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(tex[0]));}
 mat.color=c;mat.mainTexture=texture;mat.SetFloat("_Smoothness",.17f);AssetDatabase.CreateAsset(mat,path);}
 materialCache[key]=mat;
 }arr[i]=mat;}r.sharedMaterials=arr;}}
 static GameObject Place(string group,string name,Transform parent,Vector3 bottom,Vector3 size,Vector3 euler){
 var asset=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/"+group+"/"+name+".fbx");if(!asset)throw new Exception("Missing model "+name);
 var container=new GameObject(group+" · "+name);container.transform.SetParent(parent,false);
 var go=(GameObject)PrefabUtility.InstantiatePrefab(asset,container.transform);go.transform.localPosition=Vector3.zero;go.transform.localRotation=Quaternion.Euler(euler)*asset.transform.localRotation;
 var b=BoundsOf(go);var scale=new Vector3(size.x/b.size.x,size.y/b.size.y,size.z/b.size.z);container.transform.localScale=scale;b=BoundsOf(go);go.transform.position-=new Vector3(b.center.x,b.min.y,b.center.z);container.transform.position=bottom;Materials(go,group);return container;
 }
 public static void PlaceAll(){
 if(EditorApplication.isPlaying)throw new Exception("Exit Play Mode first");Directory.CreateDirectory(Root+"/Materials");AssetDatabase.Refresh();
 var world=GameObject.Find("Little Switch · Workshop");if(!world)throw new Exception("Workshop scene required");
 if(GameObject.Find("Imported artisan props"))throw new Exception("Already placed; adjust existing instances");
 Undo.SetCurrentGroupName("Add free workshop models");var root=new GameObject("Imported artisan props");Undo.RegisterCreatedObjectUndo(root,"Imported props");
 var hideNames=new[]{"Walnut worktop","Wood inlay","Workbench leg","Display shelf","Lamp base","Lamp shade","Warm diffuser","Ceramic mug","Tea","Mug handle","Screwdriver grip","A little green"};
 foreach(Transform tr in world.transform){bool hide=hideNames.Contains(tr.name);if(tr.name=="Stem"){var p=tr.position;hide=(p.z>1.7f&&p.y>2.1f&&p.x>-1.3f&&p.x<1.8f)||(p.x>2&&p.x<3.2f&&p.y>2.1f&&p.z<2);}
 if(hide){Undo.RecordObject(tr.gameObject,"Hide replaced placeholder");tr.gameObject.SetActive(false);}}
 Place("KayKit","table_medium_long",root.transform,new Vector3(0,0,0),new Vector3(11.5f,1.8f,5.5f),Vector3.zero);
 for(int s=0;s<3;s++)Place("KayKit","shelf_A_big",root.transform,new Vector3(4.25f,2.43f+s*1.25f,4),new Vector3(3.4f,.14f,1.16f),Vector3.zero);
 Place("KayKit","book_set",root.transform,new Vector3(-2.6f,2.74f,3.93f),new Vector3(.85f,.6f,.39f),new Vector3(0,180,0));
 Place("KayKit","cactus_medium_A",root.transform,new Vector3(-3.88f,2.74f,3.85f),new Vector3(.65f,.92f,.65f),Vector3.zero);
 Place("QuinGS","cozy_PottedFlowers",root.transform,new Vector3(5.12f,5.08f,4),new Vector3(.56f,.84f,.56f),Vector3.zero);
 Place("KayKit","pictureframe_standing_A",root.transform,new Vector3(-.2f,2.74f,3.9f),new Vector3(.64f,.73f,.28f),new Vector3(0,180,0));
 Place("QuinGS","cozy_KitchenCabinetB",root.transform,new Vector3(5,0,3.75f),new Vector3(2.05f,2.05f,1.2f),Vector3.zero);
 Place("QuinGS","cozy_Stool_Wooden",root.transform,new Vector3(-4.8f,.08f,-2.8f),new Vector3(1.05f,1.14f,1.05f),new Vector3(0,15,0));
 Place("MrEliptik","lamp_architect",root.transform,new Vector3(2.85f,1.81f,1.5f),new Vector3(1.6f,1.94f,1.2f),new Vector3(0,-45,0));
 Place("MrEliptik","mug",root.transform,new Vector3(3.62f,1.81f,.7f),new Vector3(.58f,.58f,.48f),new Vector3(0,15,0));
 Place("MrEliptik","pencil_holder",root.transform,new Vector3(2,1.81f,1.82f),new Vector3(.42f,.61f,.42f),Vector3.zero);
 Place("MrEliptik","speaker_bookshelf",root.transform,new Vector3(-5.35f,1.81f,1.55f),new Vector3(.46f,.76f,.5f),new Vector3(0,180,0));
 Place("MrEliptik","camera",root.transform,new Vector3(1.15f,2.74f,3.9f),new Vector3(.75f,.52f,.49f),new Vector3(0,160,0));
 Place("Sjolle","screwdriver01",root.transform,new Vector3(-1.05f,2.21f,1.97f),new Vector3(.15f,.66f,.15f),Vector3.zero);
 Place("Sjolle","screwdriver02",root.transform,new Vector3(-.37f,2.21f,1.97f),new Vector3(.15f,.7f,.15f),Vector3.zero);
 Place("Sjolle","plier01",root.transform,new Vector3(.38f,2.22f,1.97f),new Vector3(.3f,.62f,.12f),Vector3.zero);
 Place("Sjolle","cutter01",root.transform,new Vector3(3.64f,1.82f,-1.4f),new Vector3(.16f,.08f,.73f),new Vector3(90,0,-12));
 Place("Sjolle","vernier01",root.transform,new Vector3(3.98f,1.82f,-1.57f),new Vector3(.29f,.08f,.88f),new Vector3(90,0,10));
 Place("Sjolle","ruler01",root.transform,new Vector3(1.3f,1.81f,1.1f),new Vector3(1.7f,.035f,.16f),new Vector3(90,90,0));
 // Warm matte ceramic instead of default office black/white.
 foreach(var r in root.GetComponentsInChildren<Renderer>())if(r.transform.IsChildOf(root.transform.Find("MrEliptik · mug")))foreach(var mat in r.sharedMaterials){mat.color=LittleSwitch.SoftShapes.C("CBA675");EditorUtility.SetDirty(mat);}
 AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(world.scene);EditorSceneManager.SaveScene(world.scene);Debug.Log("FREE_MODELS_PLACED: "+root.transform.childCount);
 }
}
