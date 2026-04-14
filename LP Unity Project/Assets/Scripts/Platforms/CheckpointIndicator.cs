using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointIndicator : MonoBehaviour
{
    public GameObject checkpointCone;
    public float heightOffset;
    
    void Update()
    {
        // Moves the cone up and down at a consistent speed.
        checkpointCone.transform.position = new Vector3(checkpointCone.transform.position.x,
            (checkpointCone.transform.position.y + Mathf.Sin(Time.time) * heightOffset),
            checkpointCone.transform.position.z);
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
