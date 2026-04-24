using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveData.Instance.CompletePlatforming();
            Debug.Log("You Won!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("WinScene");
        }
    }
}
