using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject door;
    public GameObject end;
    public float moveSpeed;
    private bool doorOpen = true;
    private bool doorClosing;
    private Vector3 startPosition;

    void Start()
    {
        // Stores the starting position for MoveDoorDown();
        startPosition = door.transform.position;
    }
    
    void Update()
    {
        // If the door isn't open, it will move up. If it is closing, the door will move back to its starting position.
        if (!doorOpen)
        {
            MoveDoorUp();
        }
        
        if (doorClosing)
        {
            MoveDoorDown();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // When the player enters the collider, the door will start moving up.
        if (other.CompareTag("Player"))
        {
            doorOpen = false;
            doorClosing = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the player exits the collider, the door will start moving back down.
        if (other.CompareTag("Player"))
        {
            doorClosing = true;
            doorOpen = true;
        }
    }

    private void MoveDoorUp()
    {
        // Moves the door up and adjusts the doorOpen boolean after.
        door.transform.position = Vector3.MoveTowards(door.transform.position, end.transform.position, 
            moveSpeed * Time.deltaTime);

        if (door.transform.position == end.transform.position)
        {
            doorOpen = true;
        }
    }

    private void MoveDoorDown()
    {
        // Moves the door up and adjusts the doorClosing boolean after.
        door.transform.position = Vector3.MoveTowards(door.transform.position, startPosition, 
            moveSpeed * Time.deltaTime);

        if (door.transform.position == startPosition)
        {
            doorClosing = false;
        }
    }
}
