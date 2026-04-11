using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    private Vector3 currentCheckpointPosition;
    private int currentCheckpointIndex = -1;


    private void Start()
    {
        currentCheckpointPosition = transform.position;
    }

    public bool TrySetCheckpoint(int newIndex, Vector3 newPosition)
    {
        
        if (newIndex > currentCheckpointIndex)
        {
            currentCheckpointIndex = newIndex;
            currentCheckpointPosition = newPosition;

            Debug.Log("New checkpoint index: " + currentCheckpointIndex);
            return true;
        }

        return false;
    }

    public void Respawn()
    {
        transform.position = currentCheckpointPosition;
        Debug.Log("Player respawned at checkpoint index: " + currentCheckpointIndex);
    }
}