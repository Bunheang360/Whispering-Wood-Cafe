using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;

    [Header("UI Elements")]
    public GameObject interactionPanel;
    public TextMeshProUGUI interactionText;

    [Header("Settings")]
    public string defaultPrompt = "Press E to Enter";

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        HidePrompt();
    }

    public void ShowPrompt(string message = null)
    {
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(true);
            if (interactionText != null)
            {
                interactionText.text = message ?? defaultPrompt;
            }
        }
    }

    public void HidePrompt()
    {
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false);
        }
    }
}
