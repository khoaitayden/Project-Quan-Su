using UnityEngine;

public class BoatCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform boatTransform; 
    public float followDistance = 10f; 
    public float followHeight = 5f;   
    public float followSpeed = 5f;    
    public float followSmoothness = 0.3f; 
    public float minFOV = 40f; 
    public float maxFOV = 60f; 
    public float fovChangeSpeed = 2f; 

    private Rigidbody boatRigidbody;

    void Start()
    {

        boatRigidbody = boatTransform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (boatTransform == null || boatRigidbody == null) return;

        UpdateCameraPosition();
        UpdateFOV();
    }

    void UpdateCameraPosition()
    {
        Vector3 boatForward = -Vector3.ProjectOnPlane(boatTransform.right, Vector3.up).normalized;
        Vector3 targetPosition = boatTransform.position - boatForward * followDistance + Vector3.up * followHeight;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSmoothness * Time.deltaTime * followSpeed);

        transform.LookAt(boatTransform.position);
    }

    void UpdateFOV()
    {
        float speed = boatRigidbody.linearVelocity.magnitude;
        float maxSpeed = 15f;
        float speedRatio = Mathf.Clamp01(speed / maxSpeed);

        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            float targetFOV = Mathf.Lerp(maxFOV, minFOV, speedRatio);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        }
    }
}