using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Cafe/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public float cookTime;
    public int price;
    public Sprite icon;
    public List<string> ingredients; // List of ingredient names
}   