using System.Collections;
using UnityEngine;

namespace LittleSwitch.Environment
{
    // Apply the workshop's display default even when an older window size is cached.
    public static class WorkshopDisplay
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
#if UNITY_STANDALONE && !UNITY_EDITOR
            var walker = Object.FindAnyObjectByType<WorkshopWalker>();
            if (walker) walker.StartCoroutine(Apply());
#endif
        }

        static IEnumerator Apply()
        {
            Screen.SetResolution(3840, 2160, FullScreenMode.FullScreenWindow);
            yield return new WaitForSecondsRealtime(1);
            Debug.Log($"Workshop display: {Screen.width}x{Screen.height}, {Screen.fullScreenMode}");
        }
    }
}
