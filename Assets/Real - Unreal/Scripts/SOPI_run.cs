using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionFlowController : MonoBehaviour
{
    [Header("AfterQs")]
    public GameObject afterQsRoot;
    public Toggle afterQsContinueButton;

    [Header("DuringQs")]
    public GameObject duringQsRoot;
    public List<GameObject> questionBlocks; // Fill with DuringQs/QandA GameObjects
    public Toggle duringQsContinueButton;
    public TextAsset questionsJSON; // questions.json in Resources or Inspector

    private List<string> allDuringQuestions;
    private int currentIndex = 0;
    private int batchSize;

    void Start()
    {
        duringQsRoot.SetActive(false);
        afterQsRoot.SetActive(true);
        batchSize = questionBlocks.Count;
        afterQsContinueButton.interactable = true;
        afterQsContinueButton.isOn = false;

        duringQsContinueButton.interactable = false;
        duringQsContinueButton.isOn = false;

        afterQsContinueButton.onValueChanged.AddListener(HandleAfterQsContinue);
        duringQsContinueButton.onValueChanged.AddListener(HandleDuringQsContinue);

        LoadDuringQuestions();
    }

    // ------------ PHASE 1: After Qs ------------------

    void HandleAfterQsContinue(bool isOn)
    {
        if (!AllAnswered(afterQsRoot))
        {
            Debug.Log("Please complete all AfterQs.");
            return;
        }

        afterQsRoot.SetActive(false);
        duringQsRoot.SetActive(true);
        currentIndex = 0;
        ShowDuringQsBatch();
    }

    // ------------ PHASE 2: During Qs ------------------

    void LoadDuringQuestions()
    {
        allDuringQuestions = new List<string>();
        var parsed = JsonUtility.FromJson<QuestionList>(questionsJSON.text);
        allDuringQuestions = parsed.questions;
    }

    void ShowDuringQsBatch()
    {
        duringQsContinueButton.interactable = true;
        afterQsContinueButton.interactable = false;
        for (int i = 0; i < batchSize; i++)
        {
            int qIndex = currentIndex + i;

            if (qIndex < allDuringQuestions.Count)
            {
                questionBlocks[i].SetActive(true);
                var txt = questionBlocks[i].GetComponentInChildren<TMP_Text>();
                txt.text = allDuringQuestions[qIndex];

                Toggle[] toggles = questionBlocks[i].GetComponentsInChildren<Toggle>();
                foreach (var t in toggles)
                    t.isOn = false;
            }
            else
            {
                questionBlocks[i].SetActive(false);
            }
        }
    }

    void HandleDuringQsContinue(bool isOn)
    {
        if (!AllAnswered(duringQsRoot))
        {
            Debug.Log("Please answer all visible DuringQs.");
            return;
        }

        currentIndex += batchSize;
        if (currentIndex < allDuringQuestions.Count)
        {
            ShowDuringQsBatch();
        }
        else
        {
            Debug.Log("All questions completed!");
            // Add any final transition here (scene change, save, etc.)
        }
    }

    // ------------ Shared Helper ------------------

    bool AllAnswered(GameObject parent)
    {
        Toggle[] allToggles = parent.GetComponentsInChildren<Toggle>(true);

        // Group toggles by parent question block
        Dictionary<Transform, bool> answered = new Dictionary<Transform, bool>();

        foreach (Toggle t in allToggles)
        {
            if (!answered.ContainsKey(t.transform.parent))
                answered[t.transform.parent] = false;

            if (t.isOn)
                answered[t.transform.parent] = true;
        }

        foreach (var kvp in answered)
        {
            if (!kvp.Value && kvp.Key.gameObject.activeInHierarchy)
                return false;
        }

        return true;
    }

    [System.Serializable]
    public class QuestionList
    {
        public List<string> questions;
    }
}
