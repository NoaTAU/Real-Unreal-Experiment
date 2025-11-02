using System.Collections.Generic;
using System.Collections;   
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Animations;

public class QuestionFlowController : MonoBehaviour
{
    public GameObject ContentRoot;
    public GameObject ContentRoot_1;

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
    private bool questionnaireComplete = false;

    private string currentRoundName = "";

    void Start()
    {
        batchSize = questionBlocks.Count;
        LoadDuringQuestions();
    }

    public virtual IEnumerator RunQuestionnaire(string roundName)
    {
        ContentRoot.SetActive(true);
        // ContentRoot_1.SetActive(false);
        Debug.Log("Running questionnaire for round: " + roundName);
        currentRoundName = roundName;
        afterQsRoot.SetActive(true);
        afterQsContinueButtonGameObject.SetActive(true);
        afterQsContinueButton.interactable = true;
        afterQsContinueButton.isOn = false;
       
        afterQsContinueButton.onValueChanged.AddListener(HandleAfterQsContinue);
        duringQsContinueButton.onValueChanged.AddListener(HandleDuringQsContinue);

        // Wait until the questionnaire is marked complete
        //yield return new WaitUntil(() => questionnaireComplete);
        while (!questionnaireComplete)
            {

                yield return null;

            }
        duringQsContinueButtonGameObject.SetActive(false);
        duringQsRoot.SetActive(false);
        questionnaireComplete = false; // Reset for next use
        afterQsContinueButton.onValueChanged.RemoveListener(HandleAfterQsContinue);
        duringQsContinueButton.onValueChanged.RemoveListener(HandleDuringQsContinue);
        ContentRoot.SetActive(false);
        yield return new WaitForSeconds(1f); // Optional delay before proceeding
        Debug.Log("Questionnaire completed for round: " + currentRoundName);
       

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
        afterQsContinueButton.isOn = false;
        
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
            questionnaireComplete = true;
        }

        // OPTIONAL: auto-reset the toggle back to off so user can click again
        duringQsContinueButton.isOn = false;
        // ContentRoot_1.SetActive(true);
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
            {
                return false;
            }
            else
            {
                foreach (var t in toggles)
            {
                t.isOn = false;
            }
            }
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
