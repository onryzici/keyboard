using UnityEngine;
using static LittleSwitch.SoftShapes;
namespace LittleSwitch {
public partial class ShopGame {
 public void PhysicalTestKey(UnityEngine.InputSystem.Key code,string displayName) {
  string label=code switch {
   UnityEngine.InputSystem.Key.Space=>"",
   UnityEngine.InputSystem.Key.Backspace=>"⌫",
   UnityEngine.InputSystem.Key.Minus=>"−",
   UnityEngine.InputSystem.Key.CapsLock=>"caps",
   UnityEngine.InputSystem.Key.LeftShift or UnityEngine.InputSystem.Key.RightShift=>"shift",
   UnityEngine.InputSystem.Key.LeftCtrl or UnityEngine.InputSystem.Key.RightCtrl=>"ctrl",
   UnityEngine.InputSystem.Key.LeftAlt or UnityEngine.InputSystem.Key.RightAlt=>"alt",
   UnityEngine.InputSystem.Key.LeftMeta or UnityEngine.InputSystem.Key.RightMeta=>"win",
   UnityEngine.InputSystem.Key.ContextMenu=>"menu",
   _=>displayName.ToLowerInvariant()
  };
  for(int i=0;i<board.keys.Length;i++)if(board.keys[i].label.ToLowerInvariant()==label&&!state.tested[i])TestKey(i);
 }
 int previewSlot=-1;
 public int FindAssemblySlot(Vector3 point) {
  for(int i=0;i<board.keys.Length;i++) {
   var p=board.SlotPosition(i);var key=board.keys[i];
   if(Mathf.Abs(point.x-p.x)<key.width*PartScale*.5f && Mathf.Abs(point.z-p.z)<.36f*PartScale*.5f)
    return ValidSlot(i)?i:-1;
  }
  return -1;
 }
 void UpdateHeldPart(Ray ray,bool overUI,bool released) {
  var plane=new Plane(Vector3.up,new Vector3(0,4.224f,0));
  if(!plane.Raycast(ray,out float distance))return;
  var point=ray.GetPoint(distance);int best=overUI?-1:FindAssemblySlot(point);
  Highlight(best);
  if(best!=previewSlot && state.stage==BuildStage.Keycaps) {
   foreach(Transform t in held.transform)Destroy(t.gameObject);
   int i=best>=0?best:1;var k=board.keys[i];var part=catalog.keycaps[state.capChoice];
   var color=k.label.Length>1||i==0||k.label==""?part.accent:part.primary;
   KeyboardView.Cap(held.transform,best>=0?k.width-.038f:.32f,color,false);
   if(best>=0)Label(KeyboardView.Legend(k.label),held.transform,new Vector3(0,.225f,0),k.label.Length>1?.078f:.125f,"39483F",new Vector3(90,0,0));
  }
  previewSlot=best;
  var target=best>=0?board.SlotPosition(best)+Vector3.up*.19f:point+Vector3.up*.18f;
  held.transform.position=Vector3.Lerp(held.transform.position,target,1-Mathf.Exp(-Time.deltaTime*28));
  if(!released)return;
  if(best>=0)StartCoroutine(Install(best));
  else {Destroy(held);held=null;hint="Parça kutuya döndü; boş bir yuvanın üzerine bırak.";ui.Refresh();}
  previewSlot=-1;Highlight(-1);
 }
}}

