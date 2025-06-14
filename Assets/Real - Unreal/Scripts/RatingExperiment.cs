using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class RatingExperiment<T> : MonoBehaviour where T : Object // Add constraint to ensure T is a UnityEngine.Object
{
    protected List<T> stimuliList;
    protected T[] stimuliArray;

    public abstract string stimuliPath { get; }

    protected int currentStimulusIndex = 0;
    protected List<string> stimuliNames;

    protected float blackoutDuration;
    protected float stimulusDisplayDuration;

    protected bool inputReceived = false;
    protected GameObject blackScreenOverlay;
    protected GameObject metaUISliderGroup;
    protected Slider myMetaSlider;
    protected Toggle confirmToggle;
    private float stimulusAppearanceTime;
    private float RatingApperanceTime;

    protected abstract string ExperimentType { get; }
    // Define the type of experiment, e.g., Image, Model, Passthrough, etc.


    #region ---- init ----
    protected virtual void Start()
    {
        InitReferences();
        InitStimuli();
        // InitConfirmToggle();
    }

    private void InitReferences()
    {
        // Initialize references to UI elements
        blackScreenOverlay = SceneReferencer.Instance.blackScreenOverlay;
        metaUISliderGroup = SceneReferencer.Instance.metaUISliderGroup;
        myMetaSlider = SceneReferencer.Instance.myMetaSlider;
        confirmToggle = SceneReferencer.Instance.confirmToggle;
        // Initialze references to configurations
        blackoutDuration = SceneReferencer.Instance.blackoutDuration;
        stimulusDisplayDuration = SceneReferencer.Instance.stimulusDisplayDuration;
    }

    protected virtual void InitStimuli()
    {
        stimuliArray = Resources.LoadAll<T>(stimuliPath); // This now works because of the added constraint
        stimuliList = new List<T>(stimuliArray);

        TXRDataManager.Instance.LogLineToFile($"Loaded {stimuliList.Count} stimuli from Resources/{stimuliPath}");
        LoadStimuliNames();
        ShuffleStimuliList();
        Debug.Log("Stimulus count: " + stimuliList.Count);
        LogHelper.Log("finished init stimuli", "blue");
    }

    public void LoadStimuliNames()
    {
        stimuliNames = new List<string>();
        foreach (T stimuli in stimuliList)
        {
            stimuliNames.Add(stimuli.name);
        }
    }

    public void ShuffleStimuliList()
    {
        for (int i = 0; i < stimuliList.Count; i++)
        {
            T temp = stimuliList[i];
            int randomIndex = Random.Range(i, stimuliList.Count);
            stimuliList[i] = stimuliList[randomIndex];
            stimuliList[randomIndex] = temp;
        }
    }

    public void InitConfirmToggle()
    {
        confirmToggle.onValueChanged.RemoveAllListeners();
        confirmToggle.interactable = false; // Initially disable the toggle
        confirmToggle.isOn = false;
        confirmToggle.onValueChanged.AddListener(OnConfirmToggled);
    }
    #endregion

    #region ---- main logic ----

    public virtual IEnumerator ShowImageSequence()
    {
        InitConfirmToggle();
        LogHelper.Log("currentStimulusIndex: " + currentStimulusIndex, "blue");
        LogHelper.Log("stimuliList: " + stimuliList.ToString(), "blue");
        while (currentStimulusIndex < stimuliList.Count)
        {
            ShowStimulus();
            stimulusAppearanceTime = Time.time;

            TXRDataManager.Instance.LogLineToFile("Showed Stimulus: " + stimuliList[currentStimulusIndex].name);
            Debug.Log("Showed Stimulus: " + stimuliList[currentStimulusIndex].name);

            yield return new WaitForSeconds(stimulusDisplayDuration);

            HideStimulus();

            // show slider and wait for input
            metaUISliderGroup.SetActive(true);
            myMetaSlider.gameObject.SetActive(true);
            confirmToggle.interactable = true;

            RatingApperanceTime = Time.time;

            inputReceived = false;


            while (!inputReceived)
            {

                yield return null;

            }

            currentStimulusIndex++;

            // blackout before next stimulus
            CanvasGroup canvas = blackScreenOverlay.GetComponent<CanvasGroup>();
            canvas.alpha = 1;
            blackScreenOverlay.SetActive(true);

            yield return new WaitForSeconds(blackoutDuration);

            canvas.alpha = 0;
            blackScreenOverlay.SetActive(false);
        }

        Debug.Log("Finished all stimuli.");
    }

    protected abstract void ShowStimulus();
    protected abstract void HideStimulus();


    protected virtual void OnConfirmToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON

        float rating = myMetaSlider.value;
        string stimulusName = stimuliList[currentStimulusIndex].name;

        // TXRDataManager.Instance.ReportExperimentData("RatingExperiment", stimulusName, rating.ToString(), 
        //     stimulusAppearanceTime.ToString(), RatingApperanceTime.ToString());

        Debug.Log($"Rating for {stimuliList[currentStimulusIndex].name}: {rating:F2}");
        TXRDataManager.Instance.LogLineToFile($"Rating for {stimuliList[currentStimulusIndex].name}: {rating:F2}");

        confirmToggle.interactable = false;
        confirmToggle.isOn = false;

        metaUISliderGroup.SetActive(false);
        myMetaSlider.gameObject.SetActive(false);

        inputReceived = true;
        Debug.Log("inputReceived = true");
    }

    #endregion

}
