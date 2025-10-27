using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageRatingExperiment : RatingExperiment<GameObject>
{
    public override string stimuliPath => "Images/2D";
    public GameObject twoDBackground;
    protected override string ExperimentType => "2D";
    private List<GameObject> _baselineList = new List<GameObject>();
    public override List<GameObject> baselineList => _baselineList;
    public BgToggle bgToggle; // Reference to the BgToggle script to control the background
    private string baselinePath = "Images/Baseline";
    private GameObject currentInstance;
    private UnityEngine.UI.Image imageDisplay;
    // private Sprite[] baselineArray;

    protected override void Start()
    {
       
        
        Debug.Log("stop 0");
        imageDisplay = SceneReferencer.Instance.imageDisplay;
        Debug.Log("stop 1");
        imageDisplay.enabled = false; // Ensure the image display is initially hidden
        Debug.Log("stop 2");
        var baselineArray = Resources.LoadAll<GameObject>(baselinePath);
        Debug.Log("stop 3");
        _baselineList.AddRange(baselineArray);
        Debug.Log("stop 4");
        Debug.Log("Loaded baseline prefabs: " + _baselineList.Count);
        base.Start();
        // Debug.Log($"Loaded {baselineList.Count} baseline images from {baselinePath}");
        Debug.Log(string.Join(", ", baselineList));
    }

    protected override void HideStimulus()
    {
        bgToggle.UseSolid(); // Switch back to solid color background
        if (currentInstance != null)
        {
            Destroy(currentInstance);
            currentInstance = null;
            // twoDBackground.SetActive(false);
        }

    }

    protected override void ShowStimulus()
    {
        // twoDBackground.SetActive(true);
        bgToggle.UseSkybox(); // Use the skybox for 2D images
        if (currentInstance != null)
            Destroy(currentInstance);

        GameObject prefabToShow = stimuliListWithBaseline[currentStimulusIndex];
        currentInstance = Instantiate(prefabToShow, SceneReferencer.Instance.prefabSpawnPoint.position, SceneReferencer.Instance.prefabSpawnPoint.rotation);
        

        currentInstance.SetActive(true);
    }
}