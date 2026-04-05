using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float rotateSpeed;
    public GameObject player;
    private Lives playerLives;

    void Start()
    {
        playerLives = player.GetComponent<Lives>();
    }
    
    void Update()
    {
        // Rotates the attached object every second at a consistent speed.
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    public void OnTriggerEnter(Collider other)
    {
        // If the player collides with the power-up and is able to take it, it will add a life and destroy the power-up.
        if (other.CompareTag("Player"))
        {
            bool canTake = playerLives.AddLife();
            if (canTake)
            {
                Destroy(gameObject);
            }
        }
    }
}
