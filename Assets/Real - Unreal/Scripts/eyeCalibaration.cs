using UnityEngine;
using System.Collections;

public class EyeCalibrationCheck : MonoBehaviour
{
    public Transform[] targetPoints; // 5 points in the scene
    public float gazeCaptureDuration = 2f;

    TXREyeTracker eyeTracker;


    void Awake()
    {
        eyeTracker = FindObjectOfType<TXREyeTracker>();
    }

    public IEnumerator RunCalibration()
    {
        foreach (Transform point in targetPoints)
        {
            ShowTarget(point); // Move visual dot here

            yield return new WaitForSeconds(0.5f); // small delay
            // float startTime = Time.time;
            Vector3 eyePositionCalibration = eyeTracker.EyePosition;
            Vector3 eyeForward = eyeTracker.EyeForward;
            string pointName = point.name;
            Vector3 pointPosition = point.position;

            TXRDataManager.Instance.ReportEyeTrackingData(pointName, eyePositionCalibration.ToString("F4"), eyeForward.ToString("F4"), pointPosition.ToString("F4"));

        }
        HideAllTargets();
        Debug.Log("Eye calibration check complete");
    }

    void ShowTarget(Transform currentTarget)
    {
        foreach (Transform t in targetPoints)
        {
            if (t != null)
                t.gameObject.SetActive(t == currentTarget);
        }

    }
    void HideAllTargets()
    {
        foreach (Transform t in targetPoints)
        {
            if (t != null)
                t.gameObject.SetActive(false);
        }
    }

}
