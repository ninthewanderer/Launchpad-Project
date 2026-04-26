using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerInput : MonoBehaviour
{
    public Button defaultButton;

    void OnEnable()
    {
        defaultButton.Select();    
    }
    
    // void Update()
    // {
    //     if (gameObject.activeInHierarchy)
    //     {
    //         if (Input.GetKeyDown(KeyCode.Joystick1Button1))
    //         {
    //             defaultButton.onClick.Invoke();
    //         }
    //     }
    // }
}
