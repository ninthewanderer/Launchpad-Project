using UnityEngine;

public class TaskUI : MonoBehaviour
{
    public GameObject tutorialCheckmark;
    public GameObject level1Checkmark;
    public GameObject level2Checkmark;
    public GameObject level3Checkmark;

    void Start()
    {
        var save = SaveData.Instance;

        tutorialCheckmark.SetActive(save.IsTutorialComplete());
        level1Checkmark.SetActive(save.IsHideAndSeekComplete());
        level2Checkmark.SetActive(save.IsPlatformingComplete());
        level3Checkmark.SetActive(save.IsPuzzleComplete());
    }
}