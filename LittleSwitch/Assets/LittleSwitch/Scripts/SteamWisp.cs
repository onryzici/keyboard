using UnityEngine;
namespace LittleSwitch {
 public class SteamWisp:MonoBehaviour {
  public Material material;LineRenderer[] wisps;HotDrink drink;
  void Start(){drink=GetComponentInParent<HotDrink>();wisps=new LineRenderer[3];for(int i=0;i<3;i++){var go=new GameObject("Fine steam curl");go.transform.SetParent(transform,false);var line=go.AddComponent<LineRenderer>();line.sharedMaterial=material;line.useWorldSpace=false;line.positionCount=28;line.widthMultiplier=.045f+i*.008f;line.widthCurve=new AnimationCurve(new Keyframe(0,.15f),new Keyframe(.45f,1),new Keyframe(1,1.4f));line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;line.receiveShadows=false;wisps[i]=line;}}
  void Update(){if(wisps==null)return;for(int i=0;i<wisps.Length;i++){var line=wisps[i];line.enabled=!drink||!drink.IsSipping;float time=Time.time*.7f+i*2.1f;float opacity=.12f+Mathf.Sin(time)*.035f;line.startColor=line.endColor=new Color(.93f,.91f,.84f,opacity);for(int j=0;j<28;j++){float t=j/27f;float x=Mathf.Sin(t*6-time)*(.007f+t*.038f)+Mathf.Sin(time*.4f)*t*.035f;line.SetPosition(j,new Vector3(x+(i-1)*.025f,t*(.38f+i*.055f),Mathf.Sin(t*5+time)*t*.019f));}}}
 }
}
