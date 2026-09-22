using System.Collections;
using UnityEngine;
namespace LittleSwitch {
 public class HotDrink:MonoBehaviour {
  public ParticleSystem steam;
  public bool IsSipping {get;private set;}
  public int Sips {get;private set;}
  public IEnumerator Sip(Camera cam){
   if(IsSipping)yield break;IsSipping=true;
   var home=transform.position;var rotation=transform.rotation;
   if(steam)steam.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
   try{
    var target=cam.transform.TransformPoint(new Vector3(.22f,-.24f,.85f));
    var tilt=Quaternion.AngleAxis(-22,cam.transform.right)*rotation;
    for(float t=0;t<1;t+=Time.deltaTime/0.65f){float k=Mathf.SmoothStep(0,1,t);transform.position=Vector3.Lerp(home,target,k);transform.rotation=Quaternion.Slerp(rotation,tilt,k);yield return null;}
    yield return new WaitForSeconds(.7f);Sips++;
    var lifted=transform.position;var liftedRotation=transform.rotation;
    for(float t=0;t<1;t+=Time.deltaTime/.65f){float k=Mathf.SmoothStep(0,1,t);transform.position=Vector3.Lerp(lifted,home,k);transform.rotation=Quaternion.Slerp(liftedRotation,rotation,k);yield return null;}
   }finally{transform.SetPositionAndRotation(home,rotation);IsSipping=false;if(steam)steam.Play();}
  }
 }
}
