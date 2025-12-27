using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float distance = 5f;
    public float height = 2f;
    public float mouseSensitivity = 2f;
    public float smoothSpeed = 10f;

    [Header("Camera Limits")]
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 60f;

    [Header("Collision")]
    public bool avoidObstacles = true;
    public float collisionOffset = 0.3f;
    public LayerMask collisionLayers;

    private float currentX = 0f;
    private float currentY = 20f;
    private bool cameraEnabled = true;

    void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Find target if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        // Initialize camera angles
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Skip camera rotation if disabled (e.g., during dialogue)
        if (!cameraEnabled) return;

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update rotation
        currentX += mouseX;
        currentY -= mouseY;
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // Calculate desired position
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPosition = target.position + new Vector3(0, height, 0) + rotation * direction;

        // Check for obstacles
        if (avoidObstacles)
        {
            Vector3 targetPosition = target.position + new Vector3(0, height, 0);
            RaycastHit hit;
            if (Physics.Linecast(targetPosition, desiredPosition, out hit, collisionLayers))
            {
                desiredPosition = hit.point + (targetPosition - desiredPosition).normalized * collisionOffset;
            }
        }

        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + new Vector3(0, height, 0));
    }

    public void SetCameraEnabled(bool enabled)
    {
        cameraEnabled = enabled;
    }

    void OnValidate()
    {
        // Ensure min is less than max
        if (minVerticalAngle > maxVerticalAngle)
        {
            minVerticalAngle = maxVerticalAngle;
        }
    }
}
