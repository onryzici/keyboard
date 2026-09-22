using UnityEngine;
namespace LittleSwitch {
 public class LanternFlicker:MonoBehaviour {
  public Light source;
  public Renderer glass;
  public float intensity=5.2f;
  MaterialPropertyBlock block;
  void Awake(){block=new MaterialPropertyBlock();}
  void Update(){
   float t=Time.time;float wave=(Mathf.PerlinNoise(t*1.3f,3.7f)-.5f)*.20f+(Mathf.PerlinNoise(t*3.7f,9.2f)-.5f)*.065f;
   float strength=1+wave;if(source)source.intensity=intensity*strength;
   if(glass){glass.GetPropertyBlock(block);block.SetColor("_EmissionColor",new Color(1.5f,.94f,.39f)*strength);glass.SetPropertyBlock(block);}
  }
 }
}
