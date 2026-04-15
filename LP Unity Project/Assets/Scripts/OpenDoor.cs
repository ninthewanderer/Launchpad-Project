using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject door;
    public Transform endPosition;
    public float smoothTime = 15;
    public  Vector3 velocity = new Vector3(0,0,2);
    private bool doorOpen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(MoveDoor());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorOpen = false;
        }
    }

    private IEnumerator MoveDoor()
    {
        Vector3 endPos = endPosition.position;
        while (!doorOpen)
        {
            door.transform.position = Vector3.SmoothDamp(door.transform.position, endPos, ref velocity, smoothTime);
            yield return new WaitWhile(() => door.transform.position != endPos);
            doorOpen = true;
        }
    }
}
