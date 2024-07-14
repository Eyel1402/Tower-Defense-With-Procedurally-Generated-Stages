using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public GameGrid gameGrid;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lerpSpeed = 0.1f;
    private Vector3 targetPosition;
    public static bool isMoving { get; private set; } // Static variable to track camera movement

    // Screen shake variables
    [SerializeField] private float shakeMagnitude = 0.25f;
    [SerializeField] private float shakeDuration = 0.05f;
    private Coroutine shakeCoroutine;

    void Start()
    {
        targetPosition = new Vector3(gameGrid.width / 2f, gameGrid.height / 2f, transform.position.z);
    }

    void Update()
    {
        HandleInput();
        UpdatePosition();
    }

    void HandleInput()
    {
        // Get input from the user
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate the move direction
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f);

        // Update the target position based on input and unscaled time
        targetPosition += moveDirection * moveSpeed * Time.unscaledDeltaTime;

        // Clamp the target position to the game grid boundaries
        targetPosition.x = Mathf.Clamp(targetPosition.x, 0f, gameGrid.width);
        targetPosition.y = Mathf.Clamp(targetPosition.y, 0f, gameGrid.height);

        // Set isMoving based on input
        isMoving = moveDirection != Vector3.zero;

    }
    void UpdatePosition()
    {
        // Only update position if not shaking (assuming shakeCoroutine is null)
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed);
        // Debugging information
        Debug.Log("Camera Position: " + transform.position);
    }
    // Method to trigger screen shake
    public void TriggerShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ScreenShake());
    }

    private IEnumerator ScreenShake()
    {
        Vector3 initialPosition = transform.position; // Track initial position when shake starts
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomPoint = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            transform.position = randomPoint;

            elapsed += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale

            yield return null;
        }

        // Reset the position and stop shaking
        transform.position = initialPosition;
        shakeCoroutine = null;
    }
}
