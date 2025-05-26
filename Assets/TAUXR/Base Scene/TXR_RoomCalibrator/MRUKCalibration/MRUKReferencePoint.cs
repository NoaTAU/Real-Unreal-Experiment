using UnityEngine;

public class MRUKReferencePoint : MonoBehaviour
{
    public enum ReferenceType
    {
        Position,
        Rotation
    }

    [SerializeField] private ReferenceType referenceType;

    private void Start()
    {
        // Register this reference point with the calibrator
        if (referenceType == ReferenceType.Position)
        {
            MRUKEnvironmentCalibrator.Instance.SetPositionReference(transform);
        }
        else
        {
            MRUKEnvironmentCalibrator.Instance.SetRotationReference(transform);
        }
    }

    private void OnDestroy()
    {
        // Unregister this reference point when destroyed
        if (referenceType == ReferenceType.Position)
        {
            MRUKEnvironmentCalibrator.Instance.ClearPositionReference();
        }
        else
        {
            MRUKEnvironmentCalibrator.Instance.ClearRotationReference();
        }
    }
} 