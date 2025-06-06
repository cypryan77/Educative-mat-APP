using UnityEngine;

// No longer need to define DifficultyLevelData here as it's in its own file.

/// <summary>
/// Generates mathematical expressions and their answers based on difficulty.
/// </summary>
public class EquationGenerator : MonoBehaviour
{
    [Header("Difficulty Configuration")]
    [Tooltip("ScriptableObject defining current difficulty parameters (operand ranges, etc.). This should be assigned in the Inspector or by the GameManager.")]
    public DifficultyLevelData currentDifficulty;

    public enum OperationType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    [Header("Current Operation")]
    [Tooltip("The current type of operation to generate equations for. This can be set by UI or GameManager.")]
    public OperationType selectedOperation = OperationType.Addition;

    void Start()
    {
        if (currentDifficulty == null)
        {
            Debug.LogError("EquationGenerator: CurrentDifficulty is not set! Please assign a DifficultyLevelData ScriptableObject in the Inspector or via GameManager.");
            // Optionally, load a default one from Resources or create a fallback.
            // For now, it will likely cause NullReferenceExceptions if not set.
        }
    }

    /// <summary>
    /// Generates a new equation based on the current difficulty and selected operation.
    /// </summary>
    /// <param name="expression">The generated expression string (out parameter).</param>
    /// <param name="answer">The calculated answer (out parameter).</param>
    public void GenerateEquation(out string expression, out int answer)
    {
        if (currentDifficulty == null)
        {
            Debug.LogError("Cannot generate equation: currentDifficulty is not set.");
            expression = "ERROR";
            answer = 0;
            return;
        }

        int minOp = currentDifficulty.minOperandValue;
        int maxOp = currentDifficulty.maxOperandValue;

        int operand1 = Random.Range(minOp, maxOp + 1);
        int operand2 = Random.Range(minOp, maxOp + 1);

        // Ensure operand2 is not zero for division, and is within reasonable bounds for other operations if needed
        if (selectedOperation == OperationType.Division)
        {
            if (operand2 == 0) operand2 = 1; // Prevent division by zero
            // Ensure operand1 is a multiple of operand2 for clean integer division
            // Adjust operand1 to be a product that results in a quotient within a desired range if necessary.
            // For simplicity now:
            answer = Random.Range(minOp, maxOp + 1); // Decide the answer first
            if (answer == 0 && minOp > 0) answer = minOp; // Avoid answer being 0 if minOp is positive
            if (operand2 == 0) operand2 = 1; // Ensure operand2 is not zero before multiplication

            operand1 = operand2 * answer;
            // This makes division problems like "X / operand2 = answer"
            // Boundary condition: if answer was 0 and op2 was 0, op1 becomes 0.
            // If minOp is 1, answer will be at least 1. If op2 is 0, it's corrected to 1. So op1 is at least 1.
        }
        else if (selectedOperation == OperationType.Subtraction)
        {
            // Optional: ensure positive result for simplicity, or handle negative answers based on difficulty setting
            // if (!currentDifficulty.allowNegativeSubtractionResults && operand1 < operand2)
            if (operand1 < operand2) // Defaulting to simpler subtraction (no negatives)
            {
                int temp = operand1;
                operand1 = operand2;
                operand2 = temp;
            }
        }


        switch (selectedOperation)
        {
            case OperationType.Addition:
                expression = $"{operand1} + {operand2}";
                answer = operand1 + operand2;
                break;
            case OperationType.Subtraction:
                expression = $"{operand1} - {operand2}";
                answer = operand1 - operand2;
                break;
            case OperationType.Multiplication:
                // For multiplication, ensure results don't get too large too quickly if operands are big
                // Potentially cap operand2 if operand1 is large, or cap the result.
                // For now, direct multiplication:
                expression = $"{operand1} * {operand2}";
                answer = operand1 * operand2;
                break;
            case OperationType.Division:
                // Logic for operand1 and answer already handled above to ensure clean division.
                expression = $"{operand1} / {operand2}";
                // answer is already set from the logic block above for Division
                // Safety check for division by zero, though operand2 should have been corrected
                if (operand2 == 0) {
                    // This case should ideally not be reached if logic above is correct
                    Debug.LogError("Division by zero prevented post-calculation. Check logic.");
                    expression = $"{operand1} / 1"; // Fallback
                    answer = operand1;
                } else {
                    answer = operand1 / operand2; // Perform the actual division
                }
                break;
            default:
                expression = "1+0"; // Fallback
                answer = 1;
                Debug.LogError("Unsupported operation type selected in EquationGenerator.");
                break;
        }
    }

    /// <summary>
    /// Sets the current difficulty level by assigning a DifficultyLevelData object.
    /// </summary>
    /// <param name="difficultyData">The ScriptableObject defining the new difficulty.</param>
    public void SetDifficultyLevel(DifficultyLevelData difficultyData)
    {
        if (difficultyData != null)
        {
            currentDifficulty = difficultyData;
            Debug.Log($"EquationGenerator: Difficulty level set to {difficultyData.name}");
        }
        else
        {
            Debug.LogError("Attempted to set a null DifficultyLevelData.");
        }
    }

    /// <summary>
    /// Sets the type of mathematical operation to be used for generating equations.
    /// </summary>
    /// <param name="type">The operation type.</param>
    public void SetOperationType(OperationType type)
    {
        this.selectedOperation = type;
        Debug.Log($"EquationGenerator: Operation type set to {type}");
    }

    /// <summary>
    /// Sets the type of mathematical operation using an integer index.
    /// Typically used by UI elements like Dropdowns or Toggle Groups.
    /// </summary>
    /// <param name="typeIndex">The integer index corresponding to the OperationType enum.</param>
    public void SetOperationType(int typeIndex)
    {
        if (System.Enum.IsDefined(typeof(OperationType), typeIndex))
        {
            this.selectedOperation = (OperationType)typeIndex;
            Debug.Log($"EquationGenerator: Operation type set to index {typeIndex} ({(OperationType)typeIndex})");
        }
        else
        {
            Debug.LogWarning($"EquationGenerator: Invalid operation type index: {typeIndex}");
        }
    }
}
