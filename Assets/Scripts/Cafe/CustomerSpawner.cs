using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] customerPrefabs;  // Array of customer prefabs
    public Transform entryPoint;    // Where customer appears (door)
    public Transform counterPoint;  // Where customer walks to (counter)
    public RecipeData[] availableRecipes;
    public DrinkPanelAnimation panelAnimation;
    public GameObject drinkPanelUI; // The main drink-making UI panel

    [Header("Spawning Settings")]
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 2f;
    public float orderDelay = 2f;
    public float walkSpeed = 10f;
    public float customerScale = 8.18f;  // Size of customer

    private GameObject currentCustomer;
    private bool cafeWasOpen = false;

    void Update()
    {
        if (CafeStateManager.Instance == null) return;

        // When cafe opens, spawn first customer
        if (CafeStateManager.Instance.cafeOpen && !cafeWasOpen)
        {
            cafeWasOpen = true;
            StartCoroutine(SpawnFirstCustomer());
        }

        // When cafe closes, cleanup
        if (!CafeStateManager.Instance.cafeOpen && cafeWasOpen)
        {
            cafeWasOpen = false;
            StopAllCoroutines();
            DestroyAllCustomers();
            if (panelAnimation != null)
                panelAnimation.HidePanel();
        }
    }

    IEnumerator SpawnFirstCustomer()
    {
        float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
        Debug.Log($"First customer arriving in {delay:F1} seconds...");
        yield return new WaitForSeconds(delay);

        if (CafeStateManager.Instance.cafeOpen)
        {
            SpawnCustomer();
        }
    }

    void SpawnCustomer()
    {
        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogError("No CustomerPrefabs assigned!");
            return;
        }

        if (entryPoint == null || counterPoint == null)
        {
            Debug.LogError("EntryPoint or CounterPoint is not assigned!");
            return;
        }

        // Pick random prefab
        GameObject randomPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];

        // Spawn customer at entry point (door)
        currentCustomer = Instantiate(randomPrefab, entryPoint.position, entryPoint.rotation);
        currentCustomer.transform.localScale = Vector3.one * customerScale;  // Set size
        CustomerOrder order = currentCustomer.GetComponent<CustomerOrder>();

        if (order == null)
        {
            Debug.LogError("CustomerOrder component missing on prefab!");
            Destroy(currentCustomer);
            return;
        }

        Debug.Log("Customer entered!");

        // Assign the drink panel to this customer
        if (drinkPanelUI != null)
        {
            order.drinkPanel = drinkPanelUI;
        }

        // Give customer reference to spawner (so they can notify when angry)
        order.SetSpawner(this);

        // Link to ServeDrink
        ServeDrink serve = panelAnimation.GetComponent<ServeDrink>();
        if (serve != null)
        {
            serve.customer = order;
        }

        // Pick random recipe
        RecipeData recipe = null;
        if (availableRecipes != null && availableRecipes.Length > 0)
        {
            recipe = availableRecipes[Random.Range(0, availableRecipes.Length)];
        }

        // Walk to counter then place order
        StartCoroutine(WalkToCounter(currentCustomer, order, recipe));
    }

    IEnumerator WalkToCounter(GameObject customer, CustomerOrder order, RecipeData recipe)
    {
        if (customer == null) yield break;

        Transform customerTransform = customer.transform;
        Animator animator = customer.GetComponent<Animator>();

        // Start walk animation if available
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }

        // Walk to counter
        while (customer != null && Vector3.Distance(customerTransform.position, counterPoint.position) > 0.5f)
        {
            // Rotate to face walking direction
            Vector3 direction = (counterPoint.position - customerTransform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                customerTransform.rotation = Quaternion.Slerp(customerTransform.rotation, targetRotation, 10f * Time.deltaTime);
            }

            // Move towards counter
            customerTransform.position = Vector3.MoveTowards(
                customerTransform.position,
                counterPoint.position,
                walkSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Stop walk animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }

        // Turn left to face counter (rotate -90 degrees)
        Quaternion leftRotation = customerTransform.rotation * Quaternion.Euler(0, -90, 0);
        float turnTime = 0.3f;
        float elapsed = 0f;
        Quaternion startRotation = customerTransform.rotation;

        while (customer != null && elapsed < turnTime)
        {
            elapsed += Time.deltaTime;
            customerTransform.rotation = Quaternion.Slerp(startRotation, leftRotation, elapsed / turnTime);
            yield return null;
        }

        Debug.Log("Customer arrived at counter!");

        // Place order after arriving
        if (order != null && recipe != null)
        {
            yield return new WaitForSeconds(orderDelay);
            order.StartOrder(recipe);
            Debug.Log($"Customer wants: {recipe.recipeName}");

            if (panelAnimation != null)
                panelAnimation.ShowPanel();
        }
    }

    public void CustomerServed()
    {
        Debug.Log("Customer served! Walking out...");

        // Make customer walk back to entry and leave
        if (currentCustomer != null)
        {
            StartCoroutine(WalkOutAndSpawnNext(currentCustomer));
        }
        else
        {
            // No customer found, just spawn next
            StartCoroutine(SpawnNextCustomer());
        }
    }

    public void CustomerLeftAngry()
    {
        Debug.Log("Customer left ANGRY! YOU LOSE!");

        // Trigger game over
        if (CafeStateManager.Instance != null)
        {
            CafeStateManager.Instance.LoseGame("A customer left angry!");
        }

        // Make angry customer walk out
        if (currentCustomer != null)
        {
            StartCoroutine(WalkOutAndSpawnNext(currentCustomer));
        }
        else
        {
            // No customer found, just spawn next
            StartCoroutine(SpawnNextCustomer());
        }
    }

    IEnumerator WalkOutAndSpawnNext(GameObject customer)
    {
        if (customer == null) yield break;

        Transform customerTransform = customer.transform;
        Animator animator = customer.GetComponent<Animator>();

        // Turn to face exit
        Vector3 direction = (entryPoint.position - customerTransform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float turnTime = 0.3f;
            float elapsed = 0f;
            Quaternion startRotation = customerTransform.rotation;

            while (customer != null && elapsed < turnTime)
            {
                elapsed += Time.deltaTime;
                customerTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / turnTime);
                yield return null;
            }
        }

        // Start walk animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }

        // Walk back to entry point
        while (customer != null && Vector3.Distance(customerTransform.position, entryPoint.position) > 0.5f)
        {
            // Face walking direction
            direction = (entryPoint.position - customerTransform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                customerTransform.rotation = Quaternion.Slerp(customerTransform.rotation, targetRotation, 10f * Time.deltaTime);
            }

            // Move towards entry
            customerTransform.position = Vector3.MoveTowards(
                customerTransform.position,
                entryPoint.position,
                walkSpeed * Time.deltaTime
            );
            yield return null;
        }

        Debug.Log("Customer left!");

        // Destroy customer
        Destroy(customer);
        currentCustomer = null;

        // Spawn next customer
        StartCoroutine(SpawnNextCustomer());
    }

    IEnumerator SpawnNextCustomer()
    {
        float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
        Debug.Log($"Next customer in {delay:F1} seconds...");
        yield return new WaitForSeconds(delay);

        if (CafeStateManager.Instance.cafeOpen)
        {
            SpawnCustomer();
        }
    }

    void DestroyAllCustomers()
    {
        CustomerOrder[] allCustomers = FindObjectsOfType<CustomerOrder>();
        foreach (CustomerOrder c in allCustomers)
        {
            Destroy(c.gameObject);
        }
        currentCustomer = null;
    }
}
