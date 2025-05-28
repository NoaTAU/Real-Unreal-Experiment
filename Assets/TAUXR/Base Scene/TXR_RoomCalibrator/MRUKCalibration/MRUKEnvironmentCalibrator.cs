using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MRUKEnvironmentCalibrator : TXRSingleton<MRUKEnvironmentCalibrator>
{
    [SerializeField] private GameObject centerModel;
    [SerializeField] private Transform virtualReferencePointPosition;
    [SerializeField] private Transform virtualReferencePointRotation;
    [SerializeField] private UnityEvent onCalibrationComplete;
    [SerializeField] private bool showDebugSpheres = false;
    [SerializeField] private bool calibrateOnStart = false;
    
    private Transform _player;
    private Transform _positionReference;
    private Transform _rotationReference;
    private GameObject _positionDebugSphere;
    private GameObject _rotationDebugSphere;

    private void Start()
    {
        Init();
        
        if (calibrateOnStart)
        {
            CalibrateRoom().Forget();
        }
    }

    private void CreateDebugSphere(ref GameObject sphere, Vector3 position, Color color)
    {
        if (sphere != null || !showDebugSpheres) return;
        
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * 0.1f; // 10cm sphere
        
        // Set material color
        var renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            renderer.material.color = color;
        }
        
        // Remove collider
        var collider = sphere.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
    }

    private void DestroyDebugSphere(ref GameObject sphere)
    {
        if (sphere != null)
        {
            Destroy(sphere);
            sphere = null;
        }
    }

    public void SetPositionReference(Transform reference)
    {
        _positionReference = reference;
        Debug.Log($"[MRUKEnvironmentCalibrator] Position reference set to: {reference?.name ?? "null"}");
        
        if (reference != null)
        {
            CreateDebugSphere(ref _positionDebugSphere, reference.position, Color.blue);
        }
    }

    public void SetRotationReference(Transform reference)
    {
        _rotationReference = reference;
        Debug.Log($"[MRUKEnvironmentCalibrator] Rotation reference set to: {reference?.name ?? "null"}");
        
        if (reference != null)
        {
            CreateDebugSphere(ref _rotationDebugSphere, reference.position, Color.red);
        }
    }

    public void ClearPositionReference()
    {
        _positionReference = null;
        Debug.Log("[MRUKEnvironmentCalibrator] Position reference cleared");
        DestroyDebugSphere(ref _positionDebugSphere);
    }

    public void ClearRotationReference()
    {
        _rotationReference = null;
        Debug.Log("[MRUKEnvironmentCalibrator] Rotation reference cleared");
        DestroyDebugSphere(ref _rotationDebugSphere);
    }

    private void OnDestroy()
    {
        DestroyDebugSphere(ref _positionDebugSphere);
        DestroyDebugSphere(ref _rotationDebugSphere);
    }

    public async UniTask CalibrateRoom()
    {
        Init();
        
        // Wait for reference points to be set
        await WaitForReferencePoints();
        
        // Once we have the reference points, we can calibrate
        AlignVirtualToPhysicalRoom();
        
        // Disable passthrough & show model
        ExitCalibrationMode();
    }

    private void Init()
    {
        _player = TXRPlayer.Instance.transform;
    }

    private async UniTask WaitForReferencePoints()
    {
        // Wait until we have both reference points
        while (_positionReference == null || _rotationReference == null)
        {
            await UniTask.Yield();
        }
    }

    private void ExitCalibrationMode()
    {
        onCalibrationComplete?.Invoke();
    }

    public void AlignVirtualToPhysicalRoom()
    {
        // Parent the reference points to the player so they move with rotation
        _positionReference.SetParent(_player);
        _rotationReference.SetParent(_player);
        
        // ignore differences on height
        _rotationReference.position = new Vector3(_rotationReference.position.x,
            _positionReference.position.y, _rotationReference.position.z);

        Vector3 realDirection = (_rotationReference.position - _positionReference.position).normalized;
        Vector3 virtualDirection = (virtualReferencePointRotation.position - virtualReferencePointPosition.position).normalized;

        float angle = Vector3.SignedAngle(realDirection, virtualDirection, _player.up);
        _player.Rotate(_player.up, angle);

        Vector3 positionOffset = virtualReferencePointPosition.position - _positionReference.position;
        _player.position += positionOffset;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MRUKEnvironmentCalibrator))]
    public class MRUKEnvironmentCalibratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MRUKEnvironmentCalibrator calibrator = (MRUKEnvironmentCalibrator)target;
            
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Calibrate Room"))
            {
                calibrator.CalibrateRoom().Forget();
            }
        }
    }
#endif
} 