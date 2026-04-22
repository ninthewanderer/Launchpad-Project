using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowTrigger : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseMenu;
    [SerializeField] private CanvasGroup bootSwapMenu;
    public OpenCloseWindow windowScript;
    public bool oneTimePopup = true;
    public bool canToggle = false;
    private bool toggleOn;

    // Until we find a way to pause the player's movement while they're in the boot-swapping menu,
    // the boot-swap logic here won't work.
    void Update()
    {
        // If the window is set to be toggle-able, the player can choose to hide the window at any time.
        if (canToggle)
        {
            // Tab on keyboard, Back button on controller.
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Joystick1Button6))
            {
                windowScript.gameObject.SetActive(!windowScript.gameObject.activeSelf);
                toggleOn = !toggleOn;
            }
        }
        
        // If the pause menu or boot swap menu is open, the pop-up will close automatically so that it isn't overlapping.
        if (pauseMenu.gameObject.activeSelf && windowScript.gameObject.activeSelf)
        {
            windowScript.gameObject.SetActive(false);
            if (bootSwapMenu != null && bootSwapMenu.gameObject.activeSelf)
            {
                windowScript.gameObject.SetActive(false);
            }
        }
        // If the window is currently toggled off, it will not reappear after the pause menu or boot swap menu closes.
        else if (!pauseMenu.gameObject.activeSelf && !windowScript.gameObject.activeSelf && !toggleOn)
        {
            windowScript.gameObject.SetActive(true);
            if (bootSwapMenu != null && !bootSwapMenu.gameObject.activeSelf)
            {
                windowScript.gameObject.SetActive(true);
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            windowScript.OpenWindow();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!canToggle)
            {
                windowScript.CloseWindow();
            }
            
            if (oneTimePopup)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
