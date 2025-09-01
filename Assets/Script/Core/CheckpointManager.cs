using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    // Event for checkpoint changes
    public static System.Action<Checkpoint> OnCheckpointChanged;

    [Header("Player Settings")]
    public GameObject player;
    public string trapTag = "Trap";
    public float fallThreshold = -10f;

    [Header("Checkpoints")]
    public List<Checkpoint> allCheckpoints = new List<Checkpoint>();
    private Checkpoint lastCheckpoint;

    [Header("Spawners")]
    public List<BallSpawner> allSpawners = new List<BallSpawner>(); // All spawners in scene

    private float currentTime;
    private bool isTimerRunning = false;
    private bool isRespawning = false;

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
            SetCheckpoint(allCheckpoints[0]);
        }
    }

    private void Update()
    {
        if (isRespawning) return;

        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                RespawnToLastCheckpoint();
        }

        if (player.transform.position.y < fallThreshold)
            RespawnToLastCheckpoint();
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        Checkpoint previousCheckpoint = lastCheckpoint;
        lastCheckpoint = checkpoint;
        currentTime = checkpoint.timeLimit;
        isTimerRunning = true;

        checkpoint.gameObject.SetActive(false);
        
        // Notify listeners that checkpoint changed
        if (OnCheckpointChanged != null && previousCheckpoint != checkpoint)
        {
            OnCheckpointChanged(checkpoint);
        }
        
        // Handle spawner activation based on checkpoint
        HandleSpawnerActivation(checkpoint);
    }

    private void HandleSpawnerActivation(Checkpoint activeCheckpoint)
    {
        // Stop all spawners first
        foreach (var spawner in allSpawners)
        {
            // Note: Individual spawners now handle their own activation based on events
            // This is kept for backward compatibility
        }
    }

    // Called by TrapDetector when player hits a trap
    public void OnPlayerTrapTrigger()
    {
        RespawnToLastCheckpoint();
    }

    public void RespawnToLastCheckpoint()
    {
        if (lastCheckpoint == null || isRespawning) return;
        StartCoroutine(RespawnCoroutine());
    }

    public void ResetToFirstCheckpoint()
    {
        if (allCheckpoints.Count == 0 || isRespawning) return;
        StartCoroutine(ResetToFirstCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        isRespawning = true;

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        player.transform.position = lastCheckpoint.spawnPoint.position;
        player.transform.rotation = lastCheckpoint.spawnPoint.rotation;

        yield return new WaitForSeconds(1.5f);

        if (playerRb != null)
        {
            playerRb.isKinematic = false;
        }

        currentTime = lastCheckpoint.timeLimit;
        isTimerRunning = true;
        isRespawning = false;
        
        // Notify that we're back at checkpoint (in case spawners need to restart)
        if (OnCheckpointChanged != null)
        {
            OnCheckpointChanged(lastCheckpoint);
        }
    }

    private IEnumerator ResetToFirstCoroutine()
    {
        isRespawning = true;

        foreach (var cp in allCheckpoints)
        {
            cp.gameObject.SetActive(true);
        }

        var firstCheckpoint = allCheckpoints[0];
        lastCheckpoint = firstCheckpoint;

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        player.transform.position = firstCheckpoint.spawnPoint.position;
        player.transform.rotation = firstCheckpoint.spawnPoint.rotation;

        yield return new WaitForSeconds(1.5f);

        if (playerRb != null)
        {
            playerRb.isKinematic = false;
        }

        currentTime = firstCheckpoint.timeLimit;
        isTimerRunning = true;
        isRespawning = false;
        
        // Notify that we're back at first checkpoint
        if (OnCheckpointChanged != null)
        {
            OnCheckpointChanged(firstCheckpoint);
        }
    }

    public float GetCurrentTime() => currentTime;
    public bool IsTimerRunning() => isTimerRunning;
    public bool IsRespawning() => isRespawning;
    public Checkpoint GetCurrentCheckpoint() => lastCheckpoint;
}