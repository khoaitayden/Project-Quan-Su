using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;        // Speed of movement
    public float moveDistance = 5f;     // Distance to move in each direction
    public bool startMovingRight = true; // Direction to start moving

    [Header("Movement Type")]
    public bool smoothMovement = true;  // Smooth sine wave vs linear back-and-forth
    public float smoothMultiplier = 1f; // For sine wave movement speed

    [Header("Physics Settings")]
    public bool useRigidbody = true;    // Use Rigidbody for physics interaction
    public float mass = 10f;            // Mass of the obstacle (higher = harder to push)

    private Vector3 startPosition;
    private bool movingRight;
    private float timeElapsed = 0f;
    private Rigidbody obstacleRigidbody;

    void Start()
    {
        startPosition = transform.position;
        movingRight = startMovingRight;

        // Initialize Rigidbody if enabled
        if (useRigidbody)
        {
            obstacleRigidbody = GetComponent<Rigidbody>();
            if (obstacleRigidbody == null)
            {
                obstacleRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            
            obstacleRigidbody.mass = mass;
            obstacleRigidbody.useGravity = false;
            obstacleRigidbody.isKinematic = false; // Allow physics to affect it
        }
    }

    void Update()
    {
        if (useRigidbody)
        {
            // Move the obstacle using physics
            if (smoothMovement)
            {
                // Smooth sine wave movement
                timeElapsed += Time.deltaTime;
                float offset = Mathf.Sin(timeElapsed * moveSpeed * smoothMultiplier) * moveDistance;
                transform.position = startPosition + Vector3.right * offset;
            }
            else
            {
                // Linear back-and-forth movement
                MoveLinear();
            }
        }
        else
        {
            // Move without physics (if you want it to be completely unaffected by physics)
            if (smoothMovement)
            {
                // Smooth sine wave movement
                timeElapsed += Time.deltaTime;
                float offset = Mathf.Sin(timeElapsed * moveSpeed * smoothMultiplier) * moveDistance;
                transform.position = startPosition + Vector3.right * offset;
            }
            else
            {
                // Linear back-and-forth movement
                MoveLinear();
            }
        }
    }

    private void MoveLinear()
    {
        Vector3 targetPosition;
        
        if (movingRight)
        {
            targetPosition = startPosition + Vector3.right * moveDistance;
        }
        else
        {
            targetPosition = startPosition + Vector3.left * moveDistance;
        }

        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if reached target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            movingRight = !movingRight; // Reverse direction
        }
    }

    // Optional: Visualize movement path in editor
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            startPosition = transform.position;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition + Vector3.left * moveDistance, startPosition + Vector3.right * moveDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startPosition + Vector3.left * moveDistance, 0.2f);
        Gizmos.DrawSphere(startPosition + Vector3.right * moveDistance, 0.2f);
    }
}