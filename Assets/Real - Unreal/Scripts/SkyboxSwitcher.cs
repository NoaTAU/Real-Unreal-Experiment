using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class BgToggle : MonoBehaviour
{

    public string cameraSceneName = "base scene with meta and Slab Arena";
    public string cameraPath = "TXR Player For Meta/OVRCameraRig/TrackingSpace/CenterEyeAnchor";
    public Material panoramicSkybox;   // your Skybox/Panoramic material

    Camera cam;
    UniversalAdditionalCameraData urp;

    void Awake()
    {
        TryInitCamera(); // finds the camera even if it's in another loaded scene
    }

    bool TryInitCamera()
    {
        cam = Camera.main;
        if (!cam) cam = FindCameraAcrossScenes(cameraSceneName, cameraPath);
        if (!cam) { Debug.LogWarning("BgToggle: camera not found yet."); return false; }

        urp = cam.GetComponent<UniversalAdditionalCameraData>();
        if (urp && urp.renderType == CameraRenderType.Overlay)
        {
            Debug.LogWarning($"BgToggle: '{cam.name}' is an Overlay camera. Skybox renders only on a Base camera.");
        }
        return true;
    }

    Camera FindCameraAcrossScenes(string sceneName, string path)
    {
        Camera firstFound = null;

        for (int pass = 0; pass < 2; pass++)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (pass == 0 && !string.IsNullOrEmpty(sceneName) && s.name != sceneName) continue;
                if (pass == 1 && !string.IsNullOrEmpty(sceneName) && s.name == sceneName) continue;

                foreach (var root in s.GetRootGameObjects())
                {
                    // Try explicit OVR path first
                    if (!string.IsNullOrEmpty(path))
                    {
                        var t = root.transform.Find(path);
                        if (t)
                        {
                            var c = t.GetComponent<Camera>() ?? t.GetComponentInChildren<Camera>(true);
                            if (c)
                            {
                                var d = c.GetComponent<UniversalAdditionalCameraData>();
                                if (d == null || d.renderType == CameraRenderType.Base) return c;
                                firstFound ??= c; // remember overlay in case no base exists
                            }
                        }
                    }
                    // Fallback: any camera under this root
                    foreach (var c in root.GetComponentsInChildren<Camera>(true))
                    {
                        var d = c.GetComponent<UniversalAdditionalCameraData>();
                        if (d == null || d.renderType == CameraRenderType.Base) return c;
                        firstFound ??= c;
                    }
                }
            }
        }
        return firstFound ?? FindObjectOfType<Camera>(true);
    }

    public void UseSkybox()
    {
        if (!cam && !TryInitCamera()) return;

        if (panoramicSkybox) RenderSettings.skybox = panoramicSkybox;
        cam.clearFlags = CameraClearFlags.Skybox;   // <-- the actual switch
        DynamicGI.UpdateEnvironment();
    }

    public void UseSolid()
    {
        if (!cam && !TryInitCamera()) return;

        cam.clearFlags = CameraClearFlags.SolidColor; // <-- back to solid
        // (optional) cam.backgroundColor = Color.black; // only if you want to set a specific color
        DynamicGI.UpdateEnvironment();
    }
}