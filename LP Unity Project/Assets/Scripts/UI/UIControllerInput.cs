using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerInput : MonoBehaviour
{
    public Button defaultButton;
    public Button defaultButtonBackup1;
    public Button defaultButtonBackup2;
    public Button returnButton;

    void OnEnable()
    {
        if (defaultButton.IsInteractable()) defaultButton.Select();   
        else if (defaultButtonBackup1 != null && defaultButtonBackup1.IsInteractable()) defaultButtonBackup1.Select();
        else if (defaultButtonBackup2 != null && defaultButtonBackup2.IsInteractable()) defaultButtonBackup2.Select();
    }
    
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (returnButton != null && Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                returnButton.onClick.Invoke();
            }
        }
    }
}
