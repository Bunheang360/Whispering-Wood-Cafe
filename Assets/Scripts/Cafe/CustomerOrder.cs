using UnityEngine;
using UnityEngine.UI;

public class CustomerOrder : MonoBehaviour
{
    public RecipeData wantedRecipe;
    public float patience = 10f;
    public bool orderActive = false;

    public GameObject speechTextObject; // Assign any GameObject with a text component
    public GameObject drinkPanel;   // assign from spawner or Inspector

    private CustomerSpawner spawner; // Reference to spawner to notify when angry

    public void SetSpawner(CustomerSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    void Start()
    {
        // Hide speech text when customer first spawns
        if (speechTextObject != null)
            speechTextObject.SetActive(false);
    }

    // Helper method to set text - works with any text component
    private void SetSpeechText(string text)
    {
        if (speechTextObject == null)
        {
            Debug.LogWarning("speechTextObject is NULL! Assign it in the Inspector.");
            return;
        }

        // Hide text object if text is empty, show if text has content
        speechTextObject.SetActive(!string.IsNullOrEmpty(text));

        Debug.Log($"Setting speech text to: '{text}'");

        // Try Unity UI Text
        Text uiText = speechTextObject.GetComponent<Text>();
        if (uiText != null)
        {
            uiText.text = text;
            Debug.Log($"Successfully set UI Text to: '{text}'");
            return;
        }

        // Try TextMeshPro (using reflection to avoid compilation errors)
        Component tmpComponent = speechTextObject.GetComponent("TMPro.TextMeshProUGUI");
        if (tmpComponent == null)
            tmpComponent = speechTextObject.GetComponent("TMPro.TMP_Text");

        if (tmpComponent != null)
        {
            var textProperty = tmpComponent.GetType().GetProperty("text");
            if (textProperty != null)
            {
                textProperty.SetValue(tmpComponent, text);
                Debug.Log($"Successfully set TMP text to: '{text}'");
            }
        }
        else
        {
            Debug.LogWarning("No Text or TextMeshPro component found on speechTextObject!");
        }
    }

    void Update()
    {
        if (!orderActive || !CafeStateManager.Instance.cafeOpen)
            return;

        patience -= Time.deltaTime;

        if (patience <= 0)
        {
            Debug.Log("Customer got ANGRY");
            orderActive = false;
            SetSpeechText("ANGRY!");
            if (drinkPanel != null)
                drinkPanel.SetActive(false);

            // Make angry customer leave after 2 seconds
            if (spawner != null)
            {
                Invoke("LeaveAngry", 2f);
            }
        }
    }

    public void StartOrder(RecipeData recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("Recipe is null in StartOrder!");
            return;
        }

        wantedRecipe = recipe;
        patience = 10f;
        orderActive = true;

        Debug.Log($"Customer wants: {recipe.recipeName}");

        SetSpeechText("I want: " + recipe.recipeName);
        Debug.Log($"Set speech text to: I want: {recipe.recipeName}");

        if (drinkPanel != null)
            drinkPanel.SetActive(true); // show DrinkPanel for this order
    }

    public void EndOrder()
    {
        orderActive = false;

        if (drinkPanel != null)
            drinkPanel.SetActive(false);

        SetSpeechText("");
    }

    private void LeaveAngry()
    {
        // Customer leaves without being served
        if (spawner != null)
        {
            spawner.CustomerLeftAngry();
        }
    }
}
