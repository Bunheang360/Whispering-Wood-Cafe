using UnityEngine;
using TMPro;

public class MoneyPop : MonoBehaviour
{
    public TextMeshProUGUI popText;
    public float riseSpeed = 50f;
    public float duration = 1f;

    private float timer = 0f;

    public void Play(string text, Color color)
    {
        if (popText != null)
        {
            popText.text = text;
            popText.color = color;
        }

        timer = 0f;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        // Move upward
        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);

        // Auto destroy after duration
        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);
    }
}
