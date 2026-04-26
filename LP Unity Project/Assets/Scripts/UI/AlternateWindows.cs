using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateWindows : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseMenu;
    [SerializeField] private CanvasGroup bootSwapMenu;
    public OpenCloseWindow keyboardWindowScript;
    public OpenCloseWindow controllerWindowScript;
    public float timeToAlternate;
    public bool oneTimePopup = true;

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
        // If the pause menu or boot swap menu is open, the pop-ups will close automatically so that they aren't overlapping.
        if (pauseMenu.gameObject.activeSelf && (controllerWindowScript.gameObject.activeSelf 
                                                || keyboardWindowScript.gameObject.activeSelf))
        {
            keyboardWindowScript.gameObject.SetActive(false);
            controllerWindowScript.gameObject.SetActive(false);
            if (bootSwapMenu != null && bootSwapMenu.gameObject.activeSelf)
            {
                keyboardWindowScript.gameObject.SetActive(false);
                controllerWindowScript.gameObject.SetActive(false);
            }
        }

        if (!pauseMenu.gameObject.activeSelf && windowSwitchingCoroutine != null)
        {
            if (currentWindow == CurrentWindow.Keyboard && !keyboardWindowScript.gameObject.activeSelf)
                keyboardWindowScript.gameObject.SetActive(true);
            else if (currentWindow == CurrentWindow.Controller && !controllerWindowScript.gameObject.activeSelf)
                controllerWindowScript.gameObject.SetActive(true);
        }
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
        StopAllCoroutines();
        if (other.CompareTag("Player"))
        {
            keyboardWindowScript.CloseWindow();
            controllerWindowScript.CloseWindow();
            
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
            if (pauseMenu.gameObject.activeSelf)
            {
                keyboardWindowScript.gameObject.SetActive(false);
                controllerWindowScript.gameObject.SetActive(false);
                yield return null;
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
}