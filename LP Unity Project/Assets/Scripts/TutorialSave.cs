using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSave : MonoBehaviour
{
    private ToggleOn[] completedTutorials;
    
    void Start()
    {
        completedTutorials = FindObjectsOfType<ToggleOn>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CheckTutorialCompletion())
            {
                SaveData.Instance.CompleteTutorial();
            }
        }
    }

    private bool CheckTutorialCompletion()
    {
        foreach (ToggleOn tutorialTrigger in completedTutorials)
        {
            if (!tutorialTrigger.completed) return false;
        }
        return true;
    }
}
