using System.Collections;
using UnityEngine;

public class MainExperiment : MonoBehaviour
{
    private ImageRatingExperiment imagerRatingExperiment;
    private ModelRatingExperiment modelRatingExperiment;


    private void Start()
    {
        InitExperiments();

        StartCoroutine(RunAllExperiments());
    }

    private IEnumerator RunAllExperiments()
    {

        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the experiments
        Debug.Log("Starting model rating experiment...");
        TXRDataManager.Instance.LogLineToFile("Starting model rating experiment...");
        yield return modelRatingExperiment.ShowImageSequence();

        Debug.Log("Starting image rating experiment...");
        TXRDataManager.Instance.LogLineToFile("Starting image rating experiment...");
        yield return imagerRatingExperiment.ShowImageSequence();

        Debug.Log("All experiments finished.");
        TXRDataManager.Instance.LogLineToFile("All experiments finished.");
    }

    private void InitExperiments()
    {
        imagerRatingExperiment = GetComponent<ImageRatingExperiment>();
        modelRatingExperiment = GetComponent<ModelRatingExperiment>();
    }

}
