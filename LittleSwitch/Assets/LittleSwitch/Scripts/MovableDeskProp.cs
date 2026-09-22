using System; using System.IO; using System.Collections.Generic; using UnityEngine;
namespace LittleSwitch {
 public class MovableDeskProp:MonoBehaviour {
  public string id;
  public bool CanMove => !GetComponent<PartsPackage>() || GetComponent<PartsPackage>().IsOpen;
  public Bounds Footprint(){var rs=GetComponentsInChildren<Renderer>();var b=new Bounds(transform.position,Vector3.zero);bool first=true;foreach(var r in rs){if(r is ParticleSystemRenderer || r is LineRenderer)continue;if(first){b=r.bounds;first=false;}else b.Encapsulate(r.bounds);}return b;}
 }
 public static class DeskLayout {
  [Serializable] public class Entry {public string id;public Vector3 position;}
  [Serializable] class Data {public List<Entry> entries=new List<Entry>();}
  public static string PathName=>Path.Combine(Application.persistentDataPath,"desk-layout-v1.json");
  public static void Apply(){if(!File.Exists(PathName))return;try{var data=JsonUtility.FromJson<Data>(File.ReadAllText(PathName));if(data?.entries==null)return;foreach(var p in UnityEngine.Object.FindObjectsByType<MovableDeskProp>(FindObjectsSortMode.None)){var e=data.entries.Find(x=>x.id==p.id);if(e==null||!float.IsFinite(e.position.x)||!float.IsFinite(e.position.y)||!float.IsFinite(e.position.z)||Mathf.Abs(e.position.x)>4.6f||Mathf.Abs(e.position.z)>2.25f)continue;var package=p.GetComponent<PartsPackage>();if(package)package.deskPosition=new Vector3(e.position.x,package.deskPosition.y,e.position.z);else p.transform.position=new Vector3(e.position.x,p.transform.position.y,e.position.z);}}catch(Exception e){Debug.LogWarning("Desk layout could not be loaded: "+e.Message);}}
  public static void Save(){var data=new Data();foreach(var p in UnityEngine.Object.FindObjectsByType<MovableDeskProp>(FindObjectsSortMode.None)){var package=p.GetComponent<PartsPackage>();data.entries.Add(new Entry{id=p.id,position=package?package.deskPosition:p.transform.position});}try{File.WriteAllText(PathName+".tmp",JsonUtility.ToJson(data,true));if(File.Exists(PathName))File.Replace(PathName+".tmp",PathName,null);else File.Move(PathName+".tmp",PathName);}catch(Exception e){Debug.LogWarning("Desk layout could not be saved: "+e.Message);}}
 }
}
