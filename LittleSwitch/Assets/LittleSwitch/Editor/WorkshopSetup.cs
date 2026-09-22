using System;using System.IO;using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using TMPro;using LittleSwitch;
using static LittleSwitch.SoftShapes;
public static class WorkshopSetup {
 const string Dir="Assets/LittleSwitch/Generated";
 [MenuItem("Little Switch/Create Workshop")]
 public static void Build(){
 Directory.CreateDirectory(Dir);AssetDatabase.Refresh();
 if(!TMP_Settings.defaultFontAsset)throw new Exception("Import TMP essentials before creating workshop");
 var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
 var world=WorkshopArt.Build();
 var catalog=ScriptableObject.CreateInstance<ShopCatalog>();
 catalog.casePart=Part("case-cream","Cream sixty",PartKind.Case,32,"DCCCAD","DCCCAD","",.2f);
 catalog.pcb=Part("pcb-60","Hot-swap 60",PartKind.PCB,22,"47665E","47665E","",.2f);
 catalog.switches=new[]{Part("linear","Yumuşak · Linear · 18",PartKind.Switch,18,"B88668","B88668","Akıcı ve hafif; yumuşak ses.",.22f),Part("tactile","Tok · Tactile · 24",PartKind.Switch,24,"B69B59","B69B59","Parmak altında küçük bir eşik.",.4f),Part("clicky","Çıtır · Clicky · 20",PartKind.Switch,20,"789AA8","789AA8","Belirgin, parlak bir tık sesi.",.65f)};
 catalog.keycaps=new[]{Part("moss","Yosun\n26",PartKind.Keycaps,26,"DDD9B7","8DAD96","",.2f),Part("clay","Kil\n28",PartKind.Keycaps,28,"E6CEB2","BE826A","",.2f),Part("mist","Sis\n30",PartKind.Keycaps,30,"D6DED5","85A6B3","",.2f)};
 catalog.orders=new[]{
 new CustomerOrder{name="Deniz",request="Gece yazıyorum. Hafif, akıcı ve yumuşak sesli olsun. Krem ve yeşil bana iyi geliyor.",budget=150,switchId="linear",keycapId="moss"},
 new CustomerOrder{name="Ada",request="Hikâyelerimi yazarken tuşun altında küçük bir eşik hissetmek istiyorum. Toprak tonlarını severim.",budget=165,switchId="tactile",keycapId="clay"},
 new CustomerOrder{name="Ege",request="Eski daktiloların belirgin çıtırtısını özledim. Masam mavi ve açık gri.",budget=158,switchId="clicky",keycapId="mist"},
 new CustomerOrder{name="Mina",request="Çizim molalarında akıcı tuşlarla yazmak hoşuma gidiyor. Sıcak kiremit rengi olsun.",budget=160,switchId="linear",keycapId="clay"},
 new CustomerOrder{name="Can",request="Uzun yazılarda tuşu hissetmek istiyorum, ama çıtır tık sesi olmadan. Sakin yeşil tonları harika olur.",budget=170,switchId="tactile",keycapId="moss"}};
 AssetDatabase.CreateAsset(catalog,Dir+"/ShopCatalog.asset");
 var cam=new GameObject("Workshop camera",typeof(Camera),typeof(AudioListener)).GetComponent<Camera>();cam.tag="MainCamera";cam.transform.position=new Vector3(0,6.1f,-9.4f);cam.transform.LookAt(new Vector3(0,2.15f,1.15f));cam.fieldOfView=48;cam.nearClipPlane=.1f;cam.farClipPlane=80;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=C("C8B79A");cam.allowHDR=true;cam.allowMSAA=true;
 var game=new GameObject("Shop systems").AddComponent<ShopGame>();game.catalog=catalog;game.viewCamera=cam;
 PersistGenerated(world.gameObject);
 PlayerSettings.companyName="LittleSwitchStudio";PlayerSettings.productName="Little Switch";PlayerSettings.defaultScreenWidth=1600;PlayerSettings.defaultScreenHeight=1000;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=true;
 QualitySettings.shadowDistance=35;QualitySettings.shadows=ShadowQuality.All;QualitySettings.shadowResolution=ShadowResolution.High;QualitySettings.antiAliasing=4;
 EditorSceneManager.SaveScene(scene,"Assets/Scenes/Workshop.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Workshop.unity",true)};AssetDatabase.SaveAssets();
 Debug.Log("LITTLE_SWITCH_SETUP_OK");
 }
 static PartDefinition Part(string id,string name,PartKind kind,int cost,string col,string accent,string description,float volume){var p=ScriptableObject.CreateInstance<PartDefinition>();p.id=id;p.displayName=name;p.kind=kind;p.price=cost;p.primary=C(col);p.accent=C(accent);p.description=description;p.volume=volume;AssetDatabase.CreateAsset(p,Dir+"/"+id+".asset");return p;}
 static void PersistGenerated(GameObject root){int id=0;foreach(var mf in root.GetComponentsInChildren<MeshFilter>()){if(!mf.GetComponent<TMP_Text>()&&mf.sharedMesh&&!AssetDatabase.Contains(mf.sharedMesh)){AssetDatabase.CreateAsset(mf.sharedMesh,Dir+"/mesh-"+(id++)+".asset");}}foreach(var r in root.GetComponentsInChildren<Renderer>()){foreach(var m in r.sharedMaterials)if(m&&!AssetDatabase.Contains(m))AssetDatabase.CreateAsset(m,Dir+"/mat-"+(id++)+".mat");}}
 [MenuItem("Little Switch/Validate Core Loop")]
 public static void Validate(){var c=AssetDatabase.LoadAssetAtPath<ShopCatalog>(Dir+"/ShopCatalog.asset");Check(c&&c.orders.Length==5,"Five orders");Check(KeyboardView.Layout().Length==61,"61 keys");var s=new BuildState();bool blocked=false;try{s.Deliver(c);}catch(InvalidOperationException){blocked=true;}Check(blocked,"Cannot deliver unfinished build");for(int o=0;o<5;o++){s.orderIndex=o;var order=c.orders[o];s.switchChoice=Array.FindIndex(c.switches,x=>x.id==order.switchId);s.capChoice=Array.FindIndex(c.keycaps,x=>x.id==order.keycapId);Check(s.Compatible(c),"Compatible layout");Check(s.Cost(c)<=order.budget,"Budget valid");s.money-=s.Cost(c);for(int i=0;i<61;i++){s.switches[i]=s.switchChoice;s.caps[i]=s.capChoice;s.tested[i]=true;}Check(s.InstalledCaps==61&&s.Tested==61,"Full board");s.stage=BuildStage.Ready;int before=s.money;s.Deliver(c);Check(s.money==before+order.budget&&s.Quality(c)==5,"Payment and preference review");var clone=JsonUtility.FromJson<BuildState>(JsonUtility.ToJson(s));Check(clone.Tested==61&&clone.money==s.money,"Save roundtrip");s.ResetBoard();Check(s.InstalledSwitches==0&&s.Tested==0,"New board reset");}Check(s.completed==5&&s.reputation==25,"Progression");int balance=s.money;Check(s.Upgrade()&&s.money==balance-90&&!s.Upgrade(),"Upgrade purchased once");Debug.Log("LITTLE_SWITCH_TESTS_OK: 5 orders, budget, compatibility, completion gate, payments, reviews, serialization, reset, upgrade.");}
 static void Check(bool ok,string name){if(!ok)throw new Exception("FAILED: "+name);}
 public static void BuildMac(){var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/Workshop.unity"},locationPathName="../Builds/Little Switch.app",target=BuildTarget.StandaloneOSX,options=BuildOptions.None});if(result.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Build failed: "+result.summary.result);Debug.Log("LITTLE_SWITCH_BUILD_OK");}
}
