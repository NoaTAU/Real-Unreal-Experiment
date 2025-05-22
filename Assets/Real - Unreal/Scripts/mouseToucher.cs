using UnityEngine;

public class MouseToucher : MonoBehaviour
{
    private Camera sceneCamera;
    public LayerMask raycastLayers;
    public float followSpeed = 10f;
    public float rotationSpeed = 3f;
    public float zoomSpeed = 5f;
    private Vector3 lastMousePosition;

    void Start()
    {
        // Auto-find the camera
        sceneCamera = Camera.main;
        if (sceneCamera == null)
        {
            sceneCamera = FindObjectOfType<Camera>();
            Debug.LogWarning("Camera not tagged as MainCamera. Using fallback: " + sceneCamera?.name);
        }
    }

    void Update()
    {
        if (sceneCamera == null) return;

        // Left click: move to target under mouse
        if (Input.GetMouseButton(0))
        {
            Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, raycastLayers))
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, Time.deltaTime * followSpeed);

                if (Input.GetMouseButtonDown(0))
                {
                    hit.collider?.SendMessage("OnTouch", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        // Right-click drag: rotate
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float yaw = delta.x * rotationSpeed * Time.deltaTime;
            float pitch = -delta.y * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, yaw, Space.World);
            transform.Rotate(Vector3.right, pitch, Space.Self);
        }

        // Middle-click drag: translate
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 right = transform.right;
            Vector3 up = transform.up;

            transform.position -= (right * delta.x + up * delta.y) * 0.01f;
        }

        // Scroll to zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            transform.position += transform.forward * scroll * zoomSpeed;
        }

        lastMousePosition = Input.mousePosition;
    }
}
