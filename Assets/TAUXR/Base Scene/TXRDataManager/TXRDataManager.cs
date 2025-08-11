using System;
using System.Drawing;
using UnityEngine;



#region Analytics Data Classes
public interface AnalyticsDataClass
{
    string TableName { get; }
}

[Serializable]
public class AnalyticsLogLine : AnalyticsDataClass
{
    public string TableName => "TAUXR_Logs";
    public float LogTime;
    public string LogText;

    public AnalyticsLogLine(string line)
    {
        LogTime = Time.time;
        LogText = line;
    }
}

// Declare here new AnalyticsDataClasses for every table file output you desire.

public class ConfigurationsData : AnalyticsDataClass
{
    public string TableName => "Configurations";
    public float LogTime;
    public string ConfigurationName;
    public string ConfigurationValue;

    public ConfigurationsData(string name, string value)
    {
        LogTime = Time.time;
        ConfigurationName = name;
        ConfigurationValue = value;
    }
}

public class EyeTrackerData : AnalyticsDataClass
{
    public string TableName => "EyeTrackerData";
    public float LogTime;
    public string PointName; // Optional, can be used to identify specific points if needed
    public string PointPosition;
    public string EyePosition;
    public string EyeForward; // Optional, can be used to identify the forward direction of the gaze
    public EyeTrackerData(string pointName, string eyePosition, string eyeForward, string pointPosition)
    {
        LogTime = Time.time;
        PointName = pointName;
        EyePosition = eyePosition;
        EyeForward = eyeForward;
        PointPosition = pointPosition;
    }
}
public class ExperimentData : AnalyticsDataClass
{
    public string TableName => "ExperimentData";
    public float LogTime;
    public string Round;
    public string StimulusName;
    public float StimulusAppearanceTime;
    public float RatingAppearanceTime;
    public float RatingTime;
    public float RatingValue;

    public ExperimentData(string round, string stimulusName, float stimulusAppearanceTime, float ratingAppearanceTime, float ratingTime, float ratingValue)
    {
        LogTime = Time.time;
        Round = round;
        StimulusName = stimulusName;
        StimulusAppearanceTime = stimulusAppearanceTime;
        RatingAppearanceTime = ratingAppearanceTime;
        RatingTime = ratingTime;
        RatingValue = ratingValue;
    }
}

public class QuestionnaireData : AnalyticsDataClass
{
    public string TableName => "QuestionnaireData";
    public float LogTime;
    public string RoundName; 
    public string QuestionText;
    public string AnswerText;

    public QuestionnaireData(string roundName,string questionText, string answerText)
    {
        LogTime = Time.time;
        RoundName = roundName;
        QuestionText = questionText;
        AnswerText = answerText;
    }
}
public class instructionsData : AnalyticsDataClass
{
    public string TableName => "InstructionsData";
    public float LogTime;
    public string InstructionText;
    public float AppearanceTime;
    public float ConfirmationTime;

    public instructionsData(string instructionText, float appearanceTime, float confirmationTime)
    {

        LogTime = Time.time;
        InstructionText = instructionText;
        AppearanceTime = appearanceTime;
        ConfirmationTime = confirmationTime;

    }
}
#endregion

public class TXRDataManager : TXRSingleton<TXRDataManager>
{
    // updated from TAUXRPlayer
    private bool exportEyeTracking = false;
    private bool exportFaceTracking = false;

    // automatically switched to true if not in editor.
    [SerializeField]
    private bool shouldExport = false;


    private AnalyticsWriter analyticsWriter;
    private DataContinuousWriter continuousWriter;
    private DataExporterFaceExpression faceExpressionWriter;

    #region Analytics Data Classes
    // declare pointers for all experience-specific analytics classes
    private AnalyticsLogLine logLine;
    private ConfigurationsData configurationsData;
    private instructionsData instructionsData;
    private ExperimentData experimentData;
    private EyeTrackerData eyeTrackerData;
    private QuestionnaireData questionnaireData; // if you want to report questionnaire data, uncomment this line and the one below.
    // write additional events here..

    private static string uniqueParticipantId;
    public static string UniqueParticipantId => uniqueParticipantId;


    #endregion


    // Write here all the functions you'll want to use to report relevant data.

