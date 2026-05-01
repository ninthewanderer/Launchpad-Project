using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private static bool HasPlayedBefore;
    public void StartGame()
    {
        if (HasPlayedBefore)
        {
            SaveAndLoadNewScene("LvlHub");
        }
        else
        {
            SaveAndLoadNewScene("GameStartCutScene");
            HasPlayedBefore = true;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadHub()
    {
        SaveAndLoadNewScene("LvlHub");
    }

    public void SaveAndLoadNewScene(string sceneToLoad)
    {
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadLastScene()
    {
        UnityEngine.Debug.Log(PlayerPrefs.GetString("PreviousScene")); 
        string prevScene = PlayerPrefs.GetString("PreviousScene", "LvlHub");
        UnityEngine.Debug.Log(prevScene);
        SceneManager.LoadScene(prevScene);
    }
}
