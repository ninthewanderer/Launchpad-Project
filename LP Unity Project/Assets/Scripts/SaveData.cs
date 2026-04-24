using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static SaveData Instance { get; private set; }

    public bool HideAndSeekComplete { get; private set; }
    public bool PlatformingComplete  { get; private set; }
    public bool PuzzleComplete       { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CompleteHideAndSeek() => HideAndSeekComplete = true;
    public void CompletePlatforming()  => PlatformingComplete  = true;
    public void CompletePuzzle()       => PuzzleComplete       = true;
}