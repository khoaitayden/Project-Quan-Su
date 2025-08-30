using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [Header("Input Actions")]
    public PlayerInput playerInput;
    
    // Input Actions
    private InputAction moveAction;
    private InputAction strafeAction;
    private InputAction turnAction;

    // Input Values
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public float StrafeInput { get; private set; } = 0f;
    public float TurnInput { get; private set; } = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        if (playerInput != null)
        {
            // Get input actions from the PlayerInput component
            moveAction = playerInput.actions["Move"];
            strafeAction = playerInput.actions["Strafe"];
            turnAction = playerInput.actions["Turn"];
        }
    }

    private void Update()
    {
        if (playerInput != null)
        {
            // Read input values every frame
            MoveInput = moveAction.ReadValue<Vector2>();
            StrafeInput = strafeAction.ReadValue<float>();
            TurnInput = turnAction.ReadValue<float>();
        }
    }

    // Public methods for other scripts to get input values
    public float GetForwardInput() => -MoveInput.y; // W/S inverted
    public float GetStrafeInput() => StrafeInput;
    public float GetTurnInput() => TurnInput;

    // Alternative method using direct action reading (if needed)
    public Vector2 GetMoveVector()
    {
        return MoveInput;
    }
}