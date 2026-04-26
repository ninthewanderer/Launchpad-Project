using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToggleOn : MonoBehaviour
{
    [System.NonSerialized] public bool completed;
    public Toggle toggle;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            toggle.isOn = true;
            completed = true;
        }
    }
}
