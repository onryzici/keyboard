using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using LittleSwitch.Environment;
using Object=UnityEngine.Object;

// Review tools operate the same CharacterController and player camera as normal play.
public static class WideWorkshopReview
{
    static WorkshopWalker walker;
    static Camera camera;
    static RenderTexture target;
    static Keyboard keyboard;
    static int index,frames;
    static bool walking;
    static double segmentStarted;
    static Vector3 previous;
    static float distance;
    static readonly Vector3[] Route={new(-3.65f,0,-2.05f),new(-3.80f,0,-.1f),new(-1.30f,0,1.35f),new(2.35f,0,2.60f),new(3.9f,0,1.0f),new(3.8f,0,-1.1f),new(3.5f,0,-2.6f),new(1.2f,0,-2.35f),new(-.5f,0,-1.2f),new(-3.35f,0,-3.3f)};
    static readonly Vector3[] Look={new(-5,1.4f,-2.7f),new(-4.9f,1,-.1f),new(-.9f,1.6f,3.7f),new(3.5f,1.2f,3.7f),new(5,1.4f,.05f),new(4.9f,1.4f,-1.9f),new(4.9f,1.4f,-2.6f),new(.4f,1.2f,-3.6f),new(-2.7f,.9f,-.5f),new(-.9f,1.5f,2.8f)};

