using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static SaveData Instance { get; private set; }

    public bool HideAndSeekComplete { get; private set; }
    public bool PlatformingComplete  { get; private set; }
    public bool PuzzleComplete       { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstance()
    {
        GameObject obj = new GameObject("SaveData");
        Instance = obj.AddComponent<SaveData>();
        DontDestroyOnLoad(obj);
    }

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void CompleteHideAndSeek() => HideAndSeekComplete = true;
    public void CompletePlatforming()  => PlatformingComplete  = true;
    public void CompletePuzzle()       => PuzzleComplete       = true;
}