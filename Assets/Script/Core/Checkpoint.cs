using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public Transform spawnPoint;
    public float timeLimit = 30f;
    public TutorialDoor tutorialDoor; // Assign in inspector
    public bool isTutorialCheckpoint = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CheckpointManager.Instance != null)
            {
                CheckpointManager.Instance.SetCheckpoint(this);
                
                // Activate tutorial door if this is tutorial checkpoint
                if (isTutorialCheckpoint && tutorialDoor != null)
                {
                    tutorialDoor.ActivateTutorial();
                }
            }
        }
    }

    private void OnValidate()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
    }
}