using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CafeStateManager : MonoBehaviour
{
    public static CafeStateManager Instance;

    public bool cafeOpen = false;
    public Button openCafeButton; // assign in Inspector
    public GameObject player; // assign player GameObject in Inspector

    [Header("Win Condition Settings")]
    public int customersToServeToWin = 2; // How many customers need to be served to win
    public float timeLimit = 300f; // Time limit in seconds (5 minutes default)

    [Header("Win Condition UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI customerCountText;
    public GameObject winPanel;
    public GameObject losePanel;

    private int customersServed = 0;
    private float timeRemaining;
    private bool gameEnded = false;
    private bool sessionActive = false; // Track if a cafe session is running

    void Awake()
    {
        // Make this object persistent across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Show cursor when scene loads (with delay to override PlayerController)
        Invoke("ShowCursor", 0.1f);
    }

    void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Press ESC to close cafe (and prevent other ESC handlers from triggering)
        if (cafeOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCafe();
            return; // Stop processing to prevent EscapeMenu from also reacting
        }

        // Timer countdown when session is active (runs even if cafe is closed!)
        if (sessionActive && !gameEnded)
        {
            timeRemaining -= Time.deltaTime;

            // Update UI if cafe is open
            if (cafeOpen)
            {
                UpdateTimerUI();
            }

            // Check lose condition (time ran out)
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                LoseGame("Time ran out!");
            }

            // Check win condition (served enough customers)
            if (customersServed >= customersToServeToWin)
            {
                WinGame();
            }
        }
    }

    public void OpenCafe()
    {
        cafeOpen = true;
        Debug.Log("Cafe is now OPEN");

        // Start a new session only if one isn't already active
        if (!sessionActive)
        {
            sessionActive = true;
            customersServed = 0;
            timeRemaining = timeLimit;
            gameEnded = false;
            Debug.Log("Started new cafe session with persistent timer!");
        }
        else
        {
            Debug.Log($"Resuming cafe session - Time remaining: {timeRemaining:F1}s");
        }

        // Hide win/lose panels
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        UpdateCustomerCountUI();
        UpdateTimerUI();

        // Show cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Hide the button
        if (openCafeButton != null)
            openCafeButton.gameObject.SetActive(false);

        // Hide the player
        if (player != null)
        {
            player.SetActive(false);
            Debug.Log("Player hidden");
        }
    }

    public void CloseCafe()
    {
        cafeOpen = false;
        Debug.Log("Cafe is now CLOSED");

        // Hide cursor for player movement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Show the button again
        if (openCafeButton != null)
            openCafeButton.gameObject.SetActive(true);

        // Show the player again
        if (player != null)
        {
            player.SetActive(true);
            Debug.Log("Player shown");
        }
    }

    public void AddServedCustomer()
    {
        if (gameEnded) return;

        customersServed++;
        Debug.Log($"Customers served: {customersServed}/{customersToServeToWin}");
        UpdateCustomerCountUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    private void UpdateCustomerCountUI()
    {
        if (customerCountText != null)
        {
            customerCountText.text = $"Customers: {customersServed}/{customersToServeToWin}";
        }
    }

    private void WinGame()
    {
        gameEnded = true;
        sessionActive = false; // End the session
        Debug.Log("YOU WIN! You served enough customers!");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Optionally close cafe after winning
        // CloseCafe();
    }

    public void LoseGame(string reason = "Time ran out!")
    {
        if (gameEnded) return; // Prevent multiple lose triggers

        gameEnded = true;
        sessionActive = false; // End the session
        Debug.Log($"GAME OVER! {reason}");

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        // Optionally close cafe after losing
        // CloseCafe();
    }

    public void RestartCafe()
    {
        CloseCafe();
        Invoke("OpenCafe", 0.5f);
    }

    public void QuitCafe()
    {
        // Hide both panels
        if (winPanel != null)
            winPanel.SetActive(false);
        if (losePanel != null)
            losePanel.SetActive(false);

        // Close the cafe and return to normal gameplay
        CloseCafe();

        Debug.Log("Quit cafe - returning to game");
    }
}
