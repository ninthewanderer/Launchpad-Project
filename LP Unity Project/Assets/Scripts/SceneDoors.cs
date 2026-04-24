using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoors : MonoBehaviour
{
    [Header("-------- Player --------")]
    public BootMovement player;
    [Space(10)]

    [Header("-------- Optional Window Control --------")]
    [SerializeField] private float windowCloseTime;
    [SerializeField] private OpenCloseWindow bootWindowScript;
    [SerializeField] private CanvasGroup bootWarningCanvas;
    [SerializeField] private OpenCloseWindow levelWindowScript;
    [SerializeField] private CanvasGroup levelWarningCanvas;
    [Space(10)]
    [SerializeField] private OpenCloseWindow openWindowScript;
    [SerializeField] private CanvasGroup openWindowCanvas;
    [Space(10)]

    [Header("-------- Door Type --------")]
    public DoorToUse chosenDoor = DoorToUse.Hub;
    public enum DoorToUse
    {
        Hub,
        Tutorial,
        HideAndSeek,
        Platforming,
        Puzzle
    };

    private bool playerInRange;

    void Update()
    {
        if (playerInRange && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick1Button2)))
        {
            CheckBoots(player.currentBoots);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

// Checks the player's current boots and determines if they can access the chosen door
    private void CheckBoots(BootMovement.BootType boots)
    {
        switch (boots)
        {
            case BootMovement.BootType.DetectionBoots:
                if (chosenDoor == DoorToUse.Hub || chosenDoor == DoorToUse.Tutorial)
                {
                    MoveToScene(chosenDoor);
                    return;
                }

                if (chosenDoor == DoorToUse.HideAndSeek)
                {
                    if (!SaveData.Instance.HideAndSeekComplete)
                    {
                        MoveToScene(chosenDoor);
                        return;
                    }
                    WarnPlayer(2);
                    return;
                }
                WarnPlayer(1);
                return;

            case BootMovement.BootType.RocketBoots:
                if (chosenDoor == DoorToUse.Hub || chosenDoor == DoorToUse.Tutorial)
                {
                    MoveToScene(chosenDoor);
                    return;
                }

                if (chosenDoor == DoorToUse.Platforming)
                {
                    if (!SaveData.Instance.PlatformingComplete)
                    {
                        MoveToScene(chosenDoor);
                        return;
                    }
                    WarnPlayer(2);
                    return;
                }
                WarnPlayer(1);
                return;

            case BootMovement.BootType.MagnetBoots:
                if (chosenDoor == DoorToUse.Hub || chosenDoor == DoorToUse.Tutorial)
                {
                    MoveToScene(chosenDoor);
                    return;
                }

                if (chosenDoor == DoorToUse.Puzzle)
                {
                    if (!SaveData.Instance.PuzzleComplete)
                    {
                        MoveToScene(chosenDoor);
                        return;
                    }
                    WarnPlayer(2);
                    return;
                }
                WarnPlayer(1);
                return;

            default:
                if (chosenDoor == DoorToUse.Hub || chosenDoor == DoorToUse.Tutorial)
                {
                    MoveToScene(chosenDoor);
                    return;
                }
                WarnPlayer(1);
                return;
        }
    }

// Loads the appropriate scene based on the chosen door
    private void MoveToScene(DoorToUse door)
    {
        switch (door)
        {
            case DoorToUse.Hub:
                SceneManager.LoadScene("LvlHub");
                break;

            case DoorToUse.Puzzle:
                SceneManager.LoadScene("LvlPuzzle");
                break;

            case DoorToUse.Platforming:
                SceneManager.LoadScene("LvlPlatforming");
                break;

            case DoorToUse.HideAndSeek:
                SceneManager.LoadScene("LvlHide&Seek");
                break;

            case DoorToUse.Tutorial:
                SceneManager.LoadScene("Tutorial");
                break;

            default:
                Debug.LogError("Unknown door: " + door);
                break;
        }
    }
// Displays a warning to the player if they try to access a door they don't have the boots for or have already completed
    private void WarnPlayer(int warningType)
    {
        if (openWindowScript != null && openWindowCanvas != null)
        {
            openWindowScript.CloseWindow();
        }

        switch (warningType)
        {
            case 1:
                if (bootWindowScript != null)
                {
                    bootWindowScript.OpenWindow();
                    StartCoroutine(WaitToCloseWindow(1));
                }
                break;

            case 2:
                if (levelWindowScript != null)
                {
                    levelWindowScript.OpenWindow();
                    StartCoroutine(WaitToCloseWindow(2));
                }
                break;

            default:
                Debug.LogError("Unknown warning.");
                break;
        }
    }

    // Closes the warning window after a delay
    private IEnumerator WaitToCloseWindow(int window)
    {
        yield return new WaitForSeconds(windowCloseTime);
        switch (window)
        {
            case 1:
                bootWindowScript.CloseWindow();
                break;

            case 2:
                levelWindowScript.CloseWindow();
                break;
        }
    }
}