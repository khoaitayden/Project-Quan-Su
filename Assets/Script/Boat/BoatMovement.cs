using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed ; // Speed for forward/backward (W/S)
    public float strafeSpeed ;   // Speed for strafing (Q/E)
    public float turnSpeed ;    // Speed for turning (A/D)

    [Header("Physics Settings")]
    public float slideFactor ; // Reduced to allow more natural physics
    public float momentumDecay ; // Adjusted for less damping
    public float turnDamping ;

    [Header("References")]
    public Rigidbody boatRigidbody;

    private Vector3 moveVelocity;
    private float turnVelocity;
    private float moveInput;  // W/S for forward/backward
    private float strafeInput; // Q/E for strafing
    private float turnInput;   // A/D for turning

    void Start()
    {
        if (boatRigidbody == null)
            boatRigidbody = GetComponent<Rigidbody>();

        if (boatRigidbody != null)
        {
            // Enable gravity and optimize physics
            boatRigidbody.useGravity = true; // Ensure gravity is on
            boatRigidbody.linearDamping = 0.1f;       // Low drag to allow faster fall
            boatRigidbody.angularDamping = 0.5f; // Low angular drag for natural rotation
            boatRigidbody.mass = 5f;         // Increased mass for better gravity response
        }
    }

    void Update()
    {
        moveInput = -Input.GetAxisRaw("Vertical"); // W = forward, S = backward
        strafeInput = 0f;
        if (Input.GetKey(KeyCode.Q)) strafeInput = -1f; // Strafe left
        if (Input.GetKey(KeyCode.E)) strafeInput = 1f;  // Strafe right
        turnInput = Input.GetAxisRaw("Horizontal");     // A/D for turning
    }

    void FixedUpdate()
    {
        if (boatRigidbody == null) return;

        Vector3 boatForward = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        Vector3 boatSide = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        Vector3 targetVelocity = Vector3.zero;
        if (moveInput != 0) targetVelocity += boatForward * moveInput * forwardSpeed;
        if (strafeInput != 0) targetVelocity += boatSide * strafeInput * strafeSpeed;

        // Apply velocity only when on map or moving intentionally
        if (transform.position.y > -0.5f) // Threshold for map, adjust as needed
        {
            moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, slideFactor * Time.fixedDeltaTime);
            boatRigidbody.linearVelocity = Vector3.Lerp(boatRigidbody.linearVelocity, moveVelocity, momentumDecay);
        }
        else
        {
            // Let gravity take over fully when off map
            boatRigidbody.linearVelocity = new Vector3(boatRigidbody.linearVelocity.x, boatRigidbody.linearVelocity.y, boatRigidbody.linearVelocity.z);
        }

        if (turnInput != 0)
        {
            float targetTurn = turnInput * turnSpeed;
            turnVelocity = Mathf.Lerp(turnVelocity, targetTurn, slideFactor * Time.fixedDeltaTime);
        }
        else
        {
            turnVelocity *= turnDamping;
        }

        boatRigidbody.angularVelocity = new Vector3(0, turnVelocity * Time.fixedDeltaTime, 0);
        // Removed rotation Lerp to allow natural physics-driven orientation
    }
}