using UnityEngine;

/// <summary>
/// Controls the player's cannon, including rotation to aim and firing projectiles.
/// </summary>
public class PlayerCannon : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Projectile prefab to be fired. Assign in Inspector.")]
    public GameObject projectilePrefab;
    [Tooltip("The transform where projectiles will be spawned. Typically an empty child GameObject at the cannon's muzzle.")]
    public Transform projectileSpawnPoint;

    [Header("Gameplay Settings")]
    [Tooltip("Speed at which the cannon rotates towards the mouse pointer.")]
    public float rotationSpeed = 200f; // Degrees per second
    [Tooltip("Minimum time interval between shots, in seconds.")]
    public float fireRate = 0.5f; // 2 shots per second

    private float nextFireTime = 0f;
    private Camera mainCamera;
    private GameManager gameManager;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("PlayerCannon: Main Camera not found! Ensure your main camera is tagged 'MainCamera'.");
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("PlayerCannon: GameManager not found in scene!");
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("PlayerCannon: ProjectilePrefab is not assigned in the Inspector!");
        }
        if (projectileSpawnPoint == null)
        {
            Debug.LogError("PlayerCannon: ProjectileSpawnPoint is not assigned in the Inspector! Create an empty child GameObject at the cannon's muzzle and assign it.");
            // As a fallback, use the cannon's own transform, but this is not ideal.
            if (transform != null) // Check if transform is available (it should be for a MonoBehaviour)
            {
                projectileSpawnPoint = transform;
            }
        }
    }

    void Update()
    {
        if (gameManager == null || !gameManager.gameActive || gameManager.isPaused)
        {
            return; // Do nothing if game is not active or is paused
        }

        HandleRotation();
        HandleShooting();
    }

    /// <summary>
    /// Rotates the cannon to aim towards the mouse cursor's position in the game world.
    /// </summary>
    private void HandleRotation()
    {
        if (mainCamera == null) return;

        // Convert mouse position from screen space to world space
        Vector3 mouseScreenPosition = Input.mousePosition;
        // Ensure a Z-depth is provided for ScreenToWorldPoint if using a perspective camera.
        // For a 2D setup with an orthographic camera, the Z value is less critical for direction
        // but matters for where the point is in world space. We only need the direction.
        mouseScreenPosition.z = mainCamera.nearClipPlane + 10f; // Arbitrary distance from camera
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // Calculate direction from cannon to mouse
        Vector2 directionToMouse = (mouseWorldPosition - transform.position).normalized;

        // Calculate the angle for rotation. Atan2 returns angle in radians.
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        // The cannon sprite might need an offset if its 'forward' isn't aligned with 0 degrees (right).
        // If sprite points upwards by default, subtract 90 degrees: angle - 90f.
        float rotationAngle = angle - 90f; // Assuming cannon sprite points 'up' by default. Adjust if needed.

        // Create the target rotation
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAngle);

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Handles firing projectiles based on player input and fire rate.
    /// </summary>
    private void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime) // "Fire1" is typically Left Mouse Button or Ctrl
        {
            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                nextFireTime = Time.time + fireRate;
                Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                // TODO: Play shooting sound effect
            }
        }
    }

    // It's good practice to ensure the cannon's sprite is set up correctly:
    // 1. Pivot: Set to Bottom Center in Sprite Editor for proper rotation around its base.
    // 2. Default Orientation: Ideally, the cannon sprite should point "upwards" (along positive Y axis in its local space)
    //    when its Z rotation is 0. The `angle - 90f` calculation assumes this. If it points right by default,
    //    then `angle` itself would be the target Z rotation.
}
