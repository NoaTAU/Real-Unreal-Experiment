using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ImageRatingExperiment : RatingExperiment<Sprite>
{
    public override string stimuliPath => "Images/2D";

    protected override string ExperimentType => "2D";
    private List<Sprite> _baselineList = new List<Sprite>();
    public override List<Sprite> baselineList => _baselineList;
    private String baselinePath = "Images/Baseline";
    private UnityEngine.UI.Image imageDisplay;
    private Sprite[] baselineArray;

    protected override void Start()
    {
        base.Start();
        imageDisplay = SceneReferencer.Instance.imageDisplay;
        imageDisplay.enabled = false; // Ensure the image display is initially hidden
        baselineArray = Resources.LoadAll<Sprite>(baselinePath);
        baselineList.AddRange(baselineArray);
        // Debug.Log($"Loaded {baselineList.Count} baseline images from {baselinePath}");
        Debug.Log(string.Join(", ", baselineList));
    }

    protected override void HideStimulus()
    {
        imageDisplay.enabled = false;
        SceneReferencer.Instance.twoDBackground.SetActive(false); // Hide the 2D background

    }

    protected override void ShowStimulus()
    {
        SceneReferencer.Instance.twoDBackground.SetActive(true); imageDisplay.sprite = stimuliListWithBaseline[currentStimulusIndex];
        imageDisplay.enabled = true;
    }
}