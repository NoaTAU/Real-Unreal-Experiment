using UnityEngine;
using System.Collections;
using System.IO;

public class RenderTextureSaver : MonoBehaviour
{
    public RenderTexture renderTexture;

    void Start()
    {
        // Start the coroutine to delay saving until the next frame
        StartCoroutine(DelayedSave());
    }

    IEnumerator DelayedSave()
    {
        // Wait for end of frame to ensure camera has rendered
        yield return new WaitForEndOfFrame();

        SaveRenderTextureToPNG(renderTexture, "SavedModelImage.png");
    }

    void SaveRenderTextureToPNG(RenderTexture rt, string path)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D image = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        RenderTexture.active = currentRT;

        Debug.Log("Saved image to: " + path);
    }
}
