using UnityEngine;
using UnityEngine.UI;


// Stores references for everything needer to refer to in the scene.
public class SceneReferencer : TXRSingleton<SceneReferencer>
{
    [Header("Configurations")]
    public float blackoutDuration = 3.0f; // Duration of the black screen overlay
    public float stimulusDisplayDuration = 5.0f; // Duration to display each image

    [Header("Objects")]

    public Image imageDisplay;
    public Toggle confirmToggle;
    public Slider myMetaSlider;
    public GameObject metaUISliderGroup;
    public GameObject blackScreenOverlay;
    public GameObject threeDDisplay;

    // public GameObject passthroughDisplay;
    public GameObject passthroughCollider;


}
