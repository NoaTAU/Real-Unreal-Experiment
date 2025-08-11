using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Animations;

public class QuestionFlowController : MonoBehaviour
{
    [Header("AfterQs")]
    public GameObject afterQsRoot;
    public Toggle afterQsContinueButton;
     public List<GameObject> questionBlock;
    public GameObject afterQsContinueButtonGameObject; // Optional, if you want to control visibility

    [Header("DuringQs")]
    public GameObject duringQsRoot;
    public List<GameObject> questionBlocks; // Fill with DuringQs/QandA GameObjects
    public Toggle duringQsContinueButton;
    public GameObject duringQsContinueButtonGameObject; // Optional, if you want to control visibility
    public TextAsset questionsJSON; // questions.json in Resources or Inspector

    private List<string> allDuringQuestions;
    private int currentIndex = 0;
    private int batchSize;
    private List<GameObject> block; // used to store the current question blocks, either from afterQsRoot or duringQsRoot

    private string currentRoundName = "";

    void Start()
    {
        batchSize = questionBlocks.Count;
        LoadDuringQuestions();
    }

    public void RunQuestionnaire(string roundName)
    {
        currentRoundName = roundName;
        duringQsRoot.SetActive(false);
        afterQsRoot.SetActive(true);
        afterQsContinueButton.interactable = true;
        afterQsContinueButton.isOn = false;
        duringQsContinueButtonGameObject.SetActive(false);
        afterQsContinueButton.onValueChanged.AddListener(HandleAfterQsContinue);
        duringQsContinueButton.onValueChanged.AddListener(HandleDuringQsContinue);

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
        afterQsContinueButtonGameObject.SetActive(false);
        duringQsContinueButtonGameObject.SetActive(true);
        duringQsContinueButton.interactable = true;
        duringQsContinueButton.isOn = false;

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
        if (parent == afterQsRoot)
        {
            block = questionBlock;
        } else if (parent == duringQsRoot)
        {
            block = questionBlocks;
        }
        
        string[] questionText = new string[block.Count];
        string[] answerText = new string[block.Count];

        foreach (var oneBlock in block)
        {
            // Only check active blocks (important for paged view)
            if (!oneBlock.activeInHierarchy)
                continue;

            Toggle[] toggles = oneBlock.GetComponentsInChildren<Toggle>();
            bool anySelected = false;

            questionText[Array.IndexOf(block.ToArray(), oneBlock)] = oneBlock.GetComponentInChildren<TMP_Text>().text;

            foreach (var t in toggles)
            {
                if (t.isOn)
                {
                    anySelected = true;
                    answerText[Array.IndexOf(block.ToArray(), oneBlock)] = t.GetComponentInChildren<TMP_Text>().text;
                    break;
                }
            }


            if (!anySelected)
                return false;
        }

        // Report the data to the TXRDataManager
        for (int i = 0; i < questionText.Length; i++)
        {
            if (questionText[i]==null)
            {
                questionText[i] = "None";
            }
            if (answerText[i]==null)
            {
                answerText[i] = "None";
            }
            TXRDataManager.Instance.ReportQuestionnaireData(currentRoundName, questionText[i], answerText[i]);
            
        }
        return true;
    }


    [System.Serializable]
    public class QuestionList
    {
        public List<string> questions;
    }
    

}
