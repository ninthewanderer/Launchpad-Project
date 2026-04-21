using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;
    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            PlayerCheckpoint player = other.GetComponent<PlayerCheckpoint>();

            if (player != null)
            {
                if (player.TrySetCheckpoint(checkpointIndex, transform.position))
                {
                    isActivated = true;
                    Debug.Log("Checkpoint " + checkpointIndex + " activated");
                }
            }
        }
    }
}