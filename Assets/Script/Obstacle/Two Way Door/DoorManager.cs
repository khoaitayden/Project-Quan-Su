using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door")]
    public Door door1;
    public Door door2;
    //[Header("Doors Movement")]
    //public Rigidbody DoorsRigidbody;
    void Start()
    {
        //DoorsRigidbody = GetComponent<Rigidbody>();
        int correctDoor = Random.Range(0, 2); // Randomly choose between 0 and 1
        if (correctDoor == 0)
        {
            door1.IsCorrect = true;
            door2.IsCorrect = false;
        }
        else
        {
            door1.IsCorrect = false;
            door2.IsCorrect = true;
        }
    }

    void FixedUpdate()
    {
        //DoorsRigidbody.AddForce(Vector3.forward * 5f);
    }

    
}
