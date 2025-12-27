using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Scene Settings")]
    public string sceneToLoad = "TavernInterior";
    public GameObject houseObject;

    private CharacterController characterController;
    private float verticalVelocity = 0f;
    private float cameraPitch = 0f;
    private bool canInteract = false;
    private GameObject currentDoor = null;
    private bool mouseLookEnabled = true;

    void Start()
    {
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get or add CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.radius = 0.5f;
            characterController.height = 2f;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        CheckForDoor();
        HandleInteraction();
    }

    void HandleMovement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection *= moveSpeed;

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;

        // Move character
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // Skip mouse look if disabled (e.g., during dialogue)
        if (!mouseLookEnabled) return;

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate camera vertically
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
    }

    public void SetMouseLookEnabled(bool enabled)
    {
        mouseLookEnabled = enabled;
    }

    void CheckForDoor()
    {
        // Raycast to check for door
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionDistance))
        {
            // Check if hit object has "Door" tag
            if (hit.collider.CompareTag("Door"))
            {
                canInteract = true;
                currentDoor = hit.collider.gameObject;

                // Show UI prompt
                if (InteractionUI.Instance != null)
                {
                    InteractionUI.Instance.ShowPrompt();
                }
            }
            else
            {
                canInteract = false;
                currentDoor = null;

                // Hide UI prompt
                if (InteractionUI.Instance != null)
                {
                    InteractionUI.Instance.HidePrompt();
                }
            }
        }
        else
        {
            canInteract = false;
            currentDoor = null;

            // Hide UI prompt
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.HidePrompt();
            }
        }
    }

    void HandleInteraction()
    {
        // Check if player pressed interact key
        if (canInteract && Input.GetKeyDown(interactKey))
        {
            LoadScene();
        }
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // Use fade transition if available
            if (SceneTransition.Instance != null)
            {
                SceneTransition.Instance.LoadSceneWithFade(sceneToLoad);
            }
            else
            {
                // Fallback to instant scene load
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            Debug.LogWarning("No scene specified to load!");
        }
    }

    void OnDrawGizmos()
    {
        // Visualize interaction range in editor
        if (Camera.main != null)
        {
            Gizmos.color = canInteract ? Color.green : Color.red;
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * interactionDistance);
        }
    }
}
