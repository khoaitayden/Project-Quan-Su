using System.Collections.Generic;
using UnityEngine;

// This attribute adds a menu item to create the asset easily
[CreateAssetMenu(fileName = "NewQuestionBank", menuName = "Quiz/Question Bank")]
public class QuestionBank : ScriptableObject
{
    [Tooltip("List of questions for this bank.")]
    public List<QuestionData> questions = new List<QuestionData>();
}