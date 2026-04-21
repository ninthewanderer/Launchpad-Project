using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointIndicator : MonoBehaviour
{
    public GameObject checkpointCone;
    public float heightOffset;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = checkpointCone.transform.position;
    }
    
    void Update()
    {
        // Moves the cone up and down at a consistent speed.
        checkpointCone.transform.position = new Vector3(startPosition.x, Mathf.Sin(Time.time) * heightOffset + startPosition.y,
            startPosition.z);
    }
    
    public void OnTriggerEnter(Collider other)
    {
        // If the player collides with the indicator, it will destroy itself.
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
