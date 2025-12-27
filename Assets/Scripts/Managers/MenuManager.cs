using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;

    [Header("Tutorial UI Elements")]
    public TextMeshProUGUI tutorialTitle;
    public TextMeshProUGUI tutorialContent;
    public TextMeshProUGUI pageIndicator;
    public Image tutorialImage;
    public Button nextButton;
    public Button backButton;

    [Header("Tutorial Images (Optional)")]
    public Sprite[] tutorialSprites; // Assign in Inspector

    [Header("Background Animation")]
    public Image backgroundImage; // Assign background image to animate
    public float floatSpeed = 1f;
    public float floatAmount = 10f;
    public float pulseSpeed = 1f;
    public float pulseScale = 1.05f;

    private int currentTutorialPage = 0;
    private TutorialPage[] tutorialPages;
    private Vector3 backgroundStartPos;
    private Vector3 backgroundStartScale;

    [System.Serializable]
    public struct TutorialPage
    {
        public string title;
        [TextArea(3, 6)]
        public string content;
    }

    void Start()
    {
        InitializeTutorialPages();
        
        // Ensure panels start in correct state
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        // Initialize background animation
        if (backgroundImage != null)
        {
            backgroundStartPos = backgroundImage.rectTransform.anchoredPosition;
            backgroundStartScale = backgroundImage.rectTransform.localScale;
        }
    }

    void InitializeTutorialPages()
    {
        tutorialPages = new TutorialPage[]
        {
            new TutorialPage
            {
                title = "Welcome to Whispering Wood Café!",
                content = "Run your own cozy forest café!\n\nExplore the forest in the morning, gather ingredients, then return to cook and serve customers in the evening."
            },
            new TutorialPage
            {
                title = "Movement",
                content = "Use WASD keys to move around.\n\n• W - Move Up\n• A - Move Left\n• S - Move Down\n• D - Move Right\n\nHold Shift to run!"
            },
            new TutorialPage
            {
                title = "Interaction",
                content = "Press E to interact with objects and NPCs.\n\nLook for sparkling items in the forest - these can be foraged!\n\nTalk to villagers to receive quests and build friendships."
            },
            new TutorialPage
            {
                title = "Inventory & Tools",
                content = "Press Q to open your backpack.\n\nPress R to open your recipe book.\n\nUse your tools (net, fishing rod, basket) to gather different ingredients."
            },
            new TutorialPage
            {
                title = "Foraging",
                content = "Explore the forest to find ingredients:\n\n• Common: Berries, Herbs, Basic Fish\n• Uncommon: Mushrooms, Honey, Flowers\n• Rare: Glowing Flowers, Rare Fish\n\nIngredients respawn daily!"
            },
            new TutorialPage
            {
                title = "Cooking",
                content = "Use cooking stations in your café:\n\n• Cutting Board - Prep ingredients\n• Stove/Pan - Fry dishes\n• Oven - Bake goods\n• Kettle - Make drinks\n\nFollow recipes and don't overcook!"
            },
            new TutorialPage
            {
                title = "Serving Customers",
                content = "Customers have patience bars - serve quickly for better tips!\n\nEach customer has favorite dishes.\n\nHigher ratings = more customers tomorrow.\n\nGood luck with your café!"
            }
        };
    }

    // ==================== MAIN MENU ====================
    
    public void PlayGame()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.StopMusic();
        }
        SceneManager.LoadScene("OutsideMap");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit! (won't show in editor)");
    }

    // ==================== SETTINGS ====================
    
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // ==================== TUTORIAL ====================
    
    public void OpenTutorial()
    {
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(true);
        currentTutorialPage = 0;
        UpdateTutorialUI();
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void NextTutorialPage()
    {
        if (currentTutorialPage < tutorialPages.Length - 1)
        {
            currentTutorialPage++;
            UpdateTutorialUI();
        }
    }

    public void PreviousTutorialPage()
    {
        if (currentTutorialPage > 0)
        {
            currentTutorialPage--;
            UpdateTutorialUI();
        }
    }

    void UpdateTutorialUI()
    {
        // Update text content
        if (tutorialTitle != null)
            tutorialTitle.text = tutorialPages[currentTutorialPage].title;
        
        if (tutorialContent != null)
            tutorialContent.text = tutorialPages[currentTutorialPage].content;
        
        if (pageIndicator != null)
            pageIndicator.text = $"{currentTutorialPage + 1} / {tutorialPages.Length}";

        // Update tutorial image if available
        if (tutorialImage != null && tutorialSprites != null && tutorialSprites.Length > currentTutorialPage)
        {
            tutorialImage.sprite = tutorialSprites[currentTutorialPage];
            tutorialImage.gameObject.SetActive(tutorialSprites[currentTutorialPage] != null);
        }

        // Update button visibility
        if (backButton != null)
            backButton.gameObject.SetActive(currentTutorialPage > 0);
        
        if (nextButton != null)
            nextButton.gameObject.SetActive(currentTutorialPage < tutorialPages.Length - 1);
    }

    // Optional: Skip to specific page
    public void GoToTutorialPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < tutorialPages.Length)
        {
            currentTutorialPage = pageIndex;
            UpdateTutorialUI();
        }
    }

    // ==================== BACKGROUND ANIMATION ====================

    void Update()
    {
        AnimateBackground();
    }

    void AnimateBackground()
    {
        if (backgroundImage == null) return;

        // Floating animation
        float newY = backgroundStartPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        backgroundImage.rectTransform.anchoredPosition = new Vector2(backgroundStartPos.x, newY);

        // Pulsing scale animation
        float scale = 1f + (Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1f));
        backgroundImage.rectTransform.localScale = backgroundStartScale * scale;
    }
}