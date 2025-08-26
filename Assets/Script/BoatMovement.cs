using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed ; 
    public float strafeSpeed ;   
    public float turnSpeed ;    

    [Header("Physics Settings")]
    public float slideFactor;
    public float momentumDecay ;
    public float turnDamping ;

    [Header("References")]
    public Rigidbody boatRigidbody;

    private Vector3 moveVelocity;
    private float turnVelocity;
    private float moveInput;  
    private float strafeInput; 
    private float turnInput;  

    void Start()
    {
        if (boatRigidbody == null)
            boatRigidbody = GetComponent<Rigidbody>();

    }

    void Update()
    {
        moveInput = -Input.GetAxisRaw("Vertical"); 
        strafeInput = 0f;
        if (Input.GetKey(KeyCode.Q))
            strafeInput = -1f;
        if (Input.GetKey(KeyCode.E))
            strafeInput = 1f;
        turnInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        if (boatRigidbody == null) return;

        Vector3 boatForward = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        Vector3 boatSide = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        Vector3 targetVelocity = Vector3.zero;
        if (moveInput != 0) 
            targetVelocity += boatForward * moveInput * forwardSpeed;
        if (strafeInput != 0) 
            targetVelocity += boatSide * strafeInput * strafeSpeed;

        moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, slideFactor * Time.fixedDeltaTime);
        boatRigidbody.linearVelocity = Vector3.Lerp(boatRigidbody.linearVelocity, moveVelocity, momentumDecay);

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
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        boatRigidbody.rotation = Quaternion.Lerp(boatRigidbody.rotation, targetRotation, 0.1f);
    }
}