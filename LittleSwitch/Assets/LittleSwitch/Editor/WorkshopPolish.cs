using System;using System.IO;using System.Linq;using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine.Rendering;using UnityEngine.Rendering.Universal;using LittleSwitch;using static LittleSwitch.SoftShapes;
public static class WorkshopPolish {
 const string Dir="Assets/LittleSwitch/Art";static Transform root;
 static GameObject B(string n,Vector3 p,Vector3 s,string c,float r=.04f)=>Box(n,root,p,s,c,r);
 static void Cable(string name,Vector3[] points,float width,string color){var g=new GameObject(name);g.transform.SetParent(root);var l=g.AddComponent<LineRenderer>();l.useWorldSpace=false;l.positionCount=points.Length;l.SetPositions(points);l.startWidth=l.endWidth=width;l.numCapVertices=5;l.numCornerVertices=5;l.sharedMaterial=Mat(color);}
 static Material Glow(string color,float power){var m=new Material(Mat(color));m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",C(color)*power);return m;}
 static void Paint(string name,string color){var g=GameObject.Find(name);if(g){g.GetComponent<Renderer>().sharedMaterial=Mat(color);}}
 static Bounds Bounds(GameObject g){var rs=g.GetComponentsInChildren<Renderer>();var b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);return b;}
 static GameObject Model(string path,string name,Vector3 bottom,float height,float yaw){var a=AssetDatabase.LoadAssetAtPath<GameObject>(path);var holder=new GameObject(name);holder.transform.SetParent(root);var g=(GameObject)PrefabUtility.InstantiatePrefab(a,holder.transform);g.transform.localRotation=Quaternion.Euler(0,yaw,0)*a.transform.localRotation;var b=Bounds(g);holder.transform.localScale=Vector3.one*(height/b.size.y);b=Bounds(g);g.transform.position-=new Vector3(b.center.x,b.min.y,b.center.z);holder.transform.position=bottom;return holder;}
 public static void Apply(){
 if(EditorApplication.isPlaying)throw new Exception("Exit Play Mode");
 Directory.CreateDirectory(Dir);AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
 var old=GameObject.Find("Workshop art refinement");if(old)UnityEngine.Object.DestroyImmediate(old);
 root=new GameObject("Workshop art refinement").transform;
 // The stool clears both the front apron and legs, including its complete seat bounds.
 var stool=GameObject.Find("QuinGS · cozy_Stool_Wooden");if(stool)stool.transform.position=new Vector3(-4.45f,.08f,-3.65f);
 foreach(var n in new[]{"Terminal body","Screen bezel","Phosphor glass","Terminal foot","Label little switch\n\n5 lovely commissions\nno rush. just craft.","MrEliptik · lamp_architect"}){var g=GameObject.Find(n);if(g)g.SetActive(false);}
 Paint("Cutting mat edge","174758");Paint("Sage cutting mat","28617E");
 var surface=new GameObject("Printed blue cutting mat");surface.transform.SetParent(root);surface.transform.position=new Vector3(0,1.885f,-.55f);
 var mesh=new Mesh{name="Mat printed surface"};mesh.vertices=new[]{new Vector3(-3.27f,0,-1.67f),new Vector3(-3.27f,0,1.67f),new Vector3(3.27f,0,1.67f),new Vector3(3.27f,0,-1.67f)};mesh.uv=new[]{new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)};mesh.triangles=new[]{0,1,2,0,2,3};mesh.RecalculateNormals();surface.AddComponent<MeshFilter>().sharedMesh=mesh;
 var mat=new Material(Mat("FFFFFF"));mat.name="Printed matte blue cutting surface";mat.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Dir+"/CuttingMat.png");mat.SetFloat("_Smoothness",.12f);surface.AddComponent<MeshRenderer>().sharedMaterial=mat;
 var ti=(TextureImporter)AssetImporter.GetAtPath(Dir+"/CuttingMat.png");ti.maxTextureSize=2048;ti.anisoLevel=8;ti.mipmapEnabled=true;ti.textureCompression=TextureImporterCompression.Uncompressed;ti.SaveAndReimport();
 var pc=Model("Assets/LittleSwitch/ThirdParty/Refined/RetroComputer.fbx","Beveled vintage computer",new Vector3(-4.38f,1.82f,1.05f),1.32f,168);
 foreach(var r in pc.GetComponentsInChildren<Renderer>()){var mm=r.sharedMaterials;for(int i=0;i<mm.Length;i++){string n=mm[i].name;mm[i]=n.Contains("glass")?Mat("263F3C"):n.Contains("LED")?Glow("9BC980",.7f):Mat(n.Contains("recess")?"343E37":n.Contains("edge")?"89846C":n.Contains("function")?"BD8961":n.Contains("Keycap")?"DBD0AD":"C4B799");}r.sharedMaterials=mm;}
 var lamp=Model("Assets/LittleSwitch/ThirdParty/Refined/ArtisanLamp.fbx","Beveled enamel task lamp",new Vector3(3.56f,1.82f,1.45f),1.65f,-45);
 foreach(var r in lamp.GetComponentsInChildren<Renderer>()){var mm=r.sharedMaterials;for(int i=0;i<mm.Length;i++){var n=mm[i].name;mm[i]=n.Contains("Bulb")?Glow("FFE0A2",1.8f):Mat(n.Contains("Grey")?"34464A":"355F68");}r.sharedMaterials=mm;}
 Clock();Power();Details();Lighting();Recompose();
 Persist(root.gameObject);AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(root.gameObject.scene);EditorSceneManager.SaveScene(root.gameObject.scene);Debug.Log("WORKSHOP_POLISH_APPLIED");
 }
 static void Clock(){
 var previousRoot=root;root=new GameObject("Dedicated clock corner").transform;root.SetParent(previousRoot);
 B("Clock rubber foot left",new Vector3(-3.49f,2.78f,3.53f),new Vector3(.16f,.08f,.28f),"302E28");B("Clock rubber foot right",new Vector3(-2.67f,2.78f,3.53f),new Vector3(.16f,.08f,.28f),"302E28");
 B("Rounded digital clock",new Vector3(-3.08f,3.05f,3.55f),new Vector3(1.34f,.55f,.45f),"C5B69B",.15f);
 B("Clock dark lens",new Vector3(-3.08f,3.05f,3.315f),new Vector3(1.14f,.37f,.025f),"142824",.05f);
 var glow=Glow("AFE5AC",1.5f);int[] values={1,8,1,6};string[] digits={"abcdef","bc","abged","abgcd","fgbc","afgcd","afgecd","abc","abcdefg","abfgcd"};
 for(int i=0;i<4;i++){float x=-3.5f+i*.27f+(i>1?.09f:0);foreach(char c in digits[values[i]]){bool h=c=='a'||c=='g'||c=='d';float dx=c=='b'||c=='c'?.075f:c=='f'||c=='e'?-.075f:0;float dy=c=='a'?.13f:c=='d'?-.13f:c=='g'?0:c=='b'||c=='f'?.065f:-.065f;var seg=B("Clock segment",new Vector3(x+dx,3.05f+dy,3.295f),h?new Vector3(.12f,.022f,.016f):new Vector3(.021f,.102f,.016f),"AFE5AC",.01f);seg.GetComponent<Renderer>().sharedMaterial=glow;}}
 for(int j=-1;j<=1;j+=2)B("Clock colon",new Vector3(-3.08f,3.05f+j*.055f,3.295f),Vector3.one*.026f,"AFE5AC").GetComponent<Renderer>().sharedMaterial=glow;
 root.position=new Vector3(-1.82f,.4f,0);root=previousRoot;
 }
 static void Power(){
 B("Four socket power strip",new Vector3(.5f,2.9f,3.65f),new Vector3(2.3f,.34f,.24f),"BDB49F",.09f);
 for(int i=0;i<4;i++){float x=-.27f+i*.47f;var face=Cyl("Recessed round socket",root,new Vector3(x,2.9f,3.514f),new Vector3(.275f,.012f,.275f),"807E6D");face.transform.eulerAngles=new Vector3(90,0,0);for(int j=-1;j<=1;j+=2)B("Socket pin hole",new Vector3(x+j*.055f,2.9f,3.493f),new Vector3(.031f,.057f,.016f),"262F2D",.014f);}
 B("Power rocker",new Vector3(1.43f,2.9f,3.505f),new Vector3(.15f,.18f,.035f),"A96145");
 B("Lamp plug",new Vector3(1.14f,2.9f,3.46f),new Vector3(.19f,.19f,.18f),"303C3A");
 Cable("Task lamp cable",new[]{new Vector3(1.14f,2.9f,3.37f),new Vector3(1.22f,2.68f,3.3f),new Vector3(1.6f,2.54f,3.2f),new Vector3(2.17f,2.48f,3),new Vector3(2.58f,1.87f,2.15f),new Vector3(3.38f,1.86f,1.65f)},.028f,"283735");
 Cable("Power supply cable",new[]{new Vector3(-.65f,2.9f,3.65f),new Vector3(-.94f,2.88f,3.55f),new Vector3(-1.2f,2.63f,3.5f),new Vector3(-1.6f,2.52f,3.4f),new Vector3(-2.65f,2.54f,3.3f),new Vector3(-3.95f,1.9f,2.34f)},.033f,"423F32");
 }
 static void Details(){
 // Move the cactus away from the clock silhouette.
 var cactus=GameObject.Find("KayKit · cactus_medium_A");if(cactus)cactus.SetActive(false);
 B("Clock shelf",new Vector3(-4.92f,3.12f,3.7f),new Vector3(1.65f,.10f,.68f),"715239");
 // Reuse downloaded tool geometry, laid on the desk with real support contact.
 foreach(var pair in new[]{("Sjolle · screwdriver01",new Vector3(3.86f,1.84f,-1.73f)),("Sjolle · plier01",new Vector3(4.5f,1.84f,-1.7f))}){
 var original=GameObject.Find(pair.Item1);if(!original)continue;var go=UnityEngine.Object.Instantiate(original,root);go.name="Loose tool · "+original.name;go.transform.rotation=Quaternion.Euler(85,25,12);var b=Bounds(go);go.transform.position+=pair.Item2-new Vector3(b.center.x,b.min.y,b.center.z);}
 for(int i=0;i<3;i++){float x=1.0f+i*.38f;Cyl("Workshop bottle",root,new Vector3(x,2.07f,2.45f),new Vector3(.22f,.22f,.22f),i==1?"A17C52":"667E69");Cyl("Bottle cap",root,new Vector3(x,2.32f,2.45f),new Vector3(.14f,.05f,.14f),"C7B897");B("Bottle label",new Vector3(x,2.08f,2.333f),new Vector3(.15f,.18f,.016f),"D7C8A5",.01f);}
 for(int i=0;i<5;i++)Rod(root,new Vector3(2,2.05f,1.82f),new Vector3(1.8f+i*.095f,2.8f+(i%3)*.1f,1.83f+(i%2)*.12f),.028f,i%2==0?"A57A47":"405F59");
 var tray=B("Small spare parts tin",new Vector3(3.68f,1.87f,-.87f),new Vector3(.53f,.10f,.52f),"536C68",.08f);B("Tin recess",new Vector3(3.68f,1.927f,-.87f),new Vector3(.43f,.014f,.42f),"243F3E");
 for(int i=0;i<7;i++)B("Loose switch housing",new Vector3(3.53f+(i%3)*.14f,1.975f,-1+(i/3)*.14f),new Vector3(.09f,.07f,.09f),i%2==0?"C7BDA0":"9F7658",.016f);
 // A folded order and a pencilled work note beside the existing notebook.
 var paper=B("Folded repair ticket",new Vector3(-2.24f,1.845f,1.62f),new Vector3(.67f,.018f,.48f),"D9C9A8",.01f);paper.transform.rotation=Quaternion.Euler(0,-12,0);
 for(int i=0;i<4;i++)B("Ticket ink",new Vector3(-2.24f,1.857f,1.48f+i*.07f),new Vector3(.4f-(i%2)*.08f,.003f,.01f),"6A756B",.001f);
 }
 static void Lighting(){
 var p=UniversalRenderPipeline.asset;if(!p)throw new Exception("URP required");p.supportsHDR=true;p.shadowDistance=25;p.msaaSampleCount=4;EditorUtility.SetDirty(p);
 var so=new SerializedObject(p);var renderer=(UniversalRendererData)so.FindProperty("m_RendererDataList").GetArrayElementAtIndex(0).objectReferenceValue;
 if(!renderer.postProcessData){var ids=AssetDatabase.FindAssets("t:PostProcessData");if(ids.Length>0)renderer.postProcessData=AssetDatabase.LoadAssetAtPath<PostProcessData>(AssetDatabase.GUIDToAssetPath(ids[0]));}
 if(!renderer.postProcessData)throw new Exception("Renderer post-process data missing");
 var ao=renderer.rendererFeatures.OfType<ScreenSpaceAmbientOcclusion>().FirstOrDefault();if(!ao){ao=ScriptableObject.CreateInstance<ScreenSpaceAmbientOcclusion>();ao.name="Workshop contact shadows";AssetDatabase.AddObjectToAsset(ao,renderer);renderer.rendererFeatures.Add(ao);}
 var aso=new SerializedObject(ao);var settings=aso.FindProperty("m_Settings");settings.FindPropertyRelative("Intensity").floatValue=1.35f;settings.FindPropertyRelative("Radius").floatValue=.22f;settings.FindPropertyRelative("DirectLightingStrength").floatValue=.3f;settings.FindPropertyRelative("Falloff").floatValue=30;aso.ApplyModifiedPropertiesWithoutUndo();ao.SetActive(true);renderer.SetDirty();EditorUtility.SetDirty(renderer);
 var cam=Camera.main;var data=cam.GetUniversalAdditionalCameraData();data.renderPostProcessing=true;data.volumeLayerMask=1;data.antialiasing=AntialiasingMode.SubpixelMorphologicalAntiAliasing;cam.allowHDR=true;
 cam.transform.position=new Vector3(0,5.35f,-8.55f);cam.transform.LookAt(new Vector3(0,2.5f,1.1f));cam.fieldOfView=48;
 var volume=new GameObject("Warm workshop grading").AddComponent<Volume>();volume.transform.SetParent(root);volume.isGlobal=true;volume.priority=2;
 var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Dir+"/WorkshopEvening.asset");if(!profile){profile=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(profile,Dir+"/WorkshopEvening.asset");}else{foreach(var c in profile.components.ToArray())UnityEngine.Object.DestroyImmediate(c,true);profile.components.Clear();}
 profile.Add<Tonemapping>(true).mode.value=TonemappingMode.ACES;
 var color=profile.Add<ColorAdjustments>(true);color.postExposure.value=.65f;color.contrast.value=4;color.saturation.value=-5;
 var bloom=profile.Add<Bloom>(true);bloom.intensity.value=.12f;bloom.threshold.value=1.15f;bloom.scatter.value=.55f;
 var vignette=profile.Add<Vignette>(true);vignette.intensity.value=.14f;vignette.smoothness.value=.45f;
 foreach(var c in profile.components)AssetDatabase.AddObjectToAsset(c,profile);volume.sharedProfile=profile;EditorUtility.SetDirty(profile);
 RenderSettings.ambientMode=AmbientMode.Trilight;RenderSettings.ambientSkyColor=C("7C929B")*.95f;RenderSettings.ambientEquatorColor=C("857862")*.78f;RenderSettings.ambientGroundColor=C("554438")*.6f;
 foreach(var l in UnityEngine.Object.FindObjectsByType<Light>()){
 if(l.name=="Late afternoon"){l.intensity=.9f;l.color=C("E7C19A");l.transform.rotation=Quaternion.Euler(34,-38,0);l.shadowStrength=.4f;l.shadowBias=.03f;l.shadowNormalBias=.15f;l.shadows=LightShadows.Soft;}
 if(l.name=="Window bounce"){l.intensity=2.0f;l.color=C("A2BFCE");l.range=7;}
 if(l.name=="Soft room fill"){l.intensity=2.5f;l.color=C("B0C4C7");}
 if(l.name=="Bench lamp"){l.type=LightType.Spot;l.transform.position=new Vector3(2.7f,3.5f,1.05f);l.transform.LookAt(new Vector3(.8f,1.85f,-.2f));l.spotAngle=100;l.innerSpotAngle=65;l.intensity=9;l.color=C("FFDA99");l.range=7;l.shadows=LightShadows.Soft;l.shadowBias=.02f;l.shadowNormalBias=.08f;}
 }
 Paint("Back plaster","79664F");Paint("Left plaster","5D665C");Paint("Deep windowsill","886745");Paint("Window frame","654B36");Paint("Dado rail","9B835C");
 foreach(var t in GameObject.Find("Little Switch · Workshop").GetComponentsInChildren<Transform>())if(t.position.x>4.8f&&t.position.y>3.85f&&t.position.y<4.6f&&(t.name=="Parts archive"||t.name=="Paper label"||t.name=="Label EXTRAS"))t.gameObject.SetActive(false);
 B("Right room return",new Vector3(6.48f,3.7f,1),new Vector3(.23f,7.4f,8),"554B3D");
 B("Overhead walnut beam",new Vector3(0,6.72f,4.17f),new Vector3(13.1f,.35f,.48f),"493A2C");
 // A second localized warm light on the shelves, grounded by a visible lantern.
 B("Shelf lantern base",new Vector3(5.42f,3.84f,3.78f),new Vector3(.43f,.09f,.38f),"3B4237");var glass=B("Lantern frosted glass",new Vector3(5.42f,4.16f,3.78f),new Vector3(.31f,.57f,.28f),"FFCA78");glass.GetComponent<Renderer>().sharedMaterial=Glow("FFBE68",1.8f);
 B("Lantern cap",new Vector3(5.42f,4.48f,3.78f),new Vector3(.44f,.10f,.39f),"3B4237");for(int i=-1;i<=1;i+=2)for(int j=-1;j<=1;j+=2)B("Lantern upright",new Vector3(5.42f+i*.17f,4.16f,3.78f+j*.15f),new Vector3(.035f,.57f,.035f),"3B4237",.01f);
 var light=new GameObject("Shelf warm pool").AddComponent<Light>();light.transform.SetParent(root);light.transform.position=new Vector3(5.1f,4.18f,3.2f);light.type=LightType.Point;light.color=C("FFB564");light.intensity=2.2f;light.range=3;
 }
 static void Recompose(){
 var world=GameObject.Find("Little Switch · Workshop");foreach(var t in world.GetComponentsInChildren<Transform>(true))if(t.name=="Studio sign"||t.name=="Label little switch"||t.name.StartsWith("Label CUSTOM KEYBOARDS")||t.name=="Pinboard"||t.name=="Thank you note")t.gameObject.SetActive(false);
 var books=GameObject.Find("KayKit · book_set");if(books)books.transform.position=new Vector3(-1.15f,2.74f,3.93f);
 var picture=GameObject.Find("KayKit · pictureframe_standing_A");if(picture)picture.transform.position=new Vector3(.05f,2.74f,3.93f);
 var camera=GameObject.Find("MrEliptik · camera");if(camera)camera.SetActive(false);
 var desk=GameObject.Find("KayKit · table_medium_long");if(desk){var b=Bounds(desk);desk.transform.localScale=Vector3.Scale(desk.transform.localScale,new Vector3(10.8f/b.size.x,1,4.8f/b.size.z));}
 var mug=GameObject.Find("MrEliptik · mug");if(mug){var mb=Bounds(mug);mug.transform.localScale*=.48f/mb.size.y;mug.transform.position=new Vector3(4.67f,1.82f,.74f);}
 var notebook=GameObject.Find("Notebook");if(notebook){notebook.transform.position=new Vector3(4.35f,1.9f,-.2f);}
 foreach(Transform t in root){if(t.name=="Workshop bottle"||t.name=="Bottle cap"||t.name=="Bottle label"||t.name=="Folded repair ticket"||t.name=="Ticket ink")t.gameObject.SetActive(false);}
 }
 static void Persist(GameObject go){foreach(var mf in go.GetComponentsInChildren<MeshFilter>())if(!mf.GetComponent<TMPro.TMP_Text>()&&mf.sharedMesh&&!AssetDatabase.Contains(mf.sharedMesh)){var copy=UnityEngine.Object.Instantiate(mf.sharedMesh);string path=AssetDatabase.GenerateUniqueAssetPath(Dir+"/polish-mesh.asset");AssetDatabase.CreateAsset(copy,path);mf.sharedMesh=copy;}

 foreach(var r in UnityEngine.Object.FindObjectsByType<Renderer>())foreach(var m in r.sharedMaterials)if(m&&!AssetDatabase.Contains(m)&&!r.GetComponent<TMPro.TMP_Text>()){string path=AssetDatabase.GenerateUniqueAssetPath(Dir+"/polish-"+m.name.Replace('/','_')+".mat");AssetDatabase.CreateAsset(m,path);}}
}
