using System.Collections;
using UnityEngine;
namespace LittleSwitch {
// Physical switch storage: opening a row also selects its switch family.
public class WorkshopDrawer : MonoBehaviour {
 public float travel=.85f;
 public Vector3 slideDirection=Vector3.back;
 public int switchFamily=-1;
 public bool IsOpen { get; private set; }
 Vector3 closedPosition;
 Coroutine motion;
 void Awake(){closedPosition=transform.localPosition;}
 public void Toggle(){
  IsOpen=!IsOpen;
  if(IsOpen&&switchFamily>=0){var shop=FindAnyObjectByType<ShopGame>();if(shop)shop.SelectSwitchFromDrawer(switchFamily);}
  if(motion!=null)StopCoroutine(motion);
  motion=StartCoroutine(Slide());
 }
 IEnumerator Slide(){
  Vector3 start=transform.localPosition;
  Vector3 end=closedPosition+(IsOpen?slideDirection*travel:Vector3.zero);
  for(float t=0;t<1;t+=Time.deltaTime/0.45f){
   transform.localPosition=Vector3.Lerp(start,end,Mathf.SmoothStep(0,1,t));
   yield return null;
  }
  transform.localPosition=end;motion=null;
 }
}
}
