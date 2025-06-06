using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshProUGUI

/// <summary>
/// Responsible for spawning meteors/comets.
/// </summary>
public class Spawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The prefab for the standard meteor.")]
    public GameObject meteorPrefab; // Assign in Inspector
    [Tooltip("The prefab for the comet (e.g., golden comet). Assign in Inspector.")]
    public GameObject cometPrefab; // Assign in Inspector
    [Tooltip("Reference to the EquationGenerator in the scene.")]
    public EquationGenerator equationGenerator; // Assign in Inspector or FindObjectOfType

    [Header("Spawn Configuration")]
    [Tooltip("Initial interval between spawns in seconds.")]
    public float spawnInterval = 2.0f;
    [Tooltip("Minimum spawn X position.")]
    public float minSpawnX = -5.0f;
    [Tooltip("Maximum spawn X position.")]
    public float maxSpawnX = 5.0f;
    [Tooltip("Y position where meteors will be spawned.")]
    public float spawnYPosition = 7.0f;
    // Consider adding a transform reference for spawn position if not using fixed Y

    private Coroutine spawningCoroutine;
    private GameManager gameManager; // Will be set by GameManager or found

    void Start()
    {
        // It's better if GameManager provides its reference or if Spawner is explicitly set up.
        // For now, allow finding, but Inspector assignment for equationGenerator is good.
        if (equationGenerator == null)
        {
            equationGenerator = FindObjectOfType<EquationGenerator>();
            if (equationGenerator == null)
            {
                Debug.LogError("Spawner: EquationGenerator not found in scene and not assigned!");
            }
        }
        // GameManager reference will be set up via GameManager's Initialize method.
    }

    /// <summary>
    /// Initializes the Spawner with a reference to the GameManager.
    /// Call this from GameManager after it has initialized.
    /// </summary>
    public void Initialize(GameManager manager)
    {
        gameManager = manager;
    }


    /// <summary>
    /// Starts the meteor spawning coroutine if conditions are met.
    /// </summary>
    public void StartSpawning()
    {
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
        }

        // Ensure gameManager is available and game is active
        if (gameManager != null && gameManager.gameActive && meteorPrefab != null && equationGenerator != null)
        {
            spawningCoroutine = StartCoroutine(SpawnRoutine());
            Debug.Log("Spawner: Started spawning meteors.");
        }
        else
        {
            Debug.LogWarning("Spawner: Could not start spawning. Conditions not met.");
            if(gameManager == null) Debug.LogError("Spawner: GameManager reference is missing.");
            else if (!gameManager.gameActive) Debug.LogWarning("Spawner: Game is not active.");
            if(meteorPrefab == null) Debug.LogError("Spawner: MeteorPrefab is not assigned!");
            if(equationGenerator == null) Debug.LogError("Spawner: EquationGenerator is not assigned or found!");
        }
    }

    /// <summary>
    /// Stops the meteor spawning coroutine.
    /// </summary>
    public void StopSpawning()
    {
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
            spawningCoroutine = null;
            Debug.Log("Spawner: Stopped spawning meteors.");
        }
    }

    private IEnumerator SpawnRoutine()
    {
        // Wait a brief moment before first spawn, or use spawnInterval directly
        yield return new WaitForSeconds(spawnInterval * 0.5f);

        while (true) // GameManager.gameActive and isPaused checks are handled by GameManager starting/stopping this coroutine
        {
            // Determine which prefab to spawn (normal meteor or special comet)
            GameObject prefabToSpawn = meteorPrefab;
            bool isSpecialComet = false;

            // TODO: Add logic for Golden Comet spawning here later if Spawner handles it directly.
            // Example: if (Random.value < 0.1f && cometPrefab != null) { prefabToSpawn = cometPrefab; isSpecialComet = true; }

            if (prefabToSpawn != null)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(minSpawnX, maxSpawnX), spawnYPosition, 0);
                GameObject newMeteorObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

                Meteor meteorScript = newMeteorObject.GetComponent<Meteor>();
                if (meteorScript != null && equationGenerator != null)
                {
                    string expr;
                    int ans;
                    equationGenerator.GenerateEquation(out expr, out ans);

                    meteorScript.expression = expr;
                    meteorScript.answer = ans;
                    meteorScript.isComet = isSpecialComet;

                    TextMeshProUGUI tmpComponent = newMeteorObject.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmpComponent != null)
                    {
                        tmpComponent.text = expr;
                    }
                    else
                    {
                        // Debug.LogWarning($"Meteor {newMeteorObject.name} has no TextMeshProUGUI child to display expression.");
                    }
                }
                else
                {
                    if(meteorScript == null) Debug.LogError("Spawned object does not have a Meteor script component!");
                    // EquationGenerator null check already done in StartSpawning
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Spawns a specific meteor type, typically for special events like Golden Comet.
    /// </summary>
    /// <param name="specialPrefab">The prefab to spawn (e.g., cometPrefab).</param>
    /// <param name="isItComet">Is this instance a comet?</param>
    /// <param name="expr">Custom expression, if any.</param>
    /// <param name="ans">Custom answer, if any.</param>
    public void SpawnSpecial(GameObject specialPrefab, bool isItComet, string expr = null, int? ans = null)
    {
        if (specialPrefab == null || equationGenerator == null || (gameManager != null && !gameManager.gameActive))
        {
            Debug.LogWarning("Spawner: Cannot spawn special meteor. Prefab, EquationGenerator missing, or game not active.");
            return;
        }

        Vector3 spawnPosition = new Vector3(Random.Range(minSpawnX, maxSpawnX), spawnYPosition, 0);
        GameObject newMeteorObject = Instantiate(specialPrefab, spawnPosition, Quaternion.identity);
        Meteor meteorScript = newMeteorObject.GetComponent<Meteor>();

        if (meteorScript != null)
        {
            string finalExpr;
            int finalAns;

            if (expr != null && ans.HasValue)
            {
                finalExpr = expr;
                finalAns = ans.Value;
            }
            else
            {
                equationGenerator.GenerateEquation(out finalExpr, out finalAns);
            }

            meteorScript.expression = finalExpr;
            meteorScript.answer = finalAns;
            meteorScript.isComet = isItComet;

            TextMeshProUGUI tmpComponent = newMeteorObject.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpComponent != null)
            {
                tmpComponent.text = finalExpr;
            }
        }
        Debug.Log($"Spawner: Spawned special meteor {specialPrefab.name}");
    }


    /// <summary>
    /// Updates the spawn interval, e.g., for difficulty escalation.
    /// </summary>
    /// <param name="newInterval">The new interval between spawns.</param>
    public void SetSpawnInterval(float newInterval)
    {
        if (newInterval > 0.1f) // Ensure interval is not too small
        {
            spawnInterval = newInterval;
            Debug.Log($"Spawner: Spawn interval updated to {newInterval}s.");
        }
        else
        {
            Debug.LogWarning($"Spawner: Attempted to set spawn interval too low ({newInterval}s). Clamping to 0.1s.");
            spawnInterval = 0.1f;
        }
    }
}
