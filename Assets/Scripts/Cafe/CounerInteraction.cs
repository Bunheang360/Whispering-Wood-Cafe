using UnityEngine;

public class CounterInteraction : MonoBehaviour
{
    public FadeUI openCafeFade;
    private bool playerInRange = false;

    private void Update()
    {
        // Press E to open cafe when in range
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (CafeStateManager.Instance != null && !CafeStateManager.Instance.cafeOpen)
            {
                CafeStateManager.Instance.OpenCafe();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (openCafeFade != null)
                openCafeFade.FadeIn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (openCafeFade != null)
                openCafeFade.FadeOut();
        }
    }
}
