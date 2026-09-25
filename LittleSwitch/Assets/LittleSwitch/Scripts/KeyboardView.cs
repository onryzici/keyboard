using UnityEngine; using TMPro; using static LittleSwitch.SoftShapes;
namespace LittleSwitch {
public class KeySlot : MonoBehaviour {public int index;}
public class PartsSupply : MonoBehaviour {}
public class KeyboardView : MonoBehaviour {
 public static string Legend(string label)=>label=="⌫"?"back":label;
 static GameObject capPrefab;
 static Mesh switchStemMesh;
 public static GameObject Cap(Transform parent,float width,Color color,bool collider) {
  if(!capPrefab)capPrefab=Resources.Load<GameObject>("Keyboard/SculptedKeycap");
  if(!capPrefab)return Box("Keycap",parent,new Vector3(0,.1f,0),new Vector3(width,.22f,.312f),ColorUtility.ToHtmlStringRGB(color),.047f,collider);
  var go=Object.Instantiate(capPrefab,parent,false);go.name="Sculpted keycap";go.transform.localScale=new Vector3(width/.316f,1,1);
  foreach(var r in go.GetComponentsInChildren<Renderer>())r.sharedMaterial=Mat(ColorUtility.ToHtmlStringRGB(color));
  if(collider){var box=go.AddComponent<BoxCollider>();box.center=new Vector3(0,.11f,0);box.size=new Vector3(.316f,.22f,.306f);}
  return go;
 }
 public class Key {public string label; public float width,x,z;}
 public static Key[] Layout(){var keys=new System.Collections.Generic.List<Key>();
 string[][] labels={new[]{"esc","1","2","3","4","5","6","7","8","9","0","−","=","⌫"},new[]{"tab","Q","W","E","R","T","Y","U","I","O","P","[","]","\\"},new[]{"caps","A","S","D","F","G","H","J","K","L",";","'","enter"},new[]{"shift","Z","X","C","V","B","N","M",",",".","/","shift"},new[]{"ctrl","win","alt","","alt","fn","menu","ctrl"}};
 float[][] widths={new[]{1f,1,1,1,1,1,1,1,1,1,1,1,1,2},new[]{1.5f,1,1,1,1,1,1,1,1,1,1,1,1,1.5f},new[]{1.75f,1,1,1,1,1,1,1,1,1,1,1,2.25f},new[]{2.25f,1,1,1,1,1,1,1,1,1,1,2.75f},new[]{1.25f,1.25f,1.25f,6.25f,1.25f,1.25f,1.25f,1.25f}};
 for(int r=0;r<5;r++){float x=-2.7f;for(int j=0;j<labels[r].Length;j++){float w=widths[r][j]*.36f;keys.Add(new Key{label=labels[r][j],width=w,x=x+w*.5f,z=.72f-r*.36f});x+=w;}}return keys.ToArray();}
 public Transform[] tops=new Transform[61];public Renderer[] sockets=new Renderer[61];[SerializeField] Renderer[] wells=new Renderer[61];public GameObject[] pieces=new GameObject[61];[System.NonSerialized] private Key[] cachedKeys; public Key[] keys { get { if(cachedKeys==null) cachedKeys=Layout(); return cachedKeys; } set { cachedKeys=value; } }
 public void Initialize(){keys=Layout();Box("Cream sixty case",transform,Vector3.zero,new Vector3(5.85f,.26f,2.19f),"DCCCAD",.1f);Box("Inset PCB",transform,new Vector3(0,.145f,0),new Vector3(5.57f,.07f,1.91f),"334D48",.035f);
 for(int i=0;i<keys.Length;i++){var k=keys[i];var g=Box("Socket "+k.label,transform,new Vector3(k.x,.198f,k.z),new Vector3(k.width-.045f,.055f,.306f),"8B9A83",.022f,true);g.AddComponent<KeySlot>().index=i;sockets[i]=g.GetComponent<Renderer>();wells[i]=Box("Contact well",g.transform,new Vector3(0,.03f,0),new Vector3(.14f,.009f,.14f),"344C43",.008f).GetComponent<Renderer>();wells[i].shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;}
 Box("Brass maker badge",transform,new Vector3(2.42f,.146f,-.988f),new Vector3(.36f,.018f,.065f),"B79759",.015f);
 }
 public void Refresh(BuildState state,ShopCatalog catalog){for(int i=0;i<61;i++)RenderSlot(i,state,catalog);}
 public void RenderSlot(int i,BuildState s,ShopCatalog c){if(pieces[i])Destroy(pieces[i]);tops[i]=null;var k=keys[i];sockets[i].enabled=s.caps[i]<0;wells[i].enabled=s.switches[i]<0;sockets[i].sharedMaterial=Mat(s.tested[i]?"B3C693":"8B9A83");if(s.switches[i]<0)return;
 var g=new GameObject("Installed "+k.label);g.transform.SetParent(transform,false);g.transform.localPosition=new Vector3(k.x,.235f,k.z);pieces[i]=g;
 if(s.caps[i]<0){Switch(g.transform,c.switches[s.switches[i]].primary);}
 else {var part=c.keycaps[s.caps[i]];Color col=(k.label.Length>1||i==0||k.label=="")?part.accent:part.primary;var cap=Cap(g.transform,k.width-.038f,col,true);cap.AddComponent<KeySlot>().index=i;tops[i]=cap.transform;Label(Legend(k.label),g.transform,new Vector3(0,.225f,0),k.label.Length>1?.078f:.125f,"39483F",new Vector3(90,0,0));if(s.tested[i])Box("Test LED",g.transform,new Vector3(0,.012f,-.148f),new Vector3(.11f,.012f,.018f),"DBE8A2",.004f);}
 }
 public static void Switch(Transform p,Color col){
  Box("Switch housing",p,new Vector3(0,.035f,0),new Vector3(.25f,.11f,.25f),"DCCDB1",.03f);
  if(!switchStemMesh){switchStemMesh=new Mesh{name="Shared switch cross stem"};switchStemMesh.CombineMeshes(new[]{new CombineInstance{mesh=Rounded(new Vector3(.13f,.075f,.045f),.013f),transform=Matrix4x4.identity},new CombineInstance{mesh=Rounded(new Vector3(.045f,.075f,.13f),.013f),transform=Matrix4x4.identity}});}
  var stem=new GameObject("Cross stem");stem.transform.SetParent(p,false);stem.transform.localPosition=new Vector3(0,.105f,0);stem.AddComponent<MeshFilter>().sharedMesh=switchStemMesh;stem.AddComponent<MeshRenderer>().sharedMaterial=Mat(ColorUtility.ToHtmlStringRGB(col));
 }
 public Vector3 SlotPosition(int i)=>transform.TransformPoint(new Vector3(keys[i].x,.235f,keys[i].z));
}
}

