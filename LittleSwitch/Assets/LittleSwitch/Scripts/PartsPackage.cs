using System.Collections;
using UnityEngine;
namespace LittleSwitch {
 public class PartsPackage:MonoBehaviour {
  public bool keycaps;
  public Transform leftFlap,rightFlap;
  public GameObject seal;
  public Vector3 shelfPosition,deskPosition;
  public bool IsOpen {get;private set;}
  void Flaps(float t){if(leftFlap)leftFlap.localRotation=Quaternion.Euler(0,0,115*t);if(rightFlap)rightFlap.localRotation=Quaternion.Euler(0,0,-115*t);}
  public void Present(bool opened){IsOpen=opened;GetComponent<Collider>().enabled=!opened;transform.position=opened?deskPosition:shelfPosition;Flaps(opened?1:0);if(seal)seal.SetActive(!opened);}
  public IEnumerator BringAndOpen(){
   Vector3 start=transform.position;
   for(float t=0;t<1;t+=Time.deltaTime/.85f){float k=Mathf.SmoothStep(0,1,t);transform.position=Vector3.Lerp(start,deskPosition,k)+Vector3.up*Mathf.Sin(t*Mathf.PI)*.4f;yield return null;}
   transform.position=deskPosition;if(seal)seal.SetActive(false);
   for(float t=0;t<1;t+=Time.deltaTime/.65f){Flaps(Mathf.SmoothStep(0,1,t));yield return null;}
   Flaps(1);IsOpen=true;GetComponent<Collider>().enabled=false;
  }
 }
}
