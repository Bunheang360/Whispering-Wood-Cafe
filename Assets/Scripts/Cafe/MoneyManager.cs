using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int money = 0;

    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public GameObject moneyPopPrefab;
    public Transform uiParent;

    void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void Add(int amount)
    {
        money += amount;
        UpdateUI();
        SpawnPop($"+{amount}", Color.green);
    }

    public void Subtract(int amount)
    {
        money -= amount;
        if (money < 0) money = 0;
        UpdateUI();
        SpawnPop($"-{amount}", Color.red);
    }

    void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = $"{money}";
    }

    void SpawnPop(string text, Color color)
    {
        if (moneyPopPrefab == null || uiParent == null) return;

        GameObject pop = Instantiate(moneyPopPrefab, uiParent);
        pop.GetComponent<MoneyPop>().Play(text, color);
    }
}
