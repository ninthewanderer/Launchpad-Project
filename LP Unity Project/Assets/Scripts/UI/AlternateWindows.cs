using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateWindows : MonoBehaviour
{
    [SerializeField] private CanvasGroup bootSwapMenu;
    public CanvasGroup pauseMenu;
    public OpenCloseWindow keyboardWindowScript;
    public OpenCloseWindow controllerWindowScript;
    public float timeToAlternate;
    public bool oneTimePopup = true;
    private bool gamePaused;

    private Coroutine windowSwitchingCoroutine;
    private CurrentWindow currentWindow = CurrentWindow.None;
    private enum CurrentWindow
    {
        None,
        Keyboard,
        Controller
    };
    
    void Update()
    {
        CheckForPauseMenu();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keyboardWindowScript.OpenWindow();
            controllerWindowScript.OpenWindow();
            windowSwitchingCoroutine = StartCoroutine(AlternatePopUps());
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (windowSwitchingCoroutine != null) StopCoroutine(windowSwitchingCoroutine);
        if (other.CompareTag("Player"))
        {
            if (keyboardWindowScript.gameObject.activeSelf) keyboardWindowScript.CloseWindow();
            if (controllerWindowScript.gameObject.activeSelf) controllerWindowScript.CloseWindow();
            
            if (oneTimePopup)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator AlternatePopUps()
    {
        while (true)
        {
            for (int i = 0; i < pauseMenu.transform.childCount; i++)
            {
                if (pauseMenu.transform.GetChild(i).gameObject.activeSelf)
                {
                    keyboardWindowScript.gameObject.SetActive(false);
                    controllerWindowScript.gameObject.SetActive(false);
                    yield return null;
                }
            }
            
            if (bootSwapMenu != null && bootSwapMenu.gameObject.activeSelf)
            {
                keyboardWindowScript.gameObject.SetActive(false);
                controllerWindowScript.gameObject.SetActive(false);
            }
            
            yield return new WaitForSeconds(timeToAlternate);
            keyboardWindowScript.gameObject.SetActive(false);
            controllerWindowScript.gameObject.SetActive(true);
            currentWindow = CurrentWindow.Controller;
            
            yield return new WaitForSeconds(timeToAlternate);
            keyboardWindowScript.gameObject.SetActive(true);
            controllerWindowScript.gameObject.SetActive(false);
            currentWindow = CurrentWindow.Keyboard;
            
            yield return null;
        }
    }

    private void CheckForPauseMenu()
    {
        // If the pause menu or boot swap menu is open, the pop-ups will close automatically so that they aren't overlapping.
        if (bootSwapMenu != null && bootSwapMenu.gameObject.activeSelf)
        {
            if (keyboardWindowScript.gameObject.activeSelf) keyboardWindowScript.gameObject.SetActive(false);
            if (controllerWindowScript.gameObject.activeSelf) controllerWindowScript.gameObject.SetActive(false);
            gamePaused = true;
            return;
        }
        
        for (int i = 0; i < pauseMenu.transform.childCount; i++)
        {
            if (pauseMenu.transform.GetChild(i).gameObject.activeSelf)
            {
                if (keyboardWindowScript.gameObject.activeSelf) keyboardWindowScript.gameObject.SetActive(false);
                if (controllerWindowScript.gameObject.activeSelf) controllerWindowScript.gameObject.SetActive(false);
                gamePaused = true;
                return;
            }
        }
        
        gamePaused = false;
        if (!gamePaused && windowSwitchingCoroutine != null)
        {
            if (currentWindow == CurrentWindow.Keyboard && !keyboardWindowScript.gameObject.activeSelf)
                keyboardWindowScript.gameObject.SetActive(true);
            else if (currentWindow == CurrentWindow.Controller && !controllerWindowScript.gameObject.activeSelf)
                controllerWindowScript.gameObject.SetActive(true);
        }
    }
}