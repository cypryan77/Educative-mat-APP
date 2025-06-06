using UnityEngine;

/// <summary>
/// Represents a meteor or comet in the game, carrying a math expression.
/// </summary>
public class Meteor : MonoBehaviour
{
    /// <summary>
    /// The mathematical expression displayed on the meteor.
    /// Example: "2+2"
    /// </summary>
    [Tooltip("The mathematical expression displayed on the meteor. Example: "2+2"")]
    public string expression;

    /// <summary>
    /// The correct answer to the expression.
    /// </summary>
    [Tooltip("The correct answer to the expression.")]
    public int answer;

    /// <summary>
    /// Is this a special comet (e.g., golden comet for power-up)?
    /// </summary>
    [Tooltip("Is this a special comet (e.g., golden comet for power-up)?")]
    public bool isComet = false;

    // Basic Unity lifecycle methods (can be expanded later)
    void Start()
    {
        // Initialization logic for a meteor, if any.
        // For example, displaying the expression using TextMeshPro if it were a child object.
    }

    void Update()
    {
        // Movement logic will be added later.
    }
}
