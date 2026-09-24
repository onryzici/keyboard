using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using LittleSwitch;
using LittleSwitch.Environment;
using Object=UnityEngine.Object;
public static class ToolAndCameraReview {
 static WorkshopVisit visit;static object Call(string n,params object[] a)=>typeof(ShopGame).GetMethod(n,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(visit.shop,a);
 static void Check(bool b,string s){if(!b)throw new Exception(s);}
 public static void Run(){visit=Object.FindAnyObjectByType<WorkshopVisit>();ShopSave.EditorSavePathOverride=Path.GetFullPath("Logs/tool-camera-isolated.json");EditorApplication.playModeStateChanged+=Exit;visit.StartCoroutine(Test());}
 static void Exit(PlayModeStateChange c){if(c==PlayModeStateChange.EnteredEditMode){ShopSave.EditorSavePathOverride=null;EditorApplication.playModeStateChanged-=Exit;}}
 static IEnumerator Test(){while(!visit.IsWalking)yield return null;visit.EnterWorkbench(false);var s=visit.shop;var tools=Object.FindObjectsByType<WorkbenchTool>();
  s.state=new BuildState();for(int i=0;i<61;i++){s.state.switches[i]=0;s.state.caps[i]=0;}s.state.stage=BuildStage.Test;s.state.switchPackageOpened=s.state.capPackageOpened=true;Call("RestorePackages");s.board.Refresh(s.state,s.catalog);s.ui.Refresh();
  Call("EquipTool",tools.First(t=>t.kind==WorkbenchTool.Kind.Puller));yield return (IEnumerator)Call("UseToolMotion",0);Check(s.state.caps[0]==-1&&s.state.switches[0]==0&&s.state.stage==BuildStage.Keycaps,"Cap extraction failed");
  yield return (IEnumerator)Call("UseToolMotion",0);Check(s.state.switches[0]==-1&&s.state.stage==BuildStage.Switches,"Switch extraction failed");Call("ReleaseTool");
  Call("PickUp");yield return (IEnumerator)Call("Install",0);Check(s.state.switches[0]==0,"Switch reinstallation failed");Call("PickUp");yield return (IEnumerator)Call("Install",0);Check(s.state.caps[0]==0,"Cap reinstallation failed");
  s.state.stage=BuildStage.Test;s.state.fault=17;Call("EquipTool",tools.First(t=>t.kind==WorkbenchTool.Kind.PinPliers));yield return (IEnumerator)Call("UseToolMotion",17);Check(s.state.fault==-1,"Pliers repair failed");Call("ReleaseTool");
  var rt=new RenderTexture(1600,900,24);s.viewCamera.targetTexture=rt;s.closeView=true;yield return new WaitForSeconds(1.5f);
  foreach(var p in Object.FindObjectsByType<PartsPackage>()){var v=s.viewCamera.WorldToViewportPoint(p.transform.position);Check(v.x>.08f&&v.x<.92f&&v.y>.12f&&v.y<.88f,"Supply outside composition: "+v);}
  var centre=s.viewCamera.WorldToViewportPoint(s.board.transform.position);Check(Mathf.Abs(centre.x-.5f)<.02f,"Keyboard not centered");
  var old=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();File.WriteAllBytes("Logs/focus-assembly.png",image.EncodeToPNG());Object.Destroy(image);RenderTexture.active=old;s.viewCamera.targetTexture=null;rt.Release();Object.Destroy(rt);
  visit.LeaveWorkbench();yield return new WaitForSeconds(1.05f);Check(visit.IsWalking&&visit.walker.enabled,"Return incomplete");Check(Mathf.Abs(s.viewCamera.fieldOfView-visit.walkingCamera.fieldOfView)<.01f,"FOV jump on return");Check(Quaternion.Angle(s.viewCamera.transform.rotation,visit.walkingCamera.transform.rotation)<.1f,"Rotation jump on return");
  File.WriteAllText("Logs/tool-camera-review.txt","PASS: cap extraction from test stage, switch extraction, physical reinstallation, pliers repair; centered board + both supplies in frame; exact matched FOV and rotation at return.");EditorApplication.isPlaying=false;
 }
}
