using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Inventory;

public class DrinkMiniGame : MonoBehaviour
{
    public List<string> selectedIngredients = new List<string>();
    public GameObject displayTextObject; // Assign GameObject with Text component
    private bool brewed = false;

    public void AddIngredient(string ingredient)
    {
        // Check if Inventory exists
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory.Instance is NULL! Make sure Inventory exists in the scene.");
            selectedIngredients.Add(ingredient);
            UpdateText();
            return;
        }

        // Check if player has this ingredient in inventory
        int quantity = Inventory.Instance.GetItemQuantity(ingredient);

        if (quantity < 1)
        {
            // Show warning message
            SetDisplayText($"You don't have '{ingredient}'!");
            Debug.LogWarning($"Cannot add {ingredient} - not in inventory!");
            return;
        }

        selectedIngredients.Add(ingredient);
        UpdateText();
        Debug.Log($"Added {ingredient} to drink");
    }

    void UpdateText()
    {
        string textToShow = selectedIngredients.Count > 0
            ? string.Join(", ", selectedIngredients)
            : "Select ingredients...";

        SetDisplayText(textToShow);
    }

    void SetDisplayText(string text)
    {
        if (displayTextObject == null)
        {
            Debug.LogWarning("displayTextObject is not assigned!");
            return;
        }

        // Try Unity UI Text
        Text uiText = displayTextObject.GetComponent<Text>();
        if (uiText != null)
        {
            uiText.text = text;
            Debug.Log($"Updated selection text to: {text}");
            return;
        }

        // Try TextMeshPro (using reflection to avoid compilation errors)
        Component tmpComponent = displayTextObject.GetComponent("TMPro.TextMeshProUGUI");
        if (tmpComponent == null)
            tmpComponent = displayTextObject.GetComponent("TMPro.TMP_Text");

        if (tmpComponent != null)
        {
            var textProperty = tmpComponent.GetType().GetProperty("text");
            if (textProperty != null)
            {
                textProperty.SetValue(tmpComponent, text);
                Debug.Log($"Updated selection text to: {text}");
            }
        }
        else
        {
            Debug.LogWarning("No Text or TextMeshPro component found on displayTextObject!");
        }
    }

    public void Brew()
    {
        if (selectedIngredients.Count == 0)
        {
            Debug.Log("Select ingredients first!");
            return;
        }
        brewed = true;
        Debug.Log("Drink brewed: " + string.Join(", ", selectedIngredients));
    }

    public bool IsBrewed()
    {
        return brewed;
    }

    public void ResetDrink()
    {
        selectedIngredients.Clear();
        brewed = false;
        UpdateText();
    }

    public bool MatchesRecipe(RecipeData recipe)
    {
        if (recipe.ingredients.Count != selectedIngredients.Count)
            return false;

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            if (recipe.ingredients[i] != selectedIngredients[i])
                return false;
        }
        return true;
    }
}
