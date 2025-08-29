using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Player Settings")]
    public GameObject player;
    public string trapTag = "Trap";
    public float fallThreshold = -10f;
    public float respawnFreezeDuration = 1.5f; // Duration to freeze after respawn

    [Header("Checkpoints")]
    public List<Checkpoint> allCheckpoints = new List<Checkpoint>(); // Just references to checkpoint objects
    private Checkpoint lastCheckpoint;

    private float currentTime;
    private bool isTimerRunning = false;
    private bool isRespawning = false; // Flag to prevent multiple respawns

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
        // Don't update timer or check fall threshold during respawn freeze
        if (isRespawning) return;

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

        // Reset player physics momentum
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true; // Freeze physics
        }

        player.transform.position = lastCheckpoint.spawnPoint.position;
        player.transform.rotation = lastCheckpoint.spawnPoint.rotation;

        // Wait for freeze duration
        yield return new WaitForSeconds(respawnFreezeDuration);

        // Re-enable physics
        if (playerRb != null)
        {
            playerRb.isKinematic = false;
        }

        currentTime = lastCheckpoint.timeLimit;
        isTimerRunning = true;
        isRespawning = false;
    }

    private IEnumerator ResetToFirstCoroutine()
    {
        isRespawning = true;

        // Show all checkpoints again
        foreach (var cp in allCheckpoints)
        {
            cp.gameObject.SetActive(true);
        }

        // Reset to first checkpoint
        var firstCheckpoint = allCheckpoints[0];
        lastCheckpoint = firstCheckpoint;

        // Reset player physics momentum
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true; // Freeze physics
        }

        player.transform.position = firstCheckpoint.spawnPoint.position;
        player.transform.rotation = firstCheckpoint.spawnPoint.rotation;

        // Wait for freeze duration
        yield return new WaitForSeconds(respawnFreezeDuration);

        // Re-enable physics
        if (playerRb != null)
        {
            playerRb.isKinematic = false;
        }

        currentTime = firstCheckpoint.timeLimit;
        isTimerRunning = true;
        isRespawning = false;
    }

    public float GetCurrentTime() => currentTime;
    public bool IsTimerRunning() => isTimerRunning;
    public bool IsRespawning() => isRespawning;
}