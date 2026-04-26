using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HiddenCatManager : MonoBehaviour
{
    private HideAndSeekManager hnsManager;
    private GameObject hidingSpot;
    
    void Start()
    {
        hnsManager = FindObjectOfType<HideAndSeekManager>();
        
        // Ensures only the chosen cat's script is active in the scene at any given time.
        try
        {
            hidingSpot = hnsManager.GetHidingSpot();
            if (!gameObject.name.Equals(hidingSpot.name))
            {
                enabled = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SaveData.Instance.CompleteHideAndSeek();
            Debug.Log("The player found the cat!");
            PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("WinScene");
        }
    }
}