    // log a new string line with the time logged to TAUXR_Logs file.
    public void LogLineToFile(string line)
    {
        // creates a new instance of AnalyticsLogLine data class. In it's constructor, it gets the line and automatically assign Time.time to the log time.
        logLine = new AnalyticsLogLine(line);

        // tells the analytics writer to write a new line in file.
        WriteAnalyticsToFile(logLine);
    }

    #region Experiment Specific Reporters

    public void ReportConfiguration(string name, string value)
    {
        // creates a new instance of ConfigurationsData data class. In it's constructor, it gets the name and value.
        configurationsData = new ConfigurationsData(name, value);

        // tells the analytics writer to write a new line in file.
        WriteAnalyticsToFile(configurationsData);
    }

    // report eye tracking data for a specific point.
    public void ReportEyeTrackingData(string pointName, string eyePosition, string eyeForward, string pointPosition)
    {
        // creates a new instance of EyeTrackerData data class. In it's constructor, it gets the point name, position and deviation.
        eyeTrackerData = new EyeTrackerData(pointName, eyePosition, eyeForward, pointPosition);

        // tells the analytics writer to write a new line in file.
        WriteAnalyticsToFile(eyeTrackerData);
    }
    public void ReportExperimentData(string round, string stimulusName, float stimulusAppearanceTime, float ratingAppearanceTime, float ratingTime, float ratingValue)
    {
        // creates a new instance of ExperimentData data class. In it's constructor, it gets the round, stimulus name and times.
        experimentData = new ExperimentData(round, stimulusName, stimulusAppearanceTime, ratingAppearanceTime, ratingTime, ratingValue);

        // tells the analytics writer to write a new line in file.
        WriteAnalyticsToFile(experimentData);
    }
    public void ReportInstructionsData(string instructionText, float appearanceTime, float confirmationTime)
    {
        // creates a new instance of instructionsData data class. In it's constructor, it gets the instruction text.
        instructionsData = new instructionsData(instructionText, appearanceTime, confirmationTime);

        // tells the analytics writer to write a new line in file.
        WriteAnalyticsToFile(instructionsData);
    }

    public void ReportQuestionnaireData(string roundName, string questionText, string answerText)
    {
        // creates a new instance of QuestionnaireData data class. In it's constructor, it gets the question text and answer text.
        questionnaireData = new QuestionnaireData(roundName, questionText, answerText);

        // tells the analytics writer to write a new line in file.
        WriteAnalyticsToFile(questionnaireData);
    }

    #endregion

    private void WriteAnalyticsToFile(AnalyticsDataClass analyticsDataClass)
    {
        if (!shouldExport) return;

        analyticsWriter.WriteAnalyticsDataFile(analyticsDataClass);
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        // set a run specific Id
        uniqueParticipantId = "#" + KeyGenerator.GetUniqueKey(4);

        shouldExport = ShouldExportData();
        if (!shouldExport) return;

        exportEyeTracking = TXRPlayer.Instance.IsEyeTrackingEnabled;
        exportFaceTracking = TXRPlayer.Instance.IsFaceTrackingEnabled;

        analyticsWriter = new AnalyticsWriter();

        // for now, instead of making the whole interface in the datamanager, it will split between the different scripts.
        continuousWriter = GetComponent<DataContinuousWriter>();
        continuousWriter.Init(exportEyeTracking);

        if (exportFaceTracking)
        {
            faceExpressionWriter = GetComponent<DataExporterFaceExpression>();
            faceExpressionWriter.Init();
        }
    }

    // default data export on false in editor. always export on build.
    private bool ShouldExportData()
    {
        if (Application.isEditor && !shouldExport)
        {
            Debug.Log("Data Manager won't export data because it is running in editor. To export, manually enable ShouldExport");
        }
        return shouldExport || !Application.isEditor;
    }

    void FixedUpdate()
    {
        if (!shouldExport) return;

        continuousWriter.RecordContinuousData();

        if (exportFaceTracking)
        {
            faceExpressionWriter.CollectWriteDataToFile();
        }
    }
    public void FlushAnalyticsData()
    {
        analyticsWriter.FlushAndCloseWriters();
    }

    private void OnApplicationQuit()
    {
        if (!shouldExport) return;

        analyticsWriter.Close();
        continuousWriter.Close();
        faceExpressionWriter.Close();
    }

}

