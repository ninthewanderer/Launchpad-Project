using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleCat : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveData.Instance.CompletePuzzle();
            Debug.Log("You Won!");
            SceneManager.LoadScene("WinScene");
        }
    }
}
