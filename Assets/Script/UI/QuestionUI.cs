using UnityEngine;
using TMPro;

public class QuestionUI : MonoBehaviour
{
    public static QuestionUI Instance;

    [Header("UI Elements")]
    public GameObject uiPanel;
    public TMP_Text questionText;
    public TMP_Text leftOptionText;
    public TMP_Text rightOptionText;
    public TMP_Text instructionText;

    private DoorManager currentDoorManager;
    private TutorialDoor currentTutorialDoor;
    private bool isShowingQuestion = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Only call DontDestroyOnLoad if this is a root object
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (uiPanel != null)
            uiPanel.SetActive(false);
    }

    void Update()
    {
        CheckForNearbyDoors();
    }

    void CheckForNearbyDoors()
    {
        // Check for regular door managers first
        DoorManager[] doorManagers = FindObjectsOfType<DoorManager>();
        DoorManager nearestManager = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var manager in doorManagers)
        {
            float distance = Vector3.Distance(transform.position, manager.transform.position);
            if (distance < nearestDistance && distance < 30f)
            {
                nearestDistance = distance;
                nearestManager = manager;
            }
        }

        // If no regular door manager found, check for tutorial doors
        if (nearestManager == null)
        {
            TutorialDoor[] tutorialDoors = FindObjectsOfType<TutorialDoor>();
            TutorialDoor nearestTutorial = null;
            nearestDistance = Mathf.Infinity;

            foreach (var tutorial in tutorialDoors)
            {
                float distance = Vector3.Distance(transform.position, tutorial.transform.position);
                if (distance < nearestDistance && distance < 30f)
                {
                    nearestDistance = distance;
                    nearestTutorial = tutorial;
                }
            }

            if (nearestTutorial != null && nearestTutorial != currentTutorialDoor)
            {
                currentTutorialDoor = nearestTutorial;
                currentDoorManager = null;
                ShowTutorialQuestion();
            }
            else if (nearestTutorial == null && isShowingQuestion)
            {
                HideQuestion();
            }
        }
        else if (nearestManager != null && nearestManager != currentDoorManager)
        {
            currentDoorManager = nearestManager;
            currentTutorialDoor = null;
            ShowQuestion();
        }
        else if (nearestManager == null && isShowingQuestion)
        {
            HideQuestion();
        }
    }

    public void ShowQuestion()
    {
        if (currentDoorManager == null || uiPanel == null) return;

        // For regular doors, show generic question
        if (questionText != null)
            questionText.text = "Choose the correct door to proceed";

        // Randomly assign options to left/right
        bool option1IsLeft = Random.Range(0, 2) == 0;

        if (leftOptionText != null)
            leftOptionText.text = option1IsLeft ? "Left Door" : "Right Door";

        if (rightOptionText != null)
            rightOptionText.text = option1IsLeft ? "Right Door" : "Left Door";

        if (instructionText != null)
            instructionText.text = "Choose the correct door to continue";

        uiPanel.SetActive(true);
        isShowingQuestion = true;
    }

    public void ShowTutorialQuestion()
    {
        // Tutorial questions are handled by TutorialDoor.ActivateTutorial()
        // This method is called when we want to show that we're near a tutorial door
        if (uiPanel != null && currentTutorialDoor != null)
        {
            // The actual question text is set by TutorialDoor.ActivateTutorial()
            // Just make sure UI is ready
            uiPanel.SetActive(true);
            isShowingQuestion = true;
        }
    }

    public void HideQuestion()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            isShowingQuestion = false;
        }
    }
}