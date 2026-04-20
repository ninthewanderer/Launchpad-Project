using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootSwapping : MonoBehaviour
{
    private bool playerInRange;

    [Header("---------- UI Controls ----------")]
    public CanvasGroup crosshairCanvas;
    public CanvasGroup bootSwapCanvas;
    public Button steamBootsIcon;
    public Button detectionBootsIcon;
    public Button magnetBootsIcon;
    
    public bool lockSteamBoots;
    public bool lockDetectionBoots;
    public bool lockMagnetBoots;
    
    // Gets necessary components and hides the menu if it isn't already hidden.
    void Start()
    {
        // steamBootsIcon.GetComponent<Button>();
        // detectionBootsIcon = gameObject.GetComponent<Button>();
        // magnetBootsIcon = gameObject.GetComponent<Button>();
        
        if (bootSwapCanvas.gameObject.activeInHierarchy)
        {
            bootSwapCanvas.gameObject.SetActive(false);
        }
    }

    // Handles input for opening and closing the boot-swapping menu.
    void Update()
    {
        // To swap boots, you either press F on keyboard or X on controller.
        if (playerInRange && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick1Button2)))
        {
            OpenMenu();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void OpenMenu()
    {
        // Disables the crosshair canvas for the player.
        crosshairCanvas.gameObject.SetActive(false);
        
        // Unlocks the cursor so that the player can select which boots to equip. Time is also paused.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        bootSwapCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;

        // If any particular boots are locked (tutorial purposes mostly) they will not be clickable.
        if (lockSteamBoots)
        {
            steamBootsIcon.interactable = false;
        }
        if (lockDetectionBoots)
        {
            detectionBootsIcon.interactable = false;
        }
        if (lockMagnetBoots)
        {
            magnetBootsIcon.interactable = false;
        }
    }
    
    // The CloseMenu() method has been migrated to the BootSwapUI script.
}
