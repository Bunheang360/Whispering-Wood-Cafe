using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    public RectTransform menuPanel;
    public float animationSpeed = 8f;

    private bool isOpen = false;
    private Vector3 hiddenScale = Vector3.zero;
    private Vector3 shownScale = new Vector3(2f, 2f, 1f);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isOpen = !isOpen;
        }

        Vector3 targetScale = isOpen ? shownScale : hiddenScale;
        menuPanel.localScale = Vector3.Lerp(
            menuPanel.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );
    }
}   
