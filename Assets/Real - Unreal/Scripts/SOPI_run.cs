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

        public void HandleDuringQsContinue(bool isOn)
    {
        if (!isOn) return;  // only run when the toggle is turned ON

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
        }

        // OPTIONAL: auto-reset the toggle back to off so user can click again
        duringQsContinueButton.isOn = false;
    }


    // ------------ Shared Helper ------------------

        bool AllAnswered(GameObject parent)
    {
        foreach (var block in questionBlocks)
        {
            // Only check active blocks (important for paged view)
            if (!block.activeInHierarchy)
                continue;

            Toggle[] toggles = block.GetComponentsInChildren<Toggle>();
            bool anySelected = false;

            foreach (var t in toggles)
            {
                if (t.isOn)
                {
                    anySelected = true;
                    break;
                }
            }

            if (!anySelected)
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
