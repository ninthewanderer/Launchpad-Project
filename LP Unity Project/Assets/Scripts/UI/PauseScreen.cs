using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class PauseScreen : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseScreenUI;

    public static event Action<bool> ChangeScene;


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
        Time.timeScale = 0f;
        GameIsPaused = true;
        
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        ChangeScene?.Invoke(true);
        
        
    }

    public void LoadHub()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LvlHub");
        ChangeScene?.Invoke(true);
    }

    public void LoadLoseScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoseScene");
        ChangeScene?.Invoke(true);
    }
}
