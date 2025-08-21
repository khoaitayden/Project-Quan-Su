using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;


    private Rigidbody rb;
    private bool isGrounded = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
        }
    }


    void FixedUpdate()
    {
        if (isGrounded)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");


            Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;

            if (moveDirection.magnitude > 0.1f)
            {
                rb.AddForce(moveDirection * moveSpeed * 10, ForceMode.Force);
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}