using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 12f, -14f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // Stop following when cafe opens
        if (CafeStateManager.Instance != null &&
            CafeStateManager.Instance.cafeOpen)
            return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Gentle look-at, not steep tilt
        transform.LookAt(target.position + Vector3.up * 2f);
    }
}
