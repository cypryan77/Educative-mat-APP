using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro; // Required if GameManager directly updates UI text elements

/// <summary>
/// Manages the overall game state, score, lives, power-ups, and scene transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    // --- Events ---
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnLivesChanged;
    public static event Action OnGameOver;
    public static event Action<bool> OnPauseStateChanged;

    [Header("Core References")]
    [Tooltip("Reference to the Spawner in the scene.")]
    public Spawner spawner; // Assign in Inspector
    [Tooltip("Reference to the EquationGenerator in the scene.")]
    public EquationGenerator equationGenerator; // Assign in Inspector

    [Header("UI References (Optional - for direct updates or listening)")]
    [Tooltip("TMP Text for displaying score. Can also listen to OnScoreChanged.")]
    public TextMeshProUGUI scoreText; // Assign in Inspector
    [Tooltip("TMP Text for displaying lives. Can also listen to OnLivesChanged.")]
    public TextMeshProUGUI livesText; // Assign in Inspector
    [Tooltip("Panel for the Pause Menu. Activated/deactivated on pause.")]
    public GameObject pauseMenuPanel; // Assign in Inspector
    // Add other UI references as needed (e.g. for difficulty dropdown, start button to hide)


    [Header("Game State")]
    [SerializeField] private int score = 0;
    [SerializeField] private int lives = 3;
    public bool isPowerUpActive = false;
    public bool gameActive = false;
    public bool isPaused = false;

    [Header("Game Configuration")]
    [SerializeField] private int initialLives = 3;
    [SerializeField] private string gamePlaySceneName = "GamePlay"; // Ensure this matches your scene name
    [Tooltip("Default difficulty level to start with. Assign the ScriptableObject asset.")]
    public DifficultyLevelData defaultDifficulty; // Assign Easy/Medium SO in Inspector


    public int Score => score;
    public int Lives => lives;

    // public static GameManager Instance { get; private set; } // Optional Singleton

    void Awake()
    {
        // Optional Singleton
        // if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); return; }

        // Ensure critical references are set
        if (spawner == null) spawner = FindObjectOfType<Spawner>();
        if (equationGenerator == null) equationGenerator = FindObjectOfType<EquationGenerator>();

        if (spawner == null) Debug.LogError("GameManager: Spawner not found or assigned!");
        if (equationGenerator == null) Debug.LogError("GameManager: EquationGenerator not found or assigned!");
        if (defaultDifficulty == null) Debug.LogError("GameManager: DefaultDifficulty not assigned in Inspector!");
    }

    void Start()
    {
        // Initialize game state but don't start automatically
        score = 0;
        lives = initialLives;
        gameActive = false;
        isPaused = false;
        Time.timeScale = 1f; // Ensure time scale is normal at the start

        // Setup initial UI states
        UpdateScoreDisplay(score); // Direct call for initial setup
        UpdateLivesDisplay(lives); // Direct call for initial setup
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        // Initialize other components that might need GameManager reference or initial setup
        if (spawner != null) spawner.Initialize(this); // Provide GameManager instance to Spawner
        if (equationGenerator != null && defaultDifficulty != null)
        {
            equationGenerator.SetDifficultyLevel(defaultDifficulty);
        }

        // Subscribe to own events if UI is directly managed here
        OnScoreChanged += UpdateScoreDisplay;
        OnLivesChanged += UpdateLivesDisplay;
        OnPauseStateChanged += HandlePauseMenuVisibility;
        OnGameOver += HandleGameOverLogistics;

        Debug.Log("GameManager initialized. UI updated. Waiting for StartGame command.");
    }

    void OnDestroy() // Unsubscribe from events to prevent memory leaks
    {
        OnScoreChanged -= UpdateScoreDisplay;
        OnLivesChanged -= UpdateLivesDisplay
        OnPauseStateChanged -= HandlePauseMenuVisibility;
        OnGameOver -= HandleGameOverLogistics;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Allow toggling pause if the game is active OR if it's paused (e.g., game over menu is up)
            if (gameActive || isPaused)
            {
                TogglePause();
            }
        }
    }

    /// <summary>
    /// Called by UI Start Button.
    /// </summary>
    public void StartGame()
    {
        if (gameActive) return; // Don't restart if already active, use RestartGame for that

        score = 0;
        lives = initialLives;
        isPowerUpActive = false;
        gameActive = true;
        isPaused = false;
        Time.timeScale = 1f;

        // Apply selected difficulty from UI if applicable, otherwise default is already set
        if (equationGenerator != null && equationGenerator.currentDifficulty == null && defaultDifficulty != null)
        {
             equationGenerator.SetDifficultyLevel(defaultDifficulty); // Ensure difficulty is set if not already
        }


        OnScoreChanged?.Invoke(score);
        OnLivesChanged?.Invoke(lives);
        OnPauseStateChanged?.Invoke(false); // Hides pause menu

        if (spawner != null)
        {
            spawner.StartSpawning();
        }
        else Debug.LogError("GameManager: Spawner reference missing, cannot start spawning!");

        // TODO: Hide main menu elements, ensure game HUD is visible
        Debug.Log("Game Started! Score and Lives reset. Spawner activated.");
    }

    // --- UI Update Handlers ---
    private void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null) scoreText.text = $"Score: {newScore}";
    }

    private void UpdateLivesDisplay(int newLives)
    {
        if (livesText != null) livesText.text = $"Lives: {newLives}";
    }

    private void HandlePauseMenuVisibility(bool showMenu)
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(showMenu);
            // Potentially adjust what's shown on the pause menu based on whether game is active or game over
            if (showMenu && !gameActive) // Game Over state
            {
                // Example: Disable "Resume" button if it's a game over screen
                // Transform resumeButton = pauseMenuPanel.transform.Find("ResumeButton");
                // if (resumeButton != null) resumeButton.gameObject.SetActive(false);
            }
            else if (showMenu && gameActive) // Paused state
            {
                // Example: Ensure "Resume" button is active
                // Transform resumeButton = pauseMenuPanel.transform.Find("ResumeButton");
                // if (resumeButton != null) resumeButton.gameObject.SetActive(true);
            }
        }
    }

    private void HandleGameOverLogistics() // Called by OnGameOver event
    {
        if (spawner != null)
        {
            spawner.StopSpawning();
        }
        Debug.Log("Game Over Logistics: Spawner stopped.");
        // Actual display of Game Over menu is handled by GameOver() -> ShowGameOverMenu()
    }


    public void MatchAnswer(string answerBuffer)
    {
        if (!gameActive || isPaused) return;

        Debug.Log($"GameManager: Attempting to match answer: {answerBuffer}");
        bool parseSuccess = int.TryParse(answerBuffer, out int playerAnswer);

        if (!parseSuccess)
        {
            Debug.LogWarning($"GameManager: Could not parse answer: {answerBuffer}");
            return;
        }

        // VITAL TODO: Find the correct meteor to check against.
        // This requires a system to track active meteors and determine which one is the target.
        // For now, simple placeholder.
        Meteor[] activeMeteors = FindObjectsOfType<Meteor>(); // Inefficient, use a list managed by Spawner or GameManager
        bool foundMatch = false;
        Meteor matchedMeteor = null;

        foreach (Meteor meteor in activeMeteors)
        {
            if (meteor.answer == playerAnswer) // Assuming one answer is enough, no specific targeting yet
            {
                AddScore(10); // Example points
                Debug.Log($"GameManager: Correct Answer for meteor {meteor.name} with expression {meteor.expression}!");
                // TODO: Provide visual/audio feedback for correct answer
                Destroy(meteor.gameObject); // Destroy the matched meteor
                foundMatch = true;
                matchedMeteor = meteor; // Keep track to potentially remove from a list
                break;
            }
        }

        if (foundMatch)
        {
            // TODO: Any other logic on correct match (e.g. remove 'matchedMeteor' from a managed list)
        }
        else
        {
            Debug.Log("GameManager: Incorrect Answer or no matching meteor found.");
            // TODO: Handle incorrect answer (e.g., penalty, sound)
            // LoseLife(); // Example penalty for wrong answer - uncomment if this is desired game mechanic
        }
    }

    public void AddScore(int points)
    {
        if (!gameActive) return; // Only add score if game is active
        score += (isPowerUpActive ? points * 2 : points);
        OnScoreChanged?.Invoke(score); // This will trigger UpdateScoreDisplay
    }

    public void LoseLife() // Called when a meteor hits the base, or for penalty
    {
        if (!gameActive || isPaused) return;
        lives--;
        OnLivesChanged?.Invoke(lives); // This will trigger UpdateLivesDisplay
        Debug.Log($"Lost a life. Lives remaining: {lives}");
        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        if (!gameActive && isPaused) return; // Prevent multiple calls if already in game over menu

        gameActive = false; // Stop main game logic
        Debug.Log("Game Over!");

        OnGameOver?.Invoke(); // Notify Spawner to stop, etc. This calls HandleGameOverLogistics.
        ShowGameOverMenu(); // Display the game over UI
    }

    private void ShowGameOverMenu()
    {
        Time.timeScale = 0f; // Pause game time
        isPaused = true; // Set isPaused to true to manage state (e.g. input blocking for game)
        OnPauseStateChanged?.Invoke(true); // This should show the pauseMenuPanel
        // TODO: Customize pauseMenuPanel for Game Over (e.g., hide "Resume", show "Game Over" text)
        // Example:
        // if(pauseMenuPanel != null) {
        //     Transform resumeButton = pauseMenuPanel.transform.Find("ResumeButton"); // Assuming named "ResumeButton"
        //     if(resumeButton != null) resumeButton.gameObject.SetActive(false);
        //     TextMeshProUGUI title = pauseMenuPanel.GetComponentInChildren<TextMeshProUGUI>(); // Assuming title is TMP
        //     if(title != null) title.text = "Game Over";
        // }
    }


    public void TogglePause()
    {
        // If game is over (not active but is paused), "unpausing" might mean going to main menu or restarting.
        // This logic is for when the game is active and can be paused/resumed.
        if (!gameActive && isPaused)
        {
            Debug.Log("GameManager: Game is over. 'Unpausing' is handled by menu buttons (Restart/Quit).");
            return; // Don't toggle Time.timeScale or isPaused if game is already over.
        }
        if (!gameActive && !isPaused) return; // Can't pause if game hasn't started and isn't paused.

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        OnPauseStateChanged?.Invoke(isPaused); // Shows/hides pauseMenuPanel
        Debug.Log(isPaused ? "Game Paused" : "Game Resumed");
    }

    // --- UI Button Callbacks (to be assigned in Inspector) ---
    public void OnResumeButtonPressed()
    {
        if (gameActive && isPaused) // Only resume if game was active and is now paused
        {
            TogglePause();
        }
    }

    public void OnRestartButtonPressed()
    {
        RestartGame();
    }

    public void OnQuitButtonPressed()
    {
        QuitGame();
    }


    public void RestartGame()
    {
        Time.timeScale = 1f; // Reset time scale before loading
        // Reset any static events if necessary, though new scene load should handle most things
        SceneManager.LoadScene(gamePlaySceneName);
        // New GameManager instance will run its Start() method.
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
