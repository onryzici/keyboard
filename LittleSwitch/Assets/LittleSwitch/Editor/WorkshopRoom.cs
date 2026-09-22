using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using System;using System.IO;using System.Linq;using LittleSwitch;using static LittleSwitch.SoftShapes;
public static class WorkshopRoom {
 static Transform root;const string Dir="Assets/LittleSwitch/Art";static Material plaster,wood;
 static Material Surface(string id,string name,string tint,bool detail,float normal){
 var folder="Assets/LittleSwitch/ThirdParty/PolyHaven/"+id+"/";
 foreach(var n in new[]{"Diffuse","nor_gl","Mask"}){var imp=(TextureImporter)AssetImporter.GetAtPath(folder+n+".png");imp.maxTextureSize=1024;imp.anisoLevel=8;imp.wrapMode=TextureWrapMode.Repeat;if(n=="nor_gl")imp.textureType=TextureImporterType.NormalMap;if(n=="Mask")imp.sRGBTexture=false;imp.SaveAndReimport();}
 var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name,color=C(tint)};
 var diffuse=AssetDatabase.LoadAssetAtPath<Texture2D>(folder+"Diffuse.png");
 if(detail){m.SetTexture("_DetailAlbedoMap",diffuse);m.SetFloat("_DetailAlbedoMapScale",.55f);m.SetFloat("_DetailNormalMapScale",0);m.EnableKeyword("_DETAIL_SCALED");}else m.mainTexture=diffuse;
 m.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(folder+"nor_gl.png"));m.SetFloat("_BumpScale",normal);m.EnableKeyword("_NORMALMAP");
 var mask=AssetDatabase.LoadAssetAtPath<Texture2D>(folder+"Mask.png");m.SetTexture("_MetallicGlossMap",mask);m.SetTexture("_OcclusionMap",mask);m.EnableKeyword("_METALLICSPECGLOSSMAP");m.EnableKeyword("_OCCLUSIONMAP");m.SetFloat("_Smoothness",.48f);m.SetFloat("_OcclusionStrength",.35f);AssetDatabase.CreateAsset(m,AssetDatabase.GenerateUniqueAssetPath(Dir+"/"+name+".mat"));return m;
 }
 static void UV(GameObject g,Material material,float metersPerTile=3){foreach(var mf in g.GetComponentsInChildren<MeshFilter>()){if(mf.GetComponent<TMPro.TMP_Text>())continue;var mesh=UnityEngine.Object.Instantiate(mf.sharedMesh);var vv=mesh.vertices;var nn=mesh.normals;var uv=new Vector2[vv.Length];for(int i=0;i<vv.Length;i++){var v=mf.transform.TransformPoint(vv[i]);var n=mf.transform.TransformDirection(nn[i]);var a=new Vector3(Mathf.Abs(n.x),Mathf.Abs(n.y),Mathf.Abs(n.z));uv[i]=(a.y>=a.x&&a.y>=a.z?new Vector2(v.x,v.z):a.x>a.z?new Vector2(v.z,v.y):new Vector2(v.x,v.y))/metersPerTile;}mesh.uv=uv;mesh.RecalculateTangents();AssetDatabase.CreateAsset(mesh,AssetDatabase.GenerateUniqueAssetPath(Dir+"/textured-surface.asset"));mf.sharedMesh=mesh;var renderer=mf.GetComponent<Renderer>();if(renderer)renderer.sharedMaterials=Enumerable.Repeat(material,mesh.subMeshCount).ToArray();}}
 static GameObject B(string n,Vector3 p,Vector3 s,Material m,float radius=.025f){var g=Box(n,root,p,s,"FFFFFF",radius);UV(g,m);return g;}
 static Bounds Bounds(GameObject g){var rr=g.GetComponentsInChildren<Renderer>();var b=rr[0].bounds;foreach(var r in rr)b.Encapsulate(r.bounds);return b;}
 static void City(string name,Vector3 bottom,float height,float yaw){var asset=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LittleSwitch/ThirdParty/KenneyCity/"+name+".fbx");var holder=new GameObject("Exterior · "+name);holder.transform.SetParent(root);var go=(GameObject)PrefabUtility.InstantiatePrefab(asset,holder.transform);go.transform.localRotation=Quaternion.Euler(0,yaw,0)*asset.transform.localRotation;var b=Bounds(go);holder.transform.localScale=Vector3.one*(height/b.size.y);b=Bounds(go);holder.transform.position+=bottom-new Vector3(b.center.x,b.min.y,b.center.z);var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="Soft exterior palette",color=new Color(.82f,.77f,.75f),mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/ThirdParty/KenneyCity/colormap.png")};m.SetFloat("_Smoothness",.08f);AssetDatabase.CreateAsset(m,AssetDatabase.GenerateUniqueAssetPath(Dir+"/exterior.mat"));foreach(var r in go.GetComponentsInChildren<Renderer>())r.sharedMaterials=Enumerable.Repeat(m,r.sharedMaterials.Length).ToArray();}
 public static void Apply(){
 if(EditorApplication.isPlaying)throw new Exception("Exit Play Mode");if(GameObject.Find("Spacious textured workshop"))throw new Exception("Room already updated");AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);root=new GameObject("Spacious textured workshop").transform;
 plaster=Surface("beige_wall_001","Warm plaster PBR","E3DED0",false,.55f);wood=Surface("fine_grained_wood","Waxed walnut PBR","AD8965",true,.38f);
 var world=GameObject.Find("Little Switch · Workshop");foreach(Transform t in world.transform){if(new[]{"Back plaster","Left plaster","Floor","Oak floorboard","Wainscot","Dado rail","Window frame","Evening sky","Distant townhouse","Evening window","Window mullion","Window crossbar","Deep windowsill"}.Contains(t.name))t.gameObject.SetActive(false);}
 var polish=GameObject.Find("Workshop art refinement");foreach(Transform t in polish.transform){if(t.name=="Right room return"||t.name=="Overhead walnut beam")t.gameObject.SetActive(false);else if(t.name=="Dedicated clock corner"||t.name=="Clock shelf")t.position+=new Vector3(-1.4f,0,1.8f);else if(t.name.StartsWith("Lantern")||t.name=="Shelf lantern base"||t.name=="Shelf warm pool")t.position+=new Vector3(.65f,0,1.8f);}
 var imports=GameObject.Find("Imported artisan props");foreach(Transform t in imports.transform){if(t.name.Contains("shelf_A_big")||t.name.Contains("KitchenCabinet")||t.name.Contains("PottedFlowers"))t.position+=new Vector3(.65f,0,1.8f);else if(t.name.Contains("book_set")||t.name.Contains("pictureframe"))t.position+=new Vector3(0,0,1.8f);}
 var furnished=GameObject.Find("Human scale workshop");foreach(Transform t in furnished.transform)if(t.position.z>3)t.position+=new Vector3(.65f,0,1.8f);
 var upgrade=PrefabUtility.LoadPrefabContents("Assets/LittleSwitch/Resources/WorkshopUpgrade.prefab");foreach(Transform t in upgrade.transform)t.position+=new Vector3(-1.4f,0,1.8f);PrefabUtility.SaveAsPrefabAsset(upgrade,"Assets/LittleSwitch/Resources/WorkshopUpgrade.prefab");PrefabUtility.UnloadPrefabContents(upgrade);
 // Thick walls surrounding an actual opening; no opaque panel behind the window.
 B("Left plaster return",new Vector3(-8.45f,4.6f,.2f),new Vector3(.38f,9.2f,13.1f),plaster);
 B("Right plaster return",new Vector3(8.45f,4.6f,.2f),new Vector3(.38f,9.2f,13.1f),plaster);
 B("Rear wall below window",new Vector3(0,2.43f,6.45f),new Vector3(16.9f,4.86f,.42f),plaster);
 B("Rear wall above window",new Vector3(0,8.65f,6.45f),new Vector3(16.9f,1.1f,.42f),plaster);
 B("Rear left pier",new Vector3(-6.89f,6.48f,6.45f),new Vector3(3.12f,3.24f,.42f),plaster);
 B("Rear right pier",new Vector3(5.14f,6.48f,6.45f),new Vector3(6.62f,3.24f,.42f),plaster);
 B("Oak floor",new Vector3(0,-.14f,.2f),new Vector3(17.2f,.25f,14),wood,.03f);
 B("Rear baseboard",new Vector3(0,.22f,6.16f),new Vector3(16.55f,.43f,.15f),wood,.025f);
 B("Left baseboard",new Vector3(-8.16f,.22f,.2f),new Vector3(.15f,.43f,13),wood,.025f);B("Right baseboard",new Vector3(8.16f,.22f,.2f),new Vector3(.15f,.43f,13),wood,.025f);
 // Recessed timber jambs and a deep sill show the depth of the masonry.
 for(float x=-5.35f;x<2;x+=7.2f)B("Window jamb",new Vector3(x,6.48f,6.28f),new Vector3(.18f,3.38f,.72f),wood,.045f);
 B("Window header",new Vector3(-1.75f,8.12f,6.28f),new Vector3(7.38f,.20f,.72f),wood,.045f);
 B("Window lower rail",new Vector3(-1.75f,4.88f,6.28f),new Vector3(7.38f,.16f,.72f),wood,.045f);
 B("Deep solid window sill",new Vector3(-1.75f,4.86f,5.99f),new Vector3(7.75f,.16f,1.02f),wood,.05f);
 B("Window center mullion",new Vector3(-1.75f,6.49f,6.55f),new Vector3(.09f,3.05f,.14f),wood,.02f);
 B("Window transom",new Vector3(-1.75f,7.33f,6.55f),new Vector3(7.02f,.085f,.14f),wood,.02f);
 B("Upper timber cornice",new Vector3(0,9.03f,6.12f),new Vector3(16.7f,.25f,.32f),wood,.04f);
 UV(GameObject.Find("KayKit · table_medium_long"),wood,3.2f);foreach(Transform t in imports.transform)if(t.name.Contains("shelf_A_big"))UV(t.gameObject,wood,3.2f);
 // Real exterior models replace the flat city cut-outs.
 City("building-a",new Vector3(-8.7f,0,12),8f,0);City("building-b",new Vector3(-3.5f,0,13),9.2f,0);City("building-d",new Vector3(2.2f,0,13.8f),7.5f,0);City("building-f",new Vector3(7.1f,0,15),10f,0);
 var skyMaterial=new Material(Shader.Find("Universal Render Pipeline/Unlit")){name="Dusk sky",color=C("C6B3AC")};AssetDatabase.CreateAsset(skyMaterial,Dir+"/DuskSky.mat");B("Distant evening sky",new Vector3(0,9,25),new Vector3(60,30,.2f),skyMaterial,.01f);
 var windowLight=GameObject.Find("Window bounce").GetComponent<Light>();windowLight.transform.position=new Vector3(-1.7f,7f,5.45f);windowLight.intensity=3.2f;windowLight.range=10;windowLight.color=C("C0D2DC");
 AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(world.scene);EditorSceneManager.SaveScene(world.scene);Debug.Log("SPACIOUS_TEXTURED_ROOM_OK");
 }
}