    public static void Open() => EditorSceneManager.OpenScene(WideWorkshopRestore.ScenePath);
    public static void Refresh() => AssetDatabase.Refresh();
    public static void Quit() => EditorApplication.Exit(0);
    public static void Play() {Open();EditorApplication.isPlaying=true;}
    static void Begin(bool walk)
    {
        if(!EditorApplication.isPlaying)throw new Exception("Review requires Play Mode");
        walker=Object.FindAnyObjectByType<WorkshopWalker>();camera=walker.playerCamera;
        target=new RenderTexture(1600,900,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);camera.targetTexture=target;
        index=0;frames=0;distance=0;walking=walk;segmentStarted=EditorApplication.timeSinceStartup;previous=walker.transform.position;
        if(walk) {walker.reviewDriving=true;walker.reviewMovement=Vector2.up;}
        else {walker.enabled=false;Place();}
        EditorApplication.update-=Update;EditorApplication.update+=Update;
    }
    public static void Capture() => Begin(false);
    public static void Walk() => Begin(true);
    public static void Assembly() { walker=Object.FindAnyObjectByType<WorkshopWalker>(); walker.StartCoroutine(AssemblyCheck()); }
    static System.Collections.IEnumerator AssemblyCheck()
    {
        var visit=Object.FindAnyObjectByType<WorkshopVisit>();
        while(!visit.IsWalking)yield return null;
        visit.EnterWorkbench(true);
        if(!visit.shop.ui.TerminalOpen)throw new Exception("Computer terminal failed");
        visit.shop.ui.CloseTerminal();yield return new WaitForSeconds(1.2f);
        if(!visit.IsWalking)throw new Exception("Computer return failed");
        visit.EnterWorkbench(false);camera=visit.shop.viewCamera;
        target=new RenderTexture(1600,900,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);camera.targetTexture=target;
        yield return new WaitForSeconds(2);Save("wide-assembly");
        File.WriteAllText("Logs/wide-terminal.txt","PASS: separate computer order mode returns to walking; main bench assembly camera rendered.");
        Cleanup();EditorApplication.isPlaying=false;
    }
    static void Place()
    {
        var cc=walker.GetComponent<CharacterController>();cc.enabled=false;
        Vector3[] positions={new(-3.35f,.03f,-3.3f),new(-1.3f,.03f,1.4f),new(3.35f,.03f,1.45f),new(3.5f,.03f,-1.2f),new(1.6f,.03f,-2.2f),new(-3.7f,.03f,.75f)};
        Vector3[] aims={new(-.6f,1.45f,2.8f),new(-.9f,1.45f,3.8f),new(4.9f,1.35f,1.2f),new(4.95f,1.2f,-2.5f),new(-.1f,1.1f,-3.5f),new(-4.95f,1.2f,-1.7f)};
        walker.transform.position=WorkshopPolishPass.Map(positions[index]);cc.enabled=true;
        camera.transform.rotation=Quaternion.LookRotation(WorkshopPolishPass.Map(aims[index])-camera.transform.position);
    }
    static void Update()
    {
        if(!EditorApplication.isPlaying || !walker){Cleanup();return;}
        try
        {
            if(walking)
            {
                Vector3 current=walker.transform.position;distance+=Vector3.Distance(current,previous);previous=current;
                Vector3 direction=WorkshopPolishPass.Map(Route[index])-current;direction.y=0;
                if(direction.magnitude>.65f && frames==0)
                {
                    if(EditorApplication.timeSinceStartup-segmentStarted>22)throw new Exception("Walking route blocked at waypoint "+index+" position="+current+" target="+Route[index]);
                    walker.enabled=true;Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;walker.transform.rotation=Quaternion.LookRotation(direction); typeof(WorkshopWalker).GetField("targetYaw",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(walker,walker.transform.eulerAngles.y);
                    walker.reviewMovement=Vector2.up;return;
                }
                walker.reviewMovement=Vector2.zero;walker.enabled=false;
                camera.transform.rotation=Quaternion.LookRotation(WorkshopPolishPass.Map(Look[index])-camera.transform.position);
            }
            if(++frames<75)return;
            Save((walking?"wide-route-":"wide-view-")+index);
            index++;frames=0;segmentStarted=EditorApplication.timeSinceStartup;
            if(index<(walking?Route.Length:6)){if(!walking)Place();return;}
            Validate();
            File.WriteAllText("Logs/"+(walking?"wide-route-complete":"wide-capture-complete")+".txt",$"{DateTime.UtcNow:O}\nDistance={distance/4.3f:F2}m\nWaypoints={index}\n");
            Cleanup();EditorApplication.isPlaying=false;
        }
        catch(Exception e){File.WriteAllText("Logs/wide-review-error.txt",e.ToString());Debug.LogException(e);Cleanup();EditorApplication.isPlaying=false;}
    }
    static void Save(string name)
    {
        var active=RenderTexture.active;RenderTexture.active=target;var texture=new Texture2D(1600,900,TextureFormat.RGB24,false);
        texture.ReadPixels(new Rect(0,0,1600,900),0,0);texture.Apply();File.WriteAllBytes("Logs/"+name+".png",texture.EncodeToPNG());Object.DestroyImmediate(texture);RenderTexture.active=active;
    }
    static void Validate()
    {
        if(!(RenderPipelineManager.currentPipeline is HDRenderPipeline))throw new Exception("Actual pipeline is not HDRP");
        if(!Object.FindAnyObjectByType<LittleSwitch.ShopGame>())throw new Exception("Gameplay systems missing");
        var meshes=Object.FindObjectsByType<MeshFilter>();
        var invalid=Object.FindObjectsByType<Renderer>().Where(r=>r.enabled).SelectMany(r=>r.sharedMaterials).Where(m=>!m || !m.shader || !m.shader.isSupported).ToArray();
        if(invalid.Length>0)throw new Exception("Unsupported materials: "+string.Join(",",invalid.Select(m=>m?m.name:"NULL")));
        File.WriteAllText("Logs/wide-validation.txt",$"Project={Application.dataPath}\nScene={camera.gameObject.scene.path}\nHDRP=True\nGameplaySystems=1\nEyeHeight={camera.transform.localPosition.y}\nMeshRenderers={meshes.Length}\nImportedMeshes={meshes.Count(m=>AssetDatabase.GetAssetPath(m.sharedMesh).EndsWith(".fbx"))}\nActiveLights={Object.FindObjectsByType<Light>().Count(l=>l.isActiveAndEnabled)}\n");
    }
    static void Cleanup()
    {
        EditorApplication.update-=Update;
        if(keyboard!=null){InputSystem.RemoveDevice(keyboard);keyboard=null;}
        if(camera)camera.targetTexture=null;
        if(target){target.Release();Object.DestroyImmediate(target);}
        if(walker){walker.reviewDriving=false;walker.reviewMovement=Vector2.zero;walker.enabled=true;}
    }
    public static void BuildWindows()
    {
        if(EditorApplication.isPlaying)throw new Exception("Exit Play Mode");Open();
        var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{WideWorkshopRestore.ScenePath},locationPathName="../Builds/WalkableWorkshop/Little Switch Workshop.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
        File.WriteAllText("Logs/wide-build.txt",report.summary.result+"\nErrors="+report.summary.totalErrors);
        if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Environment build failed");
    }
}


