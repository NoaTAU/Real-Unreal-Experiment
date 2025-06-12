using UnityEngine;
using UnityEngine.UI;



public class ImageRatingExperiment : RatingExperiment<Sprite>
{
    public override string stimuliPath => "Images/2D";

    protected override string ExperimentType => "2D";

    private Image imageDisplay;

    protected override void Start()
    {
        base.Start();
        imageDisplay = SceneReferencer.Instance.imageDisplay;
        imageDisplay.enabled = false; // Ensure the image display is initially hidden
    }

    protected override void HideStimulus()
    {
        imageDisplay.enabled = false;
    }

    protected override void ShowStimulus()
    {
        imageDisplay.sprite = stimuliList[currentStimulusIndex];
        imageDisplay.enabled = true;
    }
}