using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 10; //initial time
    public bool timerIsRunning = false;

    public TextMeshProUGUI timeText;
    public static event Action<bool> OnSceneChange;

    private void Start()
    {
        timerIsRunning = true;

    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time's up!");
                timeRemaining = 0;
                timerIsRunning = false;
                PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
                OnSceneChange?.Invoke(true);
                SceneManager.LoadScene("LoseScene");
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
