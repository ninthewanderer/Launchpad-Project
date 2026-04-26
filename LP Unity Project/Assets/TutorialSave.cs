using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSave : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveData.Instance.CompleteTutorial();
            Debug.Log("You Won!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("LvlHub");
        }
    }
}
