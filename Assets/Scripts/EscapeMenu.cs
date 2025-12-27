using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
  [Header("UI Panel")]
  public GameObject escapePanel; // Panel that shows when ESC is pressed

  [Header("Scene Settings")]
  public string outsideSceneName = "Outside"; // Name of the outside scene

  private bool isPanelOpen = false;
  private bool wasCafeOpenLastFrame = false;

  void Start()
  {
    // Hide panel at start
    if (escapePanel != null)
      escapePanel.SetActive(false);
  }

  void Update()
  {
    // Track cafe state
    bool isCafeOpenNow = CafeStateManager.Instance != null && CafeStateManager.Instance.cafeOpen;

    // If cafe opens while panel is open, close the panel
    if (isCafeOpenNow && isPanelOpen)
    {
      ClosePanel();
    }

    // If cafe just closed this frame, don't process ESC (prevent conflict)
    if (wasCafeOpenLastFrame && !isCafeOpenNow)
    {
      wasCafeOpenLastFrame = isCafeOpenNow;
      return;
    }

    wasCafeOpenLastFrame = isCafeOpenNow;

    // Only handle ESC when cafe is NOT open
    if (isCafeOpenNow)
      return;

    if (Input.GetKeyDown(KeyCode.Escape))
    {
      TogglePanel();
    }
  }

  public void TogglePanel()
  {
    isPanelOpen = !isPanelOpen;

    if (escapePanel != null)
      escapePanel.SetActive(isPanelOpen);

    // Show/hide cursor
    if (isPanelOpen)
    {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }
    else
    {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }
  }

  public void GoToOutside()
  {
    Debug.Log($"Going to outside scene: {outsideSceneName}");

    // Use smooth transition if available
    if (SceneTransition.Instance != null)
    {
      SceneTransition.Instance.LoadSceneWithFade(outsideSceneName);
    }
    else
    {
      // Fallback to direct load
      UnityEngine.SceneManagement.SceneManager.LoadScene(outsideSceneName);
    }
  }

  public void ClosePanel()
  {
    isPanelOpen = false;

    if (escapePanel != null)
      escapePanel.SetActive(false);

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }
}
