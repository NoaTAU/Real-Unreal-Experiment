using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class PassthroughRatingExperiment : RatingExperiment<GameObject>
{
    public override string stimuliPath => "InvisibleCollider.prefab";
    private OVRPassthroughLayer passthroughLayer;
    private Transform passthroughColliderParent;

    public int[] passthroughtList = Enumerable.Range(0, 8).ToArray();

    protected override void Start()
    {
        base.Start();
        passthroughColliderParent = SceneReferencer.Instance.passthroughCollider.transform;
        passthroughLayer = gameObject.AddComponent<OVRPassthroughLayer>();
        // passthroughLayer.overlayType = OVRPassthroughLayer.OverlayType.Overlay;
        passthroughLayer.textureOpacity = 1.0f; // fully see-through

    }

    protected override void InitStimuli()
    {
        stimuliList = new List<GameObject>(); // or manually populate if you want
        stimuliNames = new List<string>(); // to match the base expectations

        // This avoids loading from Resources.
        LogHelper.Log("Passthrough round: skipping stimuli loading", "blue");
    }


    protected override void HideStimulus()
    {
        passthroughLayer.enabled = false;
    }

    protected override void ShowStimulus()
    {
        passthroughLayer.enabled = true;
    }

    public override IEnumerator ShowImageSequence()
    {
        InitConfirmToggle();
        int numRounds = passthroughtList.Length;

        for (int i = 0; i < numRounds; i++)
        {
            currentStimulusIndex = i;

            ShowStimulus();
            yield return new WaitForSeconds(stimulusDisplayDuration);
            HideStimulus();

            // rating UI
            metaUISliderGroup.SetActive(true);
            myMetaSlider.gameObject.SetActive(true);
            confirmToggle.interactable = true;
            inputReceived = false;

            while (!inputReceived)
            {
                yield return null;
            }

            // blackout
            CanvasGroup canvas = blackScreenOverlay.GetComponent<CanvasGroup>();
            canvas.alpha = 1;
            blackScreenOverlay.SetActive(true);
            yield return new WaitForSeconds(blackoutDuration);
            canvas.alpha = 0;
            blackScreenOverlay.SetActive(false);
        }

        Debug.Log("Finished all passthrough stimuli.");
    }

    protected override void OnConfirmToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON

        float rating = myMetaSlider.value;
        Debug.Log($"Rating for {currentStimulusIndex}: {rating:F2}");
        TXRDataManager.Instance.LogLineToFile($"Rating for {currentStimulusIndex}: {rating:F2}");

        confirmToggle.interactable = false;
        confirmToggle.isOn = false;

        metaUISliderGroup.SetActive(false);
        myMetaSlider.gameObject.SetActive(false);

        inputReceived = true;
        Debug.Log("inputReceived = true");
    }

}

