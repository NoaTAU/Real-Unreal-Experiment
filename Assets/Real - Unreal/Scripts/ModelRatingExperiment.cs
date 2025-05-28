using UnityEngine;

public class ModelRatingExperiment : RatingExperiment<GameObject>
{
    public override string stimuliPath => "3D";

    private GameObject currentInstantiatedModel;
    private Transform modelParent;

    protected override void Start()
    {
        base.Start();
        modelParent = SceneReferencer.Instance.threeDDisplay.transform;
    }

    protected override void HideStimulus()
    {
        Destroy(currentInstantiatedModel);
    }

    protected override void ShowStimulus()
    {
        currentInstantiatedModel = Instantiate(stimuliList[currentStimulusIndex], modelParent);
    }
}
