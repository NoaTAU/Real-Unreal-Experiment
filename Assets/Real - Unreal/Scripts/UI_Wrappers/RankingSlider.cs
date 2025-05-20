using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RankingSlider
{
    private GameObject rankingSlider;
    private TextMeshProUGUI valueText;
    private Toggle confirmButton;
    private Slider slider;


    public RankingSlider(GameObject slider)
    {
        this.rankingSlider = slider;
        valueText = slider.transform.Find("ValueText").GetComponent<TextMeshProUGUI>();
        confirmButton = slider.GetComponentInChildren<Toggle>();
        this.slider = slider.GetComponentInChildren<Slider>();
    }
    public void Show()
    {
        rankingSlider.SetActive(true);
    }
    public void Hide()
    {
        rankingSlider.SetActive(false);
    }

    public float ShowAndWaitForButtonPress()
    {
        Show();
        confirmButton.onValueChanged.AddListener(OnConfirmButtonPressed);
        return slider.value;
    }

    private void OnConfirmButtonPressed(bool isOn)
    {
        if (!isOn) return;
        Hide();
        float value = slider.value;
        TXRDataManager.Instance.LogLineToFile($"Rank button pressed - value");
        confirmButton.onValueChanged.RemoveListener(OnConfirmButtonPressed);
    }

}