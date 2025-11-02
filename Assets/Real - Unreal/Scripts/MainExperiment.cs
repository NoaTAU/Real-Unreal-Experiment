using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MainExperiment : MonoBehaviour
{
    public Toggle ExperimentsToggle;
    public Toggle FirstExperimentToggle;
    public Toggle EndQsInstructionsToggle;
    public GameObject showInstructionsButton; // Button to show instructions
    public GameObject showExperimentButton;
    public GameObject showQsButton;
    public GameObject invisibleCollider;
    public GameObject metaUISliderGroup;
    public TMP_FontAsset defaultFont;
    public EyeCalibrationCheck eyeCalibration;
    public QuestionFlowController questionFlowController;
    public GameObject ContentRoot_1;
    // private ImageRatingExperiment imagerRatingExperiment;
    private ImageRatingExperimentUnified imageRatingExperimentUnified;
    private bool readingInstructions = false;
    private bool readingQsInstructions = false;
    private bool experimentToggle = false;
    private string startMessage = "לחצו כאן כדי להתחיל";
    private string experimentEndMessage = "אנא עדכנו את הנסיין/ית ששלב זה הסתיים\n תודה רבה על שיתוף הפעולה";
    private string questionnaireStart = "אנחנו מעוניינים לדעת מה את/ה מרגיש/ה לגבי החוויה שעברת זה עתה ב'סביבה המוצגת'.\n";
    // "המונח 'סביבה מוצגת' מתייחס כאן, ולאורך השאלון הזה, לעולם הוירטואלי שהתנסת בו עכשיו.\n" +
    // "חלק מהשאלות מתייחסות ל 'תוכן' של הסביבה המוצגת. בכך אנו מתכוונים לסיפור, לסצנות או אירועים, או כל מה שאתה יכול לראות, לשמוע או לחוש שמתרחש בתוך הסביבה המוצגת.\n" +
    // "הסביבה המוצגת והתוכן שלה שונים מ 'העולם האמיתי': העולם שבו את/ה חי/ה מיום ליום.\n" +
    // "ישנם שני חלקים לשאלון זה: חלק א' וחלק ב'. חלק א' שואל על המחשבות והרגשות שלך ברגע שהסביבה המוצגת הסתיימה.\n" +
    // "חלק ב' מתייחס למחשבות ולרגשות שלך בזמן שחווית את הסביבה המוצגת.\n" +
    // "נא לא לבזבז יותר מדי זמן על אף שאלה.\n" +
    // "התגובה הראשונה שלך היא בדרך כלל הכי טובה. עבור כל שאלה, בחר/י את התשובה הקרובה ביותר לשלך.\n" +
    // "אנא זכור/י שאין תשובות נכונות או לא נכונות - אנחנו פשוט מעוניינים לדעת מה המחשבות והרגשות שלך לגבי הסביבה המוצגת.";

    private List<int> experimentList = new List<int> { 0, 1, 2 };
    private string textShuffledList = "";
    private TMP_Text generalInstructionsLabel;
    private TXRDataManager dataManager;
    public BgToggle bgToggle;
    private RectTransform bodyTextRect;
    private Vector2 restRectSize;
    private Vector2 restRectPos;
    private VerticalLayoutGroup vlg;

    private void Start()
    {
        Debug.Log("Debug: MainExperiment Start called");
        InitExperiments();

        ExperimentsToggle.interactable = false;
        ExperimentsToggle.isOn = false;
        ExperimentsToggle.onValueChanged.AddListener(OnExpToggled);

        FirstExperimentToggle.interactable = false;
        FirstExperimentToggle.isOn = false;
        FirstExperimentToggle.onValueChanged.AddListener(EndInstructionsToggled);
        EndQsInstructionsToggle.interactable = false;
        EndQsInstructionsToggle.isOn = false;
        EndQsInstructionsToggle.onValueChanged.AddListener(QsInstructionsToggleEnded);
        Debug.Log("Debug: MainExperiment Start init completed");
        StartCoroutine(RunAllExperiments());
        ApplyFontToTMP(showExperimentButton);
        ApplyFontToTMP(showInstructionsButton);
        ApplyFontToTMP(metaUISliderGroup);
        // ReportExperimentConfigurations();
        changeButtonSize();
        restRectSize = bodyTextRect.sizeDelta;
        restRectPos =  showExperimentButton.GetComponent<RectTransform>().anchoredPosition;
        vlg = showExperimentButton.transform.Find("Dialog1Button_TextOnly").GetComponent<VerticalLayoutGroup>();       
    }

    private void changeButtonSize()
    {
        bodyTextRect = showExperimentButton
        .transform
        .Find("Dialog1Button_TextOnly/BodyText")
        .GetComponent<RectTransform>();
    }
    private void ApplyFontToTMP(GameObject parent)
    {
        if (parent == null || defaultFont == null) return;

        TMP_Text[] texts = parent.GetComponentsInChildren<TMP_Text>(true);
        foreach (var text in texts)
        {
            text.font = defaultFont;
        }
    }

    private IEnumerator RunAllExperiments()
    {
        Debug.Log("Debug:Starting RunAllExperiments...");
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the experiments

        // yield return ShowDialogAndWaitForConfirm(textShuffledList);

        // Run eye calibration check
        // yield return ShowDialogAndWaitForConfirm("כעת נעשה קליברציה לתנועות העיניים.\n עקבו במבטכם אחרי הנקודה עד שהיא נעלמת");
        // yield return eyeCalibration.RunCalibration();
        // yield return ShowDialogAndWaitForConfirm("הקליברציה הסתיימה, אנא עדכנו את הנסיין/ית.");

        yield return ShowMainInstructionsAndWaitForConfirm();
        ShowDialogAndWaitForConfirm(startMessage);
        
        bgToggle.UseSkybox(); // Use the skybox for 2D images
        TXRDataManager.Instance.LogLineToFile("Starting image rating experiment...");
        Debug.Log("Starting image rating experiment...");
        yield return imageRatingExperimentUnified.ShowImageSequence();
        bgToggle.UseSolid(); // Switch back to solid color
        ContentRoot_1.SetActive(false);
        yield return ShowQsInstructionsAndWaitForConfirm();
        Debug.Log("Running questionnaire for 2D experiment...");
        yield return questionFlowController.RunQuestionnaire("2D");
        ContentRoot_1.SetActive(true);

        
         
        yield return ShowDialogAndWaitForConfirm(experimentEndMessage);
        Debug.Log("All experiments finished.");
        TXRDataManager.Instance.LogLineToFile("All experiments finished.");

        // Flush data
        TXRDataManager.Instance.FlushAnalyticsData();

        // Wait a moment to ensure all writes are done
        yield return new WaitForSeconds(1f);

        // Exit app
        Application.Quit();
            }

    private void InitExperiments()
    {
        imageRatingExperimentUnified = GetComponent<ImageRatingExperimentUnified>();
        dataManager = TXRDataManager.Instance;
        generalInstructionsLabel = showExperimentButton.transform.Find("Dialog1Button_TextOnly/BodyText").GetComponentInChildren<TMP_Text>();
    }

    private IEnumerator ShowDialogAndWaitForConfirm(string InstructionsText)
    {
        if (InstructionsText == experimentEndMessage)
        {
            bodyTextRect.sizeDelta = new Vector2(320, 200);
        }
        else
        {
            bodyTextRect.sizeDelta = restRectSize; // Reset to original size
            showExperimentButton.GetComponent<RectTransform>().anchoredPosition = restRectPos; // Reset to original position
        }
        vlg.enabled = true; 
        TMP_Text label = showExperimentButton.transform.Find("Dialog1Button_TextOnly/BodyText").GetComponentInChildren<TMP_Text>();
        label.text = InstructionsText; // strip text from /n characters and such
        string InstructionsTextTrimmed = InstructionsText.Replace("\n", ""); //add chars to trim
        ExperimentsToggle.interactable = true;
        showExperimentButton.SetActive(true);

        float appearanceTime = Time.time;

        while (!experimentToggle)
        {
            yield return null;
        }

        float confirmationTime = Time.time;

        experimentToggle = false;
        showExperimentButton.SetActive(false);
        ExperimentsToggle.interactable = false;
        ExperimentsToggle.isOn = false;
        bodyTextRect.sizeDelta = restRectSize;
        vlg.enabled = false;
        // Reset the layout group to its original state        
        dataManager.ReportInstructionsData(InstructionsTextTrimmed, appearanceTime, confirmationTime);
    }

    private IEnumerator ShowMainInstructionsAndWaitForConfirm()
    {
        // main experiment instructions:
        FirstExperimentToggle.interactable = true;
        showInstructionsButton.SetActive(true); // Show the button to start reading instructions

        while (!readingInstructions)
        {
            yield return null;
        }

        showInstructionsButton.SetActive(false);
        FirstExperimentToggle.interactable = false;
    }
    private IEnumerator ShowQsInstructionsAndWaitForConfirm()
    {
        EndQsInstructionsToggle.interactable = true;
        showQsButton.SetActive(true); // Show the button to start reading instructions

        while (!readingQsInstructions)
        {
            yield return null;
        }

        showQsButton.SetActive(false);
        EndQsInstructionsToggle.interactable = false;
        readingQsInstructions = false;

    }

    private void EndInstructionsToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON
        readingInstructions = true;
        Debug.Log("readingInstructions = true");
        FirstExperimentToggle.isOn = false;

    }

    private void QsInstructionsToggleEnded(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON
        readingQsInstructions = true;
        Debug.Log("readingQsInstructions = true");
        EndQsInstructionsToggle.isOn = false;

    }
    private void OnExpToggled(bool isOn)
    {
        if (!isOn) return; // only respond when toggled ON
        experimentToggle = true;
        Debug.Log("toggle = true");
    }

    private void ReportExperimentConfigurations()
    {
        dataManager.ReportConfiguration("BlackoutDuration", SceneReferencer.Instance.blackoutDuration.ToString());
        dataManager.ReportConfiguration("StimulusDisplayDuration", SceneReferencer.Instance.stimulusDisplayDuration.ToString());
        dataManager.ReportConfiguration("ExperimentOrderString", textShuffledList);
        dataManager.ReportConfiguration("ExperimentOrderNumbers", string.Join(",", experimentList));
    }

}
