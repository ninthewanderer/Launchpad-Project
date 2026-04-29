using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cat : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveData.Instance.CompletePlatforming();
            Debug.Log("You Won!");
            PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
        }
    }
}
