using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageRatingExperiment : RatingExperiment<GameObject>
{
    public override string stimuliPath => "Images/2D";

    protected override string ExperimentType => "2D";
    private List<GameObject> _baselineList = new List<GameObject>();
    public override List<GameObject> baselineList => _baselineList;
    public BgToggle bgToggle; // Reference to the BgToggle script to control the background
    private String baselinePath = "Images/Baseline";
    private GameObject currentInstance;
    private UnityEngine.UI.Image imageDisplay;
    private Sprite[] baselineArray;
    private Transform modelParent;

    protected override void Start()
    {
        base.Start();
        imageDisplay = SceneReferencer.Instance.imageDisplay;
        imageDisplay.enabled = false; // Ensure the image display is initially hidden
        var baselineArray = Resources.LoadAll<GameObject>(baselinePath);
        _baselineList.AddRange(baselineArray);

        Debug.Log("Loaded baseline prefabs: " + _baselineList.Count);
        // Debug.Log($"Loaded {baselineList.Count} baseline images from {baselinePath}");
        Debug.Log(string.Join(", ", baselineList));
    }

protected override void InitStimuli()
    {
        GameObject snacksPrefab = Resources.Load<GameObject>(stimuliPath);
        if (snacksPrefab == null)
        {
            Debug.LogError($"Could not load prefab at path: {stimuliPath}");
            return;
        }
        GameObject snacksInstance2D = Instantiate(snacksPrefab);
        snacksInstance2D.SetActive(false);
        modelParent = SceneReferencer.Instance.threeDDisplay.transform;

        // Collect each child as a separate stimulus
        stimuliList = new List<GameObject>();
        foreach (Transform child in snacksInstance.transform)
        {
            GameObject childClone = Instantiate(child.gameObject, modelParent);
            childClone.SetActive(false);
            if (childClone.name.StartsWith("Bamba red") || childClone.name.StartsWith("Baflot") || childClone.name.StartsWith("Bisli"))
            {
                // Debug.Log($"Adding {childClone.name} to baseline list");
                baselineList.Add(childClone);

            }
            else
            {
                stimuliList.Add(childClone);
                // Debug.Log($"Adding {childClone.name} to stimuliList list");
            }
        }

        // Optional: destroy the instance since we only needed its children
        Destroy(snacksInstance);

        TXRDataManager.Instance.LogLineToFile($"Loaded {stimuliList.Count} stimuli from prefab {stimuliPath}");
        LoadStimuliNames();
        ShuffleStimuliList();
        Debug.Log(string.Join(", ", stimuliList));
        // Debug.Log("Stimulus count: " + stimuliList.Count);
        LogHelper.Log("finished init stimuli", "blue");


    }
    protected override void HideStimulus()
    {
        bgToggle.UseSolid(); // Switch back to solid color background
        if (currentInstance != null)
        {
            Destroy(currentInstance);
            currentInstance = null;
        }

    }

    protected override void ShowStimulus()
    {
        bgToggle.UseSkybox(); // Use the skybox for 2D images
        if (currentInstance != null)
            Destroy(currentInstance);

        GameObject prefabToShow = stimuliListWithBaseline[currentStimulusIndex];
        currentInstance = Instantiate(prefabToShow, SceneReferencer.Instance.prefabSpawnPoint.position, SceneReferencer.Instance.prefabSpawnPoint.rotation);
        

        currentInstance.SetActive(true);
    }
}