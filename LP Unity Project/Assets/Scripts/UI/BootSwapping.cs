using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootSwapping : MonoBehaviour
{
    private bool playerInRange;

    [Header("---------- UI Controls ----------")]
    public CanvasGroup pauseMenu;
    public CanvasGroup crosshairCanvas;
    public CanvasGroup bootSwapCanvas;
    public Button steamBootsIcon;
    public Button detectionBootsIcon;
    public Button magnetBootsIcon;
    
    public bool lockSteamBoots;
    public bool lockDetectionBoots;
    public bool lockMagnetBoots;
    
    private BootSwapUI bootSwapUI;
    
    // Gets necessary components and hides the menu if it isn't already hidden.
    void Start()
    {
        bootSwapUI = FindObjectOfType<BootSwapUI>();
        
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

        CheckForPauseMenu();
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
        // If any particular boots are locked (tutorial purposes mostly) they will not be clickable.
        if (lockSteamBoots)
        {
            steamBootsIcon.interactable = false;
        }
        else
        {
            steamBootsIcon.interactable = true;
        }

        if (lockDetectionBoots)
        {
            detectionBootsIcon.interactable = false;
        }
        else
        {
            detectionBootsIcon.interactable = true;
        }

        if (lockMagnetBoots)
        {
            magnetBootsIcon.interactable = false;
        }
        else
        {
            magnetBootsIcon.interactable = true;
        }

            // Disables the crosshair canvas for the player.
            crosshairCanvas.gameObject.SetActive(false);
        
        // Unlocks the cursor so that the player can select which boots to equip. Time is also paused.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        bootSwapCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;       
    }
    
    // The CloseMenu() method has been migrated to the BootSwapUI script.

    private void CheckForPauseMenu()
    {
        for (int i = 0; i < pauseMenu.transform.childCount; i++)
        {
            if (pauseMenu.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                    if (bootSwapCanvas.gameObject.activeSelf) bootSwapUI.CloseMenu();
                    return;
            }
        }
    }
}
