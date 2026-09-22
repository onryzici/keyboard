using UnityEngine;
namespace LittleSwitch {
public class WorkshopSound : MonoBehaviour {
 AudioSource source,music; AudioClip[][] clicks; int[] previous={-1,-1,-1}; public bool muted;
 void Awake(){
  music=gameObject.AddComponent<AudioSource>();music.playOnAwake=false;music.loop=true;music.spatialBlend=0;music.volume=0;music.clip=Resources.Load<AudioClip>("Music/ChillLofi");if(music.clip){music.Play();StartCoroutine(FadeMusic());}
  source=gameObject.AddComponent<AudioSource>();source.spatialBlend=0;source.playOnAwake=false;
  clicks=new[]{Resources.LoadAll<AudioClip>("SwitchAudio/Linear"),Resources.LoadAll<AudioClip>("SwitchAudio/Tactile"),Resources.LoadAll<AudioClip>("SwitchAudio/Clicky")};
  for(int i=0;i<3;i++)if(clicks[i].Length==0)Debug.LogError("Missing recorded switch family "+i);
 }
 public void Click(int family,float volume=.5f){
  if(muted)return;int f=Mathf.Clamp(family,0,2);var bank=clicks[f];if(bank.Length==0)return;
  int index=Random.Range(0,bank.Length);if(bank.Length>1&&index==previous[f])index=(index+1)%bank.Length;previous[f]=index;
  source.pitch=1;source.PlayOneShot(bank[index],Mathf.Clamp01(volume));
 }
 System.Collections.IEnumerator FadeMusic(){for(float t=0;t<1;t+=Time.deltaTime/3f){music.volume=Mathf.SmoothStep(0,.13f,t);yield return null;}music.volume=.13f;}
 public void Toggle(){muted=!muted;source.mute=muted;if(music)music.mute=muted;}
}
}
