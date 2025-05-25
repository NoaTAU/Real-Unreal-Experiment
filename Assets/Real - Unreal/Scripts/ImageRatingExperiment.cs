using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class ImageRatingExperiment : MonoBehaviour
{
    private Image imageDisplay;
    private Toggle confirmToggle;
    private Slider myMetaSlider;
    private GameObject metaUISliderGroup;
    private GameObject blackScreenOverlay;

    private List<Sprite> imageList;
    private int currentImageIndex = 0;
    private string logFilePath;
    private List<string> imageNames;
    private Sprite[] sprites;
    private bool inputReceived = false;

    float blackoutDuration;
    float imageDisplayDuration;


    void Start()
    {
        InitReferences();

        sprites = Resources.LoadAll<Sprite>("Images/2D");
        Object[] allAssets = Resources.LoadAll("Images/2D");
        imageList = new List<Sprite>(sprites);
        TXRDataManager.Instance.LogLineToFile($"Loaded {imageList.Count} images from Resources/Images/2D");
        // Uncomment the following line to create a new log file
        // File.WriteAllText(logFilePath, "ImageName,Ranking\n");
        // myMetaSlider = GetComponentInChildren<Slider>();
        // confirmToggle = GetComponent<Toggle>();

        LoadImages();
        ShuffleImages();

        Debug.Log("imageDisplay: " + imageDisplay);
        Debug.Log("imageList: " + imageList.Count);

        confirmToggle.isOn = false;
        confirmToggle.onValueChanged.AddListener(OnConfirmToggled);
        StartCoroutine(ShowImageSequence());
    }

    private void InitReferences()
    {
        // Initialize references to UI elements
        imageDisplay = SceneReferencer.Instance.imageDisplay;
        confirmToggle = SceneReferencer.Instance.confirmToggle;
        myMetaSlider = SceneReferencer.Instance.myMetaSlider;
        metaUISliderGroup = SceneReferencer.Instance.metaUISliderGroup;
        blackScreenOverlay = SceneReferencer.Instance.blackScreenOverlay;

        imageDisplayDuration = SceneReferencer.Instance.imageDisplayDuration;
        blackoutDuration = SceneReferencer.Instance.blackoutDuration;
    }

    void LoadImages()
    {
        imageNames = new List<string>();
        foreach (var img in imageList)
        {
            imageNames.Add(img.name);
        }
    }

    void ShuffleImages()
    {
        for (int i = 0; i < imageList.Count; i++)
        {
            var temp = imageList[i];
            int randomIndex = Random.Range(i, imageList.Count);
            imageList[i] = imageList[randomIndex];
            imageList[randomIndex] = temp;
        }
    }

    IEnumerator ShowImageSequence()
    {
        while (currentImageIndex < imageList.Count)
        {
            imageDisplay.sprite = imageList[currentImageIndex];
            imageDisplay.enabled = true;
            Debug.Log("Setting sprite: " + imageList[currentImageIndex].name);
            // imageDisplay.gameObject.SetActive(true);
            // myMetaSlider.gameObject.SetActive(false);

            yield return new WaitForSeconds(imageDisplayDuration);

            imageDisplay.enabled = false;
            metaUISliderGroup.SetActive(true);
            myMetaSlider.gameObject.SetActive(true);
            confirmToggle.interactable = true;


            float pollInterval = 0.1f;
            float nextCheckTime = Time.time;

            inputReceived = false;


            while (!inputReceived)
            {
                if (Time.time >= nextCheckTime)
                {
                    Debug.Log("Still waiting...");
                    nextCheckTime = Time.time + pollInterval;
                }
                Debug.Log($"[Loop] isOn: {confirmToggle.isOn}, inputReceived: {inputReceived}");
                yield return null; // ✅ This keeps the coroutine alive without freezing Unity

            }

            // inputReceived = false;
            currentImageIndex++;
            Debug.Log("Current image index: " + currentImageIndex);
            CanvasGroup canvas = blackScreenOverlay.GetComponent<CanvasGroup>();
            canvas.alpha = 1;
            blackScreenOverlay.SetActive(true);

            yield return new WaitForSeconds(blackoutDuration);

            canvas.alpha = 0;
            blackScreenOverlay.SetActive(false);
        }

        Debug.Log("Finished all images.");
    }

    IEnumerator Show3DSequence()
    {
        while (currentImageIndex < imageList.Count)
        {
            imageDisplay.sprite = imageList[currentImageIndex];
            imageDisplay.enabled = true;
            Debug.Log("Setting sprite: " + imageList[currentImageIndex].name);
            // imageDisplay.gameObject.SetActive(true);
            // myMetaSlider.gameObject.SetActive(false);

            yield return new WaitForSeconds(imageDisplayDuration);

            imageDisplay.enabled = false;
            metaUISliderGroup.SetActive(true);
            myMetaSlider.gameObject.SetActive(true);
            confirmToggle.interactable = true;


            float pollInterval = 0.1f;
            float nextCheckTime = Time.time;

            inputReceived = false;


            while (!inputReceived)
            {
                if (Time.time >= nextCheckTime)
                {
                    Debug.Log("Still waiting...");
                    nextCheckTime = Time.time + pollInterval;
                }
                Debug.Log($"[Loop] isOn: {confirmToggle.isOn}, inputReceived: {inputReceived}");
                yield return null; // ✅ This keeps the coroutine alive without freezing Unity

            }

            // inputReceived = false;
            currentImageIndex++;
            Debug.Log("Current image index: " + currentImageIndex);
            CanvasGroup canvas = blackScreenOverlay.GetComponent<CanvasGroup>();
            canvas.alpha = 1;
            blackScreenOverlay.SetActive(true);

            yield return new WaitForSeconds(blackoutDuration);

            canvas.alpha = 0;
            blackScreenOverlay.SetActive(false);
        }

        Debug.Log("Finished all images.");
    }
    void OnConfirmToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON

        float rating = myMetaSlider.value;
        Debug.Log($"Rating for {imageList[currentImageIndex].name}: {rating:F1}");

        confirmToggle.interactable = false;
        confirmToggle.isOn = false;

        metaUISliderGroup.SetActive(false);
        myMetaSlider.gameObject.SetActive(false);

        inputReceived = true;
        Debug.Log("inputReceived = true");
    }

}
