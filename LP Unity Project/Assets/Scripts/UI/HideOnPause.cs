using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPause : MonoBehaviour
{
    public CanvasGroup pauseMenu;
    public GameObject objectToHideOnPause;
    public BootMovement player;
    public bool bootsUI = false;
    
    void Update()
    {
        if (bootsUI)
        {
            switch (player.currentBoots)
            {
                case BootMovement.BootType.RocketBoots:
                case BootMovement.BootType.DetectionBoots:
                    if (pauseMenu.gameObject.activeSelf) objectToHideOnPause.SetActive(false);
                    if (!pauseMenu.gameObject.activeSelf && !objectToHideOnPause.activeSelf) objectToHideOnPause.SetActive(true);
                    break;
            }
        }
        else
        {
            if (pauseMenu.gameObject.activeSelf) objectToHideOnPause.SetActive(false);
            if (!pauseMenu.gameObject.activeSelf && !objectToHideOnPause.activeSelf) objectToHideOnPause.SetActive(true);
        }
    }
}
