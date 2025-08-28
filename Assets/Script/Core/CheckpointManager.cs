using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Player Settings")]
    public GameObject player;
    public string trapTag = "Trap";
    public float fallThreshold = -10f;

    [Header("Checkpoints")]
    public List<Checkpoint> allCheckpoints = new List<Checkpoint>(); // Just references to checkpoint objects
    private Checkpoint lastCheckpoint;

    private float currentTime;
    private bool isTimerRunning = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (allCheckpoints.Count > 0)
        {
            // Start at first checkpoint
            SetCheckpoint(allCheckpoints[0]);
        }
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                RespawnToLastCheckpoint();
        }

        // Fall off map check
        if (player.transform.position.y < fallThreshold)
            RespawnToLastCheckpoint();
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        lastCheckpoint = checkpoint;
        currentTime = checkpoint.timeLimit;
        isTimerRunning = true;

        // Hide the checkpoint visually
        checkpoint.gameObject.SetActive(false);
    }

    public void RespawnToLastCheckpoint()
    {
        if (lastCheckpoint == null) return;

        player.transform.position = lastCheckpoint.spawnPoint.position;
        player.transform.rotation = lastCheckpoint.spawnPoint.rotation;

        currentTime = lastCheckpoint.timeLimit;
        isTimerRunning = true;
    }

    public void ResetToFirstCheckpoint()
    {
        if (allCheckpoints.Count == 0) return;

        // Show all checkpoints again
        foreach (var cp in allCheckpoints)
        {
            cp.gameObject.SetActive(true);
        }

        // Reset to first checkpoint
        var firstCheckpoint = allCheckpoints[0];
        lastCheckpoint = firstCheckpoint;
        player.transform.position = firstCheckpoint.spawnPoint.position;
        player.transform.rotation = firstCheckpoint.spawnPoint.rotation;

        currentTime = firstCheckpoint.timeLimit;
        isTimerRunning = true;
    }

    public float GetCurrentTime() => currentTime;
    public bool IsTimerRunning() => isTimerRunning;
}