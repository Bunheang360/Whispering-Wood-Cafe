using UnityEngine;

public class SceneDoorTrigger : MonoBehaviour
{
  [Header("Scene Settings")]
  public string sceneToLoad = "Outside"; // Name of the scene to load

  [Header("UI (Optional)")]
  public GameObject promptUI; // Optional: "Press E to Exit" UI

  private bool playerInRange = false;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      playerInRange = true;

      if (promptUI != null)
        promptUI.SetActive(true);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      playerInRange = false;

      if (promptUI != null)
        promptUI.SetActive(false);
    }
  }

  private void Update()
  {
    if (playerInRange && Input.GetKeyDown(KeyCode.E))
    {
      LoadScene();
    }
  }

  private void LoadScene()
  {
    Debug.Log($"Loading scene: {sceneToLoad}");

    // Use smooth transition if available
    if (SceneTransition.Instance != null)
    {
      SceneTransition.Instance.LoadSceneWithFade(sceneToLoad);
    }
    else
    {
      // Fallback to direct load
      UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
  }
}
