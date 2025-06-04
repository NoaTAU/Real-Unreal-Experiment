using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainExperiment : MonoBehaviour
{
    public Toggle endExperimentsToggle;
    public Toggle startExperimentsToggle;
    public Toggle FirstExperimentToggle;
    public GameObject showInstructionsButton; // Button to show instructions
    public GameObject showEndExperimentButton; // Button to end the experiment
    private ImageRatingExperiment imagerRatingExperiment;
    private ModelRatingExperiment modelRatingExperiment;
    private PassthroughRatingExperiment passthroughRatingExperiment;
    private bool readingInstructions = false;
    private bool experimentEnded = false;
    private bool experimentStrated = false;

    private void Start()
    {
        InitExperiments();

        endExperimentsToggle.interactable = false;
        endExperimentsToggle.isOn = false;
        endExperimentsToggle.onValueChanged.AddListener(OneEndToggled);

        FirstExperimentToggle.interactable = false;
        FirstExperimentToggle.isOn = false;
        FirstExperimentToggle.onValueChanged.AddListener(OneEndToggled);

        startExperimentsToggle.interactable = true;
        startExperimentsToggle.isOn = false;
        startExperimentsToggle.onValueChanged.AddListener(OnStartToggled);
        StartCoroutine(RunAllExperiments());
    }

    private IEnumerator RunAllExperiments()
    {
        FirstExperimentToggle.interactable = true;
        showInstructionsButton.SetActive(true); // Show the button to start reading instructions

        while (!experimentStrated)
        {
            yield return null;
        }

        showInstructionsButton.SetActive(false);
        FirstExperimentToggle.interactable = false;
        experimentEnded = false; // Set to true to start the experiments

        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the experiments

        while (!readingInstructions)
        {
            yield return null;
        }


        Debug.Log("Starting passthrough rating experiment...");
        TXRDataManager.Instance.LogLineToFile("Starting passthrough rating experiment...");
        yield return passthroughRatingExperiment.ShowImageSequence();

        showEndExperimentButton.SetActive(true);
        endExperimentsToggle.interactable = true;

        while (!experimentEnded)
        {
            yield return null;
        }

        experimentEnded = false;
        showEndExperimentButton.SetActive(false);
        endExperimentsToggle.interactable = false;

        Debug.Log("Starting model rating experiment...");
        TXRDataManager.Instance.LogLineToFile("Starting model rating experiment...");
        yield return modelRatingExperiment.ShowImageSequence();

        showEndExperimentButton.SetActive(true);
        endExperimentsToggle.interactable = true;

        while (!experimentEnded)
        {
            yield return null;
        }

        experimentEnded = false;
        showEndExperimentButton.SetActive(false);
        endExperimentsToggle.interactable = false;

        Debug.Log("Starting image rating experiment...");
        TXRDataManager.Instance.LogLineToFile("Starting image rating experiment...");
        yield return imagerRatingExperiment.ShowImageSequence();

        showEndExperimentButton.SetActive(true);
        endExperimentsToggle.interactable = true;

        while (!experimentEnded)
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
    private void OneEndToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON
        Debug.Log("toggle was toggled ON");
        readingInstructions = true;
        experimentEnded = true;
        Debug.Log("readingInstructions = true");
    }
    private void OnStartToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON
        experimentStrated = true;
        Debug.Log("started experiment = true");
    }
}
