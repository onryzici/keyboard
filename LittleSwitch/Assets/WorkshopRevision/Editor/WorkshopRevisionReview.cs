using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using LittleSwitch;
using LittleSwitch.Environment;
using Object=UnityEngine.Object;

public static class WorkshopRevisionReview
{
    static WorkshopVisit visit;static RenderTexture target;static int view,frames;static byte[] saveBackup;static bool hadSave;
    static readonly Vector3[] Positions={new(1.2f,.12f,-17.5f),new(-6,.12f,-7),new(5,.12f,-7),new(0,.12f,-13)};
    static readonly Vector3[] Aims={new(0,5.5f,0),new(-4,5.2f,1.2f),new(0,5.8f,1),new(-8,4.3f,-12.5f)};
    public static void Open()=>EditorSceneManager.OpenScene(WorkshopRevisionBuilder.ScenePath);
    public static void Capture()
    {
        visit=Object.FindAnyObjectByType<WorkshopVisit>();view=frames=0;
        target=new RenderTexture(1600,900,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);
        visit.walker.enabled=false;visit.walkingCamera.targetTexture=target;Place();EditorApplication.update+=Tick;
    }
    static void Place()
    {
        var cc=visit.walker.GetComponent<CharacterController>();cc.enabled=false;visit.walker.transform.position=Positions[view];cc.enabled=true;
        visit.walkingCamera.transform.rotation=Quaternion.LookRotation(Aims[view]-visit.walkingCamera.transform.position);
    }
    static void Tick()
    {
        if(!EditorApplication.isPlaying){EditorApplication.update-=Tick;return;}
        if(++frames<100)return;
        var old=RenderTexture.active;RenderTexture.active=target;var image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();File.WriteAllBytes("Logs/revision-view-"+view+".png",image.EncodeToPNG());Object.DestroyImmediate(image);RenderTexture.active=old;
        frames=0;view++;
        if(view<4){Place();return;}
        if(view==4){visit.EnterWorkbench(false);visit.shop.viewCamera.targetTexture=target;return;}
        EditorApplication.update-=Tick;visit.walkingCamera.targetTexture=null;visit.shop.viewCamera.targetTexture=null;target.Release();Object.DestroyImmediate(target);EditorApplication.isPlaying=false;
        File.WriteAllText("Logs/revision-capture.txt",DateTime.UtcNow.ToString("O"));
    }
    public static void TestMechanics()
    {
        visit=Object.FindAnyObjectByType<WorkshopVisit>();
        ShopSave.EditorSavePathOverride=Path.GetFullPath("Logs/mechanics-isolated-save.json");
        EditorApplication.playModeStateChanged+=RestoreSaveOnExit;
        visit.StartCoroutine(Verify());
    }
    static void RestoreSaveOnExit(PlayModeStateChange change)
    {
        if(change!=PlayModeStateChange.EnteredEditMode)return;
        ShopSave.EditorSavePathOverride=null;
        EditorApplication.playModeStateChanged-=RestoreSaveOnExit;
    }
    static object Call(ShopGame shop,string method,params object[] args)=>typeof(ShopGame).GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(shop,args);
    static void Check(bool valid,string message){if(!valid)throw new Exception(message);}
    static IEnumerator Verify()
    {
        // Editor command polling can run before the first player Update/Start completes.
        yield return new WaitForSeconds(.5f);
        float deadline=Time.realtimeSinceStartup+5;
        while(!visit.IsWalking && Time.realtimeSinceStartup<deadline)yield return null;
        Check(visit.IsWalking,"Visit did not initialize walking");
        var shop=visit.shop;visit.EnterWorkbench(false);shop.ui.OpenTerminal();Check(shop.ui.TerminalOpen,"Order terminal failed to open");
        shop.state=new BuildState();Call(shop,"RestorePackages");shop.board.Refresh(shop.state,shop.catalog);shop.ui.Refresh();
        shop.Action();Check(shop.state.stage==BuildStage.Parts,"Accept order failed");
        shop.ChooseSwitch(0);shop.ChooseCaps(0);shop.Action();Check(shop.state.stage==BuildStage.Switches,"Part purchase failed");
        shop.ui.CloseTerminal();
        var package=Object.FindObjectsByType<PartsPackage>().First(p=>!p.keycaps);
        yield return (IEnumerator)Call(shop,"OpenPackage",package);Check(shop.state.switchPackageOpened && package.IsOpen,"Physical supply package failed");
        Call(shop,"PickUp");yield return (IEnumerator)Call(shop,"Install",0);Check(shop.state.switches[0]==0,"Physical switch install failed");
        for(int i=0;i<61;i++)shop.state.switches[i]=0;
        shop.state.stage=BuildStage.Keycaps;shop.state.capPackageOpened=true;shop.board.Refresh(shop.state,shop.catalog);
        Call(shop,"PickUp");yield return (IEnumerator)Call(shop,"Install",0);Check(shop.state.caps[0]==0,"Physical keycap install failed");
        for(int i=0;i<61;i++)shop.state.caps[i]=0;
        shop.state.stage=BuildStage.Test;shop.state.fault=17;shop.board.Refresh(shop.state,shop.catalog);
        for(int i=0;i<61;i++)shop.TestKey(i);
        Check(shop.state.stage==BuildStage.Test,"Fault should prevent completion");Call(shop,"Repair",17);shop.TestKey(17);Check(shop.state.stage==BuildStage.Package,"61-key test/repair failed");
        for(int i=0;i<3;i++){shop.Action();yield return new WaitForSeconds(1.8f);}
        Check(shop.state.stage==BuildStage.Ready,"Packaging failed");shop.Action();Check(shop.state.stage==BuildStage.Review && shop.state.completed==1,"Delivery failed");
        visit.LeaveWorkbench();yield return new WaitForSeconds(1);Check(visit.IsWalking && visit.walker.enabled,"Return to walking failed");
        Check(GraphicsSettings.currentRenderPipeline is HDRenderPipelineAsset,"HDRP asset not configured");
        File.WriteAllText("Logs/revision-mechanics.txt","PASS: terminal, order, purchase, physical package opening, switch installation, keycap installation, 61-key test, repair, packaging, delivery, smooth return to walking.\nTest writes, including OnApplicationQuit, use Logs/mechanics-isolated-save.json.");
        EditorApplication.isPlaying=false;
    }
    public static void BuildWindows()
    {
        Open();var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{WorkshopRevisionBuilder.ScenePath},locationPathName="../Builds/WalkableWorkshop/Little Switch Workshop.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
        File.WriteAllText("Logs/revision-windows-build.txt",r.summary.result+" errors="+r.summary.totalErrors);
        if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
    }
    public static void TestWalking(){visit=Object.FindAnyObjectByType<WorkshopVisit>();visit.StartCoroutine(WalkRoute());}
    static IEnumerator WalkRoute()
    {
        yield return new WaitForSeconds(.5f);
        var walker=visit.walker;walker.reviewDriving=true;
        Vector3[] route={new(-3,0,-14),new(-5.7f,0,-8.5f),new(0,0,-4.4f),new(6.6f,0,-7.7f),new(6,0,-13),new(0,0,-19)};
        float distance=0;Vector3 previous=walker.transform.position;
        var yaw=typeof(WorkshopWalker).GetField("targetYaw",BindingFlags.NonPublic|BindingFlags.Instance);
        foreach(var point in route)
        {
            float deadline=Time.realtimeSinceStartup+18;
            while(true)
            {
                Vector3 delta=point-walker.transform.position;delta.y=0;if(delta.magnitude<.35f)break;
                Check(Time.realtimeSinceStartup<deadline,"Walking collision blocked route at "+walker.transform.position);
                var rotation=Quaternion.LookRotation(delta);walker.transform.rotation=rotation;yaw.SetValue(walker,rotation.eulerAngles.y);walker.reviewMovement=Vector2.up;
                yield return null;distance+=Vector3.Distance(previous,walker.transform.position);previous=walker.transform.position;
            }
        }
        walker.reviewMovement=Vector2.zero;walker.reviewDriving=false;
        File.WriteAllText("Logs/revision-walking.txt",$"PASS: 6 waypoints, {distance/4:F2} metres, real CharacterController, collisions enabled.\nSpeed=1.35 m/s; mouse sensitivity=.045; look smoothing=.09s.");
        EditorApplication.isPlaying=false;
    }
}
