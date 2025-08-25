using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 12f; // Slightly increased for Minecraft-like speed
    public float strafeSpeed = 4f;   // Reduced for subtle strafing
    public float turnSpeed = 30f;    // Adjusted for smoother, less aggressive turning

    [Header("Physics Settings")]
    public float slideFactor = 0.6f; // Mimics combined friction (0.5 dynamic + 0.7 static, averaged)
    public float momentumDecay ; // Low decay for ice-like sliding
    public float turnDamping = 0.9f;   // Gentle damping for smooth turn stopping

    [Header("References")]
    public Rigidbody boatRigidbody;
    public Transform mainCamera;

    private Vector3 moveVelocity; // Tracks velocity for sliding
    private float turnVelocity;   // Tracks angular velocity for turning
    private float moveInput;
    private float strafeInput;
    private float turnInput;

    void Start()
    {
        // Get Rigidbody if not assigned
        if (boatRigidbody == null)
            boatRigidbody = GetComponent<Rigidbody>();

        // Get main camera if not assigned
        if (mainCamera == null)
        {
            if (Camera.main != null)
                mainCamera = Camera.main.transform;
            else
                Debug.LogError("No main camera found! Please assign a camera.");
        }

    }

    void Update()
    {
        // Capture input
        moveInput = Input.GetAxisRaw("Vertical"); // Raw for sharper response
        strafeInput = 0f;
        turnInput = Input.GetAxisRaw("Horizontal");

        // Strafing with Q/E keys
        if (Input.GetKey(KeyCode.Q))
            strafeInput = -1f;
        if (Input.GetKey(KeyCode.E))
            strafeInput = 1f;
    }

    void FixedUpdate()
    {
        if (mainCamera == null || boatRigidbody == null) return;

        // Get camera-based directions
        Vector3 cameraForward = Vector3.ProjectOnPlane(mainCamera.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(mainCamera.right, Vector3.up).normalized;

        // Apply movement forces
        Vector3 targetVelocity = Vector3.zero;
        if (moveInput != 0)
            targetVelocity += cameraForward * moveInput * forwardSpeed;
        if (strafeInput != 0)
            targetVelocity += cameraRight * strafeInput * strafeSpeed;

        // Blend current velocity with target for sliding effect (ice-like)
        moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, slideFactor * Time.fixedDeltaTime);
        boatRigidbody.linearVelocity = Vector3.Lerp(boatRigidbody.linearVelocity, moveVelocity, momentumDecay);

        // Apply turning
        if (turnInput != 0)
        {
            float targetTurn = turnInput * turnSpeed;
            turnVelocity = Mathf.Lerp(turnVelocity, targetTurn, slideFactor * Time.fixedDeltaTime);
        }
        else
        {
            // Gradually reduce turning when no input (Minecraft-like drift)
            turnVelocity *= turnDamping;
        }

        // Apply angular velocity for rotation
        boatRigidbody.angularVelocity = new Vector3(0, turnVelocity * Time.fixedDeltaTime, 0);

        // Ensure boat stays upright (no tilting on X/Z axes)
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        boatRigidbody.rotation = Quaternion.Lerp(boatRigidbody.rotation, targetRotation, 0.1f);
    }
}