using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCat : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveData.Instance.CompletePuzzle();
            Debug.Log("You Won!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("WinScene");
        }
    }
}
