using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SaveData : MonoBehaviour
{
    public static SaveData Instance { get; private set; }


    public bool HideAndSeekComplete { get; private set; }
    public bool PlatformingComplete  { get; private set; }
    public bool PuzzleComplete       { get; private set; }
    public bool TutorialComplete     { get; private set; }

    public bool IsHideAndSeekComplete() => HideAndSeekComplete;
    public bool IsPlatformingComplete() => PlatformingComplete;
    public bool IsPuzzleComplete() => PuzzleComplete;
    public bool IsTutorialComplete() => TutorialComplete;

    public static event Action<bool> OnLevelComplete;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        OnLevelComplete += CompleteLevel;
    }

    private void OnDisable()
    {
        OnLevelComplete -= CompleteLevel;
    }

    public void CompleteHideAndSeek()
    {
        HideAndSeekComplete = true;
        OnLevelComplete?.Invoke(true);
    }

    public void CompletePlatforming()
    {
        PlatformingComplete = true;
        OnLevelComplete?.Invoke(true);
    }

    public void CompletePuzzle()
    {
        PuzzleComplete = true;
        OnLevelComplete?.Invoke(true);
    }

    public void CompleteTutorial()
    {
        TutorialComplete = true;
        OnLevelComplete?.Invoke(true);
    }

    public void CompleteLevel(bool level)
    {
        if (HideAndSeekComplete && PlatformingComplete && PuzzleComplete && TutorialComplete)
        {
            SceneManager.LoadScene("GameWin");
        } else
        {
            Debug.Log("More levels left to play!");
        }
    }

}