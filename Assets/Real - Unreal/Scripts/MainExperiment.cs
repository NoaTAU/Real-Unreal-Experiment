using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainExperiment : MonoBehaviour
{
    public Toggle ExperimentsToggle;
    public Toggle FirstExperimentToggle;
    public GameObject showInstructionsButton; // Button to show instructions
    public GameObject showExperimentButton;
    private ImageRatingExperiment imagerRatingExperiment;
    private ModelRatingExperiment modelRatingExperiment;
    private PassthroughRatingExperiment passthroughRatingExperiment;
    private bool readingInstructions = false;
    private bool experimentToggle = false;
    private string startMessage = "לחצו כאן כדי להתחיל";
    private string endMessage = "הסבב הסתיים/n אנא קראו לנסיין/ית";


    private void Start()
    {
        InitExperiments();

        ExperimentsToggle.interactable = false;
        ExperimentsToggle.isOn = false;
        ExperimentsToggle.onValueChanged.AddListener(OnExpToggled);

        FirstExperimentToggle.interactable = false;
        FirstExperimentToggle.isOn = false;
        FirstExperimentToggle.onValueChanged.AddListener(EndInstructionsToggled);

        StartCoroutine(RunAllExperiments());
    }

    private IEnumerator RunAllExperiments()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the experiments

        FirstExperimentToggle.interactable = true;
        showInstructionsButton.SetActive(true); // Show the button to start reading instructions

        while (!readingInstructions)
        {
            yield return null;
        }

        showInstructionsButton.SetActive(false);
        FirstExperimentToggle.interactable = false;
        experimentToggle = false; // Set to true to start the experiments

        TMP_Text label = showExperimentButton.transform.Find("Dialog1Button_TextOnly/BodyText").GetComponentInChildren<TMP_Text>();
        label.text = startMessage;
        ExperimentsToggle.interactable = true;
        showExperimentButton.SetActive(true);

        while (!experimentToggle)
        {
            yield return null;
        }

        experimentToggle = false;
        showExperimentButton.SetActive(false);
        ExperimentsToggle.interactable = false;

        Debug.Log("Starting passthrough rating experiment...");
        TXRDataManager.Instance.LogLineToFile("Starting passthrough rating experiment...");
        yield return passthroughRatingExperiment.ShowImageSequence();

        label.text = endMessage;
        showExperimentButton.SetActive(true);
        ExperimentsToggle.interactable = true;

        while (!experimentToggle)
        {
            yield return null;
        }

        experimentToggle = false;
        showExperimentButton.SetActive(false);
        ExperimentsToggle.interactable = false;

        label.text = startMessage;
        ExperimentsToggle.interactable = true;
        showExperimentButton.SetActive(true);

        while (!experimentToggle)
        {
            yield return null;
        }

        experimentToggle = false;
        showExperimentButton.SetActive(false);
        ExperimentsToggle.interactable = false;


        Debug.Log("Starting model rating experiment...");
        TXRDataManager.Instance.LogLineToFile("Starting model rating experiment...");
        yield return modelRatingExperiment.ShowImageSequence();

        label.text = endMessage;
        showExperimentButton.SetActive(true);
        ExperimentsToggle.interactable = true;

        while (!experimentToggle)
        {
            yield return null;
        }

        experimentToggle = false;
        showExperimentButton.SetActive(false);
        ExperimentsToggle.interactable = false;

        label.text = startMessage;
        ExperimentsToggle.interactable = true;
        showExperimentButton.SetActive(true);

        while (!experimentToggle)
        {
            yield return null;
        }

        experimentToggle = false;
        showExperimentButton.SetActive(false);
        ExperimentsToggle.interactable = false;

        Debug.Log("Starting image rating experiment...");
        TXRDataManager.Instance.LogLineToFile("Starting image rating experiment...");
        yield return imagerRatingExperiment.ShowImageSequence();

        label.text = endMessage;
        showExperimentButton.SetActive(true);
        ExperimentsToggle.interactable = true;

        while (!experimentToggle)
        {
            yield return null;
        }

        Debug.Log("All experiments finished.");
        TXRDataManager.Instance.LogLineToFile("All experiments finished.");
    }

    private void InitExperiments()
    {
        imagerRatingExperiment = GetComponent<ImageRatingExperiment>();
        modelRatingExperiment = GetComponent<ModelRatingExperiment>();
        passthroughRatingExperiment = GetComponent<PassthroughRatingExperiment>();
    }
    private void EndInstructionsToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON
        Debug.Log("toggle was toggled ON");
        readingInstructions = true;
        Debug.Log("readingInstructions = true");
    }
    private void OnExpToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON
        experimentToggle = true;
        Debug.Log("toggle = true");
    }
}
