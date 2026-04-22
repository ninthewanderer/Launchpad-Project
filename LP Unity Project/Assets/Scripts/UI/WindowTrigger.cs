using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowTrigger : MonoBehaviour
{
    public OpenCloseWindow windowScript;
    public bool oneTimePopup = true;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            windowScript.OpenWindow();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            windowScript.CloseWindow();
            
            if (oneTimePopup)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
