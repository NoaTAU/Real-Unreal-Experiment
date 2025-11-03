using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unified version of RatingExperiment<T> + ImageRatingExperiment,
/// specialized for 2D GameObject stimuli.
/// It pre-spawns children under twoDDisplay (inactive),
/// builds + shuffles a merged list, then activates/deactivates per trial.
/// </summary>
public class ImageRatingExperimentUnified : MonoBehaviour
{
    [Header("Resources")]
    [Tooltip("Prefab path under Resources that contains all 2D snack children")]
    public string stimuliPath = "Images/2D/2Dsnacks";   // from your previous script

    [Header("Classification")]
    [Tooltip("Names that start with any of these go to the baseline list")]
    public string[] baselinePrefixes = { "Bamba_red", "Baflot", "Bisli" };

    [Header("Scene References (auto-filled from SceneReferencer)")]
    public Transform twoDDisplay;         // parent for all cloned children
    // public GameObject blackScreenOverlay;
    public GameObject metaUISliderGroup;
    public Slider myMetaSlider;
    public Toggle confirmToggle;
    public BgToggle bgToggle;

    [Header("Timing (auto from SceneReferencer)")]
    public float blackoutDuration = 0.5f;
    public float stimulusDisplayDuration = 1.0f;

    [Header("Misc")]
    public bool shuffleWithFixedSeed = false;
    public int seed = 12345;              // use a fixed seed to reproduce order

    // --- Internal state ---
    private readonly List<GameObject> _baselineList = new();
    private readonly List<GameObject> _stimuliList = new();
    private List<GameObject> _playback;   // merged + shuffled list used for presentation
    private int _currentIndex = 0;
    private GameObject _current;
    private ToggleFillAlphaController _fillController;

    // logging/timestamps
    private float _stimulusAppearanceTime;
    private float _ratingAppearanceTime;
    private bool _inputReceived;

    private const string ExperimentType = "2D";

    // ---------- Unity lifecycle ----------
    private void Start()
    {
        InitReferences();
        InitStimuli();    // builds/shuffles _playback
        // Start the sequence when you want:
        // StartCoroutine(ShowImageSequence());
    }

    // ---------- Setup ----------
    private void InitReferences()
    {
        // Pull everything from your SceneReferencer (same as before)
        if (twoDDisplay == null) twoDDisplay = SceneReferencer.Instance.twoDDisplay.transform;
        // blackScreenOverlay = SceneReferencer.Instance.blackScreenOverlay;
        metaUISliderGroup = SceneReferencer.Instance.metaUISliderGroup;
        myMetaSlider = SceneReferencer.Instance.myMetaSlider;
        confirmToggle = SceneReferencer.Instance.confirmToggle;
        _fillController = myMetaSlider.GetComponent<ToggleFillAlphaController>();

        blackoutDuration = SceneReferencer.Instance.blackoutDuration;
        stimulusDisplayDuration = SceneReferencer.Instance.stimulusDisplayDuration;

        metaUISliderGroup.SetActive(false);
        confirmToggle.onValueChanged.RemoveAllListeners();
        confirmToggle.isOn = false;
        confirmToggle.interactable = false;
        _fillController.SetTransparent();
    }

    private void InitStimuli()
    {
        // Load the container prefab and clone each child under twoDDisplay (inactive)
        var containerPrefab = Resources.Load<GameObject>(stimuliPath);
        if (!containerPrefab)
        {
            Debug.LogError($"Could not load prefab at Resources/{stimuliPath}");
            return;
        }

        // Create a temporary instance to read its children
        var temp = Instantiate(containerPrefab);
        temp.SetActive(false);

        _baselineList.Clear();
        _stimuliList.Clear();

        foreach (Transform child in temp.transform)
        {
            // Pre-spawn ONCE under the display parent
            var clone = Instantiate(child.gameObject, twoDDisplay);
            clone.SetActive(false);

            // Classify by name prefix
            if (StartsWithAny(clone.name, baselinePrefixes))
                _baselineList.Add(clone);
            else
                _stimuliList.Add(clone);
        }

        // We only used temp as a container – discard it
        Destroy(temp);

        // Merge (baseline first) + shuffle only stimuli
        var shuffledStimuli = new List<GameObject>(_stimuliList);
        if (shuffleWithFixedSeed) Random.InitState(seed);
        ShuffleList(shuffledStimuli);

        _playback = new List<GameObject>(_baselineList.Count + shuffledStimuli.Count);
        _playback.AddRange(_baselineList);   // baseline first
        _playback.AddRange(shuffledStimuli); // then shuffled stimuli


        // TXRDataManager.Instance.LogLineToFile($"Loaded + prepared {_playback.Count} stimuli from Resources/{stimuliPath}");
        LogHelper.Log("finished init stimuli", "blue");
    }

