using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using System;using System.Linq;using System.IO;using LittleSwitch;using static LittleSwitch.SoftShapes;
public static class WorkshopComposition {
 const string Art="Assets/LittleSwitch/Art";static Transform root;
 static Bounds GetBounds(GameObject g){var rr=g.GetComponentsInChildren<Renderer>();var b=rr[0].bounds;foreach(var r in rr)b.Encapsulate(r.bounds);return b;}
 static void Fit(GameObject g,Vector3 bottom,Vector3 size){var b=GetBounds(g);g.transform.localScale=Vector3.Scale(g.transform.localScale,new Vector3(size.x/b.size.x,size.y/b.size.y,size.z/b.size.z));b=GetBounds(g);g.transform.position+=bottom-new Vector3(b.center.x,b.min.y,b.center.z);}
 static void Uniform(GameObject g,Vector3 bottom,float height){var b=GetBounds(g);g.transform.localScale*=height/b.size.y;b=GetBounds(g);g.transform.position+=bottom-new Vector3(b.center.x,b.min.y,b.center.z);}
 static GameObject Model(string group,string name,Vector3 bottom,Vector3 size,float yaw=0){var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LittleSwitch/ThirdParty/"+group+"/"+name+".fbx");var holder=new GameObject(name+" · furnished");holder.transform.SetParent(root);var g=(GameObject)PrefabUtility.InstantiatePrefab(prefab,holder.transform);g.transform.localRotation=Quaternion.Euler(0,yaw,0)*prefab.transform.localRotation;Fit(holder,bottom,size);foreach(var r in holder.GetComponentsInChildren<Renderer>()){var mats=r.sharedMaterials;for(int i=0;i<mats.Length;i++){var original=mats[i];var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=group+" "+original.name,color=original.HasProperty("_Color")?original.color:Color.white};m.mainTexture=original.mainTexture;m.SetFloat("_Smoothness",.15f);if(group=="KayKit"){m.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LittleSwitch/ThirdParty/KayKit/furniturebits_texture.png");m.color=new Color(.8f,.83f,.78f);}mats[i]=m;}r.sharedMaterials=mats;}return holder;}
 public static void Apply(){
 if(EditorApplication.isPlaying)throw new Exception("Exit Play Mode");if(GameObject.Find("Human scale workshop"))throw new Exception("Composition is already applied; edit existing objects.");
 AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);root=new GameObject("Human scale workshop").transform;
 var ui=(TextureImporter)AssetImporter.GetAtPath("Assets/LittleSwitch/Resources/UI/rounded-panel.png");ui.textureType=TextureImporterType.Sprite;ui.spriteBorder=new Vector4(16,16,16,16);ui.alphaIsTransparency=true;ui.mipmapEnabled=false;ui.SaveAndReimport();
 // Raise the work surface to a plausible desk height; room wall bottoms remain on the floor.
 var world=GameObject.Find("Little Switch · Workshop");foreach(Transform t in world.transform){if(t.name=="Floor"||t.name=="Oak floorboard")continue;if(t.name=="Back plaster"||t.name=="Left plaster"){t.localScale=new Vector3(1,9f/6.8f,1);t.position+=Vector3.up*1.1f;}else t.position+=Vector3.up*2.2f;}
 var imports=GameObject.Find("Imported artisan props");foreach(Transform t in imports.transform){if(t.name.Contains("table_medium_long"))continue;if(t.name.Contains("Stool"))continue;if(t.name.Contains("KitchenCabinet"))continue;t.position+=Vector3.up*2.2f;}
 var polish=GameObject.Find("Workshop art refinement");foreach(Transform t in polish.transform){if(t.name=="Right room return"){t.localScale=new Vector3(1,9.6f/7.4f,1);t.position+=Vector3.up*1.1f;}else t.position+=Vector3.up*2.2f;}
 var table=GameObject.Find("KayKit · table_medium_long");Fit(table,new Vector3(0,0,0),new Vector3(9.2f,4f,4.5f));
 var stool=GameObject.Find("QuinGS · cozy_Stool_Wooden");Uniform(stool,new Vector3(-3.1f,0,-3.7f),2.5f);
 var cabinet=GameObject.Find("QuinGS · cozy_KitchenCabinetB");Fit(cabinet,new Vector3(4.7f,0,3.7f),new Vector3(1.75f,4.15f,1.1f));
 var pc=GameObject.Find("Beveled vintage computer");Uniform(pc,new Vector3(-3.35f,4.02f,.8f),2.15f);var col=pc.AddComponent<BoxCollider>();var bounds=GetBounds(pc);col.center=pc.transform.InverseTransformPoint(bounds.center);col.size=new Vector3(bounds.size.x/pc.transform.lossyScale.x,bounds.size.y/pc.transform.lossyScale.y,bounds.size.z/pc.transform.lossyScale.z);pc.AddComponent<OrderTerminal>();
 // The mat frames a keyboard, rather than covering almost the entire desk.
 foreach(string n in new[]{"Cutting mat edge","Sage cutting mat","Printed blue cutting mat"}){var g=GameObject.Find(n);g.transform.localScale=Vector3.Scale(g.transform.localScale,new Vector3(.6f,1,.6f));}
 foreach(string n in new[]{"Parts tray","Tray inner"}){var g=GameObject.Find(n);g.transform.localScale=Vector3.Scale(g.transform.localScale,new Vector3(.54f,1,.54f));var p=g.transform.position;g.transform.position=new Vector3(-2.6f,p.y,-1.05f);}
 var lamp=GameObject.Find("Beveled enamel task lamp");Uniform(lamp,new Vector3(3.18f,4.02f,1.05f),2.05f);
 // Remove text-heavy placeholder containers and their floating labels.
 foreach(var t in world.GetComponentsInChildren<Transform>(true)){if(new[]{"Parts archive","Paper label","Shipping carton","Paper tape","Notebook","Notebook paper","Label one key\nat a time.","Tool rail"}.Contains(t.name)||t.name.StartsWith("Label LINEAR")||t.name.StartsWith("Label TACTILE")||t.name.StartsWith("Label CLICKY")||t.name.StartsWith("Label CAPS")||t.name.StartsWith("Label CABLES")||t.name.StartsWith("Label EXTRAS"))t.gameObject.SetActive(false);}
 var speaker=GameObject.Find("MrEliptik · speaker_bookshelf");if(speaker)speaker.SetActive(false);
 var notes=Model("Kenney","books",new Vector3(3.22f,4.01f,-1.15f),new Vector3(.9f,.18f,.7f),20);
 Model("MrEliptik","pencil",new Vector3(3.74f,4.02f,-.85f),new Vector3(.035f,.035f,.58f),15);
 // Each shelf has a deliberately separate group, with gaps around the objects.
 Model("Kenney","cardboardBoxClosed",new Vector3(3.23f,4.78f,4f),new Vector3(.7f,.57f,.63f));
 Model("Kenney","cardboardBoxOpen",new Vector3(4.23f,4.78f,4f),new Vector3(.85f,.63f,.7f));
 Model("Kenney","radio",new Vector3(3.47f,6.03f,4f),new Vector3(1.24f,.55f,.42f),180);
 Model("Kenney","cardboardBoxClosed",new Vector3(3.15f,7.28f,4f),new Vector3(.74f,.59f,.63f));
 Model("Kenney","cardboardBoxClosed",new Vector3(4.15f,7.28f,4f),new Vector3(.68f,.52f,.6f),8);
 Model("Kenney","cardboardBoxClosed",new Vector3(4.0f,4.02f,1.65f),new Vector3(.8f,.58f,.63f),-7);
 Model("Kenney","cardboardBoxOpen",new Vector3(2.37f,4.02f,1.83f),new Vector3(.56f,.42f,.5f),10);
 // Tools lie flat on a narrow tray, rather than standing on a featureless block.
 foreach(var t in imports.GetComponentsInChildren<Transform>().Where(t=>t.parent==imports.transform&&t.name.StartsWith("Sjolle")).ToArray()){var b=GetBounds(t.gameObject);var p=t.position;if(t.name.Contains("screwdriver01")){t.rotation=Quaternion.Euler(85,20,5);Uniform(t.gameObject,new Vector3(.9f,4.04f,1.68f),.12f);}else if(t.name.Contains("screwdriver02")){t.rotation=Quaternion.Euler(85,-8,5);Uniform(t.gameObject,new Vector3(1.28f,4.04f,1.64f),.13f);}else if(t.name.Contains("plier01"))t.gameObject.SetActive(false);else if(t.name.Contains("cutter"))t.position=new Vector3(2.7f,4.02f,-.34f);else if(t.name.Contains("vernier"))t.position=new Vector3(3.32f,4.02f,-.2f);}
 foreach(Transform t in polish.transform)if(t.name.StartsWith("Loose tool")||t.name=="Small spare parts tin"||t.name=="Tin recess"||t.name=="Loose switch housing")t.gameObject.SetActive(false);
 var mug=GameObject.Find("MrEliptik · mug");Uniform(mug,new Vector3(3.8f,4.02f,.46f),.48f);
 var pencils=GameObject.Find("MrEliptik · pencil_holder");Uniform(pencils,new Vector3(1.9f,4.02f,1.68f),.51f);
 // Smooth cable paths, sampled from Catmull-Rom curves.
 foreach(var line in polish.GetComponentsInChildren<LineRenderer>()){var old=new Vector3[line.positionCount];line.GetPositions(old);var pts=new System.Collections.Generic.List<Vector3>();for(int i=0;i<old.Length-1;i++)for(int j=0;j<20;j++){float t=j/20f;var a=old[Mathf.Max(0,i-1)];var b=old[i];var c=old[i+1];var d=old[Mathf.Min(old.Length-1,i+2)];pts.Add(.5f*((2*b)+(-a+c)*t+(2*a-5*b+4*c-d)*t*t+(-a+3*b-3*c+d)*t*t*t));}pts.Add(old[old.Length-1]);line.positionCount=pts.Count;line.SetPositions(pts.ToArray());line.numCornerVertices=8;}
 // Upgrade uses the same downloaded furniture and plant style, above the clock.
 var upgrade=new GameObject("WorkshopUpgrade");var shelf=Model("KayKit","shelf_A_small",new Vector3(-4.8f,6.85f,3.75f),new Vector3(1.55f,.12f,.8f));shelf.transform.SetParent(upgrade.transform);var plant=Model("Kenney","pottedPlant",new Vector3(-4.8f,6.97f,3.75f),new Vector3(.68f,.79f,.65f));plant.transform.SetParent(upgrade.transform);
 Persist(root.gameObject);Persist(upgrade);PrefabUtility.SaveAsPrefabAsset(upgrade,"Assets/LittleSwitch/Resources/WorkshopUpgrade.prefab");UnityEngine.Object.DestroyImmediate(upgrade);
 var camera=Camera.main;camera.transform.position=new Vector3(0,7.8f,-9.6f);camera.transform.LookAt(new Vector3(0,5.15f,1));camera.fieldOfView=45;
 var light=GameObject.Find("Bench lamp").GetComponent<Light>();light.transform.position=new Vector3(2.65f,5.85f,.83f);light.transform.LookAt(new Vector3(0,4.08f,-.55f));light.intensity=11;light.spotAngle=105;
 AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(world.scene);EditorSceneManager.SaveScene(world.scene);Debug.Log("HUMAN_SCALE_COMPOSITION_OK");
 }
 static void Persist(GameObject g){foreach(var mf in g.GetComponentsInChildren<MeshFilter>())if(!mf.GetComponent<TMPro.TMP_Text>()&&mf.sharedMesh&&!AssetDatabase.Contains(mf.sharedMesh)){var copy=UnityEngine.Object.Instantiate(mf.sharedMesh);AssetDatabase.CreateAsset(copy,AssetDatabase.GenerateUniqueAssetPath(Art+"/furnished-mesh.asset"));mf.sharedMesh=copy;}foreach(var r in g.GetComponentsInChildren<Renderer>())foreach(var m in r.sharedMaterials)if(m&&!AssetDatabase.Contains(m)&&!r.GetComponent<TMPro.TMP_Text>())AssetDatabase.CreateAsset(m,AssetDatabase.GenerateUniqueAssetPath(Art+"/furnished-material.mat"));}
}
