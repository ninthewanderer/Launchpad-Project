using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootSwapping : MonoBehaviour
{
    public CanvasGroup bootSwapCanvas;
    public BootMovement player;
    public BootMovement.BootType bootToSwitch;
    private bool playerInRange;

    void Start()
    {
        if (bootSwapCanvas.isActiveAndEnabled)
        {
            bootSwapCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // To swap boots, you either press F on keyboard or X on controller.
        if (playerInRange && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick1Button2)))
        {
            player.ChangeBoots(bootToSwitch);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            bootSwapCanvas.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            bootSwapCanvas.gameObject.SetActive(false);
        }
    }
}
