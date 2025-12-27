using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueButton;

    [Header("Settings")]
    public float typingSpeed = 0.05f;

    private string[] currentDialogueLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        dialoguePanel.SetActive(false);

        // Set up continue button click handler
        if (continueButton != null)
        {
            Button button = continueButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnContinueButtonClicked);
            }
        }
    }

    public void OnContinueButtonClicked()
    {
        Debug.Log("Continue button clicked!");
        DisplayNextLine();
    }

    void Update()
    {
        // Allow spacebar or E to advance dialogue
        if (dialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(string npcName, string[] dialogueLines)
    {
        dialoguePanel.SetActive(true);
        dialogueActive = true;
        nameText.text = npcName;
        currentDialogueLines = dialogueLines;
        currentLineIndex = 0;

        // Lock player movement
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.SetMovementEnabled(false);

        // Disable camera rotation (third person)
        ThirdPersonCamera camera = FindObjectOfType<ThirdPersonCamera>();
        if (camera != null)
            camera.SetCameraEnabled(false);

        // Disable mouse look (first person)
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerController.SetMouseLookEnabled(false);

        // Show and unlock cursor for dialogue interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start typing the first line (don't increment index yet)
        if (currentDialogueLines.Length > 0)
        {
            typingCoroutine = StartCoroutine(TypeLine(currentDialogueLines[0]));
            currentLineIndex = 1; // Next line will be index 1
        }
    }

    void DisplayNextLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (isTyping)
        {
            // Show full text immediately if still typing
            dialogueText.text = currentDialogueLines[currentLineIndex - 1];
            isTyping = false;
            continueButton.SetActive(true);
            return;
        }

        if (currentLineIndex >= currentDialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        typingCoroutine = StartCoroutine(TypeLine(currentDialogueLines[currentLineIndex]));
        currentLineIndex++;
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        continueButton.SetActive(false);

        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        continueButton.SetActive(true);
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueActive = false;
        currentDialogueLines = null;
        currentLineIndex = 0;

        // Unlock player movement
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.SetMovementEnabled(true);

        // Re-enable camera rotation (third person)
        ThirdPersonCamera camera = FindObjectOfType<ThirdPersonCamera>();
        if (camera != null)
            camera.SetCameraEnabled(true);

        // Re-enable mouse look (first person)
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerController.SetMouseLookEnabled(true);

        // Lock and hide cursor again after dialogue
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}