using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

public class EyeCalibrationCheck : MonoBehaviour
{
    public Transform[] targetPoints; // 5 points in the scene
    public float gazeCaptureDuration = 2f;

    private List<float> deviations = new List<float>();

    // void Start()
    // {
    //     StartCoroutine(RunCalibrationCheck());
    // }

    public IEnumerator RunCalibration()
    {
        foreach (Transform point in targetPoints)
        {
            ShowTarget(point); // Move visual dot here

            yield return new WaitForSeconds(0.5f); // small delay
            float startTime = Time.time;
            List<Vector3> gazePoints = new List<Vector3>();

            while (Time.time - startTime < gazeCaptureDuration)
            {
                if (TryGetEyeGaze(out Ray gazeRay))
                {
                    Vector3 hitPoint = gazeRay.origin + gazeRay.direction * 5f;
                    gazePoints.Add(hitPoint);
                }
                yield return null;
            }

            if (gazePoints.Count > 0)
            {
                Vector3 avgGaze = Average(gazePoints);
                float deviation = Vector3.Distance(avgGaze, point.position);
                deviations.Add(deviation);
                Debug.Log($"Deviation for {point.name}: {deviation:F3} meters");
            }
        }

        Debug.Log("Eye calibration check complete. Mean deviation: " + Mean(deviations));
    }

    void ShowTarget(Transform target)
    {
        // Enable a small dot at target.position
        // Optional: play sound or highlight to direct attention
    }

    bool TryGetEyeGaze(out Ray gazeRay)
    {
        InputDevice eyeDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (eyeDevice.TryGetFeatureValue(CommonUsages.eyesData, out Eyes eyes) && eyes.TryGetFixationPoint(out Vector3 point))
        {
            gazeRay = new Ray(Camera.main.transform.position, (point - Camera.main.transform.position).normalized);
            return true;
        }

        // Fallback for other SDKs like Meta Interaction SDK can go here
        gazeRay = new Ray();
        return false;
    }

    Vector3 Average(List<Vector3> vectors)
    {
        Vector3 sum = Vector3.zero;
        foreach (var v in vectors)
            sum += v;
        return sum / vectors.Count;
    }

    float Mean(List<float> values)
    {
        if (values.Count == 0) return 0;
        float sum = 0f;
        foreach (float v in values) sum += v;
        return sum / values.Count;
    }
}
