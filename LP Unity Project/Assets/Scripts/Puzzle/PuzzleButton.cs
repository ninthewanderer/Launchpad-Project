using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    public GameObject[] affectedObjects;
    // public CanvasGroup buttonCanvas;
    public float platformWaitTime = 2f;
    private bool playerInRange;
    private bool objectsEnabled;

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
                foreach (GameObject obj in affectedObjects)
                {
                    if (obj.CompareTag("Magnetic Off"))
                    {
                        EnableMagneticPlatform(obj);
                        objectsEnabled = true;
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
                        }
                    }
                }
            }
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
        StartCoroutine(MovePlatformBackForth(affectedObject));
    }

    private IEnumerator MovePlatformBackForth(GameObject affectedObject)
    {
        objectsEnabled = true;
        PlatformFB scriptToggle = affectedObject.GetComponent<PlatformFB>();
        
        scriptToggle.StartCoroutine(scriptToggle.MovePlatform());
        yield return new WaitUntil(() => !(scriptToggle.isMoving));
        yield return new WaitForSeconds(platformWaitTime);
        
        scriptToggle.StartCoroutine(scriptToggle.MovePlatformBack());
        yield return new WaitUntil(() => !(scriptToggle.isMoving));
        objectsEnabled = false;
        yield return null;
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