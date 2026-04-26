using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseScreen : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public Button resumeButton;

    public GameObject pauseScreenUI;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseScreenUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseScreenUI.SetActive(true);
        StartCoroutine(HandleControllerAnimation());
        Time.timeScale = 0f;
        GameIsPaused = true;
        
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    private IEnumerator HandleControllerAnimation()
    {
        yield return null;
        resumeButton.gameObject.SetActive(true);
        resumeButton.Select();
        resumeButton.OnSelect(null);
        // EventSystem.current.SetSelectedGameObject(null);
        // EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        yield return null;
    }
}
