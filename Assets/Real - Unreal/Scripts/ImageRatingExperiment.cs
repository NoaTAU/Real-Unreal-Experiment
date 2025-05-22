using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class ImageRatingExperiment : MonoBehaviour
{
    public Image imageDisplay;
    public Toggle confirmToggle;
    public Slider myMetaSlider;
    public GameObject metaUISliderGroup;


    private List<Sprite> imageList;
    private int currentImageIndex = 0;
    private string logFilePath;
    private List<string> imageNames;
    private Sprite[] sprites;
    private bool inputReceived = false;


    void Start()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "image_ratings.csv");
        sprites = Resources.LoadAll<Sprite>("Images/2D");
        Object[] allAssets = Resources.LoadAll("Images/2D");
        imageList = new List<Sprite>(sprites);
        File.WriteAllText(logFilePath, "ImageName,Ranking\n");
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
            Debug.Log("Setting sprite: " + imageList[currentImageIndex].name);
            imageDisplay.gameObject.SetActive(true);
            myMetaSlider.gameObject.SetActive(false);

            yield return new WaitForSeconds(3f);

            imageDisplay.gameObject.SetActive(false);
            metaUISliderGroup.SetActive(true);
            myMetaSlider.gameObject.SetActive(true);
            confirmToggle.interactable = true;

            inputReceived = false;
            // Wait until the user toggles ON
            yield return new WaitUntil(() => inputReceived);
            // inputReceived = false;
            currentImageIndex++;
            Debug.Log("Current image index: " + currentImageIndex);
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

        inputReceived = true; // ✅ This resumes the coroutine
    }

}
