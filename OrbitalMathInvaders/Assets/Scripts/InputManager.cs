using UnityEngine;
// using TMPro; // If displaying buffer to a TMP_InputField or Text. Uncomment if used.

/// <summary>
/// Handles player input for building numeric strings and submitting answers.
/// </summary>
public class InputManager : MonoBehaviour
{
    [Header("Input Configuration")]
    [Tooltip("Maximum length of the input buffer.")]
    [SerializeField] private int maxBufferLength = 5;

    // [Tooltip("Optional: TextMeshPro UI element to display the current input buffer.")]
    // public TMP_Text inputBufferDisplay; // Assign in Inspector if needed. Uncomment if used.

    private string currentInputBuffer = "";
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("InputManager: GameManager not found in scene!");
        }
    }

    void Update()
    {
        // Only process game input if GameManager exists, game is active, and not paused
        if (gameManager == null || !gameManager.gameActive || gameManager.isPaused)
        {
            return;
        }

        ProcessNumericInput();
        ProcessControlInput();
        // UpdateDisplay(); // Call this if inputBufferDisplay is used
    }

    private void ProcessNumericInput()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                AppendToBuffer(i.ToString());
                break;
            }
        }
    }

    private void ProcessControlInput()
    {
        // Submit Answer
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SubmitCurrentBuffer();
        }

        // Backspace
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            PerformBackspace();
        }
    }

    private void AppendToBuffer(string digit)
    {
        if (currentInputBuffer.Length < maxBufferLength)
        {
            currentInputBuffer += digit;
            // UpdateDisplay(); // Call if live display is needed
        }
    }

    private void PerformBackspace()
    {
        if (currentInputBuffer.Length > 0)
        {
            currentInputBuffer = currentInputBuffer.Substring(0, currentInputBuffer.Length - 1);
            // UpdateDisplay(); // Call if live display is needed
        }
    }

    private void SubmitCurrentBuffer()
    {
        if (currentInputBuffer.Length > 0)
        {
            // gameManager null check is technically redundant here due to Update() check, but good practice.
            if (gameManager != null)
            {
                gameManager.MatchAnswer(currentInputBuffer);
            }
            ClearBuffer(); // Clear buffer after submission
        }
    }

    public void ClearBuffer()
    {
        currentInputBuffer = "";
        // UpdateDisplay(); // Call if live display is needed
    }

    // private void UpdateDisplay() // Uncomment and implement if using inputBufferDisplay
    // {
    //     if (inputBufferDisplay != null)
    //     {
    //         inputBufferDisplay.text = currentInputBuffer;
    //     }
    // }

    // --- Public methods for UI Buttons (if an on-screen numpad/controls are used) ---
    public void AppendDigitViaButton(string digit)
    {
        if (gameManager == null || !gameManager.gameActive || gameManager.isPaused) return;
        AppendToBuffer(digit);
    }

    public void BackspaceViaButton()
    {
        if (gameManager == null || !gameManager.gameActive || gameManager.isPaused) return;
        PerformBackspace();
    }

    public void SubmitViaButton()
    {
        if (gameManager == null || !gameManager.gameActive || gameManager.isPaused) return;
        SubmitCurrentBuffer();
    }
}
