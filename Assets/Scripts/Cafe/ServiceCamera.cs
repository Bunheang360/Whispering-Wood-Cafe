using UnityEngine;

public class ServiceCamera : MonoBehaviour
{
    public Transform serviceView;
    public Transform originalView; // Assign Main Camera's original position
    public float speed = 5f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        // Store the original camera position and rotation
        if (originalView != null)
        {
            originalPosition = originalView.position;
            originalRotation = originalView.rotation;
        }
        else
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }
    }

    void Update()
    {
        if (CafeStateManager.Instance == null) return;

        if (CafeStateManager.Instance.cafeOpen)
        {
            // Move to service view
            transform.position = Vector3.Lerp(
                transform.position,
                serviceView.position,
                Time.deltaTime * speed
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                serviceView.rotation,
                Time.deltaTime * speed
            );
        }
        else
        {
            // Move back to original view
            transform.position = Vector3.Lerp(
                transform.position,
                originalPosition,
                Time.deltaTime * speed
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                originalRotation,
                Time.deltaTime * speed
            );
        }
    }
}
