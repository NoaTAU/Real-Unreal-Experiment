using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RendererActivator : TXRSingleton<RendererActivator>
{
    public Transform slabArenaRoot;

    private List<Renderer> rendererList = new List<Renderer>();
    void Start()
    {
        {
            if (slabArenaRoot != null)
            {
                rendererList = slabArenaRoot.GetComponentsInChildren<Renderer>().ToList();
                Debug.Log($"Found {rendererList.Count} renderers in slabArenaRoot.");
            }
            else
            {
                Debug.LogWarning("slabArenaRoot is not assigned.");
            }
        }
    }
    // Public method to hide visuals
    public void HideRenderers()
    {
        if (slabArenaRoot == null)
        {
            Debug.LogError("Slab Arena root is not assigned!");
            return;
        }
        foreach (var rend in rendererList)
        {
            rend.enabled = false;
        }

        Debug.Log($"disabled {rendererList.Count} renderers.");

    }

    // Public method to show visuals
    public void ShowRenderers()
    {
        if (slabArenaRoot == null)
        {
            Debug.LogError("Slab Arena root is not assigned!");
            return;
        }
        foreach (var rend in rendererList)
        {
            rend.enabled = true;
        }

        Debug.Log($"enabled {rendererList.Count} renderers.");
    }


}
