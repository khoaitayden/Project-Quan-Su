using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isCorrect;
    public bool IsCorrect
    {
        get { return isCorrect; }
        set { isCorrect = value; }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCorrect && CheckpointManager.Instance != null)
        CheckpointManager.Instance.OnPlayerTrapTrigger();
    }
}
