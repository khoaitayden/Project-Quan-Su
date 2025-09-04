using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject doorFramePrefab;
    public List<Checkpoint> startCheckpoints = new List<Checkpoint>();
    public List<Checkpoint> endCheckpoints = new List<Checkpoint>();
    public float spawnInterval = 15f;
    public int maxDoors = 10;
    public QuestionBank questionBank;
    
    private List<GameObject> spawnedDoorFrames = new List<GameObject>();
    private bool isSpawning = false;
    private int currentQuestionIndex = 0;
    private Vector3 slopeStartPosition;

    void Start()
    {
        slopeStartPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            PreSpawnDoors();
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        ClearDoors();
    }

    void PreSpawnDoors()
    {
        spawnedDoorFrames.Clear();
        
        for (int i = 0; i < maxDoors; i++)
        {
            Vector3 spawnPosition = slopeStartPosition + Vector3.forward * (spawnInterval * i);
            SpawnDoorPair(spawnPosition);
        }
    }

    void SpawnDoorPair(Vector3 position)
    {
        if (questionBank == null || questionBank.questions.Count == 0) return;

        // Get next question
        QuestionData question = questionBank.questions[currentQuestionIndex];
        currentQuestionIndex = (currentQuestionIndex + 1) % questionBank.questions.Count;

        // Create door frame
        GameObject doorFrameObj = Instantiate(doorFramePrefab, position, Quaternion.identity);
        doorFrameObj.transform.parent = transform;
        spawnedDoorFrames.Add(doorFrameObj);

        // Find left and right door children
        Door leftDoor = null;
        Door rightDoor = null;
        
        foreach (Transform child in doorFrameObj.transform)
        {
            if (child.name.ToLower().Contains("left"))
            {
                leftDoor = child.GetComponent<Door>();
                if (leftDoor == null) leftDoor = child.gameObject.AddComponent<Door>();
            }
            else if (child.name.ToLower().Contains("right"))
            {
                rightDoor = child.GetComponent<Door>();
                if (rightDoor == null) rightDoor = child.gameObject.AddComponent<Door>();
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
    }

    void ClearDoors()
    {
        foreach (var doorFrame in spawnedDoorFrames)
        {
            if (doorFrame != null)
                Destroy(doorFrame);
        }
        spawnedDoorFrames.Clear();
    }
}