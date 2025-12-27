using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// MiniGameManager - Handles all mini-games in WW Café
/// Attach to a GameObject in your scene (e.g., "MiniGameManager")
/// </summary>
public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [Header("=== PANELS ===")]
    [Tooltip("The main gameplay HUD (coins, time, etc.)")]
    public GameObject gameplayHUD;
    
    [Tooltip("Coffee Grinder mini-game panel")]
    public GameObject grinderPanel;
    
    [Tooltip("Water Pour mini-game panel")]
    public GameObject pourPanel;
    
    [Tooltip("Serve mini-game panel")]
    public GameObject servePanel;
    
    [Tooltip("Results panel shown after mini-game")]
    public GameObject resultsPanel;

    [Header("=== GRINDER MINI-GAME ===")]
    public RectTransform grinderPointer;
    public RectTransform grinderProgressBar;
    public RectTransform grinderPerfectZone;
    public Button grinderButton;
    public TMP_Text grinderTimerText;
    public Image[] grinderStars;
    public Sprite starFilled;
    public Sprite starEmpty;

    [Header("=== POUR MINI-GAME ===")]
    public RectTransform pourPointer;
    public RectTransform pourProgressBar;
    public RectTransform pourPerfectZone;
    public Image cupFillImage;
    public Button pourButton;
    public TMP_Text pourTimerText;
    public Image[] pourStars;

    [Header("=== RESULTS ===")]
    public TMP_Text resultsTitleText;
    public TMP_Text resultsScoreText;
    public Image[] resultsStars;
    public Button resultsCloseButton;

    [Header("=== SETTINGS ===")]
    [Tooltip("How fast the pointer moves")]
    public float pointerSpeed = 300f;
    
    [Tooltip("Time limit for each mini-game in seconds")]
    public float timeLimit = 30f;
    
    [Tooltip("Points for perfect timing")]
    public int perfectScore = 100;
    
    [Tooltip("Points for good timing")]
    public int goodScore = 50;

    [Header("=== EVENTS ===")]
    public UnityEvent onMiniGameStart;
    public UnityEvent onMiniGameComplete;
    public UnityEvent<int> onScoreEarned;

    // Private variables
    private float currentTime;
    private bool isPlaying;
    private bool isHolding;
    private float pointerPosition;
    private int totalScore;
    private int totalStars;
    private int currentStep;
    private float perfectZoneStart;
    private float perfectZoneEnd;
    private Coroutine timerCoroutine;

    // Mini-game state
    public enum MiniGameState
    {
        None,
        Grinder,
        Pour,
        Serve,
        Results
    }
    
    public MiniGameState CurrentState { get; private set; } = MiniGameState.None;

    #region Unity Lifecycle

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Hide all panels at start
        HideAllPanels();
        
        // Setup button listeners
        SetupButtons();
    }

    private void Update()
    {
        if (!isPlaying) return;

        switch (CurrentState)
        {
            case MiniGameState.Grinder:
                UpdateGrinderGame();
                break;
            case MiniGameState.Pour:
                UpdatePourGame();
                break;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start the complete coffee making sequence
    /// </summary>
    public void StartCoffeeMaking()
    {
        totalScore = 0;
        totalStars = 0;
        currentStep = 1;
        
        onMiniGameStart?.Invoke();
        
        // Start with grinder
        StartGrinderGame();
    }

    /// <summary>
    /// Start only the grinder mini-game
    /// </summary>
    public void StartGrinderGame()
    {
        CurrentState = MiniGameState.Grinder;
        HideAllPanels();
        
        if (gameplayHUD != null)
            gameplayHUD.SetActive(false);
        
        if (grinderPanel != null)
            grinderPanel.SetActive(true);
        
        ResetGrinderGame();
        StartTimer();
        isPlaying = true;
    }

    /// <summary>
    /// Start only the pour mini-game
    /// </summary>
    public void StartPourGame()
    {
        CurrentState = MiniGameState.Pour;
        HideAllPanels();
        
        if (gameplayHUD != null)
            gameplayHUD.SetActive(false);
        
        if (pourPanel != null)
            pourPanel.SetActive(true);
        
        ResetPourGame();
        StartTimer();
        isPlaying = true;
    }

    /// <summary>
    /// End current mini-game and show results
    /// </summary>
    public void EndMiniGame()
    {
        isPlaying = false;
        
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        
        ShowResults();
    }

    /// <summary>
    /// Close mini-game and return to gameplay
    /// </summary>
    public void CloseMiniGame()
    {
        isPlaying = false;
        CurrentState = MiniGameState.None;
        
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        
        HideAllPanels();
        
        if (gameplayHUD != null)
            gameplayHUD.SetActive(true);
        
        onMiniGameComplete?.Invoke();
    }

    /// <summary>
    /// Called when player presses/holds the action button
    /// </summary>
    public void OnActionButtonDown()
    {
        isHolding = true;
    }

    /// <summary>
    /// Called when player releases the action button
    /// </summary>
    public void OnActionButtonUp()
    {
        isHolding = false;
        
        if (!isPlaying) return;

        switch (CurrentState)
        {
            case MiniGameState.Grinder:
                CheckGrinderResult();
                break;
            case MiniGameState.Pour:
                CheckPourResult();
                break;
        }
    }

    #endregion

    #region Grinder Mini-Game

    private void ResetGrinderGame()
    {
        pointerPosition = 0f;
        isHolding = false;
        currentTime = timeLimit;
        
        // Reset pointer position
        if (grinderPointer != null)
        {
            grinderPointer.anchoredPosition = new Vector2(0, grinderPointer.anchoredPosition.y);
        }
        
        // Reset stars
        UpdateStars(grinderStars, 0);
        
        // Calculate perfect zone (random position)
        CalculatePerfectZone();
    }

    private void UpdateGrinderGame()
    {
        if (isHolding && grinderPointer != null && grinderProgressBar != null)
        {
            // Move pointer to the right while holding
            float barWidth = grinderProgressBar.rect.width;
            pointerPosition += pointerSpeed * Time.deltaTime;
            
            // Clamp and wrap
            if (pointerPosition > barWidth)
            {
                pointerPosition = 0f;
            }
            
            // Update pointer visual position
            float startX = -barWidth / 2;
            grinderPointer.anchoredPosition = new Vector2(startX + pointerPosition, grinderPointer.anchoredPosition.y);
        }
    }

    private void CheckGrinderResult()
    {
        int stars = CalculateStars();
        totalStars += stars;
        totalScore += stars * perfectScore / 3;
        
        UpdateStars(grinderStars, stars);
        
        onScoreEarned?.Invoke(stars * perfectScore / 3);
        
        // Move to next step or end
        StartCoroutine(NextStepDelay());
    }

    #endregion

    #region Pour Mini-Game

    private void ResetPourGame()
    {
        pointerPosition = 0f;
        isHolding = false;
        currentTime = timeLimit;
        
        // Reset pointer
        if (pourPointer != null)
        {
            pourPointer.anchoredPosition = new Vector2(0, pourPointer.anchoredPosition.y);
        }
        
        // Reset cup fill
        if (cupFillImage != null)
        {
            cupFillImage.fillAmount = 0f;
        }
        
        // Reset stars
        UpdateStars(pourStars, 0);
        
        // Calculate perfect zone
        CalculatePerfectZone();
    }

    private void UpdatePourGame()
    {
        if (pourPointer != null && pourProgressBar != null)
        {
            // Pointer moves automatically in pour game
            float barWidth = pourProgressBar.rect.width;
            pointerPosition += pointerSpeed * 0.5f * Time.deltaTime;
            
            // Update cup fill
            if (cupFillImage != null)
            {
                cupFillImage.fillAmount = pointerPosition / barWidth;
            }
            
            // Check if overflowed
            if (pointerPosition > barWidth)
            {
                // Failed - overflowed!
                CheckPourResult();
                return;
            }
            
            // Update pointer visual
            float startX = -barWidth / 2;
            pourPointer.anchoredPosition = new Vector2(startX + pointerPosition, pourPointer.anchoredPosition.y);
        }
    }

    private void CheckPourResult()
    {
        int stars = CalculateStars();
        totalStars += stars;
        totalScore += stars * perfectScore / 3;
        
        UpdateStars(pourStars, stars);
        
        onScoreEarned?.Invoke(stars * perfectScore / 3);
        
        // Move to next step or end
        StartCoroutine(NextStepDelay());
    }

    #endregion

    #region Helper Methods

    private void HideAllPanels()
    {
        if (grinderPanel != null) grinderPanel.SetActive(false);
        if (pourPanel != null) pourPanel.SetActive(false);
        if (servePanel != null) servePanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        // Setup grinder button with pointer down/up events
        if (grinderButton != null)
        {
            // Add EventTrigger for hold functionality
            AddHoldEvents(grinderButton.gameObject);
        }
        
        if (pourButton != null)
        {
            pourButton.onClick.AddListener(OnActionButtonUp);
        }
        
        if (resultsCloseButton != null)
        {
            resultsCloseButton.onClick.AddListener(CloseMiniGame);
        }
    }

    private void AddHoldEvents(GameObject buttonObj)
    {
        var trigger = buttonObj.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = buttonObj.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        // Pointer Down
        var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => OnActionButtonDown());
        trigger.triggers.Add(pointerDown);

        // Pointer Up
        var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => OnActionButtonUp());
        trigger.triggers.Add(pointerUp);
    }

    private void CalculatePerfectZone()
    {
        if (grinderProgressBar == null) return;
        
        float barWidth = grinderProgressBar.rect.width;
        float zoneWidth = barWidth * 0.2f; // 20% of bar is perfect zone
        
        // Random position for perfect zone (not at edges)
        float minStart = barWidth * 0.2f;
        float maxStart = barWidth * 0.6f;
        perfectZoneStart = Random.Range(minStart, maxStart);
        perfectZoneEnd = perfectZoneStart + zoneWidth;
        
        // Update perfect zone visual position
        if (grinderPerfectZone != null)
        {
            float startX = -barWidth / 2;
            grinderPerfectZone.anchoredPosition = new Vector2(startX + perfectZoneStart + zoneWidth / 2, grinderPerfectZone.anchoredPosition.y);
        }
        
        if (pourPerfectZone != null)
        {
            float startX = -barWidth / 2;
            pourPerfectZone.anchoredPosition = new Vector2(startX + perfectZoneStart + zoneWidth / 2, pourPerfectZone.anchoredPosition.y);
        }
    }

    private int CalculateStars()
    {
        // Check if pointer is in perfect zone
        if (pointerPosition >= perfectZoneStart && pointerPosition <= perfectZoneEnd)
        {
            // Perfect! 3 stars
            return 3;
        }
        
        // Check if close to perfect zone
        float distance = 0f;
        if (pointerPosition < perfectZoneStart)
        {
            distance = perfectZoneStart - pointerPosition;
        }
        else
        {
            distance = pointerPosition - perfectZoneEnd;
        }
        
        float barWidth = grinderProgressBar != null ? grinderProgressBar.rect.width : 600f;
        float tolerance = barWidth * 0.1f; // 10% tolerance for 2 stars
        
        if (distance <= tolerance)
        {
            return 2;
        }
        else if (distance <= tolerance * 2)
        {
            return 1;
        }
        
        return 0;
    }

    private void UpdateStars(Image[] stars, int filledCount)
    {
        if (stars == null) return;
        
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
            {
                stars[i].sprite = i < filledCount ? starFilled : starEmpty;
            }
        }
    }

    private void StartTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        
        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        currentTime = timeLimit;
        
        while (currentTime > 0 && isPlaying)
        {
            currentTime -= Time.deltaTime;
            
            // Update timer text
            string timeText = Mathf.CeilToInt(currentTime) + "s";
            
            if (CurrentState == MiniGameState.Grinder && grinderTimerText != null)
            {
                grinderTimerText.text = timeText;
            }
            else if (CurrentState == MiniGameState.Pour && pourTimerText != null)
            {
                pourTimerText.text = timeText;
            }
            
            yield return null;
        }
        
        // Time's up!
        if (isPlaying)
        {
            OnActionButtonUp(); // Force end current action
        }
    }

    private IEnumerator NextStepDelay()
    {
        isPlaying = false;
        
        yield return new WaitForSeconds(1.5f);
        
        currentStep++;
        
        if (currentStep == 2)
        {
            StartPourGame();
        }
        else if (currentStep == 3)
        {
            // Show serve/results
            ShowResults();
        }
        else
        {
            ShowResults();
        }
    }

    private void ShowResults()
    {
        CurrentState = MiniGameState.Results;
        HideAllPanels();
        
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }
        
        // Update results UI
        if (resultsTitleText != null)
        {
            if (totalStars >= 5)
                resultsTitleText.text = "Perfect Coffee!";
            else if (totalStars >= 3)
                resultsTitleText.text = "Good Coffee!";
            else
                resultsTitleText.text = "Coffee Made";
        }
        
        if (resultsScoreText != null)
        {
            resultsScoreText.text = "Score: " + totalScore;
        }
        
        // Show total stars (out of 6 for 2 mini-games)
        int displayStars = Mathf.Min(totalStars, 3); // Cap at 3 for display
        UpdateStars(resultsStars, displayStars);
    }

    #endregion

    #region Public Getters

    public int GetTotalScore() => totalScore;
    public int GetTotalStars() => totalStars;
    public bool IsPlaying() => isPlaying;

    #endregion
}