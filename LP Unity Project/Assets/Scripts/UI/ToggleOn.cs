using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleOn : MonoBehaviour
{
    public Toggle toggle;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            toggle.isOn = true;
        }
    }
}
