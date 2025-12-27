using UnityEngine;

public class DrinkPanelAnimation : MonoBehaviour
{
    public RectTransform panelTransform;
    public CanvasGroup canvasGroup;
    public Vector3 hiddenPosition;
    public Vector3 shownPosition;
    public float duration = 0.5f;

    private bool isVisible = false;

    void Awake()
    {
        // Start hidden
        panelTransform.anchoredPosition = hiddenPosition;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowPanel()
    {
        if (isVisible) return;
        isVisible = true;
        gameObject.SetActive(true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        StopAllCoroutines();
        StartCoroutine(SlideAndFade(panelTransform.anchoredPosition, shownPosition, true));
    }

    public void HidePanel()
    {
        if (!isVisible) return;
        isVisible = false;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        StopAllCoroutines();
        StartCoroutine(SlideAndFade(panelTransform.anchoredPosition, hiddenPosition, false));
    }

    System.Collections.IEnumerator SlideAndFade(Vector3 from, Vector3 to, bool fadingIn)
    {
        float elapsed = 0f;
        float startAlpha = fadingIn ? 0f : 1f;
        float endAlpha = fadingIn ? 1f : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            panelTransform.anchoredPosition = Vector3.Lerp(from, to, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        panelTransform.anchoredPosition = to;
        canvasGroup.alpha = endAlpha;

        if (!fadingIn)
            gameObject.SetActive(false);
    }
}
