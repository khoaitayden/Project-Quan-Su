using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public Transform spawnPoint;
    public float timeLimit = 30f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.Instance.SetCheckpoint(this);
        }
    }

    private void OnValidate()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
    }
}