using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isCorrect = false;
    public bool isLeftDoor = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isCorrect)
            {
                Debug.Log("Correct door chosen - passing through! Door: " + (isLeftDoor ? "Left" : "Right"));
                // Do nothing - let player pass through
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