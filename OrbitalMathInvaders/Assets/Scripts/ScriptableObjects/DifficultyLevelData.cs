using UnityEngine;

/// <summary>
/// ScriptableObject to define parameters for a difficulty level.
/// This includes operand ranges for math equations and potentially other
/// difficulty-specific settings like meteor speed modifiers, spawn rates, etc.
/// </summary>
[CreateAssetMenu(fileName = "DifficultyLevel_", menuName = "OrbitalMathInvaders/Difficulty Level Data", order = 1)]
public class DifficultyLevelData : ScriptableObject
{
    [Header("Equation Parameters")]
    [Tooltip("Minimum value for operands in equations.")]
    public int minOperandValue = 1;

    [Tooltip("Maximum value for operands in equations.")]
    public int maxOperandValue = 10;

    // Future parameters could be added here:
    // [Header("Gameplay Modifiers")]
    // [Tooltip("Multiplier for meteor base speed.")]
    // public float meteorSpeedMultiplier = 1.0f;
    // [Tooltip("Base spawn interval for meteors at this difficulty.")]
    // public float spawnInterval = 2.0f;
    // [Tooltip("Can subtraction result in negative numbers?")]
    // public bool allowNegativeSubtractionResults = false;
    // [Tooltip("Maximum complexity for division (e.g., ensuring simpler dividends).")]
    // public int maxDivisionResult = 10;
}
