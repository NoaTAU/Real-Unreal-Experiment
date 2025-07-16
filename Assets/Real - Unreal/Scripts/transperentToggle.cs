using UnityEngine;
using UnityEngine.UI;

public class ToggleFillAlphaController : MonoBehaviour
{

    private Slider myMetaSlider;

    [Tooltip("Assign the Fill image (e.g., the white circle under Handle)")]
    public Image fillImage;

    private Color originalColor;

    void Awake()
    {
        if (fillImage != null)
        {
            originalColor = fillImage.color;
            originalColor.a = 255f; // Ensure the original color is fully opaque
        }
        myMetaSlider = SceneReferencer.Instance.myMetaSlider.GetComponent<Slider>();
        myMetaSlider.onValueChanged.AddListener(RestoreOriginal);
    }

    public void SetTransparent()
    {
        myMetaSlider.value = -1; // Reset slider to min value

        if (fillImage != null)
        {
            Color c = fillImage.color;
            c.a = 0f;
            fillImage.color = c;
        }
    }

    public void RestoreOriginal(float rating)
    {
        if (rating >= 0f)
        {
            fillImage.color = originalColor;
        }

    }
}
