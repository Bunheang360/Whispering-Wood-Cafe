using UnityEngine;

public class DummyPlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Get camera forward/right directions
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Flatten vectors on XZ plane
        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        // Camera-relative movement
        Vector3 move = camForward * v + camRight * h;

        // Move player
        transform.position += move * speed * Time.deltaTime;

        // Optional: rotate player to face movement direction
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }
    }
}
