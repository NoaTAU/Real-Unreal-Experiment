using System.Collections.Generic;
using UnityEngine;
public class ModelRatingExperiment : RatingExperiment<GameObject>
{
    public override string stimuliPath => "3D/Snacks";

    protected override string ExperimentType => "3D";
    private List<GameObject> _baselineList = new List<GameObject>();
    public override List<GameObject> baselineList => _baselineList;
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
            if (childClone.name == "Bamba red(Clone)" || childClone.name == "Baflot(Clone)" || childClone.name == "Bisli(Clone)")
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
        Destroy(currentInstantiatedModel);
        RendererActivator.Instance.HideRenderers();
    }

    protected override void ShowStimulus()
    {
        RendererActivator.Instance.ShowRenderers();
        currentInstantiatedModel = Instantiate(stimuliListWithBaseline[currentStimulusIndex], modelParent);
        currentInstantiatedModel.SetActive(true);
    }
}
