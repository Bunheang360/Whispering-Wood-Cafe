using UnityEngine;
using System.Collections;

public class FadeUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.3f;

    public void FadeIn()
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(Fade(1));
    }

    public void FadeOut()
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(Fade(0));
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        canvasGroup.interactable = targetAlpha == 1;
        canvasGroup.blocksRaycasts = targetAlpha == 1;
    }
}
