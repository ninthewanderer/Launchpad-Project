using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PuzzleButton2 : MonoBehaviour
{
    public GameObject[] affectedObjects;
    public Material magneticMaterial;
    public Material emissiveRed;
    public Material nonemissiveRed;
    public GameObject ConnectedButton;
    public bool timeLimit = false;
    public float timeLimitDuration = 5f;
    // public CanvasGroup buttonCanvas;
    public float platformWaitTime = 2f;
    private bool playerInRange;
    private bool objectsEnabled;
    private bool isPressed;

    public static event Action<bool> OnButtonPressed;

    void Start()
    {
        // if (buttonCanvas.isActiveAndEnabled)
        // {
        //     buttonCanvas.gameObject.SetActive(false);
        // }
    }

    void Update()
    {
        // To press the button, you either press F on keyboard or X on controller.
        if (playerInRange && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick1Button2)))
        {
            if (!objectsEnabled)
            {
                OnButtonPressed?.Invoke(true);
                foreach (GameObject obj in affectedObjects)
                {
                    if (obj.CompareTag("Magnetic Off"))
                    {
                        if (ConnectedButton != null)
                        {
                            if (timeLimit == true)
                            {
                                isPressed = true;
                                if (ConnectedButton.GetComponent<PuzzleButton2>().isPressed == true)
                                {
                                    EnableMagneticPlatform(obj);
                                    objectsEnabled = true;
                                }
                                else
                                {
                                    Debug.Log("Connected button is not pressed, cannot enable magnetic platform");
                                    isPressed = true;
                                }
                                StartCoroutine(TurnOff());
                                return;
                            }
                            if (ConnectedButton.GetComponent<PuzzleButton2>().isPressed == true)
                            {
                                EnableMagneticPlatform(obj);
                                objectsEnabled = true;
                            } else
                            {
                                Debug.Log("Connected button is not pressed, cannot enable magnetic platform");
                                isPressed = true;
                                return;
                            }
                        }
                        else
                        {
                            EnableMagneticPlatform(obj);
                            objectsEnabled = true;
                        }
                    }
                    else if (obj.CompareTag("Door Off"))
                    {
                        EnableDoor(obj);
                        objectsEnabled = true;
                    }
                    else if (obj.CompareTag("Moving Off"))
                    {
                        if (!objectsEnabled)
                        {
                            EnableMovingPlatform(obj);
                            Debug.Log("Enabled moving platform");
                        }
                    }
                }
            }
            foreach (Transform child in transform)
            {
                Debug.Log("Swapping material for child: " + child.name);
                Material[] materials = child.GetComponent<Renderer>().materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].name.Contains("red"))
                    {
                        int indexToSwitch = i;
                        materials[indexToSwitch] = nonemissiveRed;
                    }
                }
                child.GetComponent<Renderer>().materials = materials;
            }
        }
    }

    IEnumerator TurnOff()
    {
        yield return new WaitForSeconds(timeLimitDuration);
        isPressed = false;
        Debug.Log("Time limit expired, button can be pressed again.");
            foreach (Transform child in transform)
                {
                    Material[] materials = child.GetComponent<Renderer>().materials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i].name.Contains("red"))
                        {
                            int indexToSwitch = i;
                            materials[indexToSwitch] = emissiveRed;
                        }
                    }
                    child.GetComponent<Renderer>().materials = materials;
        }
    }
    // "Enables" magnetic platforms by setting them to the right layer.
    private void EnableMagneticPlatform(GameObject affectedObject)
    {
        int magneticLayerInt = LayerMask.NameToLayer("Magnetic");
        affectedObject.layer = magneticLayerInt;

        if (affectedObject.transform.childCount > 0)
        {
            foreach (Transform child in affectedObject.transform)
            {
                child.gameObject.layer = magneticLayerInt;
                Material[] materials = child.GetComponent<Renderer>().materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].name.Contains("m_MagTileOFF"))
                    {
                        int indexToSwitch = i;
                        materials[indexToSwitch] = magneticMaterial;
                    }
                }
                child.GetComponent<Renderer>().materials = materials;
                Debug.Log("Material swapped succesfully!");
            }

        }
    }

    // Enables door triggers by enabling the object itself.
    private void EnableDoor(GameObject affectedObject)
    {
        affectedObject.SetActive(true);
    }
    
    // Moves platforms back and forth, but the player has to keep pressing to repeat the behavior.
    private void EnableMovingPlatform(GameObject affectedObject)
    {
        affectedObject.GetComponent<PlatformRL>().enabled = true;
        Debug.Log("Enabled RL platform");
    }

    private IEnumerator MovePlatformBackForth(GameObject affectedObject)
    {
        objectsEnabled = true;
        if (affectedObject.GetComponent<PlatformFB>() != null)
        {
            Debug.Log("Found FB platform");
            PlatformFB scriptToggle = affectedObject.GetComponent<PlatformFB>();
            scriptToggle.StartCoroutine(scriptToggle.MovePlatform());
            yield return new WaitUntil(() => !(scriptToggle.isMoving));
            yield return new WaitForSeconds(platformWaitTime);

            scriptToggle.StartCoroutine(scriptToggle.MovePlatformBack());
            yield return new WaitUntil(() => !(scriptToggle.isMoving));

        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // buttonCanvas.gameObject.SetActive(true);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // buttonCanvas.gameObject.SetActive(false);
        }
    }
}
