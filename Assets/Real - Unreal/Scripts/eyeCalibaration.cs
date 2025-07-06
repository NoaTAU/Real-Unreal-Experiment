// using UnityEngine;
// using UnityEngine.XR;
// using System.Collections;
// using System.Collections.Generic;

// public class EyeCalibrationCheck : MonoBehaviour
// {
//     public Transform[] targetPoints; // 5 points in the scene
//     public float gazeCaptureDuration = 2f;

//     private List<float> deviations = new List<float>();

//     // void Start()
//     // {
//     //     StartCoroutine(RunCalibrationCheck());
//     // }

//     public IEnumerator RunCalibration()
//     {
//         foreach (Transform point in targetPoints)
//         {
//             ShowTarget(point); // Move visual dot here

//             yield return new WaitForSeconds(0.5f); // small delay
//             float startTime = Time.time;

//             // calculate the direction vector from the middle of right and left eye to target point (getting them through TXRPlayer.Instance.RightEye and TXRPlayer.Instance.LeftEye)
//             // get the direction vector from the center eye to the avrage direction of the right and left eye (this is the gaze direction, through the TXRPlayer.Instance.leftEye and TXRPlayer.Instance.rightEye)
//             // calculate the deviation in angles

//             // TXRPlayer.Instance.EyeTracker.EyePosition - center eye (middle of right and left eye)
//             List<Vector3> gazePoints = new List<Vector3>();
//             // get the direction 
//             while (Time.time - startTime < gazeCaptureDuration)
//             {
//                 if (TryGetEyeGaze(out Ray gazeRay))
//                 {
//                     Vector3 hitPoint = gazeRay.origin + gazeRay.direction * 5f;
//                     gazePoints.Add(hitPoint);
//                 }
//                 yield return null;
//             }

//             if (gazePoints.Count > 0)
//             {
//                 Vector3 avgGaze = Average(gazePoints);
//                 float deviation = Vector3.Distance(avgGaze, point.position);
//                 deviations.Add(deviation);
//                 Vector3 pos = point.position;
//                 string pointName = point.name;
//                 TXRDataManager.Instance.ReportEyeTrackingData(pointName, pos.ToString(), deviation);

//                 Debug.Log($"Target {point.name} at {pos} → Deviation: {deviation:F3} meters");
//             }
//         }
//         HideAllTargets();
//         Debug.Log("Eye calibration check complete. Mean deviation: " + Mean(deviations));
//     }

//     void ShowTarget(Transform currentTarget)
//     {
//         foreach (Transform t in targetPoints)
//         {
//             if (t != null)
//                 t.gameObject.SetActive(t == currentTarget);
//         }

//     }

//     bool TryGetEyeGaze(out Ray gazeRay)
//     {
//         InputDevice eyeDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
//         if (eyeDevice.TryGetFeatureValue(CommonUsages.eyesData, out Eyes eyes) && eyes.TryGetFixationPoint(out Vector3 point))
//         {
//             gazeRay = new Ray(Camera.main.transform.position, (point - Camera.main.transform.position).normalized);
//             return true;
//         }

//         // Fallback for other SDKs like Meta Interaction SDK can go here
//         gazeRay = new Ray();
//         return false;
//     }

//     Vector3 Average(List<Vector3> vectors)
//     {
//         Vector3 sum = Vector3.zero;
//         foreach (var v in vectors)
//             sum += v;
//         return sum / vectors.Count;
//     }

//     float Mean(List<float> values)
//     {
//         if (values.Count == 0) return 0;
//         float sum = 0f;
//         foreach (float v in values) sum += v;
//         return sum / values.Count;
//     }

//     void HideAllTargets()
//     {
//         foreach (Transform t in targetPoints)
//         {
//             if (t != null)
//                 t.gameObject.SetActive(false);
//         }
//     }

// }
