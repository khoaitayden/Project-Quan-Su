using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public Transform spawnPoint;
    public float timeLimit = 30f;

    [Header("Associated Spawners")]
    public List<BallSpawner> associatedSpawners = new List<BallSpawner>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CheckpointManager.Instance != null)
            {
                CheckpointManager.Instance.SetCheckpoint(this);
            }
        }
    }

    private void OnValidate()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
    }
}