using System.Collections;using UnityEngine;using UnityEngine.InputSystem;
namespace LittleSwitch {public partial class ShopGame {
 MovableDeskProp movingProp;Vector3 dragHome,dragOffset;bool dragValid;
 bool DeskInput(Mouse mouse,Keyboard keyboard){
  if(movingProp){if(keyboard!=null&&keyboard.escapeKey.wasPressedThisFrame){EndDeskDrag(false);return true;}var ray=viewCamera.ScreenPointToRay(mouse.position.ReadValue());var plane=new Plane(Vector3.up,new Vector3(0,4.05f,0));if(plane.Raycast(ray,out float d)){var next=ray.GetPoint(d)+dragOffset;next.y=dragHome.y;movingProp.transform.position=next;var b=movingProp.Footprint();var correction=new Vector3(Mathf.Max(0,-4.55f-b.min.x)-Mathf.Max(0,b.max.x-4.55f),0,Mathf.Max(0,-2.2f-b.min.z)-Mathf.Max(0,b.max.z-2.2f));movingProp.transform.position+=correction;dragValid=ValidDeskPlacement(movingProp);}if(mouse.leftButton.wasReleasedThisFrame)EndDeskDrag(dragValid);return true;}
  if(busy||held||selectedTool||mouse==null||keyboard==null||!(keyboard.leftAltKey.isPressed||keyboard.rightAltKey.isPressed)||!mouse.leftButton.wasPressedThisFrame)return false;
  if(UnityEngine.EventSystems.EventSystem.current&&UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())return false;
  var grabRay=viewCamera.ScreenPointToRay(mouse.position.ReadValue());if(!Physics.Raycast(grabRay,out var hit,100))return false;var prop=hit.collider.GetComponentInParent<MovableDeskProp>();var anchor=hit.collider.GetComponentInParent<PartsSupplyAnchor>();if(!prop&&anchor)prop=anchor.movable;if(!prop||!prop.CanMove)return false;
  movingProp=prop;dragHome=prop.transform.position;var surface=new Plane(Vector3.up,new Vector3(0,4.05f,0));surface.Raycast(grabRay,out float distance);dragOffset=dragHome-grabRay.GetPoint(distance);dragValid=true;hint="Yerini seç ve bırak · Esc: vazgeç";ui.Refresh();return true;
 }
 public bool ValidDeskPlacement(MovableDeskProp prop){var b=prop.Footprint();if(b.min.x< -4.56f||b.max.x>4.56f||b.min.z< -2.21f||b.max.z>2.21f)return false;
  if(b.min.x<1.28f&&b.max.x> -1.28f&&b.min.z<.02f&&b.max.z> -1.12f)return false;
  foreach(var other in FindObjectsByType<MovableDeskProp>()){if(other==prop||!other.CanMove)continue;var o=other.Footprint();if(b.min.x<o.max.x-.025f&&b.max.x>o.min.x+.025f&&b.min.z<o.max.z-.025f&&b.max.z>o.min.z+.025f)return false;}return true;}
 void EndDeskDrag(bool commit){var prop=movingProp;movingProp=null;if(!prop)return;StartCoroutine(SettleDeskProp(prop,commit));}
 IEnumerator SettleDeskProp(MovableDeskProp prop,bool commit){
  busy=true;Vector3 start=prop.transform.position,end=commit?start:dragHome;
  for(float elapsed=0;elapsed<.24f;elapsed+=Time.deltaTime){
   float t=Mathf.Clamp01(elapsed/.24f);float eased=Mathf.SmoothStep(0,1,t);
   prop.transform.position=Vector3.Lerp(start,end,eased)+Vector3.up*(commit?.045f:.075f)*Mathf.Sin(Mathf.PI*t)*(1-t*.45f);
   yield return null;
  }
  prop.transform.position=end;
  if(commit){var package=prop.GetComponent<PartsPackage>();if(package)package.deskPosition=end;DeskLayout.Save();hint="Masa düzenin kaydedildi.";}
  else hint="Burada yeterli boşluk yok; eşya yerine döndü.";
  busy=false;ui.Refresh();
 }
 public void ResetBenchView(){benchYaw=benchPitch=roomYaw=roomPitch=0;zoom=zoomTarget=1;}
 float benchYaw,benchPitch,zoom=1,zoomTarget=1,rightTravel;
 void CameraInput(Mouse mouse,bool overUI){if(mouse==null||busy||overUI)return;if(mouse.rightButton.wasPressedThisFrame)rightTravel=0;if(mouse.rightButton.isPressed){var delta=mouse.delta.ReadValue();rightTravel+=delta.magnitude;if(!held&&!selectedTool){if(inspection)orbit+=delta.x*.003f;else LookAround(delta);}}zoomTarget=Mathf.Clamp(zoomTarget*Mathf.Exp(-mouse.scroll.ReadValue().y*.0015f),.65f,1.16f);}
 }
}
