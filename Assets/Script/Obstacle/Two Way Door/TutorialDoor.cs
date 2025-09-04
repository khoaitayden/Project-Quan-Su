using UnityEngine;

public class TutorialDoor : MonoBehaviour
{
    [Header("Tutorial Settings")]
    public QuestionBank tutorialQuestionBank;
    
    private Door leftDoor;
    private Door rightDoor;

    void Start()
    {
        SetupDoors();
    }

    void SetupDoors()
    {
        if (tutorialQuestionBank == null || tutorialQuestionBank.questions.Count == 0) return;

        // Get first question
        QuestionData question = tutorialQuestionBank.questions[0];

        // Find left and right door children in this GameObject's children
        foreach (Transform child in transform)
        {
            if (child.name.ToLower().Contains("left"))
            {
                leftDoor = child.GetComponent<Door>();
                if (leftDoor == null) leftDoor = child.gameObject.AddComponent<Door>();
            }
            else if (child.name.ToLower().Contains("right"))
            {
                rightDoor = child.GetComponent<Door>();
                if (rightDoor == null) rightDoor = child.gameObject.AddComponent<Door>();
            }
        }

        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("Could not find left and right doors!");
            return;
        }

        // Assign correct door based on question's correctAnswer flag
        bool leftIsCorrect = question.correctAnswer;

        // Setup doors
        leftDoor.isCorrect = leftIsCorrect;
        leftDoor.isLeftDoor = true;
        rightDoor.isCorrect = !leftIsCorrect;
        rightDoor.isLeftDoor = false;
        
        Debug.Log("Question: " + question.questionText);
        Debug.Log("Option1: " + question.option1 + " (Correct: " + question.correctAnswer + ")");
        Debug.Log("Option2: " + question.option2 + " (Correct: " + !question.correctAnswer + ")");
        Debug.Log("Left door correct: " + leftIsCorrect + ", Right door correct: " + !leftIsCorrect);
    }

    public void ActivateTutorial()
    {
        // Show UI when player reaches this checkpoint
        if (QuestionUI.Instance != null && tutorialQuestionBank != null && tutorialQuestionBank.questions.Count > 0)
        {
            QuestionData question = tutorialQuestionBank.questions[0];
            
            if (QuestionUI.Instance.questionText != null)
                QuestionUI.Instance.questionText.text = question.questionText;
            
            // Display options correctly based on correctAnswer flag
            if (QuestionUI.Instance.leftOptionText != null)
                QuestionUI.Instance.leftOptionText.text = question.option1;
            
            if (QuestionUI.Instance.rightOptionText != null)
                QuestionUI.Instance.rightOptionText.text = question.option2;
            
            if (QuestionUI.Instance.instructionText != null)
                QuestionUI.Instance.instructionText.text = "Choose the correct door to continue";
            
            if (QuestionUI.Instance.uiPanel != null)
                QuestionUI.Instance.uiPanel.SetActive(true);
        }
    }
}