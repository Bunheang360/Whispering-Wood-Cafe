using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimator : MonoBehaviour
{
    [Header("Animation Type")]
    public bool enableFloat = true;
    public bool enablePulse = true;
    public bool enableRotation = false;
    public bool enableParallax = false;

    [Header("Float Settings")]
    public float floatSpeed = 1f;
    public float floatAmountX = 0f;
    public float floatAmountY = 10f;

    [Header("Pulse Settings")]
    public float pulseSpeed = 1f;
    public float pulseMin = 0.95f;
    public float pulseMax = 1.05f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;

    [Header("Parallax Settings (Mouse Follow)")]
    public float parallaxStrength = 20f;
    public float parallaxSmoothing = 5f;

    [Header("Fade Settings")]
    public bool enableFadeInOut = false;
    public float fadeSpeed = 1f;
    public float fadeMin = 0.7f;
    public float fadeMax = 1f;

    private RectTransform rectTransform;
    private Image image;
    private Vector3 startPosition;
    private Vector3 startScale;
    private float startRotation;
    private Vector2 parallaxOffset;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        if (rectTransform != null)
        {
            startPosition = rectTransform.anchoredPosition;
            startScale = rectTransform.localScale;
            startRotation = rectTransform.localEulerAngles.z;
        }
    }

    void Update()
    {
        if (rectTransform == null) return;

        Vector3 newPosition = startPosition;
        Vector3 newScale = startScale;
        float newRotation = startRotation;

        // Floating animation
        if (enableFloat)
        {
            float offsetX = Mathf.Sin(Time.time * floatSpeed) * floatAmountX;
            float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmountY;
            newPosition = new Vector3(startPosition.x + offsetX, startPosition.y + offsetY, startPosition.z);
        }

        // Pulsing scale animation
        if (enablePulse)
        {
            float scale = Mathf.Lerp(pulseMin, pulseMax, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            newScale = startScale * scale;
        }

        // Rotation animation
        if (enableRotation)
        {
            newRotation = startRotation + (Time.time * rotationSpeed);
        }

        // Parallax effect (mouse follow)
        if (enableParallax)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 offset = (mousePos - screenCenter) / screenCenter;
            
            parallaxOffset = Vector2.Lerp(parallaxOffset, offset * parallaxStrength, Time.deltaTime * parallaxSmoothing);
            newPosition += new Vector3(parallaxOffset.x, parallaxOffset.y, 0);
        }

        // Fade in/out animation
        if (enableFadeInOut && image != null)
        {
            float alpha = Mathf.Lerp(fadeMin, fadeMax, (Mathf.Sin(Time.time * fadeSpeed) + 1f) / 2f);
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        // Apply transformations
        rectTransform.anchoredPosition = newPosition;
        rectTransform.localScale = newScale;
        rectTransform.localEulerAngles = new Vector3(0, 0, newRotation);
    }

    // Public methods to control animations at runtime
    public void SetFloatEnabled(bool enabled) => enableFloat = enabled;
    public void SetPulseEnabled(bool enabled) => enablePulse = enabled;
    public void SetRotationEnabled(bool enabled) => enableRotation = enabled;
    public void SetParallaxEnabled(bool enabled) => enableParallax = enabled;

    public void SetFloatAmount(float x, float y)
    {
        floatAmountX = x;
        floatAmountY = y;
    }

    public void SetPulseRange(float min, float max)
    {
        pulseMin = min;
        pulseMax = max;
    }
}
