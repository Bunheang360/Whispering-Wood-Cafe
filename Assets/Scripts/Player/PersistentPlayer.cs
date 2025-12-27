using UnityEngine;

public class PersistentPlayer : MonoBehaviour
{
    public static PersistentPlayer Instance;

    [Header("Scene Spawn Points")]
    public Vector3 interiorSpawnPosition = new Vector3(0, 1, 0);
    public Vector3 exteriorSpawnPosition = new Vector3(0, 1, 0);

    void Awake()
    {
        // Singleton pattern - keep this player alive between scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy duplicate players from new scene
            Destroy(gameObject);
        }
    }

    // Call this when entering a new scene to reposition player
    public void SetSpawnPosition(Vector3 position)
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            transform.position = position;
            controller.enabled = true;
        }
        else
        {
            transform.position = position;
        }
    }

    void OnEnable()
    {
        // Subscribe to scene loaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from scene loaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Reposition player based on scene name
        if (scene.name.Contains("Interior") || scene.name.Contains("Inside") || scene.name.Contains("Tavern"))
        {
            SetSpawnPosition(interiorSpawnPosition);
        }
        else
        {
            SetSpawnPosition(exteriorSpawnPosition);
        }

        // Make sure camera finds the player again
        ThirdPersonCamera camera = FindObjectOfType<ThirdPersonCamera>();
        if (camera != null && camera.target == null)
        {
            camera.target = transform;
        }
    }
}
