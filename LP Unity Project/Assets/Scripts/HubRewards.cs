using UnityEngine;

public class HubRewards : MonoBehaviour
{
    [Header("-------- Hide & Seek Rewards --------")]
    [SerializeField] private GameObject hideAndSeekCat;
    [SerializeField] private Renderer hideAndSeekClockPart;
    [SerializeField] private Material hideAndSeekClockMaterial;
    [Space(10)]

    [Header("-------- Platforming Rewards --------")]
    [SerializeField] private GameObject platformingCat;
    [SerializeField] private Renderer platformingClockPart;
    [SerializeField] private Material platformingClockMaterial;
    [Space(10)]

    [Header("-------- Puzzle Rewards --------")]
    [SerializeField] private GameObject puzzleCat;
    [SerializeField] private Renderer puzzleClockPart;
    [SerializeField] private Material puzzleClockMaterial;

    private Material blackMaterial;

    void Start()
    {
        blackMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        blackMaterial.color = Color.black;

        UpdateRewards();
    }

    private void UpdateRewards()
    {
        HandleReward(
            SaveData.Instance.HideAndSeekComplete,
            hideAndSeekCat,
            hideAndSeekClockPart,
            hideAndSeekClockMaterial
        );

        HandleReward(
            SaveData.Instance.PlatformingComplete,
            platformingCat,
            platformingClockPart,
            platformingClockMaterial
        );

        HandleReward(
            SaveData.Instance.PuzzleComplete,
            puzzleCat,
            puzzleClockPart,
            puzzleClockMaterial
        );
    }

    private void HandleReward(bool isComplete, GameObject cat, Renderer clockPart, Material originalMaterial)
    {

        if (cat != null)
        {
            cat.SetActive(isComplete);
        }


        if (clockPart != null)
        {
            clockPart.material = isComplete ? originalMaterial : blackMaterial;
        }
    }
}