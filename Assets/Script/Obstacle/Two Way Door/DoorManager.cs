using UnityEngine;
using System.Collections.Generic;

public class DoorManager : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject doorFramePrefab;
    public QuestionBank questionBank;
    public Transform slopeStart; // Bottom of slope
    public Transform slopeEnd;   // Top of slope
    public float doorSpacing = 3f; // Distance between doors
    public float movementSpeed = 1f; // How fast doors move up
    public float raycastDistance = 2f; // Distance to raycast down to find slope angle
    
    [Header("Checkpoint Settings")]
    public List<Checkpoint> startCheckpoints = new List<Checkpoint>();
    public List<Checkpoint> endCheckpoints = new List<Checkpoint>();
    
    private List<DoorPair> doorPairs = new List<DoorPair>();
    private bool isSpawning = false;
    private bool isMoving = false;
    private int currentQuestionIndex = 0;
    private float slopeLength;
    private Vector3 slopeDirection;

    void Start()
    {
        if (slopeStart != null && slopeEnd != null)
        {
            slopeDirection = (slopeEnd.position - slopeStart.position).normalized;
            slopeLength = Vector3.Distance(slopeStart.position, slopeEnd.position);
        }
        
        PreSpawnDoors();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveDoors();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartChain();
        }
    }

    void PreSpawnDoors()
    {
        if (doorFramePrefab == null || questionBank == null || questionBank.questions.Count == 0) return;
        
        doorPairs.Clear();
        
        // Calculate how many doors we need to cover the slope
        int doorCount = Mathf.CeilToInt(slopeLength / doorSpacing) + 5; // Extra doors for continuous chain
        
        for (int i = 0; i < doorCount; i++)
        {
            // Position doors starting from below the slope and extending up
            float distance = -10f + (i * doorSpacing); // Start below slope
            Vector3 spawnPosition = slopeStart.position + (slopeDirection * distance);
            
            SpawnDoorPair(spawnPosition, i);
        }
    }

    void SpawnDoorPair(Vector3 position, int index)
    {
        if (questionBank.questions.Count == 0) return;

        // Get question (cycle through questions)
        QuestionData question = questionBank.questions[currentQuestionIndex];
        currentQuestionIndex = (currentQuestionIndex + 1) % questionBank.questions.Count;

        // Create door frame
        GameObject doorFrameObj = Instantiate(doorFramePrefab, position, Quaternion.identity);
        doorFrameObj.transform.parent = transform;

        DoorPair doorPair = new DoorPair();
        doorPair.doorFrame = doorFrameObj;
        doorPair.position = position;
        doorPair.question = question;
        doorPair.index = index;
        doorPair.hasBeenAnswered = false; // Track if door has been answered

        // Find left and right door children
        Door leftDoor = null;
        Door rightDoor = null;
        
        foreach (Transform child in doorFrameObj.transform)
        {
            if (child.name.ToLower().Contains("left"))
            {
                leftDoor = child.GetComponent<Door>();
                if (leftDoor == null) leftDoor = child.gameObject.AddComponent<Door>();
                leftDoor.doorPair = doorPair; // Link back to door pair
            }
            else if (child.name.ToLower().Contains("right"))
            {
                rightDoor = child.GetComponent<Door>();
                if (rightDoor == null) rightDoor = child.gameObject.AddComponent<Door>();
                rightDoor.doorPair = doorPair; // Link back to door pair
            }
        }

        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("Could not find left and right doors in door frame prefab!");
            return;
        }

        // Assign correct door based on question's correctAnswer flag
        bool leftIsCorrect = question.correctAnswer;

        // Setup doors
        leftDoor.isCorrect = leftIsCorrect;
        leftDoor.isLeftDoor = true;
        rightDoor.isCorrect = !leftIsCorrect;
        rightDoor.isLeftDoor = false;

        doorPair.leftDoor = leftDoor;
        doorPair.rightDoor = rightDoor;
        doorPairs.Add(doorPair);
        
        // Initially align door to slope
        AlignDoorToSlope(doorPair);
    }

    public void StartChain()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            isMoving = true;
            Debug.Log("Starting door chain movement");
        }
    }

    public void StopChain()
    {
        isMoving = false;
        isSpawning = false;
    }

    void MoveDoors()
    {
        // Move all doors UP the slope (in the direction from start to end)
        for (int i = 0; i < doorPairs.Count; i++)
        {
            DoorPair doorPair = doorPairs[i];
            if (doorPair.doorFrame != null && !doorPair.hasBeenAnswered)
            {
                // Move door UP the slope (positive direction)
                doorPair.position += slopeDirection * movementSpeed * Time.deltaTime;
                doorPair.doorFrame.transform.position = doorPair.position;
                
                // Align door to slope at new position
                AlignDoorToSlope(doorPair);
                
                // If door has moved past the TOP of slope, reset it to the BOTTOM
                float distanceFromStart = Vector3.Distance(doorPair.position, slopeStart.position);
                if (distanceFromStart > slopeLength + 20f) // Past the top
                {
                    ResetDoorToBottom(doorPair);
                }
            }
        }
    }

    void AlignDoorToSlope(DoorPair doorPair)
    {
        // Raycast down to find the slope normal at this position
        RaycastHit hit;
        Vector3 raycastStart = doorPair.position + Vector3.up * 2f; // Start raycast above the door
        
        if (Physics.Raycast(raycastStart, Vector3.down, out hit, raycastDistance))
        {
            // Rotate door to match the slope normal
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            doorPair.doorFrame.transform.rotation = targetRotation;
        }
        else
        {
            // If no hit, keep doors upright (fallback)
            doorPair.doorFrame.transform.rotation = Quaternion.identity;
        }
    }

    void ResetDoorToBottom(DoorPair doorPair)
    {
        // Move door to BELOW the slope start (so it can move up again)
        doorPair.position = slopeStart.position - (slopeDirection * 10f);
        doorPair.doorFrame.transform.position = doorPair.position;
        
        // Align to slope at new position
        AlignDoorToSlope(doorPair);
        
        // Assign new question
        if (questionBank.questions.Count > 0)
        {
            QuestionData question = questionBank.questions[currentQuestionIndex];
            doorPair.question = question;
            currentQuestionIndex = (currentQuestionIndex + 1) % questionBank.questions.Count;
            
            // Update door correctness
            bool leftIsCorrect = question.correctAnswer;
            doorPair.leftDoor.isCorrect = leftIsCorrect;
            doorPair.rightDoor.isCorrect = !leftIsCorrect;
            
            // Reset answered state
            doorPair.hasBeenAnswered = false;
        }
    }

    // Called by Door when player answers
    public void OnDoorAnswered(DoorPair doorPair)
    {
        if (doorPair != null && !doorPair.hasBeenAnswered)
        {
            doorPair.hasBeenAnswered = true;
            
            // Hide the door frame
            if (doorPair.doorFrame != null)
            {
                doorPair.doorFrame.SetActive(false);
            }
            
            // Show next question UI if available
            if (QuestionUI.Instance != null)
            {
                QuestionUI.Instance.ShowDoorManagerQuestion();
            }
        }
    }

    // Visualize slope in editor
    void OnDrawGizmos()
    {
        if (slopeStart != null && slopeEnd != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(slopeStart.position, slopeEnd.position);
            Gizmos.DrawSphere(slopeStart.position, 0.5f);
            Gizmos.DrawSphere(slopeEnd.position, 0.5f);
            
            // Show movement direction (should point UP)
            Gizmos.color = Color.green;
            Vector3 midPoint = (slopeStart.position + slopeEnd.position) * 0.5f;
            Gizmos.DrawLine(midPoint, midPoint + slopeDirection * 2f);
        }
    }

    [System.Serializable]
    public class DoorPair
    {
        public GameObject doorFrame;
        public Vector3 position;
        public Door leftDoor;
        public Door rightDoor;
        public QuestionData question;
        public int index;
        public bool hasBeenAnswered = false;
    }
}