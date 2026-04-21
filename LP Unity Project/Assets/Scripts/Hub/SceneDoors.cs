using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoors : MonoBehaviour
{
    public BootMovement player;

    public DoorToUse chosenDoor = DoorToUse.Hub;
    public enum DoorToUse
    {
        Hub,
        Tutorial,
        HideAndSeek,
        Platforming,
        Puzzle
    };
    
    public bool hideAndSeekComplete;
    public bool platformingComplete;
    public bool puzzleComplete;

    private bool canEnterDoor;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canEnterDoor = CheckBoots(player.currentBoots);
        }
    }

    private bool CheckBoots(BootMovement.BootType boots)
    {
        switch (boots)
        {
            case BootMovement.BootType.DetectionBoots:
                if (chosenDoor == DoorToUse.Hub || chosenDoor == DoorToUse.Tutorial)
                {
                    MoveToScene(chosenDoor);
                    return true;
                }
                
                if (chosenDoor == DoorToUse.HideAndSeek)
                {
                    if (!hideAndSeekComplete)
                    {
                        MoveToScene(chosenDoor);
                        return true;
                    }
                    WarnPlayer(2);
                    return false;
                }
                WarnPlayer(1);
                return false;
            
            case BootMovement.BootType.RocketBoots:
                if (chosenDoor == DoorToUse.Hub || chosenDoor == DoorToUse.Tutorial)
                {
                    MoveToScene(chosenDoor);
                    return true;
                }
                
                if (chosenDoor == DoorToUse.Platforming)
                {
                    if (!platformingComplete)
                    {
                        MoveToScene(chosenDoor);
                        return true;
                    }
                    WarnPlayer(2);
                    return false;
                }
                WarnPlayer(1);
                return false;
            
            case BootMovement.BootType.MagnetBoots:
                if (chosenDoor == DoorToUse.Hub || chosenDoor == DoorToUse.Tutorial)
                {
                    MoveToScene(chosenDoor);
                    return true;
                }
                
                if (chosenDoor == DoorToUse.Puzzle)
                {
                    if (!puzzleComplete)
                    {
                        MoveToScene(chosenDoor);
                        return true;
                    }
                    WarnPlayer(2);
                    return false;
                }
                WarnPlayer(1);
                return false;
            
            default:
                if (chosenDoor == DoorToUse.Hub || chosenDoor == DoorToUse.Tutorial)
                {
                    MoveToScene(chosenDoor);
                    return true;
                }
                WarnPlayer(3);
                return false;
        }
    }
    
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

    private void WarnPlayer(int warningType)
    {
        switch (warningType)
        {
            case 1:
                Debug.Log("Wrong boots equipped. Switch boots and try again.");
                break;
            
            case 2:
                Debug.Log("Level has already been completed.");
                break;
            
            case 3:
                Debug.Log("No boots currently equipped. Switch boots and try again.");
                break;
            
            default:
                Debug.LogError("Unknown warning.");
                break;
        }
    }
}
