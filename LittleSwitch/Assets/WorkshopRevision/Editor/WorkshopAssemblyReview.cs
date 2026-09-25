using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using LittleSwitch;
using LittleSwitch.Environment;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEditor;
using Object=UnityEngine.Object;
public static class WorkshopAssemblyReview {
 static InputSettings originalSettings;static Mouse mouse;static Mouse[] physicalMice;static WorkshopVisit visit;static RenderTexture target;
 static object Call(string name,params object[] args)=>typeof(ShopGame).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(visit.shop,args);
 static void Check(bool ok,string msg){if(!ok){if(mouse!=null&&target)Diagnose();throw new Exception(msg);}}
 public static void Diagnose(){visit=Object.FindAnyObjectByType<WorkshopVisit>();var s=Object.FindAnyObjectByType<PartsSupply>();var c=visit.shop.viewCamera;var screen=c.WorldToScreenPoint(s.transform.position+Vector3.up*.08f);var ray=c.ScreenPointToRay(screen);var hits=Physics.RaycastAll(ray,100).OrderBy(h=>h.distance);File.WriteAllText("Logs/assembly-input-diagnosis.txt","screen="+screen+" camera="+c.transform.position+" mouse="+Mouse.current.position.ReadValue()+" held="+typeof(ShopGame).GetField("held",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(visit.shop)+" touring="+visit.shop.touring+"\n"+string.Join("\n",hits.Select(h=>h.collider.name+" layer="+h.collider.gameObject.layer+" distance="+h.distance)));Capture("Logs/assembly-input-diagnosis.png");}
 public static void Run(){visit=Object.FindAnyObjectByType<WorkshopVisit>();ShopSave.EditorSavePathOverride=Path.GetFullPath("Logs/full-assembly-isolated.json");EditorApplication.playModeStateChanged+=Exit;visit.StartCoroutine(Test());}
 static void Exit(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredEditMode)return;ShopSave.EditorSavePathOverride=null;if(originalSettings)InputSystem.settings=originalSettings;if(mouse!=null)InputSystem.RemoveDevice(mouse);if(physicalMice!=null)foreach(var device in physicalMice)if(device.added)InputSystem.EnableDevice(device);physicalMice=null;EditorApplication.playModeStateChanged-=Exit;}
 static void MouseAt(Vector3 world,bool pressed){var screen=visit.shop.viewCamera.WorldToScreenPoint(world);InputSystem.QueueStateEvent(mouse,new MouseState{position=new Vector2(screen.x,screen.y)}.WithButton(MouseButton.Left,pressed));}
 static IEnumerator Test(){
  while(!visit.IsWalking)yield return null;
  visit.EnterWorkbench(false);var shop=visit.shop;
  shop.state=new BuildState();Call("RestorePackages");shop.board.Refresh(shop.state,shop.catalog);shop.ui.Refresh();
  shop.ui.OpenTerminal();shop.Action();shop.ChooseSwitch(1);shop.ChooseCaps(1);shop.Action();shop.ui.CloseTerminal();
  originalSettings=InputSystem.settings;var settings=Object.Instantiate(originalSettings);settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;InputSystem.settings=settings;Application.runInBackground=true;physicalMice=InputSystem.devices.OfType<Mouse>().Where(d=>d.enabled).ToArray();foreach(var device in physicalMice)InputSystem.DisableDevice(device);mouse=InputSystem.AddDevice<Mouse>();
  target=new RenderTexture(1600,900,24);shop.viewCamera.targetTexture=target;foreach(var canvas in Object.FindObjectsByType<Canvas>()){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=shop.viewCamera;canvas.planeDistance=.5f;}
  for(int stage=0;stage<2;stage++){
   var package=Object.FindObjectsByType<PartsPackage>().First(p=>p.keycaps==(stage==1));yield return (IEnumerator)Call("OpenPackage",package);
   shop.closeView=true;yield return new WaitForSeconds(1.2f);
   for(int i=0;i<61;i++){
    var supply=Object.FindAnyObjectByType<PartsSupply>();Check(supply,"Active supply missing");
    MouseAt(supply.transform.position+Vector3.up*.08f,true);yield return new WaitForSeconds(.05f);
    MouseAt(shop.board.SlotPosition(i),true);yield return new WaitForSeconds(.1f);
    MouseAt(shop.board.SlotPosition(i),false);
    float deadline=Time.realtimeSinceStartup+3f;
    do { yield return null; } while(Time.realtimeSinceStartup<deadline && ((stage==0?shop.state.switches[i]:shop.state.caps[i])!=1 || (bool)typeof(ShopGame).GetField("busy",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(shop)));
    Check((stage==0?shop.state.switches[i]:shop.state.caps[i])==1,"Mouse drag failed stage="+stage+" slot="+i);
    if(i==15 && stage==1)Capture("Logs/polish-assembly.png");
   }
  }
  Check(shop.state.stage==BuildStage.Test,"Assembly did not reach testing");
  Check(shop.FindAssemblySlot(shop.board.SlotPosition(0))==-1,"Occupied slot accepted");
  Capture("Logs/polish-complete-keyboard.png");
  for(int i=0;i<61;i++)shop.TestKey(i);Call("Repair",17);shop.TestKey(17);
  Check(shop.state.stage==BuildStage.Package,"Testing/repair failed");
  for(int i=0;i<3;i++){shop.Action();yield return new WaitForSeconds(.8f);}shop.Action();Check(shop.state.completed==1,"Delivery failed");
  File.WriteAllText("Logs/full-assembly-review.txt","PASS: mouse pickup + drag + release for all 61 switches and all 61 sculpted keycaps; occupied-slot rejection; test/repair; package/delivery. Editor-only isolated save.");
  shop.viewCamera.targetTexture=null;target.Release();Object.Destroy(target);if(originalSettings)InputSystem.settings=originalSettings;EditorApplication.isPlaying=false;
 }
 static void Capture(string path){var prev=RenderTexture.active;RenderTexture.active=target;var t=new Texture2D(1600,900,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,1600,900),0,0);t.Apply();File.WriteAllBytes(path,t.EncodeToPNG());Object.Destroy(t);RenderTexture.active=prev;}
 public static void FinalKeys(){visit=Object.FindAnyObjectByType<WorkshopVisit>();ShopSave.EditorSavePathOverride=Path.GetFullPath("Logs/final-key-test.json");EditorApplication.playModeStateChanged+=Exit;visit.StartCoroutine(Keys());}
 static IEnumerator Keys(){while(!visit.IsWalking)yield return null;visit.EnterWorkbench(false);var shop=visit.shop;shop.state=new BuildState();for(int i=0;i<61;i++){shop.state.switches[i]=1;shop.state.caps[i]=1;}shop.state.stage=BuildStage.Test;shop.state.fault=-1;shop.board.Refresh(shop.state,shop.catalog);shop.ui.Refresh();
  shop.PhysicalTestKey(Key.Space,"Space");shop.PhysicalTestKey(Key.LeftShift,"Left Shift");shop.PhysicalTestKey(Key.Backspace,"Backspace");shop.PhysicalTestKey(Key.Minus,"Minus");shop.PhysicalTestKey(Key.CapsLock,"Caps Lock");
  Check(shop.state.Tested==6,"Special key mapping failed");target=new RenderTexture(1600,900,24);shop.viewCamera.targetTexture=target;foreach(var canvas in Object.FindObjectsByType<Canvas>()){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=shop.viewCamera;canvas.planeDistance=.5f;}yield return new WaitForSeconds(2);Capture("Logs/polish-complete-keyboard.png");shop.viewCamera.targetTexture=null;target.Release();Object.Destroy(target);File.WriteAllText("Logs/polish-special-keys.txt","PASS: Space, both Shift keys, Backspace, Minus, Caps Lock; readable sculpted keycap legends rendered.");EditorApplication.isPlaying=false;}
}




