using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HiddenCatManager : MonoBehaviour
{
    public HideAndSeekManager hnsManager;
    private GameObject hidingSpot;
    
    void Start()
    {
        // Ensures only the chosen cat's script is active in the scene at any given time.
        hidingSpot = hnsManager.GetHidingSpot();
        if (!gameObject.name.Equals(hidingSpot.name))
        {
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("The player found the cat!");
            SceneManager.LoadScene("WinScene");
        }
    }
}
