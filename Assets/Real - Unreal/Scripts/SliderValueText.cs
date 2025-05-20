using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
    public Slider sliderComponent;
    private TextMeshProUGUI valueText;
    private void Start()
    {
        valueText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        valueText.text = sliderComponent.value.ToString("0.0");
    }
}