    // ---------- Public control ----------
    public void BeginSequence()
    {
        _currentIndex = 0;
        StartCoroutine(ShowImageSequence());
    }

    // ---------- Core sequence ----------
    public IEnumerator ShowImageSequence()
    {
        InitConfirmToggle();

        while (_currentIndex < _playback.Count)
        {
            ShowStimulus();
            _stimulusAppearanceTime = Time.time;

            // TXRDataManager.Instance.LogLineToFile("Showed Stimulus: " + _playback[_currentIndex].name);
            Debug.Log("Showed Stimulus: " + _playback[_currentIndex].name);

            yield return new WaitForSeconds(stimulusDisplayDuration);

            HideStimulus();

            // Rating UI
            metaUISliderGroup.SetActive(true);
            myMetaSlider.gameObject.SetActive(true);
            confirmToggle.interactable = true;
            _ratingAppearanceTime = Time.time;
            _inputReceived = false;

            while (!_inputReceived) yield return null;

            _currentIndex++;

            // Blackout between items
            // var canvas = blackScreenOverlay.GetComponent<CanvasGroup>();
            // canvas.alpha = 1;
            // blackScreenOverlay.SetActive(true);
            yield return new WaitForSeconds(blackoutDuration);
            // canvas.alpha = 0;
            // blackScreenOverlay.SetActive(false);
        }

        Debug.Log("Finished all stimuli.");
    }

    private void InitConfirmToggle()
    {
        confirmToggle.onValueChanged.RemoveAllListeners();
        confirmToggle.isOn = false;
        confirmToggle.interactable = false;
        confirmToggle.onValueChanged.AddListener(OnConfirmToggled);
        _fillController.SetTransparent();
    }

    // ---------- Show / Hide ----------
    private void ShowStimulus()
    {
        bgToggle.UseSkybox();

        if (_current != null) _current.SetActive(false);

        _current = _playback[_currentIndex];     // no Instantiate here
        _current.transform.SetParent(twoDDisplay, true);
        _current.SetActive(true);
    }

    private void HideStimulus()
    {
        bgToggle.UseSolid();

        if (_current != null)
        {
            _current.SetActive(false);           // keep the clone for reuse
            _current = null;
        }
    }

    // ---------- UI callback ----------
    private void OnConfirmToggled(bool isOn)
    {
        if (!isOn) return;

        float ratingTime = Time.time;
        float rating = myMetaSlider.value;
        string stimulusName = _playback[_currentIndex].name;

        // TXRDataManager.Instance.ReportExperimentData(
        //     ExperimentType,
        //     stimulusName,
        //     _stimulusAppearanceTime,
        //     _ratingAppearanceTime,
        //     ratingTime,
        //     rating);

        Debug.Log($"Rating for {_playback[_currentIndex].name}: {rating:F2}");
        // TXRDataManager.Instance.LogLineToFile($"Rating for {_playback[_currentIndex].name}: {rating:F2}");

        confirmToggle.interactable = false;
        confirmToggle.isOn = false;

        metaUISliderGroup.SetActive(false);
        myMetaSlider.gameObject.SetActive(false);
        _fillController.SetTransparent();

        _inputReceived = true;
    }

    // ---------- Helpers ----------
    private static bool StartsWithAny(string name, string[] prefixes)
    {
        if (prefixes == null) return false;
        foreach (var p in prefixes)
            if (!string.IsNullOrEmpty(p) && name.StartsWith(p))
                return true;
        return false;
    }

    private static List<T> MergeLists<T>(List<T> a, List<T> b)
    {
        // Important: don't mutate 'a' like the old MergeLists did.
        var merged = new List<T>(a.Count + b.Count);
        merged.AddRange(a);
        merged.AddRange(b);
        return merged;
    }

    private static void ShuffleList<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}