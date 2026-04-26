using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public void CompleteHideAndSeek()
    {
        HideAndSeekComplete = true;
    }

    public void CompletePlatforming()
    {
        PlatformingComplete = true;
    }

    public void CompletePuzzle()
    {
        PuzzleComplete = true;
    }

    public void CompleteTutorial()
    {
        TutorialComplete = true;
    }

}