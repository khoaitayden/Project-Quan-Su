using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public Transform spawnPoint;
    public float timeLimit = 30f;
    
    [Header("Tutorial Settings")]
    public TutorialDoor tutorialDoor;
    public bool isTutorialCheckpoint = false;
    
    [Header("Slope Door Settings")]
    public DoorManager doorManager; // Changed from SlopeDoorChain to DoorManager
    public bool isSlopeStartCheckpoint = false;

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
                
                // Start slope door chain if this is slope start checkpoint
                if (isSlopeStartCheckpoint && doorManager != null)
                {
                    doorManager.StartChain();
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