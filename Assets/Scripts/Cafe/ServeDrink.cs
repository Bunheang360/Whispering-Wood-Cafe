using UnityEngine;
using Game.Inventory;

public class ServeDrink : MonoBehaviour
{
    public CustomerOrder customer;
    public DrinkMiniGame miniGame; // assign in Inspector
    public DrinkPanelAnimation panelAnimation; // assign in Inspector
    public CustomerSpawner spawner; // assign in Inspector

    public void Serve()
    {
        Debug.Log("Serve button clicked!");

        if (customer == null)
        {
            Debug.LogError("Customer is null! Make sure CustomerSpawner assigned the customer.");
            return;
        }

        if (miniGame == null)
        {
            Debug.LogError("MiniGame is null! Assign it in Inspector.");
            return;
        }

        if (panelAnimation == null)
        {
            Debug.LogError("PanelAnimation is null! Assign it in Inspector.");
            return;
        }

        if (!miniGame.IsBrewed())
        {
            Debug.LogWarning("You need to BREW the drink first! Click the Brew button.");
            return;
        }

        Debug.Log("Drink is brewed, checking recipe match...");

        // Check if recipe matches first
        bool recipeMatches = miniGame.MatchesRecipe(customer.wantedRecipe);

        // Check if player has all required ingredients in inventory
        if (!HasRequiredIngredients(customer.wantedRecipe))
        {
            Debug.Log("You don't have the required ingredients!");

            // Show popup notification
            string missingItems = GetMissingIngredients(customer.wantedRecipe);
            InventoryUI.ShowNotification($"Missing ingredients: {missingItems}");

            Debug.LogWarning($"Cannot serve - missing: {missingItems}");
            return;
        }

        // Deduct ingredients from inventory
        DeductIngredients(customer.wantedRecipe);

        // Give reward or penalty based on recipe match
        if (recipeMatches)
        {
            Debug.Log("SUCCESS!");
            MoneyManager.Instance.Add(customer.wantedRecipe.price);

            // Count this as a successful serve for win condition
            if (CafeStateManager.Instance != null)
            {
                CafeStateManager.Instance.AddServedCustomer();
            }
        }
        else
        {
            Debug.Log("WRONG DRINK!");
            MoneyManager.Instance.Subtract(10);
        }

        miniGame.ResetDrink();
        customer.EndOrder();
        panelAnimation.HidePanel();

        // Notify spawner that customer was served
        if (spawner != null)
        {
            spawner.CustomerServed();
        }
    }

    private bool HasRequiredIngredients(RecipeData recipe)
    {
        if (recipe == null || recipe.ingredients == null)
            return false;

        foreach (string ingredient in recipe.ingredients)
        {
            int quantity = Inventory.Instance.GetItemQuantity(ingredient);
            if (quantity < 1)
            {
                Debug.Log($"Missing ingredient: {ingredient}");
                return false;
            }
        }
        return true;
    }

    private void DeductIngredients(RecipeData recipe)
    {
        if (recipe == null || recipe.ingredients == null)
            return;

        foreach (string ingredient in recipe.ingredients)
        {
            Inventory.Instance.RemoveItem(ingredient, 1);
            Debug.Log($"Used ingredient: {ingredient}");
        }
    }

    private string GetMissingIngredients(RecipeData recipe)
    {
        if (recipe == null || recipe.ingredients == null)
            return "Unknown";

        System.Text.StringBuilder missing = new System.Text.StringBuilder();
        foreach (string ingredient in recipe.ingredients)
        {
            int quantity = Inventory.Instance.GetItemQuantity(ingredient);
            if (quantity < 1)
            {
                if (missing.Length > 0) missing.Append(", ");
                missing.Append(ingredient);
            }
        }
        return missing.ToString();
    }
}
