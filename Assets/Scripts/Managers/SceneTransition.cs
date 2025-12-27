using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Fade Settings")]
    public RawImage fadeImage;
    public float fadeDuration = 1f;
    public Color fadeColor = Color.black;

    private bool isTransitioning = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeAndLoadScene(sceneName));
        }
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        isTransitioning = true;

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Wait a frame for scene to load
        yield return null;

        // Fade in
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            if (fadeImage != null)
            {
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            }

            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);

            if (fadeImage != null)
            {
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            }

            yield return null;
        }

        // Ensure it's completely transparent
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        }
    }
}
