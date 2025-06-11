using System.Collections.Generic;
using UnityEngine;
public class ModelRatingExperiment : RatingExperiment<GameObject>
{
    public override string stimuliPath => "3D/Snacks";

    private GameObject currentInstantiatedModel;
    private Transform modelParent;

    protected override void Start()
    {
        base.Start();
        modelParent = SceneReferencer.Instance.threeDDisplay.transform;

    }
    protected override void InitStimuli()
    {
        GameObject snacksPrefab = Resources.Load<GameObject>(stimuliPath);
        if (snacksPrefab == null)
        {
            Debug.LogError($"Could not load prefab at path: {stimuliPath}");
            return;
        }
        GameObject snacksInstance = Instantiate(snacksPrefab);
        snacksInstance.SetActive(false);
        modelParent = SceneReferencer.Instance.threeDDisplay.transform;

        // Collect each child as a separate stimulus
        stimuliList = new List<GameObject>();
        foreach (Transform child in snacksInstance.transform)
        {
            GameObject childClone = Instantiate(child.gameObject, modelParent);
            childClone.SetActive(false);
            stimuliList.Add(childClone);
        }

        // Optional: destroy the instance since we only needed its children
        Destroy(snacksInstance);

        TXRDataManager.Instance.LogLineToFile($"Loaded {stimuliList.Count} stimuli from prefab {stimuliPath}");
        LoadStimuliNames();
        ShuffleStimuliList();
        Debug.Log("Stimulus count: " + stimuliList.Count);
        LogHelper.Log("finished init stimuli", "blue");
    }

    protected override void HideStimulus()
    {
        Destroy(currentInstantiatedModel);
    }

    protected override void ShowStimulus()
    {
        currentInstantiatedModel = Instantiate(stimuliList[currentStimulusIndex], modelParent);
        currentInstantiatedModel.SetActive(true);
    }
}
