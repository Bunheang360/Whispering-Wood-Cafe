using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpSpeed = 5f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    [Header("Animation")]
    public Animator animator;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask doorLayer;

    [Header("Scene Settings")]
    public string sceneToLoad = "TavernInterior";
    public GameObject houseObject;

    private CharacterController characterController;
    private float verticalVelocity = 0f;
    private bool canInteract = false;
    private GameObject currentDoor = null;

    void Start()
    {
        // Get or add CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.radius = 0.5f;
            characterController.height = 2f;
            characterController.center = new Vector3(0, 1, 0);
        }

        // Find camera if not assigned
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Get animator if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        HandleMovement();
        CheckForDoor();
        HandleInteraction();
    }

    void HandleMovement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction relative to camera
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten camera directions (no vertical component)
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate move direction
        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        // Move character
        if (moveDirection.magnitude > 0.1f)
        {
            // Rotate character to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Update animator - character is moving
            if (animator != null)
            {
                animator.SetFloat("MoveSpeed", 1f);
            }

            // Move
            Vector3 move = moveDirection * moveSpeed;

            // Apply gravity and jump
            if (characterController.isGrounded)
            {
                verticalVelocity = -0.5f;

                // Jump
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    verticalVelocity = jumpSpeed;
                }
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            move.y = verticalVelocity;
            characterController.Move(move * Time.deltaTime);
        }
        else
        {
            // Update animator - character is idle
            if (animator != null)
            {
                animator.SetFloat("MoveSpeed", 0f);
            }

            // Apply gravity when standing still
            if (characterController.isGrounded)
            {
                verticalVelocity = -0.5f;

                // Jump even when standing still
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    verticalVelocity = jumpSpeed;
                }
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 move = new Vector3(0, verticalVelocity, 0);
            characterController.Move(move * Time.deltaTime);
        }

        // Update grounded state in animator
        if (animator != null)
        {
            animator.SetBool("Grounded", characterController.isGrounded);
        }
    }

    void CheckForDoor()
    {
        // Raycast forward from character position
        Vector3 rayOrigin = transform.position + Vector3.up * 1f; // Chest height
        Vector3 rayDirection = transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance))
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
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Gizmos.color = canInteract ? Color.green : Color.red;
        Gizmos.DrawRay(rayOrigin, transform.forward * interactionDistance);
    }
}
