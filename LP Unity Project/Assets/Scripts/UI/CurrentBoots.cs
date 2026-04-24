using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CurrentBoots : MonoBehaviour
{
    public BootMovement player;
    public Image steamBootsIcon;
    public Image detectionBootsIcon;
    public Image magnetBootsIcon;

    public CanvasGroup currentBootsUI;
    public CanvasGroup pauseMenu;
    
    void Update()
    {
        if (pauseMenu.gameObject.activeSelf) currentBootsUI.gameObject.SetActive(false);
        if (!pauseMenu.gameObject.activeSelf && !currentBootsUI.isActiveAndEnabled) currentBootsUI.gameObject.SetActive(true);
        
        switch (player.currentBoots)
        {
            case BootMovement.BootType.RocketBoots:
                steamBootsIcon.color = Color.white;
                detectionBootsIcon.color = Color.black;
                magnetBootsIcon.color = Color.black;
                break;
            
            case BootMovement.BootType.DetectionBoots:
                steamBootsIcon.color = Color.black;
                detectionBootsIcon.color = Color.white;
                magnetBootsIcon.color = Color.black;
                break;
            
            case BootMovement.BootType.MagnetBoots:
                steamBootsIcon.color = Color.black;
                detectionBootsIcon.color = Color.black;
                magnetBootsIcon.color = Color.white;
                break;
            
            default:
                steamBootsIcon.color = Color.black;
                detectionBootsIcon.color = Color.black;
                magnetBootsIcon.color = Color.black;
                break;
        }
    }
}
