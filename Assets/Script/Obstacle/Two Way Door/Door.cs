using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isCorrect = false;
    public bool isLeftDoor = false;
    public DoorManager.DoorPair doorPair; // Reference to parent door pair

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isCorrect)
            {
                Debug.Log("Correct door chosen - passing through! Door: " + (isLeftDoor ? "Left" : "Right"));
                // Correct door - player passes through, hide door
                if (doorPair != null)
                {
                    DoorManager doorManager = FindObjectOfType<DoorManager>();
                    if (doorManager != null)
                    {
                        doorManager.OnDoorAnswered(doorPair);
                    }
                }
            }
            else
            {
                Debug.Log("Wrong door chosen - respawning! Door: " + (isLeftDoor ? "Left" : "Right"));
                if (CheckpointManager.Instance != null)
                {
                    CheckpointManager.Instance.OnPlayerTrapTrigger();
                }
            }
        }
    }
}