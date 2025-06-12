using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MainExperiment : MonoBehaviour
{
    public Toggle ExperimentsToggle;
    public Toggle FirstExperimentToggle;
    public GameObject showInstructionsButton; // Button to show instructions
    public GameObject showExperimentButton;
    public GameObject invisibleCollider;
    public TMP_FontAsset defaultFont;
    private ImageRatingExperiment imagerRatingExperiment;
    private ModelRatingExperiment modelRatingExperiment;
    private PassthroughRatingExperiment passthroughRatingExperiment;
    private bool readingInstructions = false;
    private bool experimentToggle = false;
    private string startMessage = "לחצו כאן כדי להתחיל";
    private string endMessage = "הסבב הסתיים\n אנא קראו לנסיין/ית";
    private string experimentEndMessage = "הניסוי הסתיים\n תודה רבה על שיתוף הפעולה";
    private List<int> experimentList = new List<int> { 0, 1, 2 };
    private string textShuffledList = "";
    private TMP_Text generalInstructionsLabel;

    private TXRDataManager dataManager;



    private void Start()
    {
        Debug.Log("Debug: MainExperiment Start called");
        InitExperiments();

        ExperimentsToggle.interactable = false;
        ExperimentsToggle.isOn = false;
        ExperimentsToggle.onValueChanged.AddListener(OnExpToggled);

        FirstExperimentToggle.interactable = false;
        FirstExperimentToggle.isOn = false;
        FirstExperimentToggle.onValueChanged.AddListener(EndInstructionsToggled);
        Debug.Log("Debug: MainExperiment Start init completed");
        StartCoroutine(RunAllExperiments());

        ApplyFontToTMP(showExperimentButton);
        ApplyFontToTMP(showInstructionsButton);
        ShuffleExperimentOrder();
        ReportExperimentConfigurations();

    }
    private void ApplyFontToTMP(GameObject parent)
    {
        if (parent == null || defaultFont == null) return;

        TMP_Text[] texts = parent.GetComponentsInChildren<TMP_Text>(true);
        foreach (var text in texts)
        {
            text.font = defaultFont;
        }
    }

    private void ShuffleExperimentOrder()
    {
        textShuffledList = "";
        for (int i = 0; i < experimentList.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, experimentList.Count); // upper bound is exclusive
                                                                                 // Swap elements
            var temp = experimentList[i];
            experimentList[i] = experimentList[randomIndex];
            experimentList[randomIndex] = temp;
        }
        foreach (int id in experimentList)
        {
            switch (id)
            {
                case 0:
                    textShuffledList += "פסתרו, ";
                    break;
                case 1:
                    textShuffledList += "תלת מימד, ";
                    break;
                case 2:
                    textShuffledList += "דו מימד, ";
                    break;
            }
        }
        // Remove trailing comma + space
        if (textShuffledList.Length > 2)
            textShuffledList = textShuffledList.Substring(0, textShuffledList.Length - 2);
    }

    private IEnumerator RunAllExperiments()
    {
        Debug.Log("Debug:Starting RunAllExperiments...");
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the experiments

        yield return ShowDialogAndWaitForConfirm(textShuffledList);

        yield return ShowMainInstructionsAndWaitForConfirm();

        for (int i = 0; i < experimentList.Count; i++)
        {
            ShowDialogAndWaitForConfirm(startMessage);
            switch (experimentList[i])
            {
                case 0:
                    Debug.Log("Starting passthrough rating experiment...");
                    RendererActivator.Instance.HideRenderers(); // Hide the slab arena visuals
                    invisibleCollider.SetActive(true); // Show the invisible collider
                    TXRDataManager.Instance.LogLineToFile("Starting passthrough rating experiment...");
                    yield return passthroughRatingExperiment.ShowImageSequence();
                    invisibleCollider.SetActive(false); // Hide the invisible collider
                    break;
                case 1:
                    RendererActivator.Instance.ShowRenderers(); // Show the slab arena visuals
                    Debug.Log("Starting model rating experiment...");
                    TXRDataManager.Instance.LogLineToFile("Starting model rating experiment...");
                    yield return modelRatingExperiment.ShowImageSequence();
                    break;
                case 2:
                    RendererActivator.Instance.HideRenderers(); // Show the slab arena visuals
                    TXRDataManager.Instance.LogLineToFile("Starting image rating experiment...");
                    Debug.Log("Starting image rating experiment...");
                    yield return imagerRatingExperiment.ShowImageSequence();
                    break;
                default:
                    Debug.Log("Starting image rating experiment...");
                    Debug.LogError("Invalid experiment index: " + experimentList[i]);
                    break;
            }

            if (i != 2)
            {
                yield return ShowDialogAndWaitForConfirm(endMessage);
            }
            else
            {
                yield return ShowDialogAndWaitForConfirm(experimentEndMessage);
                Debug.Log("All experiments finished.");
                TXRDataManager.Instance.LogLineToFile("All experiments finished.");
            }

        }
    }



    private void InitExperiments()
    {
        imagerRatingExperiment = GetComponent<ImageRatingExperiment>();
        modelRatingExperiment = GetComponent<ModelRatingExperiment>();
        passthroughRatingExperiment = GetComponent<PassthroughRatingExperiment>();
        dataManager = TXRDataManager.Instance;
        generalInstructionsLabel = showExperimentButton.transform.Find("Dialog1Button_TextOnly/BodyText").GetComponentInChildren<TMP_Text>();
    }

    private IEnumerator ShowDialogAndWaitForConfirm(string InstructionsText)
    {
        TMP_Text label = showExperimentButton.transform.Find("Dialog1Button_TextOnly/BodyText").GetComponentInChildren<TMP_Text>();
        label.text = InstructionsText;
        ExperimentsToggle.interactable = true;
        showExperimentButton.SetActive(true);

        float appearanceTime = Time.time;

        while (!experimentToggle)
        {
            yield return null;
        }

        float confirmationTime = Time.time;

        experimentToggle = false;
        showExperimentButton.SetActive(false);
        ExperimentsToggle.interactable = false;
        ExperimentsToggle.isOn = false;

        dataManager.ReportInstructionsData(InstructionsText, appearanceTime, confirmationTime);
    }

    private IEnumerator ShowMainInstructionsAndWaitForConfirm()
    {
        // main experiment instructions:
        FirstExperimentToggle.interactable = true;
        showInstructionsButton.SetActive(true); // Show the button to start reading instructions

        while (!readingInstructions)
        {
            yield return null;
        }

        showInstructionsButton.SetActive(false);
        FirstExperimentToggle.interactable = false;
    }

    private void EndInstructionsToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON
        readingInstructions = true;
        Debug.Log("readingInstructions = true");
    }
    private void OnExpToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON
        experimentToggle = true;
        Debug.Log("toggle = true");
    }

    private void ReportExperimentConfigurations()
    {
        dataManager.ReportConfiguration("BlackoutDuration", SceneReferencer.Instance.blackoutDuration.ToString());
        dataManager.ReportConfiguration("StimulusDisplayDuration", SceneReferencer.Instance.stimulusDisplayDuration.ToString());
        dataManager.ReportConfiguration("ExperimentOrderString", textShuffledList);
        dataManager.ReportConfiguration("ExperimentOrderNumbers", string.Join(",", experimentList));
    }

}
