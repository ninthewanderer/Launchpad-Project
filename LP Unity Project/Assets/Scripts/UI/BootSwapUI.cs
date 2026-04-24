using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootSwapUI : MonoBehaviour
{
    public CanvasGroup crosshairCanvas;
    public CanvasGroup bootSwapCanvas;
    
    public void CloseMenu()
    {
        // Re-enables the crosshair canvas for the player.
        crosshairCanvas.gameObject.SetActive(true);
        
        // Locks the cursor again, hides the boot swapping UI, and sets time back to normal.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        bootSwapCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
