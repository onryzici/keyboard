using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor.Build.Reporting;
using LittleSwitch;
using Object = UnityEngine.Object;

// Editor-only inspection; never included in the player or saved into the scene.
public static class HDRPWorkshopReview
{
    public static void Inventory()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Workshop.unity");
        Directory.CreateDirectory("Logs");
        using var output = new StreamWriter("Logs/hdrp-before-inventory.txt");
        output.WriteLine("PROJECT: " + Application.dataPath);
        foreach (var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            output.WriteLine($"LIGHT {light.name} active={light.isActiveAndEnabled} type={light.type} intensity={light.intensity} position={light.transform.position} rotation={light.transform.eulerAngles}");
        foreach (var renderer in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (renderer.gameObject.activeInHierarchy)
                output.WriteLine($"RENDER {renderer.name} bounds={renderer.bounds} shadows={renderer.shadowCastingMode} materials={string.Join(",", renderer.sharedMaterials.Select(m => m ? m.name + ":" + m.shader.name : "NULL"))}");
        foreach (var volume in UnityEngine.Object.FindObjectsByType<Volume>(FindObjectsSortMode.None))
            output.WriteLine($"VOLUME {volume.name} {AssetDatabase.GetAssetPath(volume.sharedProfile)}");
        File.WriteAllText("Logs/hdrp-before-layout.json", Layout());
        Debug.Log("LITTLE_SWITCH_INVENTORY_READY");
    }

    public static string Layout()
    {
        return string.Join("\n", UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(m => !m.name.Contains("shaft") && !m.name.Contains("shaft", StringComparison.OrdinalIgnoreCase) && !m.GetComponent<TMPro.TMP_Text>())
            .Select(m => $"{GlobalObjectId.GetGlobalObjectIdSlow(m)}|{m.name}|{m.transform.position:F5}|{m.transform.rotation:F5}|{m.transform.lossyScale:F5}|{AssetDatabase.GetAssetPath(m.sharedMesh)}|{m.gameObject.activeSelf}").OrderBy(s => s));
    }

    static int frames, view;
    static RenderTexture target;
    static ShopGame game;
    static string prefix;
    public static void Capture() => Begin("hdrp");
    public static void Baseline() => Begin("urp-before");
    static void Begin(string name)
    {
        if (!EditorApplication.isPlaying) throw new InvalidOperationException("Run with play|HDRPWorkshopReview.Capture");
        game = UnityEngine.Object.FindAnyObjectByType<ShopGame>();
        game.enabled = false;
        prefix = name; view = 0; frames = 0;
        target = new RenderTexture(1600, 900, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        game.viewCamera.targetTexture = target;
        SetView();
        EditorApplication.update -= CaptureUpdate;
        EditorApplication.update += CaptureUpdate;
    }
    static void SetView()
    {
        Vector3[] positions = { new(1.8f,8,-10.6f), new(5.5f,7.4f,-5.0f), new(-6.8f,7.3f,-1), new(6.8f,7,-3.5f), new(-1.15f,9.6f,-2.8f) };
        Vector3[] aims = { new(0,5.2f,1), new(-7,6.8f,-.5f), new(-10.4f,8.2f,-1.45f), new(5,5.8f,1.6f), new(-1.15f,4.15f,-.55f) };
        game.viewCamera.transform.SetPositionAndRotation(positions[view], Quaternion.LookRotation(aims[view]-positions[view]));
    }
    static void CaptureUpdate()
    {
        if (!EditorApplication.isPlaying || !game) { EditorApplication.update -= CaptureUpdate; return; }
        if (++frames < 100) return;
        var old = RenderTexture.active; RenderTexture.active = target;
        var pixels = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
        pixels.ReadPixels(new Rect(0,0,target.width,target.height),0,0); pixels.Apply();
        File.WriteAllBytes($"Logs/{prefix}-{view+1}.png",pixels.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(pixels); RenderTexture.active = old;
        if (++view < 5) { frames = 0; SetView(); return; }
        EditorApplication.update -= CaptureUpdate;
        try { if (prefix == "hdrp") Validate(); }
        finally
        {
            game.viewCamera.targetTexture = null; target.Release(); UnityEngine.Object.DestroyImmediate(target);
            game.enabled = true;
            EditorApplication.isPlaying = false;
        }
        File.WriteAllText("Logs/"+prefix+"-capture-complete.txt",DateTime.UtcNow.ToString("O"));
        Debug.Log("LITTLE_SWITCH_CAPTURE_COMPLETE " + prefix);
        EditorApplication.isPlaying = false;
    }
    public static void Quit() => EditorApplication.Exit(0);
    public static void Refresh() => AssetDatabase.Refresh();
    public static void Play() { EditorSceneManager.OpenScene("Assets/Scenes/Workshop.unity"); EditorApplication.isPlaying = true; }

    public static void Validate()
    {
        if (!EditorApplication.isPlaying) throw new InvalidOperationException("Validate in Play Mode.");
        var shop = Object.FindAnyObjectByType<ShopGame>();
        if (!(RenderPipelineManager.currentPipeline is HDRenderPipeline)) throw new Exception("Actual running pipeline is not HDRP.");
        var renderers = Object.FindObjectsByType<Renderer>().Where(r => r.enabled && r.gameObject.activeInHierarchy).ToArray();
        var broken = renderers.SelectMany(r => r.sharedMaterials).Where(m => !m || !m.shader || !m.shader.isSupported || m.shader.name.Contains("InternalErrorShader") || m.shader.name.StartsWith("Universal Render Pipeline")).ToArray();
        if (broken.Length > 0) throw new Exception("Unsupported visible materials: " + string.Join(", ", broken.Select(m => m ? m.name : "NULL")));
        var camera = shop.viewCamera;
        Vector3 position = camera.transform.position; Quaternion rotation = camera.transform.rotation;
        bool boardActive = shop.board.gameObject.activeSelf;
        int hits = 0;
        try
        {
            shop.board.gameObject.SetActive(true);
            camera.transform.position = new Vector3(-.35f,8.1f,-2.1f); camera.transform.LookAt(new Vector3(-.35f,4.15f,-.55f));
            Physics.SyncTransforms();
            for (int i = 0; i < 61; i++)
            {
                var slots = shop.board.GetComponentsInChildren<KeySlot>().Where(s => s.index == i).ToArray();
                var aim = slots.Last().GetComponent<Collider>().bounds.center;
                var ray = camera.ScreenPointToRay(camera.WorldToScreenPoint(aim));
                if (Physics.Raycast(ray,out var hit,100) && hit.collider.GetComponentInParent<KeySlot>()?.index == i) hits++;
            }
        }
        finally { camera.transform.SetPositionAndRotation(position,rotation); shop.board.gameObject.SetActive(boardActive); }
        if (hits != 61) throw new Exception("Keyboard picking: " + hits + "/61.");
        var lights = Object.FindObjectsByType<Light>().Where(l=>l.isActiveAndEnabled).ToArray();
        var fog = HDCamera.GetOrCreate(camera).volumeStack.GetComponent<Fog>();
        if (!fog.enabled.value || !fog.enableVolumetricFog.value) throw new Exception("Volumetric fog is not active.");
        File.WriteAllText("Logs/hdrp-validation.txt", $"PROJECT={Application.dataPath}\nPIPELINE={RenderPipelineManager.currentPipeline.GetType().FullName}\nSUPPORTED_VISIBLE_MATERIALS={broken.Length == 0}\nKEY_PICKING={hits}/61\nVOLUMETRICS={fog.enableVolumetricFog.value}\nACTIVE_LIGHTS={lights.Length}\n" + string.Join("\n", lights.Select(l=>$"{l.name}: {l.type}, {l.intensity} {l.lightUnit}")));
        Debug.Log("LITTLE_SWITCH_HDRP_VALIDATED 61/61 keys; all visible materials supported; real HDRP active");
    }

    public static void BuildWindows()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Exit Play Mode first.");
        if (!(GraphicsSettings.currentRenderPipeline is HDRenderPipelineAsset)) throw new Exception("Build must use HDRP.");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[]{"Assets/Scenes/Workshop.unity"},
            locationPathName = "../Builds/WindowsHDRP/Little Switch.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        File.WriteAllText("Logs/hdrp-build.txt", report.summary.result + "\n" + report.summary.totalErrors + " errors\n" + report.summary.totalTime);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("HDRP player build failed.");
        Debug.Log("LITTLE_SWITCH_HDRP_BUILD_SUCCEEDED");
    }
}
