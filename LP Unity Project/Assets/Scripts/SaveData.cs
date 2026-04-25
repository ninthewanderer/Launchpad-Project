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

    private Image hideAndSeekCheckmark;
    private Image platformingCheckmark;
    private Image puzzleCheckmark;
    private Image tutorialCheckmark;

    [Header("Task Checkmark Object Names")]
    [SerializeField] private string hideAndSeekCheckmarkName = "HideAndSeekCheckmark";
    [SerializeField] private string platformingCheckmarkName  = "PlatformingCheckmark";
    [SerializeField] private string puzzleCheckmarkName       = "PuzzleCheckmark";
    [SerializeField] private string tutorialCheckmarkName     = "TutorialCheckmark";

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterCheckmarks();
    }

    private void RegisterCheckmarks()
    {
        hideAndSeekCheckmark = FindImageByName(hideAndSeekCheckmarkName);
        platformingCheckmark  = FindImageByName(platformingCheckmarkName);
        puzzleCheckmark       = FindImageByName(puzzleCheckmarkName);
        tutorialCheckmark     = FindImageByName(tutorialCheckmarkName);

        RefreshCheckmarks();
    }

    private Image FindImageByName(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null) return null;
        return obj.GetComponent<Image>();
    }

    public void CompleteHideAndSeek()
    {
        HideAndSeekComplete = true;
        SetCheckmark(hideAndSeekCheckmark, true);
    }

    public void CompletePlatforming()
    {
        PlatformingComplete = true;
        SetCheckmark(platformingCheckmark, true);
    }

    public void CompletePuzzle()
    {
        PuzzleComplete = true;
        SetCheckmark(puzzleCheckmark, true);
    }

    public void CompleteTutorial()
    {
        TutorialComplete = true;
        SetCheckmark(tutorialCheckmark, true);
    }

    private void SetCheckmark(Image checkmark, bool isVisible)
    {
        if (checkmark != null)
            checkmark.enabled = isVisible;
    }

    public void RefreshCheckmarks()
    {
        SetCheckmark(hideAndSeekCheckmark, HideAndSeekComplete);
        SetCheckmark(platformingCheckmark,  PlatformingComplete);
        SetCheckmark(puzzleCheckmark,       PuzzleComplete);
        SetCheckmark(tutorialCheckmark,     TutorialComplete);
    }
}